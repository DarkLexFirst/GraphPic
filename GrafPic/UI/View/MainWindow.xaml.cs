using GraphPic.UI.ViewModel;
using System.Windows;

namespace GraphPic.UI.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow(
			MainWindowViewModel viewModel)
		{
			InitializeComponent();

			DataContext = viewModel;
		}
	}
}
