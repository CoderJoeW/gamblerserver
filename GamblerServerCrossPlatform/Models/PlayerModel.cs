using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamblerServerCrossPlatform.Models
{
    public class PlayerModel
    {
        public string Id { set; get; }

        public string Username { set; get; }

        public string Email { set; get; }

        public string PaypalAddress { set; get; }

        public float Balance { set; get; }
    }
}
