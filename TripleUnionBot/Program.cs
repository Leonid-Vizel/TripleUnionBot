using Discord;
using Discord.WebSocket;

#region Main Block
DiscordSocketClient _client = new DiscordSocketClient(); //<-- Создание объекта клиента
_client.MessageReceived += MessageReceived;
var token = File.ReadAllText("bot.token"); //<-- Считывание токена бота
await _client.LoginAsync(TokenType.Bot, token); //<-- Бот логинится
await _client.StartAsync(); //<-- Бот запускается
await Task.Delay(-1); //<-- Чтобы прога не закрывалась раньше времени
#endregion

async Task MessageReceived(SocketMessage message)
{
	
}