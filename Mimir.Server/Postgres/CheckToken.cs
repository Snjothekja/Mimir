using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace Mimir.backend.postgres
{
    internal class CheckToken
    {

        static public bool CheckUserToken(string token, int uid) 
        {
            string _neededToken = "";
            using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            conn.Open();

            using var command = new NpgsqlCommand("SELECT curtoken FROM uinfo.users WHERE uid = ($1)", conn)
            {
                Parameters =
                {
                    new() { Value = token }
                }
            };
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                _neededToken = reader.GetValue(0).ToString();
            }

            if(_neededToken != token)
            {
                return false;
            }
            else
            {
                return true;
            }
            
        }

    }
}
