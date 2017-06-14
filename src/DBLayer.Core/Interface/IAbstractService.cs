using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DBLayer.Core.Interface
{
    public interface IAbstractService
    {
        #region 增
        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        object InsertEntity<T>(Expression<Func<T>> expression, DbTransaction trans = null) where T : new();
        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        Task<object> InsertEntityAsync<T>(Expression<Func<T>> expression, DbTransaction trans = null) where T : new();
        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        R InsertEntity<T, R>(Expression<Func<T>> expression, DbTransaction trans = null)
            where T : new();
        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        Task<R> InsertEntityAsync<T, R>(Expression<Func<T>> expression, DbTransaction trans = null)
            where T : new();
        #endregion
        #region 删
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        int DeleteEntity<T>(Expression<Func<T, bool>> where, DbTransaction trans = null) where T : new();

        /// <summary>
        /// 删除数据异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        Task<int> DeleteEntityAsync<T>(Expression<Func<T, bool>> where, DbTransaction trans = null) where T : new();

        #endregion
        #region 改
        /// <summary>
        /// 更新操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="where"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        int UpdateEntity<T>(Expression<Func<T>> expression, Expression<Func<T, bool>> where, DbTransaction trans = null) where T : new();

        /// <summary>
        /// 更新操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="where"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        Task<int> UpdateEntityAsync<T>(Expression<Func<T>> expression, Expression<Func<T, bool>> where, DbTransaction trans = null) where T : new();
        
        #endregion
        #region 查
        /// <summary>
        /// 获取 T 单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        T GetEntity<T>(Expression<Func<T, bool>> where, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null) where T : new();
        /// <summary>
        /// 获取 T 单个实体异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="order"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        Task<T> GetEntityAsync<T>(Expression<Func<T, bool>> where, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null) where T : new();

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        IList<T> GetEntityList<T>(Expression<Func<T, bool>> where = null, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null, int? top = null) where T : new();

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        Task<IList<T>> GetEntityListAsync<T>(Expression<Func<T, bool>> where = null, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null, int? top = null) where T : new();
        
        #endregion

        /// <summary>
        /// 获得事务
        /// </summary>
        /// <returns></returns>
        DbTransaction GetTransaction();
    }
}
