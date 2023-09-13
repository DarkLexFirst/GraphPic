using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace GraphPic.UI.Components
{
	public class GridList : Grid
	{
		public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof(Items), typeof(IEnumerable), typeof(GridList), new PropertyMetadata(OnChanged));

		private List<UIElement> _cache = new();

		public IEnumerable Items
		{
			get => (IEnumerable)GetValue(ItemsProperty);
			set => SetValue(ItemsProperty, value);
		}

		static void OnChanged(DependencyObject sourse, DependencyPropertyChangedEventArgs e)
		{
			var grid = (GridList)sourse;

			if (grid.Items is INotifyCollectionChanged collection)
			{
				collection.CollectionChanged += grid.OnChanged;
			}
		}

		private void OnChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			foreach (UIElement item in _cache)
			{
				Children.Remove(item);
			}

			foreach (UIElement item in Items)
			{
				Children.Add(item);
				_cache.Add(item);
			}
		}
	}
}
