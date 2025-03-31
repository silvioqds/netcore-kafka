using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETKafka.Common.Events
{
    public record DepositAccount(Guid AccountId, decimal Amount) : Event(AccountId);    
}
