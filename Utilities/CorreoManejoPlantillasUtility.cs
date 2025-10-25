namespace EduConnect_API.Utilities
{
    public class CorreoManejoPlantillasUtility
    {
        

        public static string CargarPlantilla(string nombreArchivo)
        {
            // ✅ Ajusta la ruta según tu estructura actual
            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "Utilities", "PlantillasCorreo");
            string ruta = Path.Combine(basePath, nombreArchivo);

            if (!File.Exists(ruta))
                throw new FileNotFoundException($"❌ Plantilla '{nombreArchivo}' no encontrada en {ruta}");

            return File.ReadAllText(ruta);
        }

        public static string ReemplazarVariables(string plantilla, Dictionary<string, string> datos)
        {
            foreach (var kv in datos)
                plantilla = plantilla.Replace($"{{{{{kv.Key}}}}}", kv.Value);
            return plantilla;
        }
    }
}
