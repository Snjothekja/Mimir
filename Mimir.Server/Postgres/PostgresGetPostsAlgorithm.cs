using Mimir.backend.postgres;
using Npgsql;

namespace Mimir.Server.Postgres
{
    public class PostgresGetPostsAlgorithm
    {
        internal static async Task<PostgresManager.PostStruct[]> PostAlgorithm(int uid, DateTime wantedTime)
        {

            List<PostgresManager.PostStruct> allPosts = new List<PostgresManager.PostStruct>();
            NpgsqlCommand command = new NpgsqlCommand();

            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            command = new NpgsqlCommand("SELECT followers FROM public.userprofile WHERE uid = ($1)", conn)
            {
                Parameters =
                {
                    new () { Value = uid },
                    new NpgsqlParameter() { Value = wantedTime.ToUniversalTime() }
                }
            };

            NpgsqlDataReader reader = command.ExecuteReader();

            string followers = "";

            while (reader.Read())
            {
                followers = reader.GetString(0);
            }

            while (reader.IsClosed == false)
            {
                reader.Close();
            }
            
            List<string> followersArray = followers.Split(",").ToList<string>();
            Console.WriteLine("Followers: " + followersArray.Count);

            if(followersArray.Count > 1)
            {
                for (int i = 0; i < followersArray.Count; i++)
                {
                    Random ran = new Random();
                    string getFollower = followersArray[ran.Next(followersArray.Count - 1)];

                    command = new NpgsqlCommand("SELECT * FROM public.posts WHERE posteruid = ($1) AND postdate <= ($2) ORDER BY postdate DESC FETCH FIRST 2 ROWS ONLY", conn)
                    {
                        Parameters =
                        {
                            new () { Value = int.Parse(getFollower) },
                            new NpgsqlParameter() { Value = wantedTime.ToUniversalTime() }
                        }
                    };

                    reader = command.ExecuteReader();

                    try
                    {
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
                        //Console.WriteLine(ps.postText);
                        allPosts.Add(ps);
                        reader.Read();
                    }
                    catch
                    {
                        //Console.WriteLine("Stopped at Row " + i.ToString() + ": When getting posts");
                        break;
                    }            

                    if (i > 2)
                    {
                        break;
                    }

                }
            }

            reader.Close();

            Console.WriteLine(wantedTime.ToUniversalTime().ToString());

            command = new NpgsqlCommand("SELECT * FROM public.posts WHERE postdate <= ($1) ORDER BY postdate DESC FETCH FIRST 2 ROWS ONLY", conn)
            {
                Parameters =
                {
                    new NpgsqlParameter() { Value = wantedTime.ToUniversalTime() }
                }
            };

            reader = command.ExecuteReader();

            for (int i = 0; i < 2; i++)
            {
                reader.Read();
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
                    likes = reader.GetFieldValue<string>(4),
                    reposts = reader.GetFieldValue<string>(5),
                    likeAmt = reader.GetFieldValue<int>(7),
                    repostAmt = reader.GetFieldValue<int>(8),
                    commentAmt = reader.GetFieldValue<int>(9),
                    datePosted = reader.GetFieldValue<DateTime>(10)
                };
                //Console.WriteLine(ps.postText);
                allPosts.Add(ps);
                

            }


            Random rng = new Random();

            int n = allPosts.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                PostgresManager.PostStruct value = allPosts[k];
                allPosts[k] = allPosts[n];
                allPosts[n] = value;
            }
            reader.Close();
            conn.CloseAsync();
            
            return allPosts.ToArray();

        }


    }
}
