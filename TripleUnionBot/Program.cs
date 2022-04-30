using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Text;
using TripleUnionBot;
using TripleUnionBot.Classes;

#region Main Block
DataBank.UnionInfo = new UnionInfo();
for (int i = 0; i < 90; i++)
{
    DataBank.UnionInfo.Holidays.Add(new HolidayInfo(i, $"{i}День залупы", DateTime.Now));
}
DiscordSocketClient _client = new DiscordSocketClient(); //<-- Создание объекта клиента
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
            await component.Message.ModifyAsync(x =>
            {
                x.Content = null;
                x.Embeds = new Embed[1] { embedBuilder.Build() };
            });
            await component.UpdateAsync(x =>
            {
                x.Components = buttonBuilder.Build();
            });
            break;
        case "MoneyControl":
            EmbedButtonMenus.ApplyMoneyControl(embedBuilder, buttonBuilder);
            await component.Message.ModifyAsync(x =>
            {
                x.Content = null;
                x.Embeds = new Embed[1] { embedBuilder.Build() };
            });
            await component.UpdateAsync(x =>
            {
                x.Components = buttonBuilder.Build();
            });
            break;
        case "CreditsControl":
            EmbedButtonMenus.ApplyCreditsControl(embedBuilder, buttonBuilder);
            await component.Message.ModifyAsync(x =>
            {
                x.Content = null;
                x.Embeds = new Embed[1] { embedBuilder.Build() };
            });
            await component.UpdateAsync(x =>
            {
                x.Components = buttonBuilder.Build();
            });
            break;
        case "HolidayControl":
            EmbedButtonMenus.ApplyHolidayControl(embedBuilder, buttonBuilder);
            await component.Message.ModifyAsync(x =>
            {
                x.Content = null;
                x.Embeds = new Embed[1] { embedBuilder.Build() };
            });
            await component.UpdateAsync(x =>
            {
                x.Components = buttonBuilder.Build();
            });
            break;
        case "AddHoliday":
            await component.Message.DeleteAsync();
            await component.RespondWithModalAsync(EmbedButtonMenus.ApplyAddHoliday().Build());
            break;
        case "AddMoneyMenu":
            EmbedButtonMenus.ApplyAddMoneyMenu(buttonBuilder);
            await component.Message.ModifyAsync(x =>
            {
                x.Content = "Выберите от лица кого будет начисление:";
                x.Embeds = null;
            });
            await component.UpdateAsync(x =>
            {
                x.Components = buttonBuilder.Build();
            });
            break;
        case "SpendMoneyMenu":
            EmbedButtonMenus.ApplyRemoveMoneyMenu(buttonBuilder);
            await component.Message.ModifyAsync(x =>
            {
                x.Content = "Выберите от лица кого будет счисление:";
                x.Embeds = null;
            });
            await component.UpdateAsync(x =>
            {
                x.Components = buttonBuilder.Build();
            });
            break;
        case "SetPercent":
            await component.Message.DeleteAsync();
            await component.RespondWithModalAsync(EmbedButtonMenus.ApplySetPercent().Build());
            break;
        case "EmilMaksudovInvestment":
        case "EmilMumdzhiInvestment":
        case "NikitaInvestment":
        case "GeneralInvestment":
            await component.Message.DeleteAsync();
            await component.RespondWithModalAsync(EmbedButtonMenus.ApplyInestment(component.Data.CustomId).Build());
            break;
        case "EmilMaksudovSpend":
        case "EmilMumdzhiSpend":
        case "NikitaSpend":
        case "GeneralSpend":
            await component.Message.DeleteAsync();
            await component.RespondWithModalAsync(EmbedButtonMenus.ApplySpend(component.Data.CustomId).Build());
            break;
        default:
            if (component.Data.CustomId.StartsWith("ListHoliday"))
            {
                string pageString = component.Data.CustomId.Split(":")[1];
                if (int.TryParse(pageString, out int pageParsed))
                {
                    EmbedButtonMenus.ApplyHolidayList(pageParsed, embedBuilder, buttonBuilder);
                    await component.Message.ModifyAsync(x =>
                    {
                        x.Content = null;
                        x.Embeds = new Embed[1] { embedBuilder.Build() };
                    });
                    await component.UpdateAsync(x =>
                    {
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
        case "HolidayAddModal":
            string nameParse = string.Empty;
            DateTime dateParse = DateTime.Now;
            foreach(SocketMessageComponentData data in modal.Data.Components)
            {
                if (data.CustomId.Equals("HolidayInput"))
                {
                    nameParse = data.Value;
                }
                else if (data.CustomId.Equals("HolidayDate"))
                {
                    if (!DateTime.TryParse(data.Value,out dateParse))
                    {
                        EmbedButtonMenus.ApplyHolidayControl(embedBuilder, buttonBuilder);
                        await modal.RespondAsync("Неверынй формат даты", new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
                        return;
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
                DataBank.UnionInfo.AddHoliday(nameParse,dateParse);
                EmbedButtonMenus.ApplyHolidayControl(embedBuilder, buttonBuilder);
                await modal.RespondAsync($"Праздник '{nameParse}' назначен на {dateParse.ToString("dd.MM.yyyy")}!", new Embed[1] { embedBuilder.Build() }, components: buttonBuilder.Build());
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
            foreach(SocketMessageComponentData data in modal.Data.Components)
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