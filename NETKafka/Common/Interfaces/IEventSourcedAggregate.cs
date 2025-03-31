using NETKafka.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETKafka.Common.Interfaces
{
    public interface IEventSourcedAggregate
    {
        public void Apply(Event @event);
    }
}
