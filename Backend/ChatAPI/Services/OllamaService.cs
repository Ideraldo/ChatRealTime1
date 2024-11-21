using OllamaSharp;
using OllamaSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChatAPI.Constants;
using Microsoft.Extensions.Configuration;

public class OllamaService
{
    private readonly OllamaApiClient _client;
    private const string ModelName = "llama2-uncensored";
    private readonly List<OllamaMessage> _context = [];
    private readonly Random _random = new Random();
    private DateTime _lastResponseTime = DateTime.MinValue;

    public OllamaService(IConfiguration configuration)
    {
        var ollamaEndpoint = configuration["OllamaSettings:Endpoint"] ?? "http://localhost:11434";
        _client = new OllamaApiClient(ollamaEndpoint);
    }

    public void AddMessageToContext(string username, string message)
    {
        _context.Add(new OllamaMessage { Username = username, Content = message });
        if (_context.Count > 10) // Manter apenas as últimas 10 mensagens
        {
            _context.RemoveAt(0);
        }
    }

    public bool ShouldRespond()
    {
        // Responder com uma probabilidade de 30% e não mais frequente que a cada 30 segundos
         return (DateTime.Now - _lastResponseTime).TotalSeconds > 10 && _context.Select(m => m.Username != UserConstants.OLLAMA_USER).Any();
    }

    public async IAsyncEnumerable<string> GenerateResponseAsync()
    {
        _lastResponseTime = DateTime.Now;
        var initialContextPrompt =
            "Voce esta em um grupo com os seus amigos. Converse normalmente como se fosse uma conversa descontraida, independente do assunto, voce deve expressar as suas opinioes e debater como se fosse uma pessoa normal em um grupo de Whatsapp. Vou te passar o historico recente do chat para que voce possa responder adequadamente com o contexto";
        var contextPrompt = string.Join("\n", _context.Select(m => $"{m.Username} diz: {m.Content}"));
        var prompt = $"{initialContextPrompt} {contextPrompt}";

        var request = new GenerateRequest
        {
            Model = ModelName,
            Prompt = prompt,
            Stream = true
        };

        var currentMessage = new StringBuilder();

        await foreach (var response in _client.GenerateAsync(request))
        {
            if (response != null && !string.IsNullOrEmpty(response.Response))
            {
                currentMessage.Append(response.Response);

                // Verificar se temos uma mensagem completa
                // As vezes as mensagens cortam no meio. Try to find a solution
                if (IsCompleteMessage(currentMessage.ToString()))
                {
                    yield return currentMessage.ToString().Trim();
                    currentMessage.Clear();
                }

                if (response.Done)
                {
                    if (currentMessage.Length > 0)
                    {
                        yield return currentMessage.ToString().Trim();
                    }

                    break;
                }
            }
        }
    }

    private bool IsCompleteMessage(string message)
    {
        // Lógica para determinar se uma mensagem está completa
        // Pode ser baseada em pontuação, comprimento, etc.
        return message.EndsWith(".") || message.EndsWith("!") || message.EndsWith("?") ||
               message.Length > 100; // Exemplo: quebrar após 100 caracteres
    }
}

public class OllamaMessage
{
    public string Username { get; set; }
    public string Content { get; set; }
}