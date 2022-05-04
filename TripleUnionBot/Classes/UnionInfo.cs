namespace TripleUnionBot.Classes
{
    internal class UnionInfo
    {
        public string ImageLink { get; private set; }
        public string Slogan { get; private set; }
        public DateTime BeginDate { get; private set; }
        public decimal Money { get; private set; }
        public decimal Percent { get; private set; }
        public List<HolidayInfo> Holidays { get; private set; }
        public List<Transaction> Transactions { get; private set; }
        public List<CreditInfo> Credits { get; private set; }
        public ulong? MainChannelId { get; private set; }

        public UnionInfo()
        {
            Money = 0;
            Holidays = new List<HolidayInfo>();
            Transactions = new List<Transaction>();
            Credits = new List<CreditInfo>();
        }

        public UnionInfo(decimal money, IEnumerable<HolidayInfo> holidays, IEnumerable<Transaction> transactions, IEnumerable<CreditInfo> credits)
        {
            Money = money;
            Holidays = holidays.ToList();
            Transactions = transactions.ToList();
            Credits = credits.ToList();
        }

        public bool SetChannelId(ulong? channelId)
        {
            if (channelId.Equals(MainChannelId))
            {
                return false;
            }
            MainChannelId = channelId;
            //DbInteraction
            return true;
        }

        public bool SetPercent(decimal updateValue)
        {
            if (updateValue < 0)
            {
                return false;
            }
            Percent = updateValue;
            //DbInteraction
            return true;
        }

        public bool ExecuteWaste(UnionMember member, decimal money, string? description = null)
        {
            if (money > Money)
            {
                return false;
            }
            Money -= money;
            Transactions.Add(new Transaction(GetNextTransactionId(), member, -money, description, DateTime.Now));
            //Db interation
            return true;
        }

        public void ExecuteAddition(UnionMember member, decimal money, string? description = null)
        {
            Money += money;
            Transactions.Add(new Transaction(GetNextTransactionId(), member, money, description, DateTime.Now));
            //Db interation
        }

        public void AddHoliday(string name, DateTime date)
        {
            HolidayInfo holidayInfo = new HolidayInfo(GetNextHolidayId(),name,date);
            Holidays.Add(holidayInfo);
            //Db interaction
        }

        public void RemoveHoliday(HolidayInfo found)
        {
            Holidays.Remove(found);
            //DbInteraction
        }

        private int GetNextTransactionId()
        {
            if (Transactions.Count > 0)
            {
                return Transactions.Max(x => x.Id) + 1;
            }
            else
            {
                return 0;
            }
        }

        private int GetNextHolidayId()
        {
            if (Holidays.Count > 0)
            {
                return Holidays.Max(x => x.Id) + 1;
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
