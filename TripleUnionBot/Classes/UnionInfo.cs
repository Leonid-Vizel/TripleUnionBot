﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripleUnionBot.Classes
{
    internal class UnionInfo
    {
        public decimal Money { get; private set; }
        public decimal Percent { get; private set; }
        public List<HolidayInfo> Holidays { get; private set; }
        public List<Transaction> Transactions { get; private set; }
        public List<CreditInfo> Credits { get; private set; }
        public string MainChannelId { get; private set; }

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
            Transaction transaction = new Transaction(GetNextTransactionId(), member, -money, description, DateTime.Now);
            if (money > Money)
            {
                return false;
            }
            Money -= money;
            Transactions.Add(transaction);
            //Db interation
            return true;
        }

        public void ExecuteAddition(UnionMember member, decimal money, string? description = null)
        {
            Transaction transaction = new Transaction(GetNextTransactionId(), member, money, description, DateTime.Now);
            Money += money;
            Transactions.Add(transaction);
            //Db interation
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

        public HolidayInfo? CheckIfDayIsHoliday(DateTime dateCheck)
            => Holidays.FirstOrDefault(x => x.Date.Day == dateCheck.Day && x.Date.Month == dateCheck.Month);

    }
}
