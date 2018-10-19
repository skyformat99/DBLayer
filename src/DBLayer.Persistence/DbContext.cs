using DBLayer.Core.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBLayer.Persistence
{
    public class DbContext:IDbContext
    {
        public DbContext(IDbProvider dbProvider,IConnectionString connectionString,IGenerator generator, IPagerGenerator pagerGenerator)
        {
            this.DbProvider = dbProvider;
            this.ConnectionString = connectionString;
            this.Generator = generator;
            this.PagerGenerator = pagerGenerator;

        }

        public IDbProvider DbProvider { get; private set; }

        public IConnectionString ConnectionString { get; private set; }

        public IGenerator Generator { get; private set; }

        public IPagerGenerator PagerGenerator { get; private set; }
    }
}
