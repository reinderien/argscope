using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Argscope
{
	class MainController
	{
		Arduino.Hotplug hotplug = new Arduino.Hotplug();

		ObservableCollection<Arduino.Descriptor> Devices
		{
			get { return hotplug.Devices; }
		}

	}
}
