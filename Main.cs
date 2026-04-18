using HWiNFO64_Plugin;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Variables;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Timers;

namespace sugoides.HWiNFO64_Plugin
{
    public class HWiNFO64Plugin : MacroDeckPlugin
    {
        public override bool CanConfigure => true;

        public static int sensors = 0;

        int refreshTime = 2000;

        readonly Microsoft.Win32.RegistryKey registryPath = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\HWiNFO64\VSB"); //HWiNFO64 Values get stored here;

        internal static MacroDeckPlugin Instance { get; set; }

        public HWiNFO64Plugin()
        {
            Instance = this;
        }

        public override void Enable()
        {
            if (registryPath != null)
            {
                sensors = 0;
                while (registryPath.GetValue("Label" + sensors) != null)
                {
                    sensors++;
                }
            }

            var refreshTimeFromRegistry = PluginConfiguration.GetValue(HWiNFO64Plugin.Instance, "refreshTime");
            if (int.TryParse(refreshTimeFromRegistry, out refreshTime) == false)
                refreshTime = 2000;

            var sensorTimer = new Timer()
            {
                Enabled = true,
                Interval = refreshTime, //Default HWiNFO64 Interval, shouldn't be changed to not cause unnecessary load
            };

            sensorTimer.Elapsed += SensorTimer_Elapsed;
            sensorTimer.Start();
        }

        private void SensorTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            for (int i = 0; i < sensors; i++)
            {
                //set all values as string cause HWiNFO64 already formatted them for us
                var variableName = "hwi64_" + (string)registryPath.GetValue("Label" + i);
                var regexSpecialChars = new Regex("[()]");
                variableName = regexSpecialChars.Replace(variableName, string.Empty);
                VariableManager.SetValue(variableName, (string)registryPath.GetValue("Value" + i), VariableType.String, HWiNFO64Plugin.Instance, (string[])null);
                VariableManager.SetValue(variableName + "_raw", registryPath.GetValue("ValueRaw" + i), VariableType.Float, HWiNFO64Plugin.Instance, (string[])null);
            }
        }

        public override void OpenConfigurator()
        {
            using (var configurator = new PluginConfigurationView())
            {
                configurator.ShowDialog();
            }
        }
    }

}
