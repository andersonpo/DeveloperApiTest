﻿using System.Text.Json.Serialization;

namespace DeveloperApiTest.Dominio.Externo;

public class ViaCepEndereco
{
    [JsonPropertyName("cep")]
    public string Cep { get; set; } = string.Empty;

    [JsonPropertyName("logradouro")]
    public string Logradouro { get; set; } = string.Empty;

    [JsonPropertyName("complemento")]
    public string Complemento { get; set; } = string.Empty;

    [JsonPropertyName("bairro")]
    public string Bairro { get; set; } = string.Empty;

    [JsonPropertyName("localidade")]
    public string Localidade { get; set; } = string.Empty;

    [JsonPropertyName("uf")]
    public string Uf { get; set; } = string.Empty;

    [JsonPropertyName("ibge")]
    public string Ibge { get; set; } = string.Empty;

    [JsonPropertyName("gia")]
    public string Gia { get; set; } = string.Empty;

    [JsonPropertyName("ddd")]
    public string DDD { get; set; } = string.Empty;

    [JsonPropertyName("siafi")]
    public string Siafi { get; set; } = string.Empty;
}
