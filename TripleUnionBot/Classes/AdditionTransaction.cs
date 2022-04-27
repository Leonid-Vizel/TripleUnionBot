namespace TripleUnionBot.Classes
{
    internal class AdditionTransaction
    {
        public int Id { get; private set; }
        public UnionMember Member { get; private set; }
        public decimal Money { get; private set; }

        public AdditionTransaction(int id, UnionMember member, decimal money)
        {
            Id = id;
            Member = member;
            Money = money;
        }
    }
}
