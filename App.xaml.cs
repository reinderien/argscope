using System;
using System.Linq;
using System.Windows;

namespace Argscope
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		Arduino.Hotplug hotplug;

		public App()
		{
			hotplug = new Arduino.Hotplug();
			hotplug.Arrived += arrived =>
			{
			};
			hotplug.Removed += removed =>
			{
			};
		}

		protected override void OnExit(ExitEventArgs e)
		{
			hotplug.Dispose();
		}
    }
}
