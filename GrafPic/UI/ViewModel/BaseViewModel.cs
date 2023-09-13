using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GraphPic.UI.ViewModel
{
	public abstract class BaseViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}

		public void OnPropertyChanged<T>(ref T obj, T value, [CallerMemberName] string prop = "")
		{
			obj = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
	}
}
