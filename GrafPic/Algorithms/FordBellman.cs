using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GraphPic.Algorithms
{
	public sealed class FordBellman
	{
		[ModuleInitializer]
		public static void Initialize()
		{
			AlgorithmsManager.AddAlgorithm("Ford-Bellman", Execute);
		}

		public static string Execute(GraphData data, Vertex vertex)
		{
			var result = data.Vertexes.ToDictionary(val => val, val => float.MaxValue);
			result[vertex] = 0;

			var lines = new List<string>();

			for (var i = 0; i < result.Count; i++)
			{
				foreach (var edge in data.Edges)
				{
					//if (result[edge.Source] < int.MaxValue)
					{
						result[edge.Sink] = Math.Min(result[edge.Sink], result[edge.Source] + edge.Weight.Value);

						var line = string.Join(", ", result.Select(val => $"{val.Key.Number}: {val.Value}"));

						if (lines.LastOrDefault() != line)
						{
							lines.Add(line);
						}
					}
				}
			}

			return "\n" + string.Join("\n", lines);
		}
	}
}
