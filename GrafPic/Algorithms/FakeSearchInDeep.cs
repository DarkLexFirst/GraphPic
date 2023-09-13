using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GraphPic.Algorithms
{
	public sealed class FakeSearchInDeep
	{
		[ModuleInitializer]
		public static void Initialize()
		{
			//AlgorithmsManager.AddAlgorithm("Fake search in deep", Execute);
		}

		public static string Execute(GraphData data, Vertex start)
		{
			if (start == null) return "The initial vertex is required";

			List<Vertex> counted = new List<Vertex>() { start };

			float weight = GoDeep(start, counted);

			//while (start != null)
			//{
			//	start = GetNextVertex(start, counted, out float _weight);
			//	counted.Add(start);
			//	weight += _weight;
			//}

			return $"Caclculated weight: {weight}";
		}

		private static float GoDeep(Vertex start, List<Vertex> counted)
		{
			float weight = 0;

			while (start != null)
			{
				start = GetNextVertex(start, counted, out float _weight);
				counted.Add(start);
				weight += _weight;
			}

			return weight;
		}

		private static Vertex GetNextVertex(Vertex current, List<Vertex> counted, out float weight)
		{
			weight = 0;

			var edge = current.OutgoingEdges
				.Where(edge => !counted.Contains(edge.Sink) || !counted.Contains(edge.Source))
				.OrderBy(edge => edge.Weight ?? 0)
				.FirstOrDefault();

			if (edge == null) return null;

			weight = edge.Weight ?? 0;
			edge.LightRed();

			return edge.Sink == current ? edge.Source : edge.Sink;
		}
	}
}
