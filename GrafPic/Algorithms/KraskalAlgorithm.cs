using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GraphPic.Algorithms
{
	public sealed class KraskalAlgorithm
	{
		[ModuleInitializer]
		public static void Initialize()
		{
			AlgorithmsManager.AddAlgorithm("Kraskal", Execute);
		}

		public static string Execute(GraphData data)
		{
			List<Edge> matchedEdges = new List<Edge>();
			float weight = 0;

			foreach (var edge in data.Edges.OrderBy(edge => edge.Weight ?? 0))
			{
				if (DetectCycle(edge, matchedEdges)) continue;

				edge.LightRed();
				weight += edge.Weight ?? 0;
			}

			return $"Caclculated weight: {weight}";
		}

		private static bool DetectCycle(Edge edge, List<Edge> matchedEdges)
		{
			var _matchedEdges = matchedEdges.ToArray();
			IEnumerable<Vertex> leavesCache = new List<Vertex>() { edge.Source };
			var lowCache = leavesCache;

			matchedEdges.Add(edge);

			while (leavesCache.Any())
			{
				var cache = leavesCache;
				leavesCache = new List<Vertex>();

				foreach (var vertex in cache)
				{
					var vertexes = vertex.Edges
						.Where(edge => _matchedEdges.Contains(edge))
						.Select(edge => edge.Sink == vertex ? edge.Source : edge.Sink)
						.Where(v => !cache.Contains(v) && !lowCache.Contains(v));

					if (vertexes.Contains(edge.Sink)) return true;

					leavesCache = leavesCache.Union(vertexes).ToArray();
				}

				lowCache = cache;
			}

			return false;
		}
	}
}
