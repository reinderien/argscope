using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Argscope
{
	public class Scope : IDisposable
	{
		public enum VertMaxEnum
		{
			V1p1,
			V5p0
		}

		public enum TriggerDirEnum
		{
			Positive,
			Negative,
			Continuous,
			Stop
		}

		public event Action Trigger;

		public event Action<TimeSpan, double> Add;

		const UInt16 VertMaxRaw = 1024;

		Task captureTask;
		CancellationToken cancelToken;
		CancellationTokenSource cancelSource;

		public Arduino.Device Device { get; set; }

		VertMaxEnum vertMax = VertMaxEnum.V1p1;
		public VertMaxEnum VertMax
		{
			get { return vertMax; }
			set { vertMax = value; }
		}

		public double VertMaxVolts
		{
			get
			{
				switch (VertMax)
				{
					case VertMaxEnum.V1p1: return 1.1;
					case VertMaxEnum.V5p0: return 5.0;
					default: throw new InvalidOperationException();
				}
			}
		}

		TimeSpan horzWindow = TimeSpan.FromMilliseconds(100);
		public TimeSpan HorzWindow
		{
			get { return horzWindow; }
			set { horzWindow = value; }
		}

		/*
		// For resampling
		
		uint maxHorzResolution;
		public uint MaxHorzResolution
		{
			get { return maxHorzResolution; }
			set
			{
				if (value < 2) throw new ArgumentOutOfRangeException();
				maxHorzResolution = value;
			}
		}*/

		TriggerDirEnum triggerDir = TriggerDirEnum.Continuous;
		public TriggerDirEnum TriggerDir
		{
			get { return triggerDir; }
			set { triggerDir = value; }
		}

		public double TriggerLevelVolts
		{
			get { return triggerLevelRaw * VertMaxVolts / VertMaxRaw; }
			set
			{
				if (value < 0 || value > VertMaxVolts)
					throw new ArgumentOutOfRangeException();
				triggerLevelRaw = (UInt16)(value * VertMaxRaw / VertMaxVolts);
			}
		}

		UInt16 triggerLevelRaw = VertMaxRaw / 2;

		double refreshRate = 5;
		double RefreshRate
		{
			get { return refreshRate; }
			set
			{
				if (refreshRate < 0.1 || refreshRate > 1000)
					throw new ArgumentOutOfRangeException();
				refreshRate = value;
			}
		}

		public TimeSpan HorzWindowEffective
		{
			get
			{
				return TimeSpan.FromSeconds(Math.Min(
					horzWindow.TotalSeconds,
					1 / RefreshRate));
			}
		}

		public Scope()
		{
			captureTask = new Task(CaptureSync);
			cancelSource = new CancellationTokenSource();
			cancelToken = cancelSource.Token;
		}

		public void Dispose()
		{
			StopCapture();
			Device.Dispose();
		}

		void InvokeOrCancel(Action act)
		{
			DispatcherOperation dispatchOp = App.Current.Dispatcher.BeginInvoke(
				new Action(() =>
				{
					if (!cancelToken.IsCancellationRequested)
						act();
				}), null);

			try { dispatchOp.Task.Wait(cancelToken); }
			catch (OperationCanceledException) { }
		}

		void AddRaw(TimeSpan t, UInt16 val)
		{
			if (Add != null)
				InvokeOrCancel(() => Add(t, val * VertMaxVolts / VertMaxRaw));
		}

		bool ShouldTrigger(UInt16 v1, UInt16 v2)
		{
			switch (TriggerDir)
			{
				case TriggerDirEnum.Continuous:
					return true;

				case TriggerDirEnum.Stop:
					return false;

				case TriggerDirEnum.Positive:
					return v1 <= triggerLevelRaw &&
						v2 >= triggerLevelRaw;

				case TriggerDirEnum.Negative:
					return v1 >= triggerLevelRaw &&
						v2 <= triggerLevelRaw;

				default:
					throw new ApplicationException();
			}
		}

		public void CaptureSync()
		{
			while (!cancelToken.IsCancellationRequested)
			{
				// Wait for trigger

				UInt16 vnew, vprev = Device.ReadRaw();
				DateTime tnew, tprev = DateTime.Now;
				for (; ; )
				{
					if (cancelToken.IsCancellationRequested)
						return;

					vnew = Device.ReadRaw();
					tnew = DateTime.Now;

					if (ShouldTrigger(vprev, vnew))
						break;

					vprev = vnew;
					tprev = tnew;
				}

				AddRaw(TimeSpan.Zero, vprev);
				AddRaw(tnew - tprev, vnew);

				// Capture data

				if (Trigger != null)
					InvokeOrCancel(Trigger);

				while ((tnew - tprev).TotalSeconds < 1 / RefreshRate)
				{
					if (cancelToken.IsCancellationRequested)
						return;

					if (tnew - tprev >= HorzWindow)
					{
						Thread.Sleep(TimeSpan.FromSeconds(1 / RefreshRate) -
							(tnew - tprev));
						break;
					}

					vnew = Device.ReadRaw();
					tnew = DateTime.Now;
					AddRaw(tnew - tprev, vnew);
				}
			}
		}

		public void CaptureAsync()
		{
			if (captureTask.Status == TaskStatus.Running)
				throw new InvalidOperationException();
			if (Device == null)
				throw new InvalidOperationException();

			captureTask.Start();
		}

		public void StopCapture()
		{
			if (captureTask.Status == TaskStatus.Running)
			{
				cancelSource.Cancel();
				if (!captureTask.Wait(5000))
					throw new ApplicationException("Couldn't cancel capture thread");
			}
		}
	}
}
