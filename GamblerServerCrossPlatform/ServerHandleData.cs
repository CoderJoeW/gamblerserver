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
            packetListener.Add((int)ClientPackages.CCheckAccountExist, HandleCheckAccountExist);
            packetListener.Add((int)ClientPackages.CCreateAccount, HandleCreateAccount);
            packetListener.Add((int)ClientPackages.CLoadAccountInfo, HandleLoadAccountInfo);
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

        private static void HandleCheckAccountExist(int connectionID, byte[] data) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packageID = buffer.ReadInteger();
            string msg = buffer.ReadString();

            Console.WriteLine("Connection id {0} is checking if thier account exists", connectionID);

            PlayerModel player_info = (PlayerModel)Lib.FromJSON(msg);

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

            PlayerModel player_model = (PlayerModel)Lib.FromJSON(msg);


        }

        private static void HandleLoadAccountInfo(int connectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packageID = buffer.ReadInteger();
            string msg = buffer.ReadString();

            Console.WriteLine("Connection id {0} is loading thier account data", connectionID);

            PlayerModel player_info = (PlayerModel)Lib.FromJSON(msg);

            PlayerModel account_info = Database.LoadAccountInfo(player_info);

            ServerTCP.PACKET_AccountLoaded(connectionID, account_info);
        }
    }
}
