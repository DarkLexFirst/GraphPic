using System.Windows;
using System.Windows.Controls;

namespace GraphPic.UI.Components
{
	public abstract class CustomContextMenu : ContextMenu
	{
		protected MenuItem AddNewMenuItem(string header, RoutedEventHandler click)
		{
			var item = new MenuItem() { Header = header };
			item.Click += click;
			Items.Add(item);

			return item;
		}
	}
}
