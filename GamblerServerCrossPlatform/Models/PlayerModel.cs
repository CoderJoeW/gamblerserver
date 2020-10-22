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
        public string Id;

        public string Username;

        public string Email;

        public string PaypalAddress;

        public float Balance;
    }
}
