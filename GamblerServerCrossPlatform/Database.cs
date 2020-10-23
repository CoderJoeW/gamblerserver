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
                    string exists = "";

                    while (reader.Read())
                    {
                        exists = (string)reader["result"];
                    }

                    reader.Close();

                    conn.Close();

                    return (exists == "0" ? false : true);
                }
            }
        }

        public static void CreateAccount(PlayerModel player_model)
        {
            string query = "INSERT INTO Player SET Id='" + player_model.Id +"', Username='" + player_model.Username +"', Email='" + player_model.Email +"', PaypalAddress='" + player_model.PaypalAddress +"'";

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
                        player_info.Balance = (float)reader["Balance"];
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
