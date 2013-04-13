using System;
using System.Windows;

namespace Argscope
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    partial class App : Application
    {
		internal static readonly MainController MainController = new MainController();

		protected override void OnExit(ExitEventArgs e)
		{
		}
    }
}
