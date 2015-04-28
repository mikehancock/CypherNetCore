namespace CypherNet.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Transactions;

    internal interface IApiClientFactory
    {
        ISendRestCommandsToNeo GetApiClient();
    }

    internal class ApiClientFactory : IApiClientFactory
    {
        #region Static Fields

        private static readonly IDictionary<string, ISendRestCommandsToNeo> ActiveClients = new ConcurrentDictionary<string, ISendRestCommandsToNeo>();

        #endregion

        #region Fields

        private readonly NeoDataRootResponse dataRoot;

        private readonly IJsonHttpClientWrapper httpClient;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="ApiClientFactory"/> class.
        /// </summary>
        /// <param name="dataRoot">
        /// The base url.
        /// </param>
        /// <param name="httpClient">
        /// The http client.
        /// </param>
        public ApiClientFactory(NeoDataRootResponse dataRoot, IJsonHttpClientWrapper httpClient)
        {
            this.dataRoot = dataRoot;
            this.httpClient = httpClient;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get api client.
        /// </summary>
        /// <returns>
        /// The <see cref="ISendRestCommandsToNeo"/>.
        /// </returns>
        public ISendRestCommandsToNeo GetApiClient()
        {
            ISendRestCommandsToNeo client;
            if (Transaction.Current == null)
            {
                client = new NonTransactionalNeoRestApiClient(this.httpClient, this.dataRoot.Transaction);
            }
            else
            {
                var key = Transaction.Current.TransactionInformation.LocalIdentifier;
                client = (ActiveClients as ConcurrentDictionary<string, ISendRestCommandsToNeo>)
                    .GetOrAdd(key,
                            k =>
                            {
                                var cl = new TransactionalNeoRestApiClient(this.httpClient, this.dataRoot.Transaction);
                                var notifier = new ResourceManager((ICypherUnitOfWork)cl);

                                notifier.Complete += (o, e) =>
                                    {
                                        ActiveClients.Remove(key);
                                    };

                                Transaction.Current.EnlistVolatile(notifier, EnlistmentOptions.EnlistDuringPrepareRequired);
                                return cl;
                            });
            }

            return client;
        }

        #endregion

        private class ResourceManager : IEnlistmentNotification
        {
            #region Fields

            private readonly ICypherUnitOfWork unitOfWork;

            #endregion

            #region Constructors and Destructors

            internal ResourceManager(ICypherUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            #endregion

            #region Events

            internal event EventHandler Complete;

            #endregion

            #region Public Methods and Operators

            /// <summary>
            /// The commit.
            /// </summary>
            /// <param name="enlistment">
            /// The enlistment.
            /// </param>
            public void Commit(Enlistment enlistment)
            {
                this.unitOfWork.CommitAsync()
                    .ContinueWith(t =>
                    {
                        this.OnComplete();
                        enlistment.Done();
                    });
            }

            /// <summary>
            /// The in doubt.
            /// </summary>
            /// <param name="enlistment">
            /// The enlistment.
            /// </param>
            public void InDoubt(Enlistment enlistment)
            {
                enlistment.Done();
            }

            /// <summary>
            /// The prepare.
            /// </summary>
            /// <param name="preparingEnlistment">
            /// The preparing enlistment.
            /// </param>
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

            /// <summary>
            /// The rollback.
            /// </summary>
            /// <param name="enlistment">
            /// The enlistment.
            /// </param>
            public void Rollback(Enlistment enlistment)
            {
                this.unitOfWork.RollbackAsync()
                    .ContinueWith(t => this.OnComplete());
            }

            #endregion

            #region Methods

            private void OnComplete()
            {
                var handler = this.Complete;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }

            #endregion
        }
    }
}