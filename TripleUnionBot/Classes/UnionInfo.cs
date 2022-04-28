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
        public List<AdditionTransaction> Additions { get; private set; }
        public List<WasteTransaction> Wastes { get; private set; }
        public List<CreditInfo> Credits { get; private set; }
        public string MainChannelId { get; private set; }

        public UnionInfo()
        {
            Money = 0;
            Holidays = new List<HolidayInfo>();
            Additions = new List<AdditionTransaction>();
            Wastes = new List<WasteTransaction>();
            Credits = new List<CreditInfo>();
        }

        public UnionInfo(decimal money, IEnumerable<HolidayInfo> holidays, IEnumerable<AdditionTransaction> transactions, IEnumerable<CreditInfo> credits)
        {
            Money = money;
            Holidays = holidays.ToList();
            Additions = transactions.ToList();
            Wastes = new List<WasteTransaction>();
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

        public bool ExecuteWaste(UnionMember member, decimal money, string? description = null)
        {
            WasteTransaction transaction = new WasteTransaction(GetNextWasteId(), member, money, description, DateTime.Now);
            if (transaction.Money > Money)
            {
                return false;
            }
            Money -= transaction.Money;
            Wastes.Add(transaction);
            //Db interation
            return true;
        }

        public void ExecuteAddition(UnionMember member, decimal money, string? description = null)
        {
            AdditionTransaction transaction = new AdditionTransaction(GetNextAdditionId(), member, money, description, DateTime.Now);
            Money += transaction.Money;
            Additions.Add(transaction);
            //Db interation
        }

        private int GetNextAdditionId()
        {
            if (Additions.Count > 0)
            {
                return Additions.Max(x => x.Id) + 1;
            }
            else
            {
                return 0;
            }
        }

        private int GetNextWasteId()
        {
            if (Additions.Count > 0)
            {
                return Additions.Max(x => x.Id) + 1;
            }
            else
            {
                return 0;
            }
        }

        public HolidayInfo? CheckIfDayIsHoliday(DateTime dateCheck)
            => Holidays.FirstOrDefault(x => x.Date.Day == dateCheck.Day && x.Date.Month == dateCheck.Month);

    }
}
