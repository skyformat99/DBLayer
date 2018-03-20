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
    public abstract class AbstractService<T> : AbstractService, IAbstractService<T> where T:new()
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public AbstractService():base() { }
        public AbstractService(DataSource dataSource):base(dataSource){}

        #region public method
        
        #region GetEntity
        
        /// <summary>
        /// 获取 T 单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public T GetEntity(Expression<Func<T,bool>> where,Expression<Func<OrderExpression<T>, object>> order=null,DbTransaction trans=null)
        {
            var entity = base.GetEntity<T>(where, order, trans);
            return entity;
        }
        /// <summary>
        /// 获取 T 单个实体
        /// </summary>
        /// <typeparam name="T">实体(泛型)类</typeparam>
        /// <param name="paramers">参数数组</param>
        /// <returns>T 实体</returns>
        public T GetEntity(string cmdText, params DbParameter[] paramers)
        {
            T entity = GetEntity<T>(cmdText, paramers);

            return entity;
        }

        /// <summary>
        /// 获取 T 单个实体
        /// </summary>
        /// <typeparam name="T">实体(泛型)类</typeparam>
        /// <param name="paramers">参数数组</param>
        /// <returns>T 实体</returns>
        public T GetEntity(string cmdText, DbTransaction trans = null, params DbParameter[] paramers)
        {
            T entity = GetEntity<T>(cmdText, trans, paramers);

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
        public T GetEntity(string cmdText, object obj , DbTransaction trans = null)
        {
            T entity = GetEntity<T>(cmdText, obj, trans);
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
        public T GetEntity(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] paramers)
        {
            T entity = GetEntity<T>(cmdText, trans, commandType,  paramers);
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
        public async Task<T> GetEntityAsync(Expression<Func<T, bool>> where, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null)
        {
            var entity = await GetEntityAsync<T>(where, order, trans);
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
        public async Task<T> GetEntityAsync(string cmdText, object obj, DbTransaction trans = null)
        {
            var entity = await GetEntityAsync<T>(cmdText, obj, trans);
            return entity;
        }
        /// <summary>
        /// 获取 T 单个实体异步
        /// </summary>
        /// <typeparam name="T">实体(泛型)类</typeparam>
        /// <param name="paramers">参数数组</param>
        /// <returns>T 实体</returns>
        public async Task<T> GetEntityAsync(string cmdText, params DbParameter[] paramers)
        {
            var entity = await GetEntityAsync<T>(cmdText, paramers);

            return entity;
        }

        /// <summary>
        /// 获取 T 单个实体异步
        /// </summary>
        /// <typeparam name="T">实体(泛型)类</typeparam>
        /// <param name="paramers">参数数组</param>
        /// <returns>T 实体</returns>
        public async Task<T> GetEntityAsync(string cmdText, DbTransaction trans = null, params DbParameter[] paramers)
        {
            var entity = await GetEntityAsync<T>(cmdText, trans, paramers);

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
        public async Task<T> GetEntityAsync(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] paramers)
        {
            var entity = await GetEntityAsync<T>(cmdText, trans , commandType , paramers);

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
        public IDictionary<string, object> GetEntityDic(Expression<Func<T, bool>> where, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null)
        {
            var result = GetEntityDic<T>(where, order, trans);
            return result;
        }

        /// <summary>
        /// 获得单个实体字典异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public async Task<IDictionary<string, object>> GetEntityDicAsync(Expression<Func<T, bool>> where, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null)
        {
            var result =await GetEntityDicAsync<T>(where, order, trans);
            return result;
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
        public IList<IDictionary<string, object>> GetEntityDicList(Expression<Func<T, bool>> where = null, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null, int? top = null, params string[] exclusionList)
        {
            var result =base.GetEntityDicList<T>(where, order, trans,top, exclusionList);
            return result;
        }

        /// <summary>
        /// 获得实体字典列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="exclusionList"></param>
        /// <returns></returns>
        public async Task<IList<IDictionary<string, object>>> GetEntityDicListAsync(Expression<Func<T, bool>> where = null, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null, int? top = null, params string[] exclusionList)
        {
            var result =await base.GetEntityDicListAsync<T>(where, order, trans, top, exclusionList);
            return result;
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
        public IList<T> GetEntityList(Expression<Func<T, bool>> where = null,Expression<Func<OrderExpression<T>, object>> order=null, DbTransaction trans = null, int? top = null) 
        {
            var result = base.GetEntityList<T>(where, order, trans, top);
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
        public IList<T> GetEntityList(string cmdText, params DbParameter[] paramers) 
        {
            var entityList = GetEntityList<T>(cmdText, paramers);

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
        public IList<T> GetEntityList(string cmdText, DbTransaction trans = null, params DbParameter[] paramers) 
        {
            var entityList = GetEntityList<T>(cmdText, trans, paramers);

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
        public IList<T> GetEntityList(string cmdText, object obj, DbTransaction trans = null) 
        {
            var entityList = GetEntityList<T>(cmdText, obj, trans);

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
        public IList<T> GetEntityList(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] paramers) 
        {
            var entityList = GetEntityList<T>(cmdText, trans, commandType, paramers);

            return entityList;
        }

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public async Task<IList<T>> GetEntityListAsync(Expression<Func<T, bool>> where = null, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null, int? top = null) 
        {
            var entityList = await GetEntityListAsync<T>(where, order, trans, top);

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
        public async Task<IList<T>> GetEntityListAsync(string cmdText, params DbParameter[] paramers) 
        {
            var result = await GetEntityListAsync<T>(cmdText, paramers);

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
        public async Task<IList<T>> GetEntityListAsync(string cmdText,DbTransaction trans = null, params DbParameter[] paramers) 
        {
            var result = await GetEntityListAsync<T>(cmdText, trans, paramers);

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
        public async Task<IList<T>> GetEntityListAsync(string cmdText, object obj, DbTransaction trans = null) 
        {
            var result =await GetEntityListAsync<T>(cmdText, obj, trans);

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
        public async Task<IList<T>> GetEntityListAsync(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] paramers) 
        {
            var result = await GetEntityListAsync<T>(cmdText, trans, commandType, paramers);

            return result;
        }
        #endregion

        #region GetScalar
        /// <summary>
        /// 返回首行首列
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>返回首行首列</returns>
        public T GetSingle(object obj)
        {
            var result = GetSingle<T>(obj);

            return result;
        }
        /// <summary>
        /// 返回首行首列
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>返回首行首列</returns>
        public async Task<T> GetSingleAsync<T>(Task<object> obj)
        {
            var result = await GetSingleAsync<T>(obj);

            return result;
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
        public IList<T> GetList(string cmdText, params DbParameter[] paramers)
        {
            var result= GetList<T>(cmdText, paramers);
            return result;
        }
        /// <summary>
        /// 获取 T 类型的数据集
        /// </summary>
        /// <typeparam name="T">数据类型（泛型）</typeparam>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>数据集合</returns>
        public IList<T> GetList(string cmdText,DbTransaction trans = null, params DbParameter[] paramers)
        {
            var result = GetList<T>(cmdText, trans, paramers);
            return result;
        }

        /// <summary>
        /// 获取 T 类型的数据集
        /// </summary>
        /// <typeparam name="T">数据类型（泛型）</typeparam>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>数据集合</returns>
        public IList<T> GetList(string cmdText, object obj, DbTransaction trans = null)
        {
            var result = GetList<T>(cmdText, obj, trans);
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
        public IList<T> GetList(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] paramers)
        {
            var result = GetList<T>(cmdText, trans, commandType, paramers);
            return result;
        }

        /// <summary>
        /// 获取 T 类型的数据集
        /// </summary>
        /// <typeparam name="T">数据类型（泛型）</typeparam>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>数据集合</returns>
        public async Task<IList<T>> GetListAsync(string cmdText, params DbParameter[] paramers)
        {
            var result= await GetListAsync<T>(cmdText,paramers);
            return result;
        }
        /// <summary>
        /// 获取 T 类型的数据集
        /// </summary>
        /// <typeparam name="T">数据类型（泛型）</typeparam>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>数据集合</returns>
         public async Task <IList<T>> GetListAsync(string cmdText, DbTransaction trans = null, params DbParameter[] paramers)
        {
            var result = await GetListAsync<T>(cmdText, trans, paramers);
            return result;
        }
        /// <summary>
        /// 获取 T 类型的数据集
        /// </summary>
        /// <typeparam name="T">数据类型（泛型）</typeparam>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="paramers">参数数组</param>
        /// <returns>数据集合</returns>
         public async Task <IList<T>> GetListAsync(string cmdText, object obj, DbTransaction trans = null)
        {
            var result = await GetListAsync<T>(cmdText, obj, trans);
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
        public async Task<IList<T>> GetListAsync(string cmdText, DbTransaction trans = null, CommandType commandType = CommandType.Text, params DbParameter[] paramers)
        {
            var result = await GetListAsync(cmdText, trans, commandType, paramers);
            return result;
        }
        #endregion

        #region GetEntityDicStr
        /// <summary>
        /// 获得单个实体字符串字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public IDictionary<string, string> GetEntityDicStr(Expression<Func<T, bool>> where,Expression<Func<OrderExpression<T>, object>> order, DbTransaction trans = null) 
        {

            var result = GetEntityDicStr<T>(where, order, trans);
            return result;
        }
        
        /// <summary>
        /// 获得单个实体字符串字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
         public async Task <IDictionary<string, string>> GetEntityDicStrAsync(Expression<Func<T, bool>> where, Expression<Func<OrderExpression<T>, object>> order, DbTransaction trans = null) 
        {
            var result = await GetEntityDicStrAsync<T>(where, order, trans);
            return result;
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
        public IList<IDictionary<string, string>> GetEntityDicStrList(Expression<Func<T, bool>> where = null,Expression<Func<OrderExpression<T>, object>> order=null, DbTransaction trans = null, int? top = null, params string[] exclusionList) 
        {

            var result = GetEntityDicStrList<T>(where, order, trans, top, exclusionList);
            return result;
        }
        /// <summary>
        /// 获得实体字典字符串列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="exclusionList"></param>
        /// <returns></returns>
         public async Task <IList<IDictionary<string, string>>> GetEntityDicStrListAsync(Expression<Func<T, bool>> where = null, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null, int? top = null, params string[] exclusionList) 
        {

            var result = await GetEntityDicStrListAsync<T>(where, order, trans, top, exclusionList);
            return result;
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
        public object InsertEntity(Expression<Func<T>> expression, DbTransaction trans = null) 
        {
            var result = InsertEntity<T>(expression, trans);
            return result;
        }
        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="expression"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public R InsertEntity<R>(Expression<Func<T>> expression, DbTransaction trans = null) 
        {
            var result = InsertEntity<T,R>(expression, trans);
            return result;
        }
        /// <summary>
        /// 通过实体类型 T 向表中增加一条记录
        /// </summary>
        /// <typeparam name="T">实体泛型(类)</typeparam>
        /// <param name="entity">实体对象</param>
        /// <param name="exclusionList"></param>
        /// <returns>所新增记录的 ID,如果返回 -1 则表示增加失败</returns>
        public object InsertEntity(T entity, DbTransaction trans = null, params string[] exclusionList) 
        {
            var result = InsertEntity<T,object>(entity, trans, exclusionList);
            return result;
        }
        public R InsertEntity<R>(T entity, DbTransaction trans = null, params string[] exclusionList)
            
        {
            var result = InsertEntity<T, R>(entity, trans, exclusionList);
            return result;
        }
        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
         public async Task <object> InsertEntityAsync(Expression<Func<T>> expression, DbTransaction trans = null) 
            
        {
            var result = await InsertEntityAsync<T,object>(expression, trans);
            return result;
        }
        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="expression"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
         public async Task <R> InsertEntityAsync<R>(Expression<Func<T>> expression, DbTransaction trans = null)
            
        {
            var result = await InsertEntityAsync<T,R>(expression, trans);
            return result;
        }
        /// <summary>
        /// 通过实体类型 T 向表中增加一条记录
        /// </summary>
        /// <typeparam name="T">实体泛型(类)</typeparam>
        /// <param name="entity">实体对象</param>
        /// <param name="exclusionList"></param>
        /// <returns>所新增记录的 ID,如果返回 -1 则表示增加失败</returns>
        public async Task<object> InsertEntityAsync(T entity, DbTransaction trans = null, params string[] exclusionList)
        {
            var result = await InsertEntityAsync<T,object>(entity, trans, exclusionList);
            return result;
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
         public async Task <R> InsertEntityAsync<R>(T entity, DbTransaction trans = null, params string[] exclusionList)
            
        {
            var result = await InsertEntityAsync<T,R>(entity, trans, exclusionList);
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
        public int UpdateEntity(Expression<Func<T>> expression, Expression<Func<T, bool>> where, DbTransaction trans = null) 
        {
            var result = UpdateEntity<T>(expression, where, trans);
            return result;
        }

        /// <summary>
        /// 修改 T 实体
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>影响数据条数</returns>
        public int UpdateEntity(T entity,DbTransaction trans=null, params string[] exclusionList)
        {
            var result = UpdateEntity<T>(entity, trans, exclusionList);
            return result;
        }

        /// <summary>
        /// 更新操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="where"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
         public async Task <int> UpdateEntityAsync(Expression<Func<T>> expression, Expression<Func<T, bool>> where, DbTransaction trans = null) 
        {
            var result = await UpdateEntityAsync<T>(expression, where, trans);
            return result;
        }

        /// <summary>
        /// 修改 T 实体
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>影响数据条数</returns>
         public async Task <int> UpdateEntityAsync(T entity, DbTransaction trans = null, params string[] exclusionList)
        {
            var result = await UpdateEntityAsync<T>(entity, trans, exclusionList);
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
        public int DeleteEntity(Expression<Func<T, bool>> where, DbTransaction trans = null) 
        {
            var result = DeleteEntity<T>(where, trans);
            return result;
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <param name="ID">主键</param>
        /// <returns>影响的条数</returns>
        public int DeleteEntity(object ID,DbTransaction trans=null, params string[] exclusionList)
        {
            var result = DeleteEntity<T>(ID, trans, exclusionList);
            return result;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>影响数据条数</returns>
        public int DeleteEntity(T entity,DbTransaction trans=null, params string[] exclusionList)
        {
            var result = DeleteEntity<T>(entity, trans, exclusionList);
            return result;
        }

        /// <summary>
        /// 删除数据异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
         public async Task <int> DeleteEntityAsync(Expression<Func<T, bool>> where, DbTransaction trans = null) 
        {
            var result = await  DeleteEntityAsync<T>(where, trans);
            return result;
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <param name="ID">主键</param>
        /// <returns>影响的条数</returns>
         public async Task <int> DeleteEntityAsync(object ID, DbTransaction trans = null, params string[] exclusionList)
        {
            var result = await DeleteEntityAsync<T>(ID, trans, exclusionList);
            return result;

        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>影响数据条数</returns>
         public async Task <int> DeleteEntityAsync(T entity, DbTransaction trans = null, params string[] exclusionList)
        {
            var result = await DeleteEntityAsync<T>(entity, trans, exclusionList);
            return result;
        }
        #endregion

        

        #endregion
        
        #region 分页

        #region pager
    

        #region pager result list
        
        
        /// <summary>
        /// 返回分页实体列表集合
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetResultByPager<T1>(Pager<T1> page)
            
            where T1 : BasePageCondition, new()
        {
            var result = GetResultByPager<T,T1>(page);
            return result;
        }

        /// <summary>
        /// 返回分页实体列表集合异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetResultByPageAsync<T1>(Pager<T1> page)
            
            where T1 : BasePageCondition, new()
        {
            var result = await GetResultByPageAsync<T, T1>(page);
            return result;
        }

        /// <summary>
        /// 返回分页实体字典列表集合异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IDictionary<string, object>>> GetResultByPagerDicAsync<T1>(Pager<T1> page)
            
            where T1 : BasePageCondition, new()
        {
            var result = await GetResultByPagerDicAsync<T, T1>(page);
            return result;
        }
        #endregion
        #endregion



        #endregion

    }
}
