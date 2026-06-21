namespace PrimerChatbot;

internal static class Utilidades
{
    internal static void CargarVariablesDeAmbiente()
    {
        foreach (var linea in File.ReadAllLines(".env"))
        {
            // LLAVE=VALOR
            var partes = linea.Split("=");
            if (partes.Length == 2)
            {
                Environment.SetEnvironmentVariable(partes[0], partes[1]);
            }
        }
    }
}
