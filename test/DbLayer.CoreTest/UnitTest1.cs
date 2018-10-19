using DBLayer.Core;
using DBLayer.Core.Condition;
using DBLayer.Persistence;
using DBLayer.Persistence.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Specialized;

namespace DbLayer.CoreTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var connectionString = new ConnectionString
            {
                Properties = new NameValueCollection
                {
                    { "userid","sa"},
                    { "password","1234@#zxcv"},
                    { "passwordKey",""},
                    { "database","NNEV"},
                    { "datasource","192.168.1.108"}
                },
                ConnectionToken= "Password=${password};Persist Security Info=True;User ID=${userid};Initial Catalog=${database};Data Source=${datasource};pooling=true;min pool size=5;max pool size=10"
            };
            //data source=192.168.16.122;initial catalog=NNEV;persist security info=True;user id=sa;password=P@ssw0rd;

            var provider = new DbProvider
            {
                ProviderName= "System.Data.SqlClient",
                ParameterPrefix="@",
                SelectKey= "SELECT @@IDENTITY;"
            };

            var guidGenerator = new GUIDGenerator();
            var sqlServerPagerGenerator = new SqlServerPagerGenerator();


            var service = new SysLogService
            {
                TheGenerator=guidGenerator,
                ThePagerGenerator=sqlServerPagerGenerator,
                DbProvider=provider,
                ConnectionString=connectionString
            };

            var list = service.GetEntityList();
            var count = list.Count;

        }

        #region syslog
        #region entity
        /// <summary>
        /// 系统配置
        /// </summary>
        [DataTable("SYS_CONFIGURATION")]
        public class SysConfiguration
        {
            /// <summary>
            /// 主键
            /// </summary>
            [DataField("id", IsAuto = true, IsKey = true, KeyType = KeyType.MANUAL)]
            public Guid Id { get; set; }

            /// <summary>
            /// 值
            /// </summary>
            [DataField("VALUE")]
            public string Value { get; set; }
            /// <summary>
            /// 显示值
            /// </summary>
            [DataField("TEXT")]
            public string Text { get; set; }
            /// <summary>
            /// 顺序号
            /// </summary>
            [DataField("DISPLAY_NO")]
            public int DisplayNo { get; set; }
            /// <summary>
            /// 分类
            /// </summary>
            [DataField("CATEGORY")]
            public string Category { get; set; }
            /// <summary>
            /// 备注
            /// </summary>
            [DataField("REMARK")]
            public string Remark { get; set; }

            /// <summary>
            /// 是否隐藏
            /// </summary>
            [DataField("IS_HIDE")]
            public bool IsHide { get; set; }
            /// <summary>
            /// 是否删除
            /// </summary>
            [DataField("IS_DELETE")]
            public bool IsDelete { get; set; }
            /// <summary>
            /// 创建用户
            /// </summary>
            [DataField("CREATE_USER_ID")]
            public Guid CreateUserId { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            [DataField("CREATE_DT")]
            public DateTime CreateDt { get; set; }
            /// <summary>
            /// 更新用户
            /// </summary>
            [DataField("UPDATE_USER_ID")]
            public Guid UpdateUserId { get; set; }
            /// <summary>
            /// 更新时间 
            /// </summary>
            [DataField("UPDATE_DT")]
            public DateTime UpdateDt { get; set; }

        }
        #endregion
        #region Condition
        public class SysLogCondition
        {
            public class Search : BasePageCondition
            {
            }
        }
        #endregion
        #region Service
        public class SysLogService : AbstractService<SysConfiguration>
        { 
        }
        #endregion
        #endregion
    }
}
