using Mimir.backend.postgres;
using Npgsql;

namespace Mimir.Server.Postgres
{
    public class PostgresUpdateUserDesc
    {

        public static async Task<bool> UpdateUserAccount(string wantedChange, string text, string uid) {

            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            NpgsqlCommand cmd2 = new NpgsqlCommand("UPDATE public.userprofile SET profiledesc = ($1) WHERE uid = ($2)", conn)
            {
            Parameters =
              {
                 new() { Value = text },
                 new() { Value = int.Parse(uid) }
              }
            };
            await cmd2.ExecuteNonQueryAsync();
            await conn.CloseAsync();
            return true ;
        }
    }
}
