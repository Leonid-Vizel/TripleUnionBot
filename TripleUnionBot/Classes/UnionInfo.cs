using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripleUnionBot.Classes
{
    internal class UnionInfo
    {
        public decimal Money { get; private set; }
        public List<HolidayInfo> Holidays { get; private set; }
        public List<AdditionTransaction> Additionals { get; private set; }
        public List<WasteTransaction> Wastes { get; private set; }
        public List<CreditInfo> Credits { get; private set; }
        public string MainChannelId { get; private set; }

        public UnionInfo()
        {
            Money = 0;
            Holidays = new List<HolidayInfo>();
            Additionals = new List<AdditionTransaction>();
            Credits = new List<CreditInfo>();
        }

        public UnionInfo(decimal money, IEnumerable<HolidayInfo> holidays, IEnumerable<AdditionTransaction> transactions, IEnumerable<CreditInfo> credits)
        {
            Money = money;
            Holidays = holidays.ToList();
            Additionals = transactions.ToList();
            Credits = credits.ToList();
        }

        public bool SetChannelId(string channelId)
        {
            if (channelId.Equals(MainChannelId))
            {
                return false;
            }
            MainChannelId = channelId;
            //DbInteraction
            return true;
        }

        public bool ExecuteWaste(WasteTransaction transaction)
        {
            if (transaction.Money > Money)
            {
                return false;
            }
            Money -= transaction.Money;
            Wastes.Add(transaction);
            //Db interation
            return true;
        }

        public void ExecuteAddition(AdditionTransaction transaction)
        {
            Money += transaction.Money;
            Additionals.Add(transaction);
            //Db interation
        }

        public HolidayInfo? CheckIfDayIsHoliday(DateTime dateCheck)
            => Holidays.FirstOrDefault(x => x.Date.Day == dateCheck.Day && x.Date.Month == dateCheck.Month);

    }
}
