DBLayer is a orm db access project.
it's light weight easy to use.
with spring.net is a good way to use i recommond use it.

add a log data to db
<code>
var id = TheService.InsertEntity<SysLog, long>(
        () => new SysLog()
        {
            LogId = -1,
            LogContentJson = "测试",
            LogCreater = "测试",
            LogCreateTime = DateTime.Now,
            LogType = "1"
        });
</code>

paged search engine
<code>
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public IEnumerable<SysUser> Seach(SysUserCondition.Search condition)
        {
            var page = new Pager<SysUserCondition.Search>()
            {
                Condition = condition,
                Table = "sys_user",
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
</code>


