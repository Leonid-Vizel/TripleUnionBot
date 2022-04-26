namespace TripleUnionBot.Classes
{
    internal class WasteTransaction
    {
        public int Id { get; private set; }
        public string Username { get; private set; }
        public decimal Money { get; private set; }

        public WasteTransaction(int id, string username, decimal money)
        {
            Id = id;
            Username = username;
            Money = money;
        }
    }
}
