using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TripleUnionBot;

#region Main Block
DiscordSocketClient _client = new DiscordSocketClient(); //<-- Создание объекта клиента
CommandHandler handler = new CommandHandler(_client, new CommandService(new CommandServiceConfig()));
await handler.InstallCommandsAsync(); //<- Настройка комманд бота
var token = File.ReadAllText("bot.token"); //<-- Считывание токена бота
await _client.LoginAsync(TokenType.Bot, token); //<-- Бот логинится
await _client.StartAsync(); //<-- Бот запускается
await Task.Delay(-1); //<-- Чтобы прога не закрывалась раньше времени
#endregion
