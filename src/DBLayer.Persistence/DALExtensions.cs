using DBLayer.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
/*------------------------------------------------------------------------------
 * 单元名称：数据库层--数据处理扩展类
 * 单元描述： 
 * 创 建 人：吴涛
 * 创建日期： 
 * 修改日志
 * 修 改 人   修改日期    修改内容
 * 
 * ----------------------------------------------------------------------------*/
namespace DBLayer.Persistence
{
    public static class DALExtensions
    {
        public static IEnumerable<T> ReadEnumerable<T>(this IDataReader reader, Func<IDataReader, T> map)
        {
            while (reader.Read())
            {
                yield return map(reader);
            }
        }

        public static IEnumerable<T> ReadList<T>(this IDataReader reader, Func<IDataReader, T> map, ref int totalCount)
        {
            try
            {
                var collection =new List<T>(reader.ReadEnumerable(map));
                reader.NextResult();
                reader.Read();
                totalCount = reader.ReadValue<int>("TotalRecords");

                return collection;
            }
            catch (Exception)
            {
                if (reader != null && !reader.IsClosed) { reader.Close(); }
                throw;
            }

        }

     

        public static IDictionary<string, object> ReadSelf(this IDataReader reader, params string[] exclusionList)
        {
            try
            {
                var dicentity = new Dictionary<string, object>();
                reader.ReadSelf(dicentity, exclusionList);

                return dicentity;
            }
            catch (Exception)
            {
                if (reader != null && !reader.IsClosed) { reader.Close(); }
                throw;
            }
            
        }

        public static IDictionary<string, object> ReadSelf(this IDataReader reader, IDictionary<string, object> item, params string[] exclusionList)
        {
            try
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    try
                    {
                        item.Add(reader.GetName(i), reader.GetValue(i));
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                return item;
            }
            catch (Exception)
            {
                if (reader != null && !reader.IsClosed) { reader.Close(); }
                throw;
            }
            
        }

        public static T ReadObject<T>(this IDataReader reader, params string[] exclusionList) where T : new()
        {
            try
            {
                var item = new T();
                reader.ReadObject(item, exclusionList);

                return item;
            }
            catch (Exception)
            {
                if (reader != null && !reader.IsClosed) { reader.Close(); }
                throw;
            }
            
        }

        public static T ReadObject<T>(this IDataReader reader, T item, params string[] exclusionList)
        {
            try
            {
                var entityType = item.GetType();
                var propertyInfos = entityType.GetProperties();

                foreach (var property in propertyInfos)
                {
                    //不可读
                    if (!property.CanRead)
                    {
                        continue;
                    }
                    //不可写
                    if (!property.CanWrite)
                    {
                        continue;
                    }

                    if (!property.PropertyType.IsReadablePropertyType())
                    {
                        continue;
                    }

                    if (exclusionList != null && exclusionList.IsExcluded(property.Name))
                    {
                        continue;
                    }

                    // We need to catch this exception in cases when we're upgrading and the column might not exist yet.
                    // It'd be nice to have a cleaner way of doing this.
                    try
                    {
                        string fieldName;
                        property.GetDataFieldAttribute(out fieldName);
                        object value = reader[fieldName];
                        if (value != DBNull.Value)
                        {
                            if (property.PropertyType != typeof(Uri))
                            {
                                var dataType = value.GetType();
                                if (property.PropertyType == dataType)
                                {
                                    property.SetValue(item, value, null);
                                }
                                else
                                {
                                    property.SetValue(item,value.ChangeType(property.PropertyType), null);
                                }
                            }
                            else
                            {
                                var url = value as string;
                                if (!String.IsNullOrEmpty(url))
                                {
                                    Uri uri;
                                    if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
                                    {
                                        property.SetValue(item, uri, null);
                                    }
                                }
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {

                    }
                }

                return item;
            }
            catch (Exception)
            {
                if (reader != null && !reader.IsClosed) { reader.Close(); }
                throw;
            }
        }

        public static T ReadValue<T>(this IDataReader reader, string columnName)
        {
            try
            {
                return reader.ReadValue(columnName, default(T));
            }
            catch (Exception)
            {
                if (reader != null && !reader.IsClosed) { reader.Close(); }
                throw;
            }
            
        }

        public static T ReadValue<T>(this IDataReader reader, string columnName, T defaultValue)
        {
            try
            {
                return reader.ReadValue(columnName, value =>(T)value.ChangeType(typeof(T)), defaultValue);
            }
            catch (Exception)
            {
                if (reader != null && !reader.IsClosed) { reader.Close(); }
                throw;
            }
            
        }

        public static T ReadValue<T>(this IDataReader reader, string columnName, Func<object, T> map, T defaultValue)
        {
            try
            {
                var value = reader[columnName];
                if (value != null && value != DBNull.Value)
                {
                    return map(value);
                }
                return defaultValue;
            }
            catch (FormatException)
            {
                return defaultValue;
            }
            catch (IndexOutOfRangeException)
            {
                return defaultValue;
            }
            catch (Exception) 
            {
                if (reader != null && !reader.IsClosed) { reader.Close(); }
                throw;
            }
        }
        
        /// <summary>
        /// 将table转换为相应对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static IList<T> ConvertToModel<T>(this DataTable dt) where T : new()
        {
            // 定义集合
            var ts = new List<T>();

            // 获得此模型的类型
            var type = typeof(T);

            var tempName = "";

            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();

                // 获得此模型的公共属性
                var propertys = t.GetType().GetProperties();

                foreach (var pi in propertys)
                {
                    var datafieldAttribute = pi.GetDataFieldAttribute(out tempName);

                    // 检查DataTable是否包含此列
                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter
                        if (!pi.CanWrite) continue;

                        var value = dr[tempName];
                        if (value != DBNull.Value) 
                        {
                            var dataType = value.GetType();
                            if (pi.PropertyType == dataType)
                            {
                                pi.SetValue(t, value, null);
                            }
                            else
                            {
                                pi.SetValue(t, value.ChangeType(pi.PropertyType), null);
                            }
                        }
                            
                    }
                }

                ts.Add(t);
            }

            return ts;
        }

        /// <summary>
        /// 读取ip地址
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="columnName">Name of the coumn.</param>
        /// <returns></returns>
        public static IPAddress ReadIpAddress(this IDataReader reader, string columnName)
        {
            try
            {
                return reader.ReadValue(columnName, value => IPAddress.Parse((string)value), IPAddress.None);
            }
            catch (Exception)
            {
                if (reader != null && !reader.IsClosed) { reader.Close(); }
                throw;
            }
            
        }

        /// <summary>
        /// 从数据库里面读取一个URI
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static Uri ReadUri(this IDataReader reader, string columnName)
        {
            try
            {
                return reader.ReadValue(columnName, value => new Uri((string)value), null);
            }
            catch (Exception)
            {
                if (reader != null && !reader.IsClosed) { reader.Close(); }
                throw;
            }
            
        }

        /// <summary>
        /// 利用反射根据对象和属性名取对应属性的值
        /// </summary>
        /// <param name="feildname"></param>
        /// <param name="obEntity"></param>
        /// <returns></returns>
        public static string GetValueByPropertyName(this object obEntity, string feildname )
        {
            var PropertyVaule = string.Empty;
            var tpEntity = obEntity.GetType();
            var pis = tpEntity.GetProperties();
            var a = pis.FirstOrDefault(m => m.Name == feildname);
            if (a != null)
            {
                object obj = a.GetValue(obEntity, null);
                if (obj != null)
                {
                    PropertyVaule = obj.ToString();
                }
            }
            return PropertyVaule;
        }
        /// <summary>
        /// 利用反射根据对象和属性名为对应的属性赋值
        /// </summary>
        /// <param name="feildname"></param>
        /// <param name="obEntity"></param>
        /// <returns></returns>
        public static void SetValueByPropertyName(this object obEntity, string feildname, object Value)
        {
            var tpEntity = obEntity.GetType();
            var pis = tpEntity.GetProperties();
            var a = pis.FirstOrDefault(m => m.Name == feildname);
            if (a != null)
            {
                a.SetValue(obEntity,Value.ChangeType(a.PropertyType), null);
            }
        }


        
        

    } //end class
}// end namespace
