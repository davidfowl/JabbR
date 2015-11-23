using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace JabbR.Client.ViewModels
{
	[PropertyChanged.ImplementPropertyChanged]
	public class Logger 
	{
		public Logger ()
		{
			this.Log = new ObservableCollection<LogItem>();

			return;
		}


		public ObservableCollection<LogItem> Log
		{
			get;
			set;
		}
	}
}


