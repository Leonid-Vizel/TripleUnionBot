﻿namespace TripleUnionBot.Classes
{
    internal class WasteTransaction
    {
        public int Id { get; private set; }
        public UnionMember Member { get; private set; }
        public decimal Money { get; private set; }

        public WasteTransaction(int id, UnionMember member, decimal money)
        {
            Id = id;
            Member = member;
            Money = money;
        }
    }
}
