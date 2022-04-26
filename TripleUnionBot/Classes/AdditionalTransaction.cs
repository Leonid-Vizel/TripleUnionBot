using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripleUnionBot.Classes
{
    internal class AdditionalTransaction
    {
        public int Id { get; private set; }
        public string Username { get; private set; }
        public decimal Money { get; private set; }

        public AdditionalTransaction(int id, string username, decimal money)
        {
            Id = id;
            Username = username;
            Money = money;
        }
    }
}
