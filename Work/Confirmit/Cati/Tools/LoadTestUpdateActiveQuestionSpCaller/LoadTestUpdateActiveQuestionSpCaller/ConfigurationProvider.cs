using Newtonsoft.Json;

namespace LoadTestUpdateActiveQuestionSpCaller;

public static class ConfigurationProvider
{
    private static Configuration _configuration;

    public static Configuration GetConfiguration()
    {
        if (_configuration == null)
        {
            var settings = File.ReadAllText("appsettings.json");
            _configuration = JsonConvert.DeserializeObject<Configuration>(settings);
        }

        return _configuration;
    }
}