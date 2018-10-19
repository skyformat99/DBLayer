using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using DBLayer.Core.Interface;
using DBLayer.Persistence;
using DBLayer.Persistence.Data;
using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Configuration;

namespace DBLayer.Core.Extensions
{
    public class DBLayerOptions
    {
        public IDbProvider DbProvider { get; set; }
        public IConnectionString ConnectionString { get; set; }
        public IGenerator Generator { get; set; }
        public IPagerGenerator PageGenerator { get; set; }

    }
    public static class ServiceBuilderExtensions
    {
        public static void AddDBLayer(this IServiceCollection services, DBLayerOptions options)
        {
            services.AddSingleton(options.DbProvider);
            services.AddSingleton(options.ConnectionString);
            services.AddSingleton(options.Generator);
            services.AddSingleton(options.PageGenerator);
        }

        //public static void AddDBLayer(this IServiceCollection services)
        //{
        //    var connectionString = new ConnectionString
        //    {
        //        Properties = new NameValueCollection
        //        {
        //            { "userid","sa"},
        //            { "password","P@ssw0rd"},
        //            { "passwordKey",""},
        //            { "database","NNEV"},
        //            { "datasource","192.168.16.122"}
        //        },
        //        ConnectionToken = "Password=${password};Persist Security Info=True;User ID=${userid};Initial Catalog=${database};Data Source=${datasource};pooling=true;min pool size=5;max pool size=10"
        //    };

        //    //data source=192.168.16.122;initial catalog=NNEV;persist security info=True;user id=sa;password=P@ssw0rd;

        //    var provider = new DbProvider
        //    {
        //        ProviderName = "System.Data.SqlClient.SqlClientFactory, System.Data.SqlClient",
        //        ParameterPrefix = "@",
        //        SelectKey = "SELECT @@IDENTITY;"
        //    };


        //    var guidGenerator = new GUIDGenerator();
        //    var sqlServerPagerGenerator = new SqlServerPagerGenerator();
        //    var datasource = new DataSource(provider, connectionString);


        //    var service = new SysLogService
        //    {
        //        TheGenerator = guidGenerator,
        //        ThePagerGenerator = sqlServerPagerGenerator,
        //        DbProvider = provider,
        //        ConnectionString = connectionString
        //    };

        //}
    }
}
