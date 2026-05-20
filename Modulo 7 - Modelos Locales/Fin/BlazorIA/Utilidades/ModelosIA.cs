namespace BlazorIA.Utilidades
{
    public static class ModelosIA
    {
        private static readonly Dictionary<string, string> Modelos = new(StringComparer.OrdinalIgnoreCase)
        {
            ["qwen3.5:0.8b"] = "ollama",
            ["qwen3.5:2b"] = "ollama",
        };

        public static string ObtenerProveedor(string modelo)
        {
            if (Modelos.TryGetValue(modelo, out var proveedor))
            {
                return proveedor;
            }

            throw new ArgumentException($"Modelo no soportado: {modelo}");
        }

        public static IEnumerable<string> ObtenerModelosDisponibles() => Modelos.Keys;
        public static string ObtenerModeloPorDefecto => "qwen3.5:2b";
    }
}
