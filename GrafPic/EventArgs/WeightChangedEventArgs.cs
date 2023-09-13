using System;

namespace GraphPic.Events
{
	public class WeightChangedEventArgs : EventArgs
	{
		public float? Weight { get; set; }
	}
}
