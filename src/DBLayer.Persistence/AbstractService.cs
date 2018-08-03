using System;
using System.Collections.Generic;
using System.Linq; 
using System.Data;
using System.Reflection;
using System.Data.Common;
using System.Text;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.Logging;
using DBLayer.Core;
using DBLayer.Core.Condition;
using DBLayer.Core.Interface;
using DBLayer.Persistence.Data;

/*------------------------------------------------------------------------------
 * 单元名称：
 * 单元描述： 
 * 创 建 人：wutao
 * 创建日期：2011-11-19
 * 修改日志
 * 修 改 人   修改日期    修改内容
 * 
 * ----------------------------------------------------------------------------*/
namespace DBLayer.Persistence
{
    public abstract class AbstractService : DataSource, IAbstractService
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region property
        public IGenerator TheGenerator { get; set; }
        public IPagerGenerator ThePagerGenerator { get; set; }
        #endregion
        public AbstractService() { }
        public AbstractService(DataSource dataSource) 
        {
            base.DbProvider = dataSource.DbProvider;
            base.ConnectionString = dataSource.ConnectionString;
        }
        #region public method
        
        #region GetEntity
        
        /// <summary>
        /// 获取 T 单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public T GetEntity<T>(Expression<Func<T,bool>> where,Expression<Func<OrderExpression<T>, object>> order=null,DbTransaction trans=null) where T : new()
        {
            var paramerList = new List<DbParameter>();
            var whereStr = this.Where<T>(where, ref paramerList);
            var orderStr = this.Order<T>(order);

            var cmdText=ThePagerGenerator.GetSelectCmdText<T>(this, ref paramerList,whereStr,orderStr,1);

            T entity = GetEntity<T>(cmdText.ToString(), trans, CommandType.Text, paramerList.ToArray());
            return entity;
        }
        /// <summary>
        /// 获取 T 单个实体
        /// </summary>
        /// <typeparam name="T">实体(泛型)类</typeparam>
        /// <param name="paramers">参数数组</param>
        /// <returns>T 实体</returns>
        public T GetEntity<T>(string cmdText, params DbParameter[] paramers) where T : new()
        {
            T entity = GetEntity<T>(cmdText, null, paramers);

            return entity;
        }

        /// <summary>
        /// 获取 T 单个实体
        /// </summary>
        /// <typeparam name="T">实体(泛型)类</typeparam>
        /// <param name="paramers">参数数组</param>
        /// <returns>T 实体</returns>
        public T GetEntity<T>(string cmdText, DbTransaction trans = null, params DbParameter[] paramers) where T : new()
        {
            T entity = GetEntity<T>(cmdText, trans, CommandType.Text, paramers);

            return entity;
        }
        /// <summary>
        /// 获得单个对象
        /// </summary>
        /// <typeparam name="T">实体(泛型)类</typeparam>
        /// <param name="cmdText">sql</param>
        /// <param name="obj">参数</param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public T GetEntity<T>(string cmdText, object obj , DbTransaction trans = null) where T : new()
        {
            var parameter = this.ToDbParameters(obj);

            T entity = GetEntity<T>(cmdText, trans, CommandType.Text, parameter);
            return entity;
        }
        /// <summary>
        /// 获取 T 单个实体
        /// </summary>
        /// <typeparam name="T">实体(泛型)类</typeparam>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="commandType">cmdText 执行类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>T 实体</returns>
        public T GetEntity<T>(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] paramers) where T : new()
        {
            T entity = new T();
            using (var reader = trans == null ? 
                base.CreateDataReader(cmdText, commandType, paramers) :
                base.CreateDataReader(trans, cmdText, commandType, paramers))
            {
                if (reader.Read())
                {
                    ForeachDataAddLogError(reader, ref entity);
                }
                else
                {
                    entity = default(T);
                }
            }

            return entity;
        }

        /// <summary>
        /// 获取 T 单个实体异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="order"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public async Task<T> GetEntityAsync<T>(Expression<Func<T, bool>> where, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null) where T : new()
        {
            var paramerList = new List<DbParameter>();
            var whereStr = this.Where<T>(where, ref paramerList);
            var orderStr = this.Order<T>(order);

            var cmdText = ThePagerGenerator.GetSelectCmdText<T>(this,ref paramerList,whereStr, orderStr,1);

            var entity = await GetEntityAsync<T>(cmdText.ToString(), trans, CommandType.Text, paramerList.ToArray());
            return entity;
        }

        /// <summary>
        /// 获取 T 单个实体异步
        /// </summary>
        /// <typeparam name="T">实体(泛型)类</typeparam>
        /// <param name="cmdText">sql</param>
        /// <param name="obj">参数</param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public async Task<T> GetEntityAsync<T>(string cmdText, object obj, DbTransaction trans = null) where T : new()
        {
            var parameter = this.ToDbParameters(obj);

            var entity =await GetEntityAsync<T>(cmdText, trans, CommandType.Text, parameter);
            return entity;
        }
        /// <summary>
        /// 获取 T 单个实体异步
        /// </summary>
        /// <typeparam name="T">实体(泛型)类</typeparam>
        /// <param name="paramers">参数数组</param>
        /// <returns>T 实体</returns>
        public async Task<T> GetEntityAsync<T>(string cmdText, params DbParameter[] paramers) where T : new()
        {
            var entity = await GetEntityAsync<T>(cmdText, null, paramers);

            return entity;
        }

        /// <summary>
        /// 获取 T 单个实体异步
        /// </summary>
        /// <typeparam name="T">实体(泛型)类</typeparam>
        /// <param name="paramers">参数数组</param>
        /// <returns>T 实体</returns>
        public async Task<T> GetEntityAsync<T>(string cmdText, DbTransaction trans = null, params DbParameter[] paramers) where T : new()
        {
            var entity = await GetEntityAsync<T>(cmdText, trans, CommandType.Text, paramers);

            return entity;
        }

        /// <summary>
        /// 获取 T 单个实体异步
        /// </summary>
        /// <typeparam name="T">实体(泛型)类</typeparam>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="commandType">cmdText 执行类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>T 实体</returns>
        public async Task<T> GetEntityAsync<T>(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] paramers) where T : new()
        {
            T entity = default(T);
            using (var reader = trans == null ?
               await base.CreateDataReaderAsync(cmdText, commandType, paramers) :
               await  base.CreateDataReaderAsync(trans, cmdText, commandType, paramers))
            {
                if (reader.Read())
                {
                    ForeachDataAddLogError(reader, ref entity);
                }
                else
                {
                    entity = default(T);
                }
            }

            return entity;
        }
        #endregion

        #region GetEntityDic
        /// <summary>
        /// 获得单个实体字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public IDictionary<string, object> GetEntityDic<T>(Expression<Func<T, bool>> where, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null) where T : new()
        {
            var paramerList = new List<DbParameter>();
            var whereStr = this.Where<T>(where, ref paramerList);
            var orderStr = this.Order<T>(order);

            var cmdText = ThePagerGenerator.GetSelectDictionaryCmdText<T>(this, ref paramerList, whereStr, orderStr, 1);

            var result = GetEntityDic(cmdText.ToString(), trans, CommandType.Text, paramerList.ToArray());
            return result;
        }

        /// <summary>
        /// 获得字典
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public IDictionary<string, object> GetEntityDic(string cmdText,  params DbParameter[] parameters)
        {
            var returnDictionary = GetEntityDic(cmdText, null, parameters);

            return returnDictionary;
        }

        /// <summary>
        /// 获得字典
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public IDictionary<string, object> GetEntityDic(string cmdText, DbTransaction trans = null, params DbParameter[] parameters)
        {
            var returnDictionary = GetEntityDic(cmdText, trans, CommandType.Text, parameters);

            return returnDictionary;
        }

        /// <summary>
        /// 获得字典
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="obj">参数</param>
        /// <param name="trans">事物</param>
        /// <returns></returns>
        public IDictionary<string, object> GetEntityDic(string cmdText, object obj, DbTransaction trans = null)
        {
            var parameter = this.ToDbParameters(obj);

            var returnDictionary = GetEntityDic(cmdText, trans, CommandType.Text, parameter);
            return returnDictionary;
        }

        /// <summary>
        /// 获得字典
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="commandType">cmdText 执行类型</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public IDictionary<string, object> GetEntityDic(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] parameters)
        {
            var entity = new Dictionary<string, object>();
            using (var reader = trans == null ? 
                base.CreateDataReader(cmdText, commandType, parameters) : 
                base.CreateDataReader(trans,cmdText, commandType, parameters))
            {
                if (reader.Read())
                {
                    ForeachDicAddLogError(reader, ref entity);
                }
                else
                {
                    return null;
                }
            }

            return entity;
        }


        /// <summary>
        /// 获得单个实体字典异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public async Task<IDictionary<string, object>> GetEntityDicAsync<T>(Expression<Func<T, bool>> where, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null) where T : new()
        {
            var paramerList = new List<DbParameter>();
            var whereStr = this.Where<T>(where, ref paramerList);
            var orderStr = this.Order<T>(order);

            var cmdText = ThePagerGenerator.GetSelectDictionaryCmdText<T>(this, ref paramerList, whereStr, orderStr, 1);

            var result = await GetEntityDicAsync(cmdText.ToString(), trans, CommandType.Text, paramerList.ToArray());
            return result;
        }
        /// <summary>
        /// 获得字典异步
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public async Task<IDictionary<string, object>> GetEntityDicAsync(string cmdText, params DbParameter[] parameters)
        {
            var returnDictionary = await GetEntityDicAsync(cmdText, null, parameters);

            return returnDictionary;
        }
        /// <summary>
        /// 获得字典异步
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public async Task<IDictionary<string, object>> GetEntityDicAsync(string cmdText, DbTransaction trans = null, params DbParameter[] parameters)
        {
            var returnDictionary = await GetEntityDicAsync(cmdText, trans, CommandType.Text, parameters);

            return returnDictionary;
        }
        /// <summary>
        /// 获得字典
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="obj">参数</param>
        /// <param name="trans">事物</param>
        /// <returns></returns>
        public async Task<IDictionary<string, object>> GetEntityDicAsync(string cmdText, object obj, DbTransaction trans = null)
        {
            var parameter = this.ToDbParameters(obj);

            var returnDictionary = await GetEntityDicAsync(cmdText, trans, CommandType.Text, parameter);
            return returnDictionary;
        }
        /// <summary>
        /// 获得字典异步
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="commandType">cmdText 执行类型</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public async Task<IDictionary<string, object>> GetEntityDicAsync(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] parameters)
        {
            var entity = new Dictionary<string, object>();
            using (var reader = trans == null ?
                await base.CreateDataReaderAsync(cmdText, commandType, parameters) :
                await base.CreateDataReaderAsync(trans, cmdText, commandType, parameters))
            {
                if (reader.Read())
                {
                    ForeachDicAddLogError(reader, ref entity);
                }
                else
                {
                    return null;
                }
            }

            return entity;
        }

        #endregion

        #region GetEntityDicList
        
        /// <summary>
        /// 获得实体字典列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="exclusionList"></param>
        /// <returns></returns>
        public IList<IDictionary<string, object>> GetEntityDicList<T>(Expression<Func<T, bool>> where = null, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null, int? top = null, params string[] exclusionList) where T : new()
        {

            var paramerList = new List<DbParameter>();
            var whereStr = this.Where<T>(where, ref paramerList);
            var orderStr = this.Order<T>(order);

            var cmdText = ThePagerGenerator.GetSelectDictionaryCmdText<T>(this, ref paramerList, whereStr, orderStr, top, exclusionList);

            var result = GetEntityDicList(cmdText.ToString(), trans, CommandType.Text, paramerList.ToArray());
            return result;
        }

        /// <summary>
        /// 获得字典列表
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public IList<IDictionary<string, object>> GetEntityDicList(string cmdText, params DbParameter[] parameters)
        {
            var entityDicList = GetEntityDicList(cmdText, null, parameters);

            return entityDicList;
        }

        /// <summary>
        /// 获得字典列表
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public IList<IDictionary<string, object>> GetEntityDicList(string cmdText, DbTransaction trans = null, params DbParameter[] parameters)
        {
            var entityDicList = GetEntityDicList(cmdText, trans, CommandType.Text, parameters);

            return entityDicList;
        }

        /// <summary>
        /// 获得字典列表
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public IList<IDictionary<string, object>> GetEntityDicList(string cmdText,object obj=null, DbTransaction trans = null)
        {
            var parameters= this.ToDbParameters(obj);
            var entityDicList = GetEntityDicList(cmdText, trans, CommandType.Text, parameters);

            return entityDicList;
        }
        /// <summary>
        /// 获得字典列表
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="commandType">cmdText 执行类型</param>
        /// <param name="parameters">参数数组</param>
        /// <exception cref="">这里是 Exception</exception>
        /// <returns></returns>
        public IList<IDictionary<string, object>> GetEntityDicList(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] parameters)
        {
            var entityList = new List<IDictionary<string, object>>();
            using (var reader = trans == null ? 
                base.CreateDataReader(cmdText, commandType, parameters) :
                base.CreateDataReader(trans, cmdText, commandType, parameters))
            {
                while (reader.Read())
                {
                    var entity = new Dictionary<string, object>();
                    ForeachDicAddLogError(reader, ref entity);
                    entityList.Add(entity);
                }
            }

            return entityList;
        }


        /// <summary>
        /// 获得实体字典列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="exclusionList"></param>
        /// <returns></returns>
        public async Task<IList<IDictionary<string, object>>> GetEntityDicListAsync<T>(Expression<Func<T, bool>> where = null, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null, int? top = null, params string[] exclusionList) where T : new()
        {

            var paramerList = new List<DbParameter>();
            var whereStr = this.Where<T>(where, ref paramerList);
            var orderStr = this.Order<T>(order);

            var cmdText = ThePagerGenerator.GetSelectDictionaryCmdText<T>(this, ref paramerList, whereStr, orderStr, top, exclusionList);

            var result = await GetEntityDicListAsync(cmdText.ToString(), trans, CommandType.Text, paramerList.ToArray());
            return result;
        }

        /// <summary>
        /// 获得字典列表
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public async Task<IList<IDictionary<string, object>>> GetEntityDicListAsync(string cmdText, params DbParameter[] parameters)
        {
            var result = await GetEntityDicListAsync(cmdText, null, CommandType.Text, parameters);

            return result;
        }
        /// <summary>
        /// 获得字典列表
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public async Task<IList<IDictionary<string, object>>> GetEntityDicListAsync(string cmdText,DbTransaction trans = null, params DbParameter[] parameters)
        {
            var result = await GetEntityDicListAsync(cmdText, trans, CommandType.Text, parameters);

            return result;
        }
        /// <summary>
        /// 获得字典列表
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public async Task<IList<IDictionary<string, object>>> GetEntityDicListAsync(string cmdText, object obj, DbTransaction trans = null)
        {
            var parameters = this.ToDbParameters(obj);
            var entityDicList = await GetEntityDicListAsync(cmdText, trans, CommandType.Text, parameters);

            return entityDicList;
        }
        /// <summary>
        /// 获得字典列表 异步
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="commandType">cmdText 执行类型</param>
        /// <param name="parameters">参数数组</param>
        /// <exception cref="">这里是 Exception</exception>
        /// <returns></returns>
        public async Task<IList<IDictionary<string, object>>> GetEntityDicListAsync(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] parameters)
        {
            var entityList = new List<IDictionary<string, object>>();
            using (var reader = trans == null ?
                await base.CreateDataReaderAsync(cmdText, commandType, parameters) :
                await base.CreateDataReaderAsync(trans, cmdText, commandType, parameters))
            {
                while (reader.Read())
                {
                    var entity = new Dictionary<string, object>();
                    ForeachDicAddLogError(reader, ref entity);
                    entityList.Add(entity);
                }

            }

            return entityList;
        }
        #endregion

        #region GetEntityList
        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public IList<T> GetEntityList<T>(Expression<Func<T, bool>> where = null,Expression<Func<OrderExpression<T>, object>> order=null, DbTransaction trans = null, int? top = null) where T : new()
        {
            var paramerList = new List<DbParameter>();

            var whereStr = this.Where<T>(where, ref paramerList);
            var orderStr = this.Order<T>(order);

            var cmdText = ThePagerGenerator.GetSelectCmdText<T>(this, ref paramerList, whereStr, orderStr, top);

            var entityList = GetEntityList<T>(cmdText.ToString(), trans, CommandType.Text, paramerList.ToArray());

            return entityList;
        }

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <typeparam name="T">实体(泛型)</typeparam>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="exeEntityType">按属性/特性映射</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>实体集合</returns>
        public IList<T> GetEntityList<T>(string cmdText, params DbParameter[] paramers) where T : new()
        {
            var entityList = GetEntityList<T>(cmdText,null, CommandType.Text, paramers);

            return entityList;
        }
        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <typeparam name="T">实体(泛型)</typeparam>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="exeEntityType">按属性/特性映射</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>实体集合</returns>
        public IList<T> GetEntityList<T>(string cmdText, DbTransaction trans = null, params DbParameter[] paramers) where T : new()
        {
            var entityList = GetEntityList<T>(cmdText, trans, CommandType.Text, paramers);

            return entityList;
        }

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <typeparam name="T">实体(泛型)</typeparam>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="exeEntityType">按属性/特性映射</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>实体集合</returns>
        public IList<T> GetEntityList<T>(string cmdText, object obj, DbTransaction trans = null) where T : new()
        {
            var parameters = this.ToDbParameters(obj);
            var entityList = GetEntityList<T>(cmdText, trans, CommandType.Text, parameters);

            return entityList;
        }
        
        /// <summary>
        /// 获取 T 实体集合
        /// </summary>
        /// <typeparam name="T">实体(泛型)</typeparam>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="commandType">cmdText 执行类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>T 实体集合</returns>
        public IList<T> GetEntityList<T>(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] paramers) where T : new()
        {
            var entityList = new List<T>();
            using (var reader = trans == null ? 
                base.CreateDataReader(cmdText, commandType, paramers) :
                base.CreateDataReader(trans, cmdText, commandType, paramers))
            {
                while (reader.Read())
                {
                    T entity = new T();
                    ForeachDataAddLogError(reader, ref entity);
                    entityList.Add(entity);
                }
            }

            return entityList;
        }

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public async Task<IList<T>> GetEntityListAsync<T>(Expression<Func<T, bool>> where = null, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null, int? top = null) where T : new()
        {
            var paramerList = new List<DbParameter>();

            var whereStr = this.Where<T>(where, ref paramerList);
            var orderStr = this.Order<T>(order);

            var cmdText = ThePagerGenerator.GetSelectCmdText<T>(this, ref paramerList, whereStr, orderStr, top);

            var result = await GetEntityListAsync<T>(cmdText.ToString(), trans, CommandType.Text, paramerList.ToArray());

            return result;
        }

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <typeparam name="T">实体(泛型)</typeparam>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="exeEntityType">按属性/特性映射</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>实体集合</returns>
        public async Task<IList<T>> GetEntityListAsync<T>(string cmdText, params DbParameter[] paramers) where T : new()
        {
            var result = await GetEntityListAsync<T>(cmdText, null, CommandType.Text, paramers);

            return result;
        }
        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <typeparam name="T">实体(泛型)</typeparam>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="exeEntityType">按属性/特性映射</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>实体集合</returns>
        public async Task<IList<T>> GetEntityListAsync<T>(string cmdText,DbTransaction trans = null, params DbParameter[] paramers) where T : new()
        {
            var result = await GetEntityListAsync<T>(cmdText, trans, CommandType.Text, paramers);

            return result;
        }

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <typeparam name="T">实体(泛型)</typeparam>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="exeEntityType">按属性/特性映射</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>实体集合</returns>
        public async Task<IList<T>> GetEntityListAsync<T>(string cmdText, object obj, DbTransaction trans = null) where T : new()
        {
            var parameters = this.ToDbParameters(obj);
            var result = await GetEntityListAsync<T>(cmdText, trans, CommandType.Text, parameters);

            return result;
        }

        /// <summary>
        /// 获取 T 实体集合
        /// </summary>
        /// <typeparam name="T">实体(泛型)</typeparam>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="commandType">cmdText 执行类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>T 实体集合</returns>
        public async Task<IList<T>> GetEntityListAsync<T>(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] paramers) where T : new()
        {
            var entityList = new List<T>();
            using (var readerTask = trans == null ?
                base.CreateDataReaderAsync(cmdText, commandType, paramers) :
                base.CreateDataReaderAsync(trans, cmdText, commandType, paramers))
            {
                using (var reader=await readerTask)
                {
                    while (reader.Read())
                    {
                        T entity = new T();
                        ForeachDataAddLogError(reader, ref entity);
                        entityList.Add(entity);
                    }
                }
                
            }

            return entityList;
        }
        #endregion

        #region GetScalar
        /// <summary>
        /// 返回首行首列
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>返回首行首列</returns>
        public T GetSingle<T>(object obj)
        {
            T single = default(T);

            try
            {
                single = (T)obj.ChangeType(typeof(T));
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return single;
        }
        #endregion

        #region GetList
        /// <summary>
        /// 获取 T 类型的数据集
        /// </summary>
        /// <typeparam name="T">数据类型（泛型）</typeparam>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>数据集合</returns>
        public IList<T> GetList<T>(string cmdText, params DbParameter[] paramers)
        {
            return GetList<T>(cmdText,null, CommandType.Text, paramers);
        }
        /// <summary>
        /// 获取 T 类型的数据集
        /// </summary>
        /// <typeparam name="T">数据类型（泛型）</typeparam>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>数据集合</returns>
        public IList<T> GetList<T>(string cmdText,DbTransaction trans = null, params DbParameter[] paramers)
        {
            return GetList<T>(cmdText, trans, CommandType.Text, paramers);
        }

        /// <summary>
        /// 获取 T 类型的数据集
        /// </summary>
        /// <typeparam name="T">数据类型（泛型）</typeparam>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>数据集合</returns>
        public IList<T> GetList<T>(string cmdText, object obj, DbTransaction trans = null)
        {
            var parameters = this.ToDbParameters(obj);
            return GetList<T>(cmdText, trans, CommandType.Text, parameters);
        }

        /// <summary>
        /// 获取 T 类型的数据集
        /// </summary>
        /// <typeparam name="T">数据类型（泛型）</typeparam>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="commandType">cmdText 执行类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>数据集合</returns>
        public IList<T> GetList<T>(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] paramers)
        {
            var entityList = new List<T>();
            using (var reader = trans == null ? 
                base.CreateDataReader(cmdText, commandType, paramers) : 
                base.CreateDataReader(trans,cmdText, commandType, paramers))
            {
                while (reader.Read())
                {
                    var data = reader[0];
                    T entity = default(T);
                    if (!(data is DBNull))
                    {
                        entity = GetSingle<T>(data);
                    }
                    entityList.Add(entity);
                }
            }

            return entityList;

        }

        /// <summary>
        /// 获取 T 类型的数据集
        /// </summary>
        /// <typeparam name="T">数据类型（泛型）</typeparam>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>数据集合</returns>
        public async Task<IList<T>> GetListAsync<T>(string cmdText, params DbParameter[] paramers)
        {
            var result= await GetListAsync<T>(cmdText, null, CommandType.Text, paramers);
            return result;
        }
        /// <summary>
        /// 获取 T 类型的数据集
        /// </summary>
        /// <typeparam name="T">数据类型（泛型）</typeparam>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>数据集合</returns>
        public async Task<IList<T>> GetListAsync<T>(string cmdText, DbTransaction trans = null, params DbParameter[] paramers)
        {
            var result = await GetListAsync<T>(cmdText, trans, CommandType.Text, paramers);
            return result;
        }
        /// <summary>
        /// 获取 T 类型的数据集
        /// </summary>
        /// <typeparam name="T">数据类型（泛型）</typeparam>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>数据集合</returns>
        public async Task<IList<T>> GetListAsync<T>(string cmdText, object obj, DbTransaction trans = null)
        {
            var parameters = this.ToDbParameters(obj);
            var result = await GetListAsync<T>(cmdText, trans, CommandType.Text, parameters);
            return result;
        }
        /// <summary>
        /// 获取 T 类型的数据集
        /// </summary>
        /// <typeparam name="T">数据类型（泛型）</typeparam>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="commandType">cmdText 执行类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>数据集合</returns>
        public async Task<IList<T>> GetListAsync<T>(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] paramers)
        {
            var entityList = new List<T>();
            using (var readerTask = trans == null ?
                base.CreateDataReaderAsync(cmdText, commandType, paramers) :
                base.CreateDataReaderAsync(trans, cmdText, commandType, paramers))
            {
                using (var reader = await readerTask)
                {
                    while (reader.Read())
                    {
                        var data = reader[0];
                        T entity = default(T);
                        if (!(data is DBNull))
                        {
                            entity = GetSingle<T>(data);
                        }
                        entityList.Add(entity);
                    }
                }
                
            }

            return entityList;

        }
        #endregion

        #region GetEntityDicStr
        /// <summary>
        /// 获得单个实体字符串字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public IDictionary<string, string> GetEntityDicStr<T>(Expression<Func<T, bool>> where,Expression<Func<OrderExpression<T>, object>> order, DbTransaction trans = null) where T : new()
        {

            var paramerList = new List<DbParameter>();
            var whereStr = this.Where<T>(where, ref paramerList);
            var orderStr = this.Order<T>(order);

            var cmdText = ThePagerGenerator.GetSelectDictionaryCmdText<T>(this, ref paramerList, whereStr, orderStr, 1);

            var result = GetEntityDicStr(cmdText.ToString(),trans,CommandType.Text, paramerList.ToArray());
            return result;
        }


        /// <summary>
        /// 获得字符串字典
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public IDictionary<string, string> GetEntityDicStr(string cmdText, params DbParameter[] parameters)
        {
            var returnDictionary = GetEntityDicStr(cmdText,null, CommandType.Text, parameters);

            return returnDictionary;
        }
        /// <summary>
        /// 获得字符串字典
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public IDictionary<string, string> GetEntityDicStr(string cmdText, DbTransaction trans = null, params DbParameter[] parameters)
        {
            var returnDictionary = GetEntityDicStr(cmdText, trans, CommandType.Text, parameters);

            return returnDictionary;
        }
        /// <summary>
        /// 获得字符串字典
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public IDictionary<string, string> GetEntityDicStr(string cmdText, object obj, DbTransaction trans = null)
        {
            var parameters = this.ToDbParameters(obj);
            var returnDictionary = GetEntityDicStr(cmdText, trans, CommandType.Text, parameters);

            return returnDictionary;
        }
        
        /// <summary>
        /// 获得字符串字典
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="commandType">cmdText 执行类型</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public IDictionary<string, string> GetEntityDicStr(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] parameters)
        {
            var entity = new Dictionary<string, string>();
            using (var reader = trans == null ? 
                base.CreateDataReader(cmdText, commandType, parameters) :
                base.CreateDataReader(trans, cmdText, commandType, parameters))
            {
                if (reader.Read())
                {
                    ForeachDicAddLogErrorStr(reader, ref entity);
                }
                else
                {
                    return null;
                }
            }

            return entity;
        }


        /// <summary>
        /// 获得单个实体字符串字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public async Task<IDictionary<string, string>> GetEntityDicStrAsync<T>(Expression<Func<T, bool>> where, Expression<Func<OrderExpression<T>, object>> order, DbTransaction trans = null) where T : new()
        {

            var paramerList = new List<DbParameter>();
            var whereStr = this.Where<T>(where, ref paramerList);
            var orderStr = this.Order<T>(order);

            var cmdText = ThePagerGenerator.GetSelectDictionaryCmdText<T>(this, ref paramerList, whereStr, orderStr, 1);

            var result = await GetEntityDicStrAsync(cmdText.ToString(), trans, CommandType.Text, paramerList.ToArray());
            return result;
        }


        /// <summary>
        /// 获得字符串字典
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public async Task<IDictionary<string, string>> GetEntityDicStrAsync(string cmdText, params DbParameter[] parameters)
        {
            var result = await GetEntityDicStrAsync(cmdText, null, CommandType.Text, parameters);

            return result;
        }
        /// <summary>
        /// 获得字符串字典
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public async Task<IDictionary<string, string>> GetEntityDicStrAsync(string cmdText, DbTransaction trans = null, params DbParameter[] parameters)
        {
            var result = await GetEntityDicStrAsync(cmdText, trans, CommandType.Text, parameters);

            return result;
        }
        /// <summary>
        /// 获得字符串字典
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public async Task<IDictionary<string, string>> GetEntityDicStrAsync(string cmdText, object obj, DbTransaction trans = null)
        {
            var parameters = this.ToDbParameters(obj);
            var result = await GetEntityDicStrAsync(cmdText, trans, CommandType.Text, parameters);

            return result;
        }
        /// <summary>
        /// 获得字符串字典
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="commandType">cmdText 执行类型</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public async Task<IDictionary<string, string>> GetEntityDicStrAsync(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] parameters)
        {
            var entity = new Dictionary<string, string>();
            using (var readerTask = trans == null ?
                base.CreateDataReaderAsync(cmdText, commandType, parameters) :
                base.CreateDataReaderAsync(trans, cmdText, commandType, parameters))
            {
                using (var reader = await readerTask) {
                    if (reader.Read())
                    {
                        ForeachDicAddLogErrorStr(reader, ref entity);
                    }
                    else
                    {
                        return null;
                    }
                }
                
            }

            return entity;
        }

        #endregion

        #region GetEntityDicStrList
        /// <summary>
        /// 获得实体字典字符串列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="exclusionList"></param>
        /// <returns></returns>
        public IList<IDictionary<string, string>> GetEntityDicStrList<T>(Expression<Func<T, bool>> where = null,Expression<Func<OrderExpression<T>, object>> order=null, DbTransaction trans = null, int? top = null, params string[] exclusionList) where T : new()
        {

            var paramerList = new List<DbParameter>();
            var whereStr = this.Where<T>(where, ref paramerList);
            var orderStr = this.Order<T>(order);

            var cmdText = ThePagerGenerator.GetSelectDictionaryCmdText<T>(this, ref paramerList, whereStr, orderStr, top, exclusionList);

            var result = GetEntityDicStrList(cmdText.ToString(),trans,CommandType.Text, paramerList.ToArray());
            return result;
        }

        /// <summary>
        /// 获得典字符串列表
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public IList<IDictionary<string, string>> GetEntityDicStrList(string cmdText, params DbParameter[] parameters)
        {
            var entityDicList = GetEntityDicStrList(cmdText, null, CommandType.Text, parameters);

            return entityDicList;
        }
        /// <summary>
        /// 获得典字符串列表
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public IList<IDictionary<string, string>> GetEntityDicStrList(string cmdText, DbTransaction trans = null, params DbParameter[] parameters)
        {
            var entityDicList = GetEntityDicStrList(cmdText, trans, CommandType.Text, parameters);

            return entityDicList;
        }
        /// <summary>
        /// 获得典字符串列表
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public IList<IDictionary<string, string>> GetEntityDicStrList(string cmdText, object obj, DbTransaction trans = null)
        {
            var parameters = this.ToDbParameters(obj);
            var entityDicList = GetEntityDicStrList(cmdText, trans, CommandType.Text, parameters);

            return entityDicList;
        }
        
        /// <summary>
        /// 获得典字符串列表
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="commandType">cmdText 执行类型</param>
        /// <param name="parameters">参数数组</param>
        /// <exception cref="">这里是 Exception</exception>
        /// <returns></returns>
        public IList<IDictionary<string, string>> GetEntityDicStrList(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] parameters)
        {
            var entityList = new List<IDictionary<string, string>>();
            using (var reader = trans == null ? 
                base.CreateDataReader(cmdText, commandType, parameters) : 
                base.CreateDataReader(trans,cmdText, commandType, parameters))
            {
                while (reader.Read())
                {
                    var entity = new Dictionary<string, string>();
                    ForeachDicAddLogErrorStr(reader, ref entity);
                    entityList.Add(entity);
                }
            }

            return entityList;
        }

        /// <summary>
        /// 获得实体字典字符串列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="exclusionList"></param>
        /// <returns></returns>
        public async Task<IList<IDictionary<string, string>>> GetEntityDicStrListAsync<T>(Expression<Func<T, bool>> where = null, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null, int? top = null, params string[] exclusionList) where T : new()
        {

            var paramerList = new List<DbParameter>();
            var whereStr = this.Where<T>(where, ref paramerList);
            var orderStr = this.Order<T>(order);

            var cmdText = ThePagerGenerator.GetSelectDictionaryCmdText<T>(this, ref paramerList, whereStr, orderStr, top, exclusionList);

            var result = await GetEntityDicStrListAsync(cmdText.ToString(), trans, CommandType.Text, paramerList.ToArray());
            return result;
        }

        /// <summary>
        /// 获得典字符串列表
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public async Task<IList<IDictionary<string, string>>> GetEntityDicStrListAsync(string cmdText, params DbParameter[] parameters)
        {
            var result = await GetEntityDicStrListAsync(cmdText, null, CommandType.Text, parameters);

            return result;
        }

        /// <summary>
        /// 获得典字符串列表
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public async Task<IList<IDictionary<string, string>>> GetEntityDicStrListAsync(string cmdText, DbTransaction trans = null, params DbParameter[] parameters)
        {
            var result = await GetEntityDicStrListAsync(cmdText, trans, CommandType.Text, parameters);

            return result;
        }

        /// <summary>
        /// 获得典字符串列表
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public async Task<IList<IDictionary<string, string>>> GetEntityDicStrListAsync(string cmdText, object obj, DbTransaction trans = null)
        {
            var parameters = this.ToDbParameters(obj);
            var result = await GetEntityDicStrListAsync(cmdText, trans, CommandType.Text, parameters);

            return result;
        }
        /// <summary>
        /// 获得典字符串列表
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="commandType">cmdText 执行类型</param>
        /// <param name="parameters">参数数组</param>
        /// <exception cref="">这里是 Exception</exception>
        /// <returns></returns>
        public async Task<IList<IDictionary<string, string>>> GetEntityDicStrListAsync(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] parameters)
        {
            var entityList = new List<IDictionary<string, string>>();
            using (var readerTask =trans==null?
                base.CreateDataReaderAsync(cmdText, commandType, parameters):
                base.CreateDataReaderAsync(trans,cmdText, commandType, parameters))
            {
                using (var reader = await readerTask)
                {
                    while (reader.Read())
                    {
                        var entity = new Dictionary<string, string>();
                        ForeachDicAddLogErrorStr(reader, ref entity);
                        entityList.Add(entity);
                    }  
                }
            }

            return entityList;
        }
        #endregion

        #region InsertEntity
        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public object InsertEntity<T>(Expression<Func<T>> expression, DbTransaction trans = null) where T : new()
        {
            object newID = null;
            var paramerList = new List<DbParameter>();
            var insertStr = this.Insert<T>(expression, ref newID, ref paramerList, TheGenerator);
            var allInsert = CreateInsertAllSql<T>();
            var insertCmd = new StringBuilder();

            insertCmd.AppendFormat("{0}{1}", allInsert, insertStr);

            if (newID == null)
            {
                //var cmdText = ThePagerGenerator.GetInsertCmdText<T>(this, ref paramerList, insertCmd);
                newID = ThePagerGenerator.InsertExecutor<T>(this, insertCmd, paramerList, trans);
                //newID = trans == null ?
                //    base.ExecuteScalar(cmdText.ToString(), CommandType.Text, paramerList.ToArray()) :
                //    base.ExecuteScalar(trans, cmdText.ToString(), CommandType.Text, paramerList.ToArray());
            }
            else {
                var cmdText = insertCmd;
                if (trans == null)
                {
                    base.ExecuteNonQuery(cmdText.ToString(), CommandType.Text, paramerList.ToArray());
                }
                else
                {
                    base.ExecuteNonQuery(trans, cmdText.ToString(), CommandType.Text, paramerList.ToArray());
                }
            }
            return newID;
        }
        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="expression"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public R InsertEntity<T,R>(Expression<Func<T>> expression, DbTransaction trans = null) where T : new()
        {
            var newID = this.InsertEntity<T>(expression, trans);
            var result = GetSingle<R>(newID);
            
            return result;
        }
        /// <summary>
        /// 通过实体类型 T 向表中增加一条记录
        /// </summary>
        /// <typeparam name="T">实体泛型(类)</typeparam>
        /// <param name="entity">实体对象</param>
        /// <param name="exclusionList"></param>
        /// <returns>所新增记录的 ID,如果返回 -1 则表示增加失败</returns>
        public object InsertEntity<T>(T entity, DbTransaction trans = null, params string[] exclusionList) 
            where T : new()
        {
            var insertCmd = new StringBuilder();
            object newID = null;

            var paramerList = CreateInsertSql(ref insertCmd, entity, ref newID, exclusionList);

            var para = paramerList.ToArray();
            if (newID == null)
            {
                return ThePagerGenerator.InsertExecutor<T>(this, insertCmd, paramerList, trans);
            }

            var retval = -1;
            if (trans == null)
            {
                retval = base.ExecuteNonQuery(insertCmd.ToString(), CommandType.Text, para);
            }
            else
            {
                retval = base.ExecuteNonQuery(trans, insertCmd.ToString(), CommandType.Text, para);
            }

            if (retval <= 0)
            {
                return -1;
            }

            return newID;
        }
        public R InsertEntity<T, R>(T entity, DbTransaction trans = null, params string[] exclusionList)
            where T : new()
        {
            var newID = this.InsertEntity<T>(entity, trans, exclusionList);

            var result = GetSingle<R>(newID);
            return result;
        }
        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public async Task<object> InsertEntityAsync<T>(Expression<Func<T>> expression, DbTransaction trans = null) 
            where T : new()
        {
            object newID = null;
            var paramerList = new List<DbParameter>();
            var insertStr = this.Insert<T>(expression, ref newID, ref paramerList, TheGenerator);
            var allInsert = CreateInsertAllSql<T>();

            var insertCmd = new StringBuilder();
            insertCmd.AppendFormat("{0}{1}", allInsert, insertStr);

            if (newID == null)
            {
                return await ThePagerGenerator.InsertExecutorAsync<T>(this, insertCmd, paramerList, trans);
            }
            
            var cmdText = insertCmd.ToString();
            var retval = -1;
            if (trans == null)
            {
                retval = await base.ExecuteNonQueryAsync(cmdText, CommandType.Text, paramerList.ToArray());
            }
            else
            {
                retval = await base.ExecuteNonQueryAsync(trans, cmdText, CommandType.Text, paramerList.ToArray());
            }

            if (retval <= 0)
            {
                return -1;
            }
            return newID;
        }
        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="expression"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public async Task<R> InsertEntityAsync<T, R>(Expression<Func<T>> expression, DbTransaction trans = null)
            where T : new()
        {
            var newID = await this.InsertEntityAsync<T>(expression, trans);

            var result = GetSingle<R>(newID);

            return result;
        }
        /// <summary>
        /// 通过实体类型 T 向表中增加一条记录
        /// </summary>
        /// <typeparam name="T">实体泛型(类)</typeparam>
        /// <param name="entity">实体对象</param>
        /// <param name="exclusionList"></param>
        /// <returns>所新增记录的 ID,如果返回 -1 则表示增加失败</returns>
        public async Task<object> InsertEntityAsync<T>(T entity, DbTransaction trans = null, params string[] exclusionList)
            where T : new()
        {
            
            var insertCmd = new StringBuilder();
            object newID = null;

            var paramerList = CreateInsertSql(ref insertCmd, entity, ref newID, exclusionList);
            var para = paramerList.ToArray();

            if (newID == null)
            {
                return await ThePagerGenerator.InsertExecutorAsync<T>(this, insertCmd, paramerList, trans);
            }

            var retval = -1;
            if (trans == null)
            {
                retval = await base.ExecuteNonQueryAsync(insertCmd.ToString(), CommandType.Text, para);
            }
            else
            {
                retval = await base.ExecuteNonQueryAsync(trans, insertCmd.ToString(), CommandType.Text, para);
            }

            if (retval <= 0)
            {
                return -1;
            }

            return newID;
        }
        /// <summary>
        /// 通过实体类型 T 向表中增加一条记录
        /// </summary>
        /// <typeparam name="T">实体泛型(类)</typeparam>
        /// <typeparam name="R">返回主键值</typeparam>
        /// <param name="entity"></param>
        /// <param name="trans"></param>
        /// <param name="exclusionList"></param>
        /// <returns></returns>
        public async Task<R> InsertEntityAsync<T, R>(T entity, DbTransaction trans = null, params string[] exclusionList)
            where T : new()
        {
            var newID = await this.InsertEntityAsync<T>(entity, trans, exclusionList);
            var result = GetSingle<R>(newID);
            
            return result;
        }
        #endregion

        #region UpdateEntity

        /// <summary>
        /// 更新操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="where"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public int UpdateEntity<T>(Expression<Func<T>> expression, Expression<Func<T, bool>> where, DbTransaction trans = null) where T : new()
        {
            var result=-1;
            var paramerList = new List<DbParameter>();

            var whereStr = this.Where<T>(where, ref paramerList);
            var updateStr = this.Update<T>(expression,ref paramerList);
            var allUpdate=this.CreateUpdateAllSql<T>();
            var cmdText = string.Format("{0}{1}{2}", allUpdate, updateStr, whereStr);
            result = trans == null ? 
                base.ExecuteNonQuery(cmdText, CommandType.Text, paramerList.ToArray()): 
                base.ExecuteNonQuery(trans,cmdText, CommandType.Text, paramerList.ToArray());
            return result;
        }

        /// <summary>
        /// 修改 T 实体
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>影响数据条数</returns>
        public int UpdateEntity<T>(T entity,DbTransaction trans=null, params string[] exclusionList)
        {
            var cmdText = "";
            var returnValue = 0;

            var paramerList = CreateUpdateSql(ref cmdText, entity, exclusionList);
            returnValue =trans==null? 
                base.ExecuteNonQuery(cmdText, CommandType.Text, paramerList.ToArray()): 
                base.ExecuteNonQuery(trans,cmdText, CommandType.Text, paramerList.ToArray());

            return returnValue;
        }

        /// <summary>
        /// 更新操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="where"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public async Task<int> UpdateEntityAsync<T>(Expression<Func<T>> expression, Expression<Func<T, bool>> where, DbTransaction trans = null) where T : new()
        {
            var paramerList = new List<DbParameter>();

            var whereStr = this.Where<T>(where, ref paramerList);
            var updateStr = this.Update<T>(expression, ref paramerList);
            var allUpdate = this.CreateUpdateAllSql<T>();
            var cmdText = string.Format("{0}{1}{2}", allUpdate, updateStr, whereStr);
            var result = trans == null ?
                await base.ExecuteNonQueryAsync(cmdText, CommandType.Text, paramerList.ToArray()) :
                await base.ExecuteNonQueryAsync(trans, cmdText, CommandType.Text, paramerList.ToArray());
            return result;
        }

        /// <summary>
        /// 修改 T 实体
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>影响数据条数</returns>
        public async Task<int> UpdateEntityAsync<T>(T entity, DbTransaction trans = null, params string[] exclusionList)
        {
            var cmdText = "";
            
            var paramerList = CreateUpdateSql(ref cmdText, entity, exclusionList);
            var result = trans == null ?
                await base.ExecuteNonQueryAsync(cmdText, CommandType.Text, paramerList.ToArray()) :
                await base.ExecuteNonQueryAsync(trans, cmdText, CommandType.Text, paramerList.ToArray());

            return result;
        }


        #endregion

        #region DeleteEntity
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public int DeleteEntity<T>(Expression<Func<T, bool>> where, DbTransaction trans = null) where T : new()
        {
            var paramerList = new List<DbParameter>();

            var whereStr = this.Where<T>(where, ref paramerList);
            var allDelete = this.CreateDeleteAllSql<T>();
            var cmdText = string.Format("{0}{1}", allDelete, whereStr);
            var result = trans == null ?
                base.ExecuteNonQuery(cmdText, CommandType.Text, paramerList.ToArray()) :
                base.ExecuteNonQuery(trans, cmdText, CommandType.Text, paramerList.ToArray());

            return result;
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <param name="ID">主键</param>
        /// <returns>影响的条数</returns>
        public int DeleteEntity<T>(object ID,DbTransaction trans=null, params string[] exclusionList)
        {
            var cmdText = "";
            var paramerList = CreateDelSql(ref cmdText, true, typeof(T), ID, exclusionList);
            return 
                trans==null?
                base.ExecuteNonQuery(cmdText, CommandType.Text, paramerList.ToArray()):
                base.ExecuteNonQuery(trans, cmdText, CommandType.Text, paramerList.ToArray());
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>影响数据条数</returns>
        public int DeleteEntity<T>(T entity,DbTransaction trans=null, params string[] exclusionList)
        {
            var cmdText = "";
            var paramerList = CreateDeleteSql(ref cmdText, entity, exclusionList);
            return 
                trans==null?
                base.ExecuteNonQuery(cmdText, CommandType.Text, paramerList.ToArray()):
                base.ExecuteNonQuery(trans,cmdText, CommandType.Text, paramerList.ToArray());
        }

        /// <summary>
        /// 删除数据异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public async Task<int> DeleteEntityAsync<T>(Expression<Func<T, bool>> where, DbTransaction trans = null) where T : new()
        {

            var paramerList = new List<DbParameter>();

            var whereStr = this.Where<T>(where, ref paramerList);
            var allDelete = this.CreateDeleteAllSql<T>();
            var cmdText = string.Format("{0}{1}", allDelete, whereStr);
            var result = trans == null ?
                await base.ExecuteNonQueryAsync(cmdText, CommandType.Text, paramerList.ToArray()) :
                await base.ExecuteNonQueryAsync(trans,cmdText, CommandType.Text, paramerList.ToArray());

            return result;
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <param name="ID">主键</param>
        /// <returns>影响的条数</returns>
        public async Task<int> DeleteEntityAsync<T>(object ID, DbTransaction trans = null, params string[] exclusionList)
        {
            var cmdText = "";
            var paramerList = CreateDelSql(ref cmdText, true, typeof(T), ID, exclusionList);

            var result=trans == null ?
                await base.ExecuteNonQueryAsync(cmdText, CommandType.Text, paramerList.ToArray()) :
                await base.ExecuteNonQueryAsync(trans, cmdText, CommandType.Text, paramerList.ToArray());
            return result;
                
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>影响数据条数</returns>
        public async Task<int> DeleteEntityAsync<T>(T entity, DbTransaction trans = null, params string[] exclusionList)
        {
            var cmdText = "";
            var paramerList = CreateDeleteSql(ref cmdText, entity, exclusionList);
            var result=trans == null ?
                await base.ExecuteNonQueryAsync(cmdText, CommandType.Text, paramerList.ToArray()) :
                await base.ExecuteNonQueryAsync(trans, cmdText, CommandType.Text, paramerList.ToArray());
            return result;
        }
        #endregion

        #region Other

        #region ExecuteNonQuery
        /// <summary>
        /// 执行 SQL 语句
        /// </summary>
        /// <param name="cmdText">>SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>SQL 语句所影响的行数</returns>
        public int ExecuteNonQuery(string cmdText, params DbParameter[] paramers)
        {
            var returnValue = this.ExecuteNonQuery(cmdText, null, CommandType.Text, paramers);

            return returnValue;
        }

        /// <summary>
        /// 执行 SQL 语句
        /// </summary>
        /// <param name="cmdText">>SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>SQL 语句所影响的行数</returns>
        public int ExecuteNonQuery(string cmdText, DbTransaction trans = null, params DbParameter[] paramers)
        {
            var returnValue = this.ExecuteNonQuery(cmdText, trans, CommandType.Text, paramers);

            return returnValue;
        }
        /// <summary>
        /// 执行 SQL 语句
        /// </summary>
        /// <param name="cmdText">>SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>SQL 语句所影响的行数</returns>
        public int ExecuteNonQuery(string cmdText, object obj, DbTransaction trans = null)
        {
            var parameters = this.ToDbParameters(obj);
            var returnValue = this.ExecuteNonQuery(cmdText, trans, CommandType.Text, parameters);

            return returnValue;
        }
        
        /// <summary>
        /// 执行 SQL 语句
        /// </summary>
        /// <param name="cmdText">>SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>SQL 语句所影响的行数</returns>
        public int ExecuteNonQuery(string cmdText, DbTransaction trans=null, CommandType commandType = CommandType.Text, params DbParameter[] paramers)
        {
            var returnValue =
                trans==null?
                base.ExecuteNonQuery(cmdText, commandType, paramers):
                base.ExecuteNonQuery(trans, cmdText, CommandType.Text, paramers);

            return returnValue;
        }

        /// <summary>
        /// 执行 SQL 语句
        /// </summary>
        /// <param name="cmdText">>SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>SQL 语句所影响的行数</returns>
        public async Task<int> ExecuteNonQueryAsync(string cmdText, params DbParameter[] paramers)
        {
            var returnValue = await this.ExecuteNonQueryAsync(cmdText, null, CommandType.Text, paramers);

            return returnValue;
        }

        /// <summary>
        /// 执行 SQL 语句
        /// </summary>
        /// <param name="cmdText">>SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>SQL 语句所影响的行数</returns>
        public async Task<int> ExecuteNonQueryAsync(string cmdText, DbTransaction trans = null, params DbParameter[] paramers)
        {
            var returnValue = await this.ExecuteNonQueryAsync(cmdText, trans, CommandType.Text, paramers);

            return returnValue;
        }
        /// <summary>
        /// 执行 SQL 语句
        /// </summary>
        /// <param name="cmdText">>SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>SQL 语句所影响的行数</returns>
        public async Task<int> ExecuteNonQueryAsync(string cmdText, object obj, DbTransaction trans = null)
        {
            var parameters = this.ToDbParameters(obj);
            var returnValue = await this.ExecuteNonQueryAsync(cmdText, trans, CommandType.Text, parameters);

            return returnValue;
        }
        
        /// <summary>
        /// 执行 SQL 语句
        /// </summary>
        /// <param name="cmdText">>SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>SQL 语句所影响的行数</returns>
        public async Task<int> ExecuteNonQueryAsync(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] paramers)
        {
            var returnValue =
                trans == null ?
                await base.ExecuteNonQueryAsync(cmdText, commandType, paramers) :
                await base.ExecuteNonQueryAsync(trans, cmdText, CommandType.Text, paramers);

            return returnValue;
        }
        #endregion

        #region ExecuteScalar
        /// <summary>
        /// 执行数据库操作，返回首行首列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmdText"></param>
        /// <param name="paramers"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string cmdText, params DbParameter[] paramers) 
        {
            var result = ExecuteScalar<T>(cmdText, null, paramers);
            return result;
        }
        /// <summary>
        /// 执行数据库操作，返回首行首列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmdText"></param>
        /// <param name="paramers"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string cmdText, DbTransaction trans = null, params DbParameter[] paramers)
        {
            var result = ExecuteScalar<T>(cmdText, trans, CommandType.Text, paramers);
            return result;
        }
        /// <summary>
        /// 执行数据库操作，返回首行首列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmdText"></param>
        /// <param name="paramers"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string cmdText, object obj, DbTransaction trans = null)
        {
            var parameters = this.ToDbParameters(obj);
            var result = ExecuteScalar<T>(cmdText, trans, parameters);
            return result;
        }
        
        /// <summary>
        /// 执行数据库操作，返回首行首列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmdText"></param>
        /// <param name="trans"></param>
        /// <param name="commandType"></param>
        /// <param name="paramers"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] paramers) 
        {
            var obj=ExecuteScalar(cmdText, trans, commandType, paramers);
            var result=GetSingle<T>(obj);
            return result;
        }
        /// <summary>
        /// 执行数据库操作，返回首行首列
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>返回首行首列</returns>
        public object ExecuteScalar(string cmdText, params DbParameter[] paramers)
        {
            var returnObj = this.ExecuteScalar(cmdText, null, CommandType.Text, paramers);

            return returnObj;
        }
        /// <summary>
        /// 执行数据库操作，返回首行首列
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>返回首行首列</returns>
        public object ExecuteScalar(string cmdText, DbTransaction trans = null, params DbParameter[] paramers)
        {
            var returnObj = this.ExecuteScalar(cmdText, trans, CommandType.Text, paramers);

            return returnObj;
        }
        /// <summary>
        /// 执行数据库操作，返回首行首列
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>返回首行首列</returns>
        public object ExecuteScalar(string cmdText, object obj, DbTransaction trans = null)
        {
            var parameters = this.ToDbParameters(obj);
            var returnObj = this.ExecuteScalar(cmdText, trans, CommandType.Text, parameters);

            return returnObj;
        }
        
        /// <summary>
        /// 执行数据库操作，返回首行首列
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>返回首行首列</returns>
        public object ExecuteScalar(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] paramers)
        {
            var returnObj =
                trans == null ?
                base.ExecuteScalar(cmdText, commandType, paramers) :
                base.ExecuteScalar(trans, cmdText, commandType, paramers);

            return returnObj;
        }

        /// <summary>
        /// 执行数据库操作，返回首行首列
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>返回首行首列</returns>
        public async Task<object> ExecuteScalarAsync(string cmdText, params DbParameter[] paramers)
        {
            var result = await this.ExecuteScalarAsync(cmdText, null, CommandType.Text, paramers);
            return result;
        }
        /// <summary>
        /// 执行数据库操作，返回首行首列
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>返回首行首列</returns>
        public async Task<object> ExecuteScalarAsync(string cmdText, DbTransaction trans = null, params DbParameter[] paramers)
        {
            var result = await this.ExecuteScalarAsync(cmdText, trans, CommandType.Text, paramers);

            return result;
        }
        /// <summary>
        /// 执行数据库操作，返回首行首列
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>返回首行首列</returns>
        public async Task<object> ExecuteScalarAsync(string cmdText, object obj, DbTransaction trans = null)
        {
            var parameters = this.ToDbParameters(obj);
            var result = await this.ExecuteScalarAsync(cmdText, trans, CommandType.Text, parameters);
            return result;
        }
        /// <summary>
        /// 执行数据库操作，返回首行首列
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>返回首行首列</returns>
        public async Task<object> ExecuteScalarAsync(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] paramers)
        {
            var result =
                trans == null ?
                await base.ExecuteScalarAsync(cmdText, commandType, paramers) :
                await base.ExecuteScalarAsync(trans, cmdText, commandType, paramers);

            return result;
        }
        #endregion
        
        #region IsExists
        /// <summary>
        /// 判断记录是否存在
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>记录是否存在,true:表示存在记录,false:表示不存在记录</returns>
        public bool IsExists(string cmdText, params DbParameter[] paramers)
        {
            var returnValue = IsExists(cmdText,null, CommandType.Text, paramers);

            return returnValue;
        }
        /// <summary>
        /// 判断记录是否存在
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>记录是否存在,true:表示存在记录,false:表示不存在记录</returns>
        public bool IsExists(string cmdText, DbTransaction trans = null, params DbParameter[] paramers)
        {
            var returnValue = IsExists(cmdText, trans, CommandType.Text, paramers);

            return returnValue;
        }
        /// <summary>
        /// 判断记录是否存在
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>记录是否存在,true:表示存在记录,false:表示不存在记录</returns>
        public bool IsExists(string cmdText, object obj, DbTransaction trans = null)
        {
            var parameters = this.ToDbParameters(obj);
            var returnValue = IsExists(cmdText, trans, CommandType.Text, parameters);

            return returnValue;
        }
        
        /// <summary>
        /// 判断记录是否存在
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="commondType">cmdText 执行类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>记录是否存在,true:表示存在记录,false:表示不存在记录</returns>
        public bool IsExists(string cmdText, DbTransaction trans=null, CommandType commondType = CommandType.Text, params DbParameter[] paramers)
        {

            var obj =
                trans == null ?
                base.ExecuteScalar(cmdText, commondType, paramers) :
                base.ExecuteScalar(trans, cmdText, commondType, paramers);

            var result = true;

            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 判断记录是否存在
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>记录是否存在,true:表示存在记录,false:表示不存在记录</returns>
        public async Task<bool> IsExistsAsync(string cmdText, params DbParameter[] paramers)
        {
            var returnValue = await IsExistsAsync(cmdText, null, CommandType.Text, paramers);

            return returnValue;
        }
        /// <summary>
        /// 判断记录是否存在
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>记录是否存在,true:表示存在记录,false:表示不存在记录</returns>
        public async Task<bool> IsExistsAsync(string cmdText, DbTransaction trans = null, params DbParameter[] paramers)
        {
            var returnValue = await IsExistsAsync(cmdText, trans, CommandType.Text, paramers);

            return returnValue;
        }
        /// <summary>
        /// 判断记录是否存在
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>记录是否存在,true:表示存在记录,false:表示不存在记录</returns>
        public async Task<bool> IsExistsAsync(string cmdText, object obj, DbTransaction trans = null)
        {
            var parameters = this.ToDbParameters(obj);
            var returnValue = await IsExistsAsync(cmdText, trans, CommandType.Text, parameters);

            return returnValue;
        }
        
        /// <summary>
        /// 判断记录是否存在
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="commondType">cmdText 执行类型</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>记录是否存在,true:表示存在记录,false:表示不存在记录</returns>
        public async Task<bool> IsExistsAsync(string cmdText, DbTransaction trans = null, CommandType commondType = CommandType.Text, params DbParameter[] paramers)
        {

            var objTask =
                trans == null ?
                base.ExecuteScalarAsync(cmdText, commondType, paramers) :
                base.ExecuteScalarAsync(trans, cmdText, commondType, paramers);

            var result = true;

            var obj = await objTask;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                result = false;
            }

            return result;
        }
        #endregion

        #region GetDataSet
        /// <summary>
        /// 返回数据集
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>数据集</returns>
        public DataSet GetDataSet(string cmdText, params DbParameter[] paramers)
        {
            var returnDataSet = base.CreateDataSet(cmdText, CommandType.Text, paramers);

            return returnDataSet;
        }
        /// <summary>
        /// 返回数据集
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>数据集</returns>
        public DataSet GetDataSet(string cmdText, DbTransaction trans = null, params DbParameter[] paramers)
        {
            var returnDataSet = this.GetDataSet(cmdText, trans, CommandType.Text, paramers);

            return returnDataSet;
        }
        /// <summary>
        /// 返回数据集
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>数据集</returns>
        public DataSet GetDataSet(string cmdText, object obj, DbTransaction trans = null)
        {
            var parameters = this.ToDbParameters(obj);
            var returnDataSet = this.GetDataSet(cmdText,trans, CommandType.Text, parameters);

            return returnDataSet;
        }

        /// <summary>
        /// 返回数据集
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>数据集</returns>
        public DataSet GetDataSet(string cmdText, DbTransaction trans = null, CommandType commondType = CommandType.Text, params DbParameter[] paramers)
        {
            var returnDataSet = 
                trans == null ? 
                base.CreateDataSet(cmdText, commondType, paramers) :
                base.CreateDataSet(trans,cmdText, commondType, paramers);

            return returnDataSet;
        }

        #endregion

        #endregion

        #endregion
        
        #region 分页

        /// <summary>
        /// 生成分页sql
        /// </summary>
        /// <param name="UnionText"></param>
        /// <param name="TableName"></param>
        /// <param name="FldName"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="Filter"></param>
        /// <param name="Group"></param>
        /// <param name="Sort"></param>
        /// <param name="parameter"></param>
        /// <param name="paramers"></param>
        /// <returns></returns>
        private string GetPageCmdText(string UnionText,string TableName, string FldName,ref int? PageIndex,ref int? PageSize, string Filter, string Group, string Sort, ref DbParameter[] parameter, params DbParameter[] paramers)
        {
            var result= ThePagerGenerator.GetPageCmdText(this, UnionText,TableName, FldName,ref PageIndex,ref PageSize,Filter, Group,Sort,ref parameter,paramers);
            return result.ToString();
        }
        #region pager
        private IDataReader GetResultByPager<T>(Pager<T> page) where T : BasePageCondition, new()
        {
            var cmdText = "";
            DbParameter[] parameter = null;

            page.Execute();

            var pageIndex=page.Condition.PageIndex;
            var pageSize=page.Condition.PageSize;

            cmdText = GetPageCmdText(page.UnionText,
                page.Table,
                page.Field,
                ref pageIndex,
                ref pageSize,
                page.Where.ToString(),
                page.Group,
                page.Order,
                ref parameter,
                page.Parameters.ToArray());

            page.Condition.PageIndex = pageIndex;
            page.Condition.PageSize = pageSize;

            var reader = base.CreateDataReader(cmdText, CommandType.Text, parameter);
            return reader;
        }
        private Task<DbDataReader> GetResultByPagerAsync<T>(Pager<T> page) where T : BasePageCondition, new()
        {
            var cmdText = "";
            DbParameter[] parameter = null;
            page.Execute();
            var pageIndex = page.Condition.PageIndex;
            var pageSize = page.Condition.PageSize;
            //page.Paramers;
            cmdText = GetPageCmdText(page.UnionText,
                page.Table,
                page.Field,
                ref pageIndex,
                ref pageSize,
                page.Where.ToString(),
                page.Group,
                page.Order,
                ref parameter,
                page.Parameters.ToArray());
            page.Condition.PageIndex = pageIndex;
            page.Condition.PageSize = pageSize;

            var reader = base.CreateDataReaderAsync(cmdText, CommandType.Text, parameter);
            return reader;
        }

        #region pager result list
        
        
        /// <summary>
        /// 返回分页实体列表集合
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetResultByPager<T, T1>(Pager<T1> page)
            where T : new()
            where T1 : BasePageCondition, new()
        {
            IEnumerable<T> result = null;
            using (var reader = GetResultByPager(page))
            {
                var totalCount = 0;
                result = reader.ReadList(r => r.ReadObject<T>(), ref totalCount);
                page.SetTotalCount(totalCount);
            }
            return result;
        }

        /// <summary>
        /// 返回分页实体列表集合异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetResultByPageAsync<T, T1>(Pager<T1> page)
            where T : new()
            where T1 : BasePageCondition, new()
        {
            IEnumerable<T> result = null;
            using (var readerTask = GetResultByPagerAsync(page))
            {
                using (var reader = await readerTask)
                {
                    var totalCount = 0;
                    result = reader.ReadList(r => r.ReadObject<T>(), ref totalCount);
                    page.SetTotalCount(totalCount);
                }
            }
            return result;
        }

        /// <summary>
        /// 返回分页实体字典列表集合
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="page"></param>
        /// <returns></returns>
        public IEnumerable<IDictionary<string, object>> GetResultByPagerDic<T1>(Pager<T1> page) 
            where T1 : BasePageCondition, new()
        {
            IEnumerable<IDictionary<string, object>> result = null;
            using (var reader = GetResultByPager(page))
            {
                var totalCount = 0;
                result = reader.ReadList(r => r.ReadSelf(), ref totalCount);
                page.SetTotalCount(totalCount);
            }
            return result;
        }

        /// <summary>
        /// 返回分页实体字典列表集合异步
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IDictionary<string, object>>> GetResultByPagerDicAsync<T1>(Pager<T1> page) 
            where T1 : BasePageCondition, new()
        {
            IEnumerable<IDictionary<string, object>> result = null;
            using (var readerTask = GetResultByPagerAsync(page))
            {
                using (var reader = await readerTask)
                {
                    var totalCount = 0;
                    result = reader.ReadList(r => r.ReadSelf(), ref totalCount);
                    page.SetTotalCount(totalCount);
                }
            }
            return result;
        }


        /// <summary>
        /// 返回分页实体字典列表集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="page"></param>
        /// <returns></returns>
        public IEnumerable<IDictionary<string, object>> GetResultByPagerDic<T, T1>(Pager<T1> page)
            where T : new()
            where T1 : BasePageCondition, new()
        {
            IEnumerable<IDictionary<string, object>> result = null;
            var FldName = CreateAllEntityDicSql<T>();
            page.Field = FldName;
            using (var reader = GetResultByPager(page))
            {
                var totalCount = 0;
                result = reader.ReadList(r => r.ReadSelf(), ref totalCount);
                page.SetTotalCount(totalCount);
            }
            return result;
        }

        /// <summary>
        /// 返回分页实体字典列表集合异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IDictionary<string, object>>> GetResultByPagerDicAsync<T, T1>(Pager<T1> page)
            where T : new()
            where T1 : BasePageCondition, new()
        {
            IEnumerable<IDictionary<string, object>> result = null;
            var FldName = CreateAllEntityDicSql<T>();
            page.Field = FldName;
            using (var readerTask = GetResultByPagerAsync(page))
            {
                using (var reader = await readerTask)
                {
                    var totalCount = 0;
                    result = reader.ReadList(r => r.ReadSelf(), ref totalCount);
                    page.SetTotalCount(totalCount);
                }
            }
            return result;
        }

        /// <summary>
        /// 返回分页数据集
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="page"></param>
        /// <returns></returns>
        public DataSet GetResultByPagerDs<T1>(Pager<T1> page)
            where T1 : BasePageCondition, new()
        {

            var cmdText = "";
            DbParameter[] parameter = null;

            page.Execute();
            var pageIndex = page.Condition.PageIndex;
            var pageSize = page.Condition.PageSize;
            
            cmdText = GetPageCmdText(page.UnionText,
                page.Table,
                page.Field,
                ref pageIndex,
                ref pageSize,
                page.Where.ToString(),
                page.Group,
                page.Order,
                ref parameter,
                page.Parameters.ToArray());
            page.Condition.PageIndex = pageIndex;
            page.Condition.PageSize = pageSize;

            var result = this.GetDataSet(cmdText, parameter);

            var totalCount = 0;
            if (int.TryParse(result.Tables[1].Rows[0]["TotalRecords"].ToString(), out totalCount))
            {
                page.SetTotalCount(totalCount);
            }
            return result;
        }
        #endregion
        #endregion



        #endregion

        

        //#region Transaction
        ///// <summary>
        ///// 获得事务
        ///// </summary>
        ///// <returns></returns>
        //public DbTransaction GetTransaction() 
        //{
        //    return base.GetTransaction();
        //}
        //#endregion

        #region 生成SQL
        /// <summary>
        /// 生成添加SQL语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmdText">执行SQL</param>
        /// <param name="entity">实体对象</param>
        /// <param name="exclusionList">不包括属性名</param>
        /// <returns>返回[T-SQL:Insert]</returns>
        private List<DbParameter> CreateInsertSql<T>(ref StringBuilder cmdText, T entity,ref object newID, params string[] exclusionList)
        {
            var paramerList = new List<DbParameter>();
            var entityType = typeof(T);
            var propertyInfos = entityType.GetProperties();
            var sqlFields = new StringBuilder();
            var sqlValues = new StringBuilder();
            var tableName = string.Empty;
            
            var dataTable = entityType.GetDataTableAttribute(out tableName);

            cmdText = new StringBuilder("");
            
            foreach (var property in propertyInfos)
            {
                //不可读
                if (!property.CanRead ||!property.CanWrite||(exclusionList != null && exclusionList.IsExcluded(property.Name)))
                {
                    continue;
                }

                var fieldName = "";
                object oval = null;
                object genOval=null;

                var propVal = property.GetValue(entity, null);

                var da = property.GetDataFieldAttribute(out fieldName);
                if (da != null)
                {
                    if (da.IsKey && da.IsAuto && da.KeyType == KeyType.SEQ)
                    {
                        ThePagerGenerator.ProcessInsertId<T>(fieldName, ref sqlFields, ref sqlValues);
                        continue;
                    }
                    else if (da.IsKey && da.IsAuto && da.KeyType != KeyType.SEQ)
                    {
                        genOval = propVal;

                        if (genOval != null && !string.IsNullOrEmpty(genOval.ToString()))
                        {
                            try
                            {
                                var currentLongId = GetSingle<long>(genOval);
                                if (currentLongId <= 0)
                                {
                                    genOval = TheGenerator.Generate();
                                }
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            genOval = TheGenerator.Generate();
                        }

                        newID = genOval;
                    }
                }

                oval = (genOval == null ? propVal : genOval);

                if (oval == null && da.DefaultValue != null) {
                    oval = da.DefaultValue;
                }

                if(oval != null){

                    paramerList.Add(base.CreateParameter(base.DbProvider.ParameterPrefix + fieldName, oval));
                    sqlFields.Append(fieldName);
                    sqlFields.Append(",");
                    sqlValues.Append(base.DbProvider.ParameterPrefix);
                    sqlValues.Append(fieldName);
                    sqlValues.Append(",");
                }
            }

            if (sqlFields.Length > 0)
            {
                sqlFields.Length = sqlFields.Length - 1;
            }

            if (sqlValues.Length > 0)
            {
                sqlValues.Length = sqlValues.Length - 1;
            }

            //dataTable.SequenceName

            cmdText.AppendFormat("INSERT INTO {0}({1}) VALUES({2})", tableName, sqlFields, sqlValues);

            return paramerList;
        }
        /// <summary>
        /// 生成Insert SQL语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private string CreateInsertAllSql<T>()
        {
            var entityType = typeof(T);

            var propertyInfos = entityType.GetProperties();
            var sqlFields = new StringBuilder();
            var tableName = string.Empty; ;
            var dataTable = entityType.GetDataTableAttribute(out tableName);


            var cmdText = $"INSERT INTO {tableName} ";

            return cmdText;
        }

        /// <summary>
        /// 生成删除SQL语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmdText">执行SQL</param>
        /// <param name="entity">实体对象</param>
        /// <param name="exclusionList">不包括属性名</param>
        /// <returns>返回[T-SQL:DELETE]</returns>
        private List<DbParameter> CreateDeleteSql<T>(ref string cmdText, T entity, params string[] exclusionList)
        {
            var paramerList = new List<DbParameter>();
            cmdText = "";
            var entityType = typeof(T);//entity.GetType();
            var propertyInfos = entityType.GetProperties();
            var sqlValues = new StringBuilder();
            var tableName = string.Empty; ;
            var dataTable = entityType.GetDataTableAttribute(out tableName);
            
            foreach (var property in propertyInfos)
            {
                //不可读
                if (!property.CanRead || !property.CanWrite ||( exclusionList != null && exclusionList.IsExcluded(property.Name)))
                {
                    continue;
                }

                var fieldName = string.Empty; ;
                object oval = null;
                var datafieldAttribute = property.GetDataFieldAttribute(out fieldName);
                
                oval = property.GetValue(entity, null);
                if (oval != null && oval.ToString() != "")
                {
                    paramerList.Add(base.CreateParameter(base.DbProvider.ParameterPrefix + fieldName, oval));
                    sqlValues.Append(fieldName);
                    sqlValues.Append("=");
                    sqlValues.Append(base.DbProvider.ParameterPrefix);
                    sqlValues.Append(fieldName);
                    sqlValues.Append(",");
                }
            }

            if (sqlValues.Length > 0)
            {
                sqlValues.Length = sqlValues.Length - 1;
            }

            cmdText = $"DELETE FROM {tableName} WHERE {sqlValues}";

            return paramerList;
        }

        /// <summary>
        /// 生成修改SQL语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmdText">执行SQL</param>
        /// <param name="entity">实体对象</param>
        /// <param name="exclusionList">不包括属性名</param>
        /// <returns>返回[T-SQL:UPDATE]</returns>
        private List<DbParameter> CreateUpdateSql<T>(ref string cmdText,T entity, params string[] exclusionList)
        {
            var paramerList = new List<DbParameter>();
            cmdText = "";
            var entityType = typeof(T);// entity.GetType();
            var propertyInfos = entityType.GetProperties();
            var sqlFields = new StringBuilder();
            var sqlValues = new StringBuilder();
            var tableName = string.Empty; ;
            var dataTable = entityType.GetDataTableAttribute(out tableName);
            
            foreach (var property in propertyInfos)
            {
                var aa = property.PropertyType.IsReadablePropertyType();

                //不可读
                if (!property.CanRead || !property.CanWrite || (exclusionList != null && exclusionList.IsExcluded(property.Name)))
                {
                    continue;
                }

                var fieldName = string.Empty; ;
                object oval = null;
                var datafieldAttribute = property.GetDataFieldAttribute(out fieldName);
                if (datafieldAttribute != null)
                {
                    if (datafieldAttribute.IsKey)
                    {

                        sqlValues.Append(fieldName);
                        sqlValues.Append("=");
                        sqlValues.Append(base.DbProvider.ParameterPrefix);
                        sqlValues.Append(fieldName);
                        sqlValues.Append(",");
                        oval = property.GetValue(entity, null);
                        oval = oval == null ? DBNull.Value : oval;
                        paramerList.Add(base.CreateParameter(base.DbProvider.ParameterPrefix + fieldName, oval));
                        continue;
                    }
                }
                oval = property.GetValue(entity, null);

                if (oval == null && datafieldAttribute.DefaultValue != null)
                {
                    oval = datafieldAttribute.DefaultValue;
                }

                if (oval != null)
                {
                    paramerList.Add(base.CreateParameter(base.DbProvider.ParameterPrefix + fieldName, oval));
                    sqlFields.Append(fieldName);
                    sqlFields.Append("=");
                    sqlFields.Append(base.DbProvider.ParameterPrefix);
                    sqlFields.Append(fieldName);
                    sqlFields.Append(",");
                }
            }
            if (sqlFields.Length > 0)
            {
                sqlFields.Length = sqlFields.Length - 1;
            }

            if (sqlValues.Length > 0)
            {
                sqlValues.Length = sqlValues.Length - 1;
            }

            cmdText = $"UPDATE {tableName} SET {sqlFields} WHERE {sqlValues} ";

            return paramerList;
        }
        /// <summary>
        /// 生成修改SQL语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private string CreateUpdateAllSql<T>()
        {
            var entityType = typeof(T);

            var propertyInfos = entityType.GetProperties();
            var sqlFields = new StringBuilder();
            var tableName = string.Empty; ;
            var dataTable = entityType.GetDataTableAttribute(out tableName);


            var cmdText = $"UPDATE {tableName} SET ";

            return cmdText;
        }
        /// <summary>
        /// 生成添加SQL语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CreateSelectAllSql<T>(string top="")
        {
            var entityType = typeof(T);
            var tableName = string.Empty; ;
            var dataTable = entityType.GetDataTableAttribute(out tableName);

            var cmdText = $"SELECT {top} * FROM {tableName} ";
            return cmdText;
        }

        /// <summary>
        /// 生成添加SQL语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CreateSelectAllEntityDicSql<T>(string top="",params string[] exclusionList)
        {
            var entityType = typeof(T);
            var tableName = string.Empty; ;
            var dataTable = entityType.GetDataTableAttribute(out tableName);

            var fields = CreateAllEntityDicSql<T>(exclusionList);
            var cmdText = $"SELECT {top} {fields} FROM {tableName} ";

            return cmdText;
        }

        private string CreateAllEntityDicSql<T>(params string[] exclusionList) 
        {

            var entityType = typeof(T);
            var propertyInfos = entityType.GetProperties();
            var sqlFields = new StringBuilder();
            foreach (var property in propertyInfos)
            {
                //不可读
                if (!property.CanRead || !property.CanWrite || (exclusionList != null && exclusionList.IsExcluded(property.Name)))
                {
                    continue;
                }

                var fieldName = string.Empty; ;
                var datafieldAttribute = property.GetDataFieldAttribute(out fieldName);

                sqlFields.Append(fieldName);
                sqlFields.Append(" AS ");
                sqlFields.Append(property.Name);

                sqlFields.Append(",");
            }

            if (sqlFields.Length > 0)
            {
                sqlFields.Length = sqlFields.Length - 1;
            }
            return sqlFields.ToString();
        }
        /// <summary>
        /// 删除所有该表的数据的sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private string CreateDeleteAllSql<T>()
        {
            var entityType = typeof(T);

            var propertyInfos = entityType.GetProperties();
            var sqlFields = new StringBuilder();
            var tableName = string.Empty; ;
            var dataTable = entityType.GetDataTableAttribute(out tableName);


            var cmdText = $"DELETE FROM {tableName} ";

            return cmdText;
        }

        /// <summary>
        /// 生成数据库处理（查询和删除）语句
        /// </summary>
        /// <param name="cmdText">要生成的sql语句</param>
        /// <param name="isDel">true为删除false为查询</param>
        /// <param name="entityType">实体类型</param>
        /// <param name="ID">主键</param>
        /// <returns>参数</returns>
        private List<DbParameter> CreateDelSql(ref string cmdText, bool isDel, Type entityType, object ID, params string[] exclusionList)
        {
            var paramerList = new List<DbParameter>();
            cmdText = "";
            var propertyInfos = entityType.GetProperties();
            var tableName=string.Empty;;
            var dataTable = entityType.GetDataTableAttribute(out tableName);
            
            var fieldName = "";
            var sqlValues = "";
            object oval = ID == null ? DBNull.Value : ID;
            foreach (var property in propertyInfos)
            {
                //不可读
                if (!property.CanRead || !property.CanWrite || (exclusionList != null && exclusionList.IsExcluded(property.Name)))
                {
                    continue;
                }

                var df = property.GetDataFieldAttribute(out fieldName);
                if (df != null && df.IsKey)
                {
                    paramerList.Add(base.CreateParameter(base.DbProvider.ParameterPrefix + fieldName, oval));
                    break;
                }
            }
            if (fieldName == "")
            {
                fieldName = propertyInfos[0].Name;
                paramerList.Add(base.CreateParameter(base.DbProvider.ParameterPrefix + fieldName, oval));
            }
            sqlValues = fieldName + "=" + base.DbProvider.ParameterPrefix + fieldName;

            if (isDel)
            {
                cmdText = $"DELETE FROM {tableName} WHERE {sqlValues}";
            }
            else
            {
                cmdText = $"SELECT * FROM {tableName} WHERE {sqlValues}";
            }

            return paramerList;
        }

        /// <summary>
        /// 遍历对象并附加出错日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="entity"></param>
        /// <param name="exclusionList"></param>
        private void ForeachDataAddLogError<T>(IDataReader reader, ref T entity, params string[] exclusionList) where T : new()
        {
            if (entity==null) {
                entity = new T();
            }
            var entityType = typeof(T); //entity.GetType();
            var propertyInfos = entityType.GetProperties();
            var fieldName=string.Empty;
            foreach (var property in propertyInfos)
            {
                //不可读
                if (!property.CanRead || !property.CanWrite || (exclusionList != null && exclusionList.IsExcluded(property.Name)))
                {
                    continue;
                }

                var datafieldAttribute = property.GetDataFieldAttribute(out fieldName);
                try
                {
                    if (!(reader[fieldName] is DBNull))
                    {
                        var propType=property.PropertyType;
                        property.SetValue(entity, reader[fieldName].ChangeType(propType), null);
                    }

                }
                catch (IndexOutOfRangeException iofex)
                {
                    _logger.Error("ForeachDataAddLogError_iofex:" + iofex.Message, iofex);
                }
                catch (Exception ex)
                {
                    _logger.Error("ForeachDataAddLogError:" + ex.Message, ex);
                    if (!reader.IsClosed) { reader.Close(); }
                    throw;
                }
            }
        }

        /// <summary>
        /// 遍历对象并附加出错日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="entity"></param>
        /// <param name="exclusionList"></param>
        private void ForeachDicAddLogError(IDataReader reader, ref Dictionary<string,object> entity, params string[] exclusionList)
        {
                try
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var name = reader.GetName(i);
                        //不可读
                        if ((exclusionList != null && exclusionList.IsExcluded(name)))
                        {
                            continue;
                        }
                        entity.Add(name, reader.GetValue(i));
                    }

                }
                catch (Exception ex)
                {
                    _logger.Error("ForeachDicAddLogError:" + ex.Message, ex);
                    if (!reader.IsClosed) { reader.Close(); }
                    throw;
                }
        }

        /// <summary>
        /// 遍历对象并附加出错日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="entity"></param>
        /// <param name="exclusionList"></param>
        private void ForeachDicAddLogErrorStr(IDataReader reader, ref Dictionary<string, string> entity, params string[] exclusionList)
        {
            try
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);
                    //不可读
                    if ((exclusionList != null && exclusionList.IsExcluded(name)))
                    {
                        continue;
                    }
                    entity.Add(name, (reader.GetValue(i) == DBNull.Value ? "" : reader.GetValue(i).ToString()));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ForeachDicAddLogErrorStr:" + ex.Message, ex);
                if (!reader.IsClosed) { reader.Close(); }
                throw;
            }
        }
        #endregion
        
    }
}
