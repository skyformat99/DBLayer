﻿using DBLayer.Core;
using DBLayer.Core.Interface;
using DBLayer.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLayer.Persistence.PagerGenerator
{
    public class MySqlPagerGenerator : IPagerGenerator
    {
        /// <summary>
        /// 查询所有数据-不包含字段*
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSource"></param>
        /// <param name="paramerList"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public StringBuilder GetSelectCmdText<T>(IDataSource dataSource, ref List<DbParameter> paramerList, StringBuilder whereStr, StringBuilder orderStr, int? top = null)
        {
            var cmdText = new StringBuilder();
            var topStr = new StringBuilder();
            if (top != null && top.Value > 0)
            {
                var topparameter = dataSource.DbProvider.ParameterPrefix + "topParameter";
                topStr.AppendFormat("LIMIT 0,{0}", topparameter);
                paramerList.Add(dataSource.CreateParameter(topparameter, top.Value));
            }

            var entityType = typeof(T);
            var tableName = string.Empty;

            var dataTable = entityType.GetDataTableAttribute(out tableName);

            cmdText.AppendFormat("SELECT * FROM {0} {2} {3} {1} ", tableName, topStr, whereStr, orderStr);

            return cmdText;
        }
        /// <summary>
        /// 查询所有数据-包含字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSource"></param>
        /// <param name="paramerList"></param>
        /// <param name="top"></param>
        /// <param name="exclusionList"></param>
        /// <returns></returns>
        public StringBuilder GetSelectDictionaryCmdText<T>(IDataSource dataSource, ref List<DbParameter> paramerList, StringBuilder whereStr, StringBuilder orderStr, int? top = null, params string[] exclusionList)
        {
            var cmdText = new StringBuilder();
            var topStr = new StringBuilder();
            if (top != null && top.Value > 0)
            {
                var topparameter = dataSource.DbProvider.ParameterPrefix + "topParameter";
                topStr.AppendFormat("LIMIT 0,{0}topParameter", topparameter);
                paramerList.Add(dataSource.CreateParameter(topparameter, top.Value));
            }

            var entityType = typeof(T);
            var tableName = string.Empty;

            var dataTable = entityType.GetDataTableAttribute(out tableName);
            var fields = CreateAllEntityDicSql<T>(exclusionList);

            cmdText.AppendFormat("SELECT {1} FROM {0} {3} {4} {2} ", tableName, fields, topStr, whereStr, orderStr);

            return cmdText;
        }
        /// <summary>
        /// 在insert中id自动编号的处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <param name="sbField"></param>
        /// <param name="sbValue"></param>
        public void ProcessInsertId<T>(string fieldName, ref StringBuilder sbField, ref StringBuilder sbValue)
        {
        }
        /// <summary>
        /// 执行Insert
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="insertCmd">执行命令</param>
        /// <param name="paramerList">参数列表</param>
        /// <param name="trans">会话</param>
        /// <returns>Id</returns>
        public object InsertExecutor<T>(IDataSource dataSource, StringBuilder insertCmd, List<DbParameter> paramerList, DbTransaction trans = null)
        {
            var cmdText = new StringBuilder();
            cmdText.AppendFormat("{0};{1}", insertCmd, dataSource.DbProvider.SelectKey);

            var newID = dataSource.ExecuteScalar(cmdText.ToString(), trans,  CommandType.Text, paramerList.ToArray());

            return newID;
        }
        /// <summary>
        /// 执行Insert
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="insertCmd">执行命令</param>
        /// <param name="paramerList">参数列表</param>
        /// <param name="trans">会话</param>
        /// <returns>Id</returns>
        public Task<object> InsertExecutorAsync<T>(IDataSource dataSource, StringBuilder insertCmd, List<DbParameter> paramerList, DbTransaction trans = null)
        {

            var cmdText = new StringBuilder();
            cmdText.AppendFormat("{0};{1}", insertCmd, dataSource.DbProvider.SelectKey);

            var newID = dataSource.ExecuteScalarAsync(cmdText.ToString(), trans,  CommandType.Text, paramerList.ToArray());

            return newID;
        }
        /// <summary>
        /// 查询条件 InFunc
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public StringBuilder GetInFunc(Func<StringBuilder> left, Func<StringBuilder> right)
        {
            var leftString = left();
            var rightString = right();

            var result = new StringBuilder();
            result.AppendFormat("{0} IN ({1})", leftString, rightString);
            return result;
        }
        /// <summary>
        /// 查询条件 NotInFunc
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public StringBuilder GetNotInFunc(Func<StringBuilder> left, Func<StringBuilder> right)
        {
            var leftString = left();
            var rightString = right();
            var result = new StringBuilder();
            result.AppendFormat("{0} NOT IN ({1})", leftString, rightString);
            return result;
        }
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
        public StringBuilder GetPageCmdText(IDataSource dataSource, string UnionText, string TableName, string FldName, ref int? PageIndex, ref int? PageSize, string Filter, string Group, string Sort, ref DbParameter[] parameter, params DbParameter[] paramers)
        {
            var cmdText = new StringBuilder();
            var strPageSize = -1;
            var strStartRow = -1;
            var strEndRow = -1;
            var strFilter = "";
            var strGroup = "";
            PageIndex = PageIndex ?? 1;
            PageSize = PageSize ?? 20;

            if (PageIndex <= 0)
            {
                PageIndex = 1;
            }
            if (PageSize <= 0)
            {
                PageSize = 20;
            }

            strPageSize = PageSize.Value;
            strStartRow = ((PageIndex - 1) * PageSize + 1).Value;
            strEndRow = (PageIndex * PageSize).Value;

            if (!string.IsNullOrEmpty(Filter))
            {
                strFilter = " WHERE " + Filter + " ";
            }

            if (!string.IsNullOrEmpty(Group))
            {
                strGroup = " GROUP BY " + Group + " ";
            }

            if (!string.IsNullOrEmpty(Sort))
            {
                strGroup = " ORDER BY " + Sort + " ";
            }

            if (!string.IsNullOrWhiteSpace(UnionText))
            {
                TableName = string.Format("({0}){1}", UnionText, TableName);
            }

            cmdText.AppendFormat("SELECT {0} FROM {1} {2} {3} {4} LIMIT {5}strStartRow,{5}strEndRow ;",
                FldName, TableName, strFilter, strGroup, Sort, dataSource.DbProvider.ParameterPrefix);

            if (string.IsNullOrEmpty(strGroup))
            {
                cmdText.AppendFormat("SELECT COUNT(0) AS TotalRecords FROM {0}{1}", TableName, strFilter);
            }
            else
            {
                cmdText.AppendFormat("SELECT COUNT(0) AS TotalRecords FROM (SELECT COUNT(0) AS C0 FROM {0} {1} {2}) T1", TableName, strFilter, strGroup);
            }

            var paras = new List<DbParameter>();
            paras.Add(dataSource.CreateParameter(dataSource.DbProvider.ParameterPrefix + "strStartRow", strStartRow - 1));
            paras.Add(dataSource.CreateParameter(dataSource.DbProvider.ParameterPrefix + "strEndRow", PageSize));

            if (paramers != null && paramers.Count() > 0)
            {
                paras.AddRange(paramers);
            }

            parameter = paras.ToArray();

            return cmdText;
        }
        #region private
        private StringBuilder CreateAllEntityDicSql<T>(params string[] exclusionList)
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
            return sqlFields;
        }
        #endregion
    }
}
