using GraphPic.UI.Components;
using GraphPic.UI.View;
using Unity;
using Unity.Extension;

namespace GraphPic.UI
{
	public class ContainerExtension : UnityContainerExtension
	{
		protected override void Initialize()
		{
			RegisterViews();
			RegisterViewModels();
			RegisterComponents();
		}

		private void RegisterViews()
		{
			Container.RegisterType<MainWindow>();
		}

		private void RegisterViewModels()
		{

		}

		private void RegisterComponents()
		{
			Container.RegisterType<GraphFiledContextMenu>();
			Container.RegisterType<VertexSelectionController>();
		}
	}
}
