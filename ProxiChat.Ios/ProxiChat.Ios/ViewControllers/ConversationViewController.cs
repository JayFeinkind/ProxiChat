using Foundation;
using System;
using UIKit;
using ProxiChat.Mobile.ViewModels;
using CoreGraphics;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using MobileCoreServices;
using MediaPlayer;
using System.IO;

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
		#region PhotoLibrary
		private async void AddPhotoFromLibrary(UIAlertAction action)
		{

			var imagePicker = new UIImagePickerController();
			imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);

			imagePicker.FinishedPickingMedia += HandleFinishedPickingMedia;
			imagePicker.Canceled += HandleCancel;

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
			(_messageBoxTextView.Delegate as ExpandingTextViewDelegate).UserEnteredText = true;
			MessageBoxHeight.Constant = _messageBoxTextView.ContentSize.Height + 10;
		}

		#endregion



		private async void AddFromCamera()
		{
			try
			{
				var imagePicker = new UIImagePickerController();

				if (UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera))
				{
					imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;
					imagePicker.ShowsCameraControls = true;
					imagePicker.MediaTypes = new string[] { UTType.Image, UTType.Video };
					imagePicker.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
					imagePicker.FinishedPickingMedia += HandleFinishedPickingMedia;
					imagePicker.Canceled += HandleCancel;
					await PresentViewControllerAsync(imagePicker, true);
				}
				else
				{
					var alert = AppUtilities.CreateGenericAlert("Sorry", "No access to camera");
				}
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		#region Add Media
		protected async void HandleFinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
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

			if (_currentMessage == null) _currentMessage = new UIMessage();
			if (_currentMessage.ImageWrappers == null) _currentMessage.ImageWrappers = new List<UIImageWrapper>();


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

					_currentMessage.ImageWrappers.Add(new UIImageWrapper
					{
						image = originalImage,
						ImagePosition = _messageBoxTextView.Text != MessagePlaceHolderText ?
						                                   _messageBoxTextView.AttributedText.Length :
						                                   0
					});

					AddMediaToTextView(originalImage); // display
				}
			}
			else 
			{ 
				// if it's a video
				   // get video url
				NSUrl mediaURL = e.Info[UIImagePickerController.MediaURL] as NSUrl;
				var videoData = NSData.FromUrl(mediaURL);

				var movie = new MPMoviePlayerController(mediaURL);
				movie.ShouldAutoplay = false;
				double videoPositionSec = 1;
				UIImage videoThumbnail = movie.ThumbnailImageAt(videoPositionSec, MPMovieTimeOption.NearestKeyFrame);

				if (mediaURL != null)
				{
					_currentMessage.ImageWrappers.Add(new UIImageWrapper
					{
						image = videoThumbnail,
						IsVideo = true,
						VideoData = videoData,
						ImagePosition = _messageBoxTextView.Text != MessagePlaceHolderText ?
														   _messageBoxTextView.AttributedText.Length :
														   0
					});

					AddMediaToTextView(videoThumbnail);

					Console.WriteLine(mediaURL.ToString());
				}
			}
			// dismiss the picker
			await (sender as UIImagePickerController).DismissViewControllerAsync(true);
		}

		private async void HandleCancel(object sender, EventArgs e)
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

		#endregion

		private void SendButtonPressed(object sender, EventArgs args)
		{
			if ((_messageBoxTextView.Delegate as ExpandingTextViewDelegate).UserEnteredText == false)
				return;

			if (_currentMessage == null) _currentMessage = new UIMessage();

			_currentMessage.AttributedText = _messageBoxTextView.AttributedText;
			_currentMessage.MessageBody = _messageBoxTextView.AttributedText.Value;
			_currentMessage.SentByUser = true;
			_currentMessage.TimeSent = DateTime.Now;

			_uiMessages.Add(_currentMessage);

			var newIndexPath = NSIndexPath.FromRowSection(_uiMessages.Count - 1, 0);
			_conversationTableview.BeginUpdates();
			_conversationTableview.InsertRows(new NSIndexPath[] { newIndexPath }, UITableViewRowAnimation.Automatic);
			_conversationTableview.EndUpdates();

			var mockResponse = new UIMessage();
			mockResponse.AttributedText = _currentMessage.AttributedText;
			mockResponse.ImageWrappers = _currentMessage.ImageWrappers;
			mockResponse.MessageBody = _currentMessage.MessageBody;
			mockResponse.SentByUser = false;
			mockResponse.TimeSent = DateTime.Now;

			_uiMessages.Add(mockResponse);

			_currentMessage = null;

			newIndexPath = NSIndexPath.FromRowSection(_uiMessages.Count - 1, 0);
			_conversationTableview.BeginUpdates();
			_conversationTableview.InsertRows(new NSIndexPath[] { newIndexPath }, UITableViewRowAnimation.Automatic);
			_conversationTableview.EndUpdates();

			(_messageBoxTextView.Delegate as ExpandingTextViewDelegate).UserEnteredText = false;
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
		public UIImage image { get; set; }
		public nint ImagePosition { get; set; }
		public NSData VideoData { get; set; }
		public bool IsVideo { get; set; }
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
			var message = _messages[indexPath.Row];

			UITableViewCell cell = null;

			if (message.SentByUser)
			{
				cell = tableView.DequeueReusableCell("RightMessageTableViewCell");
				((ReceiverMessageTableViewCell)cell).UpdateCell(_viewModel.ProfileImageUrl, message);
			}
			else
			{
				cell = tableView.DequeueReusableCell("LeftMessageTableViewCell");
				((ReceiverMessageTableViewCell)cell).UpdateCell(_viewModel.ProfileImageUrl, message);
			}

			return cell;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return _messages.Count;
		}

	}

	/// <summary>
	/// Handles placeholder functionality for text view.  Keeps track if user
	/// 	enteres the text or if its the placeholder.  Any changes made programatically not
	/// 		call these event overrides. programatically changes  will
	/// 		have to also update these values to keep them current.
	/// </summary>
	public class ExpandingTextViewDelegate : UITextViewDelegate
	{
		NSLayoutConstraint _heightConstraint;
		UITextView _textView;

		public bool UserEnteredText { get; set; }

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
			{
				textView.Text = string.Empty;
				UserEnteredText = true;
			}

			return true;
		}

		public override bool ShouldEndEditing(UITextView textView)
		{
			if (string.IsNullOrEmpty(textView.Text))
			{
				UserEnteredText = false;
				textView.Text = ConversationViewController.MessagePlaceHolderText;
			}

			return true;
		}
	}
}
		