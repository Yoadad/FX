using XF.Entities;

namespace XF.Services
{
    public class ConfigService
    {
        public static string GetValue(string key,XFModel db)
        {
            var config = db.Configs.Find(key);
            return string.IsNullOrWhiteSpace(config.Value) 
                ? string.Empty 
                : config.Value; 
        }
    }
}