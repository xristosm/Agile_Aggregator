using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile_Aggregator.Domain.Filtering
{
    public class Sort
    {
        public string PropertyName { get; set; } = string.Empty;
        public bool Descending { get; set; }
    }
}
