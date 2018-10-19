using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace DBLayer.Core.Interface
{
    public interface IConnectionString
    {
        NameValueCollection Properties { get; set; }
        string ConnectionToken { get; set; }
        string ConnectionValue { get; }
    }
}
