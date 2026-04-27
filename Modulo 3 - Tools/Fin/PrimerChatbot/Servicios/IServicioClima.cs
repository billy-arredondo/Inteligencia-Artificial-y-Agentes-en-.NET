namespace PrimerChatbot.Servicios
{
    internal interface IServicioClima
    {
        Task<string> ObtenerClima(string ciudad);
    }
}