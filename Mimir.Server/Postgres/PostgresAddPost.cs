using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mimir.backend.postgres
{
    internal class PostgresAddPost
    {

        static public async Task AddPost(string postText, int uid)
        {
            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            await using var command = new NpgsqlCommand("INSERT INTO public.posts (ptext, uid) VALUES (($1), ($2))", conn)
            {
                Parameters =
                {
                    new () { Value = postText },
                    new () { Value = uid }
                }
            };

            await command.ExecuteNonQueryAsync();

            await conn.CloseAsync();
        }

    }
}
