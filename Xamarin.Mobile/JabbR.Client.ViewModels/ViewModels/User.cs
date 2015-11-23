using System;
using System.Threading.Tasks;

namespace JabbR.Client.ViewModels
{
	[PropertyChanged.ImplementPropertyChanged]
	public partial class User 
	{
		public static User Current
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

		public JabbR.Client.Models.User UserJabbRAPI
		{
			get;
			set;
		}

		public User ()
		{
		}

		public User (JabbR.Client.Models.User user)
		{
			this.UserJabbRAPI = user;

			return;
		}

		private static JabbR.Client.Models.User current_user_jabbr = null;

		public static async Task<User> UserInfoAsync()
		{
			// Get my user info
			current_user_jabbr = await State.StateData.Client.ClientJabbRAPI.GetUserInfo();

			User user = new User(current_user_jabbr);

			return user;
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

		public void ToLog ()
		{
			ToLog(this.UserJabbRAPI.Name);
			ToLog(this.UserJabbRAPI.LastActivity.ToString());
			ToLog(this.UserJabbRAPI.Status.ToString());
			ToLog(this.UserJabbRAPI.Country);

			return;
		}

	}
}


