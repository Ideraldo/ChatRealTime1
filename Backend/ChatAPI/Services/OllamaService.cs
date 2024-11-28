using OllamaSharp;
using OllamaSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatAPI.Constants;
using Microsoft.Extensions.Configuration;

public class OllamaService
{
    private readonly OllamaApiClient _client;
    private const string ModelName = "llama3.2";
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
        if (_context.Count > 10)
        {
            _context.RemoveAt(0);
        }
    }

    public async Task<bool> ShouldRespond()
    {var lastMessages = _context.ToList();
        if (!lastMessages.Any())
        {
            return false;
        }
        
        var decisionPrompt = @"
            Analise o contexto da conversa abaixo e decida se você deve responder ou não.
            Responda apenas com 'true' se deve responder ou 'false' se não deve responder.
            Considere:

            - O seu nome é Maira
            - Voce deve responder as mensagens em que voce for citada
            - Voce deve responder caso o contexto das mensagens seja relacionado a algo que voce disse
            - Voce deve responder caso tenha algo a acrescentar no que foi dito anteriormente
            - Voce deve responder as mensagens direcionadas a você
            - Voce deve responder caso seja uma pergunta geral que você pode contribuir
            - Voce deve responder se o tópico é relevante para a conversa
            - Voce NAO deve responder se a conversa é entre outras pessoas específicas
            - Voce NAO deve responder suas proprias mensagens

            Contexto da conversa:
            ";
        var conversationContext = string.Join("\n", lastMessages.Select(m => $"{m.Username}: {m.Content}"));
        var fullPrompt = decisionPrompt + conversationContext;
        var request = new GenerateRequest
        {
            Model = ModelName,
            Prompt = fullPrompt,
            Stream = false
        };
        
        string fullResponse = "";
        await foreach (var response in _client.GenerateAsync(request))
        {
            if (response == null) continue;
            fullResponse += response.Response;
            if (response.Done)
            {
                break;
            }
        }

        return fullResponse.Trim().ToLower() == "true" || fullResponse.Trim().ToLower() == "true.";
    }

    public async IAsyncEnumerable<string> GenerateResponseAsync()
    {
        var lastMessages = _context.TakeLast(10).ToList();
        var contextPrompt = @"
            Com base no contexto da conversa abaixo, gere uma resposta natural e apropriada.
            Lembre-se:
            - O seu nome é Maira
            - Cuidado para nao confundir as mensagens que tem relação a voce, com as que voce enviou
            - Mantenha um tom casual e amigável
            - Responda de forma relevante ao contexto
            - Use linguagem natural
            - Não force respostas quando não necessário
            - Faça a distinção de quem enviou cada mensagem
            - Nao responda suas proprias mensagens

            Contexto da conversa:
            ";
        var conversationContext = string.Join("\n", lastMessages.Select(m => $"{m.Username}: {m.Content}"));
        var fullPrompt = contextPrompt + conversationContext;
        var request = new GenerateRequest
        {
            Model = ModelName,
            Prompt = fullPrompt,
            Stream = true
        };

        var currentMessage = new StringBuilder();

        await foreach (var response in _client.GenerateAsync(request))
        {
            if (response != null && !string.IsNullOrEmpty(response.Response))
            {
                currentMessage.Append(response.Response);

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
        return message.EndsWith(".") || message.EndsWith("!") || message.EndsWith("?");
    }
}

public class OllamaMessage
{
    public string Username { get; set; }
    public string Content { get; set; }
}