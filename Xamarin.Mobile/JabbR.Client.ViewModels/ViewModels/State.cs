using System;
using System.Threading.Tasks;

namespace JabbR.Client.ViewModels
{
	[PropertyChanged.ImplementPropertyChanged]
	public class State 
	{
		public State ()
		{
			this.ClientMobile =  new JabbR.Client.Models.JabbRClientMobile();
			this.Logger = new Logger();

			this.Settings = new Settings();
			this.Server = new Server();
			this.Client = new Client(this.Server.Url);
			this.Client.Server = this.Server;
			this.Client.Connect();
			this.Login = new Login();

			return;
		}

		public static State StateData
		{
			get;
			set;
		}


		public JabbR.Client.Models.JabbRClientMobile ClientMobile
		{
			get;
			set;
		}

		public Server Server
		{
			get;
			set;
		}

		public Client Client
		{
			get;
			set;
		}

		public Settings Settings
		{
			get;
			set;
		}

		public Login Login
		{
			get;
			set;
		}

		public User User
		{
			get;
			set;
		}

		public Logger Logger
		{
			get;
			set;
		}

		public JabbR.Client.Models.Room Room
		{
			get;
			set;
		}

	}
}


