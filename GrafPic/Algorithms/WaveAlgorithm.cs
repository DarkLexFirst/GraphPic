using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GraphPic.Algorithms
{
	public class WaveAlgorithm
	{
		[ModuleInitializer]
		public static void Initialize()
		{
			AlgorithmsManager.AddAlgorithm("Wave algorithm", Execute);
			AlgorithmsManager.AddAlgorithm("Wave algorithm (one zero)", ZeroExecute);
			AlgorithmsManager.AddAlgorithm("Wave algorithm (one negative)", NegativeExecute);
		}

		public static string Execute(GraphData data, Vertex first, Vertex second)
		{
			if (first == null || second == null)
			{
				return "Select first and second vertexes!";
			}

			var waves = GetWaves(first);
			var way = GetMinWay(waves, second).GetResultWay();

			ColorWay(way);
			return string.Join(", ", way.Select(v => v.Number));
		}

		public static string ZeroExecute(GraphData data, Vertex first, Vertex second)
		{
			return WeightUpdateExecute(data, first, second, w => w.HasValue ? 0 : null);
		}

		public static string NegativeExecute(GraphData data, Vertex first, Vertex second)
		{
			return WeightUpdateExecute(data, first, second, w => w.HasValue ? -w : null);
		}

		private static string WeightUpdateExecute(GraphData data, Vertex first, Vertex second, Func<float?, float?> updateWeight)
		{
			StringBuilder sb = new StringBuilder("Ways:\n");

			foreach (var edge in data.Edges)
			{
				var oldWeight = edge.Weight;
				edge.SetWeight(updateWeight(edge.Weight));

				var waves = GetWaves(first);
				var way = GetMinWay(waves, second);
				var resultWay = way.GetResultWay();

				sb.AppendLine($"way with set edge {edge.Source.Number}-{edge.Sink.Number} to {edge.Weight}: {string.Join(", ", resultWay.Select(v => v.Number))} = {way.Weight}");

				edge.SetWeight(oldWeight);
			}

			return sb.ToString();
		}

		private static Way GetMinWay(Dictionary<Vertex, int> waves, Vertex second)
		{
			return Step(waves, new Way(new() { second }), null);
		}

		private static Way Step(Dictionary<Vertex, int> waves, Way currentWay, Way minWay)
		{
			var last = currentWay.Last;
			var currentWave = waves[last];

			foreach (var next in last.IncomingEdges.Select(edge => edge.Source))
			{
				var nextWave = waves[next];
				if (nextWave > currentWave) continue;

				var newWay = currentWay.GetNext(next);

				if (nextWave != 0)
				{
					newWay = Step(waves, newWay, minWay);
				}

				if (minWay == null || newWay.Weight < minWay.Weight)
				{
					minWay = newWay;
				}
			}

			return minWay;
		}

		private static Dictionary<Vertex, int> GetWaves(Vertex first)
		{
			var waves = new Dictionary<Vertex, int>();

			var currentWave = new List<Vertex>() { first };

			int wave = 0;
			while(currentWave.Count > 0)
			{
				var tmp = new List<Vertex>();

				foreach (var vertex in currentWave)
				{
					if (waves.ContainsKey(vertex)) continue;

					waves.Add(vertex, wave);
					tmp.AddRange(vertex.OutgoingEdges.Select(edge => edge.Sink));
				}

				currentWave = tmp;
				wave++;
			}

			return waves;
		}

		private static void ColorWay(List<Vertex> way)
		{
			for (var i = 0; i < way.Count - 1; i++)
			{
				var current = way[i];
				var next = way[i + 1];

				var edge = current.OutgoingEdges.First(edge => edge.Sink == next);

				var filling = (double)i / (way.Count - 1);
				edge.Color = new SolidColorBrush(Color.FromRgb((byte)(0xFF * (1 - filling)), (byte)(0xFF * filling), 0x00));
			}
		}

		private class Way
		{
			public float Weight { get; }
			public Vertex Last => _vertexWay.Last();

			private List<Vertex> _vertexWay;

			public Way(List<Vertex> vertexWay)
			{
				_vertexWay = vertexWay;

				float weight = 0;
				for (var i = 0; i < vertexWay.Count - 1; i++)
				{
					var current = vertexWay[i];
					var next = vertexWay[i + 1];

					weight += current.IncomingEdges.First(edge => edge.Source == next).Weight ?? 0;
				}

				Weight = weight;
			}

			private Way(Way old, Vertex next)
			{
				var diff = old._vertexWay.Last().IncomingEdges.First(edge => edge.Source == next).Weight ?? 0;
				
				var newWay = old._vertexWay.ToList();
				newWay.Add(next);

				_vertexWay = newWay;
				Weight = old.Weight + diff;
			}

			public Way GetNext(Vertex next)
			{
				return new Way(this, next);
			}

			public List<Vertex> GetResultWay()
			{
				var tmp = _vertexWay.ToList();
				tmp.Reverse();

				return tmp;
			}
		}
	}
}
