using Foundation;
using System;
using UIKit;
using ProxiChat.Mobile.ViewModels;

namespace ProxiChat.Ios
{
    public partial class ConversationViewController : UIViewController
    {
		ConversationViewModel _viewModel;

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

        public ConversationViewController (IntPtr handle) : base (handle)
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

			_conversationTableview.Source = new ConversationTableViewSource(_viewModel);
			_conversationTableview.ReloadData();
			_sendButton.TouchDown += SendButtonPressed;
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

		private void SendButtonPressed(object sender, EventArgs args)
		{
			string text = _messageBoxTextView.Text;
			_messageBoxTextView.Text = string.Empty;
			_viewModel.AddMessage(text);

			var newIndexPath = NSIndexPath.FromRowSection(_viewModel.Messages.Count - 1, 0);
			_conversationTableview.BeginUpdates();
			_conversationTableview.InsertRows(new NSIndexPath[] { newIndexPath }, UITableViewRowAnimation.Automatic);
			_conversationTableview.EndUpdates();
		}
	}

	public class ConversationTableViewSource : UITableViewSource
	{
		ConversationViewModel _viewModel;

		public ConversationTableViewSource(ConversationViewModel viewModel)
		{
			_viewModel = viewModel;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell("ReceiverMessageTableViewCell") as ReceiverMessageTableViewCell;
			cell.UpdateCell(_viewModel.ProfileImageUrl, _viewModel.Messages[indexPath.Row]);
			return cell;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return _viewModel.Messages.Count;
		}

	}

	public class ExpandingTextViewDelegate : UITextViewDelegate
	{
		NSLayoutConstraint _heightConstraint;
		UITextView _textView;
		private string placeholderText = "Enter Message";

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
			if (textView.Text == placeholderText)
				textView.Text = string.Empty;

			return true;
		}

		public override bool ShouldEndEditing(UITextView textView)
		{
			if (string.IsNullOrEmpty(textView.Text))
				textView.Text = placeholderText;

			return true;
		}
	}
}