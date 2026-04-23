using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mimir.backend.postgres
{
    internal class PostgresRemovePost
    {

        // This might mess things up, but until it does it deletes all records of the posts existence. Great for privacy/Right for Deletion, we will see in practice.

        static public async Task DeletePost(int puid)
        {
            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            await using var command = new NpgsqlCommand("DELETE FROM public.posts WHERE puid = ($1)", conn)
            {
                Parameters =
                {
                    new () { Value = puid }
                }
            };

            await command.ExecuteNonQueryAsync();

            await conn.CloseAsync();
        }
    }
}
