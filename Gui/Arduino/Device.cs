using System;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace Argscope.Arduino
{
	public class Device : IDisposable
	{
		SerialPort port;
		readonly public Descriptor Descriptor;
		readonly BinaryReader reader;
		readonly BinaryWriter writer;
		const byte request = (byte)'R',
			ready = (byte)'G';

		public Device(Descriptor descriptor) : this(descriptor, 2000000) { }

		public Device(Descriptor descriptor, int baudRate)
		{
			Descriptor = descriptor;

			port = new SerialPort(
				portName: descriptor.Port,
				baudRate: baudRate,
				parity: Parity.None,
				dataBits: 8,
				stopBits: StopBits.One);

			port.ErrorReceived += (s, e) =>
			{
				throw new ApplicationException(string.Format(
					"Serial error: {0}", e.EventType));
			};

			port.DtrEnable = false;
			port.Open();
			Thread.Sleep(50);
			port.DtrEnable = true;

			reader = new BinaryReader(port.BaseStream);
			writer = new BinaryWriter(port.BaseStream);

			port.ReadTimeout = 5000;

			byte r;
			do r = reader.ReadByte();
			while (r != ready);

			port.ReadTimeout = 500;
		}

		public void Dispose()
		{
			reader.Close();
			reader.Dispose();
			writer.Close();
			writer.Dispose();
			port.Close();
			port.Dispose();
		}

		public UInt16 ReadRaw()
		{
			writer.Write(request);
			return reader.ReadUInt16();
		}
	}
}
