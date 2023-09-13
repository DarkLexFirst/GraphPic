using GraphPic.Events;
using Newtonsoft.Json;
using System;
using System.IO;

namespace GraphPic
{
	public class GraphData
	{
		public Vertex[] Vertexes { get; private set; } = new Vertex[0];
		public Edge[] Edges { get; private set; } = new Edge[0];

		private string _original;

		public void UpdateData(Vertex[] vertexes, Edge[] edges)
		{
			Vertexes = vertexes;
			Edges = edges;
		}

		public void ResetAllColors()
		{
			foreach (var edge in Edges)
			{
				edge.ResetColor();
			}
		}

		public bool OpenFromFile(string path)
		{
			if (!File.Exists(path)) return false;

			_original = File.ReadAllText(path);
			UpdateFromOriginal();
			return true;
		}

		public void SaveToFile(string path)
		{
			var json = JsonConvert.SerializeObject(this);

			using (var stream = File.CreateText(path))
			{
				stream.Write(json);
			}
		}

		public void Update(Vertex[] vertexes, Edge[] edges)
		{
			Vertexes = vertexes;
			Edges = edges;

			Update();
		}

		public void Update()
		{
			Update(new GraphDataUpdateEventArgs()
			{
				Vertexes = Vertexes,
				Edges = Edges
			});
		}

		public void UpdateFromOriginal()
		{
			if (_original == null) return;

			Update(JsonConvert.DeserializeObject<GraphDataUpdateEventArgs>(_original));
		}

		private void Update(GraphDataUpdateEventArgs args)
		{
			DataUpdated?.Invoke(this, args);
		}

		public event EventHandler<GraphDataUpdateEventArgs> DataUpdated;
	}
}
