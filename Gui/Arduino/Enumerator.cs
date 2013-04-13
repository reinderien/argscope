using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace Argscope.Arduino
{
	static class Enumerator
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

		public static IEnumerable<Descriptor> SelectArduino(
			this IEnumerable<string> portNames)
		{
			return portNames
				.SelectMany(name => ManSearch(name))
				.Select(r => new Descriptor(r));
		}

		public static IEnumerable<Descriptor> AllDevices
		{
			get { return ManSearch().Select(r => new Descriptor(r)); }
		}
	}
}
