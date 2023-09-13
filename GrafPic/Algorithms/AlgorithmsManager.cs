using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphPic.Algorithms
{
	public sealed class AlgorithmsManager
	{

		private static readonly Dictionary<string, Func<GraphData, Vertex, Vertex, string>> _alhorithm = new();

		public static string[] Algorithms => _alhorithm.Keys.ToArray();

		public static void AddAlgorithm(string name, Func<GraphData, Vertex, Vertex, string> algorithm)
		{
			_alhorithm.Add(name, algorithm);
		}

		public static void AddAlgorithm(string name, Func<GraphData, Vertex, string> algorithm)
		{
			_alhorithm.Add(name, (data, first, _) => algorithm(data, first));
		}

		public static void AddAlgorithm(string name, Func<GraphData, string> algorithm)
		{
			AddAlgorithm(name, (data, _) => algorithm(data));
		}

		public static string Execute(string name, GraphData data, Vertex first = null, Vertex second = null)
		{
			if (_alhorithm.TryGetValue(name, out var algorithm))
			{
				return algorithm(data, first, second);
			}

			return null;
		}
	}
}
