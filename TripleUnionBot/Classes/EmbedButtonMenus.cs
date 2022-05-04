﻿using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripleUnionBot.Classes
{
    internal static class EmbedButtonMenus
    {
        private static readonly Color leftColor;

        static EmbedButtonMenus()
        {
            leftColor = new Color(0x9e0606);
        }

        public static void ApplyCurrentTimeFooterAndColor(EmbedBuilder embedBuilder)
        {
            embedBuilder.WithFooter(new EmbedFooterBuilder().WithText($"Информация на {DateTime.Now}"));
            embedBuilder.WithColor(leftColor);
        }

        public static void ApplyInfoMenu(EmbedBuilder embedBuilder, ComponentBuilder buttonBuilder)
        {
            embedBuilder.Title = "Состояние казны";
            embedBuilder.AddField("Баланс:", $"{DataBank.UnionInfo.Money} ₽", true);
            embedBuilder.AddField("Процент:", $"{DataBank.UnionInfo.Percent}%", true);
            HolidayInfo? foundHoliday = DataBank.UnionInfo.CheckIfDayIsHoliday(DateTime.Today);
            if (foundHoliday != null)
            {
                embedBuilder.AddField("Сегодня:", foundHoliday.Name);
            }
            ApplyCurrentTimeFooterAndColor(embedBuilder);
            buttonBuilder.WithButton("Вклад", "MoneyControl");
            buttonBuilder.WithButton("Кредиты", "CreditsControl");
            buttonBuilder.WithButton("Праздники", "HolidayControl");
            buttonBuilder.WithButton("Настройки", "Settings");
        }

        public static void ApplyMoneyControl(EmbedBuilder embedBuilder, ComponentBuilder buttonBuilder)
        {
            embedBuilder.Title = "Информация о вкладе";
            embedBuilder.AddField("Баланс:", $"{DataBank.UnionInfo.Money} ₽", true);
            embedBuilder.AddField("Процент:", $"{DataBank.UnionInfo.Percent}%", true);
            ApplyCurrentTimeFooterAndColor(embedBuilder);
            buttonBuilder.WithButton("Добавить", "AddMoneyMenu");
            buttonBuilder.WithButton("Потратить", "SpendMoneyMenu");
            buttonBuilder.WithButton("История", "TransactionHistory");
            buttonBuilder.WithButton("Изменить процент", "SetPercent");
            buttonBuilder.WithButton("Назад", "InfoMenu");
        }

        public static void ApplyCreditsControl(EmbedBuilder embedBuilder, ComponentBuilder buttonBuilder)
        {
            embedBuilder.Title = "Информация о кредитах";
            embedBuilder.AddField("Всего кредитов:", DataBank.UnionInfo.Credits.Count, true);
            ApplyCurrentTimeFooterAndColor(embedBuilder);
            buttonBuilder.WithButton("Добавить кредит", "AddMoney");
            buttonBuilder.WithButton("Закрыть кредит", "SpendMoney");
            buttonBuilder.WithButton("Назад", "InfoMenu");
        }

        public static void ApplyHolidayControl(EmbedBuilder embedBuilder, ComponentBuilder buttonBuilder)
        {
            embedBuilder.Title = "Информация о праздниках";
            embedBuilder.AddField("Всего праздников:", DataBank.UnionInfo.Holidays.Count, true);
            HolidayInfo? foundInfo = DataBank.UnionInfo.CheckIfDayIsHoliday(DateTime.Today);
            if (foundInfo != null)
            {
                embedBuilder.AddField("Сегодня:", foundInfo.Name, true);
            }
            ApplyCurrentTimeFooterAndColor(embedBuilder);
            buttonBuilder.WithButton("Список", "ListHoliday:0");
            buttonBuilder.WithButton("Добавить", "AddHoliday");
            buttonBuilder.WithButton("Убрать", "RemoveHoliday");
            buttonBuilder.WithButton("Назад", "InfoMenu");
        }

        public static void ApplyHolidayList(int page, EmbedBuilder embedBuilder, ComponentBuilder buttonBuilder)
        {
            embedBuilder.Title = "Спиок праздников";
            ApplyCurrentTimeFooterAndColor(embedBuilder);
            buttonBuilder.WithButton("Назад", "HolidayControl");
            if (DataBank.UnionInfo.Holidays.Count == 0)
            {
                embedBuilder.AddField("Страница 1","🕸Праздновать пока нечего🕸");
                return;
            }
            int counter = page * 30;
            if (counter >= DataBank.UnionInfo.Holidays.Count)
            {
                counter = 0;
                page = 0;
            }
            int endCount = counter + 30;
            StringBuilder builder = new StringBuilder();
            for (; counter < endCount; counter++)
            {
                if (counter >= DataBank.UnionInfo.Holidays.Count)
                {
                    break;
                }
                builder.AppendLine($"[{DataBank.UnionInfo.Holidays[counter].Date.ToString("dd.MM.yyyy")}] {DataBank.UnionInfo.Holidays[counter].Name}");
            }
            embedBuilder.AddField($"Страница {page + 1}", builder.ToString());
            int totalPageCount;
            if ((double)DataBank.UnionInfo.Holidays.Count / 30 % 1 != 0)
            {
                totalPageCount = DataBank.UnionInfo.Holidays.Count / 30 + 1;
            }
            else
            {
                totalPageCount = DataBank.UnionInfo.Holidays.Count / 30;
            }
            for (int i = 0; i < totalPageCount; i++)
            {
                if (i != page)
                {
                    buttonBuilder.WithButton((i+1).ToString(), $"ListHoliday:{i}");
                }
            }
        }

        public static void ApplyAddMoneyMenu(ComponentBuilder buttonBuilder)
        {
            buttonBuilder.WithSelectMenu(
                new SelectMenuBuilder()
                    .WithCustomId("InvestmentMenu")
                    .WithOptions(
                        new List<SelectMenuOptionBuilder>()
                        {
                            new SelectMenuOptionBuilder().WithValue("EmilMaksudovInvestment").WithLabel("Эмиль Максудов"),
                            new SelectMenuOptionBuilder().WithValue("EmilMumdzhiInvestment").WithLabel("Эмиль Мумджи"),
                            new SelectMenuOptionBuilder().WithValue("NikitaInvestment").WithLabel("Никита Гордеев"),
                            new SelectMenuOptionBuilder().WithValue("GeneralInvestment").WithLabel("Общее вложение"),
                        }).WithPlaceholder("Выберите от лица кого будет начисление"));
            buttonBuilder.WithButton("Назад", "MoneyControl");
        }

        public static void ApplyRemoveMoneyMenu(ComponentBuilder buttonBuilder)
        {
            buttonBuilder.WithSelectMenu(
                new SelectMenuBuilder()
                    .WithCustomId("SpendMenu")
                    .WithOptions(
                        new List<SelectMenuOptionBuilder>()
                        {
                            new SelectMenuOptionBuilder().WithValue("EmilMaksudovSpend").WithLabel("Эмиль Максудов"),
                            new SelectMenuOptionBuilder().WithValue("EmilMumdzhiSpend").WithLabel("Эмиль Мумджи"),
                            new SelectMenuOptionBuilder().WithValue("NikitaSpend").WithLabel("Никита Гордеев"),
                            new SelectMenuOptionBuilder().WithValue("GeneralSpend").WithLabel("Общее вложение"),
                        }).WithPlaceholder("Выберите от лица кого будет счисление"));
            buttonBuilder.WithButton("Назад", "MoneyControl");
        }

        public static ModalBuilder ApplyInestment(string customId)
        {
            return new ModalBuilder()
                .WithCustomId($"{customId}Modal")
                .WithTitle("Добавление средств")
                .AddTextInput(new TextInputBuilder()
                    .WithLabel("Сумма дополнительного перевода:")
                    .WithCustomId($"{customId}Input")
                    .WithStyle(TextInputStyle.Short)
                    .WithMinLength(1)
                    .WithMaxLength(10)
                    .WithRequired(true)
                    .WithPlaceholder("228"))
                .AddTextInput(new TextInputBuilder()
                    .WithLabel("Описание (не обязательно)")
                    .WithCustomId($"{customId}Desc")
                    .WithStyle(TextInputStyle.Paragraph)
                    .WithMinLength(1)
                    .WithMaxLength(80)
                    .WithRequired(false)
                    .WithPlaceholder("Говно, залупа, пенис, хер..."));
        }

        public static ModalBuilder ApplySpend(string customId)
        {
            return new ModalBuilder()
                .WithCustomId($"{customId}Modal")
                .WithTitle("Снятие средств")
                .AddTextInput(new TextInputBuilder()
                    .WithLabel("Сумма снятия:")
                    .WithCustomId($"{customId}Input")
                    .WithStyle(TextInputStyle.Short)
                    .WithMinLength(1)
                    .WithMaxLength(10)
                    .WithRequired(true)
                    .WithPlaceholder("228"))
                .AddTextInput(new TextInputBuilder()
                    .WithLabel("Описание (не обязательно)")
                    .WithCustomId($"{customId}Desc")
                    .WithStyle(TextInputStyle.Paragraph)
                    .WithMinLength(1)
                    .WithMaxLength(80)
                    .WithRequired(false)
                    .WithPlaceholder("Говно, залупа, пенис, хер..."));
        }

        public static ModalBuilder ApplyAddHoliday()
        {
            return new ModalBuilder()
                .WithCustomId($"HolidayAddModal")
                .WithTitle("Добавление праздника")
                .AddTextInput(new TextInputBuilder()
                    .WithLabel("Название праздника:")
                    .WithCustomId($"HolidayInput")
                    .WithStyle(TextInputStyle.Short)
                    .WithMinLength(1)
                    .WithMaxLength(20)
                    .WithRequired(true)
                    .WithPlaceholder("228"))
                .AddTextInput(new TextInputBuilder()
                    .WithLabel("Дата:")
                    .WithCustomId($"HolidayDate")
                    .WithStyle(TextInputStyle.Short)
                    .WithMinLength(1)
                    .WithMaxLength(10)
                    .WithRequired(true)
                    .WithPlaceholder(DateTime.Today.ToString("dd.MM.yyyy")));
        }

        public static ModalBuilder ApplySetPercent()
        {
            return new ModalBuilder()
                .WithCustomId("PercentModal")
                .WithTitle("Изменение процента вклада")
                .AddTextInput(new TextInputBuilder()
                    .WithLabel("Новый процент:")
                    .WithCustomId("PercentInput")
                    .WithStyle(TextInputStyle.Short)
                    .WithMinLength(1)
                    .WithMaxLength(5)
                    .WithRequired(true)
                    .WithValue(DataBank.UnionInfo.Percent.ToString()));
        }

        public static ModalBuilder ApplyRemoveHoliday()
        {
            return new ModalBuilder()
                .WithCustomId("HolidayRemoveModal")
                .WithTitle("Удаление праздника")
                .AddTextInput(new TextInputBuilder()
                    .WithLabel("Введите дату праздника:")
                    .WithCustomId("HolidayRemoveInput")
                    .WithStyle(TextInputStyle.Short)
                    .WithMinLength(1)
                    .WithMaxLength(10)
                    .WithRequired(true));
        }
    }
}
