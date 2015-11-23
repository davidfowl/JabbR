using System;
using System.Threading.Tasks;

namespace JabbR.Client.ViewModels
{
	[PropertyChanged.ImplementPropertyChanged]
	public class Server 
	{
		public static Server Current
		{
			get;
			set;
		}

		public string Url
		{
			get;
			set;
		}

		public Client Client
		{
			get;
			set;
		}

		public JabbR.Client.JabbRClient ClientJabbRAPI
		{
			get;
			set;
		}

		static Server ()
		{
			return;
		}

		public Server ()
		{
			this.Url = @"http://jabbr.net";

			return;
		}

		public Client InitializeClient(string url)
		{
			Client client = new Client(url);

			this.Client = client;
			this.ClientJabbRAPI = client.ClientJabbRAPI;
			client.Server = this;
			Current = this;

			return client;
		}

	}
}


