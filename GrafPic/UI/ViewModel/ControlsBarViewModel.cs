using GraphPic.Algorithms;
using GraphPic.Events;
using System;

namespace GraphPic.UI.ViewModel
{
	public class ControlsBarViewModel : BaseViewModel
	{
		private GraphControls _graphControls;

		private bool _directNewEdge;
		private string _output;
		private RelayCommand _calculateCommand;
		private RelayCommand _fileOpenCommand;
		private RelayCommand _fileSaveAsCommand;
		private RelayCommand _fileSaveCommand;
		private RelayCommand _clearOutputCommand;

		public ControlsBarViewModel(GraphControls graphControls)
		{
			_graphControls = graphControls;

			_graphControls.DirectNewEdgeChanded += (_, e) => 
				OnPropertyChanged(ref _directNewEdge, e.UseDirection, nameof(DirectNewEdge));
			_graphControls.ExecutionLogged += (_, e) => Output = e.Message + "\n" + Output;
		}

		public bool DirectNewEdge
		{
			get => _directNewEdge;
			set
			{
				_graphControls.DirectNewEdge = value;
				OnPropertyChanged(ref _directNewEdge, value);
			}
		}

		public bool ShowVertexNumbers
		{
			get => _graphControls.ShowVertexNumbers;
			set
			{
				_graphControls.ShowVertexNumbers = value;
				OnPropertyChanged();
			}
		}

		public string Output
		{
			get => _output;
			set => OnPropertyChanged(ref _output, value);
		}

		public string[] Algorithms => AlgorithmsManager.Algorithms;

		public string SelectedAlgorithm
		{
			get => _graphControls.SelectedAlgorithm;
			set
			{
				_graphControls.SelectedAlgorithm = value;
				OnPropertyChanged();
			}
		}

		private void ExecuteCalculate()
		{
			_graphControls.ExecuteAlgorithm();
		}

		public RelayCommand CalculateCommand
		{
			get => _calculateCommand ??= new RelayCommand(obj => ExecuteCalculate());
		}

		public RelayCommand FileOpenCommand
		{
			get => _fileOpenCommand ??= new RelayCommand(obj => _graphControls.OpenFromFile());
		}

		public RelayCommand FileSaveAsCommand
		{
			get => _fileSaveAsCommand ??= new RelayCommand(obj => _graphControls.SaveToFile());
		}

		public RelayCommand FileSaveCommand
		{
			get => _fileSaveCommand ??= new RelayCommand(obj => _graphControls.SaveCurrent());
		}

		public RelayCommand ClearOutputCommand
		{
			get => _clearOutputCommand ??= new RelayCommand(obj => Output = string.Empty);
		}
	}
}
