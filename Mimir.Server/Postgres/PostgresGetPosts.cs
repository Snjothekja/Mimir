using Mimir.backend.postgres;
using Npgsql;

namespace Mimir.Server.Postgres
{
    public class PostgresGetPosts
    {

        public async Task<int[]> GetPosts(int uid)
        {
            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            await using var command = new NpgsqlCommand("SELECT following FROM public.userinfo WHERE puid = ($1)", conn)
            {
                Parameters =
                {
                    new () { Value = uid }
                }
            };


            return null;
        }

    }
}
