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
        public class SysLogService : AbstractService<SysConfiguration>
        { 
        }
        #endregion
        #endregion
    }
}
