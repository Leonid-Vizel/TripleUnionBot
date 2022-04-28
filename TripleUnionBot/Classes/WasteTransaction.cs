namespace TripleUnionBot.Classes
{
    internal class WasteTransaction
    {
        public int Id { get; private set; }
        public UnionMember Member { get; private set; }
        public decimal Money { get; private set; }
        public string? Description { get; private set; }
        public DateTime ExecuteTime { get; private set; }

        public WasteTransaction(int id, UnionMember member, decimal money, string? description, DateTime executeTime)
        {
            Id = id;
            Member = member;
            Money = money;
            Description = description;
            ExecuteTime = executeTime;
        }
    }
}
