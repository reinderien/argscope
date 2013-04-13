using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace Argscope.Arduino
{
	class Hotplug : IDisposable
	{
		List<Descriptor> oldDevices = Devices;
		ManagementEventWatcher arrival, removal;

		public event Action<List<Descriptor>> Arrived, Removed;

		public static List<Descriptor> Devices
		{
			get { return Enumerator.AllDevices.ToList(); }
		}

		enum EventType
		{
			ConfChanged = 1,
			Arrival = 2,
			Removal = 3,
			Docking = 4
		}

		public Hotplug()
		{
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

		void DeviceArrived(object sender, EventArrivedEventArgs e)
		{
			List<Descriptor> newDevices = Devices,
				arrivedDevices = newDevices.Except(oldDevices).ToList();
			if (arrivedDevices.Count > 0)
				Arrived(arrivedDevices);
			oldDevices = newDevices;
		}

		void DeviceRemoved(object sender, EventArrivedEventArgs e)
		{
			List<Descriptor> newDevices = Devices,
				removedDevices = oldDevices.Except(newDevices).ToList();
			if (removedDevices.Count > 0)
				Removed(removedDevices);
			oldDevices = newDevices;
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
