namespace ProcessoSelecao.Domain.Helpers;

/// <summary>
/// Validador de CPF com algoritmo de dígitos verificadores oficiais.
/// </summary>
public static class CpfValidator
{
    /// <summary>
    /// Remove caracteres não numéricos do CPF (pontos, traços, espaços).
    /// </summary>
    public static string Clean(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return string.Empty;

        return new string(cpf.Where(char.IsDigit).ToArray());
    }

    /// <summary>
    /// Formata o CPF como 000.000.000-00.
    /// </summary>
    public static string Format(string? cpf)
    {
        var cleaned = Clean(cpf);

        if (cleaned.Length != 11)
            return cpf ?? string.Empty;

        return $"{cleaned[..3]}.{cleaned[3..6]}.{cleaned[6..9]}-{cleaned[9..]}";
    }

    /// <summary>
    /// Valida se o CPF é válido (11 dígitos com dígitos verificadores corretos).
    /// </summary>
    public static bool IsValid(string? cpf)
    {
        var cleaned = Clean(cpf);

        if (cleaned.Length != 11)
            return false;

        // Rejeita CPFs com todos dígitos iguais (ex: 111.111.111-11)
        if (cleaned.Distinct().Count() == 1)
            return false;

        // Calcula primeiro dígito verificador
        var sum = 0;
        for (int i = 0; i < 9; i++)
            sum += (cleaned[i] - '0') * (10 - i);

        var remainder = sum % 11;
        var firstDigit = remainder < 2 ? 0 : 11 - remainder;

        if (cleaned[9] - '0' != firstDigit)
            return false;

        // Calcula segundo dígito verificador
        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += (cleaned[i] - '0') * (11 - i);

        remainder = sum % 11;
        var secondDigit = remainder < 2 ? 0 : 11 - remainder;

        return cleaned[10] - '0' == secondDigit;
    }
}
