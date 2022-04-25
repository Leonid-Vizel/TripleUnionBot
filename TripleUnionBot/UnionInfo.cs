using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripleUnionBot
{
    internal class UnionInfo
    {
        public decimal Money { get; private set; }
        public List<HolidayInfo> Holidays { get; private set; }
        public List<AdditionalTransaction> AdditionalTransactions { get; private set; }

        public UnionInfo()
        {
            Money = 0;
            Holidays = new List<HolidayInfo>();
            AdditionalTransactions = new List<AdditionalTransaction>();
        }

        public UnionInfo(decimal money, IEnumerable<HolidayInfo> holidays, IEnumerable<AdditionalTransaction> transactions)
        {
            Money = money;
            Holidays = holidays.ToList();
            AdditionalTransactions = transactions.ToList();
        }

        public HolidayInfo AddHoliday()
        {

        }

        public AdditionalTransaction AddTrasaction()
        {

        }

        public void RemoveHoliday()
        {

        }

        public HolidayInfo CheckIfDayIsHoliday()
        {

        }
    }
}
