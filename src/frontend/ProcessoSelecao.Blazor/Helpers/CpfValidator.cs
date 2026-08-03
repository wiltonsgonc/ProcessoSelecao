namespace ProcessoSelecao.Blazor.Helpers;

public static class CpfValidator
{
    public static string Clean(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return string.Empty;
        return new string(cpf.Where(char.IsDigit).ToArray());
    }

    public static string Format(string? cpf)
    {
        var cleaned = Clean(cpf);
        if (cleaned.Length != 11)
            return cpf ?? string.Empty;
        return $"{cleaned[..3]}.{cleaned[3..6]}.{cleaned[6..9]}-{cleaned[9..]}";
    }

    public static string FormatProgressive(string? cpf)
    {
        var digits = Clean(cpf);
        if (digits.Length > 11) digits = digits[..11];

        if (digits.Length >= 10)
            return $"{digits[..3]}.{digits[3..6]}.{digits[6..9]}-{digits[9..]}";
        if (digits.Length >= 7)
            return $"{digits[..3]}.{digits[3..6]}.{digits[6..]}";
        if (digits.Length >= 4)
            return $"{digits[..3]}.{digits[3..]}";
        return digits;
    }

    public static bool IsValid(string? cpf)
    {
        var cleaned = Clean(cpf);
        if (cleaned.Length != 11)
            return false;
        if (cleaned.Distinct().Count() == 1)
            return false;

        var sum = 0;
        for (int i = 0; i < 9; i++)
            sum += (cleaned[i] - '0') * (10 - i);
        var remainder = sum % 11;
        var firstDigit = remainder < 2 ? 0 : 11 - remainder;
        if (cleaned[9] - '0' != firstDigit)
            return false;

        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += (cleaned[i] - '0') * (11 - i);
        remainder = sum % 11;
        var secondDigit = remainder < 2 ? 0 : 11 - remainder;
        return cleaned[10] - '0' == secondDigit;
    }
}
