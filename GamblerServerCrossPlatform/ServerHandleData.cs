using GamblerServerCrossPlatform.Helpers;
using GamblerServerCrossPlatform.Models;
using System;
using System.Collections.Generic;

namespace GamblerServerCrossPlatform
{
    public class ServerHandleData {
        public delegate void Packet_(int connectionID, byte[] data);
        public static Dictionary<int, Packet_> packetListener;
        private static int pLength;

        public static void InitializePacketListener() {
            packetListener = new Dictionary<int, Packet_>();
            packetListener.Add((int)ClientPackages.CAbortConnection, HandleAbortConnection);
            packetListener.Add((int)ClientPackages.CCheckAccountExist, HandleCheckAccountExist);
            packetListener.Add((int)ClientPackages.CCreateAccount, HandleCreateAccount);
            packetListener.Add((int)ClientPackages.CLoadAccountInfo, HandleLoadAccountInfo);
            packetListener.Add((int)ClientPackages.CJoinLobby, HandleJoinLobby);
            packetListener.Add((int)ClientPackages.CLeaveLobby, HandleLeaveLobby);
        }

        public static void HandleData(int connectionID, byte[] data) {
            //Copying our packet info into a temporary array to edit/peek
            byte[] buffer = (byte[])data.Clone();

            //Check if connected player has an instance of the byte buffer
            if (ServerTCP.clientObjects[connectionID].buffer == null) {
                //If there is no instance create a new instance
                ServerTCP.clientObjects[connectionID].buffer = new ByteBuffer();
            }

            //Read package from player
            ServerTCP.clientObjects[connectionID].buffer.WriteBytes(buffer);

            //Check if recieved package is empty, if so dont continue executing
            if (ServerTCP.clientObjects[connectionID].buffer.Count() == 0) {
                ServerTCP.clientObjects[connectionID].buffer.Clear();
                return;
            }

            //Check if package contains info
            if (ServerTCP.clientObjects[connectionID].buffer.Length() >= 4) {
                //if so read full package length
                pLength = ServerTCP.clientObjects[connectionID].buffer.ReadInteger(false);

                if (pLength <= 0) {
                    //If there is no package or package is invalid close this method
                    ServerTCP.clientObjects[connectionID].buffer.Clear();
                    return;
                }
            }

            while (pLength > 0 & pLength <= ServerTCP.clientObjects[connectionID].buffer.Length() - 4) {
                if (pLength <= ServerTCP.clientObjects[connectionID].buffer.Length() - 4) {
                    ServerTCP.clientObjects[connectionID].buffer.ReadInteger();
                    data = ServerTCP.clientObjects[connectionID].buffer.ReadBytes(pLength);
                    HandleDataPackages(connectionID, data);
                }

                pLength = 0;

                if (ServerTCP.clientObjects[connectionID].buffer.Length() >= 4) {
                    pLength = ServerTCP.clientObjects[connectionID].buffer.ReadInteger(false);

                    if (pLength <= 0) {
                        //If there is no package or package is invalid close this method
                        ServerTCP.clientObjects[connectionID].buffer.Clear();
                        return;
                    }
                }

                if (pLength <= 1) {
                    ServerTCP.clientObjects[connectionID].buffer.Clear();
                }
            }
        }

        private static void HandleDataPackages(int connectionID, byte[] data) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packageID = buffer.ReadInteger();

            if (packetListener.TryGetValue(packageID, out Packet_ packet)) {
                packet.Invoke(connectionID, data);
            }
        }

        private static void HandleAbortConnection(int connectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packageID = buffer.ReadInteger();
            string msg = buffer.ReadString();

            Console.WriteLine("Connection id {0} has aborted thier connection", connectionID);

            PlayerModel player_info = Lib.FromJSON<PlayerModel>(msg);

            //Check if the player was in any lobbies
            bool in_lobby = Database.IsInLobby(player_info);

            if (in_lobby)
            {
                LobbyModel lobby_info = Database.GetLobbyModel(player_info);

                //If player is player1 we need to send disconnected packet to other player
                if(lobby_info.Player1Id == player_info.Id)
                {
                    if (!String.IsNullOrEmpty(lobby_info.Player2Id))
                    {
                        ServerTCP.PACKET_PlayerDisconnected(lobby_info.Player2ConID,PlayerType.Host);
                    }
                }
                else
                {
                    //if we are not player1 just leave the match
                    //And notify the host we left
                    ServerTCP.PACKET_PlayerDisconnected(lobby_info.Player1ConID,PlayerType.Player);
                }
            }

            Console.WriteLine("Connection id {0} has been successfull disconnected from the server", connectionID);
        }

        private static void HandleCheckAccountExist(int connectionID, byte[] data) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packageID = buffer.ReadInteger();
            string msg = buffer.ReadString();

            Console.WriteLine("Connection id {0} is checking if thier account exists", connectionID);

            PlayerModel player_info = Lib.FromJSON<PlayerModel>(msg);

            bool account_exists = Database.CheckAccountExist(player_info);

            if (account_exists)
            {
                ServerTCP.PACKET_AccountExist(connectionID);
            }
            else
            {
                ServerTCP.PACKET_AccountDoesNotExist(connectionID);
            }
        }

        private static void HandleCreateAccount(int connectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packageID = buffer.ReadInteger();
            string msg = buffer.ReadString();

            Console.WriteLine("Connection id {0} is creating a new account", connectionID);

            PlayerModel player_model = Lib.FromJSON<PlayerModel>(msg);

            Database.CreateAccount(player_model);

            ServerTCP.PACKET_AccountCreated(connectionID);
        }

        private static void HandleLoadAccountInfo(int connectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packageID = buffer.ReadInteger();
            string msg = buffer.ReadString();

            Console.WriteLine("Connection id {0} is loading thier account data", connectionID);

            PlayerModel player_info = Lib.FromJSON<PlayerModel>(msg);

            PlayerModel account_info = Database.LoadAccountInfo(player_info);

            ServerTCP.PACKET_AccountLoaded(connectionID, account_info);
        }

        private static void HandleJoinLobby(int connectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packageID = buffer.ReadInteger();
            string msg = buffer.ReadString();
            float bet = buffer.ReadFloat();
            string game_name = buffer.ReadString();


            PlayerModel player_info = Lib.FromJSON<PlayerModel>(msg);

            PlayerModel actual_info = Database.LoadAccountInfo(player_info);

            if(actual_info.Balance < bet)
            {
                ServerTCP.PACKET_LobbyJoinError(connectionID, "Insufficient Balance");
                return;
            }

            int lobby_id = Database.GetLobby(game_name,bet);

            if(lobby_id == 0)
            {
                //Lobby does not exist creating new lobby
                Database.CreateLobby(connectionID, actual_info, bet, game_name);
            }
            else
            {
                //Lobby already exist joining lobby
                Database.JoinLobby(lobby_id, actual_info, connectionID);

                LobbyModel lobby_info = Database.GetLobbyModel(lobby_id);

                ServerTCP.PACKET_LobbyStart(lobby_info.Player1ConID, lobby_info.Player2ConID, game_name,lobby_id);

                Database.StartLobby(lobby_id);
            }
        }

        private static void HandleLeaveLobby(int connectionID,byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packageID = buffer.ReadInteger();

            Database.LeaveLobby(connectionID);

        }
    }
}
