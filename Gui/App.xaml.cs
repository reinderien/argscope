using System;
using System.Windows;

namespace Argscope
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	partial class App : Application
	{
		internal static void Invoke(Action a)
		{
			Application.Current.Dispatcher.Invoke(a);
		}
	}
}
