using DBLayer.Core;
using DBLayer.Core.Condition;
using DBLayer.Persistence;
using DBLayer.Persistence.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Specialized;
using DBLayer.Core.Extensions;
using DBLayer.Core.Interface;

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
                        { "password","P@ssw0rd"},
                        { "passwordKey",""},
                        { "database","NNEV"},
                        { "datasource","192.168.16.122"}
                    },
                ConnectionToken = "Password=${password};Persist Security Info=True;User ID=${userid};Initial Catalog=${database};Data Source=${datasource};pooling=true;min pool size=5;max pool size=10"
            };
            //data source=192.168.16.122;initial catalog=NNEV;persist security info=True;user id=sa;password=P@ssw0rd;

            var provider = new DbProvider
            {
                ProviderName = "System.Data.SqlClient.SqlClientFactory, System.Data.SqlClient",
                //ProviderName = "MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data",
                //ProviderName = "Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess.Core",
                ParameterPrefix = "@",
                SelectKey = "SELECT @@IDENTITY;"
            };
            var guidGenerator = new GUIDGenerator();
            var sqlServerPagerGenerator = new SqlServerPagerGenerator();



            var service = new SysConfigurationService
            {
                TheGenerator = guidGenerator,
                ThePagerGenerator = sqlServerPagerGenerator,
                DbProvider = provider,
                ConnectionString = connectionString
            };
            
            var list = service.GetEntityList(w => w.Category == "aviOperation");

            //using (var trans = service.GetTransaction())
            //{
            //    service.UpdateEntity(() => new SysConfiguration { Category = "aa" },w=>w.IsDelete==false, trans);
            //    trans.Rollback();
            //    trans.Commit();
            //}
            
            var count = list.Count;
            Assert.IsNotNull(list);
        }
        [TestMethod]
        public void TestIoc()
        {
            IServiceCollection collection =new ServiceCollection();
            collection.AddDBLayer(new DBLayerOptions
            {
                ConnectionString = new ConnectionString
                {
                    Properties = new NameValueCollection
                    {
                        { "userid","sa"},
                        { "password","P@ssw0rd"},
                        { "passwordKey",""},
                        { "database","NNEV"},
                        { "datasource","192.168.16.122"}
                    },
                    ConnectionToken = "Password=${password};Persist Security Info=True;User ID=${userid};Initial Catalog=${database};Data Source=${datasource};pooling=true;min pool size=5;max pool size=10"
                },
                DbProvider = new DbProvider
                {
                    ProviderName = "System.Data.SqlClient.SqlClientFactory, System.Data.SqlClient",
                    ParameterPrefix = "@",
                    SelectKey = "SELECT @@IDENTITY;"
                },
                Generator = new GUIDGenerator(),
                PageGenerator = new SqlServerPagerGenerator()

            });

            collection.AddTransient<ISysConfigurationService, SysConfigurationService>();

            var bsp = collection.BuildServiceProvider();

            var service = bsp.GetService<ISysConfigurationService>();
            var list = service.GetEntityListAsync(w => w.Category == "aviOperation").GetAwaiter().GetResult();
            var count = list.Count;
            Assert.IsNotNull(list);
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
        public interface ISysConfigurationService: IAbstractService<SysConfiguration> { }
        public class SysConfigurationService : AbstractService<SysConfiguration>, ISysConfigurationService
        {
            public SysConfigurationService() { }
            public SysConfigurationService(IDbProvider dbProvider, IConnectionString connectionString, IGenerator generator, IPagerGenerator pagerGenerator) : base(dbProvider, connectionString, generator, pagerGenerator)
            { }

        }
        #endregion
        #endregion
    }
}
