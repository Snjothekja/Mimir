using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mimir.backend.postgres
{
    internal class PostgresUpdatePost
    {
        static public async Task UpdatePost(int puid, string postText)
        {
            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            await using var command = new NpgsqlCommand("UPDATE public.posts SET ptext = ($1) WHERE puid = ($2)", conn)
            {
                Parameters =
                {
                    new () { Value = postText },
                    new () { Value = puid }
                }
            };

            await command.ExecuteNonQueryAsync();

            await conn.CloseAsync();
        }
    }
}
