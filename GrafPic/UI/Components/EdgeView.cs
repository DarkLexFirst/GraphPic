using GraphPic.Events;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphPic.UI.Components
{
	public class EdgeView : Grid
	{
		public static readonly Brush DefaultColor = new SolidColorBrush(Color.FromRgb(0x00, 0xBB, 0xCC));

		private static readonly int LineSize = 6;
		private static readonly int AddedSize = LineSize + 20;

		private static int _idCounter = 0;

		public VertexView First { get; private set; }
		public VertexView Second { get; private set; }

		private Line _line;
		private Line _lineShadow;
		private Line _lineBoundingBox;
		private TextBox _weightTextBox;
		private Polyline _arrow;
		private Polyline _arrowShadow;

		private float? _weight;
		private bool _useDirection;
		private Brush _color;

		public EdgeView(VertexView first, VertexView second)
		{
			First = first;
			Second = second;

			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Bottom;

			_lineShadow = new Line();
			//Children.Add(_lineShadow = new Line());
			Children.Add(_line = new Line());
			Children.Add(_lineBoundingBox = new Line());
			Children.Add(_arrowShadow = new Polyline());
			Children.Add(_arrow = new Polyline());
			Children.Add(_weightTextBox = new TextBox());

			_lineShadow.StrokeStartLineCap = _line.StrokeStartLineCap = PenLineCap.Triangle;
			_lineShadow.StrokeEndLineCap = _line.StrokeEndLineCap = PenLineCap.Triangle;
			_line.StrokeThickness = LineSize;
			_lineShadow.Stroke = Brushes.Black;
			_lineShadow.StrokeThickness = LineSize + 1;
			_lineBoundingBox.Stroke = Brushes.Transparent;
			_lineBoundingBox.StrokeThickness = LineSize * 3;

			_arrowShadow.StrokeStartLineCap = _arrow.StrokeStartLineCap = PenLineCap.Flat;
			_arrowShadow.StrokeEndLineCap = _arrow.StrokeEndLineCap = PenLineCap.Flat;
			_arrow.StrokeThickness = LineSize / 3;
			_arrowShadow.Stroke = Brushes.Black;
			_arrowShadow.StrokeThickness = LineSize / 3 + 1;
			_arrowShadow.Points = _arrow.Points;

			EdgeColor = null;

			_weightTextBox.HorizontalAlignment = HorizontalAlignment.Center;
			_weightTextBox.VerticalAlignment = VerticalAlignment.Center;
			_weightTextBox.Margin = new Thickness(0, -3.8, 0, 0);
			_weightTextBox.FontSize = 20;
			_weightTextBox.FontWeight = FontWeights.Bold;
			_weightTextBox.KeyDown += (_, e) =>
			{
				if (e.Key == Key.Enter || e.Key == Key.Escape)
				{
					StopEditWeight();
				}
			};

			StopEditWeight();
			UpdatePosition();
		}

		public int Id { get; } = _idCounter++;

		protected override void OnMouseDown(MouseButtonEventArgs e) => ProcessRoutedEvent(e);

		private void ProcessRoutedEvent(RoutedEventArgs e)
		{
			if (e.Source is VertexView && e.OriginalSource is not VertexView)
			{
				e.Handled = true;
			}
		}

		public float? Weight
		{
			get => _weight;
			set
			{
				_weight = value;
				_weightTextBox.Text = value?.ToString() ?? string.Empty;
				WeightChanged?.Invoke(this, new WeightChangedEventArgs() { Weight = value });
			}
		}

		public bool UseDirection
		{
			get => _useDirection;
			set
			{
				_useDirection = value;
				_arrowShadow.Visibility = _arrow.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
				if (value) UpdateArrow();

				UseDirectionChanged?.Invoke(this, new UseDirectionEventArgs() { UseDirection = value });
			}
		}

		public Brush EdgeColor
		{
			get => _color;
			set
			{
				if (value == null) value = DefaultColor;

				_color = value;
				_line.Stroke = value;
				_arrow.Stroke = value;
			}
		}

		public event EventHandler<UseDirectionEventArgs> UseDirectionChanged;
		public event EventHandler<WeightChangedEventArgs> WeightChanged;

		public void StartEditWeight()
		{
			_weightTextBox.IsReadOnly = false;
			_weightTextBox.Background = Brushes.White;
			_weightTextBox.BorderBrush = Brushes.Black;
			_weightTextBox.Focusable = true;
			_weightTextBox.BorderThickness = new Thickness(1);
			_weightTextBox.SelectAll();
			_weightTextBox.Focus();
		}

		public void StopEditWeight()
		{
			if (_weightTextBox.IsReadOnly) return;

			_weightTextBox.IsReadOnly = true;
			_weightTextBox.Focusable = false;
			_weightTextBox.IsEnabled = false;
			_weightTextBox.IsEnabled = true;
			_weightTextBox.Background = Brushes.Transparent;
			_weightTextBox.BorderBrush = Brushes.Transparent;
			_weightTextBox.BorderThickness = new Thickness(0);
			_weightTextBox.SelectionLength = 0;

			if (string.IsNullOrWhiteSpace(_weightTextBox.Text))
			{
				Weight = null;
			}
			else if (float.TryParse(_weightTextBox.Text, out float weight))
			{
				Weight = weight;
			}
			else
			{
				Weight = _weight;
			}
		}

		public bool Edited => !_weightTextBox.IsReadOnly;

		public void SwithDirection()
		{
			var tmp = First;
			First = Second;
			Second = tmp;

			UpdatePosition();
		}

		public void UpdatePosition()
		{
			var x = Math.Min(First.Position.X, Second.Position.X);
			var y = Math.Min(First.Position.Y, Second.Position.Y);

			Margin = new Thickness(x - AddedSize, 0, 0, y - AddedSize);

			var diff = Second.Position - First.Position;

			Width = Math.Abs(diff.X) + AddedSize * 2;
			Height = Math.Abs(diff.Y) + AddedSize * 2;

			var vector = new Vector(x - AddedSize, y - AddedSize);

			var first = First.Position - vector;
			var second = Second.Position - vector;

			_lineBoundingBox.X1 = _lineShadow.X1 = _line.X1 = first.X;
			_lineBoundingBox.Y1 = _lineShadow.Y1 = _line.Y1 = second.Y;
			_lineBoundingBox.X2 = _lineShadow.X2 = _line.X2 = second.X;
			_lineBoundingBox.Y2 = _lineShadow.Y2 = _line.Y2 = first.Y; 

			UpdateArrow();
		}

		private void UpdateArrow()
		{
			if (!_useDirection) return;

			double offset = 7;
			double widthCoeff = 1;
			double lengthCoeff = 4;

			var x = Math.Min(First.Position.X, Second.Position.X);
			var y = Math.Min(First.Position.Y, Second.Position.Y);

			var vector = new Vector(x - AddedSize, y - AddedSize);

			var fixedPosition = new Point(Second.Position.X, First.Position.Y);

			var diff = Second.Position - First.Position;

			Trace.TraceInformation(diff.Y.ToString());
			var ortoVector = Math.Abs(diff.Y) <= 0.1 ? new Vector(0, -1) : new Vector(1,  diff.X / diff.Y);
			ortoVector.Normalize();

			var normalizedDiff = diff;
			normalizedDiff.Normalize();
			normalizedDiff.Y = -normalizedDiff.Y;

			ortoVector *= LineSize * widthCoeff;

			var lowArrowPoint = fixedPosition - normalizedDiff * (offset + LineSize * lengthCoeff);

			_arrow.Points.Clear();
			Trace.TraceInformation((lowArrowPoint + ortoVector - vector).ToString());
			Trace.TraceInformation((fixedPosition - vector).ToString());
			Trace.TraceInformation((lowArrowPoint - ortoVector - vector).ToString());
			_arrow.Points.Add(lowArrowPoint + ortoVector - vector);
			_arrow.Points.Add(fixedPosition - vector - normalizedDiff * offset);
			_arrow.Points.Add(lowArrowPoint - ortoVector - vector);
		}

		protected override void OnRender(DrawingContext dc)
		{
			if (ContextMenu != null)
			{
				_line.ContextMenu = ContextMenu;
				_lineShadow.ContextMenu = ContextMenu;
				_lineBoundingBox.ContextMenu = ContextMenu;
				_arrow.ContextMenu = ContextMenu;
				_arrowShadow.ContextMenu = ContextMenu;
				_weightTextBox.ContextMenu = ContextMenu;
				ContextMenu = null;
			}

			base.OnRender(dc);
		}
	}
}
