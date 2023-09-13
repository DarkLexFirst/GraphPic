using System.Linq;
using System.Windows;
using Unity;

namespace GraphPic
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public IUnityContainer Container { get; set; }

		protected override void OnStartup(StartupEventArgs e)
		{
			SetupContainer();

			InitializeMainWindow();

			LoadFile(e.Args.FirstOrDefault());
		}

		private void LoadFile(string path)
		{
			if (string.IsNullOrEmpty(path)) return;

			var graphControl = Container.Resolve<GraphControls>();
			graphControl.OpenFromFile(path);
		}

		private void SetupContainer()
		{
			Container = new UnityContainer();

			Container.RegisterInstance(this);

			Container.RegisterSingleton<GraphData>();
			Container.RegisterSingleton<GraphControls>();

			Container.AddNewExtension<UI.ContainerExtension>();
		}

		private void InitializeMainWindow()
		{
			Container.Resolve<UI.View.MainWindow>().Show();
		}
	}
}
