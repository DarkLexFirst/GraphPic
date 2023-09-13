using GraphPic.Algorithms;
using GraphPic.Events;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace GraphPic
{
	public class GraphControls
	{
		private static readonly string OpenFileDialogTitle = "Reading graph from file";
		private static readonly string SaveFileDialogTitle = "Saving graph to file";
		private static readonly string GraphFileFilter = "graph (*.graph)|*.graph";

		private static readonly string AlertTitle = "Are you sure?";
		private static readonly string OpenGraphAlertMessage = "If you open a graph from a file, the current graph will be lost.";

		private GraphData _graphData;

		private bool _directNewEdge;
		private bool _showVertexNumbers = true;
		private string _openedFile;

		public GraphControls(GraphData graphData)
		{
			_graphData = graphData;
		}

		public bool DirectNewEdge
		{
			get => _directNewEdge;
			set
			{
				_directNewEdge = value;
				DirectNewEdgeChanded?.Invoke(this, new UseDirectionEventArgs() { UseDirection = value });
			}
		}
		public bool ShowVertexNumbers
		{
			get => _showVertexNumbers;
			set
			{
				_showVertexNumbers = value;
				VertexNumberVisibilityChanged?.Invoke(this, new VertexNumbersVisibilityChangeArgs() { ShowNumbers = value });
			}
		}

		public string SelectedAlgorithm { get; set; } = AlgorithmsManager.Algorithms.First();

		public void ExecuteAlgorithm(Vertex first = null, Vertex second = null)
		{
			_graphData.ResetAllColors();
			var result = AlgorithmsManager.Execute(SelectedAlgorithm, _graphData, first, second);
			LogExecution(first == null 
				? $"[{DateTime.Now}]{SelectedAlgorithm} -> {result};"
				: second == null 
				? $"[{DateTime.Now}]{SelectedAlgorithm} from {first.Number} -> {result};"
				: $"[{DateTime.Now}]{SelectedAlgorithm} from {first.Number} to {second.Number} -> {result};");
		}

		public void LogExecution(string message)
		{
			ExecutionLogged?.Invoke(this, new LogExecutionEventArgs() { Message = message });
		}

		public event EventHandler<UseDirectionEventArgs> DirectNewEdgeChanded;
		public event EventHandler<VertexNumbersVisibilityChangeArgs> VertexNumberVisibilityChanged;
		public event EventHandler<LogExecutionEventArgs> ExecutionLogged;
		public event EventHandler ClearField;

		public void OpenFromFile()
		{
			if (_graphData.Vertexes.Length > 0 || _graphData.Edges.Length > 0)
			{
				if (!ShowAlert(OpenGraphAlertMessage)) return;
			}

			OpenFileDialog openFileDialog = new OpenFileDialog()
			{
				Title = OpenFileDialogTitle,
				Filter = GraphFileFilter
			};

			if(openFileDialog.ShowDialog() ?? false)
			{
				OpenFromFile(openFileDialog.FileName);
			}
		}

		public void OpenFromFile(string path)
		{
			Clear();
			_openedFile = path;
			_graphData.OpenFromFile(path);
		}

		public void SaveToFile()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog()
			{
				Title = SaveFileDialogTitle,
				Filter = GraphFileFilter
			};

			if (!string.IsNullOrEmpty(_openedFile))
			{
				saveFileDialog.FileName = Path.GetFileName(_openedFile);
			}

			if (saveFileDialog.ShowDialog() ?? false)
			{
				_openedFile = saveFileDialog.FileName;
				_graphData.SaveToFile(saveFileDialog.FileName);
			}
		}

		public void SaveCurrent()
		{
			if (string.IsNullOrEmpty(_openedFile))
			{
				SaveToFile();
			}
			else
			{
				_graphData.SaveToFile(_openedFile);
			}
		}

		public bool ShowAlert(string message)
		{
			MessageBoxResult result = MessageBox.Show(
				message,
				AlertTitle,
				MessageBoxButton.OKCancel, 
				MessageBoxImage.Warning);

			return result == MessageBoxResult.OK;
		}

		public void Clear()
		{
			ClearField?.Invoke(this, new EventArgs());
		}
	}
}
