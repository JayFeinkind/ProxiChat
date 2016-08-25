using Foundation;
using System;
using UIKit;
using SDWebImage;
using ProxiChat.Mobile.ViewModels;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;

namespace ProxiChat.Ios
{
    public partial class ReceiverMessageTableViewCell : UITableViewCell
    {
		/// <summary>
		/// keep track of added views so they can be removed before reuse
		/// </summary>
		List<UIView> _messageViews = new List<UIView>();

		public UILongPressGestureRecognizer GestureRecognizer { get; set; }

        public ReceiverMessageTableViewCell (IntPtr handle) : base (handle)
        {
        }

		public override void PrepareForReuse()
		{
			base.PrepareForReuse();

			if (GestureRecognizer != null)
				ContentView.RemoveGestureRecognizer(GestureRecognizer);

			foreach (var view in _messageViews)
			{
				view.RemoveFromSuperview();
			}

			_messageViews.Clear();
		}

		#region Constraints

		private void SetImageConstraints(UIImageView view, nfloat height, nfloat width)
		{
			ContentView.AddConstraint(
				NSLayoutConstraint.Create(
					view,
					NSLayoutAttribute.Height,
					NSLayoutRelation.Equal,
					null,
					NSLayoutAttribute.NoAttribute,
					1,
					height));
			
			ContentView.AddConstraint(
				NSLayoutConstraint.Create(
					view,
					NSLayoutAttribute.Width,
					NSLayoutRelation.Equal,
					null,
					NSLayoutAttribute.NoAttribute,
					1,
					width));
		}

		private void SetContentConstraints()
		{
			for (int counter = 0; counter < _messageViews.Count; counter++)
			{
				var view = _messageViews[counter];

				ContentView.AddConstraint(
					NSLayoutConstraint.Create(
						view,
						NSLayoutAttribute.Left,
						NSLayoutRelation.Equal,
						_messageBodyView,
						NSLayoutAttribute.Left,
						1,
						0));

				ContentView.AddConstraint(
					NSLayoutConstraint.Create(
						_messageBodyView,
						NSLayoutAttribute.Right,
						NSLayoutRelation.GreaterThanOrEqual,
						view,
						NSLayoutAttribute.Right,
						1,
						0));

				if (counter == 0)
				{
					ContentView.AddConstraint(
						NSLayoutConstraint.Create(
							view,
							NSLayoutAttribute.Top,
							NSLayoutRelation.Equal,
							_messageBodyView,
							NSLayoutAttribute.Top,
							1,
							3));
				}
				else
				{
					ContentView.AddConstraint(
						NSLayoutConstraint.Create(
							view,
							NSLayoutAttribute.Top,
							NSLayoutRelation.Equal,
							_messageViews[counter - 1],
							NSLayoutAttribute.Bottom,
							1,
							3));
				}

				if (counter == _messageViews.Count - 1)
				{
					ContentView.AddConstraint(
						NSLayoutConstraint.Create(
							view,
							NSLayoutAttribute.Bottom,
							NSLayoutRelation.Equal,
							_messageBodyView,
							NSLayoutAttribute.Bottom,
							1,
							3));
				}
			}
		}

		#endregion

		#region Add Content
		private UIImageView AddImageView(UIImage image, CGSize newSize)
		{
			var imageView = new UIImageView();
			imageView.Image = image;
			imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			_messageBodyView.AddSubview(imageView);
			imageView.TranslatesAutoresizingMaskIntoConstraints = false;

			return imageView;
		}

		private UILabel AddMessageLabel(string message)
		{
			var messageLabel = new UILabel();
			messageLabel.Text = message;
			messageLabel.TranslatesAutoresizingMaskIntoConstraints = false;
			_messageBodyView.AddSubview(messageLabel);
			messageLabel.Font = UIFont.SystemFontOfSize(15);

			messageLabel.Lines = 0;
			messageLabel.LineBreakMode = UILineBreakMode.WordWrap;
			messageLabel.AdjustsFontSizeToFitWidth = false;

			return messageLabel;
		}
		private UILabel AddTimeLabel(string time)
		{
			UILabel timeLabel = new UILabel();
			timeLabel.Font = UIFont.BoldSystemFontOfSize(13);
			timeLabel.TextColor = UIColor.LightTextColor;
			timeLabel.Text = time;
			timeLabel.TranslatesAutoresizingMaskIntoConstraints = false;
			_messageBodyView.AddSubview(timeLabel);

			return timeLabel;
		}

		private void AddContentWidthMedia(UIMessage message)
		{
			int currentPosition = 0;

			for (int counter = 0; counter < message.ImageWrappers.Count; counter++)
			{
				var imageWrapper = message.ImageWrappers[counter];

				if (imageWrapper.ImagePosition > 0)
				{
					var label = AddMessageLabel(
						message.MessageBody.Substring(currentPosition,
						(int)imageWrapper.ImagePosition - 1));
					
					currentPosition += (int)imageWrapper.ImagePosition;

					_messageViews.Add(label);
				}

				var image = imageWrapper.image;
				nfloat ratio = image.Size.Height / image.Size.Width;
				var newSize = new CGSize(100, 100.0f * ratio);

				var imageView = AddImageView(image, newSize);

				imageView.UserInteractionEnabled = true;
				imageView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
				                                imageWrapper.InteractionAction(imageWrapper)));

				SetImageConstraints(imageView, newSize.Height, newSize.Width);
				_messageViews.Add(imageView);
			}

			if (currentPosition != message.MessageBody.Length)
			{
				var label = AddMessageLabel(message.MessageBody.Substring(currentPosition));
				_messageViews.Add(label);
			}
		}
		#endregion

		private void SetProfileImage(string url)
		{
			// required to round image to a circle
			_profileImageView.Layer.MasksToBounds = true;
			_profileImageView.Layer.CornerRadius = 25.0f;

			if (!string.IsNullOrWhiteSpace(url))
			{
				// if url is invalid it will throw an exception when instantiating NSUrl()
				try
				{
					_profileImageView.SetImage(new NSUrl(url), ImageUtility.UserProfileImagePlaceHolder.Value);
				}
				catch
				{
					_profileImageView.Image = ImageUtility.UserProfileImagePlaceHolder.Value;
				}
			}
			else
			{
				_profileImageView.Image = ImageUtility.UserProfileImagePlaceHolder.Value;
			}
		}

		public void UpdateCell(string url, UIMessage message)
		{
			ContentView.UserInteractionEnabled = true;

			if (GestureRecognizer != null)
				ContentView.AddGestureRecognizer(GestureRecognizer);

			if (message?.ImageWrappers != null && message.ImageWrappers.Count > 0)
			{
				AddContentWidthMedia(message);
			}
			else
			{
				var label = AddMessageLabel(message.MessageBody);

				_messageViews.Add(label);
			}

			var timeLabel = AddTimeLabel(message.TimeSent.ToShortTimeString());

			_messageViews.Add(timeLabel);

			SetContentConstraints();

			SetProfileImage(url);
		}
    }
}