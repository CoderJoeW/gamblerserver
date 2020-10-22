using GamblerServerCrossPlatform.Helpers;
using GamblerServerCrossPlatform.Models;
using System;
using System.Net;
using System.Net.Sockets;

namespace GamblerServerCrossPlatform
{
    class ServerTCP {
        private static TcpListener serverSocket;

        public static ClientObject[] clientObjects;

        public static void InitializeServer() {
            InitializeMySQLServer();
            InitializeClientObjects();
            InitializeServerSocket();
        }

        private static void InitializeMySQLServer() {
            MySQL.mySQLSettings.user = "gambler";
            MySQL.mySQLSettings.password = "Glitchking12!#";
            MySQL.mySQLSettings.server = "35.225.52.164";
            MySQL.mySQLSettings.database = "Gambler";
        }

        private static void InitializeServerSocket() {
            serverSocket = new TcpListener(IPAddress.Any, Constants.PORT);
            serverSocket.Start();
            serverSocket.BeginAcceptTcpClient(new AsyncCallback(ClientConnectCallback), null);
        }

        private static void InitializeClientObjects() {
            clientObjects = new ClientObject[Constants.MAX_PLAYERS];

            for (int i = 1; i < Constants.MAX_PLAYERS; i++) {
                clientObjects[i] = new ClientObject(null, 0);
            }
        }

        private static void ClientConnectCallback(IAsyncResult result) {
            TcpClient tempClient = serverSocket.EndAcceptTcpClient(result);
            serverSocket.BeginAcceptTcpClient(new AsyncCallback(ClientConnectCallback), null);

            for (int i = 1; i < Constants.MAX_PLAYERS; i++) {
                if(clientObjects[i].socket == null) {
                    clientObjects[i] = new ClientObject(tempClient, i);

                    ServerTCP.PACKET_Connected(i, "Welcome to the server");

                    return;
                }
            }
        }

        public static void SendDataTo(int connectionID, byte[] data) {
            ByteBuffer buffer = new ByteBuffer();

            buffer.WriteInteger((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
            buffer.WriteBytes(data);
            clientObjects[connectionID].myStream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
            buffer.Dispose();
        }

        public static void PACKET_Connected(int connectionID,string msg) {
            ByteBuffer buffer = new ByteBuffer();
            //add package id
            buffer.WriteInteger((int)ServerPackages.SConnected);

            //send info
            buffer.WriteString(msg);

            SendDataTo(connectionID, buffer.ToArray());
        }

        public static void PACKET_AccountExist(int connectionID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SAccountExists);
            SendDataTo(connectionID, buffer.ToArray());
        }

        public static void PACKET_AccountDoesNotExist(int connectionID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SAccountDoesNotExist);
            SendDataTo(connectionID, buffer.ToArray());
        }

        public static void PACKET_AccountCreated(int connectionID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SAccountCreated);
            SendDataTo(connectionID, buffer.ToArray());
        }

        public static void PACKET_AccountLoaded(int connectionID,PlayerModel player_info)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SAccountLoaded);
            buffer.WriteString(Lib.ToJSON(player_info));
            SendDataTo(connectionID, buffer.ToArray());
        }
    }
}
