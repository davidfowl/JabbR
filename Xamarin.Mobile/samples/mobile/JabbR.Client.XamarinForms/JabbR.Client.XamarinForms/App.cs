using System;

using Xamarin.Forms;

namespace JabbR.Client.XamarinForms
{
	public partial class App
	{	

		public static Page GetMainPage()
		{
			JabbR.Client.ViewModels.State.StateData = new JabbR.Client.ViewModels.State();

			JabbR.Client.ViewModels.State.StateData.Server.Url = @"https://jabbr.net";
			JabbR.Client.ViewModels.State.StateData.Login.Username = @"moljac";

			return new JabbR.Client.Views.Pages.JabbRClient();
		}


	}
}
