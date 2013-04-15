using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management;
using System.Windows;

namespace Argscope.Arduino
{
	class Hotplug : IDisposable
	{
		ManagementEventWatcher arrival, removal;

		public ObservableCollection<Descriptor> Devices { get; private set; }

		enum EventType
		{
			ConfChanged = 1,
			Arrival = 2,
			Removal = 3,
			Docking = 4
		}

		public Hotplug()
		{
			Devices = new ObservableCollection<Descriptor>();

			WqlEventQuery
				arrivalQuery = new WqlEventQuery(
					"select * from Win32_DeviceChangeEvent where EventType = " +
					(int)EventType.Arrival),
				removalQuery = new WqlEventQuery(
					"select * from Win32_DeviceChangeEvent where EventType = " +
					(int)EventType.Removal);

			arrival = new ManagementEventWatcher(arrivalQuery);
			arrival.EventArrived += DeviceArrived;
			arrival.Start();

			removal = new ManagementEventWatcher(removalQuery);
			removal.EventArrived += DeviceRemoved;
			removal.Start();
		}

		public void Sync()
		{
			Devices.Clear();
			Devices.AddRange(Enumerator.AllDevices);
		}

		void DeviceArrived(object sender, EventArrivedEventArgs e)
		{
			List<Descriptor> arrivedDevices =
				Enumerator.AllDevices.Except(Devices).ToList();
			if (arrivedDevices.Count > 0)
				App.Invoke(() => Devices.AddRange(arrivedDevices));
		}

		void DeviceRemoved(object sender, EventArrivedEventArgs e)
		{
			List<Descriptor> removedDevices =
				Devices.Except(Enumerator.AllDevices).ToList();
			if (removedDevices.Count > 0)
				App.Invoke(() => Devices.RemoveRange(removedDevices));
		}

		public void Dispose()
		{
			arrival.Stop();
			arrival.Dispose();
			removal.Stop();
			removal.Dispose();
		}
	}
}
