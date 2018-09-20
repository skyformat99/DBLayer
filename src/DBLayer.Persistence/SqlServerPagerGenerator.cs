using DBLayer.Core;
using DBLayer.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLayer.Persistence
{
    public class SqlServerPagerGenerator : IPagerGenerator
    {
        /// <summary>
        /// 查询所有数据-不包含字段*
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSource"></param>
        /// <param name="paramerList"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public StringBuilder GetSelectCmdText<T>(DataSource dataSource, ref List<DbParameter> paramerList, StringBuilder whereStr, StringBuilder orderStr, int? top = null)
        {
            var cmdText = new StringBuilder();
            var topStr = new StringBuilder();
            if (top != null && top.Value > 0)
            {
                var topparameter = dataSource.DbProvider.ParameterPrefix + "topParameter";
                topStr.AppendFormat("TOP({0})", topparameter);
                paramerList.Add(dataSource.CreateParameter(topparameter, top.Value));
            }

            var entityType = typeof(T);
            var tableName = string.Empty;

            var dataTable = entityType.GetDataTableAttribute(out tableName);

            cmdText.AppendFormat("SELECT {1} * FROM {0} {2} {3} ", tableName, topStr, whereStr, orderStr);

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
        public StringBuilder GetSelectDictionaryCmdText<T>(DataSource dataSource, ref List<DbParameter> paramerList, StringBuilder whereStr, StringBuilder orderStr, int? top = null, params string[] exclusionList)
        {
            var cmdText = new StringBuilder();
            var topStr = new StringBuilder();
            if (top != null && top.Value > 0)
            {
                var topparameter = dataSource.DbProvider.ParameterPrefix + "topParameter";
                topStr.AppendFormat("TOP({0})", topparameter);
                paramerList.Add(dataSource.CreateParameter(topparameter, top.Value));
            }

            var entityType = typeof(T);
            var tableName = string.Empty;

            var dataTable = entityType.GetDataTableAttribute(out tableName);
            var fields = CreateAllEntityDicSql<T>(exclusionList);

            cmdText.AppendFormat("SELECT {2} {1} FROM {0} {3} {4} ", tableName, fields, topStr, whereStr, orderStr);

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
        public object InsertExecutor<T>(DataSource dataSource, StringBuilder insertCmd, List<DbParameter> paramerList, DbTransaction trans = null)
        {

            var cmdText = new StringBuilder();
            cmdText.AppendFormat("{0};{1}", insertCmd, dataSource.DbProvider.SelectKey);

            var newID = trans == null ?
                        dataSource.ExecuteScalar(cmdText.ToString(), CommandType.Text, paramerList.ToArray()) :
                        dataSource.ExecuteScalar(trans, cmdText.ToString(), CommandType.Text, paramerList.ToArray());

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
        public Task<object> InsertExecutorAsync<T>(DataSource dataSource, StringBuilder insertCmd, List<DbParameter> paramerList, DbTransaction trans = null)
        {

            var cmdText = new StringBuilder();
            cmdText.AppendFormat("{0};{1}", insertCmd, dataSource.DbProvider.SelectKey);

            var newID = trans == null ?
                        dataSource.ExecuteScalarAsync(cmdText.ToString(), CommandType.Text, paramerList.ToArray()) :
                        dataSource.ExecuteScalarAsync(trans, cmdText.ToString(), CommandType.Text, paramerList.ToArray());

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
            result.AppendFormat("{0} IN (SELECT [value] FROM f_SPLIT({1},default))", leftString, rightString);
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
            result.AppendFormat("{0} NOT IN (SELECT [value] FROM f_SPLIT({1},default))", leftString, rightString);
            return result;
        }
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
        public StringBuilder GetPageCmdText(DataSource dataSource, string UnionText, string TableName, string FldName, ref int? PageIndex, ref int? PageSize, string Filter, string Group, string Sort, ref DbParameter[] parameter, params DbParameter[] paramers)
        {
            var cmdText = new StringBuilder();
            var strPageSize = -1;
            var strStartRow = -1;
            var strEndRow = -1;
            var strFilter = "";
            var strGroup = "";
            var strSort = "";

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
                strSort = " ORDER BY " + Sort;
            }
            else
            {
                strSort = " ORDER BY (SELECT 0)";
            }

            cmdText.AppendLine(UnionText);
            //
            cmdText.AppendFormat("SELECT * FROM ( SELECT ROW_NUMBER() OVER({0}) AS ROWNUM, {1} FROM {2} {3} {4}) AS QUERY_T1 WHERE ROWNUM >= {5}strStartRow AND ROWNUM <= {5}strEndRow ORDER BY ROWNUM;",
                 strSort, FldName, TableName, strFilter, strGroup, dataSource.DbProvider.ParameterPrefix);

            //cmdText.AppendFormat("SELECT * FROM (SELECT {0},ROW_NUMBER() OVER({1}) AS row FROM {2}{3}{4}) a WHERE row BETWEEN {5}strStartRow AND {5}strEndRow;", 
            //    FldName, strSort, TableName, strFilter, strGroup, dataSource.DbProvider.ParameterPrefix);

            cmdText.AppendLine(UnionText);
            if (string.IsNullOrEmpty(strGroup))
            {
                cmdText.AppendFormat("SELECT COUNT(0) AS TotalRecords FROM {0}{1}", TableName, strFilter);
            }
            else
            {
                cmdText.AppendFormat("SELECT COUNT(0) AS TotalRecords FROM (SELECT COUNT(0) AS C0 FROM {0} {1} {2}) T1", TableName, strFilter, strGroup);
            }

            var paras = new List<DbParameter>();
            paras.Add(dataSource.CreateParameter(dataSource.DbProvider.ParameterPrefix + "Pagesize", PageSize));
            paras.Add(dataSource.CreateParameter(dataSource.DbProvider.ParameterPrefix + "strStartRow", strStartRow));
            paras.Add(dataSource.CreateParameter(dataSource.DbProvider.ParameterPrefix + "strEndRow", strEndRow));

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
