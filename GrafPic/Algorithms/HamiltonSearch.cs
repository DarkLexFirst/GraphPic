using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace GraphPic.Algorithms
{
	public class HamiltonSearch
	{
		[ModuleInitializer]
		public static void Initialize()
		{
			AlgorithmsManager.AddAlgorithm("Hamilton search", TryFindHamilton);
			AlgorithmsManager.AddAlgorithm("Semi-Hamilton search", TryFindSemiHamilton);
			AlgorithmsManager.AddAlgorithm("Add edges to Hamilton", AddEdgesToHamilton);
			AlgorithmsManager.AddAlgorithm("Add edges to Semi-Hamilton", AddEdgesToSemiHamilton);
			AlgorithmsManager.AddAlgorithm("Add edges to directed Hamilton", AddEdgesToHamiltonDirected);
			AlgorithmsManager.AddAlgorithm("Add edges to directed Semi-Hamilton", AddEdgesToSemiHamiltonDirected);
			AlgorithmsManager.AddAlgorithm("Add vertex and edges to Hamilton", AddVertexAndEdgesToHamilton);
			AlgorithmsManager.AddAlgorithm("Add vertex and edges to Semi-Hamilton", AddVertexAndEdgesToSemiHamilton);
			AlgorithmsManager.AddAlgorithm("Add vertex and edges to directed Hamilton", AddVertexAndEdgesToHamiltonDirected);
			AlgorithmsManager.AddAlgorithm("Add vertex and edges to directed Semi-Hamilton", AddVertexAndEdgesToSemiHamiltonDirected);
			AlgorithmsManager.AddAlgorithm("Search for Hamilton", SearchForHamilton);
			AlgorithmsManager.AddAlgorithm("Search for Semi-Hamilton", SearchForSemiHamilton);
		}

		private static IEnumerator<List<int>> _ways;
		private static IEnumerator<List<int>> _semiways;

		public static string TryFindHamilton(GraphData data)
		{
			var way = GetHamiltonWay(data.Edges, data.Vertexes, new(), new(), false);
			return BuildOutput(way);
		}

		public static string TryFindSemiHamilton(GraphData data)
		{
			var way = GetHamiltonWay(data.Edges, data.Vertexes, new(), new(), true);
			return BuildOutput(way);
		}

		public static string AddEdgesToHamilton(GraphData data)
		{
			if (data.Edges.Any(edge => edge.Directed)) return "Only with non-directed graphs";

			AddEdgesToHamilton(data, false, false);

			return BuildOutput(GetHamiltonWay(data.Edges, data.Vertexes, new(), new(), false));
		}

		public static string AddEdgesToSemiHamilton(GraphData data)
		{
			if (data.Edges.Any(edge => edge.Directed)) return "Only with non-directed graphs";

			AddEdgesToHamilton(data, false, true);

			return BuildOutput(GetHamiltonWay(data.Edges, data.Vertexes, new(), new(), true));
		}

		public static string AddEdgesToHamiltonDirected(GraphData data)
		{
			if (data.Edges.Any(edge => !edge.Directed)) return "Only with directed graphs";

			AddEdgesToHamilton(data, true, false);

			return BuildOutput(GetHamiltonWay(data.Edges, data.Vertexes, new(), new(), false));
		}

		public static string AddEdgesToSemiHamiltonDirected(GraphData data)
		{
			if (data.Edges.Any(edge => !edge.Directed)) return "Only with directed graphs";

			AddEdgesToHamilton(data, true, true);

			return BuildOutput(GetHamiltonWay(data.Edges, data.Vertexes, new(), new(), true));
		}

		public static string AddVertexAndEdgesToHamilton(GraphData data)
		{
			if (data.Edges.Any(edge => edge.Directed)) return "Only with non-directed graphs";

			AddVertexesAndEdgesToHamilton(data, false, false);

			return BuildOutput(GetHamiltonWay(data.Edges, data.Vertexes, new(), new(), false));
		}

		public static string AddVertexAndEdgesToSemiHamilton(GraphData data)
		{
			if (data.Edges.Any(edge => edge.Directed)) return "Only with non-directed graphs";

			AddVertexesAndEdgesToHamilton(data, false, true);

			return BuildOutput(GetHamiltonWay(data.Edges, data.Vertexes, new(), new(), true));
		}

		public static string AddVertexAndEdgesToHamiltonDirected(GraphData data)
		{
			if (data.Edges.Any(edge => !edge.Directed)) return "Only with directed graphs";

			AddVertexesAndEdgesToHamilton(data, true, false);

			return BuildOutput(GetHamiltonWay(data.Edges, data.Vertexes, new(), new(), false));
		}

		public static string AddVertexAndEdgesToSemiHamiltonDirected(GraphData data)
		{
			if (data.Edges.Any(edge => !edge.Directed)) return "Only with directed graphs";

			AddVertexesAndEdgesToHamilton(data, true, true);

			return BuildOutput(GetHamiltonWay(data.Edges, data.Vertexes, new(), new(), true));
		}

		private static void AddEdgesToHamilton(GraphData data, bool directed, bool semi)
		{
			var way = GetHamiltonWay(data.Edges, data.Vertexes, new(), new(), semi);
			if (way != null) return;

			var edges = data.Edges.ToList();

			var toWay = data.Vertexes.Except(_maxWay).ToList();
			if (!semi) toWay.Add(_maxWay.First());

			var last = _maxWay.Last();
			foreach (var vertex in toWay)
			{
				TryAddVertex(edges, last, vertex, directed);

				last = vertex;
			}

			data.Update(data.Vertexes, edges.ToArray());
		}

		private static void AddVertexesAndEdgesToHamilton(GraphData data, bool directed, bool semi)
		{
			var way = GetHamiltonWay(data.Edges, data.Vertexes, new(), new(), semi);
			if (way != null) return;

			var edges = data.Edges.ToList();
			var vertexes = data.Vertexes.ToList();

			var toWay = data.Vertexes.Except(_maxWay).ToList();
			if (!semi) toWay.Add(_maxWay.First());

			var last = _maxWay.Last();
			int count = 0;
			foreach (var vertex in toWay)
			{
				if (count++ < 2)
				{
					var mildde = new Vertex(vertexes.Count, vertexes.Count, new Vector2(10));
					vertexes.Add(mildde);

					edges.Add(AddEdge(last, mildde, edges.Count, directed));
					edges.Add(AddEdge(mildde, vertex, edges.Count, directed));
				}
				else
				{
					TryAddVertex(edges, last, vertex, directed);
				}

				last = vertex;
			}

			data.Update(vertexes.ToArray(), edges.ToArray());
		}

		private static void TryAddVertex(List<Edge> edges, Vertex first, Vertex second, bool directed)
		{
			if (edges.Any(edge => 
				(edge.Source == first && edge.Sink == second) 
				|| (edge.Source == second && edge.Sink == first)))
			{
				return;
			}

			edges.Add(AddEdge(first, second, edges.Count, directed));
		}

		private static Edge AddEdge(Vertex first, Vertex second, int id, bool directed)
		{
			var edge = new Edge(id, first, second, null, directed);

			first.AddEdge(edge, true);
			second.AddEdge(edge, false);

			return edge;
		}

		public static string SearchForHamilton(GraphData data)
		{
			try
			{
				if (_ways?.Current == null) _ways = null;
				_ways ??= SubSearchForHamilton(data.Edges, data.Vertexes, false).GetEnumerator();

				do
				{
					_ways.MoveNext();
				}
				while (_semiways.Current.First() != _semiways.Current.Last());
			}
			catch
			{
				return "Can't find Hamilton's way";
			}

			return BuildOutput(_ways.Current);
		}

		public static string SearchForSemiHamilton(GraphData data)
		{
			try
			{
				if (_semiways?.Current == null) _semiways = null;
				_semiways ??= SubSearchForHamilton(data.Edges, data.Vertexes, true).GetEnumerator();

				do
				{
					_semiways.MoveNext();
				}
				while (_semiways.Current.First() == _semiways.Current.Last());
			}
			catch
			{
				return "Can't find Semi-Hamilton's way";
			}

			return BuildOutput(_semiways.Current);
		}

		private static IEnumerable<List<int>> SubSearchForHamilton(Edge[] edges, Vertex[] vertexes, bool semi)
		{
			var combinations = vertexes.Select(vertex => new List<Vertex>() { vertex });

			for (var i = 3; i < vertexes.Length; i++)
			{
				var tmp = new List<List<Vertex>>();

				foreach (var combo in combinations)
				{
					var way = GetHamiltonWay(edges, vertexes, new(), combo, semi);
					if (way != null) yield return way;

					foreach (var vertex in vertexes)
					{
						if (combo.Last().Id <= vertex.Id) continue;

						var newCombo = combo.ToList();
						newCombo.Add(vertex);

						tmp.Add(newCombo);
					}
				}

				combinations = tmp;
			}
		}

		private static List<Vertex> _maxWay;

		private static List<int> GetHamiltonWay(Edge[] edges, Vertex[] vertexes, List<Edge> counted, List<Vertex> ignore, bool semi)
		{
			_maxWay = null;

			foreach (var start in vertexes)
			{
				if (ignore.Contains(start)) continue;

				var edgesWay = counted.ToList();
				var way = GetHamiltonWay(edges, vertexes, edgesWay, new() { start }, ignore, semi);
				if (way != null)
				{
					foreach (var noWay in ignore.SelectMany(vertex => vertex.Edges).ToList())
					{
						noWay.Color = Brushes.White;
					}

					ColorWay(edgesWay);

					return way;
				}
			}

			return null;
		}

		private static List<int> GetHamiltonWay(Edge[] edges, Vertex[] vertexes, List<Edge> counted, List<Vertex> vCounted, List<Vertex> ignore, bool semi)
		{
			var first = vCounted.First();
			var last = vCounted.Last();

			foreach (var edge in last.OutgoingEdges)
			{
				if (counted.Contains(edge)) continue;

				var next = edge.Source == last ? edge.Sink : edge.Source;
				if (ignore.Contains(next)) continue;

				if (vCounted.Count == vertexes.Length - ignore.Count && next == first)
				{
					counted.Add(edge);
					vCounted.Add(first);
					return vCounted.Select(vertex => vertex.Number).ToList();
				}

				if (vCounted.Contains(next)) continue;

				counted.Add(edge);
				vCounted.Add(next);

				if (semi && vCounted.Count == vertexes.Length - ignore.Count)
				{
					return vCounted.Select(vertex => vertex.Number).ToList();
				}
				else
				{
					var way = GetHamiltonWay(edges, vertexes, counted, vCounted, ignore, semi);
					if (way != null) return way;

					if ((_maxWay?.Count ?? 0) < vCounted.Count)
					{
						_maxWay = vCounted.ToList();
					}

					counted.RemoveAt(counted.Count - 1);
					vCounted.RemoveAt(vCounted.Count - 1);
				}
			}

			return null;
		}

		private static string BuildOutput(List<int> way)
		{
			if (way == null)
			{
				return "Non-Hamilton graph; Way can't be founded";
			}
			else
			{
				if (way.First() == way.Last())
				{
					return $"Hamilton graph; Way founded: {string.Join(", ", way)}";
				}
				else
				{
					return $"Semi-Hamilton graph; Way founded: {string.Join(", ", way)}";
				}
			}
		}

		private static void ColorWay(List<Edge> way)
		{
			for (var i = 0; i < way.Count; i++)
			{
				var edge = way[i];

				var filling = (double)i / way.Count;
				edge.Color = new SolidColorBrush(Color.FromRgb((byte)(0xFF * (1 - filling)), (byte)(0xFF * filling), 0x00));
			}
		}
	}
}
