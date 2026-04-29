using Mimir.backend.postgres;
using Npgsql;

namespace Mimir.Server.Postgres
{
    public class PostgresAddFollower
    {

        static public async Task AddFollower(int uid, int followID)
        {
            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            await using var command = new NpgsqlCommand("SELECT following FROM public.userinfo WHERE uid = ($1)", conn)
            {
                Parameters =
                {
                    new () { Value = uid }
                }
            };

            string _followerString = "";

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                _followerString = reader.GetString(0);
            }

            while (reader.IsClosed == false)
            {
                await reader.CloseAsync();
            }

            _followerString += "," + followID.ToString();

            await using var command2 = new NpgsqlCommand("UPDATE public.userinfo SET following = ($1) WHERE uid = ($2)", conn)
            {
                Parameters =
                {
                    new () { Value = _followerString },
                    new NpgsqlParameter() { Value =  uid }
                }
            };

            await command2.ExecuteNonQueryAsync();

            await conn.CloseAsync();
        }

    }
}
