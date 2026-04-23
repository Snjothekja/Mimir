using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mimir.backend.postgres
{
    internal class PostgresAddComment
    {
        // {User ID, Comment ID, Comment Text, Comment on Comment ID (To comment on a comment)}  

        public static async Task AddComment(int uid, string text, int postID, int commentCommentID)
        {
            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            await using var command = new NpgsqlCommand("SELECT (comments, commentAmt) FROM public.posts WHERE postid = ($1)", conn)
            {
                Parameters =
                {
                    new () { Value = postID }
                }
            };

            string _currentCommentString = "";
            string[] _commentArray = new string[0];
            int _commentCount = 0;
            int _commentId = 0;

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                _currentCommentString = reader.GetString(0);
                _commentCount = reader.GetInt32(1);
            }

            while (reader.IsClosed == false)
            {
                reader.Close();
            }

            _commentArray = _currentCommentString.Split(';');

            _commentId = Int32.Parse((_commentArray[_commentArray.Length - 1].Split(",")).ElementAt(3)) + 1;
            
            string _commmentArrayPart = $"{uid},{_commentId},{text},{commentCommentID};";

            _currentCommentString += _commmentArrayPart;

            await using var command2 = new NpgsqlCommand("UPDATE public.posts SET (comments = ($1), commentamt = ($2)) WHERE postid = ($3)", conn)
            {
                Parameters =
                {
                    new () { Value = _currentCommentString },
                    new NpgsqlParameter() { Value = _commentId },
                    new () { Value = postID }
                }
            };

            await command.ExecuteNonQueryAsync();

            await conn.CloseAsync(); 
        }

    }
}
