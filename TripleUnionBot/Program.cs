﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TripleUnionBot;
using TripleUnionBot.Classes;

#region Main Block
DataBank.UnionInfo = new UnionInfo();
DiscordSocketClient _client = new DiscordSocketClient(); //<-- Создание объекта клиента
CommandHandler handler = new CommandHandler(_client, new CommandService(new CommandServiceConfig())); //<-- Настройка обработчика текстовых команд
await handler.InstallCommandsAsync(); //<- Настройка комманд бота
_client.Ready += ConfigureCommands; //<- Настройка слэш-комманд бота
_client.SlashCommandExecuted += SlashCommandHandler;//<- Настройка обработки слэш-комманд бота
_client.ButtonExecuted += MyButtonHandler; //<-- Настройка обработки кнопок
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

async Task MyButtonHandler(SocketMessageComponent component)
{
    EmbedBuilder embedBuilder = new EmbedBuilder()
            .WithFooter(new EmbedFooterBuilder().WithText($"Информация на {DateTime.Now}"));
    ComponentBuilder buttonBuilder = new ComponentBuilder();
    switch (component.Data.CustomId)
    {
        case "InfoMenu":
            //Создаю Embed
            embedBuilder.Title = "Состояние казны";
            embedBuilder.AddField("Баланс:", $"{DataBank.UnionInfo.Money} ₽", true);
            embedBuilder.AddField("Процент:", "10%", true);
            if (false)
            {
                embedBuilder.AddField("Сегодня:", "[Эта строка только если сегодня праздник]");
            }
            //Создаю кнопки
            buttonBuilder.WithButton("Вклад", "MoneyControl");
            buttonBuilder.WithButton("Кредиты", "CreditsControl");
            buttonBuilder.WithButton("Праздники", "HolidayControl");

            await component.RespondAsync(null, new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
            break;
        case "MoneyControl":
            embedBuilder.Title = "Информация о вкладе";
            embedBuilder.AddField("Баланс:", $"{DataBank.UnionInfo.Money} ₽", true);
            embedBuilder.AddField("Процент:", "10%", true);
            //Создаю кнопки
            buttonBuilder.WithButton("Добавить", "AddMoney");
            buttonBuilder.WithButton("Потратить", "SpendMoney");
            buttonBuilder.WithButton("Изменить процент", "SetPercent");
            buttonBuilder.WithButton("Назад", "InfoMenu");
            await component.RespondAsync(null, new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
            break;
        case "CreditsControl":
            embedBuilder.Title = "Информация о кредитах";
            embedBuilder.AddField("Всего кредитов:", DataBank.UnionInfo.Credits.Count, true);
            embedBuilder.AddField("Процент:", "10%", true);
            //Создаю кнопки
            buttonBuilder.WithButton("Добавить кредит", "AddMoney");
            buttonBuilder.WithButton("Закрыть кредит", "SpendMoney");
            buttonBuilder.WithButton("Назад", "InfoMenu");
            await component.RespondAsync(null, new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
            break;
        case "HolidayControl":
            await component.RespondAsync($"{component.User.Mention} has clicked the button!");
            break;
        default:
            return;
    }
    await component.Message.DeleteAsync();
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