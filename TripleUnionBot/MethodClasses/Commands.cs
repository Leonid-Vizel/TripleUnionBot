using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripleUnionBot.Classes;

namespace TripleUnionBot.MethodClasses
{
    public static class Commands
    {
        public static async Task FinCommand(SocketSlashCommand command)
        {
            EmbedBuilder finEmbedBuilder = new EmbedBuilder();
            ComponentBuilder finbButtonBuilder = new ComponentBuilder();
            EmbedButtonMenus.ApplyInfoMenu(finEmbedBuilder, finbButtonBuilder);
            await command.RespondAsync(null, new Embed[] { finEmbedBuilder.Build() }, components: finbButtonBuilder.Build());
        }

        public static async Task SelebCommand(SocketSlashCommand command)
        {
            EmbedBuilder selebEmbedBuilder = new EmbedBuilder();
            ComponentBuilder selebButtonBuilder = new ComponentBuilder();
            EmbedButtonMenus.ApplyHolidayControl(selebEmbedBuilder, selebButtonBuilder);
            await command.RespondAsync(null, new Embed[] { selebEmbedBuilder.Build() }, components: selebButtonBuilder.Build());
        }

        public static async Task InfoCommand(SocketSlashCommand command)
        {
            EmbedBuilder infoEmbedBuilder = new EmbedBuilder();
            ComponentBuilder infoButtonBuilder = new ComponentBuilder();
            EmbedButtonMenus.ApplyInfoMenu(infoEmbedBuilder, infoButtonBuilder);
            await command.RespondAsync(null, new Embed[] { infoEmbedBuilder.Build() }, components: infoButtonBuilder.Build());
        }

        public static async Task RandomCommand(SocketSlashCommand command)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int number = random.Next(1, 4);
            string member = "";
            switch (number)
            {
                case 1:
                    member = "Мумджи Эмиль Шамилевич";
                    break;
                case 2:
                    member = "Гордеев Никита Андреевич";
                    break;
                case 3:
                    member = "Максудов Эмиль Альбертович";
                    break;
            }
            await command.RespondAsync($"Вам выпало число {number}.\nСоответствующий участник: {member}");
        }
    }

    public delegate Task SlashCommandsHandler(SocketSlashCommand command);
}
