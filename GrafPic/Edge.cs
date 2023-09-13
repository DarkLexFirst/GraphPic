using GraphPic.Events;
using Newtonsoft.Json;
using System;
using System.Windows.Media;

namespace GraphPic
{
	public class Edge
	{
		[JsonIgnore]
		public int Id { get; }

		public Vertex Source { get; }
		public Vertex Sink { get; }
		public float? Weight { get; private set; }
		public bool Directed { get; }

		private Brush _color;

		[JsonConstructor]
		private Edge(Vertex source, Vertex sink, float? weight, bool directed) :
			this(0, source, sink, weight, directed)
		{
		}

		public Edge(int id, Vertex source, Vertex sink, float? weight, bool directed)
		{
			Id = id;
			Source = source;
			Sink = sink;
			Weight = weight;
			Directed = directed;
		}

		[JsonIgnore]
		public Brush Color
		{
			get => _color;
			set
			{
				_color = value;
				ColorChanged?.Invoke(this, new EdgeColorUpdateArgs() { Color = value });
			}
		}

		public event EventHandler<EdgeColorUpdateArgs> ColorChanged;

		public void LightRed() => Color = Brushes.Red;

		public void ResetColor() => Color = null;

		public void SetWeight(float? weight) => Weight = weight;
	}
}
