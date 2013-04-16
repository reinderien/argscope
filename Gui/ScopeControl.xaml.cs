using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Argscope
{
	/// <summary>
	/// Interaction logic for ScopeControl.xaml
	/// </summary>
	public partial class ScopeControl : UserControl, IDisposable
	{
		public Scope Scope { get; private set; }

		List<Point> points = new List<Point>();

		public ScopeControl()
		{
			InitializeComponent();

			Scope = new Scope();
			Scope.Trigger += ScopeTrigger;
			Scope.Add += ScopeAdd;
		}

		public void Dispose()
		{
			if (Scope != null)
			{
				Scope.Dispose();
				Scope = null;
			}
		}

		Point SecVoltsToPixel(Point p)
		{
			return new Point(
				p.X * ScopeCanvas.ActualWidth / Scope.HorzWindow.TotalSeconds,
				(1 - p.Y / Scope.VertMaxVolts) * ScopeCanvas.ActualHeight);
		}

		void SyncPoints()
		{
			TraceLine.Points.Clear();
			TraceLine.Points.AddRange(points.Select(p => SecVoltsToPixel(p)));

		}

		private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			SyncPoints();
		}

		void ScopeTrigger()
		{
			points.Clear();
			TraceLine.Points.Clear();
		}

		void ScopeAdd(System.TimeSpan t, double v)
		{
			Point pnew = new Point(t.TotalSeconds, v);
			points.Add(pnew);
			TraceLine.Points.Add(SecVoltsToPixel(pnew));
		}

		public void DevicesChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					if (Scope.Device == null)
					{
						Scope.Device = new Arduino.Device((Arduino.Descriptor)e.NewItems[0]);
						Scope.CaptureAsync();
					}
					break;

				case NotifyCollectionChangedAction.Remove:
					if (!e.NewItems.Contains(Scope.Device.Descriptor))
					{
						Scope.StopCapture();
						Scope.Device.Dispose();
						Scope.Device = null;
					}
					break;
			}
		}

		private void UserControl_Unloaded(object sender, RoutedEventArgs e)
		{
			Scope.Dispose();
			Scope = null;
		}
	}
}
