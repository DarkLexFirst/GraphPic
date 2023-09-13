namespace GraphPic.UI.ViewModel
{
	public class MainWindowViewModel : BaseViewModel
	{
		public MainWindowViewModel(
			GraphFieldViewModel graphFieldViewModel,
			ControlsBarViewModel controlsBarViewModel)
		{
			GraphFieldViewModel = graphFieldViewModel;
			ControlsBarViewModel = controlsBarViewModel;
		}

		public GraphFieldViewModel GraphFieldViewModel { get; }
		public ControlsBarViewModel ControlsBarViewModel { get; }

		public RelayCommand ExitCommand { get => new RelayCommand(obj => App.Current.Shutdown()); }
	}
}
