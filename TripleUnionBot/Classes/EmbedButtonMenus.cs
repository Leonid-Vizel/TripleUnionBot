using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripleUnionBot.Classes
{
    internal static class EmbedButtonMenus
    {
        public static void ApplyCurrentTimeFooter(EmbedBuilder embedBuilder)
        {
            embedBuilder.WithFooter(new EmbedFooterBuilder().WithText($"Информация на {DateTime.Now}"));
        }

        public static void ApplyInfoMenu(EmbedBuilder embedBuilder, ComponentBuilder buttonBuilder)
        {
            embedBuilder.Title = "Состояние казны";
            embedBuilder.AddField("Баланс:", $"{DataBank.UnionInfo.Money} ₽", true);
            embedBuilder.AddField("Процент:", "10%", true);
            HolidayInfo? foundHoliday = DataBank.UnionInfo.CheckIfDayIsHoliday(DateTime.Today);
            if (foundHoliday != null)
            {
                embedBuilder.AddField("Сегодня:", foundHoliday.Name);
            }
            ApplyCurrentTimeFooter(embedBuilder);
            buttonBuilder.WithButton("Вклад", "MoneyControl");
            buttonBuilder.WithButton("Кредиты", "CreditsControl");
            buttonBuilder.WithButton("Праздники", "HolidayControl");
            buttonBuilder.WithButton("Настройки", "Settings");
        }

        public static void ApplyMoneyControl(EmbedBuilder embedBuilder, ComponentBuilder buttonBuilder)
        {
            embedBuilder.Title = "Информация о вкладе";
            embedBuilder.AddField("Баланс:", $"{DataBank.UnionInfo.Money} ₽", true);
            embedBuilder.AddField("Процент:", "10%", true);
            ApplyCurrentTimeFooter(embedBuilder);
            buttonBuilder.WithButton("Добавить", "AddMoneyMenu");
            buttonBuilder.WithButton("Потратить", "SpendMoneyMenu");
            buttonBuilder.WithButton("Изменить процент", "SetPercent");
            buttonBuilder.WithButton("Назад", "InfoMenu");
        }

        public static void ApplyCreditsControl(EmbedBuilder embedBuilder, ComponentBuilder buttonBuilder)
        {
            embedBuilder.Title = "Информация о кредитах";
            embedBuilder.AddField("Всего кредитов:", DataBank.UnionInfo.Credits.Count, true);
            embedBuilder.AddField("Процент:", "10%", true);
            ApplyCurrentTimeFooter(embedBuilder);
            buttonBuilder.WithButton("Добавить кредит", "AddMoney");
            buttonBuilder.WithButton("Закрыть кредит", "SpendMoney");
            buttonBuilder.WithButton("Назад", "InfoMenu");
        }

        public static void ApplyHolidayControl(EmbedBuilder embedBuilder, ComponentBuilder buttonBuilder)
        {
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
            ApplyCurrentTimeFooter(embedBuilder);
            buttonBuilder.WithButton("Добавить", "AddHoliday");
            buttonBuilder.WithButton("Отменить праздник", "RemoveHoliday");
            buttonBuilder.WithButton("Назад", "InfoMenu");
        }

        public static void ApplyAddMoneyMenu(ComponentBuilder buttonBuilder)
        {
            buttonBuilder.WithButton("Эмиль Максудов", "EmilMaksudovInvestment");
            buttonBuilder.WithButton("Эмиль Мумджи", "EmilMumdzhiInvestment");
            buttonBuilder.WithButton("Никита Гордеев", "NikitaInvestment");
            buttonBuilder.WithButton("Общее вложение", "GeneralInvestment");
            buttonBuilder.WithButton("Назад", "MoneyControl");
        }

        public static void ApplyRemoveMoneyMenu(ComponentBuilder buttonBuilder)
        {
            buttonBuilder.WithButton("Эмиль Максудов", "EmilMaksudovSpend");
            buttonBuilder.WithButton("Эмиль Мумджи", "EmilMumdzhiSpend");
            buttonBuilder.WithButton("Никита Гордеев", "NikitaSpend");
            buttonBuilder.WithButton("Общее вложение", "GeneralSpend");
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
                    .WithPlaceholder("228"));
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
                    .WithPlaceholder("228"));
        }
    }
}
