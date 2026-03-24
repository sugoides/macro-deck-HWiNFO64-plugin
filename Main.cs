using HWiNFO64_Plugin;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Variables;
using System.Timers;

namespace Ize.HWiNFO64_Plugin
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
                sensors = registryPath.ValueCount / 5; //each sensor has 5 values in registry: Color, Label, Sensor, Value, ValueRaw; counting starts at 0
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
                VariableManager.SetValue("hwi64_" + (string)registryPath.GetValue("Label" + i), (string)registryPath.GetValue("Value" + i), VariableType.String, HWiNFO64Plugin.Instance, true);
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
