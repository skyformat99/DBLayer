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
            services.AddSingleton<IDbContext, DbContext>();
        }
    }
}
