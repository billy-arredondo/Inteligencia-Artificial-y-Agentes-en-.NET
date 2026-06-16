using Microsoft.Extensions.AI;

namespace WebAPI_IA.Servicios.Chatbots
{
    public class ChatClientFactory(IConfiguration configuration, IServiceProvider sp) : IChatClientFactory
    {
        public IChatClient Crear()
        {
            var llaveOpenAI = configuration.GetValue<string>("OPENAI_LLAVE");
            var modelo = configuration.GetValue<string>("OPENAI_MODELO");

            var cliente = new OpenAI.Chat.ChatClient(modelo ?? "gpt-5.4-nano", llaveOpenAI).AsIChatClient();

            return cliente.AsBuilder()
            .UseFunctionInvocation(null, c =>
            {
                c.IncludeDetailedErrors = true;
            })
            .Build(sp);
        }
    }

}
