using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GraphPic.Algorithms
{
	public sealed class SearchInDeep
	{
		[ModuleInitializer]
		public static void Initialize()
		{
			AlgorithmsManager.AddAlgorithm("Search in deep", Execute);
		}

		public static string Execute(GraphData data, Vertex start)
		{
			if (start == null) return "The initial vertex is required";

			var counted = new List<Vertex>() { start };
			var max = new List<Vertex>();

			var result = DFS(data, start, counted, max);

			if (!result)
			{
				if (max.Count > 0)
				{
					Vertex next = max[0];
					for (var i = 1; i < max.Count; i++)
					{
						var current = next;
						next = max[i];

						var edge = current.Edges.First(edge => edge.Sink == next || edge.Source == next);
						edge.LightRed();
					}
				}

				return $"Way can't be found. max length: {max.Count}";
			}

			return $"Way founded";
		}

		private static bool DFS(GraphData data, Vertex start, List<Vertex> counted, List<Vertex> max)
		{
			foreach (var edge in start.OutgoingEdges)
			{
				var vertex = edge.Sink == start ? edge.Source : edge.Sink;
				if (counted.Contains(vertex)) continue;

				counted.Add(vertex);

				if (max.Count < counted.Count)
				{
					max.Clear();
					max.AddRange(counted);
				}

				if (DFS(data, vertex, counted, max))
				{
					edge.LightRed();
					return true;
				}
				else
				{
					counted.Remove(vertex);
				}
			}

			return counted.Count == data.Vertexes.Length;
		}
	}
}
