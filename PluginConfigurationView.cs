using Ize.HWiNFO64_Plugin;
using SuchByte.MacroDeck.Plugins;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;


namespace HWiNFO64_Plugin
{
    public partial class PluginConfigurationView : Form
    {
        float angle = 0;
        float rotSpeed = 5;
        Point origin = new Point(51, 237);  // your origin
        int distance = 50;                  // your distance

        public PluginConfigurationView()
        {
            InitializeComponent();
        }

        private void PluginConfigurationView_Shown(object sender, EventArgs e)
        {
            sensorsCountLabel.Text = "" + HWiNFO64Plugin.sensors;

            var refreshTimeFromRegistry = PluginConfiguration.GetValue(HWiNFO64Plugin.Instance, "refreshTime");
            if (int.TryParse(refreshTimeFromRegistry, out var refreshTime) == false)
                refreshTime = 2000; //default

            refreshTimeInput.Value = refreshTime;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer", "https://www.hwinfo.com");
        }

        private void SaveSettingsButton_Click(object sender, EventArgs e)
        {
            PluginConfiguration.SetValue(HWiNFO64Plugin.Instance, "refreshTime", refreshTimeInput.Value.ToString());
            MessageBox.Show("Settings Saved.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void lameTimer_Tick(object sender, EventArgs e)
        {
            angle += rotSpeed;
            int x = (int)(origin.X + distance * Math.Sin(angle / 2 * Math.PI / 180f));
            int y = (int)(origin.Y + distance * Math.Cos(angle * 2 * Math.PI / 180f));
            izeLogo.Location = new Point(x, y);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer", "https://github.com/Ize83/Ize.HWiNFO64");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            Microsoft.Win32.RegistryKey registryPath = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\HWiNFO64\VSB");
            if (registryPath != null)
            {
                for (int i = 0; i < HWiNFO64Plugin.sensors; i++)
                {
                    var item = new ListViewItem
                    {
                        Text = i.ToString()
                    };
                    item.SubItems.Add(registryPath.GetValue("Sensor" + i).ToString());
                    item.SubItems.Add(registryPath.GetValue("Label" + i).ToString());
                    item.SubItems.Add(registryPath.GetValue("Value" + i).ToString());
                    item.SubItems.Add(registryPath.GetValue("ValueRaw" + i).ToString());

                    item.Font = new Font(item.Font, FontStyle.Regular);
                    listView1.Items.Add(item);
                }
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            this.Width = 1091;
        }
    }

}
