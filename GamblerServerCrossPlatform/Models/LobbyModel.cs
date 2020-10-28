using System;
using System.Collections.Generic;
using System.Text;

namespace GamblerServerCrossPlatform.Models
{
    [Serializable]
    public class LobbyModel
    {
        public int Id;

        public string Player1Id;

        public int Player1ConID;

        public string Player2Id;

        public int Player2ConID;

        public int Bet;

        public string Game;
    }
}
