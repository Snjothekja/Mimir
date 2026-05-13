using Microsoft.AspNetCore.Connections;
using Mimir.backend.postgres;
using Npgsql;

namespace Mimir.Server.Postgres
{
    public class PostgresGetPosts
    {

        internal static async Task<PostgresManager.PostStruct[]> GetPosts(int uid, DateTime wantedTime)
        {
            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            await using var command = new NpgsqlCommand("SELECT TOP 5 * FROM public.posts WHERE posteruid = ($1) AND postdate <= ($2) ORDER BY postdate DESC", conn)
            {
                Parameters =
                {
                    new () { Value = uid },
                    new NpgsqlParameter() { Value = wantedTime }
                }
            };

            PostgresManager.PostStruct[] psarray = new PostgresManager.PostStruct[4];

            var reader = command.ExecuteReader();

            int i = 0;

            while (reader.Read()) 
            {

                PostgresManager.PostStruct ps = new PostgresManager.PostStruct()
                {
                    postid = reader.GetInt32(0),
                    posterUID = reader.GetInt32(1),
                    postText = reader.GetString(2),
                    images = reader.GetString(3),
                    likeAmt = reader.GetInt32(4),
                    repostAmt = reader.GetInt32(5),
                    commentAmt = reader.GetInt32(6),
                    datePosted = reader.GetDateTime(7),
                    originalPostID = reader.GetInt32(8),
                };

                psarray[i] = ps;
                i++;
            }

            await conn.CloseAsync();
            return psarray;
        }

    }
}
