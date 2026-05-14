using Newtonsoft.Json;

namespace Confirmit.CATI.Supervisor.Core.SupervisorSettings.ActivityViews
{
    public class ColumnDescription
    {
        public string Key { get; set; }

        [JsonIgnore]
        public string ColumnText { get; set; }
        public bool IsVisible { get; set; }
        public int Value { get; set; }

        public ColumnDescription(string key, string columnText, bool isVisible)
        {
            Key = key;
            ColumnText = columnText;
            IsVisible = isVisible;
        }

        public ColumnDescription(string key, string columnText, bool isVisible, int value) : this(key, columnText, isVisible)
        {
            Value = value;
        }

        // empty constructor for JS deserializer
        public ColumnDescription()
        {
        }
    }
}