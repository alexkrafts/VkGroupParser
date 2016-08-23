using System;
using System.Text;
using System.Windows.Input;
using Core;
using Microsoft.Practices.Prism.Events;
using Prism.Commands;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Utils;

namespace Wpf.ViewModels
{
    public class LoginViewModel : Notifier
    {
        public string Source { get; set; }
        private VkParser _parser;
        private IEventAggregator _aggregator;

        public LoginViewModel(VkParser parser, IEventAggregator aggregator)
        {
            _parser = parser;
            _aggregator = aggregator;
            Source = CreateAuthorizeUrlFor(3524567, Settings.All, Display.Popup);
        }
        public ICommand NavigatedCommand => new DelegateCommand<Uri>(Navigate);

        private void Navigate(Uri obj)
        {
            var act = VkAuthorization.From(obj);
            if (act.IsAuthorized && _parser.Auth(act.AccessToken))
            {
               _aggregator.GetEvent<TokenReceived>().Publish(act.AccessToken);
            }
        }

        internal static string CreateAuthorizeUrlFor(ulong appId, Settings settings, Display display)
        {
            var builder = new StringBuilder("https://oauth.vk.com/authorize?");

            builder.AppendFormat("client_id={0}&", appId);
            builder.AppendFormat("scope={0}&", settings);
            builder.Append("redirect_uri=https://oauth.vk.com/blank.html&");
            builder.AppendFormat("display={0}&", display);
            builder.Append("response_type=token");

            return builder.ToString();
        }
    }
}
