using DBLayer.Core.Interface;
using System;
using System.Data.Common;

namespace DBLayer.Persistence.Data
{
    /// <summary>
    /// Commmon DbProvider
    /// </summary>
    public class DbProvider : IDbProvider
    {
        private const string SQLPARAMETER = "?";
        #region 接口
        public string ProviderName { get; set; }
        private DbProviderFactory dbProviderFactory { get; set; }
        /// <summary>
        /// Parameter prefix use in store procedure.
        /// </summary>
        /// <example> @ for Sql Server.</example>
        public string ParameterPrefix { get; set; }
        public string SelectKey { get; set; }  
        public DbProviderFactory GetDbProviderFactory()
        {
            if (dbProviderFactory==null)
            {
#if NET461
                dbProviderFactory = DbProviderFactories.GetFactory(this.ProviderName);
#else
                var factoryType = Type.GetType(this.ProviderName);
                dbProviderFactory = (DbProviderFactory)factoryType.GetField("Instance").GetValue(null);
#endif
            }
            return dbProviderFactory;
        }
        #endregion
    }
}
