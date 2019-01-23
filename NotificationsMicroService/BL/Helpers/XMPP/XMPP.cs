using DotNetty.Transport.Channels;
using Matrix;
using Matrix.Extensions.Client.Roster;
using Matrix.Network.Resolver;
using Matrix.Xml;
using Matrix.Xmpp;
using Matrix.Xmpp.Client;
using Matrix.Xmpp.Roster;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BL.Helpers.XMPP
{
    public class XMPP
    {
        private readonly XmppClient _client;        
        private readonly string _domain;
        private readonly string _ipAddress;
        private readonly int _port;
        private readonly string _userName;
        private readonly string _password;

        private IChannel _channel;

        public event EventHandler<EventArgs<Message>> MessageAvailable;

        public XMPP()
        {
            _domain = ConfigurationManager.AppSettings["XMPPDomain"];
            _ipAddress = ConfigurationManager.AppSettings["XMPPIp"];
            _port = int.Parse(ConfigurationManager.AppSettings["XMPPPort"]);
            _userName = ConfigurationManager.AppSettings["XMPPUserName"];
            _password = ConfigurationManager.AppSettings["XMPPPassword"];

            var pipelineInitializerAction = new Action<IChannelPipeline>(pipeline =>
            {
                pipeline.AddFirst(new MyLoggingHandler());
            });

            _client = new XmppClient(pipelineInitializerAction)
            {
                Tls = false,
                XmppDomain = _domain,
                Resource = "",
                HostnameResolver = new StaticNameResolver(IPAddress.Parse(_ipAddress), _port)
            };

        }

        private async Task Connect(string userName, string password)
        {
            _client.Username = userName;
            _client.Password = password;
            _channel = await _client.ConnectAsync();
            await _client.SendAsync(new Presence(Show.Chat));
        }

        public async Task Register(string userName, string password)
        {
            _client.RegistrationHandler = new RegisterAccountHandler(_client);
            _client.Username = userName;
            _client.Password = password;
            await _client.ConnectAsync();
        }

        public async Task<IEnumerable<string>> GetRoster()
        {
            var res = await _client.RequestRosterAsync();
            var roster = res.Query as Roster; 
            return roster.GetRoster().Select(r => r.Jid.User);
        }

        public async Task RemoveUser(string userName)
        {
            //JGJG: Don't know how to implement it yet
            return;
         
        }


        public async Task SendPrivateMessage(string message, string to)
        {
            await Connect(_userName, _password);
            await _client.SendAsync(new Message(new Jid(to, _domain, null), message));
        }

        private void OnSessionStateChanged(SessionState sessionState)
        {
            Trace.WriteLine(sessionState);
        }

        private void OnXElementStreamChanged(XmppXElement element)
        {
            if (element is Message m)
            {
                MessageAvailable?.Invoke(this, new EventArgs<Message>(m));
            }

            Trace.WriteLine(element);
        }
    }
}
