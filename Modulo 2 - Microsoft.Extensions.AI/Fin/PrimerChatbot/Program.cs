
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PrimerChatbot;
using PrimerChatbot.Chatbots;

Utilidades.CargarVariablesDeAmbiente();

// Ejemplos: dotnet run -- openai gpt-5.4-nano
// Ejemplos: dotnet run -- claude claude-haiku-4-5

var proveedor = args.Length > 0 ? args[0].ToLowerInvariant() : "openai";
var modeloPorDefecto = proveedor == "openai" ? "gpt-5.4-nano" : "claude-haiku-4-5";
var modelo = args.Length > 1 ? args[1] : modeloPorDefecto;

Console.WriteLine($"{proveedor}: {modelo}");

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddPrimerChatbot(proveedor, modelo);
var host = builder.Build();

var chatClient = host.Services.GetRequiredService<IChatClient>();
await Chatbot.Correr(chatClient);
