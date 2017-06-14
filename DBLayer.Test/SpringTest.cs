using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DBLayer.Core;
using Spring.Context.Support;
using DBLayer.Persistence;
using DBLayer.Core.Interface;
using DBLayer.Core.Condition;
using System.Text;
using System.Collections.Generic;

namespace DBLayer.Test
{
    [TestClass]
    public class SpringTest
    {
        [TestInitialize]
        public void Initialize() 
        {
            ContextRegistry.GetContext();
        }
        [TestMethod]
        public void SqlServerTest()
        {
            var TheService= ContextRegistry.GetContext().GetObject<SysUserService>();

           var user= TheService.GetEntityList<SysUser>();
        }

        [TestMethod]
        public void OracleTest()
        {
            var TheService = ContextRegistry.GetContext().GetObject<OracleUserService>();
            var user = TheService.GetEntityList<OracleUser>();
        }

        [TestMethod]
        public void MySqlTest()
        {
            var TheService = ContextRegistry.GetContext().GetObject<MySqlUserService>();
            var user = TheService.GetEntityList<MySqlUser>();
        }

        [TestMethod]
        public void PageSqlserverTest() 
        {
            var TheService = ContextRegistry.GetContext().GetObject<SysUserService>();
            var list = TheService.Seach(new SysUserCondition.Search() { });

        }
        [TestMethod]
        public void PageSqlserverUnionTest()
        {
            var TheService = ContextRegistry.GetContext().GetObject<SysUserService>();
            var list = TheService.Seach(new SysUserCondition.Search() { });

        }
        [TestMethod]
        public void PageOracleTest()
        {
            var TheService = ContextRegistry.GetContext().GetObject<OracleUserService>();
            var condition = new OracleUserCondition.Search() { };
            var list = TheService.Seach(condition);

        }
        [TestMethod]
        public void PageOracleUnionTest()
        {
            var TheService = ContextRegistry.GetContext().GetObject<OracleUserService>();

            var list = TheService.Seach(new OracleUserCondition.Search() { });

        }
        [TestMethod]
        public void PageMySqlTest()
        {

            var TheService = ContextRegistry.GetContext().GetObject<MySqlUserService>();

            var list = TheService.Seach(new MySqlUserCondition.Search() { });

        }
        [TestMethod]
        public void PageMySqlUionTest()
        {

            var TheService = ContextRegistry.GetContext().GetObject<MySqlUserService>();

            var list = TheService.Seach(new MySqlUserCondition.Search() { });

        }


        [TestMethod]
        public void OpreatorSqlServerTest()
        {
            var TheService = ContextRegistry.GetContext().GetObject<SysLogService>();
            using (var trans=TheService.GetTransaction())
            {
                try
                {

                    var id = TheService.InsertEntity<SysLog, long>(
                        () => new SysLog() {
                        LogId = -1,
                        LogContentJson = "测试",
                        LogCreater = "测试",
                        LogCreateTime = DateTime.Now,
                        LogType = "1"
                    }, trans);

                    TheService.UpdateEntity<SysLog>(() => new SysLog() {
                        LogContentJson = "测试2",
                        LogType = "2"
                    }, w => w.LogId == id, trans);

                    var user = TheService.GetEntity<SysLog>(w => w.LogId == id, null, trans);

                    TheService.DeleteEntity<SysLog>(w => w.LogId == id, trans);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                }
            }
        }

        [TestMethod]
        public void OpreatorOracleTest()
        {
            var TheService = ContextRegistry.GetContext().GetObject<DynamicChanelService>();
            using (var trans = TheService.GetTransaction())
            {
                try
                {
                    var id = TheService.InsertEntity<DynamicChanel, long>(() => new DynamicChanel() 
                    {
                        Id=-1,
                        Name = "测试",
                        Isvalid = 1,
                        Createtime = DateTime.Now
                    }, trans);

                    TheService.UpdateEntity<DynamicChanel>(() => new DynamicChanel() 
                    {
                        Name = "测试2",
                        Isvalid = 0,
                        Createtime = DateTime.Now
                    }, w => w.Id == id, trans);

                    var dynamicChanel = TheService.GetEntity<DynamicChanel>(w => w.Id == id, null, trans);

                    TheService.DeleteEntity<DynamicChanel>(w => w.Id == id, trans);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        [TestMethod]
        public void OpreatorMySqlTest()
        {
            var TheService = ContextRegistry.GetContext().GetObject<OptionService>();
            using (var trans = TheService.GetTransaction())
            {
                try
                {
                    var id = TheService.InsertEntity<Option, long>(() => new Option() 
                    {
                        Id = -1,
                        Name = "IT测试",
                        Code = "test_it",
                        Typecode = "test_it",
                        Typename = "测试",
                        Isvalid = 1,
                        Sortkey = 1,
                    }, trans);
                    TheService.UpdateEntity<Option>(() => new Option() 
                    {
                        Name = "测试",
                        Code = "测试",
                        Typecode = "测试",
                        Typename = "测试",
                    }, w => w.Id == id, trans);

                    var user = TheService.GetEntity<Option>(w => w.Id == id, null, trans);

                    TheService.DeleteEntity<Option>(w => w.Id == id, trans);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }
    }

    #region syslog
    #region entity
    /// <summary>
    /// 系统日志表
    /// SysLog
    /// </summary>
    [Serializable]
    [DataTable("sys_log")]
    public class SysLog
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DataField("log_id", IsAuto = true, IsKey = true, KeyType = KeyType.MANUAL)]
        public long LogId { get; set; }
        /// <summary>
        /// 日志json内容
        /// </summary>
        [DataField("log_content_json")]
        public string LogContentJson { get; set; }
        /// <summary>
        /// 类型:1、信息,2、警告,3、错误
        /// </summary>
        [DataField("log_type")]
        public string LogType { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [DataField("log_creater")]
        public string LogCreater { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataField("log_create_time")]
        public DateTime LogCreateTime { get; set; }

    }
    #endregion
    #region Condition
    public class SysLogCondition
    {
        public class Search : BasePageCondition
        {
            /// <summary>
            /// 主键
            /// </summary>
            public long LogId { get; set; }
            /// <summary>
            /// 日志json内容
            /// </summary>
            public string LogContentJson { get; set; }
            /// <summary>
            /// 类型:1、信息,2、警告,3、错误
            /// </summary>
            public string LogType { get; set; }
            /// <summary>
            /// 创建人
            /// </summary>
            public string LogCreater { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime LogCreateTime { get; set; }
        }
    }
    #endregion
    #region Service
    public class SysLogService : AbstractService
    {

    }
    #endregion
    #endregion
    #region DynamicChanel
    #region enttiy
    /// <summary>
    /// DynamicChanel  选项 领域层实体定义(Model)
    /// </summary>
    [DataTable("t_d_dynamic_chanel", SequenceName = "SEQ_PUBLIC")]
    public class DynamicChanel
    {
        /// <summary>
        /// SEQ_PUBLIC
        /// </summary>
        [DataField("id", IsAuto = true, IsKey = true, KeyType = KeyType.SEQ)]
        public long Id { get; set; }

        /// <summary>
        /// 频道名称
        /// </summary>
        [DataField("NAME")]
        public string Name { get; set; }

        /// <summary>
        /// 状态 
        /// </summary>
        [DataField("ISVALID")]
        public int Isvalid { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataField("CREATETIME")]
        public DateTime Createtime { get; set; }
    }
    #endregion
    #region Condition
    public class DynamicChanelCondition
    {
        public class Search : BasePageCondition
        {
            /// <summary>
            /// SEQ_PUBLIC
            /// </summary>
            public long Id { get; set; }

            /// <summary>
            /// 频道名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 状态 
            /// </summary>
            public int Isvalid { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime Createtime { get; set; }
        }
    }
    #endregion
    #region Service
    public class DynamicChanelService : AbstractService
    {

    }
    #endregion
    #endregion

    #region mysql Option
    #region entity
    [DataTable("t_b_option")]
    public class Option
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DataField("ID", IsAuto = true, IsKey = true, KeyType = KeyType.SEQ)]
        public long Id { get; set; }

        [DataField("CODE")]
        public string Code { get; set; }

        [DataField("NAME")]
        public string Name { get; set; }

        [DataField("TYPECODE")]
        public string Typecode { get; set; }

        [DataField("TYPENAME")]
        public string Typename { get; set; }

        [DataField("ISVALID")]
        public int Isvalid { get; set; }

        [DataField("SORTKEY")]
        public int Sortkey { get; set; }
    }
    #endregion
    #region Service
    public class OptionService : AbstractService
    {

    }
    #endregion
    #endregion

    [DataTable("sys_user")]
    public class SysUser
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DataField("user_id", IsAuto=true, IsKey=true, KeyType=KeyType.MANUAL)]
        public long UserId{ get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [DataField("user_name")]
        public string UserName{ get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [DataField("user_pwd")]
        public string UserPwd{ get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [DataField("user_email")]
        public string UserEmail{ get; set; }
        /// <summary>
        /// 邮箱验证:0、没有认证,2、已经认证
        /// </summary>
        [DataField("user_email_audit")]
        public bool UserEmailAudit{ get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        [DataField("user_mobile")]
        public string UserMobile{ get; set; }
        /// <summary>
        /// 手机验证:0、没有认证,2、已经认证
        /// </summary>
        [DataField("user_mobile_audit")]
        public bool UserMobileAudit{ get; set; }
        /// <summary>
        /// 用户证件号
        /// </summary>
        [DataField("user_code")]
        public string UserCode { get; set; }
        /// <summary>
        /// 证件类型:1、身份证,2、学生号
        /// </summary>
        [DataField("user_code_type")]
        public int UserCodeType { get; set; }
        /// <summary>
        /// 证件验证:0、没有认证,2、已经认证
        /// </summary>
        [DataField("user_code_audit")]
        public bool UserCodeAudit { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        [DataField("user_regtime")]
        public DateTime UserRegtime{ get; set; }
        /// <summary>
        /// 冻结时间
        /// </summary>
        [DataField("user_frozentime")]
        public DateTime UserFrozentime{ get; set; }
        /// <summary>
        /// 状态:1、正常,2、冻结,3、锁定
        /// </summary>
        [DataField("user_status")]
        public int UserStatus{ get; set; }
        /// <summary>
        /// 编辑状态:1、可编辑,0、不可编辑
        /// </summary>
        [DataField("user_isedit")]
        public bool UserIsedit{ get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        [DataField("user_roles")]
        public string UserRoles{ get; set; }

    }

    [DataTable("t_d_user", SequenceName = "SEQ_CMS_USER")]
    public class OracleUser 
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DataField("ID", IsAuto = true, IsKey = true, KeyType = KeyType.SEQ)]
        public long Id { get; set; }
    }

    [DataTable("t_d_user")]
    public class MySqlUser
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DataField("ID", IsAuto = true, IsKey = true, KeyType = KeyType.SEQ)]
        public long Id { get; set; }

        [DataField("PHONEBIND")]
        public string Phonebind { get; set; }

        [DataField("SHOWNAME")]
        public string Showname { get; set; }
    }

    

    public class SysUserCondition
    {
        public class Search : BasePageCondition
        {
            /// <summary>
            /// 主键
            /// </summary>
            public long UserId { get; set; }
            /// <summary>
            /// 用户名
            /// </summary>
            public string UserName { get; set; }
            /// <summary>
            /// 密码
            /// </summary>
            public string UserPwd { get; set; }
            /// <summary>
            /// 邮箱
            /// </summary>
            public string UserEmail { get; set; }
            /// <summary>
            /// 邮箱验证:0、没有认证,2、已经认证
            /// </summary>
            public bool UserEmailAudit { get; set; }
            /// <summary>
            /// 手机
            /// </summary>
            public string UserMobile { get; set; }
            /// <summary>
            /// 手机验证:0、没有认证,2、已经认证
            /// </summary>
            public bool UserMobileAudit { get; set; }
            /// <summary>
            /// 用户证件号
            /// </summary>
            public string UserCode { get; set; }
            /// <summary>
            /// 证件类型:1、身份证,2、学生号
            /// </summary>
            public int UserCodeType { get; set; }
            /// <summary>
            /// 证件验证:0、没有认证,2、已经认证
            /// </summary>
            public bool UserCodeAudit { get; set; }
            /// <summary>
            /// 注册时间
            /// </summary>
            public DateTime UserRegtime { get; set; }
            /// <summary>
            /// 冻结时间
            /// </summary>
            public DateTime UserFrozentime { get; set; }
            /// <summary>
            /// 状态:1、正常,2、冻结,3、锁定
            /// </summary>
            public int UserStatus { get; set; }
            /// <summary>
            /// 编辑状态:1、可编辑,0、不可编辑
            /// </summary>
            public bool UserIsedit { get; set; }
            /// <summary>
            /// 角色
            /// </summary>
            public string UserRoles { get; set; }
        }

    }
    public class OracleUserCondition 
    {
        public class Search : BasePageCondition
        {
            /// <summary>
            /// 主键
            /// </summary>
            public long Id { get; set; }
        }
    }
    public class MySqlUserCondition
    {
        public class Search : BasePageCondition
        {
            /// <summary>
            /// 主键
            /// </summary>
            public long Id { get; set; }
        }
    }

    public class SysUserService : AbstractService 
    {
     
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public IEnumerable<SysUser> Seach(SysUserCondition.Search condition)
        {
            var page=new Pager<SysUserCondition.Search>(){
             Condition=condition, 
             Table="sys_user",
             Key="user_id",
             Order=string.Empty,
             Field = "*",
             WhereAction=(Condition,Where,Paramters)=>
             {
                if(!string.IsNullOrEmpty(Condition.UserName))
                {
    	            Where.Append("AND user_name LIKE @user_name ");
                    Paramters.Add(base.CreateParameter("@user_name",  string.Concat("%", Condition.UserName, "%")));
                }
                if(!string.IsNullOrEmpty(Condition.UserEmail))
                {
    	            Where.Append("AND user_email LIKE @user_email ");
                    Paramters.Add(base.CreateParameter("@user_email",  string.Concat("%", Condition.UserEmail, "%")));
                }
                if(!string.IsNullOrEmpty(Condition.UserMobile))
                {
    	            Where.Append("AND user_mobile LIKE @user_mobile ");
                    Paramters.Add(base.CreateParameter("@user_mobile",  string.Concat("%", Condition.UserMobile, "%")));
                }
             }
            };

            var result=base.GetResultByPager<SysUser,SysUserCondition.Search>(page);
            return result;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public IEnumerable<SysUser> UnionSeach(SysUserCondition.Search condition)
        {
            var page = new Pager<SysUserCondition.Search>()
            {
                Condition = condition,
                UnionText = @"WITH alluser AS 
                            (
                            SELECT * FROM sys_user
                            UNION ALL
                            SELECT * FROM sys_user
                            )",
                Table = "alluser",
                Key = "user_id",
                Order = string.Empty,
                Field = "*",
                WhereAction = (Condition, Where, Paramters) =>
                {
                    if (!string.IsNullOrEmpty(Condition.UserName))
                    {
                        Where.Append("AND user_name LIKE @user_name ");
                        Paramters.Add(base.CreateParameter("@user_name", string.Concat("%", Condition.UserName, "%")));
                    }
                    if (!string.IsNullOrEmpty(Condition.UserEmail))
                    {
                        Where.Append("AND user_email LIKE @user_email ");
                        Paramters.Add(base.CreateParameter("@user_email", string.Concat("%", Condition.UserEmail, "%")));
                    }
                    if (!string.IsNullOrEmpty(Condition.UserMobile))
                    {
                        Where.Append("AND user_mobile LIKE @user_mobile ");
                        Paramters.Add(base.CreateParameter("@user_mobile", string.Concat("%", Condition.UserMobile, "%")));
                    }
                }
            };

            var result = base.GetResultByPager<SysUser, SysUserCondition.Search>(page);
            return result;
        }
    }
    public class OracleUserService : AbstractService
    {
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public IEnumerable<OracleUser> Seach(OracleUserCondition.Search condition)
        {
            var page = new Pager<OracleUserCondition.Search>()
            {
                Condition = condition,
                Table = "t_d_user",
                Key = "id",
                Order = string.Empty,
                Field = "*",
                WhereAction = (Condition, Where, Paramters) =>
                {
                    if (Condition.Id>0)
                    {
                        Where.Append("AND ID = :ID ");
                        Paramters.Add(base.CreateParameter(":ID", Condition.Id));
                    }
                }
            };

            var result = base.GetResultByPager<OracleUser, OracleUserCondition.Search>(page);
            return result;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public IEnumerable<OracleUser> UionSeach(OracleUserCondition.Search condition)
        {
            var page = new Pager<OracleUserCondition.Search>()
            {
                Condition = condition,
                UnionText = @"WITH alluser AS 
                            (
                            SELECT * FROM t_d_user
                            UNION ALL
                            SELECT * FROM t_d_user
                            )",
                Table = "alluser",
                Key = "id",
                Order = string.Empty,
                Field = "*",
                WhereAction = (Condition, Where, Paramters) =>
                {
                    if (Condition.Id > 0)
                    {
                        Where.Append("AND ID = :ID ");
                        Paramters.Add(base.CreateParameter(":ID", Condition.Id));
                    }
                }
            };

            var result = base.GetResultByPager<OracleUser, OracleUserCondition.Search>(page);
            return result;
        }
    }
    public class MySqlUserService : AbstractService
    {
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public IEnumerable<MySqlUser> Seach(MySqlUserCondition.Search condition)
        {
            var page = new Pager<MySqlUserCondition.Search>()
            {
                Condition = condition,
                UnionText = @"
                            SELECT * FROM t_d_user
                            UNION ALL
                            SELECT * FROM t_d_user",
                Table = "alluser",
                Key = "id",
                Order = string.Empty,
                Field = "*",
                WhereAction = (Condition, Where, Paramters) =>
                {
                    if (Condition.Id > 0)
                    {
                        Where.Append("AND ID = :ID ");
                        Paramters.Add(base.CreateParameter(":ID", Condition.Id));
                    }
                }
            };

            var result = base.GetResultByPager<MySqlUser, MySqlUserCondition.Search>(page);
            return result;
        }
    }
}
