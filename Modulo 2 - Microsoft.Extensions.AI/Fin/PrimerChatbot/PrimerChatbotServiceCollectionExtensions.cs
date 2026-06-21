using Anthropic;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Chat;

namespace PrimerChatbot;

internal static class PrimerChatbotServiceCollectionExtensions
{
    public static IServiceCollection AddPrimerChatbot(this IServiceCollection services, string proveedor, string? modelo)
    {
        string llaveOpenAI = Environment.GetEnvironmentVariable("OPENAI_LLAVE")!;
        string llaveAnthropic = Environment.GetEnvironmentVariable("ANTHROPIC_LLAVE")!;

        services.AddSingleton(sp =>
        {
            var cliente = proveedor switch
            {
                "openai" => new ChatClient(modelo ?? "gpt-5.4-nano", llaveOpenAI).AsIChatClient(),
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
                .Use(async (mensajes, opciones, next, cancellationToken) =>
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Antes de llamar al modelo...");
                    Console.ResetColor();

                    await next(mensajes, opciones, cancellationToken);

                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Después de llamar al modelo...");
                    Console.ResetColor();
                })
                .Build(sp);
        });

        return services;
    }
}
