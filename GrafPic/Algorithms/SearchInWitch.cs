using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GraphPic.Algorithms
{
	public sealed class SearchInWitch
	{
		[ModuleInitializer]
		public static void Initialize()
		{
			AlgorithmsManager.AddAlgorithm("Search in witch", Execute);
		}

		public static string Execute(GraphData data, Vertex start)
		{
			if (start == null) return "The initial vertex is required";

			IEnumerable<Vertex> vertexes = new List<Vertex>() { start };
			List<Vertex> matchedVertexes = new List<Vertex>() { start };
			List<Edge> matchedEdges = new List<Edge>();

			float weight = 0;

			while (vertexes.Any())
			{
				List<Vertex> temp = new List<Vertex>();

				foreach (var vertex in vertexes)
				{
					foreach (var edge in vertex.OutgoingEdges)
					{
						if (matchedEdges.Contains(edge) ||
							(matchedVertexes.Contains(edge.Sink) && matchedVertexes.Contains(edge.Source)))
						{
							continue;
						}

						var anyVertex = edge.Sink == vertex ? edge.Source : edge.Sink;
						temp.Add(anyVertex);
						matchedVertexes.Add(anyVertex);
						matchedEdges.Add(edge);

						edge.LightRed();
						weight += edge.Weight ?? 0;
					}
				}

				vertexes = temp;
			}

			return $"Caclculated weight: {weight}";
		}
	}
}
