using Mimir.backend.postgres;
using Npgsql;

namespace Mimir.Server.Postgres
{
    public class PostgresGetForiegnAccount
    {

        static internal async Task<PostgresManager.ForeignAccountStruct[]> GetForeignAccountData(string[] uidArray)
        {
            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            PostgresManager.ForeignAccountStruct[] allForeignAccounts = new PostgresManager.ForeignAccountStruct[uidArray.Count()];
            int i = 0;
            foreach (string uid in uidArray)
            {
                await using var command = new NpgsqlCommand("SELECT username, pfp FROM public.userprofile WHERE uid = ($1)", conn)
                {
                    Parameters =
                {
                    new() { Value = int.Parse(uid) }
                }
                };

                await using var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    allForeignAccounts[i].username = reader.GetString(0);
                    allForeignAccounts[i].pfp = reader.GetString(1);
                }

                while (reader.IsClosed == false)
                {
                    await reader.CloseAsync();
                }
                i++;

            }
            

            conn.CloseAsync();

            return allForeignAccounts;
        }

    }
}
