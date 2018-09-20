using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DBLayer.Core.Interface
{
    public interface IAbstractService
    {
        #region insert
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
        #region delete
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
        #region update
        /// <summary>
        /// update entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="where"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        int UpdateEntity<T>(Expression<Func<T>> expression, Expression<Func<T, bool>> where, DbTransaction trans = null) where T : new();

        /// <summary>
        /// update entity async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="where"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        Task<int> UpdateEntityAsync<T>(Expression<Func<T>> expression, Expression<Func<T, bool>> where, DbTransaction trans = null) where T : new();

        #endregion
        #region select
        /// <summary>
        /// get only one entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        T GetEntity<T>(Expression<Func<T, bool>> where, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null) where T : new();
        /// <summary>
        /// get only one entity async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="order"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        Task<T> GetEntityAsync<T>(Expression<Func<T, bool>> where, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null) where T : new();

        /// <summary>
        /// find a list of entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        IList<T> GetEntityList<T>(Expression<Func<T, bool>> where = null, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null, int? top = null) where T : new();

        /// <summary>
        /// find a list of entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        Task<IList<T>> GetEntityListAsync<T>(Expression<Func<T, bool>> where = null, Expression<Func<OrderExpression<T>, object>> order = null, DbTransaction trans = null, int? top = null) where T : new();
        
        #endregion

        /// <summary>
        /// get transaction
        /// </summary>
        /// <returns></returns>
        DbTransaction GetTransaction();
    }
}
