// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiClientFactory.cs" Copyright (c) 2013 Plaza De Armas Ltd>
//   Copyright (c) 2013 Plaza De Armas Ltd
// </copyright>
// <summary>
//   Defines the ApiClientFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CypherTwo.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Transactions;

    internal class ApiClientFactory
    {
        #region Static Fields

        private static readonly IDictionary<string, ISendRestCommandsToNeo> ActiveClients = new ConcurrentDictionary<string, ISendRestCommandsToNeo>();

        private static readonly object Lock = new object();

        #endregion

        #region Fields

        private readonly NeoDataRootResponse dataRoot;

        private readonly IJsonHttpClientWrapper httpClient;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="ApiClientFactory"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        /// <param name="httpClient">
        /// The http client.
        /// </param>
        public ApiClientFactory(NeoDataRootResponse baseUrl, IJsonHttpClientWrapper httpClient)
        {
            this.dataRoot = baseUrl;
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
                this.unitOfWork.CommitAsync().Wait();
                this.OnComplete();
                enlistment.Done();
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
                this.unitOfWork.RollbackAsync().Wait();
                this.OnComplete();
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