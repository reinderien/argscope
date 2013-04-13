using System.Management;

namespace Argscope.Arduino
{
	public class Descriptor
	{
		public string Description { get; private set; }
		public string Port { get; private set; }
		public uint MaxBaudRate { get; private set; }

		static T MProp<T>(ManagementObject result, string prop)
		{
			return (T)result.Properties[prop].Value;
		}

		public Descriptor(ManagementObject d)
		{
			Description = MProp<string>(d, "Description");
			Port = MProp<string>(d, "DeviceID");
			MaxBaudRate = MProp<uint>(d, "MaxBaudRate");
		}
	}
}