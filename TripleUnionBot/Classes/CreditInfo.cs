using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripleUnionBot.Classes
{
    internal class CreditInfo
    {
        public int Id { get; private set; }
        public string Owner { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime ReturnDate { get; private set; }

        public CreditInfo(int id, string owner, decimal amount, DateTime returnDate)
        {
            Id = id;
            Owner = owner;
            Amount = amount;
            ReturnDate = returnDate;
        }
    }
}
