using ModSettings;

namespace DiyBySantana
{
    internal class Settings : JsonModSettings
    {
        internal static Settings instance = new Settings();

        [Name("Example setting")]
        [Description("Setting description")]
        public bool exampleSetting = false;
    }
}
