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
        /// ϵͳ����
        /// </summary>
        [DataTable("SYS_CONFIGURATION")]
        public class SysConfiguration
        {
            /// <summary>
            /// ����
            /// </summary>
            [DataField("id", IsAuto = true, IsKey = true, KeyType = KeyType.MANUAL)]
            public Guid Id { get; set; }

            /// <summary>
            /// ֵ
            /// </summary>
            [DataField("VALUE")]
            public string Value { get; set; }
            /// <summary>
            /// ��ʾֵ
            /// </summary>
            [DataField("TEXT")]
            public string Text { get; set; }
            /// <summary>
            /// ˳���
            /// </summary>
            [DataField("DISPLAY_NO")]
            public int DisplayNo { get; set; }
            /// <summary>
            /// ����
            /// </summary>
            [DataField("CATEGORY")]
            public string Category { get; set; }
            /// <summary>
            /// ��ע
            /// </summary>
            [DataField("REMARK")]
            public string Remark { get; set; }

            /// <summary>
            /// �Ƿ�����
            /// </summary>
            [DataField("IS_HIDE")]
            public bool IsHide { get; set; }
            /// <summary>
            /// �Ƿ�ɾ��
            /// </summary>
            [DataField("IS_DELETE")]
            public bool IsDelete { get; set; }
            /// <summary>
            /// �����û�
            /// </summary>
            [DataField("CREATE_USER_ID")]
            public Guid CreateUserId { get; set; }
            /// <summary>
            /// ����ʱ��
            /// </summary>
            [DataField("CREATE_DT")]
            public DateTime CreateDt { get; set; }
            /// <summary>
            /// �����û�
            /// </summary>
            [DataField("UPDATE_USER_ID")]
            public Guid UpdateUserId { get; set; }
            /// <summary>
            /// ����ʱ�� 
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
