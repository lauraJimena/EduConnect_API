namespace EduConnect_API.Utilities
{
    public class CorreoManejoPlantillasUtility
    {
        private readonly IWebHostEnvironment _env;

        public CorreoManejoPlantillasUtility(IWebHostEnvironment env)
        {
            _env = env;
        }


        public string CargarPlantilla(string nombreArchivo)
        {
            string basePath = Path.Combine(_env.ContentRootPath, "Utilities", "PlantillasCorreo");


            string ruta = Path.Combine(basePath, nombreArchivo);

            if (!File.Exists(ruta))
                throw new FileNotFoundException($"❌ Plantilla '{nombreArchivo}' no encontrada en {ruta}");

            return File.ReadAllText(ruta);
        }

        public string ReemplazarVariables(string plantilla, Dictionary<string, string> datos)
        {
            foreach (var kv in datos)
                plantilla = plantilla.Replace($"{{{{{kv.Key}}}}}", kv.Value);
            return plantilla;
        }
    }
}
