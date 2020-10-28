using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GamblerServerCrossPlatform.Models;
using MySql.Data.MySqlClient;

namespace GamblerServerCrossPlatform
{
    class Database {
        public static bool CheckAccountExist(PlayerModel player_info)
        {
            string query = "SELECT COUNT(1) as result FROM Player WHERE Id='" + player_info.Id + "'";

            MySqlConnection conn = MySQL.GetConn();
            conn.Open();

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    Int64 exists = 0;

                    while (reader.Read())
                    {
                        exists = (Int64)reader["result"];
                    }

                    reader.Close();

                    conn.Close();

                    return (exists == 0 ? false : true);
                }
            }
        }

        public static void CreateAccount(PlayerModel player_model)
        {
            string query = "INSERT INTO Player SET Id='" + player_model.Id +"', Username='" + player_model.Username +"', Email='" + player_model.Email +"', PaypalAddress='" + player_model.PaypalAddress +"'";

            ExecuteNonQuery(query);
        }

        public static int GetLobby(string game_name, float bet)
        {
            string query = "SELECT Id FROM Lobbies WHERE Started='0' AND Game='" + game_name + "' AND Bet='" + bet + "' ORDER BY Id ASC LIMIT 1";

            MySqlConnection conn = MySQL.GetConn();
            conn.Open();

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    int id = 0;

                    while (reader.Read())
                    {
                        id = (int)reader["Id"];
                    }

                    reader.Close();

                    conn.Close();

                    return id;
                }
            }
        } 

        public static LobbyModel GetLobbyModel(int lobby_id)
        {
            string query = "SELECT * FROM Lobbies WHERE id='" + lobby_id + "' LIMIT 1";

            MySqlConnection conn = MySQL.GetConn();
            conn.Open();

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    LobbyModel lobby_info = new LobbyModel();

                    while (reader.Read())
                    {
                        lobby_info.Id = (int)reader["Id"];
                        lobby_info.Player1Id = (int)reader["Player1Id"];
                        lobby_info.Player1ConID = (int)reader["Player1ConID"];
                        lobby_info.Player2Id = (int)reader["Player2Id"];
                        lobby_info.Player2ConID = (int)reader["Player2ConID"];
                        lobby_info.Bet = int.Parse((string)reader["Bet"]);
                        lobby_info.Game = (string)reader["Game"];
                    }

                    reader.Close();

                    conn.Close();

                    return lobby_info;
                }
            }
        }

        public static void CreateLobby(int connectionID,PlayerModel player_info,float bet_amount,string game_name)
        {
            string query = "INSERT INTO Lobbies SET Player1Id='"  + player_info.Id +  "', Player1ConID='"  + connectionID +  "', Bet='"  + bet_amount +  "', Game='"  + game_name +  "'";

            ExecuteNonQuery(query);
        }

        public static void JoinLobby(int lobby_id,PlayerModel player_info,int connectionID)
        {
            string query = "UPDATE Lobbies SET Player2Id='" + player_info.Id + "', Player2ConID='" + connectionID + "' WHERE Id='" + lobby_id + "'";

            ExecuteNonQuery(query);
        }

        public static void LeaveLobby(int connectionID)
        {
            string query = "DELETE FROM Lobbies WHERE Player1ConID='" + connectionID + "'";

            ExecuteNonQuery(query);
        }

        public static void StartLobby(int lobby_id)
        {
            string query = "UPDATE Lobbies SET Started='1' WHERE Id='" + lobby_id + "'";

            ExecuteNonQuery(query);
        }

        public static PlayerModel LoadAccountInfo(PlayerModel player_info)
        {
            string query = "SELECT * FROM Player WHERE Id='" + player_info.Id + "'";

            MySqlConnection conn = MySQL.GetConn();
            conn.Open();

            using(MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                using(MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        player_info.Id = (string)reader["Id"];
                        player_info.Username = (string)reader["Username"];
                        player_info.Email = (string)reader["Email"];
                        player_info.PaypalAddress = (string)reader["PaypalAddress"];
                        player_info.Balance = float.Parse((string)reader["Balance"]);
                    }

                    reader.Close();
                    conn.Close();

                    return player_info;
                }
            }
        }

        public static void LogError(string errorMessage) {
            String timeStamp = lib.GetTimestamp(DateTime.Now);
            string query = "INSERT INTO error_logs SET error='" + errorMessage + "', timestamp='" + timeStamp + "'";
            
            Console.WriteLine("CUSTOM ERROR MESSAGE: " + errorMessage);

            ExecuteNonQuery(query);
        }

        private static void ExecuteNonQuery(string query)
        {
            MySqlConnection conn = MySQL.GetConn();
            conn.Open();

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Database.LogError(e.Message);
                }
            }

            conn.Close();
        }
    }
}
