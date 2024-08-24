namespace Aether.Core.Utils
{
    public static class Logging
    {
        public static string GetRequiredLogsFolder(string filename)
        {
            try
            {
                var currentDir = Directory.GetCurrentDirectory();
                var appDataDir = Path.Join(currentDir, "App_Data", "logs");

                Directory.CreateDirectory(appDataDir);

                return Path.Join(currentDir, "App_Data", "logs", filename);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to create App_Data directory. See inner-exception for details", ex);
            }
        }
    }
}
