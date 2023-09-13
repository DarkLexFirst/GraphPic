using GraphPic.UI.Components;

namespace GraphPic.UI
{
	public class VertexSelectionController
	{
		public VertexView Selected { get; set; }

		public void Select(VertexView vertexView)
		{
			if (Selected != null) Selected.Selected = false;

			Selected = vertexView;
			Selected.Selected = true;
		}

		public void Unselect()
		{
			if (Selected != null)
			{
				Selected.Selected = false;
				Selected = null;
			}
		}
	}
}
