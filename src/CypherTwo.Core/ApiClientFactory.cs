namespace CypherTwo.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;

    using Newtonsoft.Json;

    internal class ApiClientFactory : ISendRestCommandsToNeo
    {
        private static readonly Dictionary<string, ISendRestCommandsToNeo> ActiveClients =
            new Dictionary<string, ISendRestCommandsToNeo>();
        private static readonly object Lock = new object();

        private readonly string baseUrl;
        private readonly IJsonHttpClientWrapper httpClient;
        private Dictionary<string, object> serviceRoot;

        public ApiClientFactory(string baseUrl, IJsonHttpClientWrapper httpClient)
        {
            this.baseUrl = baseUrl;
            this.httpClient = httpClient;
        }

        public Task<NeoResponse> SendCommandAsync(string command)
        {
            if (this.serviceRoot == null || !this.serviceRoot.Any())
                throw new InvalidOperationException("you must call connect before anything else cunts!");

            return this.GetApiClient().SendCommandAsync(command);
        }

        public async Task LoadServiceRootAsync()
        {
            var result = await this.httpClient.GetAsync(this.baseUrl);
            this.serviceRoot = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
        }

        private ISendRestCommandsToNeo GetApiClient()
        {
            if (Transaction.Current != null)
            {
                lock (Lock)
                {
                    var key = Transaction.Current.TransactionInformation.LocalIdentifier;
                    ISendRestCommandsToNeo client;
                    if (ActiveClients.ContainsKey(key))
                    {
                        client = ActiveClients[key];
                    }
                    else
                    {
                        client = new TransactionalNeoRestApiClient(this.httpClient, this.serviceRoot);
                        var notifier = new ResourceManager((ICypherUnitOfWork)client);

                        notifier.Complete += (o, e) =>
                            {
                                lock (Lock)
                                {
                                    ActiveClients.Remove(key);
                                }
                            };

                        ActiveClients.Add(key, client);
                        Transaction.Current.EnlistVolatile(notifier, EnlistmentOptions.EnlistDuringPrepareRequired);
                    }

                    return client;
                }
            }

            return new NonTransactionalNeoRestApiClient(this.httpClient, this.serviceRoot);
        }

        private class ResourceManager : IEnlistmentNotification
        {
            private readonly ICypherUnitOfWork unitOfWork;

            internal ResourceManager(ICypherUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            internal event EventHandler Complete;

            public void Commit(Enlistment enlistment)
            {
                this.unitOfWork.CommitAsync().Wait();
                this.OnComplete();
                enlistment.Done();
            }

            public void InDoubt(Enlistment enlistment)
            {
                enlistment.Done();
            }

            public void Prepare(PreparingEnlistment preparingEnlistment)
            {
                var keepAlive = this.unitOfWork.KeepAliveAsync().Result;

                if (keepAlive)
                {
                    preparingEnlistment.Prepared();
                }
                else
                {
                    preparingEnlistment.ForceRollback();
                }
            }

            public void Rollback(Enlistment enlistment)
            {
                this.unitOfWork.RollbackAsync().Wait();
                this.OnComplete();
            }

            private void OnComplete()
            {
                var handler = this.Complete;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }
    }
}