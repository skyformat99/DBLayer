namespace DBLayer.Core.Interface
{
    public interface IDataSource
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        IConnectionString ConnectionString { set; get; }

        /// <summary>
        /// Gets or sets the db provider.
        /// </summary>
        /// <value>The db provider.</value>
        IDbProvider DbProvider { set; get; }

    }
}
