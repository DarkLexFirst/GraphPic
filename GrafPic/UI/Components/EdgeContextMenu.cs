using System.Windows;
using System.Windows.Controls;

namespace GraphPic.UI.Components
{
	public class EdgeContextMenu : CustomContextMenu
	{
		private MenuItem _useDirectionItem;

		public EdgeContextMenu()
		{
			AddNewMenuItem("Remove", (s, e) => Remove?.Invoke(s, e));
			AddNewMenuItem("Edit", (s, e) => Edit?.Invoke(s, e));
			AddNewMenuItem("Switch direction", (s, e) => SwitchDirection?.Invoke(s, e));

			_useDirectionItem = AddNewMenuItem("Use direction", (s, e) => UseDirection?.Invoke(s, e));
			_useDirectionItem.IsCheckable = true;
		}

		public bool UseDirectionChecked
		{
			get => _useDirectionItem.IsChecked;
			set => _useDirectionItem.IsChecked = value;
		}

		public event RoutedEventHandler Remove;
		public event RoutedEventHandler Edit;
		public event RoutedEventHandler UseDirection;
		public event RoutedEventHandler SwitchDirection;
	}
}
