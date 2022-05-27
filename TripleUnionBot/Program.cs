using Discord;
using Discord.WebSocket;
using System.Timers;
using TripleUnionBot;
using TripleUnionBot.Classes;
using TripleUnionBot.MethodClasses;

#region Preparing Components
Dictionary<string, SelectMenusHandler> selectMenuActions = new Dictionary<string, SelectMenusHandler>();
Dictionary<string, SlashCommandsHandler> commandActions = new Dictionary<string, SlashCommandsHandler>();
Dictionary<string, ButtonsHandler> buttonActions = new Dictionary<string, ButtonsHandler>();
Dictionary<string, ModalsHandler> modalActions = new Dictionary<string, ModalsHandler>();
ConfigureDictionaries();
#endregion

#region Main Block
System.Timers.Timer? holidayCheckTimer = null;
DataBank.UnionInfo = new UnionInfo();
DiscordSocketClient _client = new DiscordSocketClient(); //<-- Создание объекта клиента
ConfigureEvents();
await StartBot();
ConfigureHolidays();
await Task.Delay(-1); //<-- Чтобы прога не закрывалась раньше времени
#endregion

#region Setting Up
void ConfigureHolidays()
{
    DateTime TimeToExecuteTask;
    if (DateTime.Now.Hour < 12)
    {
        TimeToExecuteTask = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0);
    }
    else
    {
        TimeToExecuteTask = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1, 12, 0, 0);
    }
    holidayCheckTimer = new System.Timers.Timer((TimeToExecuteTask - DateTime.Now).TotalMilliseconds);
    holidayCheckTimer.Elapsed += CheckTodayHoliday;

}

void ConfigureEvents()
{
    _client.Ready += ConfigureSlashCommands; //<- Настройка слэш-комманд бота
    _client.SlashCommandExecuted += SlashCommandHandler;//<- Настройка обработки слэш-комманд бота
    _client.ModalSubmitted += HandleModalSubmit; //<-- Настройка обработки вопросов
    _client.ButtonExecuted += HandleButtonClick; //<-- Настройка обработки кнопок
    _client.SelectMenuExecuted += HandleSelectMenu; //<-- Найтройка обработки выпадающих меню
}

void ConfigureDictionaries()
{
    selectMenuActions.Add("InvestmentMenu", SelectMenus.InvestmentMenu);
    selectMenuActions.Add("SpendMenu", SelectMenus.SpendMenu);

    commandActions.Add("fin", Commands.FinCommand);
    commandActions.Add("seleb", Commands.SelebCommand);
    commandActions.Add("info", Commands.InfoCommand);
    commandActions.Add("random", Commands.RandomCommand);

    buttonActions.Add("InfoMenu", Buttons.InfoMenu);
    buttonActions.Add("MoneyControl", Buttons.MoneyControl);
    buttonActions.Add("CreditsControl", Buttons.CreditsControl);
    buttonActions.Add("HolidayControl", Buttons.HolidayControl);
    buttonActions.Add("AddHoliday", Buttons.AddHoliday);
    buttonActions.Add("RemoveHoliday", Buttons.RemoveHoliday);
    buttonActions.Add("AddMoneyMenu", Buttons.AddMoneyMenu);
    buttonActions.Add("SpendMoneyMenu", Buttons.SpendMoneyMenu);
    buttonActions.Add("SetPercent", Buttons.SetPercent);
    buttonActions.Add("Settings", Buttons.Settings);
    buttonActions.Add("SetCurrentChannel", Buttons.SetCurrentChannel);
    buttonActions.Add("SetDefault", Buttons.SetDefault);
    buttonActions.Add("ListHoliday", Buttons.ListHoliday);

    modalActions.Add("PercentModal", Modals.PercentModal);
    modalActions.Add("HolidayRemoveModal", Modals.HolidayRemoveModal);
    modalActions.Add("HolidayAddModal", Modals.HolidayAddModal);
    modalActions.Add("EmilMaksudovInvestmentModal", Modals.InvestmentModal);
    modalActions.Add("EmilMumdzhiInvestmentModal", Modals.InvestmentModal);
    modalActions.Add("NikitaInvestmentModal", Modals.InvestmentModal);
    modalActions.Add("GeneralInvestmentModal", Modals.InvestmentModal);
    modalActions.Add("EmilMaksudovSpendModal", Modals.InvestmentModal);
    modalActions.Add("EmilMumdzhiSpendModal", Modals.InvestmentModal);
    modalActions.Add("NikitaSpendModal", Modals.InvestmentModal);
    modalActions.Add("GeneralSpendModal", Modals.InvestmentModal);
}

async Task StartBot()
{
    var token = File.ReadAllText("bot.token"); //<-- Считывание токена бота
    await _client.LoginAsync(TokenType.Bot, token); //<-- Бот логинится
    await _client.StartAsync(); //<-- Бот запускается
}

async Task ConfigureSlashCommands()
{
    SlashCommandBuilder[] commandBuilders = new SlashCommandBuilder[]
    {
        new SlashCommandBuilder().WithName("fin").WithDescription("Показывает информацию о казне союза"),
        new SlashCommandBuilder().WithName("info").WithDescription("Показывает информацию о союзе"),
        new SlashCommandBuilder().WithName("seleb").WithDescription("Праздники союза"),
        new SlashCommandBuilder().WithName("rules").WithDescription("Правила союза"),
        new SlashCommandBuilder().WithName("random").WithDescription("Выдаст рандомного участника союза")
    };
    try
    {
        SocketGuild? guild = _client.GetGuild(DataBank.GuildId);
        if (guild != null)
        {
            foreach (SlashCommandBuilder commandBuilder in commandBuilders)
            {
                await _client.CreateGlobalApplicationCommandAsync(commandBuilder.Build());
                Console.WriteLine($"{commandBuilder.Name} is registered");
            }
        }
        else
        {
            Console.WriteLine($"Shit, fuck: guild is null!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Shit, fuck: {ex}");
    }
}
#endregion

async Task HandleButtonClick(SocketMessageComponent component)
{
    if (buttonActions.ContainsKey(component.Data.CustomId))
    {
        await buttonActions[component.Data.CustomId].Invoke(component);
    }
    else
    {
        await buttonActions["ListHoliday"].Invoke(component);
    }
}

async Task HandleModalSubmit(SocketModal modal)
{
    if (modalActions.ContainsKey(modal.Data.CustomId))
    {
        await modalActions[modal.Data.CustomId].Invoke(modal);
    }
}

async Task HandleSelectMenu(SocketMessageComponent component)
{
    if (component.Data.Values.Count == 0)
    {
        return; //<-- Для избежания ошибки
    }
    if (selectMenuActions.ContainsKey(component.Data.CustomId))
    {
        await selectMenuActions[component.Data.CustomId].Invoke(component);
    }
}

async Task SlashCommandHandler(SocketSlashCommand command)
{
    if (commandActions.ContainsKey(command.Data.Name))
    {
        await commandActions[command.Data.Name].Invoke(command);
    }
}

async void CheckTodayHoliday(object? sender, ElapsedEventArgs e)
{
    holidayCheckTimer.Stop();
    HolidayInfo? foundHoliday = DataBank.UnionInfo.CheckIfDayIsHoliday(DateTime.Today);
    if (foundHoliday != null && DataBank.UnionInfo.MainChannelId != null)
    {
        SocketTextChannel? channel = _client.GetGuild(DataBank.GuildId).GetChannel(DataBank.UnionInfo.MainChannelId.Value) as SocketTextChannel;
        if (channel != null)
        {
            await channel.SendMessageAsync($"{channel.Mention}, с праздником!\nСегодня {foundHoliday.Name}");
        }
        else
        {
            DataBank.UnionInfo.SetChannelId(null);
        }
    }
    DateTime TimeToExecuteTask = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1, 12, 0, 0);
    holidayCheckTimer.Interval = (TimeToExecuteTask - DateTime.Now).TotalMilliseconds;
    holidayCheckTimer.Start();
}