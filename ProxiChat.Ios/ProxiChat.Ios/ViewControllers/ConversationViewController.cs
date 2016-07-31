using Foundation;
using System;
using UIKit;
using ProxiChat.Mobile.ViewModels;
using CoreGraphics;
using System.Drawing;
using System.Collections.Generic;

namespace ProxiChat.Ios
{
	public partial class ConversationViewController : UIViewController, IUnsubscribeViewController
	{
		public static string MessagePlaceHolderText = "Enter Message";

		ConversationViewModel _viewModel;

		List<UIMessage> _uiMessages = new List<UIMessage>();
		UIMessage _currentMessage = null;

		public ConversationViewModel ViewModel
		{
			get
			{
				return _viewModel;
			}
			set
			{
				_viewModel = value;
			}
		}

		public ConversationViewController(IntPtr handle) : base(handle)
		{
		}

		public override void LoadView()
		{
			base.LoadView();

			var navView = UserView.Create();
			navView.SetValues(_viewModel.UserName, _viewModel.ProfileImageUrl);

			this.NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Action, null);
			this.NavigationItem.TitleView = navView;

			// handle moving the message box up when keyboard is visible
			RegisterForKeyboardNotifications();
			DismissKeyboardOnBackgroundTap();

			_messageBoxTextView.Delegate = new ExpandingTextViewDelegate(MessageBoxHeight, _messageBoxTextView);

			_conversationTableview.RowHeight = UITableView.AutomaticDimension;
			_conversationTableview.EstimatedRowHeight = 100;

			_conversationTableview.Source = new ConversationTableViewSource(_viewModel, _uiMessages);
			_conversationTableview.ReloadData();
			_sendButton.TouchDown += SendButtonPressed;
			_paperClipButton.TouchDown += PaperclipButtonPressed;
		}

		#region Logic to move message box when keyboard is up

		private void RegisterForKeyboardNotifications()
		{
			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
		}

		private void OnKeyboardNotification(NSNotification notification)
		{
			if (!IsViewLoaded) return;

			//Check if the keyboard is becoming visible
			var visible = notification.Name == UIKeyboard.WillShowNotification;

			//Start an animation, using values from the keyboard
			UIView.BeginAnimations("AnimateForKeyboard");
			UIView.SetAnimationBeginsFromCurrentState(true);
			UIView.SetAnimationDuration(UIKeyboard.AnimationDurationFromNotification(notification));
			UIView.SetAnimationCurve((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification(notification));

			//Pass the notification, calculating keyboard height, etc.
			var keyboardFrame = visible
									? UIKeyboard.FrameEndFromNotification(notification)
									: UIKeyboard.FrameBeginFromNotification(notification);

			OnKeyboardChanged(visible, keyboardFrame.Height);

			//Commit the animation
			UIView.CommitAnimations();
		}

		/// <summary>
		/// Override this method to apply custom logic when the keyboard is shown/hidden
		/// </summary>
		/// <param name='visible'>
		/// If the keyboard is visible
		/// </param>
		/// <param name='keyboardHeight'>
		/// Calculated height of the keyboard (width not generally needed here)
		/// </param>
		protected virtual void OnKeyboardChanged(bool visible, nfloat keyboardHeight)
		{
			if (visible)
			{
				MessageBoxToBottom.Constant = 10 + keyboardHeight;
			}
			else
			{
				MessageBoxToBottom.Constant = 10;
			}
		}

		/// <summary>
		/// Call it to force dismiss keyboard when background is tapped
		/// </summary>
		private void DismissKeyboardOnBackgroundTap()
		{
			// Add gesture recognizer to hide keyboard
			var tap = new UITapGestureRecognizer { CancelsTouchesInView = false };
			tap.AddTarget(() => View.EndEditing(true));
			View.AddGestureRecognizer(tap);
		}

		#endregion

		private async void AddPhotoFromLibrary(UIAlertAction action)
		{

			var imagePicker = new UIImagePickerController();
			imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);

			imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
			imagePicker.Canceled += Handle_Canceled;

			await PresentViewControllerAsync(imagePicker, true);
		}

		private void AddMediaToTextView(UIImage image)
		{
			nfloat ratio = image.Size.Height / image.Size.Width;

			var newSize = new CGSize(50, 50.0f * ratio);
			;
			var attachment = new NSTextAttachment();
			attachment.Image = image.Scale(newSize);
			var initialText = _messageBoxTextView.Text == MessagePlaceHolderText ? new NSAttributedString() : _messageBoxTextView.AttributedText;
			var newText = new NSMutableAttributedString(initialText);
			newText.Append(NSAttributedString.CreateFrom(attachment));

			_messageBoxTextView.AttributedText = newText;
			MessageBoxHeight.Constant = _messageBoxTextView.ContentSize.Height + 10;
		}

		protected async void Handle_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
		{
			// determine what was selected, video or image
			bool isImage = false;
			switch (e.Info[UIImagePickerController.MediaType].ToString())
			{
				case "public.image":
					Console.WriteLine("Image selected");
					isImage = true;
					break;
				case "public.video":
					Console.WriteLine("Video selected");
					break;
			}

			// get common info (shared between images and video)
			NSUrl referenceURL = e.Info[new NSString("UIImagePickerControllerReferenceUrl")] as NSUrl;
			if (referenceURL != null)
				Console.WriteLine("Url:" + referenceURL.ToString());

			// if it was an image, get the other image info
			if (isImage)
			{
				// get the original image
				UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
				if (originalImage != null)
				{
					// do something with the image
					Console.WriteLine("got the original image");

					if (_currentMessage == null) _currentMessage = new UIMessage();
					if (_currentMessage.ImageWrappers == null) _currentMessage.ImageWrappers = new List<UIImageWrapper>();

					_currentMessage.ImageWrappers.Add(new UIImageWrapper
					{
						image = originalImage,
						ImagePosition = _messageBoxTextView.AttributedText.Length
					});

					AddMediaToTextView(originalImage); // display
				}
			}
			else { // if it's a video
				   // get video url
				NSUrl mediaURL = e.Info[UIImagePickerController.MediaURL] as NSUrl;
				if (mediaURL != null)
				{
					Console.WriteLine(mediaURL.ToString());
				}
			}
			// dismiss the picker
			await (sender as UIImagePickerController).DismissViewControllerAsync(true);
		}

		private async void Handle_Canceled(object sender, EventArgs e)
		{
			await (sender as UIImagePickerController).DismissViewControllerAsync(true);
		}

		private async void PaperclipButtonPressed(object sender, EventArgs args)
		{
			var alert = UIAlertController.Create(string.Empty, string.Empty, UIAlertControllerStyle.ActionSheet);
			alert.AddAction(UIAlertAction.Create("Take Photo", UIAlertActionStyle.Default, null));
			alert.AddAction(UIAlertAction.Create("Add Media", UIAlertActionStyle.Default, AddPhotoFromLibrary));

			if (UIDevice.CurrentDevice.UserInterfaceIdiom != UIUserInterfaceIdiom.Pad)
			{
				alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
				await PresentViewControllerAsync(alert, true);
			}
			else
			{
				var popUp = new UIPopoverController(alert);
				popUp.PresentFromRect(_paperClipButton.Bounds, _paperClipButton, UIPopoverArrowDirection.Any, true);
			}
		}

		private void SendButtonPressed(object sender, EventArgs args)
		{
			if (_messageBoxTextView.AttributedText.Value == MessagePlaceHolderText)
				return;
			
			var text = _messageBoxTextView.AttributedText;
			_messageBoxTextView.Text = string.Empty;
			_viewModel.AddMessage(text.Value);

			if (_currentMessage == null) _currentMessage = new UIMessage();

			_currentMessage.AttributedText = text;
			_currentMessage.MessageBody = text.Value;
			_currentMessage.SentByUser = true;
			_currentMessage.TimeSent = DateTime.Now;

			_uiMessages.Add(_currentMessage);
			_currentMessage = null;

			var newIndexPath = NSIndexPath.FromRowSection(_viewModel.Messages.Count - 1, 0);
			_conversationTableview.BeginUpdates();
			_conversationTableview.InsertRows(new NSIndexPath[] { newIndexPath }, UITableViewRowAnimation.Automatic);
			_conversationTableview.EndUpdates();

			_messageBoxTextView.Text = ConversationViewController.MessagePlaceHolderText;
			MessageBoxHeight.Constant = _messageBoxTextView.ContentSize.Height + 10;
		}

		public void UnsubscribeFromEvents()
		{
			_sendButton.TouchDown -= SendButtonPressed;
			_paperClipButton.TouchDown -= PaperclipButtonPressed;

		}
	}

	public class UIImageWrapper
	{
		public UIImage image;
		public nint ImagePosition;
	}

	public class UIMessage : Message
	{
		public List<UIImageWrapper> ImageWrappers{ get; set; }
		public NSAttributedString AttributedText { get; set; }
	}

	public class ConversationTableViewSource : UITableViewSource
	{
		List<UIMessage> _messages;
		ConversationViewModel _viewModel;

		public ConversationTableViewSource(ConversationViewModel viewModel, List<UIMessage> messages)
		{
			_viewModel = viewModel;
			_messages = messages;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell("ReceiverMessageTableViewCell") as ReceiverMessageTableViewCell;
			cell.UpdateCell(_viewModel.ProfileImageUrl, _messages[indexPath.Row]);
			return cell;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return _messages.Count;
		}

	}

	public class ExpandingTextViewDelegate : UITextViewDelegate
	{
		NSLayoutConstraint _heightConstraint;
		UITextView _textView;


		public ExpandingTextViewDelegate(NSLayoutConstraint heightConstraint, UITextView view)
		{
			_textView = view;
			_heightConstraint = heightConstraint;
		}


		public override void Changed(UITextView textView)
		{
			if (_heightConstraint.Constant != _textView.ContentSize.Height + 10)
			{
				_heightConstraint.Constant = _textView.ContentSize.Height + 10;
			}
		}

		public override bool ShouldBeginEditing(UITextView textView)
		{
			if (textView.Text == ConversationViewController.MessagePlaceHolderText)
				textView.Text = string.Empty;

			return true;
		}

		public override bool ShouldEndEditing(UITextView textView)
		{
			if (string.IsNullOrEmpty(textView.Text))
				textView.Text = ConversationViewController.MessagePlaceHolderText;

			return true;
		}
	}
}