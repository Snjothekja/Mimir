using Microsoft.VisualBasic;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mimir.backend.postgres
{
    internal class PostgresAddPost
    {

        static public async Task AddPost(string postText, int uid, string imagePath = "")
        {
            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            Console.WriteLine("Adding: " + postText + " With Image: " + imagePath + " For User: " + uid.ToString());

            await using var command = new NpgsqlCommand("INSERT INTO public.posts (ptext, images, posteruid, postdate, likeamt, commentamt, repostamt) VALUES (($1), ($2), ($3), ($4), ($5), ($5), ($5))", conn)
            {
                Parameters =
                {
                    new () { Value = postText },
                    new () { Value = imagePath },
                    new () { Value = uid },
                    new () { Value = DateTime.Now },
                    new NpgsqlParameter() { Value = 0},
                }
            };

            await command.ExecuteNonQueryAsync();

            await conn.CloseAsync();
        }

    }
}
