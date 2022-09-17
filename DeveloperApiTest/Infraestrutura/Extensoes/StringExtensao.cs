namespace DeveloperApiTest.Infraestrutura.Extensoes;

public static class StringExtensao
{
    public static long ApenasNumeros(this string valor)
    {
        long resultado = 0;
        if (string.IsNullOrWhiteSpace(valor))
            return resultado;

        var numberString = string.Empty;
        foreach (char caracter in valor.ToCharArray())
        {
            if (char.IsDigit(caracter))
            {
                numberString += caracter;
            }
        }

        try
        {
            resultado = long.Parse(numberString);
        }
        catch
        {
            resultado = 0;
        }

        return resultado;
    }
}
