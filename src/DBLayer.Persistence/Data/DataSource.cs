using DBLayer.Core.Interface;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;


namespace DBLayer.Persistence.Data
{
    public abstract class DataSource: IDataSource
    {
        //private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        
        public IConnectionString ConnectionString { get; set; }

        public IDbProvider DbProvider { get; set; }

        public DataSource() { }
        public DataSource(IDbProvider dbProvider, IConnectionString connectionString)
        {
            this.DbProvider = dbProvider;
            this.ConnectionString = connectionString;
        }

        #region 连接命令

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <returns>dbconnection</returns>
        internal DbConnection CreateConnection()
        {
            var dbConn = DbProvider.GetDbProviderFactory().CreateConnection();
            dbConn.ConnectionString = ConnectionString.ConnectionValue;

            return dbConn;
        }
        /// <summary>
        /// 创建事务
        /// </summary>
        /// <returns></returns>
        public DbTransaction GetTransaction()
        {
            var dbConn = CreateConnection();
            dbConn.Open();

            return dbConn.BeginTransaction();
        }


        /// <summary>
        /// 创建命令
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="tran">事务</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>command</returns>
        internal DbCommand CreateCommand(string cmdText, DbTransaction trans=null, CommandType commandType=CommandType.Text, params DbParameter[] paramers)
        {

            //if (_logger.IsDebugEnabled)
            //{
            //    _logger.Debug(cmdText);
            //}

            var dbCmd = DbProvider.GetDbProviderFactory().CreateCommand();
            
            dbCmd.CommandText = cmdText;
            dbCmd.CommandType = commandType;
            if (trans != null)
            {
                dbCmd.Connection = trans.Connection;
                dbCmd.Transaction = trans;
            }
            else {
                dbCmd.Connection = CreateConnection();
                dbCmd.Connection.Open();
            }
            
            if (paramers != null){
                dbCmd.Parameters.AddRange(paramers);
            }

            return dbCmd;
        }
        #endregion

        #region 数据执行方法

        /// <summary>
        /// 创建带参数的只读器
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="tran">事务</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>reader</returns>
        internal DbDataReader CreateDataReader(string cmdText, DbTransaction tran=null, CommandType commandType= CommandType.Text, params DbParameter[] paramers)
        {
            var reader = CreateCommand(cmdText,tran, commandType, paramers)
                .ExecuteReader(tran==null
                ? CommandBehavior.Default
                : CommandBehavior.CloseConnection);

            return reader;
        }


        /// <summary>
        /// 创建带参数的只读器
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="tran">事务</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>reader</returns>
        internal async Task<DbDataReader> CreateDataReaderAsync( string cmdText, DbTransaction tran=null, CommandType commandType=CommandType.Text, params DbParameter[] paramers)
        {
            var reader = await CreateCommand( cmdText, tran, commandType, paramers)
                .ExecuteReaderAsync(tran == null 
                ? CommandBehavior.Default 
                : CommandBehavior.CloseConnection);

            return reader;
        }


        /// <summary>
        /// 执行一个带参数的SQL/存储过程有事务的
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="tran">事务</param>
        /// <param name="commmandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>影响行数</returns>
        public int ExecuteNonQuery(string cmdText, DbTransaction trans=null, CommandType commandType=CommandType.Text, params DbParameter[] paramers)
        {
            int retval = -1;
            using (var dbCmd = CreateCommand( cmdText, trans, commandType, paramers))
            {
                if (trans == null)
                {
                    using (dbCmd.Connection)
                    {
                        retval = dbCmd.ExecuteNonQuery();
                    }
                }
                else {
                    retval = dbCmd.ExecuteNonQuery();
                }
            }
            return retval;
        }

        /// <summary>
        /// 执行一个带参数的SQL/存储过程有事务的
        /// </summary>
        /// 
        /// <param name="cmdText">命令</param>
        /// <param name="tran">事务</param>
        /// <param name="commmandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>影响行数</returns>
        public async Task<int> ExecuteNonQueryAsync( string cmdText, DbTransaction trans=null, CommandType commandType= CommandType.Text, params DbParameter[] paramers)
        {
            var retval = -1;
            using (var dbCmd = CreateCommand( cmdText, trans, commandType, paramers))
            {
                if (trans == null)
                {
                    using (dbCmd.Connection)
                    {
                        retval = await dbCmd.ExecuteNonQueryAsync();
                    }
                }
                else
                {
                    retval = await dbCmd.ExecuteNonQueryAsync();
                }
                
            }
            return retval;
        }

        /// <summary>
        /// 执行一个带参数的SQL/存储过程返回首行首列
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="trans">事务</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>值</returns>
        public object ExecuteScalar( string cmdText, DbTransaction trans=null, CommandType commandType=CommandType.Text, params DbParameter[] paramers)
        {
            object retval = null;
            using (var dbCmd = CreateCommand( cmdText, trans, commandType, paramers))
            {
                if (trans == null)
                {
                    using (dbCmd.Connection)
                    {
                        retval = dbCmd.ExecuteScalar();
                    }
                }
                else
                {
                    retval = dbCmd.ExecuteScalar();
                }
            }
            return retval;
        }

        /// <summary>
        /// 执行一个带参数的SQL/存储过程返回首行首列
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="trans">事务</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>值</returns>
        public async Task<object> ExecuteScalarAsync( string cmdText, DbTransaction trans=null, CommandType commandType=CommandType.Text, params DbParameter[] paramers)
        {
            object retval = null;
            using (var dbCmd = CreateCommand( cmdText, trans, commandType, paramers))
            {
                if (trans == null)
                {
                    using (dbCmd.Connection)
                    {
                        retval = await dbCmd.ExecuteScalarAsync();
                    }
                }
                else
                {
                    retval = await dbCmd.ExecuteScalarAsync();
                }
            }
            return retval;
        }

        /// <summary>
        /// 执行一个语句返回数据集
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="trans">事务</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>数据集</returns>
        public DataSet CreateDataSet( string cmdText, DbTransaction trans=null, CommandType commandType=CommandType.Text, params DbParameter[] paramers)
        {
            var dataSet = new DataSet();
            using (var dbCmd = CreateCommand( cmdText, trans, commandType, paramers))
            {
                if (trans == null)
                {
                    using (dbCmd.Connection)
                    {
                        using (var dbDA = DbProvider.GetDbProviderFactory().CreateDataAdapter())
                        {
                            dbDA.SelectCommand = dbCmd;
                            dbDA.Fill(dataSet);
                        }
                    }
                }
                else {
                    using (var dbDA = DbProvider.GetDbProviderFactory().CreateDataAdapter())
                    {
                        dbDA.SelectCommand = dbCmd;
                        dbDA.Fill(dataSet);
                    }
                }
            }
            return dataSet;
        }

        #endregion

        #region 创建参数方法

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="pName">参数名</param>
        /// <param name="pValue">参数值</param>
        /// <param name="pType">参数类型</param>
        /// <param name="pSize">长度</param>
        /// <returns>参数</returns>
        public DbParameter CreateParameter(string pName, object pValue, DbType? pType=null, int? pSize=null)
        {
            pName = ReplaceParameter(pName);
            var paramer = DbProvider.GetDbProviderFactory().CreateParameter();
            paramer.ParameterName = pName;
            paramer.Value = pValue;
            if (pType != null)
            {
                paramer.DbType = pType.Value;
            }
            if (pSize != null)
            {
                paramer.Size = pSize.Value;
            }

            return paramer;
        }

        #region output
        /// <summary>
        /// 创建一个带有返回值的存储过程参数
        /// </summary>
        /// <param name="pName">参数名称</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="sourceType">映射属性值</param>
        /// <returns></returns>
        public virtual DbParameter CreateOutPutParameter(string pName,string propertyName,object propertyValue)
        {
            pName = ReplaceParameter(pName);
            var paramer = DbProvider.GetDbProviderFactory().CreateParameter();
            paramer.ParameterName = pName;
            paramer.Direction = ParameterDirection.Output;

            paramer.SetValueByPropertyName(propertyName, propertyValue);

            return paramer;
        }

        /// <summary>
        /// 创建一个带有返回值的存储过程参数
        /// </summary>
        /// <param name="pName">参数名称</param>
        /// <param name="pType">参数类型</param>
        /// <param name="pSize">长度</param>
        /// <returns></returns>
        public virtual DbParameter CreateOutPutParameter(string pName, DbType? pType=null, int? pSize=null)
        {
            pName = ReplaceParameter(pName);
            var paramer = DbProvider.GetDbProviderFactory().CreateParameter();
            paramer.ParameterName = pName;
            if (pType == null)
            {
                paramer.DbType = pType.Value;
            }
            if (pSize != null)
            {
                paramer.Size = pSize.Value;
            }
            paramer.Direction = ParameterDirection.Output;
           
            return paramer;
        }

        /// <summary>
        /// 创建一个带有返回值的存储过程参数
        /// </summary>
        /// <param name="pName">参数名称</param>
        /// <param name="pValue">参数值</param>
        /// <param name="pType">参数类型</param>
        /// <param name="pSize">长度</param>
        /// <returns></returns>
        public virtual DbParameter CreateOutPutParameter(string pName, object pValue, DbType? pType=null, int? pSize=null)
        {
            pName = ReplaceParameter(pName);
            var paramer = DbProvider.GetDbProviderFactory().CreateParameter();
            paramer.ParameterName = pName;
            if (pType==null)
            {
                paramer.DbType = pType.Value;
            }
            if (pSize!=null)
            {
                paramer.Size = pSize.Value;
            }
            paramer.Direction = ParameterDirection.Output;
            paramer.Value = pValue;

            return paramer;
        }
        #endregion

        #region 返回参数类型
        /// <summary>
        /// 创建一个带有返回值的存储过程参数
        /// </summary>
        /// <param name="pName">参数名称</param>
        /// <param name="pType">参数类型</param>
        /// <param name="pSize">长度</param>
        /// <returns></returns>
        public virtual DbParameter CreateReturnValueParameter(string pName, DbType? pType = null, int? pSize = null)
        {
            pName = ReplaceParameter(pName);
            var paramer = DbProvider.GetDbProviderFactory().CreateParameter();
            paramer.ParameterName = pName;
            if (pType == null)
            {
                paramer.DbType = pType.Value;
            }
            if (pSize != null)
            {
                paramer.Size = pSize.Value;
            }
            paramer.Direction = ParameterDirection.ReturnValue;

            return paramer;
        }

        /// <summary>
        /// 创建一个带有返回值的存储过程参数
        /// </summary>
        /// <param name="pName">参数名称</param>
        /// <param name="pValue">参数值</param>
        /// <param name="pType">参数类型</param>
        /// <param name="pSize">长度</param>
        /// <returns></returns>
        public virtual DbParameter CreateReturnValueParameter(string pName, object pValue, DbType? pType = null, int? pSize = null)
        {
            pName=ReplaceParameter(pName);
            var paramer = DbProvider.GetDbProviderFactory().CreateParameter();
            paramer.ParameterName = pName;
            if (pType == null)
            {
                paramer.DbType = pType.Value;
            }
            if (pSize != null)
            {
                paramer.Size = pSize.Value;
            }
            paramer.Direction = ParameterDirection.ReturnValue;
            paramer.Value = pValue;

            return paramer;
        }
        #endregion

        #endregion
        #region 过滤sql 支持 #参数
        protected string ReplaceParameter(string cmdText)
        {
            cmdText = cmdText?.Replace("#", this.DbProvider.ParameterPrefix);
            return cmdText;
        }
        #endregion
    }
}
