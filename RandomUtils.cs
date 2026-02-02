using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
//using Allure.Commons;

// Примечание: Для Allure-шагов требуется пакет Allure.Commons
// Для использования в тестовом проекте подключите:
// - NuGet: Allure.Commons
// - NuGet: xunit (или другой фреймворк, если используется)

public static class RandomUtils
{
    private static readonly Random SecureRandom = new Random();

    #region Числа

    public static int GenerateRandomInt(int from, int to)
    {
        return SecureRandom.Next(from, to);
    }

    public static int GenerateRandomInt(int from, int to, int step)
    {
        int rand = GenerateRandomInt(from, to);
        return rand - (rand % step);
    }

    public static decimal GenerateRandomDecimal(int from, int to)
    {
        return GenerateRandomInt(from, to);
    }

    public static decimal GenerateRandomDecimal(double from, double to)
    {
        return (decimal)SecureRandom.NextDouble() * (decimal)(to - from) + (decimal)from;
    }

    public static decimal GenerateRandomDecimalWithScale(double from, double to, int scale)
    {
        decimal number = GenerateRandomDecimal(from, to);
        return Math.Round(number, scale);
    }

    public static long GenerateRandomLong(long from, long to)
    {
        byte[] buffer = new byte[8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(buffer);
        }

        long value = BitConverter.ToInt64(buffer, 0);
        return Math.Abs(value % (to - from)) + from;
    }

    public static double GenerateRandomDouble(double from, double to)
    {
        return SecureRandom.NextDouble() * (to - from) + from;
    }

    #endregion

    #region Строки

    public static string GenerateStringFromAlphabetWithLength(int length, string alphabet)
    {
        var builder = new System.Text.StringBuilder();
        builder.Append("7");

        for (int i = 0; i < length - 1; i++)
        {
            int index = SecureRandom.Next(alphabet.Length);
            builder.Append(alphabet[index]);
        }

        return builder.ToString();
    }

    public static string GenerateRandomString(int length)
    {
        const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        return GenerateStringFromAlphabetWithLength(length, Alphabet);
    }

    public static string GenerateRandomStringFromNumbers(int length)
    {
        const string Digits = "1234567890";
        return GenerateStringFromAlphabetWithLength(length, Digits);
    }

    #endregion

    #region Коллекции

    public static List<int> GenerateCopies(int startFrom, int step, int limit)
    {
        return Enumerable.Range(0, limit)
                         .Select(i => startFrom + i * step)
                         .ToList();
    }

    public static List<long> GenerateCopies(long startFrom, long step, int limit)
    {
        return Enumerable.Range(0, limit)
                         .Select(i => startFrom + i * step)
                         .ToList();
    }

    public static List<decimal> GenerateCopiesOfBigDecimals(long startFrom, long step, int limit)
    {
        return GenerateCopies((long)startFrom, step, limit)
            .Select(x => (decimal)x)
            .ToList();
    }

    public static List<string> GenerateCopiesOfStrings(long startFrom, long step, int limit)
    {
        return GenerateCopies(startFrom, step, limit)
            .Select(x => x.ToString())
            .ToList();
    }

    #endregion

    #region Специальные значения

    //Step("Генерируем случайное число с округлением до 2-ух знаков после запятой")]
    public static decimal GenerateRandomNumberWithScale2(double from, double to)
    {
        var number = GenerateRandomDecimal(from, to);
        var rounded = Math.Round(number, 2); //, MidpointRounding.ToPositiveInfinity);
        //Step($"Сгенерировано число [{rounded}]");
        return rounded;
    }

    //Step("Создание случайного номера счета, которого еще нет в БД")]
    public static string GenerateRandomAccountNumber()
    {
        while (true)
        {
            string number = GenerateRandomStringFromNumbers(10);
            int flag = 1;

            if (!number.StartsWith("0") && flag == 0)
                return number;
        }
    }

    //Step("Создание случайного номера Siebel ID, которого еще нет в БД")]
    public static string GenerateRandomSiebelId()
    {
        string id = $"99-TEST{GenerateRandomString(6)}";
        int flag = 1;

        return flag == 0 ? id : GenerateRandomSiebelId();
    }

    public static string GenerateRandomClientId()
    {
        string id = $"1-TEST{GenerateRandomString(4)}";
        int flag = 1; //GlobalAccountSteps.CheckClientId(id); // реализация должна быть отдельно

        return flag == 0 ? id : GenerateRandomSiebelId();
    }

    //Step("Генерируем случайное число с округлением до {scale} знаков после запятой с BigDecimal")]
    public static decimal GenerateRandomBigDecimalWithScale(decimal from, decimal to, int scale)
    {
        decimal number = GenerateRandomDecimal((double)from, (double)to);
        return Math.Round(number, scale); //, MidpointRounding.ToPositiveInfinity);
    }

    //Step("Генерируем случайный ОГРН для ЮЛ")]
    public static string GenerateRandomOgrnForLegalClient()
    {
        string[] markers = { "1", "5" };
        int randomIndex = SecureRandom.Next(0, 2);
        int currentYear = DateTime.Now.Year % 2000;
        int year = GenerateRandomInt(2, currentYear);
        string yearStr = year.ToString("D2");

        int part1 = GenerateRandomInt(1, 99);
        string part1Str = part1.ToString("D2");

        string part1Final = markers[randomIndex] + yearStr + part1Str;
        string part2 = GenerateRandomInt(1, 9999999).ToString("D7");

        string ogrn = part1Final + part2;
        int controlNumber = (int)(long.Parse(ogrn) % 11);
        string controlDigit = controlNumber.ToString();

        return ogrn + controlDigit;
    }

    public static T GetRandomElement<T>(IList<T> list)
    {
        int index = SecureRandom.Next(list.Count);
        return list[index];
    }

    //Step("Генерируем валидный номер карты")]
    public static string GenerateValidCardNumber()
    {
        const string Iin = "123456";
        int randomLength = 16 - Iin.Length - 1;
        var cardBuilder = new System.Text.StringBuilder(Iin);

        for (int i = 0; i < randomLength; i++)
        {
            cardBuilder.Append(SecureRandom.Next(0, 10));
        }

        int sum = 0;
        for (int i = 0; i < cardBuilder.Length; i++)
        {
            int digit = int.Parse(cardBuilder[i].ToString());

            if (i % 2 == 0)
            {
                digit *= 2;
                if (digit > 9)
                {
                    digit = (digit / 10) + (digit % 10);
                }
            }

            sum += digit;
        }

        int checkDigit = (10 - (sum % 10)) % 10;
        cardBuilder.Append(checkDigit);

        return cardBuilder.ToString();
    }

    #endregion

    #region Вспомогательные методы

    public static string GenerateUuid()
    {
        return Guid.NewGuid().ToString();
    }

    #endregion
}