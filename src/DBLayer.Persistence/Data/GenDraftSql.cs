using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace DBLayer.Persistence.Data
{
    /// <summary>
    ///  generate exe sql for debug
    /// </summary>
    internal static class GenDraftSql
    {
        static readonly Dictionary<Type, SqlDbType> _dic = new Dictionary<Type, SqlDbType>
            {
                { typeof(sbyte), SqlDbType.SmallInt },
                { typeof(byte), SqlDbType.TinyInt },
                { typeof(short), SqlDbType.SmallInt },
                { typeof(ushort), SqlDbType.SmallInt },
                { typeof(int), SqlDbType.Int },
                { typeof(uint), SqlDbType.Int },
                { typeof(long), SqlDbType.BigInt },
                { typeof(ulong), SqlDbType.BigInt },
                { typeof(float), SqlDbType.Float },
                { typeof(double), SqlDbType.Decimal },
                { typeof(string), SqlDbType.VarChar },
                { typeof(bool), SqlDbType.Bit },
                { typeof(DateTime), SqlDbType.DateTime },
                { typeof(decimal), SqlDbType.Decimal },
                { typeof(Enum), SqlDbType.Int },

                { typeof(sbyte?), SqlDbType.SmallInt },
                { typeof(byte?), SqlDbType.TinyInt },
                { typeof(short?), SqlDbType.SmallInt },
                { typeof(ushort?), SqlDbType.SmallInt },
                { typeof(int?), SqlDbType.Int },
                { typeof(uint?), SqlDbType.Int },
                { typeof(long?), SqlDbType.BigInt },
                { typeof(ulong?), SqlDbType.BigInt },
                { typeof(float?), SqlDbType.Float },
                { typeof(double?), SqlDbType.Decimal },
                { typeof(bool?), SqlDbType.Bit },
                { typeof(DateTime?), SqlDbType.DateTime },
                { typeof(decimal?), SqlDbType.Decimal },
            };

        static string GetDraftSql(string cmdText, params DbParameter[] parameters)
        {
            var sql = cmdText;
            if (parameters == null)
            {
                return sql;
            }

            try
            {
                foreach (var parameter in parameters)
                {
                    var value = parameter.Value;
                    var nameInSQL = parameter.ParameterName;

                    if (value == null)
                        sql = sql.Replace(nameInSQL, "NULL");
                    else
                    {
                        Type type = value.GetType();
                        if (type.IsEnum)
                            type = typeof(Enum);

                        if (_dic.ContainsKey(type))
                        {
                            switch (_dic[type])
                            {
                                case SqlDbType.Bit:
                                    sql = sql.Replace(nameInSQL, Convert.ToInt16(value).ToString());
                                    break;
                                case SqlDbType.BigInt:
                                case SqlDbType.Int:
                                case SqlDbType.Decimal:
                                case SqlDbType.Float:
                                case SqlDbType.SmallInt:
                                case SqlDbType.TinyInt:
                                    sql = sql.Replace(nameInSQL, value.GetType().IsEnum
                                        ? ((Enum)value).ToString("d")
                                        : value.ToString());
                                    break;
                                case SqlDbType.DateTime:
                                case SqlDbType.VarChar:
                                    var sTempValue = value.ToString().Replace("'", "''");
                                    sql = sql.Replace(nameInSQL, $"'{sTempValue}'");
                                    break;
                                default:
                                    throw new Exception("【exception difined 111111】not add switch type refrence");
                            }
                        }
                        else
                        {
                            throw new Exception($"【exception difined 111112】not contains this type: 【{type.FullName}】");
                        }
                    }
                }

                return sql;
            }
            catch (Exception ex)
            {
                return $"generate sql exception:{ex.ToString()}";
            }
        }

        public static void LogSQL(this ILogger _logger, string cmdText, params DbParameter[] parameters)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                var log = GetDraftSql(cmdText, parameters);
                _logger.LogDebug(log);
            }
        }
    }
}
