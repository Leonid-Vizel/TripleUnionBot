﻿namespace TripleUnionBot.Classes
{
    internal class AdditionTransaction
    {
        public int Id { get; private set; }
        public UnionMember Member { get; private set; }
        public decimal Money { get; private set; }
        public string? Description { get; private set; }
        public DateTime ExecuteTime { get; private set; }

        public AdditionTransaction(int id, UnionMember member, decimal money, string? description, DateTime executeTime)
        {
            Id = id;
            Member = member;
            Money = money;
            Description = description;
            ExecuteTime = executeTime;
        }
    }
}
