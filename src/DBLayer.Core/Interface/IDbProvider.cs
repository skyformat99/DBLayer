using System.Data.Common;

namespace DBLayer.Core.Interface
{
    public interface IDbProvider
    {
        string ProviderName { get; set; }
        /// <summary>
        /// Parameter prefix use in store procedure.
        /// </summary>
        /// <example> @ for Sql Server.</example>
        string ParameterPrefix { get; set; }
        string SelectKey{get;set;}  
        DbProviderFactory GetDbProviderFactory();
    }
}
