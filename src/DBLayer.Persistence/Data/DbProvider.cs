﻿using Common.Logging;
using DBLayer.Core.Interface;
using System;
using System.Data.Common;
using System.Reflection;

namespace DBLayer.Persistence.Data
{
    /// <summary>
    /// 通用数据库提供类
    /// </summary>
    public class DbProvider : IDbProvider
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
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
                dbProviderFactory = (DbProviderFactory)Activator.CreateInstance(Type.GetType(this.ProviderName));
#endif
            }
            return dbProviderFactory;
        }
        #endregion
    }
}
