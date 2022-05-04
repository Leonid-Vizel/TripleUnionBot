using TripleUnionBot.Classes;

namespace TripleUnionBot
{
    internal static class DataBank
    {
        public static UnionInfo UnionInfo { get; set; }
        public static ulong GuildId { get; set; }

        static DataBank()
        {
            GuildId = 886180298239402034;
        }
    }
}
