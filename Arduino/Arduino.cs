using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;

namespace Argscope
{
	class ArduinoDescriptor
	{
		public string Description { get; private set;  }
		public string Port { get; private set; }
		public uint MaxBaudRate { get; private set; }

		static T MProp<T>(ManagementObject result, string prop)
		{
			return (T)result.Properties[prop].Value;
		}

		public ArduinoDescriptor(ManagementObject d)
		{
			Description = MProp<string>(d, "Description");
			Port = MProp<string>(d, "DeviceID");
			MaxBaudRate = MProp<uint>(d, "MaxBaudRate");
		}
	}

    class Arduino : IDisposable
    {
        SerialPort port;
		readonly public ArduinoDescriptor Descriptor;

        public Arduino(ArduinoDescriptor descriptor, int baudRate)
        {
			Descriptor = descriptor;

            port = new SerialPort(
                portName: descriptor.Port,
                baudRate: baudRate,
                parity: Parity.None,
                dataBits: 8,
                stopBits: StopBits.One);
            port.Open();
        }

        public void Dispose()
        {
			port.Dispose();
        }
    }
}
