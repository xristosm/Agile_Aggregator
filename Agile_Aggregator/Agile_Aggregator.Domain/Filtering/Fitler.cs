using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile_Aggregator.Domain.Filtering
{
    public class Filter
    {
        public string PropertyName { get; set; } = string.Empty;
        public string Operator { get; set; } = "eq";
        public string Value { get; set; } = string.Empty;
    }

}
