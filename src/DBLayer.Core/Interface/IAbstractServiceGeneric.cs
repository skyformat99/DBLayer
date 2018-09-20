using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DBLayer.Core.Interface
{

    public interface IAbstractService<T> : IAbstractService where T : new()
    {
        #region 增
        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        object InsertEntity(Expression<Func<T>> expression, DbTransaction trans = null);
        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        Task<object> InsertEntityAsync(Expression<Func<T>> expression, DbTransaction trans = null);
        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        R InsertEntity<R>(Expression<Func<T>> expression, DbTransaction trans = null);
        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        Task<R> InsertEntityAsync<R>(Expression<Func<T>> expression, DbTransaction trans = null);
        #endregion
        #region 删
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="where"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        int DeleteEntity(Expression<Func<T, bool>> where, DbTransaction trans = null);

        /// <summary>
        /// 删除数据异步
        /// </summary>
        /// <param name="where"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        Task<int> DeleteEntityAsync(Expression<Func<T, bool>> where, DbTransaction trans = null);

        #endregion
        #region 改
        /// <summary>
        /// 更新操作
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="where"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        int UpdateEntity(Expression<Func<T>> expression, Expression<Func<T, bool>> where, DbTransaction trans = null);

        /// <summary>
        /// 更新操作
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="where"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        Task<int> UpdateEntityAsync(Expression<Func<T>> expression, Expression<Func<T, bool>> where, DbTransaction trans = null);

        #endregion
        #region 查
        /// <summary>
        /// 获取 T 单个实体
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        T GetEntity(Expression<Func<T, bool>> where, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null);
        /// <summary>
        /// 获取 T 单个实体异步
        /// </summary>
        /// <param name="where"></param>
        /// <param name="order"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        Task<T> GetEntityAsync(Expression<Func<T, bool>> where, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null);

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <param name="where"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        IList<T> GetEntityList(Expression<Func<T, bool>> where = null, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null, int? top = null);

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <param name="where"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        Task<IList<T>> GetEntityListAsync(Expression<Func<T, bool>> where = null, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null, int? top = null);
        /// <summary>
        /// 获得单个实体字典
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        IDictionary<string, object> GetEntityDic(Expression<Func<T, bool>> where, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null);

        /// <summary>
        /// 获得单个实体字典异步
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        Task<IDictionary<string, object>> GetEntityDicAsync(Expression<Func<T, bool>> where, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null);
        /// <summary>
        /// 获得实体字典列表
        /// </summary>
        /// <param name="where"></param>
        /// <param name="exclusionList"></param>
        /// <returns></returns>
        IList<IDictionary<string, object>> GetEntityDicList(Expression<Func<T, bool>> where = null, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null, int? top = null, params string[] exclusionList);
        /// <summary>
        /// 获得实体字典列表
        /// </summary>
        /// <param name="where"></param>
        /// <param name="exclusionList"></param>
        /// <returns></returns>
        Task<IList<IDictionary<string, object>>> GetEntityDicListAsync(Expression<Func<T, bool>> where = null, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null, int? top = null, params string[] exclusionList);
        /// <summary>
        /// 获得单个实体字符串字典
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        IDictionary<string, string> GetEntityDicStr(Expression<Func<T, bool>> where, Expression<Func<OrderExpression<T>, object>> order, DbTransaction trans = null);
        /// <summary>
        /// 获得单个实体字符串字典
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        Task<IDictionary<string, string>> GetEntityDicStrAsync(Expression<Func<T, bool>> where, Expression<Func<OrderExpression<T>, object>> order, DbTransaction trans = null);
        /// <summary>
        /// 获得实体字典字符串列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="exclusionList"></param>
        /// <returns></returns>
        IList<IDictionary<string, string>> GetEntityDicStrList(Expression<Func<T, bool>> where = null, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null, int? top = null, params string[] exclusionList);
        /// <summary>
        /// 获得实体字典字符串列表
        /// </summary>
        /// <param name="func"></param>
        /// <param name="exclusionList"></param>
        /// <returns></returns>
        Task<IList<IDictionary<string, string>>> GetEntityDicStrListAsync(Expression<Func<T, bool>> where = null, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null, int? top = null, params string[] exclusionList);
        #endregion
    }
}
