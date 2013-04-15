using System;
using System.IO;
using System.IO.Ports;

namespace Argscope.Arduino
{
	public class Device : IDisposable
	{
		SerialPort port;
		readonly public Descriptor Descriptor;

		readonly public BinaryReader Reader;

		public Device(Descriptor descriptor, int baudRate)
		{
			Descriptor = descriptor;

			port = new SerialPort(
				portName: descriptor.Port,
				baudRate: baudRate,
				parity: Parity.None,
				dataBits: 8,
				stopBits: StopBits.One)
			{
				ReadTimeout = 1000
			};
			port.Open();

			Reader = new BinaryReader(port.BaseStream);
		}

		public void Dispose()
		{
			Reader.Close();
			Reader.Dispose();
			port.Close();
			port.Dispose();
		}
	}
}
