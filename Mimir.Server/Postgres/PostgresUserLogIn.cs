using Mimir.backend.postgres;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Mimir.backend.postgres
{
    internal class PostgresUserLogIn
    {
        static private string pgsSettings = "";
        static public async Task<(int, string)> UserLogin()
        {
            Console.WriteLine("Starting Postgres User Login");

            pgsSettings = GetPostgres.GetPostgresSettings();

            int _uid = await LoginInfo();


            if(_uid == 0)
            {
                Console.WriteLine("Username/Password incorrect.");
                return (0, "");
            }
            else
            {
                Console.WriteLine("User ID: " + _uid.ToString());
                string token = await LoginToUID(_uid);
                return (_uid, token);
            }
            //await UserActions();
            
            

            
        }

        static private async Task<int> LoginInfo()
        {
            Console.WriteLine("Username: ");
            string _username = Console.ReadLine();
            Console.WriteLine("Password: ");
            string _password = Console.ReadLine();
            int _uid = 0;
            byte[] _puid = new byte[64];
            byte[] _salt = new byte[64];

            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            await using var command = new NpgsqlCommand("SELECT uid FROM uinfo.users WHERE username = ($1)", conn)
            {
                Parameters =
                {
                    new() { Value = _username }
                }
            };

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                _uid = Int32.Parse(reader.GetValue(0).ToString());
            }

            while (reader.IsClosed == false)
            {
                await reader.CloseAsync();
            }

            if (_uid == 0)
            {
                conn.Close();
                return 0;
            }

            await using var command2 = new NpgsqlCommand("SELECT puid, salt FROM uinfo.wpu WHERE uid = ($1)", conn)
            {
                Parameters =
                {
                    new() { Value = _uid }
                }
            };

            await using NpgsqlDataReader reader2 = await command2.ExecuteReaderAsync();

            while (await reader2.ReadAsync())
            {
                _puid = (byte[])reader2.GetValue(0);
                _salt = (byte[])reader2.GetValue(1);
            }

            while (reader2.IsClosed == false)
            {
                await reader2.CloseAsync();
            }

            await conn.CloseAsync();

            byte[] derivedKey = Rfc2898DeriveBytes.Pbkdf2(_password, _salt, 500000, HashAlgorithmName.SHA3_512, 64);

            string _derivedKeyString = "";
            foreach(byte b in derivedKey)
            {
                _derivedKeyString += b.ToString();
            }

            string _puidString = "";
            foreach(byte b in _puid)
            {
                _puidString += b.ToString();
            }

            if(_derivedKeyString != _puidString)
            {
                return 0;
            }

            return _uid;
        }

        static private async Task<string> LoginToUID(int uid)
        {
            string token = "";

            Random rand = new Random();
            var arr = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            for(int i = 0; i <= 32; i++)
            {
                token += arr[rand.Next(arr.Length)];
            }

            Console.WriteLine(token);
            Console.WriteLine(uid);
            DateTime curDate = DateTime.Now;

            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            await using var command2 = new NpgsqlCommand("UPDATE uinfo.users SET curtoken = ($1), lastlogin = ($2) WHERE uid = ($3)", conn)
            {
                Parameters =
                {
                    new() { Value = token },
                    new() { Value = curDate },
                    new() { Value = uid }
                }
            };
            await command2.ExecuteNonQueryAsync();
            await conn.CloseAsync();

            return token;
        }
    }
}
