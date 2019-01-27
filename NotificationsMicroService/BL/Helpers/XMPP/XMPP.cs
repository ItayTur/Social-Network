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

        private IChannel _channel = null;

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

            _client.Password = _password;
            _client.Username = _userName;

            ConnectAppUser();

        }

        /// <summary>
        /// Get a connection to ejabber.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task ConnectAppUser()
        {            
            _channel = await _client.ConnectAsync();
            await _client.SendAsync(new Presence(Show.Chat));
        }

        /// <summary>
        /// Register a new user.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task Register(string userName, string password)
        {
            XmppClient client;

            var pipelineInitializerAction = new Action<IChannelPipeline>(pipeline =>
            {
                pipeline.AddFirst(new MyLoggingHandler());
            });

            client = new XmppClient(pipelineInitializerAction)
            {
                Tls = false,
                XmppDomain = _domain,
                Resource = "",
                HostnameResolver = new StaticNameResolver(IPAddress.Parse(_ipAddress), _port)
            };


            client.RegistrationHandler = new RegisterAccountHandler(client);
            client.Username = userName;
            client.Password = password;
            await client.ConnectAsync();
            await client.DisconnectAsync();
        }

        /// <summary>
        /// Gets the users roster.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetRoster()
        {
            var res = await _client.RequestRosterAsync();
            var roster = res.Query as Roster; 
            return roster.GetRoster().Select(r => r.Jid.User);
        }

        /// <summary>
        /// Removes a registered user.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task RemoveUser(string userName)
        {
            //JGJG: Don't know how to implement it yet
            return;
         
        }

        /// <summary>
        /// Sends a message to a specific user.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task SendPrivateMessage(string message, string to)
        {           
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
