using NETKafka.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETKafka.Common.Events
{
    public record Opened(Guid AccountId, string AccountOwner, decimal InitialDeposit, string Coin = "BRL") : Event(AccountId);

}
