namespace Byndyusoft.DotNet.Testing.Infrastructure.Extensions;

using System;
using System.Text;

/// <summary>
///     Расширение к типу <see cref="string"/>
/// </summary>
internal static class StringExtensions
{
    /// <summary>
    ///     Проверяет равенство строк без учёта регистра, лидирующих и оконечных пробелов
    /// </summary>
    internal static bool EqualsTrimmedIgnoreCase(this string? str1, string? str2)
    {
        return string.Equals(str1?.Trim(), str2?.Trim(), StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    ///     Конвертирует строку в base64
    /// </summary>
    internal static string ToBase64(this string str)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(str);
        return Convert.ToBase64String(plainTextBytes);
    }
}