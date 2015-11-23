using System;

namespace JabbR.Client.Models
{
	public partial class Settings
	{
		public Settings ()
		{
			this.Server = "";
		}

		private string server = null;

		public string Server
		{
			get;
			set;
		}

	}
}

