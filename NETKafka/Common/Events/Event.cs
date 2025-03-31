using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETKafka.Common.Events
{
    public abstract record Event(Guid Id)
    {        
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
