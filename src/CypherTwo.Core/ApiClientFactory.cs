namespace CypherTwo.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Transactions;

    internal class ApiClientFactory 
    {
        private static readonly IDictionary<string, ISendRestCommandsToNeo> ActiveClients =
            new ConcurrentDictionary<string, ISendRestCommandsToNeo>();
        private static readonly object Lock = new object();

        private readonly NeoDataRootResponse dataRoot;
        private readonly IJsonHttpClientWrapper httpClient;

        public ApiClientFactory(NeoDataRootResponse baseUrl, IJsonHttpClientWrapper httpClient)
        {
            this.dataRoot = baseUrl;
            this.httpClient = httpClient;
        }

        public ISendRestCommandsToNeo GetApiClient()
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
                        client = new TransactionalNeoRestApiClient(this.httpClient, this.dataRoot.Transaction);
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

            return new NonTransactionalNeoRestApiClient(this.httpClient, this.dataRoot.Transaction);
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