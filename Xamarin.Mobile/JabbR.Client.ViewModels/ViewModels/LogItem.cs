using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace JabbR.Client.ViewModels
{
	[PropertyChanged.ImplementPropertyChanged]
	public partial class LogItem
	{
		public DateTime TimeStamp
		{
			get;
			set;
		}

		public string Event
		{
			get;
			set;
		}

	}

}


