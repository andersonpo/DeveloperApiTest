using DeveloperApiTest.Infraestrutura.Extensoes;

namespace DeveloperTestXUnit.Extensao;

public class StringExtensaoTest
{
    [Fact]
    public void Deve_Retornar_Apenas_Numeros()
    {
        // Arrange
        var valor = "ABC1234567890987654321ZZZ";
        var resultadoEsperado = 1234567890987654321;

        // Action
        var resultado = valor.ApenasNumeros();

        // Assert
        resultado.Should().Be(resultadoEsperado);
    }

    [Fact]
    public void Deve_Retornar_Zero_Quando_Nao_Tem_Numeros()
    {
        // Arrange
        var valor = "ABCZZZ";
        var resultadoEsperado = 0;

        // Action
        var resultado = valor.ApenasNumeros();

        // Assert
        resultado.Should().Be(resultadoEsperado);
    }

    [Fact]
    public void Deve_Retornar_Zero_Quando_String_Vazia()
    {
        // Arrange
        var valor = string.Empty;
        var resultadoEsperado = 0;

        // Action
        var resultado = valor.ApenasNumeros();

        // Assert
        resultado.Should().Be(resultadoEsperado);
    }

}
