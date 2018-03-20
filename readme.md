<p align="center">
    <br>
    <a href="http://www.wxius.com">
        <img width="200" src="http://style.wxius.com/assets/frontend/layout/img/logos/logo-corp-red.png">
    </a>
    <br>
</p>
<br/>


#### DBLayer is a orm db access project.

* it's light weight easy to use.
* with spring.net is a good way to use i recommond use it.

```
//add a log data to db

var id = TheService.InsertEntity<SysLog, long>(
        () => new SysLog()
        {
            LogId = -1,
            LogContentJson = "test",
            LogCreater = "test",
            LogCreateTime = DateTime.Now,
            LogType = "1"
        });
```
```
///paged search engine

        /// <summary>
        /// paged search
        /// </summary>
        /// <param name="condition">the search condition</param>
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
```

## plan 
* Read-write separation
* simply separate read and write database with power string r1w1
* when you op cud a timer will run , in timer range,the connection will go to r1 database

