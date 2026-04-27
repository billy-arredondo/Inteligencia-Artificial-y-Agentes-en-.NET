
using OpenAI.Chat;
using PrimerChatbot;
using System.Text;

Utilidades.CargarVariablesDeAmbiente();

await ChatbotOpenAI.Correr();
//await ChatbotAnthropic.Correr();