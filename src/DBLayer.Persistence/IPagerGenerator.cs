using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBLayer.Core;
using DBLayer.Persistence.Data;
using System.Data;

namespace DBLayer.Persistence
{
    public interface IPagerGenerator
    {
        /// <summary>
        /// 查询所有数据-不包含字段*
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSource"></param>
        /// <param name="paramerList"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        StringBuilder GetSelectCmdText<T>(DataSource dataSource, ref List<DbParameter> paramerList, StringBuilder whereStr, StringBuilder orderStr, int? top = null);
        /// <summary>
        /// 查询所有数据-包含字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSource"></param>
        /// <param name="paramerList"></param>
        /// <param name="top"></param>
        /// <param name="exclusionList"></param>
        /// <returns></returns>
        StringBuilder GetSelectDictionaryCmdText<T>(DataSource dataSource, ref List<DbParameter> paramerList, StringBuilder whereStr, StringBuilder orderStr , int? top = null, params string[] exclusionList);
         /// <summary>
        /// 在insert中id自动编号的处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <param name="sbField"></param>
        /// <param name="sbValue"></param>
        void ProcessInsertId<T>(string fieldName, ref StringBuilder sbField, ref StringBuilder sbValue);
        /// <summary>
        /// 执行Insert
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="insertCmd">执行命令</param>
        /// <param name="paramerList">参数列表</param>
        /// <param name="trans">会话</param>
        /// <returns>Id</returns>
        object InsertExecutor<T>(DataSource dataSource, StringBuilder insertCmd, List<DbParameter> paramerList, DbTransaction trans = null);
        /// <summary>
        /// 执行Insert
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="insertCmd">执行命令</param>
        /// <param name="paramerList">参数列表</param>
        /// <param name="trans">会话</param>
        /// <returns>Id</returns>
        Task<object> InsertExecutorAsync<T>(DataSource dataSource, StringBuilder insertCmd, List<DbParameter> paramerList, DbTransaction trans = null);
        /// <summary>
        /// 查询条件 InFunc
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        StringBuilder GetInFunc(Func<StringBuilder> left, Func<StringBuilder> right);
        /// <summary>
        /// 查询条件 NotInFunc
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        StringBuilder GetNotInFunc(Func<StringBuilder> left, Func<StringBuilder> right);

        /// <summary>
        /// 生成分页sql
        /// </summary>
        /// <param name="UnionText"></param>
        /// <param name="TableName"></param>
        /// <param name="PrimaryKey"></param>
        /// <param name="FldName"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="Filter"></param>
        /// <param name="Group"></param>
        /// <param name="Sort"></param>
        /// <param name="parameter"></param>
        /// <param name="paramers"></param>
        /// <returns></returns>
        StringBuilder GetPageCmdText(DataSource dataSource, string UnionText, string TableName, string PrimaryKey, string FldName, ref int? PageIndex, ref int? PageSize, string Filter, string Group, string Sort, ref DbParameter[] parameter, params DbParameter[] paramers);


    }
}
