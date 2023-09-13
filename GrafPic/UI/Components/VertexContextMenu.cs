using System.Windows;

namespace GraphPic.UI.Components
{
	public class VertexContextMenu : CustomContextMenu
	{
		public VertexContextMenu()
		{
			AddNewMenuItem("Remove", (s, e) => Remove?.Invoke(s, e));
			AddNewMenuItem("Unlink", (s, e) => Unlink?.Invoke(s, e));
			AddNewMenuItem("Execute algorithm", (s, e) => ExecuteAlgorithm?.Invoke(s, e));
		}

		public event RoutedEventHandler Remove;
		public event RoutedEventHandler Unlink;
		public event RoutedEventHandler ExecuteAlgorithm;
	}
}
