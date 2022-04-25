using Discord.Commands;

namespace TripleUnionBot.Commands
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
		[Command("info")]
		[Summary("Echoes a message.")]
		public Task SayAsync([Remainder][Summary("The text to echo")] string echo)
			=> ReplyAsync(echo);
	}
}
