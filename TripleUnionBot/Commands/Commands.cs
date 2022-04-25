﻿using Discord.Commands;

namespace TripleUnionBot.Commands
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
		// ~say hello world -> hello world
		[Command("info")]
		[Summary("Echoes a message.")]
		public Task SayAsync([Remainder][Summary("The text to echo")] string echo)
			=> ReplyAsync(echo);

		// ReplyAsync is a method on ModuleBase 
	}
}
