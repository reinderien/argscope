using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;

namespace Argscope
{
	static class ArduinoEnumerator
	{
		static IEnumerable<ManagementObject> ManQuery(string query)
		{
			ManagementObjectSearcher search = new ManagementObjectSearcher(query);
			return search.Get().Cast<ManagementObject>();
		}

		static IEnumerable<ManagementObject> ManSearch()
		{
			return ManQuery("select * from Win32_SerialPort where " +
				"Description like '%Arduino%'");
		}

		static IEnumerable<ManagementObject> ManSearch(string port)
		{
			return ManQuery(string.Format(
				"select * from Win32_SerialPort where " +
				"Description like '%Arduino%' and " +
				"DeviceID = '{0}'", port));
		}

		public static IEnumerable<ArduinoDescriptor> SelectArduino(
			this IEnumerable<string> portNames)
		{
			return portNames
				.SelectMany(name => ManSearch(name))
				.Select(r => new ArduinoDescriptor(r));
		}

		public static IEnumerable<ArduinoDescriptor> AllDevices
		{
			get { return ManSearch().Select(r => new ArduinoDescriptor(r)); }
		}
	}

	class ArduinoHotplug : IDisposable
	{
		List<ArduinoDescriptor> oldDevices = Devices;
		ManagementEventWatcher arrival, removal;

		public event Action<List<ArduinoDescriptor>> Arrived, Removed;

		public static List<ArduinoDescriptor> Devices
		{
			get { return ArduinoEnumerator.AllDevices.ToList(); }
		}

		enum EventType
		{
			ConfChanged = 1,
			Arrival = 2,
			Removal = 3,
			Docking = 4
		}

		public ArduinoHotplug()
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
			List<ArduinoDescriptor> newDevices = Devices,
				arrivedDevices = newDevices.Except(oldDevices).ToList();
			if (arrivedDevices.Count > 0)
				Arrived(arrivedDevices);
			oldDevices = newDevices;
		}

		void DeviceRemoved(object sender, EventArrivedEventArgs e)
		{
			List<ArduinoDescriptor> newDevices = Devices,
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
