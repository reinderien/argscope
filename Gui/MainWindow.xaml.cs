using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Argscope
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	partial class MainWindow : Window
	{
		Arduino.Hotplug hotplug = new Arduino.Hotplug();

		public ObservableCollection<Arduino.Descriptor> Devices
		{
			get { return hotplug.Devices; }
		}

		public MainWindow()
		{
			InitializeComponent();

			Devices.CollectionChanged += ScopeView.DevicesChanged;
			hotplug.Sync();
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			hotplug.Dispose();
			ScopeView.Dispose();
		}
	}
}
