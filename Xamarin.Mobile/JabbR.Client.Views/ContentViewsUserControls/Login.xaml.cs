using System;
using System.Collections.Generic;
using Xamarin.Forms;

using JabbR.Client.ViewModels;

namespace JabbR.Client.Views.ContentViewsUserControls
{
	public partial class Login : ContentView
	{
		public Login ()
		{
			InitializeComponent ();

			XLabs.Forms.Controls.BindablePicker picker = null;
			picker = new XLabs.Forms.Controls.BindablePicker ();
			picker.ItemsSource = State.StateData.Login.RoomsSubscribed;
			picker.SelectedIndexChanged += Picker_SelectedIndexChanged; 

			//grid.Children.Add (picker, 0, 0);

			return;
		}

		void Picker_SelectedIndexChanged (object sender, EventArgs e)
		{
			XLabs.Forms.Controls.BindablePicker picker = null;

			picker = sender as XLabs.Forms.Controls.BindablePicker;
			string room = picker.SelectedIndex.ToString();

			return;
		}


		public async void buttonLogin_Clicked(object sender, EventArgs ea)
		{
			string username = textBoxUsername.Text;
			string password = textBoxPassword.Text;

			JabbR.Client.ViewModels.Login l = 
					await State.StateData.Client.LoginAsync(username, password);

			State.StateData.Client.SubscribeJabbREvents();

			return;
		}
	}
}

