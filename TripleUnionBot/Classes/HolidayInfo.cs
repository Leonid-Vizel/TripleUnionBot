namespace TripleUnionBot.Classes
{
    internal class HolidayInfo
    {
        public int Id { get; private set; }
        public string Name { get; private set; } //Max=20
        public DateTime Date { get; private set; }

        public HolidayInfo(int id, string name, DateTime date)
        {
            Id = id;
            Name = name;
            Date = date;
        }
    }
}
