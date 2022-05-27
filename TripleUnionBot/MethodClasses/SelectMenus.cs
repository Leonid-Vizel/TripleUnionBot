using Discord.WebSocket;
using TripleUnionBot.Classes;

namespace TripleUnionBot.MethodClasses
{
    public static class SelectMenus
    {
        public static async Task InvestmentMenu(SocketMessageComponent component)
        {
            await component.RespondWithModalAsync(EmbedButtonMenus.ApplyInestment(component.Data.Values.First()).Build());
            await component.Message.DeleteAsync();
        }

        public static async Task SpendMenu(SocketMessageComponent component)
        {
            await component.RespondWithModalAsync(EmbedButtonMenus.ApplySpend(component.Data.Values.First()).Build());
            await component.Message.DeleteAsync();
        }
    }

    public delegate Task SelectMenusHandler(SocketMessageComponent component);
}
