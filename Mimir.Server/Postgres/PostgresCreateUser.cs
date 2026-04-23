using Npgsql;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;

namespace Mimir.backend.postgres
{
    internal class PostgresCreateUser
    {
        public static async Task<bool> CreateUser(string username, string password)
        {
            bool avaliable = await CheckUsernameAvaliable(username);

            if(avaliable == false)
            {
                return false;
            }

            Random _ran = new Random();
            byte[] salt = new byte[64];
            _ran.NextBytes(salt);
            byte[] derivedKey = Rfc2898DeriveBytes.Pbkdf2(password, salt, 500000, HashAlgorithmName.SHA3_512, 64);

            await AddUserToDB(username, salt, derivedKey);

            return true;
        }

        static public async Task AddUserToDB(string username, byte[] salt, byte[] hashedPass)
        {

            int _uid = 0;

            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();
            await using var transaction = await conn.BeginTransactionAsync();

            await using var command = new NpgsqlCommand("INSERT INTO uinfo.wpu (puid, salt) VALUES (($1), ($2))", conn, transaction)
            {
                Parameters =
                {
                    new() { Value = hashedPass },
                    new() { Value = salt }
                }
            };
            command.ExecuteNonQuery();

            using var command2 = new NpgsqlCommand("SELECT uid FROM uinfo.wpu WHERE puid = ($1)", conn, transaction)
            {
                Parameters =
                {
                    new() { Value = hashedPass },
                }
            };
            var reader2 = command2.ExecuteReader();

            while (reader2.Read())
            {
                _uid = Int32.Parse(reader2.GetValue(0).ToString());
            }

            while(reader2.IsClosed == false)
            {
                await reader2.CloseAsync();
            }

            using var command3 = new NpgsqlCommand("INSERT INTO uinfo.users (uid, username) VALUES (($1), ($2))", conn, transaction)
            {
                Parameters =
                {
                    new() { Value = _uid },
                    new() { Value = username }
                }
            };

            command3.ExecuteNonQuery();

            //using var command4 = new NpgsqlCommand(HeaderEncodingSelector setval(pg_get_serial_seq);

            await transaction.CommitAsync();
            await conn.CloseAsync();
        }

        static private async Task<bool> CheckUsernameAvaliable(string name)
        {
            string _nameAvaliable = "";
            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            using var command = new NpgsqlCommand("SELECT username FROM uinfo.users WHERE username = ($1)", conn)
            {
                Parameters =
                {
                    new() { Value = name }
                }
            };
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                _nameAvaliable = reader.GetValue(0).ToString();
            }

            while (reader.IsClosed == false)
            {
                await reader.CloseAsync();
            }

            await conn.CloseAsync();

            if(_nameAvaliable == name)
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
