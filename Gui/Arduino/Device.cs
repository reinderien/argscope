using System;
using System.IO.Ports;

namespace Argscope.Arduino
{
    class Device : IDisposable
    {
        SerialPort port;
		readonly public Descriptor Descriptor;

		public Device(Descriptor descriptor, int baudRate)
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
			port.Close();
			port.Dispose();
        }
    }
}
