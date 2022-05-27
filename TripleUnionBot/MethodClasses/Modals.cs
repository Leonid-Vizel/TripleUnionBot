using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripleUnionBot.Classes;

namespace TripleUnionBot.MethodClasses
{
    public static class Modals
    {
        public static async Task PercentModal(SocketModal modal)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            ComponentBuilder buttonBuilder = new ComponentBuilder();
            SocketMessageComponentData? componentData = modal.Data.Components.First();
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
        }

        public static async Task HolidayRemoveModal(SocketModal modal)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            ComponentBuilder buttonBuilder = new ComponentBuilder();
            SocketMessageComponentData? componentData = modal.Data.Components.First();
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
        }

        public static async Task HolidayAddModal(SocketModal modal)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            ComponentBuilder buttonBuilder = new ComponentBuilder();
            SocketMessageComponentData? componentData = modal.Data.Components.First();
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
        }

        public static async Task InvestmentModal(SocketModal modal)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            ComponentBuilder buttonBuilder = new ComponentBuilder();
            SocketMessageComponentData? componentData = modal.Data.Components.First();
            UnionMember currentMember = GetMember(modal.Data.CustomId);
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
        }

        public static async Task SpendModal(SocketModal modal)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            ComponentBuilder buttonBuilder = new ComponentBuilder();
            SocketMessageComponentData? componentData = modal.Data.Components.First();
            UnionMember currentMember = GetMember(modal.Data.CustomId);
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
        }

        private static UnionMember GetMember(string value)
        {
            if (value.StartsWith("EmilMaksudov"))
            {
                return UnionMember.EmilMaksudov;
            }
            else if (value.StartsWith("EmilMumdzhi"))
            {
                return UnionMember.EmilMumdzhi;
            }
            else if (value.StartsWith("Nikita"))
            {
                return UnionMember.NikitaGordeev;
            }
            else
            {
                return UnionMember.General;
            }
        }
    }

    public delegate Task ModalsHandler(SocketModal modal);
}
