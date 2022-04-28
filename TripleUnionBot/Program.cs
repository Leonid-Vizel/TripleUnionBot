using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Text;
using TripleUnionBot;
using TripleUnionBot.Classes;

#region Main Block
DataBank.UnionInfo = new UnionInfo();
DiscordSocketClient _client = new DiscordSocketClient(); //<-- Создание объекта клиента
CommandHandler handler = new CommandHandler(_client, new CommandService(new CommandServiceConfig())); //<-- Настройка обработчика текстовых команд
await handler.InstallCommandsAsync(); //<- Настройка комманд бота
_client.Ready += ConfigureCommands; //<- Настройка слэш-комманд бота
_client.SlashCommandExecuted += SlashCommandHandler;//<- Настройка обработки слэш-комманд бота
_client.ModalSubmitted += HandleModalSubmit;
_client.ButtonExecuted += HandleButtonClick; //<-- Настройка обработки кнопок
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

async Task HandleButtonClick(SocketMessageComponent component)
{
    EmbedBuilder embedBuilder = new EmbedBuilder();
    ComponentBuilder buttonBuilder = new ComponentBuilder();
    switch (component.Data.CustomId)
    {
        case "InfoMenu":
            EmbedButtonMenus.ApplyInfoMenu(embedBuilder,buttonBuilder);
            await component.RespondAsync(null, new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
            break;
        case "MoneyControl":
            EmbedButtonMenus.ApplyMoneyControl(embedBuilder, buttonBuilder);
            await component.RespondAsync(null, new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
            break;
        case "CreditsControl":
            EmbedButtonMenus.ApplyCreditsControl(embedBuilder, buttonBuilder);
            await component.RespondAsync(null, new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
            break;
        case "HolidayControl":
            EmbedButtonMenus.ApplyHolidayControl(embedBuilder, buttonBuilder);
            await component.RespondAsync(null, new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
            break;
        case "AddMoneyMenu":
            EmbedButtonMenus.ApplyAddMoneyMenu(buttonBuilder);
            await component.RespondAsync("Выберите от лица кого будет начисление:", components: buttonBuilder.Build());
            break;
        case "SpendMoneyMenu":
            EmbedButtonMenus.ApplyRemoveMoneyMenu(buttonBuilder);
            await component.RespondAsync("Выберите от лица кого будет начисление:", components: buttonBuilder.Build());
            break;
        case "EmilMaksudovInvestment":
        case "EmilMumdzhiInvestment":
        case "NikitaInvestment":
        case "GeneralInvestment":
            await component.RespondWithModalAsync(EmbedButtonMenus.ApplyInestment(component.Data.CustomId).Build());
            break;
        case "EmilMaksudovSpend":
        case "EmilMumdzhiSpend":
        case "NikitaSpend":
        case "GeneralSpend":
            await component.RespondWithModalAsync(EmbedButtonMenus.ApplySpend(component.Data.CustomId).Build());
            break;
        default:
            return;
    }
    await component.Message.DeleteAsync();
}

async Task HandleModalSubmit(SocketModal modal)
{
    switch (modal.Data.CustomId)
    {
        case "EmilMaksudovInvestmentModal":
        case "EmilMumdzhiInvestmentModal":
        case "NikitaInvestmentModal":
        case "GeneralInvestmentModal":
            UnionMember currentMember = 0;
            switch (modal.Data.CustomId)
            {
                case "EmilMaksudovInvestment":
                    currentMember = UnionMember.EmilMaksudov;
                    break;
                case "EmilMumdzhiInvestment":
                    currentMember = UnionMember.EmilMumdzhi;
                    break;
                case "NikitaInvestment":
                    currentMember = UnionMember.NikitaGordeev;
                    break;
                case "GeneralInvestment":
                    currentMember = UnionMember.General;
                    break;
            }
            SocketMessageComponentData? componentData = modal.Data.Components.First();
            if (decimal.TryParse(componentData.Value.Replace(",", "."), out decimal parseResult))
            {
                if (parseResult > 0)
                {
                    DataBank.UnionInfo.ExecuteAddition(currentMember, parseResult);
                    EmbedBuilder embedBuilder = new EmbedBuilder();
                    ComponentBuilder buttonBuilder = new ComponentBuilder();
                    EmbedButtonMenus.ApplyMoneyControl(embedBuilder, buttonBuilder);
                    await modal.RespondAsync("Успешно зачислено", new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
                }
                else
                {
                    await modal.RespondAsync("Зачиление не может быть 0 или отрицательным числом. Для этого используйте списания");
                }
            }
            else
            {
                await modal.RespondAsync("Не удалось преоразовать в число, извините.");
            }
            break;
    }
}

async Task SlashCommandHandler(SocketSlashCommand command)
{
    switch (command.Data.Name)
    {
        case "info":
            EmbedBuilder embedBuilder = new EmbedBuilder();
            ComponentBuilder buttonBuilder = new ComponentBuilder();
            EmbedButtonMenus.ApplyInfoMenu(embedBuilder, buttonBuilder);
            await command.RespondAsync(null, new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
            break;
    }
}