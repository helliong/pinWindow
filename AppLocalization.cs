namespace PinWindow;

internal static class AppLocalization
{
    private static string _language = "ru";

    public static string Language => _language;

    public static bool IsEnglish => _language.Equals("en", StringComparison.OrdinalIgnoreCase);

    public static void SetLanguage(string? language)
    {
        _language =
            language?.Equals("en", StringComparison.OrdinalIgnoreCase) == true ? "en" : "ru";
    }

    public static string Text(string russian, string english) => IsEnglish ? english : russian;
}
