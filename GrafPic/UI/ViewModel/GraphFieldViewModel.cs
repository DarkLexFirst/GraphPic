using GraphPic.UI.Components;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GraphPic.UI.ViewModel
{
	public class GraphFieldViewModel : BaseViewModel
	{
		private VertexSelectionController _vertexSelectionController;
		private DraggingManager _draggingManager;
		private GraphControls _graphControls;
		private GraphData _graphData;

		private Point _lastMouseClick = new Point();
		private VertexView _clickedVertex;
		private RelayCommand _clearCommand;
		private RelayCommand _resetCommand;

		public GraphFieldViewModel(
			GraphFiledContextMenu graphFiledContextMenu,
			VertexSelectionController vertexSelectionController,
			DraggingManager draggingManager,
			GraphControls graphControls,
			GraphData graphData)
		{
			_vertexSelectionController = vertexSelectionController;
			_draggingManager = draggingManager;
			_graphControls = graphControls;
			_graphData = graphData;

			graphControls.ClearField += (_, _) => ClearAll();
			graphControls.VertexNumberVisibilityChanged += (_, e) => SetVertexNumbersVisibility(e.ShowNumbers);
			graphData.DataUpdated += (_, e) => LoadData(e.Vertexes, e.Edges);

			GraphFiledContextMenu = graphFiledContextMenu;
			GraphFiledContextMenu.CreateVertex += (_, _) => CreateVertex(_lastMouseClick);
			GraphFiledContextMenu.Clear += (_, _) => ClearAll();

			_draggingManager.Drag += Drag;
		}

		public GraphFiledContextMenu GraphFiledContextMenu { get; }


		public ObservableCollection<EdgeView> Edges { get; } = new();

		public ObservableCollection<VertexView> Vertexes { get; } = new();


		public void MouseClick(object sourse, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Right)
			{
				_lastMouseClick = e.GetPosition((IInputElement)sourse).SwithTop((UIElement)sourse);
			}
			else if (e.ChangedButton == MouseButton.Left)
			{
				_draggingManager.StartDrag((IInputElement) sourse);

				if (e.Source.GetType() == typeof(Grid) && e.OriginalSource.GetType() == typeof(Grid))
				{
					if (e.ClickCount == 2)
					{
						CreateVertex(e.GetPosition((IInputElement)sourse).SwithTop((UIElement)sourse));
					}

					UneditAllEdges();
				}
			}
		}

		public void MouseUp(object sourse, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				_draggingManager.StopDrag();
				_clickedVertex = null;
			}
		}

		public void KeyDown(object sourse, KeyEventArgs e)
		{
			if (_vertexSelectionController.Selected != null)
			{
				if (e.Key == Key.Escape) _vertexSelectionController.Unselect();
				else if (e.Key == Key.Delete) RemoveVertex(_vertexSelectionController.Selected);
			}
		}

		protected void Drag(object sourse, RoutedEventArgs e)
		{
			if (_clickedVertex != null)
			{
				MoveVertex(_clickedVertex, Mouse.GetPosition((IInputElement)sourse).SwithTop((UIElement)sourse));
			}
		}

		private void CreateVertex(Point position)
		{
			var vertexView = CreateVertexView(position, 0);

			Vertexes.Add(vertexView);
			UpdateVertexIndex();
			UpdateData();
		}

		private VertexView CreateVertexView(Point position, int number)
		{
			var vertexView = new VertexView() { Position = position, Number = number, ShowNumber = _graphControls.ShowVertexNumbers };
			vertexView.MouseDown += (_, e) => _clickedVertex = vertexView;
			vertexView.MouseUp += (_, e) =>
			{
				if (e.ChangedButton == MouseButton.Left &&
					_clickedVertex == vertexView &&
					!_draggingManager.Moving)
				{
					VertexClick(vertexView);
				}

				UneditAllEdges();
			};

			var vertexContexMenu = new VertexContextMenu();
			vertexContexMenu.Remove += (_, _) => RemoveVertex(vertexView);
			vertexContexMenu.Unlink += (_, _) => UnlinkVertex(vertexView);
			vertexContexMenu.ExecuteAlgorithm += (_, _) =>
			{
				var first = _graphData.Vertexes.FirstOrDefault(v => v.Id == _vertexSelectionController.Selected?.Id);
				var second = _graphData.Vertexes.FirstOrDefault(v => v.Id == vertexView.Id);

				if (first == null)
				{
					_graphControls.ExecuteAlgorithm(second);
				}
				else
				{
					_graphControls.ExecuteAlgorithm(first, second);
				}
			};

			vertexView.ContextMenu = vertexContexMenu;

			return vertexView;
		}

		private void UpdateVertexIndex()
		{
			int i = 0;

			foreach (var vertexView in Vertexes)
			{
				vertexView.Number = i++;
			}
		}

		private void RemoveVertex(VertexView vertexView)
		{
			if (_vertexSelectionController.Selected == vertexView)
			{
				_vertexSelectionController.Unselect();
			}

			UnlinkVertex(vertexView);
			Vertexes.Remove(vertexView);
			UpdateVertexIndex();
			UpdateData();
		}

		private void VertexClick(VertexView vertexView)
		{
			if (_vertexSelectionController.Selected == null)
			{
				_vertexSelectionController.Select(vertexView);
			}
			else
			{
				if (_vertexSelectionController.Selected != vertexView)
				{
					UpdateEdge(_vertexSelectionController.Selected, vertexView);
				}

				_vertexSelectionController.Unselect();
			}
		}

		private void UnlinkVertex(VertexView vertexView)
		{
			var exsistedEdges = Edges.Where(edge => edge.First == vertexView || edge.Second == vertexView).ToArray();

			foreach (var edge in exsistedEdges)
			{
				Edges.Remove(edge);
			}

			UpdateData();
		}

		private void MoveVertex(VertexView vertexView, Point position)
		{
			vertexView.Position = position;

			var exsistedEdges = Edges.Where(edge => edge.First == vertexView || edge.Second == vertexView).ToArray();

			foreach (var edge in exsistedEdges)
			{
				edge.UpdatePosition();
			}
		}

		private void UpdateEdge(VertexView first, VertexView second)
		{
			var exsistedEdge = Edges.FirstOrDefault(edge =>
				(edge.First == first && edge.Second == second) ||
				(edge.First == second && edge.Second == first));

			if (exsistedEdge != null)
			{
				RemoveEdge(exsistedEdge);
				return;
			}

			var edgeView = CreateEdgeView(first, second);

			Edges.Add(edgeView);
			UpdateData();
		}

		private EdgeView CreateEdgeView(VertexView first, VertexView second)
		{
			var edgeView = new EdgeView(first, second);
			edgeView.WeightChanged += (_, _) => UpdateData();
			edgeView.MouseDown += (_, e) =>
			{
				if (e.ClickCount == 2)
				{
					UneditAllEdges();
					edgeView.StartEditWeight();
				}
			};

			var edgeContexMenu = new EdgeContextMenu();
			edgeContexMenu.Remove += (_, _) => RemoveEdge(edgeView);
			edgeContexMenu.Edit += (_, _) => edgeView.StartEditWeight();
			edgeContexMenu.SwitchDirection += (_, _) => SwitchEdgeDirection(edgeView);
			edgeContexMenu.UseDirection += (s, _) => ToggleEdgeUseDirection(edgeView, ((MenuItem)s).IsChecked);

			edgeView.UseDirectionChanged += (_, e) => edgeContexMenu.UseDirectionChecked = e.UseDirection;

			edgeView.ContextMenu = edgeContexMenu;

			edgeView.UseDirection = _graphControls.DirectNewEdge;

			return edgeView;
		}

		private void SwitchEdgeDirection(EdgeView edgeView)
		{
			edgeView.SwithDirection();
			UpdateData();
		}

		private void ToggleEdgeUseDirection(EdgeView edgeView, bool value)
		{
			edgeView.UseDirection = value;
			UpdateData();
		}

		private void RemoveEdge(EdgeView edgeView)
		{
			Edges.Remove(edgeView);
			UpdateData();
		}

		public RelayCommand ClearCommand
		{
			get => _clearCommand ??= new RelayCommand(obj => ClearAll());
		}

		public RelayCommand ResetCommand
		{
			get => _resetCommand ??= new RelayCommand(obj => _graphData.UpdateFromOriginal());
		}

		private void ClearAll()
		{
			_vertexSelectionController.Unselect();
			Vertexes.Clear();
			Edges.Clear();
			_clickedVertex = null;

			UpdateData();
		}

		private void UneditAllEdges()
		{
			bool hasEdited = false;

			foreach (var edge in Edges)
			{
				hasEdited |= edge.Edited;
				edge.StopEditWeight();
			}

			if (hasEdited) UpdateData();
		}

		private void SetVertexNumbersVisibility(bool visible)
		{
			foreach (var vertexView in Vertexes)
			{
				vertexView.ShowNumber = visible;
			}
		}

		private void UpdateData()
		{
			Dictionary<int, (Vertex, Dictionary<Edge, bool>)> vertexes = new();
			List<Edge> allEdges = new List<Edge>();
			
			foreach (var vertexView in Vertexes)
			{
				var edges = new Dictionary<Edge, bool>();
				var position = new Vector2((float) vertexView.Position.X, (float)vertexView.Position.Y);

				vertexes.Add(vertexView.Id, (new Vertex(vertexView.Id, vertexView.Number, position, edges), edges));
			}

			foreach (var edgeView in Edges)
			{
				edgeView.EdgeColor = null;

				var first = vertexes[edgeView.First.Id];
				var second = vertexes[edgeView.Second.Id];

				var edge = new Edge(edgeView.Id, first.Item1, second.Item1, edgeView.Weight, edgeView.UseDirection);
				allEdges.Add(edge);

				edge.ColorChanged += (_, e) => edgeView.EdgeColor = e.Color;

				first.Item2.Add(edge, true);
				second.Item2.Add(edge, !edgeView.UseDirection);
			}

			_graphData.UpdateData(vertexes.Select(vertex => vertex.Value.Item1).ToArray(), allEdges.ToArray());
		}

		private void LoadData(Vertex[] vertexes, Edge[] edges)
		{
			ClearAll();

			Dictionary<int, VertexView> vertexViews = new();

			foreach (var vertex in vertexes)
			{
				var position = new Point(vertex.Position.X, vertex.Position.Y);
				var vertexView = CreateVertexView(position, vertex.Number);

				Vertexes.Add(vertexView);
				vertexViews.Add(vertexView.Number, vertexView);
			}

			foreach (var edge in edges)
			{
				var first = vertexViews[edge.Source.Number];
				var second = vertexViews[edge.Sink.Number];

				var edgeView = CreateEdgeView(first, second);
				edgeView.Weight = edge.Weight;
				edgeView.UseDirection = edge.Directed;
				Edges.Add(edgeView);
			}

			UpdateData();
		}
	}
}
