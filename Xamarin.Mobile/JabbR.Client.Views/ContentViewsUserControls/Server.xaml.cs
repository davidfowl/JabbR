using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace JabbR.Client.Views.ContentViewsUserControls
{
	public partial class Server : ContentView
	{
		public Server ()
		{
			InitializeComponent ();

			return;
		}

		public async void buttonConnect_Clicked(object sender, EventArgs ea)
		{
			string server = textBoxJabbRServer.Text;

			JabbR.Client.ViewModels.State.StateData.Client.Connect(server);
			JabbR.Client.ViewModels.State.StateData.Client.SubscribeJabbREvents();

			return;
		}

	}
}

