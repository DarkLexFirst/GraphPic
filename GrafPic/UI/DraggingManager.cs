using System;
using System.Windows;
using System.Windows.Input;

namespace GraphPic.UI
{
	public class DraggingManager
	{
		private readonly double DragStartDistance = 7;
		private readonly double NoDragDistance = 30;

		private bool _dragging;
		private bool _moving;
		private DateTime _draggingStartTime;
		private Point _mouseStartPosition;

		public bool Dragging => _dragging;
		public bool Moving => _moving;

		public IInputElement Target { get; private set; }

		public void StartDrag(IInputElement target)
		{
			_dragging = true;
			_draggingStartTime = DateTime.Now;
			_mouseStartPosition = Mouse.GetPosition(target).SwithTop((UIElement)target);
			Target = target;
			Mouse.AddPreviewMouseMoveHandler((DependencyObject)target, MouseMove);
		}

		public void StopDrag()
		{
			_dragging = false;
			_moving = false;
			_draggingStartTime = default;
			_mouseStartPosition = default;

			if (Target == null) return;

			Mouse.AddPreviewMouseMoveHandler((DependencyObject)Target, MouseMove);
			Target = null;
		}

		public TimeSpan DraggingTime { get => _dragging ? DateTime.Now - _draggingStartTime : default; }

		public event MouseEventHandler Drag;

		public void MouseMove(object sourse, MouseEventArgs e)
		{
			if (_dragging)
			{
				var current = e.GetPosition(Target).SwithTop((UIElement)Target);
				var distance = (_mouseStartPosition - current).Length;

				if ((_moving && distance > 1) || distance > DragStartDistance && distance < NoDragDistance)
				{
					_moving = true;
					Drag?.Invoke(sourse, e);
					_mouseStartPosition = current;
				}
			}
		}
	}
}
