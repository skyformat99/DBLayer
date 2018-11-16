using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace DBLayer.Core.Interface
{
    public interface IConnectionString
    {
        NameValueCollection Properties { get;}
        string ConnectionToken { get; }
        string ConnectionValue { get; }
    }
}
