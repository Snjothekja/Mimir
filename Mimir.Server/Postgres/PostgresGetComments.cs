using Mimir.backend.postgres;
using Npgsql;
using System.Text.Json;

namespace Mimir.Server.Postgres
{
    public class PostgresGetComments
    {

        static internal async Task<List<PostgresManager.Comments>> GetComments(int postid)
        {
            Console.WriteLine("Getting comments from " + postid.ToString() + " Post ID");
            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            using var command = new NpgsqlCommand("SELECT commentsreplies FROM public.posts WHERE postid=($1)", conn)
            {
                Parameters =
                {
                    new () { Value = postid }
                }
            };

            string _currentCommentString = "";

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                _currentCommentString = (string)reader.GetValue(0);
            }

            while (reader.IsClosed == false)
            {
                reader.Close();
            }
       

            Console.WriteLine("Got this comment string: " + _currentCommentString);
            try
            {
                JsonSerializerOptions _serializeOptions = new JsonSerializerOptions { AllowTrailingCommas = true };
                List<PostgresManager.Comments> _posts = JsonSerializer.Deserialize<List<PostgresManager.Comments>>(_currentCommentString, _serializeOptions);
                Console.WriteLine(_posts[0].commentText);

                return _posts;
            }
            catch
            {
                Console.WriteLine("Cannot Make Comments Array");
                return new List<PostgresManager.Comments>();
            }
            

            

        }
    }
}
