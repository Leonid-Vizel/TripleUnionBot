using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TripleUnionBot;

#region Main Block
DataBank.UnionInfo = new UnionInfo();
DiscordSocketClient _client = new DiscordSocketClient(); //<-- Создание объекта клиента
CommandHandler handler = new CommandHandler(_client, new CommandService(new CommandServiceConfig()));
await handler.InstallCommandsAsync(); //<- Настройка комманд бота
_client.Ready += ConfigureCommands;
_client.SlashCommandExecuted += SlashCommandHandler;
var token = File.ReadAllText("bot.token"); //<-- Считывание токена бота
await _client.LoginAsync(TokenType.Bot, token); //<-- Бот логинится
await _client.StartAsync(); //<-- Бот запускается
await Task.Delay(-1); //<-- Чтобы прога не закрывалась раньше времени
#endregion

async Task ConfigureCommands()
{
    var commandBuilder = new SlashCommandBuilder();
    // regex: ^[\w-]{3,32}$
    commandBuilder.WithName("info");
    commandBuilder.WithDescription("Показывает информацию о состоянии союза!");
    try
    {
        await _client.GetGuild(886180298239402034).CreateApplicationCommandAsync(commandBuilder.Build());
        Console.WriteLine($"{commandBuilder.Name} is registered");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Shit, fuck: {ex}");
    }
}

async Task SlashCommandHandler(SocketSlashCommand command)
{
    switch (command.Data.Name)
    {
        case "info":
            //Создаю Embed
            EmbedBuilder builder = new EmbedBuilder();
            builder.Title = "Состояние казны";
            EmbedFooterBuilder footerbuilder = new EmbedFooterBuilder();
            footerbuilder.Text = $"Информация на {DateTime.Now}";
            builder.Footer = footerbuilder;
            builder.AddField("Баланс:", $"{DataBank.UnionInfo.Money} ₽", true);
            builder.AddField("Процент:", "10%", true);
            if (false)
            {
                builder.AddField("Сегодня:", "[Эта строка только если сегодня праздник]");
            }
            //Создаю кнопки
            ComponentBuilder buttonBuilder = new ComponentBuilder();
            buttonBuilder.WithButton("Вклад", "MoneyControl");
            buttonBuilder.WithButton("Кредиты", "CreditsControl");
            buttonBuilder.WithButton("Праздники", "HolidayControl");

            await command.RespondAsync(null, new Embed[1] { builder.Build() },components: buttonBuilder.Build());
            break;
    }
}