using Anthropic;
using BlazorIA.Utilidades;
using Microsoft.Extensions.AI;
using OllamaSharp;

namespace BlazorIA.Servicios
{
    public class ChatClientFactory(IConfiguration configuration, IServiceProvider sp) : IChatClientFactory
    {
        public IChatClient Crear(string modelo)
        {
            var llaveOpenAI = configuration.GetValue<string>("OPENAI_LLAVE");
            var llaveAnthropic = configuration.GetValue<string>("ANTHROPIC_LLAVE");
            var urlOllama = configuration.GetValue<string>("OLLAMA_ENDPOINT")!;

            var proveedor = ModelosIA.ObtenerProveedor(modelo);

            var cliente = proveedor switch
            {
                "openai" => new OpenAI.Chat.ChatClient(modelo ?? "gpt-5.4-nano", llaveOpenAI).AsIChatClient(),
                "claude" => new AnthropicClient()
                {
                    ApiKey = llaveAnthropic
                }.AsIChatClient().AsBuilder().ConfigureOptions(c => c.ModelId = modelo ?? "claude-haiku-4-5").Build(),
                "ollama" => new OllamaApiClient(urlOllama, modelo ?? "qwen3.5:2b"),
                _ => throw new ArgumentException($"Proveedor desconocido: {proveedor}")
            };

            return cliente.AsBuilder()
            .UseFunctionInvocation(null, c =>
            {
                c.IncludeDetailedErrors = true;
            })
            .Build(sp);
        }
    }
}
