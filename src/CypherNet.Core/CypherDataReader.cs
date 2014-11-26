namespace CypherNet.Core
{
    using System;
    using System.Linq;

    /// <summary>
    /// The CypherDataReader interface.
    /// </summary>
    public interface ICypherDataReader
    {
        #region Public Properties

        /// <summary>
        /// Gets the columns.
        /// </summary>
        string[] Columns { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T Get<T>(int index);

        /// <summary>
        /// The read.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Read();

        #endregion
    }

    /// <summary>
    /// The cypher data reader.
    /// </summary>
    public class CypherDataReader : ICypherDataReader
    {
        #region Fields

        private readonly NeoResponse data;

        private int rowPointer = -1;

        #endregion

        #region Constructors and Destructors

        internal CypherDataReader(NeoResponse data)
        {
            this.data = data;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the columns.
        /// </summary>
        public string[] Columns
        {
            get { return this.data.results.First().columns.Select(c => c.ToString()).ToArray(); }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public T Get<T>(int index)
        {
            if (index < 0 || index > this.data.results.First().columns.Length)
            {
                throw new IndexOutOfRangeException("index");
            }

            if (this.data.results.First().data.Length <= this.rowPointer)
            {
                throw new InvalidOperationException("exceed result count");
            }

            return this.data.results.First().data[this.rowPointer].row[index].ToObject<T>();
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Read()
        {
            return this.data.results.First().data.Length > ++this.rowPointer;
        }

        #endregion
    }
}