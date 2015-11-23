using System;


namespace HolisticWare.XamarinForms.XamarinIOS
{
	/// <summary>
	/// Content page.
	///	Not in XAML to reduce code pollution. Base class with iOS platform tweak
	///
	///	Usage:
	///
	///	<h:ContentPage 
	///		xmlns="http://xamarin.com/schemas/2014/forms" 
	///		xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" 
	///		x:Class="JabbR.Client.Views.Pages.User"
	///		xmlns:h="clr-namespace:HolisticWare.XamarinForms.XamarinIOS;assembly=JabbR.Client.Views.Pages"
	///		Title="User"
	///		>
	///		<ContentPage.Content>
	///		</ContentPage.Content>
	///	</h:ContentPage>
	/// </summary>
	public class ContentPage : Xamarin.Forms.ContentPage
	{
		public ContentPage ()
			: base()
		{

			if (Xamarin.Forms.Device.OS == Xamarin.Forms.TargetPlatform.iOS) 
			{
				// move layout under the status bar
			    Padding = new Xamarin.Forms.Thickness (0, 20, 0, 0);
			}

			return;
		}
	}
}


