using Foundation;
using System;
using UIKit;
using SDWebImage;
using ProxiChat.Mobile.ViewModels;

namespace ProxiChat.Ios
{
    public partial class ReceiverMessageTableViewCell : UITableViewCell
    {
        public ReceiverMessageTableViewCell (IntPtr handle) : base (handle)
        {
        }

		public void UpdateCell(string url, Message message)
		{
			string messageStr = message.MessageBody + Environment.NewLine;
			string time = message.TimeSent.ToShortTimeString();

			var attrMessage = new NSMutableAttributedString(messageStr + time);

			attrMessage.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(15), new NSRange(0, messageStr.Length));
			attrMessage.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.Black, new NSRange(0, messageStr.Length));
			attrMessage.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(13), new NSRange(messageStr.Length, time.Length));
			attrMessage.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.LightTextColor, new NSRange(messageStr.Length, time.Length));

			_messageLabel.AttributedText = attrMessage;

			_profileImageView.Layer.MasksToBounds = true;
			_profileImageView.Layer.CornerRadius = 30.0f;
			_profileImageView.SetImage(new NSUrl(url), ImageUtility.UserProfileImagePlaceHolder.Value);

		}
    }
}