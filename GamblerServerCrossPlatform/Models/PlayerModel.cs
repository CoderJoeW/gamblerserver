using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamblerServerCrossPlatform.Models
{
    [Serializable]
    public class PlayerModel
    {
        public System.String Id;

        public System.String Username;

        public System.String Email;

        public System.String PaypalAddress;

        public System.String Balance;
    }
}
