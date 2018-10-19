using System;
using System.Collections.Generic;
using System.Text;

namespace DBLayer.Core.Interface
{
    public interface IDbContext
    {
        IDbProvider DbProvider { get; }
        IConnectionString ConnectionString { get; }
        IGenerator Generator { get; }
        IPagerGenerator PagerGenerator { get; }
    }
}
