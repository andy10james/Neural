using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NL.Common;

namespace NL.Server.Controllers {
    class DeliveryController : IRemoteController {

        public override String GetName() {
            return "Delivery Controller";
        }

        protected override void DefaultAction(CommandPattern command, TcpClient client)
        {
            throw new NotImplementedException();
        }
    }
}
