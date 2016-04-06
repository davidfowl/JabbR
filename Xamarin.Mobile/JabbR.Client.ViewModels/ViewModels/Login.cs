using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace JabbR.Client.ViewModels
{
	[PropertyChanged.ImplementPropertyChanged]
	public class Login 
	{
		public Login ()
		{
		}

		public bool IsLoggedIn
		{
			get;
			set;
		}

		public string Username
		{
			get;
			set;
		}

		public string Password
		{
			get;
			set;
		}

		public JabbR.Client.Models.LogOnInfo LogOnInfo
		{
			get;
			set;
		}

		public ObservableCollection<JabbR.Client.Models.Room> RoomsSubscribed
		{
			get;
			set;
		}

		public ObservableCollection<JabbR.Client.Models.Room> RoomsAvailable
		{
			get;
			set;
		}

		public async Task< ObservableCollection<JabbR.Client.Models.Room>> GetRoomsSubscribed()
		{
			ObservableCollection<JabbR.Client.Models.Room> rooms = null;

			Task.Run
			(
				()
				=>
				{
					rooms = new ObservableCollection<JabbR.Client.Models.Room>(this.LogOnInfo.Rooms);
				}
			);

			this.RoomsSubscribed = rooms;

			return rooms;
		}

		public async Task< ObservableCollection<JabbR.Client.Models.Room>> GetRoomsAvailable ()
		{
			ObservableCollection<JabbR.Client.Models.Room> rooms = null;

			Task.Run
			(
				async ()
				=>
				{
					IEnumerable<JabbR.Client.Models.Room> rooms_get = null;
					rooms_get = await State.StateData.Client.ClientJabbRAPI.GetRooms();
					rooms = new ObservableCollection<JabbR.Client.Models.Room>(rooms_get);
				}
			);

			this.RoomsAvailable = rooms;

			return rooms;
		}

		public void ToLog (string msg)
		{
			System.Diagnostics.Debug.WriteLine (DateTime.Now + ": " + msg);

			LogItem li =  new LogItem()
			{
				TimeStamp = DateTime.Now,
				Event = msg,
			};

			State.StateData.Logger.Log.Add(li);

			return;
		}
	}
}


