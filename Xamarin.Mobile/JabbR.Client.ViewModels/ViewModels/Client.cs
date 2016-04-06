using System;
using System.Threading.Tasks;
using System.Text;
using System.Collections.ObjectModel;

namespace JabbR.Client.ViewModels
{
	[PropertyChanged.ImplementPropertyChanged]
	public class Client 
	{
		public static Client Current
		{
			get;
			set;
		}

		public Server Server
		{
			get;
			set;
		}

		public JabbR.Client.JabbRClient ClientJabbRAPI
		{
			get;
			set;
		}

		public JabbR.Client.Models.LogOnInfo LogOnInfo
		{
			get;
			set;
		}

		static Client ()
		{
		}

		public Client (string server)
		{
            //RunClientAPITestsAsync(server, roomName, username, password, ClientJabbRAPI, wh);
			Current = this;

			return;
		}

		public void Connect ()
		{
			this.Connect(this.Server.Url);

			return;
		}

		public void Connect (string url_server_jabbr)
		{
			JabbR.Client.JabbRClient client_jabbr_api = new JabbR.Client.JabbRClient(url_server_jabbr);

			this.ClientJabbRAPI = client_jabbr_api;

            // Uncomment to see tracing
            // not available in Xamarin profiles and portable
			// JabbRClientAPI.TraceWriter = Debug.Out;

			// Uncomment to see tracing
            // not available in Xamarin profiles and portable
			//this.ClientJabbRAPI.TraceWriter = System.Diagnostics.Debug.Out;

			return;
		}

		public void SubscribeJabbREvents ()
		{
			this.ClientJabbRAPI.MessageReceived += ClientJabbRAPI_MessageReceived;
			this.ClientJabbRAPI.UserJoined += ClientJabbRAPI_UserJoined;
			this.ClientJabbRAPI.UserLeft += ClientJabbRAPI_UserLeft;
			this.ClientJabbRAPI.PrivateMessage += ClientJabbRAPI_PrivateMessage;
			this.ClientJabbRAPI.AddMessageContent += ClientJabbRAPI_AddMessageContent;
			this.ClientJabbRAPI.FlagChanged += ClientJabbRAPI_FlagChanged;
			this.ClientJabbRAPI.GravatarChanged += ClientJabbRAPI_GravatarChanged;
			this.ClientJabbRAPI.JoinedRoom += ClientJabbRAPI_JoinedRoom;
			this.ClientJabbRAPI.Kicked += ClientJabbRAPI_Kicked;
			this.ClientJabbRAPI.LoggedOut += ClientJabbRAPI_LoggedOut;
			this.ClientJabbRAPI.MeMessageReceived += ClientJabbRAPI_MeMessageReceived;
			this.ClientJabbRAPI.NoteChanged += ClientJabbRAPI_NoteChanged;
			this.ClientJabbRAPI.OwnerAdded += ClientJabbRAPI_OwnerAdded;
			this.ClientJabbRAPI.OwnerRemoved += ClientJabbRAPI_OwnerRemoved;
			this.ClientJabbRAPI.RoomChanged += ClientJabbRAPI_RoomChanged;
			this.ClientJabbRAPI.TopicChanged += ClientJabbRAPI_TopicChanged;
			this.ClientJabbRAPI.UserActivityChanged += ClientJabbRAPI_UserActivityChanged;
			this.ClientJabbRAPI.UserJoined += ClientJabbRAPI_UserJoined;
			this.ClientJabbRAPI.UsernameChanged += ClientJabbRAPI_UsernameChanged;
			this.ClientJabbRAPI.UsersInactive += ClientJabbRAPI_UsersInactive;
			this.ClientJabbRAPI.UserTyping += ClientJabbRAPI_UserTyping;

			//this.ClientJabbRAPI.StateChanged += ClientJabbRAPI_StateChanged;
			//this.ClientJabbRAPI.Disconnected += ClientJabbRAPI_Disconnected;

			return;
		}

		public async Task<Login> LoginAsync (string username, string password)
		{
			//await CreateAccountAsync(server, username, password);

			Login login = new Login () {
				LogOnInfo = null,
				IsLoggedIn = false,
				Username = username,
				Password = password
			};
			// Connect to chat
			if (null == password || "".Equals (password))
			{
			}
			else
			{
				try
				{
					this.LogOnInfo = await ClientJabbRAPI.Connect (username, password);
				}
				catch (Exception exc)
				{
					string msg = exc.Message;
				}
				login.LogOnInfo = this.LogOnInfo;
				login.RoomsSubscribed = new ObservableCollection<JabbR.Client.Models.Room>(this.LogOnInfo.Rooms);
			}

			if (null != this.LogOnInfo)
			{
				login.IsLoggedIn = true;
				State.StateData.Login = login;
			}

			ObservableCollection<JabbR.Client.Models.Room> rooms_subscribed = null;
			ObservableCollection<JabbR.Client.Models.Room> rooms_available = null;

			rooms_subscribed = await login.GetRoomsSubscribed();
			rooms_available = await login.GetRoomsAvailable();

			return login;
		}


		protected void ClientJabbRAPI_JoinedRoom (JabbR.Client.Models.Room obj)
		{
			
		}

		protected void ClientJabbRAPI_UserTyping (JabbR.Client.Models.User arg1, string arg2)
		{
			
		}

		protected void ClientJabbRAPI_UsersInactive (System.Collections.Generic.IEnumerable<JabbR.Client.Models.User> obj)
		{
			
		}

		protected void ClientJabbRAPI_UsernameChanged (string arg1, JabbR.Client.Models.User arg2, string arg3)
		{
			
		}

		protected void ClientJabbRAPI_UserLeft1 (JabbR.Client.Models.User arg1, string arg2)
		{
			
		}

		protected void ClientJabbRAPI_UserActivityChanged (JabbR.Client.Models.User obj)
		{
			
		}

		protected void ClientJabbRAPI_TopicChanged (string arg1, string arg2, string arg3)
		{
			
		}

		protected void ClientJabbRAPI_StateChanged (Microsoft.AspNet.SignalR.Client.StateChange obj)
		{
			
		}

		protected void ClientJabbRAPI_RoomChanged (JabbR.Client.Models.Room obj)
		{
			
		}

		protected void ClientJabbRAPI_OwnerRemoved (JabbR.Client.Models.User arg1, string arg2)
		{
			
		}

		protected void ClientJabbRAPI_OwnerAdded (JabbR.Client.Models.User arg1, string arg2)
		{
			
		}

		protected void ClientJabbRAPI_NoteChanged (JabbR.Client.Models.User arg1, string arg2)
		{
			
		}

		protected void ClientJabbRAPI_MeMessageReceived (string arg1, string arg2, string arg3)
		{
			
		}

		protected void ClientJabbRAPI_LoggedOut (System.Collections.Generic.IEnumerable<string> obj)
		{
			
		}

		protected void ClientJabbRAPI_Kicked (string obj)
		{
			
		}

		protected void ClientJabbRAPI_GravatarChanged (JabbR.Client.Models.User arg1, string arg2)
		{
			
		}

		protected void ClientJabbRAPI_FlagChanged (JabbR.Client.Models.User arg1, string arg2)
		{
			
		}

		protected void ClientJabbRAPI_Disconnected ()
		{
			
		}

		protected void ClientJabbRAPI_AddMessageContent (string arg1, string arg2, string arg3)
		{
			
		}

		private void ClientJabbRAPI_MessageReceived(JabbR.Client.Models.Message message, string arg2)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("[").Append(message.When).Append("] ");
			sb.Append(message.User.Name).Append(": ").Append(message.Content);

			this.ToLog(sb.ToString());

			return;
		}

		private void ClientJabbRAPI_UserJoined(JabbR.Client.Models.User user, string room, bool arg3)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(user.Name).Append(" joined ").Append(room);

			this.ToLog(sb.ToString());

			return;
		}

		private void ClientJabbRAPI_UserLeft(JabbR.Client.Models.User user, string room)
		{
			System.Diagnostics.Debug.WriteLine("{0} left {1}", user.Name, room);

			return;
		}

		private void ClientJabbRAPI_PrivateMessage(string from, string to, string message)
		{
			System.Diagnostics.Debug.WriteLine("*PRIVATE* {0} -> {1} ", from, message);

			return;
		}

		public void ToLog (string msg)
		{
			System.Diagnostics.Debug.WriteLine (DateTime.Now + ": " + msg);

			LogItem li =  new LogItem()
			{
				TimeStamp = DateTime.Now,
				Event = msg,
			};

			State.StateData?.Logger.Log.Add(li);

			return;
		}
	}
}


