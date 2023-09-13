using System.Windows;

namespace GraphPic.UI.Components
{
	public class GraphFiledContextMenu : CustomContextMenu
	{
		public GraphFiledContextMenu()
		{
			AddNewMenuItem("Add", (s, e) => CreateVertex?.Invoke(s, e));
			AddNewMenuItem("Clear", (s, e) => Clear?.Invoke(s, e));
		}

		public event RoutedEventHandler CreateVertex;
		public event RoutedEventHandler Clear;
	}
}
