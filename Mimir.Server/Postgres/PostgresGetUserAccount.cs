using Microsoft.Extensions.ObjectPool;
using Mimir.backend.postgres;
using Npgsql;
using System.Text;

namespace Mimir.Server.Postgres
{
    public class PostgresGetUserAccount
    {

        public static async Task<string[]> GetUserAccountDetails(int uid)
        {
            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            await using var command = new NpgsqlCommand("SELECT username, pfp, profiledesc, followers, following FROM public.userprofile WHERE uid = ($1)", conn)
            {
                Parameters =
                {
                    new() { Value = uid }
                }
            };

            await using var reader = await command.ExecuteReaderAsync();
            string[] _accountDetails = new string[6];

            while (reader.Read())
            {
                _accountDetails[0] = reader.GetString(0);
                _accountDetails[1] = reader.GetString(1);
                _accountDetails[2] = reader.GetString(2);
                _accountDetails[3] = reader.GetString(3);
                _accountDetails[4] = reader.GetString(4);
            }

            while (reader.IsClosed == false)
            {
                await reader.CloseAsync();
            }
            await conn.CloseAsync();
            return _accountDetails;
        }

    }
}
