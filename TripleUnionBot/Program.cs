using Discord;
using Discord.WebSocket;
using System.Timers;
using TripleUnionBot;
using TripleUnionBot.Classes;

#region Main Block
DataBank.UnionInfo = new UnionInfo();
DiscordSocketClient _client = new DiscordSocketClient(); //<-- Создание объекта клиента
_client.Ready += ConfigureCommands; //<- Настройка слэш-комманд бота
_client.SlashCommandExecuted += SlashCommandHandler;//<- Настройка обработки слэш-комманд бота
_client.ModalSubmitted += HandleModalSubmit; //<-- Настройка обработки вопросов
_client.ButtonExecuted += HandleButtonClick; //<-- Настройка обработки кнопок
_client.SelectMenuExecuted += HandleSelectMenu; //<-- Найтройка обработки выпадающих меню
var token = File.ReadAllText("bot.token"); //<-- Считывание токена бота
await _client.LoginAsync(TokenType.Bot, token); //<-- Бот логинится
await _client.StartAsync(); //<-- Бот запускается
await Task.Delay(-1); //<-- Чтобы прога не закрывалась раньше времени
#region Настройка таймера поздравлений
DateTime TimeToExecuteTask;
if (DateTime.Now.Hour < 12)
{
    TimeToExecuteTask = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0);
}
else
{
    TimeToExecuteTask = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1, 12, 0, 0);
}
System.Timers.Timer holidayCheckTimer = new System.Timers.Timer((TimeToExecuteTask - DateTime.Now).TotalMilliseconds);
holidayCheckTimer.Elapsed += CheckTodayHoliday;
#endregion
#endregion

async Task ConfigureCommands()
{
    SlashCommandBuilder[] commandBuilders = new SlashCommandBuilder[2]
    {
        new SlashCommandBuilder().WithName("info").WithDescription("Показывает информацию о состоянии союза"),
        new SlashCommandBuilder().WithName("ебани").WithDescription("Ебанёт число в указанном Вами отрезке")
            .AddOption("начало",ApplicationCommandOptionType.Integer, "Начало отрезка", true, minValue: int.MinValue, maxValue: int.MaxValue)
            .AddOption("конец", ApplicationCommandOptionType.Integer, "Конец отрезка", true, minValue: int.MinValue, maxValue: int.MaxValue)
    };
    try
    {
        SocketGuild? guild = _client.GetGuild(DataBank.GuildId);
        if (guild != null)
        {
            foreach (SlashCommandBuilder commandBuilder in commandBuilders)
            {
                await guild.CreateApplicationCommandAsync(commandBuilder.Build());
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

async Task HandleButtonClick(SocketMessageComponent component)
{
    EmbedBuilder embedBuilder = new EmbedBuilder();
    ComponentBuilder buttonBuilder = new ComponentBuilder();
    switch (component.Data.CustomId)
    {
        case "InfoMenu":
            EmbedButtonMenus.ApplyInfoMenu(embedBuilder, buttonBuilder);
            await component.UpdateAsync(x =>
            {
                x.Content = null;
                x.Embeds = new Embed[1] { embedBuilder.Build() };
                x.Components = buttonBuilder.Build();
            });
            break;
        case "MoneyControl":
            EmbedButtonMenus.ApplyMoneyControl(embedBuilder, buttonBuilder);
            await component.UpdateAsync(x =>
            {
                x.Content = null;
                x.Embeds = new Embed[1] { embedBuilder.Build() };
                x.Components = buttonBuilder.Build();
            });
            break;
        case "CreditsControl":
            EmbedButtonMenus.ApplyCreditsControl(embedBuilder, buttonBuilder);
            await component.UpdateAsync(x =>
            {
                x.Content = null;
                x.Embeds = new Embed[1] { embedBuilder.Build() };
                x.Components = buttonBuilder.Build();
            });
            break;
        case "HolidayControl":
            EmbedButtonMenus.ApplyHolidayControl(embedBuilder, buttonBuilder);
            await component.UpdateAsync(x =>
            {
                x.Content = null;
                x.Embeds = new Embed[1] { embedBuilder.Build() };
                x.Components = buttonBuilder.Build();
            });
            break;
        case "AddHoliday":
            await component.RespondWithModalAsync(EmbedButtonMenus.ApplyAddHoliday().Build());
            await component.Message.DeleteAsync();
            break;
        case "RemoveHoliday":
            await component.RespondWithModalAsync(EmbedButtonMenus.ApplyRemoveHoliday().Build());
            await component.Message.DeleteAsync();
            break;
        case "AddMoneyMenu":
            EmbedButtonMenus.ApplyAddMoneyMenu(buttonBuilder);
            await component.UpdateAsync(x =>
            {
                x.Content = null;
                x.Embeds = null;
                x.Components = buttonBuilder.Build();
            });
            break;
        case "SpendMoneyMenu":
            EmbedButtonMenus.ApplyRemoveMoneyMenu(buttonBuilder);
            await component.UpdateAsync(x =>
            {
                x.Content = null;
                x.Embeds = null;
                x.Components = buttonBuilder.Build();
            });
            break;
        case "SetPercent":
            await component.RespondWithModalAsync(EmbedButtonMenus.ApplySetPercent().Build());
            await component.Message.DeleteAsync();
            break;
        case "Settings":
            EmbedButtonMenus.ApplySettings(_client, embedBuilder, buttonBuilder);
            await component.UpdateAsync(x =>
            {
                x.Content = null;
                x.Embeds = new Embed[1] { embedBuilder.Build() };
                x.Components = buttonBuilder.Build();
            });
            break;
        case "SetCurrentChannel":
            DataBank.UnionInfo.SetChannelId(component.Message.Channel.Id);
            Console.WriteLine(component.Message.Channel.GetType().FullName);
            EmbedButtonMenus.ApplySettings(_client, embedBuilder, buttonBuilder);
            await component.UpdateAsync(x =>
            {
                x.Content = null;
                x.Embeds = new Embed[1] { embedBuilder.Build() };
                x.Components = buttonBuilder.Build();
            });
            break;
        case "SetDefault":
            DataBank.UnionInfo.SetChannelId(null);
            EmbedButtonMenus.ApplySettings(_client, embedBuilder, buttonBuilder);
            await component.UpdateAsync(x =>
            {
                x.Content = null;
                x.Embeds = new Embed[1] { embedBuilder.Build() };
                x.Components = buttonBuilder.Build();
            });
            break;
        default:
            if (component.Data.CustomId.StartsWith("ListHoliday"))
            {
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
            return;
    }
}

async Task HandleModalSubmit(SocketModal modal)
{
    UnionMember currentMember = 0;
    EmbedBuilder embedBuilder = new EmbedBuilder();
    ComponentBuilder buttonBuilder = new ComponentBuilder();
    SocketMessageComponentData? componentData = modal.Data.Components.First();
    switch (modal.Data.CustomId)
    {
        case "PercentModal":
            if (decimal.TryParse(componentData.Value.Replace(",", "."), out decimal parsePercentResult))
            {
                if (DataBank.UnionInfo.SetPercent(parsePercentResult))
                {
                    EmbedButtonMenus.ApplyMoneyControl(embedBuilder, buttonBuilder);
                    await modal.RespondAsync("Процент успешно изменён", new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
                }
                else
                {
                    EmbedButtonMenus.ApplyMoneyControl(embedBuilder, buttonBuilder);
                    await modal.RespondAsync("Значение процента не может быть ниже нуля!", new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
                }
            }
            else
            {
                EmbedButtonMenus.ApplyMoneyControl(embedBuilder, buttonBuilder);
                await modal.RespondAsync("Не удалось преоразовать в число, извините.", new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
            }
            break;
        case "HolidayRemoveModal":
            if (DateTime.TryParse(componentData.Value, out DateTime dateTimeParse))
            {
                HolidayInfo? foundInfo = DataBank.UnionInfo.CheckIfDayIsHoliday(dateTimeParse);
                if (foundInfo != null)
                {
                    DataBank.UnionInfo.Holidays.Remove(foundInfo);
                    EmbedButtonMenus.ApplyHolidayControl(embedBuilder, buttonBuilder);
                    await modal.RespondAsync($"Праздник '{foundInfo.Name}' был успешно удалён!", new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
                }
                else
                {
                    EmbedButtonMenus.ApplyHolidayControl(embedBuilder, buttonBuilder);
                    await modal.RespondAsync("Не найдено праздников на эту дату", new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
                }
            }
            else
            {
                EmbedButtonMenus.ApplyHolidayControl(embedBuilder, buttonBuilder);
                await modal.RespondAsync("Неверынй формат даты", new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
            }
            break;
        case "HolidayAddModal":
            string nameParse = string.Empty;
            DateTime dateParse = DateTime.Now;
            foreach (SocketMessageComponentData data in modal.Data.Components)
            {
                if (data.CustomId.Equals("HolidayInput"))
                {
                    nameParse = data.Value;
                }
                else if (data.CustomId.Equals("HolidayDate"))
                {
                    if (!DateTime.TryParse(data.Value, out dateParse))
                    {
                        EmbedButtonMenus.ApplyHolidayControl(embedBuilder, buttonBuilder);
                        await modal.RespondAsync("Неверынй формат даты", new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
                    }
                    else
                    {
                        HolidayInfo? foundInfo = DataBank.UnionInfo.CheckIfDayIsHoliday(dateParse);
                        if (foundInfo != null)
                        {
                            EmbedButtonMenus.ApplyHolidayControl(embedBuilder, buttonBuilder);
                            await modal.RespondAsync($"На дату {dateParse.ToString("dd.MM.yyyy")} уже назначен праздник {foundInfo.Name}!", new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
                        }
                    }
                }
            }
            if (nameParse.Length == 0)
            {
                EmbedButtonMenus.ApplyHolidayControl(embedBuilder, buttonBuilder);
                await modal.RespondAsync("Ошибка наименования праздника", new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
            }
            else
            {
                if (DataBank.UnionInfo.Holidays.Any(x => x.Name.ToLower().Equals(nameParse)))
                {
                    EmbedButtonMenus.ApplyHolidayControl(embedBuilder, buttonBuilder);
                    await modal.RespondAsync("Праздник с похожим названием уже сущетсвует", new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
                }
                else
                {
                    DataBank.UnionInfo.AddHoliday(nameParse, dateParse);
                    EmbedButtonMenus.ApplyHolidayControl(embedBuilder, buttonBuilder);
                    await modal.RespondAsync($"Праздник '{nameParse}' назначен на {dateParse.ToString("dd.MM.yyyy")}!", new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
                }
            }
            break;
        case "EmilMaksudovInvestmentModal":
        case "EmilMumdzhiInvestmentModal":
        case "NikitaInvestmentModal":
        case "GeneralInvestmentModal":
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
            decimal inputAddMoney = 0;
            string? descriptionAdd = null;
            foreach (SocketMessageComponentData data in modal.Data.Components)
            {
                if (data.CustomId.EndsWith("Input"))
                {
                    if (decimal.TryParse(componentData.Value.Replace(",", "."), out inputAddMoney))
                    {
                        if (inputAddMoney <= 0)
                        {
                            EmbedButtonMenus.ApplyMoneyControl(embedBuilder, buttonBuilder);
                            await modal.RespondAsync("Зачиление не может быть 0 или отрицательным числом. Для этого используйте списания", new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
                            return;
                        }
                    }
                    else
                    {
                        EmbedButtonMenus.ApplyMoneyControl(embedBuilder, buttonBuilder);
                        await modal.RespondAsync("Не удалось преоразовать в число, извините.", new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
                        return;
                    }
                }
                else
                {
                    descriptionAdd = data.Value;
                }
            }
            DataBank.UnionInfo.ExecuteAddition(currentMember, inputAddMoney, descriptionAdd);
            EmbedButtonMenus.ApplyMoneyControl(embedBuilder, buttonBuilder);
            await modal.RespondAsync("Успешно зачислено", new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
            break;
        case "EmilMaksudovSpendModal":
        case "EmilMumdzhiSpendModal":
        case "NikitaSpendModal":
        case "GeneralSpendModal":
            switch (modal.Data.CustomId)
            {
                case "EmilMaksudovSpend":
                    currentMember = UnionMember.EmilMaksudov;
                    break;
                case "EmilMumdzhiSpend":
                    currentMember = UnionMember.EmilMumdzhi;
                    break;
                case "NikitaSpend":
                    currentMember = UnionMember.NikitaGordeev;
                    break;
                case "GeneralSpend":
                    currentMember = UnionMember.General;
                    break;
            }
            decimal inputSpendMoney = 0;
            string? descriptionSpend = null;
            foreach (SocketMessageComponentData data in modal.Data.Components)
            {
                if (data.CustomId.EndsWith("Input"))
                {
                    if (decimal.TryParse(componentData.Value.Replace(",", "."), out inputSpendMoney))
                    {
                        if (inputSpendMoney > 0)
                        {
                            if (DataBank.UnionInfo.Money < inputSpendMoney)
                            {
                                EmbedButtonMenus.ApplyMoneyControl(embedBuilder, buttonBuilder);
                                await modal.RespondAsync("На счету недостаточно средств.", new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
                                return;
                            }
                        }
                        else
                        {
                            EmbedButtonMenus.ApplyMoneyControl(embedBuilder, buttonBuilder);
                            await modal.RespondAsync("Зачиление не может быть 0 или отрицательным числом. Для этого используйте списания", new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
                            return;
                        }
                    }
                    else
                    {
                        EmbedButtonMenus.ApplyMoneyControl(embedBuilder, buttonBuilder);
                        await modal.RespondAsync("Не удалось преоразовать в число, извините.", new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
                        return;
                    }
                }
                else
                {
                    descriptionSpend = data.Value;
                }
            }
            if (DataBank.UnionInfo.ExecuteWaste(currentMember, inputSpendMoney, descriptionSpend))
            {
                EmbedButtonMenus.ApplyMoneyControl(embedBuilder, buttonBuilder);
                await modal.RespondAsync("Успешно снято", new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
            }
            else
            {
                EmbedButtonMenus.ApplyMoneyControl(embedBuilder, buttonBuilder);
                await modal.RespondAsync("Ошибка снятия", new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
            }
            break;
    }
}

async Task HandleSelectMenu(SocketMessageComponent component)
{
    if (component.Data.Values.Count == 0)
    {
        return; //<-- Для избежания ошибки
    }
    switch (component.Data.CustomId)
    {
        case "InvestmentMenu":
            await component.RespondWithModalAsync(EmbedButtonMenus.ApplyInestment(component.Data.Values.First()).Build());
            await component.Message.DeleteAsync();
            break;
        case "SpendMenu":
            await component.RespondWithModalAsync(EmbedButtonMenus.ApplySpend(component.Data.Values.First()).Build());
            await component.Message.DeleteAsync();
            break;
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
        case "ебани":
            try
            {
                int begin = 0;
                int end = 0;
                foreach (SocketSlashCommandDataOption option in command.Data.Options)
                {
                    if (option.Name.Equals("начало"))
                    {
                        if (option.Value is long)
                        {
                            begin = Convert.ToInt32(option.Value);
                        }
                        else
                        {
                            await command.RespondAsync("Кажется ебануть не получится, произошла ошибка!");
                        }
                    }
                    else if (option.Name.Equals("конец"))
                    {
                        if (option.Value is long)
                        {
                            end = Convert.ToInt32(option.Value);
                        }
                        else
                        {
                            await command.RespondAsync("Кажется ебануть не получится, произошла ошибка!");
                        }
                    }
                }
                if (begin > end)
                {
                    await command.RespondAsync("Кажется ебануть не получится, произошла ошибка!");
                }
                else
                {
                    Random random = new Random(Guid.NewGuid().GetHashCode());
                    await command.RespondAsync($"Хоба: {random.Next(begin, end + 1)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            break;
    }
}