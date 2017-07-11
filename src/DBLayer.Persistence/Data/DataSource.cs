using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using System.Reflection;
using Common.Logging;
using DBLayer.Core;
using DBLayer.Core.Interface;
using DBLayer.Persistence;


namespace DBLayer.Persistence.Data
{
    public class DataSource : IDataSource
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region 接口
        public ConnectionString ConnectionString { get; set; }

        public IDbProvider DbProvider { get; set; }
        #endregion

        #region 连接命令

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <returns>dbconnection</returns>
        internal DbConnection CreateConnection()
        {
            var dbConn = DbProvider.GetDbProviderFactory().CreateConnection();
            dbConn.ConnectionString = ConnectionString.ToString();

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
        /// <param name="commandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>command</returns>
        internal DbCommand CreateCommand(string cmdText, CommandType commandType, params DbParameter[] paramers)
        {
            if (_logger.IsDebugEnabled)
            {
                _logger.Debug(cmdText);
            }
            var dbConn = CreateConnection();
            dbConn.Open();
            var dbCmd = DbProvider.GetDbProviderFactory().CreateCommand();
            dbCmd.Connection = dbConn;
            dbCmd.CommandText = cmdText;
            dbCmd.CommandType = commandType;


            if (paramers != null)
            {
                dbCmd.Parameters.AddRange(paramers);
            }

            return dbCmd;
        }

        /// <summary>
        /// 创建命令
        /// </summary>
        /// <param name="tran">事务</param>
        /// <param name="cmdText">命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>command</returns>
        internal DbCommand CreateCommand(DbTransaction trans, string cmdText, CommandType commandType, params DbParameter[] paramers)
        {
            if (_logger.IsDebugEnabled)
            {
                _logger.Debug(cmdText);
            }

            var dbCmd = DbProvider.GetDbProviderFactory().CreateCommand();
            dbCmd.Connection = trans.Connection;
            dbCmd.CommandText = cmdText;
            dbCmd.CommandType = commandType;
            dbCmd.Transaction = trans;
            if (paramers != null)
            {
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
        /// <param name="commandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>reader</returns>
        internal DbDataReader CreateDataReader(string cmdText, CommandType commandType, params DbParameter[] paramers)
        {
            var reader = CreateCommand(cmdText, commandType, paramers).ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }

        /// <summary>
        /// 创建带参数的只读器
        /// </summary>
        /// <param name="tran">事务</param>
        /// <param name="cmdText">命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>reader</returns>
        internal DbDataReader CreateDataReader(DbTransaction tran, string cmdText, CommandType commandType, params DbParameter[] paramers)
        {
            var reader = CreateCommand(tran, cmdText, commandType, paramers).ExecuteReader();

            return reader;
        }

        /// <summary>
        /// 创建带参数的只读器
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>reader</returns>
        internal Task<DbDataReader> CreateDataReaderAsync(string cmdText, CommandType commandType, params DbParameter[] paramers)
        {
            var reader = CreateCommand(cmdText, commandType, paramers).ExecuteReaderAsync(CommandBehavior.CloseConnection);

            return reader;
        }

        /// <summary>
        /// 创建带参数的只读器
        /// </summary>
        /// <param name="tran">事务</param>
        /// <param name="cmdText">命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>reader</returns>
        internal Task<DbDataReader> CreateDataReaderAsync(DbTransaction tran, string cmdText, CommandType commandType, params DbParameter[] paramers)
        {
            var reader = CreateCommand(tran, cmdText, commandType, paramers).ExecuteReaderAsync();

            return reader;
        }


        /// <summary>
        /// 执行一个带参数的SQL/存储过程
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>影响行数</returns>
        public int ExecuteNonQuery(string cmdText, CommandType commandType, params DbParameter[] paramers)
        {
            int retval = -1;

            using (var cmd = CreateCommand(cmdText, commandType, paramers))
            {
                using (cmd.Connection)
                {
                    try
                    {
                        retval = cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        if (cmd.Connection.State == ConnectionState.Open) { cmd.Connection.Close(); }
                        _logger.Error(e.Message, e);
                        throw;
                    }
                }
            }

            return retval;
        }

        /// <summary>
        /// 执行一个带参数的SQL/存储过程有事务的
        /// </summary>
        /// <param name="tran">事务</param>
        /// <param name="cmdText">命令</param>
        /// <param name="commmandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>影响行数</returns>
        public int ExecuteNonQuery(DbTransaction trans, string cmdText, CommandType commandType, params DbParameter[] paramers)
        {
            int retval = -1;
            using (var dbCmd = CreateCommand(trans, cmdText, commandType, paramers))
            {
                retval = dbCmd.ExecuteNonQuery();
            }
            return retval;
        }

        /// <summary>
        /// 执行一个带参数的SQL/存储过程
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>影响行数</returns>
        public Task<int> ExecuteNonQueryAsync(string cmdText, CommandType commandType, params DbParameter[] paramers)
        {
            var retval = new Task<int>(() => -1);

            using (var cmd = CreateCommand(cmdText, commandType, paramers))
            {
                using (cmd.Connection)
                {
                    try
                    {
                        retval = cmd.ExecuteNonQueryAsync();
                    }
                    catch (Exception)
                    {
                        if (cmd.Connection.State == ConnectionState.Open) { cmd.Connection.Close(); }
                        throw;
                    }
                }
            }

            return retval;
        }

        /// <summary>
        /// 执行一个带参数的SQL/存储过程有事务的
        /// </summary>
        /// <param name="tran">事务</param>
        /// <param name="cmdText">命令</param>
        /// <param name="commmandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>影响行数</returns>
        public Task<int> ExecuteNonQueryAsync(DbTransaction trans, string cmdText, CommandType commandType, params DbParameter[] paramers)
        {
            Task<int> retval = new Task<int>(() => -1);
            using (var dbCmd = CreateCommand(trans, cmdText, commandType, paramers))
            {
                retval = dbCmd.ExecuteNonQueryAsync();
            }
            return retval;
        }

        /// <summary>
        /// 执行一个带参数的SQL/存储过程返回首行首列
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>值</returns>
        public object ExecuteScalar(string cmdText, CommandType commandType, params DbParameter[] paramers)
        {
            object retval = null;
            using (var dbCmd = CreateCommand(cmdText, commandType, paramers))
            {
                using (dbCmd.Connection)
                {
                    try
                    {
                        retval = dbCmd.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        if (dbCmd.Connection.State == ConnectionState.Open) { dbCmd.Connection.Close(); }
                        _logger.Error(ex.Message, ex);
                        throw;
                    }

                }
            }

            return retval;
        }


        /// <summary>
        /// 执行一个带参数的SQL/存储过程返回首行首列
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>值</returns>
        public object ExecuteScalar(DbTransaction trans, string cmdText, CommandType commandType, params DbParameter[] paramers)
        {
            object retval = null;
            using (var dbCmd = CreateCommand(trans, cmdText, commandType, paramers))
            {
                retval = dbCmd.ExecuteScalar();
            }
            return retval;
        }

        /// <summary>
        /// 执行一个带参数的SQL/存储过程返回首行首列
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>值</returns>
        public Task<object> ExecuteScalarAsync(string cmdText, CommandType commandType, params DbParameter[] paramers)
        {
            Task<object> retval = new Task<object>(() => null);
            using (var dbCmd = CreateCommand(cmdText, commandType, paramers))
            {
                using (dbCmd.Connection)
                {
                    try
                    {
                        retval = dbCmd.ExecuteScalarAsync();
                    }
                    catch (Exception ex)
                    {
                        if (dbCmd.Connection.State == ConnectionState.Open) { dbCmd.Connection.Close(); }
                        _logger.Error(ex.Message, ex);
                        throw;
                    }

                }
            }

            return retval;
        }


        /// <summary>
        /// 执行一个带参数的SQL/存储过程返回首行首列
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>值</returns>
        public Task<object> ExecuteScalarAsync(DbTransaction trans, string cmdText, CommandType commandType, params DbParameter[] paramers)
        {
            Task<object> retval = new Task<object>(() => null);
            using (var dbCmd = CreateCommand(trans, cmdText, commandType, paramers))
            {
                retval = dbCmd.ExecuteScalarAsync();
            }
            return retval;
        }

        /// <summary>
        /// 执行一个语句返回数据集
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>数据集</returns>
        public DataSet CreateDataSet(string cmdText, CommandType commandType, params DbParameter[] paramers)
        {
            var dataSet = new DataSet();
            using (var dbCmd = CreateCommand(cmdText, commandType, paramers))
            {
                using (dbCmd.Connection)
                {
                    using (var dbDA = DbProvider.GetDbProviderFactory().CreateDataAdapter())
                    {
                        try
                        {
                            dbDA.SelectCommand = dbCmd;
                            dbDA.Fill(dataSet);
                        }
                        catch (Exception ex)
                        {
                            if (dbCmd.Connection.State == ConnectionState.Open) { dbCmd.Connection.Close(); }
                            _logger.Error(ex.Message, ex);
                            throw;
                        }

                    }

                }
            }
            return dataSet;
        }

        /// <summary>
        /// 执行一个语句返回数据集
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>数据集</returns>
        public DataSet CreateDataSet(DbTransaction trans, string cmdText, CommandType commandType, params DbParameter[] paramers)
        {
            var dataSet = new DataSet();
            using (var dbCmd = CreateCommand(trans, cmdText, commandType, paramers))
            {
                using (var dbDA = DbProvider.GetDbProviderFactory().CreateDataAdapter())
                {
                    dbDA.SelectCommand = dbCmd;
                    dbDA.Fill(dataSet);
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
        /// <returns>参数</returns>
        public DbParameter CreateParameter(string pName, object pValue)
        {
            var paramer = DbProvider.GetDbProviderFactory().CreateParameter();
            paramer.ParameterName = pName;
            paramer.Value = pValue;

            return paramer;
        }

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="pName">参数名</param>
        /// <param name="pValue">参数值</param>
        /// <param name="pType">参数类型</param>
        /// <returns>参数</returns>
        public DbParameter CreateParameter(string pName, object pValue, DbType pType)
        {
            var paramer = DbProvider.GetDbProviderFactory().CreateParameter();
            paramer.ParameterName = pName;
            paramer.Value = pValue;
            paramer.DbType = pType;

            return paramer;
        }

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="pName">参数名</param>
        /// <param name="pValue">参数值</param>
        /// <param name="pType">参数类型</param>
        /// <param name="pSize">长度</param>
        /// <returns>参数</returns>
        public DbParameter CreateParameter(string pName, object pValue, DbType pType, int pSize)
        {
            var paramer = DbProvider.GetDbProviderFactory().CreateParameter();
            paramer.ParameterName = pName;
            paramer.Value = pValue;
            paramer.DbType = pType;
            paramer.Size = pSize;

            return paramer;
        }

        #region output
        /// <summary>
        /// 创建一个带有返回值的存储过程参数
        /// </summary>
        /// <param name="pName">参数名称</param>
        /// <returns></returns>
        public virtual DbParameter CreateOutPutParameter(string pName)
        {
            var paramer = DbProvider.GetDbProviderFactory().CreateParameter();
            paramer.ParameterName = pName;
            paramer.Direction = ParameterDirection.Output;

            return paramer;
        }
        /// <summary>
        /// 创建一个带有返回值的存储过程参数
        /// </summary>
        /// <param name="pName">参数名称</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="sourceType">映射属性值</param>
        /// <returns></returns>
        public virtual DbParameter CreateOutPutParameter(string pName,string propertyName,object propertyValue)
        {
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
        /// <param name="pValue">参数值</param>
        /// <returns></returns>
        public virtual DbParameter CreateOutPutParameter(string pName, object pValue)
        {
            var paramer = DbProvider.GetDbProviderFactory().CreateParameter();
            paramer.ParameterName = pName;
            paramer.Direction = ParameterDirection.Output;
            paramer.Value = pValue;

            return paramer;
        }

        /// <summary>
        /// 创建一个带有返回值的存储过程参数
        /// </summary>
        /// <param name="pName">参数名称</param>
        /// <param name="pType">参数类型</param>
        /// <returns></returns>
        public virtual DbParameter CreateOutPutParameter(string pName, DbType pType)
        {
            var paramer = DbProvider.GetDbProviderFactory().CreateParameter();
            paramer.ParameterName = pName;
            paramer.DbType = pType;
            paramer.Direction = ParameterDirection.Output;

            return paramer;
        }

        /// <summary>
        /// 创建一个带有返回值的存储过程参数
        /// </summary>
        /// <param name="pName">参数名称</param>
        /// <param name="pValue">参数值</param>
        /// <param name="pType">参数类型</param>
        /// <returns></returns>
        public virtual DbParameter CreateOutPutParameter(string pName, object pValue, DbType pType)
        {
            var paramer = DbProvider.GetDbProviderFactory().CreateParameter();
            paramer.ParameterName = pName;
            paramer.DbType = pType;
            paramer.Direction = ParameterDirection.Output;
            paramer.Value = pValue;

            return paramer;
        }

        /// <summary>
        /// 创建一个带有返回值的存储过程参数
        /// </summary>
        /// <param name="pName">参数名称</param>
        /// <param name="pType">参数类型</param>
        /// <param name="pSize">长度</param>
        /// <returns></returns>
        public virtual DbParameter CreateOutPutParameter(string pName, DbType pType, int pSize)
        {
            var paramer = DbProvider.GetDbProviderFactory().CreateParameter();
            paramer.ParameterName = pName;
            paramer.DbType = pType;
            paramer.Size = pSize;
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
        public virtual DbParameter CreateOutPutParameter(string pName, object pValue, DbType pType, int pSize)
        {
            var paramer = DbProvider.GetDbProviderFactory().CreateParameter();
            paramer.ParameterName = pName;
            paramer.DbType = pType;
            paramer.Size = pSize;
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
        /// <returns></returns>
        public virtual DbParameter CreateReturnValueParameter(string pName)
        {
            var paramer = DbProvider.GetDbProviderFactory().CreateParameter();
            paramer.ParameterName = pName;
            paramer.Direction = ParameterDirection.ReturnValue;

            return paramer;
        }

        /// <summary>
        /// 创建一个带有返回值的存储过程参数
        /// </summary>
        /// <param name="pName">参数名称</param>
        /// <param name="pValue">参数值</param>
        /// <returns></returns>
        public virtual DbParameter CreateReturnValueParameter(string pName, object pValue)
        {
            var paramer = DbProvider.GetDbProviderFactory().CreateParameter();
            paramer.ParameterName = pName;
            paramer.Direction = ParameterDirection.ReturnValue;
            paramer.Value = pValue;

            return paramer;
        }

        /// <summary>
        /// 创建一个带有返回值的存储过程参数
        /// </summary>
        /// <param name="pName">参数名称</param>
        /// <param name="pType">参数类型</param>
        /// <returns></returns>
        public virtual DbParameter CreateReturnValueParameter(string pName, DbType pType)
        {
            var paramer = DbProvider.GetDbProviderFactory().CreateParameter();
            paramer.ParameterName = pName;
            paramer.DbType = pType;
            paramer.Direction = ParameterDirection.ReturnValue;

            return paramer;
        }

        /// <summary>
        /// 创建一个带有返回值的存储过程参数
        /// </summary>
        /// <param name="pName">参数名称</param>
        /// <param name="pValue">参数值</param>
        /// <param name="pType">参数类型</param>
        /// <returns></returns>
        public virtual DbParameter CreateReturnValueParameter(string pName, object pValue, DbType pType)
        {
            var paramer = DbProvider.GetDbProviderFactory().CreateParameter();
            paramer.ParameterName = pName;
            paramer.DbType = pType;
            paramer.Direction = ParameterDirection.ReturnValue;
            paramer.Value = pValue;

            return paramer;
        }

        /// <summary>
        /// 创建一个带有返回值的存储过程参数
        /// </summary>
        /// <param name="pName">参数名称</param>
        /// <param name="pType">参数类型</param>
        /// <param name="pSize">长度</param>
        /// <returns></returns>
        public virtual DbParameter CreateReturnValueParameter(string pName, DbType pType, int pSize)
        {
            var paramer = DbProvider.GetDbProviderFactory().CreateParameter();
            paramer.ParameterName = pName;
            paramer.DbType = pType;
            paramer.Size = pSize;
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
        public virtual DbParameter CreateReturnValueParameter(string pName, object pValue, DbType pType, int pSize)
        {
            var paramer = DbProvider.GetDbProviderFactory().CreateParameter();
            paramer.ParameterName = pName;
            paramer.DbType = pType;
            paramer.Size = pSize;
            paramer.Direction = ParameterDirection.ReturnValue;
            paramer.Value = pValue;

            return paramer;
        }
        #endregion

        #endregion
    }
}
