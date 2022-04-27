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
    EmbedBuilder embedBuilder = new EmbedBuilder()
            .WithFooter(new EmbedFooterBuilder().WithText($"Информация на {DateTime.Now}"));
    ComponentBuilder buttonBuilder = new ComponentBuilder();
    switch (component.Data.CustomId)
    {
        case "InfoMenu":
            embedBuilder.Title = "Состояние казны";
            embedBuilder.AddField("Баланс:", $"{DataBank.UnionInfo.Money} ₽", true);
            embedBuilder.AddField("Процент:", "10%", true);
            HolidayInfo? foundHoliday = DataBank.UnionInfo.CheckIfDayIsHoliday(DateTime.Today);
            if (foundHoliday != null)
            {
                embedBuilder.AddField("Сегодня:", foundHoliday.Name);
            }
            buttonBuilder.WithButton("Вклад", "MoneyControl");
            buttonBuilder.WithButton("Кредиты", "CreditsControl");
            buttonBuilder.WithButton("Праздники", "HolidayControl");
            buttonBuilder.WithButton("Настройки", "Settings");
            await component.RespondAsync(null, new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
            break;
        case "MoneyControl":
            embedBuilder.Title = "Информация о вкладе";
            embedBuilder.AddField("Баланс:", $"{DataBank.UnionInfo.Money} ₽", true);
            embedBuilder.AddField("Процент:", "10%", true);

            buttonBuilder.WithButton("Добавить", "AddMoneyMenu");
            buttonBuilder.WithButton("Потратить", "SpendMoneyMenu");
            buttonBuilder.WithButton("Изменить процент", "SetPercent");
            buttonBuilder.WithButton("Назад", "InfoMenu");
            await component.RespondAsync(null, new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
            break;
        case "CreditsControl":
            embedBuilder.Title = "Информация о кредитах";
            embedBuilder.AddField("Всего кредитов:", DataBank.UnionInfo.Credits.Count, true);
            embedBuilder.AddField("Процент:", "10%", true);

            buttonBuilder.WithButton("Добавить кредит", "AddMoney");
            buttonBuilder.WithButton("Закрыть кредит", "SpendMoney");
            buttonBuilder.WithButton("Назад", "InfoMenu");
            await component.RespondAsync(null, new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
            break;
        case "HolidayControl":
            embedBuilder.Title = "Информация о праздниках";
            embedBuilder.AddField("Всего праздников:", DataBank.UnionInfo.Credits.Count, true);
            HolidayInfo? foundInfo = DataBank.UnionInfo.CheckIfDayIsHoliday(DateTime.Today);
            if (foundInfo != null)
            {
                embedBuilder.AddField("Сегодня:", foundInfo.Name, true);
            }
            if (DataBank.UnionInfo.Credits.Count > 0)
            {
                StringBuilder listBuilder = new StringBuilder();
                int pageCount = 0;
                foreach (HolidayInfo info in DataBank.UnionInfo.Holidays.OrderBy(x => x.Date))
                {
                    string line = $"[{info.Date.ToString("dd.MM.yyyy")}] {info.Name}";
                    if (listBuilder.Length + line.Length > 1024)
                    {
                        embedBuilder.AddField($"Страница {++pageCount}", listBuilder.ToString());
                        listBuilder.Clear();
                    }
                    else
                    {
                        listBuilder.AppendLine($"[{info.Date.ToString("dd.MM.yyyy")}] {info.Name}");
                    }
                }
                if (pageCount == 0)
                {
                    embedBuilder.AddField($"Список", listBuilder.ToString());
                }
                else if (listBuilder.Length > 0)
                {
                    embedBuilder.AddField($"Страница {++pageCount}", listBuilder.ToString());
                }
            }
            else
            {
                embedBuilder.AddField($"Список", "🕸Здесь пока пусто🕸");
            }

            buttonBuilder.WithButton("Добавить", "AddHoliday");
            buttonBuilder.WithButton("Отменить праздник", "RemoveHoliday");
            buttonBuilder.WithButton("Назад", "InfoMenu");
            await component.RespondAsync(null, new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
            break;
        case "AddMoneyMenu":
            //Создаю кнопки
            buttonBuilder.WithButton("Эмиль Максудов", "EmilMaksudovInvestment");
            buttonBuilder.WithButton("Эмиль Мумджи", "EmilMumdzhiInvestment");
            buttonBuilder.WithButton("Никита Гордеев", "NikitaInvestment");
            buttonBuilder.WithButton("Общее вложение", "GeneralInvestment");
            buttonBuilder.WithButton("Назад", "MoneyControl");
            await component.RespondAsync("Выберите от лица кого будет начисление:", components: buttonBuilder.Build());
            break;
        case "EmilMaksudovInvestment":
        case "EmilMumdzhiInvestment":
        case "NikitaInvestment":
        case "GeneralInvestment":
            ModalBuilder modalBuilder = new ModalBuilder()
                .WithCustomId($"{component.Data.CustomId}Modal")
                .WithTitle("Залупа")
                .AddTextInput(new TextInputBuilder()
                    .WithLabel("Сумма дополнительного перевода:")
                    .WithCustomId($"{component.Data.CustomId}Input")
                    .WithStyle(TextInputStyle.Short)
                    .WithMinLength(1)
                    .WithMaxLength(10)
                    .WithRequired(true)
                    .WithPlaceholder("228"));
            await component.RespondWithModalAsync(modalBuilder.Build());
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
            UnionMember currentMember;
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
            break;
            SocketMessageComponentData? componentData = modal.Data.Components.First();
            componentData.Values
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
            HolidayInfo? info = DataBank.UnionInfo.CheckIfDayIsHoliday(DateTime.Today);
            if (info != null)
            {
                builder.AddField("Сегодня:", info.Name);
            }
            //Создаю кнопки
            ComponentBuilder buttonBuilder = new ComponentBuilder();
            buttonBuilder.WithButton("Вклад", "MoneyControl");
            buttonBuilder.WithButton("Кредиты", "CreditsControl");
            buttonBuilder.WithButton("Праздники", "HolidayControl");
            buttonBuilder.WithButton("Настройки", "Settings");
            await command.RespondAsync(null, new Embed[1] { builder.Build() }, components: buttonBuilder.Build());
            break;
    }
}