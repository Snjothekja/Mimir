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
            Console.WriteLine(wantedTime.ToString());
            await using var command = new NpgsqlCommand("SELECT * FROM public.posts WHERE posteruid = ($1) AND postdate <= ($2) ORDER BY postdate DESC FETCH FIRST 5 ROWS ONLY", conn)
            {
                Parameters =
                {
                    new () { Value = uid },
                    new NpgsqlParameter() { Value = wantedTime.ToUniversalTime() }
                }
            };

            PostgresManager.PostStruct[] psarray = new PostgresManager.PostStruct[4];

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                for (int i = 0; i < 4; i++)
                {
                    try
                    {
                        reader.GetFieldValue<int>(0);
                        
                    }
                    catch
                    {
                        Console.WriteLine("Stopped at Row " + i.ToString() + ": When getting posts");
                        break;
                    }
                        PostgresManager.PostStruct ps = new PostgresManager.PostStruct()
                        {
                            postid = reader.GetFieldValue<int>(0),
                            posterUID = reader.GetFieldValue<int>(1),
                            postText = reader.GetFieldValue<string>(2),
                            images = reader.GetFieldValue<string>(3),
                            likeAmt = reader.GetFieldValue<int>(7),
                            repostAmt = reader.GetFieldValue<int>(8),
                            commentAmt = reader.GetFieldValue<int>(9),
                            datePosted = reader.GetFieldValue<DateTime>(10)
                        };
                    Console.WriteLine(ps.postText); 
                    if (i > 0 && ps.postid == psarray[i - 1].postid)
                    {
                        break;
                    }
                    psarray[i] = ps;
                    reader.Read();
                        
                }

                await conn.CloseAsync();
                return psarray;
            }

            return psarray;
        }
    }
}
