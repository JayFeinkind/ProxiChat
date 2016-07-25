using System;
using UIKit;

namespace ProxiChat.Ios
{
	public static class AppUtilities
	{
		/*
		 * Returns an alert view used for a pop up alert.
		 * alert will have an ok button which will only close the alert.  No further
		 * action will be available.
		 */ 
		public static UIAlertController CreateGenericAlert(string title, string message)
		{
			var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
			alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));

			return alert;
		}

		/*
		 * Returns an alert view used for a pop up alert.
		 * alert will have an ok button which will only close the alert. further action will depend
		 * on parameter list
		 */
		public static UIAlertController CreateGenericAlert(string title, string message, params UIAlertAction[] actions)
		{
			var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);

			foreach (var action in actions)
			{
				alert.AddAction(action);
			}

			return alert;
		}
	}
}