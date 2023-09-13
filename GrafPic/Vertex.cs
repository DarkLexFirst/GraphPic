using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GraphPic
{
	public class Vertex
	{
		[JsonIgnore]
		public int Id { get; }

		public int Number { get; }
		public Vector2 Position { get; }

		private Dictionary<Edge, bool> _edges;

		[JsonConstructor]
		private Vertex(int number, Vector2 position) :
			this(0, number, position)
		{

		}

		public Vertex(int id, int number, Vector2 position) :
			this(id, number, position, new Dictionary<Edge, bool>())
		{

		}

		public Vertex(int id, int number, Vector2 position, Dictionary<Edge, bool> edges)
		{
			Id = id;
			Number = number;
			Position = position;
			_edges = edges;
		}

		public void AddEdge(Edge edge, bool outgoing)
		{
			_edges.Add(edge, outgoing);
		}

		[JsonIgnore]
		public Edge[] Edges => _edges.Keys.ToArray();

		[JsonIgnore]
		public Edge[] OutgoingEdges => _edges.Where(edge => edge.Value).Select(edge => edge.Key).ToArray();

		[JsonIgnore]
		public Edge[] IncomingEdges => _edges.Where(edge => !edge.Value).Select(edge => edge.Key).ToArray();
	}
}
