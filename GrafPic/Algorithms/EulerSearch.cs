using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace GraphPic.Algorithms
{
	public class EulerSearch
    {
		[ModuleInitializer]
		public static void Initialize()
		{
			AlgorithmsManager.AddAlgorithm("Euler search", Execute);
			AlgorithmsManager.AddAlgorithm("Add edges to Euler", AddEdgesToEuler);
			AlgorithmsManager.AddAlgorithm("Add edges to Semi-Euler", AddEdgesToSemiEuler);
			AlgorithmsManager.AddAlgorithm("Add edges to directed Euler", AddEdgesToEulerDirected);
			AlgorithmsManager.AddAlgorithm("Add edges to directed Semi-Euler", AddEdgesToSemiEulerDirected);
			AlgorithmsManager.AddAlgorithm("Add vertex and edges to Euler", AddVertexAndEdgesToEuler);
			AlgorithmsManager.AddAlgorithm("Add vertex and edges to Semi-Euler", AddVertexAndEdgesToSemiEuler);
			AlgorithmsManager.AddAlgorithm("Add vertex and edges to directed Euler", AddVertexAndEdgesToEulerDirected);
			AlgorithmsManager.AddAlgorithm("Add vertex and edges to directed Semi-Euler", AddVertexAndEdgesToSemiEulerDirected);
			AlgorithmsManager.AddAlgorithm("Search for Euler", SearchForEuler);
			AlgorithmsManager.AddAlgorithm("Search for Semi-Euler", SearchForSemiEuler);
		}

		private static IEnumerator<List<int>> _ways;

		public static string Execute(GraphData data)
		{
			return BuildOutput(GetEulerWay(data.Edges, data.Vertexes, new List<Edge>()));
		}

		public static string AddEdgesToEuler(GraphData data)
		{
			if (data.Edges.Any(edge => edge.Directed)) return "Only with non-directed graphs";

			AddEdgesToEuler(data, false);

			return BuildOutput(GetEulerWay(data.Edges, data.Vertexes, new List<Edge>()));
		}

		public static string AddEdgesToSemiEuler(GraphData data)
		{
			if (data.Edges.Any(edge => edge.Directed)) return "Only with non-directed graphs";

			AddEdgesToEuler(data, true);

			return BuildOutput(GetEulerWay(data.Edges, data.Vertexes, new List<Edge>()));
		}

		public static string AddEdgesToEulerDirected(GraphData data)
		{
			if (data.Edges.Any(edge => !edge.Directed)) return "Only with directed graphs";

			AddEdgesToEulerDirected(data, false);

			return BuildOutput(GetEulerWay(data.Edges, data.Vertexes, new List<Edge>()));
		}

		public static string AddEdgesToSemiEulerDirected(GraphData data)
		{
			if (data.Edges.Any(edge => !edge.Directed)) return "Only with directed graphs";

			AddEdgesToEulerDirected(data, true);

			return BuildOutput(GetEulerWay(data.Edges, data.Vertexes, new List<Edge>()));
		}

		public static string AddVertexAndEdgesToEuler(GraphData data)
		{
			if (data.Edges.Any(edge => edge.Directed)) return "Only with non-directed graphs";

			AddVertexAndEdgesToEuler(data, false);

			return BuildOutput(GetEulerWay(data.Edges, data.Vertexes, new List<Edge>()));
		}

		public static string AddVertexAndEdgesToSemiEuler(GraphData data)
		{
			if (data.Edges.Any(edge => edge.Directed)) return "Only with non-directed graphs";

			AddVertexAndEdgesToEuler(data, true);

			return BuildOutput(GetEulerWay(data.Edges, data.Vertexes, new List<Edge>()));
		}

		public static string AddVertexAndEdgesToEulerDirected(GraphData data)
		{
			if (data.Edges.Any(edge => !edge.Directed)) return "Only with directed graphs";

			AddVertexAndEdgesToEulerDirected(data, false);

			return BuildOutput(GetEulerWay(data.Edges, data.Vertexes, new List<Edge>()));
		}

		public static string AddVertexAndEdgesToSemiEulerDirected(GraphData data)
		{
			if (data.Edges.Any(edge => !edge.Directed)) return "Only with directed graphs";

			AddVertexAndEdgesToEulerDirected(data, true);

			return BuildOutput(GetEulerWay(data.Edges, data.Vertexes, new List<Edge>()));
		}

		private static void AddEdgesToEuler(GraphData data, bool semiEuler)
		{
			var lim = semiEuler ? 2 : 0;

			while (true)
			{
				var oddVertexes = data.Vertexes.Where(vertex => vertex.Edges.Length % 2 != 0).ToList();

				if (oddVertexes.Count <= lim) break;

				var first = oddVertexes[0];
				var second = oddVertexes.FirstOrDefault(vertex => first != vertex && !vertex.Edges.Any(edge => 
					(edge.Source == first && edge.Sink == vertex) || (edge.Source == vertex && edge.Sink == first))) ?? oddVertexes[1];

				var newEdges = data.Edges.ToList();
				newEdges.Add(AddEdge(first, second, newEdges.Count, false));

				data.UpdateData(data.Vertexes, newEdges.ToArray());
			}

			data.Update();
		}

		private static void AddVertexAndEdgesToEuler(GraphData data, bool semiEuler)
		{
			var lim = semiEuler ? 2 : 0;

			var first = new Vertex(data.Vertexes.Length, data.Vertexes.Length, new Vector2(10));
			var vertexes = data.Vertexes.ToList();
			vertexes.Add(first);
			data.UpdateData(vertexes.ToArray(), data.Edges);

			while (true)
			{
				var oddVertexes = data.Vertexes.Where(vertex => vertex != first && vertex.Edges.Length % 2 != 0).ToList();

				if (oddVertexes.Count <= lim) break;

				var second = oddVertexes.First(vertex => vertex != first);

				var newEdges = data.Edges.ToList();
				newEdges.Add(AddEdge(first, second, newEdges.Count, false));

				data.UpdateData(data.Vertexes, newEdges.ToArray());
			}

			data.Update();
		}

		private static Edge AddEdge(Vertex first, Vertex second, int id, bool directed)
		{
			var edge = new Edge(id, first, second, null, directed);

			first.AddEdge(edge, true);
			second.AddEdge(edge, false);

			return edge;
		}

		private static void AddEdgesToEulerDirected(GraphData data, bool semiEuler)
		{
			var lim = semiEuler ? 2 : 0;

			while (true)
			{
				var inVertexes = data.Vertexes.Where(vertex => vertex.OutgoingEdges.Length < vertex.IncomingEdges.Length).ToList();
				var outVertexes = data.Vertexes.Where(vertex => vertex.OutgoingEdges.Length > vertex.IncomingEdges.Length).ToList();

				if (outVertexes.Count + inVertexes.Count <= lim) break;

				var first = inVertexes.First();
				var second = outVertexes.FirstOrDefault(vertex => first != vertex && !vertex.Edges.Any(edge =>
					(edge.Source == first && edge.Sink == vertex) || (edge.Source == vertex && edge.Sink == first))) ?? outVertexes[0];

				var newEdges = data.Edges.ToList();
				newEdges.Add(AddEdge(first, second, newEdges.Count, true));

				data.UpdateData(data.Vertexes, newEdges.ToArray());
			}

			data.Update();
		}

		private static void AddVertexAndEdgesToEulerDirected(GraphData data, bool semiEuler)
		{
			var lim = semiEuler ? 2 : 0;

			var tmp = new Vertex(data.Vertexes.Length, data.Vertexes.Length, new Vector2(10));
			var vertexes = data.Vertexes.ToList();
			vertexes.Add(tmp);
			data.UpdateData(vertexes.ToArray(), data.Edges);

			while (true)
			{
				var oddVertexes = data.Vertexes.Where(vertex => vertex.OutgoingEdges.Length != vertex.IncomingEdges.Length).ToList();

				if (oddVertexes.Count <= lim) break;

				var second = oddVertexes.First(vertex => vertex != tmp);

				var direction = second.IncomingEdges.Length < second.OutgoingEdges.Length;
				var first = tmp;
				if (!direction)
				{
					first = second;
					second = tmp;
				}

				var newEdges = data.Edges.ToList();
				newEdges.Add(AddEdge(first, second, newEdges.Count, true));

				data.UpdateData(data.Vertexes, newEdges.ToArray());
			}

			data.Update();
		}

		public static string SearchForEuler(GraphData data)
		{
			_ways ??= SubSearchForEuler(data.Edges, data.Vertexes, new(), new()).GetEnumerator();

			do
			{
				_ways.MoveNext();
			}
			while (_ways.Current.First() != _ways.Current.Last());

			return BuildOutput(_ways.Current);
		}

		public static string SearchForSemiEuler(GraphData data)
		{
			_ways ??= SubSearchForEuler(data.Edges, data.Vertexes, new(), new()).GetEnumerator();

			do
			{
				_ways.MoveNext();
			}
			while (_ways.Current.First() == _ways.Current.Last());

			return BuildOutput(_ways.Current);
		}

		private static string BuildOutput(List<int> way)
		{
			if (way == null)
			{
				return "Non-Euler graph; Way can't be founded";
			}
			else if (way.First() == way.Last())
			{
				return $"Euler graph; Way founded: {string.Join(", ", way)}";
			}
			else
			{
				return $"Semi-Euler graph; Way founded: {string.Join(", ", way)}";
			}
		}

		private static IEnumerable<List<int>> SubSearchForEuler(Edge[] edges, Vertex[] vertexes, List<Edge> counted, List<List<Edge>> counts)
		{
			var combinations = edges.Select(edge => new List<Edge>() { edge });

			for (var i = 3; i < edges.Length; i++)
			{
				var tmp = new List<List<Edge>>();

				foreach (var combo in combinations)
				{
					var way = GetEulerWay(edges, vertexes, combo);
					if (way != null)
					{
						yield return way;
					}

					foreach (var edge in edges)
					{
						if (combo.Last().Id <= edge.Id) continue;

						var newCombo = combo.ToList();
						newCombo.Add(edge);

						tmp.Add(newCombo);
					}
				}

				combinations = tmp;
			}
		}

		private static List<int> GetEulerWay(Edge[] edges, Vertex[] vertexes, List<Edge> counted)
		{
			var starts = vertexes.Where(vertex => vertex.Edges.Where(edge => !counted.Contains(edge)).Count() % 2 != 0);
			if (starts.Count() > 2)
			{
				return null;
			}

			foreach (var edge in counted)
			{
				edge.Color = Brushes.White;
			}

			var baseStart = starts.FirstOrDefault();

			if (baseStart != null)
			{
				return GetEulerWay(edges, baseStart, counted.ToList());
			}

			foreach (var start in vertexes)
			{
				var way = GetEulerWay(edges, start, counted.ToList());

				if (way != null) return way;
			}

			return null;
		}

		private static List<int> GetEulerWay(Edge[] edges, Vertex start, List<Edge> counted)
		{
			var oldCounted = counted.ToList();

			if (BuildWay(edges, start, counted))
			{
				var way = new List<int>() { start.Number };
				var current = start;

				foreach (var edge in counted.Except(oldCounted))
				{
					current = edge.Sink == current ? edge.Source : edge.Sink;
					way.Add(current.Number);
				}

				return way;
			}

			return null;
		}

		private static bool BuildWay(Edge[] edges, Vertex start, List<Edge> counted)
		{
			foreach (var edge in start.OutgoingEdges)
			{
				if (counted.Contains(edge)) continue;
				counted.Add(edge);

				var vertex = edge.Sink == start ? edge.Source : edge.Sink;

				if (BuildWay(edges, vertex, counted))
				{
					var filling = (double)counted.IndexOf(edge) / edges.Length;

					edge.Color = new SolidColorBrush(Color.FromRgb((byte)(0xFF * (1 - filling)), (byte)(0xFF * filling), 0x00));
					return true;
				}
				else
				{
					counted.Remove(edge);
				}
			}

			return counted.Count == edges.Length;
		}
	}
}
