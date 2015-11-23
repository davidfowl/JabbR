using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace JabbR.Client.Views.ContentViewsUserControls
{
	public partial class Rooms : ContentView
	{
		public Rooms ()
		{
			InitializeComponent ();
		}

		protected void ListViewRooms_OnItemSelected (object sender, SelectedItemChangedEventArgs e)
		{
			return;	
		}

		protected void ListViewRooms_ItemTapped (object sender, ItemTappedEventArgs ea)
		{
			JabbR.Client.Models.Room r = ea.Item as JabbR.Client.Models.Room;

			if (r == null)
			{
				return;
			}

			JabbR.Client.ViewModels.State.StateData.Room = r;

			((ListView)sender).SelectedItem = null; 

			/*
			JabbR.Client.Views.Pages.Room page = null;
			page = new JabbR.Client.Views.Pages.Room ();
			Navigation.PushAsync(new NavigationPage(page));
			*/

			return;
		}
	}
}

