using Anthropic;
using Anthropic.Models.Messages;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
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

        builder.Services.AddSingleton<IChatClient>(sp =>
        {
            var cliente = proveedor switch
            {
                "openai" => new OpenAI.Chat.ChatClient(modelo ?? "gpt-5.4-nano", llaveOpenAI).AsIChatClient(),
                "claude" => new AnthropicClient()
                {
                    ApiKey = llaveAnthropic
                }.AsIChatClient().AsBuilder().ConfigureOptions(c => c.ModelId = modelo ?? "claude-haiku-4-5").Build(),
                _ => throw new ArgumentException($"Proveedor desconocido: {proveedor}")
            };

            return cliente.AsBuilder()
            .ConfigureOptions(o =>
            {
                o.MaxOutputTokens = 2000;
                o.Temperature = 0.7f;
            })
            //.Use(async (mensajes, opciones, next, cancellationToken) =>
            //{
            //    Console.WriteLine();
            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Console.WriteLine("Antes de llamar al modelo...");
            //    Console.ResetColor();

            //    await next(mensajes, opciones, cancellationToken);

            //    Console.WriteLine();
            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Console.WriteLine("Después de llamar al modelo...");
            //    Console.ResetColor();

            //})
            .Build(sp);
            ;
        });
    }
}
