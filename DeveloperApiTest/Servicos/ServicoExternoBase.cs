using Refit;

namespace DeveloperApiTest.Servicos;

public class ServicoExternoBase
{
    public void ValidarResultadoRequisicao<T>(ApiResponse<T> apiResponse, bool deveTerResponseBody = false)
    {
        var mensagemDetalhes = string.Empty;
        if (apiResponse == null)
        {
            throw new ArgumentNullException(nameof(apiResponse), "Resposta esta nula!");
        }

        if (apiResponse.Error != null)
        {
            if (!string.IsNullOrWhiteSpace(apiResponse.Error.Content))
                mensagemDetalhes = apiResponse.Error.Content;
            else
                mensagemDetalhes = apiResponse.ReasonPhrase;
        }

        if (!apiResponse.IsSuccessStatusCode)
        {
            mensagemDetalhes = $"{mensagemDetalhes} Resposta invalida!";
            throw new HttpRequestException(mensagemDetalhes, null, apiResponse.StatusCode);
        }
        else if (deveTerResponseBody)
        {
            if (apiResponse.Content == null)
            {
                mensagemDetalhes = $"{mensagemDetalhes} Corpo da resposta esta nulo!";
                throw new HttpRequestException(mensagemDetalhes, null, apiResponse.StatusCode);
            }
        }

    }
}
