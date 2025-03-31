using NETKafka.Common.Events;
using NETKafka.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETKafka.Aggregate
{
    public class BankAccount : IEventSourcedAggregate
    {
        public Guid Id { get; private set; }
        public string AccountOwner { get; private set; }
        public decimal Balance { get; private set; }
        public string Coin { get; private set; }

        public List<Event> Events { get; private set; } = [];

        public BankAccount() { }


        public static BankAccount Open(Guid id, string accountOwner, decimal balance, string Coin = "BRL")
        {
            BankAccount account = new BankAccount();
            account.Apply(new Opened(id, accountOwner, balance, Coin));
            return account;
        }

        public void Deposit(Guid accountId, decimal amount)
        {
            if (amount < 0) throw new InvalidOperationException("Negative value is not allowed for amount");

            Apply(new DepositAccount(accountId, amount));
        }

        public void WithDraw(Guid accountId, Decimal amount)
        {
            if (amount < 0) throw new InvalidOperationException("Negative value is not allowed for amount");
            Apply(new WithDrawAccount(accountId, amount));
        }

        public void Apply(Event @event)
        {

            switch (@event)
            {
                case Opened e:
                    Id = e.AccountId;
                    AccountOwner = e.AccountOwner;
                    Balance = e.InitialDeposit;
                    Coin = e.Coin;
                    break;
                case DepositAccount e:
                    Balance += e.Amount;
                    break;
                case WithDrawAccount e:
                    Balance -= e.Amount;
                    break;
            }

            Events.Add(@event);
        }

        public override string ToString()
        {
            return $"{AccountOwner} your account contain {Balance}{Coin}";
        }
    }
}