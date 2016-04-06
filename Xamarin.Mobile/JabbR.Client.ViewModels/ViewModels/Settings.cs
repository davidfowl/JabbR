using System;


namespace JabbR.Client.ViewModels
{
	[PropertyChanged.ImplementPropertyChanged]
	public class Settings
	{
		JabbR.Client.Models.Settings settings;
		public Settings()
		{
			settings = new JabbR.Client.Models.Settings();
			this.Server = settings.Server;
		}

		private string server = null;

		public string Server
		{
			get;
			set;
		}

	}
}


