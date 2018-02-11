using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
/*------------------------------------------------------------------------------
* 单元名称：
* 单元描述：
* 创建人：
* 创建日期：
* 修改日志
* 修改人   修改日期    修改内容
* 
* ----------------------------------------------------------------------------*/
namespace DBLayer.Core
{
    public static class DataFieldExtensions
    {
        public static bool IsExcluded(this IEnumerable<string> exclusionList, string propertyName)
        {
            bool IsSuccess = exclusionList.Any(excludedProperty => excludedProperty == propertyName);

            return IsSuccess;
        }

        public static bool IsReadablePropertyType(this Type t)
        {
            var isReadable = t.IsPrimitive ||
                              t == typeof(Decimal) ||
                              t == typeof(DateTime) ||
                              t == typeof(string) ||
                              t == typeof(Uri);

            if (!isReadable)
            {
                //Maybe it's a nullable.
                var underlyingType = Nullable.GetUnderlyingType(t);

                if (underlyingType != null)
                {
                    return IsReadablePropertyType(underlyingType);
                }
            }
            return isReadable;
        }

        /// <summary>
        ///  获取DataFieldAttribute特性
        /// </summary>
        /// <param name="entityType">Class类</param>
        /// <returns>DataFieldAttribute</returns>
        public static DataFieldAttribute GetDataFieldAttribute(this PropertyInfo prop, out string fieldName)
        {
            fieldName = prop.Name;
            var oArr = prop.GetCustomAttributes(true);
            for (var i = 0; i < oArr.Length; i++)
            {
                if (oArr[i] is DataFieldAttribute)
                {
                    var df=(DataFieldAttribute)oArr[i];
                    fieldName = df.FieldName;
                    return (DataFieldAttribute)oArr[i];
                }
            }
            return null;
        }
       

        /// <summary>
        ///  获取DataFieldAttribute特性
        /// </summary>
        /// <param name="entityType">Class类</param>
        /// <returns>DataFieldAttribute</returns>
        public static DataTableAttribute GetDataTableAttribute(this Type propType, out string tableName)
        {
            tableName = propType.Name;
            var oArr = propType.GetCustomAttributes(true);
            for (int i = 0; i < oArr.Length; i++)
            {
                if (oArr[i] is DataTableAttribute) 
                {
                    var dt = (DataTableAttribute)oArr[i];
                    tableName=dt.TableName;
                    return dt;
                }
            }
            return null;
        }
    }
}
