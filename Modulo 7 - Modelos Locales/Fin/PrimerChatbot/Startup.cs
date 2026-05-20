using Anthropic;
using Anthropic.Models.Messages;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OllamaSharp;
using PrimerChatbot.Servicios;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrimerChatbot;

internal static class Startup
{
    public static void ConfigureServices(HostApplicationBuilder builder, string proveedor, string? modelo)
    {
        string llaveOpenAI = Environment.GetEnvironmentVariable("OPENAI_LLAVE")!;
        string llaveAnthropic = Environment.GetEnvironmentVariable("ANTHROPIC_LLAVE")!;

        builder.Services.AddTransient<IServicioClima, ServicioClimaOpenWeather>();
        builder.Services.AddTransient<ServicioEvaluaCondiciones>();
        builder.Logging.AddFilter("System.Net.Http.HttpClient", LogLevel.None);
        builder.Services.AddHttpClient();

        builder.Services.AddTransient<ServicioEnviarCorreoFalso>();
        builder.Services.AddTransient<ServicioObtenerCorreoFalso>();


        builder.Services.AddSingleton<IChatClient>(sp =>
        {
            var cliente = proveedor switch
            {
                "openai" => new OpenAI.Chat.ChatClient(modelo ?? "gpt-5.4-nano", llaveOpenAI).AsIChatClient(),
                "claude" => new AnthropicClient()
                {
                    ApiKey = llaveAnthropic
                }.AsIChatClient().AsBuilder().ConfigureOptions(c => c.ModelId = modelo ?? "claude-haiku-4-5").Build(),
                "ollama" => new OllamaApiClient("http://127.0.0.1:11434", modelo ?? "qwen3.5:2b"),
                _ => throw new ArgumentException($"Proveedor desconocido: {proveedor}")
            };

            return cliente.AsBuilder()
            .ConfigureOptions(o =>
            {
                o.MaxOutputTokens = 2000;
                o.Temperature = 0.7f;
                o.Tools = [.. Tools.Tools.ObtenerTools(sp)];
            })
            .UseFunctionInvocation(null, c =>
            {
                c.IncludeDetailedErrors = true;
            })
                            .Use(async (messages, options, next, cancellationToken) =>
                            {
                                await next(messages, options, cancellationToken);
                            })

            .Build(sp);
            ;
        });
    }
}
