using Discord;
using Discord.WebSocket;
using TripleUnionBot.Classes;

namespace TripleUnionBot.MethodClasses
{
    public static class Buttons
    {
        public static async Task InfoMenu(SocketMessageComponent component)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            ComponentBuilder buttonBuilder = new ComponentBuilder();
            EmbedButtonMenus.ApplyInfoMenu(embedBuilder, buttonBuilder);
            await component.UpdateAsync(x =>
            {
                x.Content = null;
                x.Embeds = new Embed[1] { embedBuilder.Build() };
                x.Components = buttonBuilder.Build();
            });
        }

        public static async Task MoneyControl(SocketMessageComponent component)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            ComponentBuilder buttonBuilder = new ComponentBuilder();
            EmbedButtonMenus.ApplyMoneyControl(embedBuilder, buttonBuilder);
            await component.UpdateAsync(x =>
            {
                x.Content = null;
                x.Embeds = new Embed[1] { embedBuilder.Build() };
                x.Components = buttonBuilder.Build();
            });
        }

        public static async Task CreditsControl(SocketMessageComponent component)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            ComponentBuilder buttonBuilder = new ComponentBuilder();
            EmbedButtonMenus.ApplyCreditsControl(embedBuilder, buttonBuilder);
            await component.UpdateAsync(x =>
            {
                x.Content = null;
                x.Embeds = new Embed[1] { embedBuilder.Build() };
                x.Components = buttonBuilder.Build();
            });
        }

        public static async Task HolidayControl(SocketMessageComponent component)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            ComponentBuilder buttonBuilder = new ComponentBuilder();
            EmbedButtonMenus.ApplyHolidayControl(embedBuilder, buttonBuilder);
            await component.UpdateAsync(x =>
            {
                x.Content = null;
                x.Embeds = new Embed[1] { embedBuilder.Build() };
                x.Components = buttonBuilder.Build();
            });
        }

        public static async Task AddHoliday(SocketMessageComponent component)
        {
            await component.RespondWithModalAsync(EmbedButtonMenus.ApplyAddHoliday().Build());
            await component.Message.DeleteAsync();
        }

        public static async Task RemoveHoliday(SocketMessageComponent component)
        {
            await component.RespondWithModalAsync(EmbedButtonMenus.ApplyRemoveHoliday().Build());
            await component.Message.DeleteAsync();
        }

        public static async Task AddMoneyMenu(SocketMessageComponent component)
        {
            ComponentBuilder buttonBuilder = new ComponentBuilder();
            EmbedButtonMenus.ApplyAddMoneyMenu(buttonBuilder);
            await component.UpdateAsync(x =>
            {
                x.Content = null;
                x.Embeds = null;
                x.Components = buttonBuilder.Build();
            });
        }

        public static async Task SpendMoneyMenu(SocketMessageComponent component)
        {
            ComponentBuilder buttonBuilder = new ComponentBuilder();
            EmbedButtonMenus.ApplyRemoveMoneyMenu(buttonBuilder);
            await component.UpdateAsync(x =>
            {
                x.Content = null;
                x.Embeds = null;
                x.Components = buttonBuilder.Build();
            });
        }

        public static async Task SetPercent(SocketMessageComponent component)
        {
            await component.RespondWithModalAsync(EmbedButtonMenus.ApplySetPercent().Build());
            await component.Message.DeleteAsync();
        }

        public static async Task Settings(SocketMessageComponent component)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            ComponentBuilder buttonBuilder = new ComponentBuilder();
            EmbedButtonMenus.ApplySettings(GetClientFromComponent(component), embedBuilder, buttonBuilder);
            await component.UpdateAsync(x =>
            {
                x.Content = null;
                x.Embeds = new Embed[1] { embedBuilder.Build() };
                x.Components = buttonBuilder.Build();
            });
        }

        public static async Task SetCurrentChannel(SocketMessageComponent component)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            ComponentBuilder buttonBuilder = new ComponentBuilder();
            DataBank.UnionInfo.SetChannelId(component.Message.Channel.Id);
            EmbedButtonMenus.ApplySettings(GetClientFromComponent(component), embedBuilder, buttonBuilder);
            await component.UpdateAsync(x =>
            {
                x.Content = null;
                x.Embeds = new Embed[1] { embedBuilder.Build() };
                x.Components = buttonBuilder.Build();
            });
        }

        public static async Task SetDefault(SocketMessageComponent component)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            ComponentBuilder buttonBuilder = new ComponentBuilder();
            DataBank.UnionInfo.SetChannelId(null);
            EmbedButtonMenus.ApplySettings(GetClientFromComponent(component), embedBuilder, buttonBuilder);
            await component.UpdateAsync(x =>
            {
                x.Content = null;
                x.Embeds = new Embed[1] { embedBuilder.Build() };
                x.Components = buttonBuilder.Build();
            });
        }

        public static async Task ListHoliday(SocketMessageComponent component)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            ComponentBuilder buttonBuilder = new ComponentBuilder();
            string pageString = component.Data.CustomId.Split(":")[1];
            if (int.TryParse(pageString, out int pageParsed))
            {
                EmbedButtonMenus.ApplyHolidayList(pageParsed, embedBuilder, buttonBuilder);
                await component.UpdateAsync(x =>
                {
                    x.Content = null;
                    x.Embeds = new Embed[1] { embedBuilder.Build() };
                    x.Components = buttonBuilder.Build();
                });
            }
            else
            {
                EmbedButtonMenus.ApplyInfoMenu(embedBuilder, buttonBuilder);
                await component.RespondWithModalAsync(EmbedButtonMenus.ApplySpend(component.Data.CustomId).Build());
            }
        }

        private static DiscordSocketClient GetClientFromComponent(SocketMessageComponent component)
            => typeof(SocketEntity<ulong>).GetProperty("Discord", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(component) as DiscordSocketClient;
    }

    public delegate Task ButtonsHandler(SocketMessageComponent component);
}
