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
using System.Threading.Tasks;
using AVFoundation;

namespace ProxiChat.Ios
{
	public partial class ConversationViewController : UIViewController, IUnsubscribeViewController
	{
		// Keep static reference for comparison
		public static string MessagePlaceHolderText = "Enter Message";

		ConversationViewModel _viewModel;

		List<UIMessage> _uiMessages = new List<UIMessage>();

		// Holds info placed into message box, represents current message body
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

			// create custom view for the navigation bar to hold profile image 
			//		and username of person user is chatting with
			var navView = UserView.Create();
			navView.SetValues(_viewModel.UserName, _viewModel.ProfileImageUrl);
			this.NavigationItem.TitleView = navView;

			//TODO button should probably do something
			this.NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Action, null);

			// handle moving the message box up when keyboard is visible
			RegisterForKeyboardNotifications();
			DismissKeyboardOnBackgroundTap();

			_messageBoxTextView.Delegate = new ExpandingTextViewDelegate(MessageBoxHeight, _messageBoxTextView);

			// UITableView.AutomaticDimension is used to allow the os auto layout determine height
			_conversationTableview.RowHeight = UITableView.AutomaticDimension;
 			// specifying an estimated height is required for Automatic Dimensions to workk
			_conversationTableview.EstimatedRowHeight = 100;

			_conversationTableview.Source = new ConversationTableViewSource(_viewModel, _uiMessages, ShowViewController);
			_conversationTableview.ReloadData();

			_sendButton.TouchDown += SendButtonPressed;
			_paperClipButton.TouchDown += PaperclipButtonPressed;
		}

		/// <summary>
		/// Wrap PresentViewControllerAsync so that it can be awaited, it is passed to the table source as an Action<UIviewController>
		/// </summary>
		/// <param name="viewController">View controller.</param>
		private async void ShowViewController(UIViewController viewController)
		{
			await PresentViewControllerAsync(viewController, true);
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
					await PresentViewControllerAsync(alert, true);
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
						InteractionAction = UserTappedMedia,
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
						VideoData = mediaURL,
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

			_currentMessage = null;

			// reset message box values
			ResetMessageBoxValues();
		}

		private void ResetMessageBoxValues()
		{
			(_messageBoxTextView.Delegate as ExpandingTextViewDelegate).UserEnteredText = false;
			_messageBoxTextView.Text = ConversationViewController.MessagePlaceHolderText;
			MessageBoxHeight.Constant = _messageBoxTextView.ContentSize.Height + 10;
		}

		public void UnsubscribeFromEvents()
		{
			_sendButton.TouchDown -= SendButtonPressed;
			_paperClipButton.TouchDown -= PaperclipButtonPressed;

		}

		#region Navigate To Media

		private void UserTappedMedia(UIImageWrapper wrapper)
		{
			var viewController = new UIViewController();
			viewController.View.BackgroundColor = UIColor.White;

			if (wrapper.IsVideo)
			{
				AddVideoToViewController(viewController, wrapper.VideoData);
			}
			else
			{
				AddImageToViewController(viewController, wrapper.image);
			}

			NavigationController.PushViewController(viewController, true);
		}

		private void AddVideoToViewController(UIViewController viewController, NSUrl videoData)
		{
			var asset = AVAsset.FromUrl(videoData);
			var playerItem = new AVPlayerItem(asset);
			var player = new AVPlayer(playerItem);
			var playerLayer = AVPlayerLayer.FromPlayer(player);

			viewController.View.Layer.AddSublayer(playerLayer);
		}

		private void AddImageToViewController(UIViewController viewController, UIImage image)
		{
			var imageView = new UIImageView();
			imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			imageView.Image = image;
			imageView.TranslatesAutoresizingMaskIntoConstraints = false;
			viewController.View.AddSubview(imageView);

			AddConstraints(viewController.View, imageView);
		}

		private void AddConstraints(UIView view, UIView imageView)
		{
			view.AddConstraint(
				NSLayoutConstraint.Create
				(view,
				 NSLayoutAttribute.Left,
				 NSLayoutRelation.Equal,
				 imageView,
				 NSLayoutAttribute.Left,
				 1,
				 0));
			view.AddConstraint(
				NSLayoutConstraint.Create
				(view,
				 NSLayoutAttribute.Top,
				 NSLayoutRelation.Equal,
				 imageView,
				 NSLayoutAttribute.Top,
				 1,
				 0));
			view.AddConstraint(
				NSLayoutConstraint.Create
				(view,
				 NSLayoutAttribute.Right,
				 NSLayoutRelation.Equal,
				 imageView,
				 NSLayoutAttribute.Right,
				 1,
				 0));
			view.AddConstraint(
				NSLayoutConstraint.Create
				(view,
				 NSLayoutAttribute.Bottom,
				 NSLayoutRelation.Equal,
				 imageView,
				 NSLayoutAttribute.Bottom,
				 1,
				 0));
		}
	}

	#endregion

	public class UIImageWrapper
	{
		public UIImage image { get; set; }
		public nint ImagePosition { get; set; }
		public NSUrl VideoData { get; set; }
		public bool IsVideo { get; set; }
		public Action<UIImageWrapper> InteractionAction { get; set; }
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
		Action<UIViewController> _showViewController;

		public ConversationTableViewSource(ConversationViewModel viewModel, List<UIMessage> messages, Action<UIViewController> showViewController)
		{
			_showViewController = showViewController;
			_viewModel = viewModel;
			_messages = messages;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var message = _messages[indexPath.Row];

			UITableViewCell cell = null;
			string profileUrl = string.Empty;

			if (message.SentByUser)
			{
				profileUrl = "https://www.cheme.cornell.edu/engineering2/customcf" + 
					"/iws_ai_faculty_display/ai_images/mjp31-profile.jpg";
				
				cell = tableView.DequeueReusableCell("RightMessageTableViewCell");
			}
			else
			{
				cell = tableView.DequeueReusableCell("LeftMessageTableViewCell");
				profileUrl = _viewModel.ProfileImageUrl;
			}

			cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			
			((ReceiverMessageTableViewCell)cell).GestureRecognizer = new UILongPressGestureRecognizer(() =>
			{
				var alert = UIAlertController.Create(string.Empty, string.Empty, UIAlertControllerStyle.ActionSheet);
				alert.AddAction(UIAlertAction.Create("Copy text", UIAlertActionStyle.Default, null));
				alert.AddAction(UIAlertAction.Create("Delete", UIAlertActionStyle.Destructive, (action) =>
				{
					DeleteMessage(message, tableView);
				}));
				alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

				if (_showViewController != null)
				{
					_showViewController(alert);
				}
			});

			((ReceiverMessageTableViewCell)cell).UpdateCell(profileUrl, message);

			return cell;
		}

		private void DeleteMessage(UIMessage message, UITableView tableView)
		{
			var index = _messages.IndexOf(message);
			_messages.Remove(message);

			tableView.BeginUpdates();
			tableView.DeleteRows(new NSIndexPath[] { NSIndexPath.FromRowSection(index, 0) }, UITableViewRowAnimation.Automatic);
			tableView.EndUpdates();
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
		