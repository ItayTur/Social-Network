using Matrix;
using Matrix.Xmpp.Client;
using Matrix.Xmpp.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Helpers.XMPP
{
    class RegisterAccountHandler : IRegister
    {
        private XmppClient xmppClient;
        public RegisterAccountHandler(XmppClient xmppClient)
        {
            this.xmppClient = xmppClient;
        }

        public bool RegisterNewAccount => true;

        public async Task<Register> RegisterAsync(Register register)
        {
            register.Username = xmppClient.Username;
            register.Password = xmppClient.Password;
            return register;
        }
    }
}
