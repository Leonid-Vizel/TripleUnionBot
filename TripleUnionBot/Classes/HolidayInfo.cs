using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripleUnionBot.Classes
{
    internal class HolidayInfo
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public DateTime Date { get; private set; }

        public HolidayInfo(int id, string name, DateTime date)
        {
            Id = id;
            Name = name;
            Date = date;
        }
    }
}
