using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mimir.backend.postgres
{
    internal class PostgresRemoveLike
    {
        static public async Task RemoveLike(int puid, int uid)
        {
            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            await using var command = new NpgsqlCommand("SELECT (likes, likeamt) FROM public.posts WHERE puid = ($1)", conn)
            {
                Parameters =
                {
                    new () { Value = puid }
                }
            };

            string _likeString = "";
            int _likeAmt = 0;

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                _likeString = reader.GetString(0);
                _likeAmt = reader.GetInt32(1);
            }

            while (reader.IsClosed == false)
            {
                await reader.CloseAsync();
            }

            _likeString.Replace(uid.ToString(), "");
            _likeAmt--;

            await using var command2 = new NpgsqlCommand("UPDATE public.posts SET likes = ($1), likeamt = ($2) WHERE puid = ($3)", conn)
            {
                Parameters =
                {
                    new () { Value = _likeString },
                    new () { Value = _likeAmt },
                    new () { Value = puid }
                }
            };

            await command2.ExecuteNonQueryAsync();

            await conn.CloseAsync();
        }

    }
}
