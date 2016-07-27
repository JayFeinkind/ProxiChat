using Foundation;
using System;
using UIKit;
using SDWebImage;

namespace ProxiChat.Ios
{
    public partial class UserListTableViewCell : UITableViewCell
    {
		public static readonly string Key = "UserListTableViewCell";

        public UserListTableViewCell (IntPtr handle) : base (handle)
        {
        }

		public void UpdateCell(string url, string userName, string messageSnippet, DateTime time)
		{
			_dateLabel.Text = time.Date == DateTime.Now.Date ? time.ToShortTimeString() : time.ToShortDateString();

			var tmp = "https://upload.wikimedia.org/wikipedia/commons/9/98/Christopher_Fabian_profile.jpg";

			_userNameLabel.Text = userName;

			_messageSnippetLabel.Text = messageSnippet;

			_profileImageView.Layer.MasksToBounds = true;
			_profileImageView.Layer.CornerRadius = 30;

			_profileImageView.SetImage(
				url: new NSUrl(tmp),
				placeholder: ImageUtility.UserProfileImagePlaceHolder.Value
			);
		}
    }
}