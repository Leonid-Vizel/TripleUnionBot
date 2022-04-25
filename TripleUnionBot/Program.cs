using Discord;
using Discord.WebSocket;

#region Main Block
DiscordSocketClient _client = new DiscordSocketClient(); //<-- Создание объекта клиента
_client.Log += Log; //<-- Подписка на событие логгирования
_client.MessageUpdated += MessageUpdated;
var token = File.ReadAllText("bot.token"); //<-- Считывание токена бота
await _client.LoginAsync(TokenType.Bot, token); //<-- Бот логинится
await _client.StartAsync(); //<-- Бот запускается
await Task.Delay(-1); //<-- Чтобы прога не закрывалась раньше времени
#endregion

Task Log(LogMessage msg)
{
	Console.WriteLine(msg.ToString());
	return Task.CompletedTask;
}

async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
{
	// If the message was not in the cache, downloading it will result in getting a copy of `after`.
	var message = await before.GetOrDownloadAsync();
	Console.WriteLine($"{message} -> {after}");
}