using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLayer.Core.Condition
{
    public class BasePageCondition : BaseCondition
    {
        /// <summary>
        /// 页面数
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }
    }
}
