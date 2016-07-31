using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prism.Mvvm;
using ProxiChat.Mobile.Interfaces;

namespace ProxiChat.Mobile.ViewModels
{
	public class ConversationViewModel : BindableBase, IApplicationViewModel
	{
		List<Message> _messages;

		public string UserName { get; set; }
		public string ProfileImageUrl { get; set; }

		public List<Message> Messages
		{
			get
			{
				return _messages;
			}
		}

		public ConversationViewModel()
		{
			_messages = new List<Message>();
		}

		public event ViewModelNavigationRequestHandler ViewModelNavigationRequested;

		public Task Start()
		{
			throw new NotImplementedException();
		}

		public void AddMessage(string message)
		{
			
			_messages.Add(new Message {
				TimeSent = DateTime.Now,
				MessageBody = message,
				SentByUser = true
			});
		}
	}

	public class Message
	{
		public DateTime TimeSent { get; set; }
		public string MessageBody { get; set; }
		public bool SentByUser { get; set; }
	}
}

