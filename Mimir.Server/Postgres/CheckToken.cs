using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Transactions;

namespace Mimir.backend.postgres
{
    internal class CheckToken
    {

        static public bool CheckUserToken(string token, int uid) 
        {
            string _neededToken = "";
            DateOnly dateOnly = new DateOnly();

            using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            conn.Open();

            using var command = new NpgsqlCommand("SELECT curtoken, lastlogin FROM uinfo.users WHERE uid = ($1)", conn)
            {
                Parameters =
                {
                    new() { Value = uid }
                }
            };
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                _neededToken = reader.GetValue(0).ToString();
                dateOnly = (DateOnly)reader.GetValue(1);
            }

            if(_neededToken != token || dateOnly.Month < DateTime.Now.Month || dateOnly.Day < DateTime.Now.Day - 4)
            {
                Console.WriteLine("Check token failed");
                if (_neededToken != token) { Console.WriteLine("Incorrect token, wanted: '" + _neededToken.ToString() + "' But instead got: '" + token.ToString() + "'"); }
                if (dateOnly.Month + dateOnly.Day <= DateTime.Now.Month + DateTime.Now.Day - 4) { Console.WriteLine("Outdated Token: " + (dateOnly.Month + dateOnly.Day).ToString() + " Current Date: " + (DateTime.Now.Month + DateTime.Now.Day - 4).ToString()); }
                return false;
            }
            else
            {
                return true;
            }
            
        }

    }
}
