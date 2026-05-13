using Mimir.backend.postgres;
using Npgsql;
using System.ComponentModel;

namespace Mimir.Server.Postgres
{
    public class PostgresUpdateUserPFP
    {

        public static async Task<bool> UpdatePFP(int uid, string path)
        {
            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            await using var command = new NpgsqlCommand("UPDATE public.userprofile SET pfp = ($1) WHERE uid = ($2)", conn)
            {
                Parameters =
                {
                    new() { Value = path },
                    new() { Value = uid }
                }
            };

            await command.ExecuteNonQueryAsync();
            await conn.CloseAsync();
            return true;
        }
    }
}
