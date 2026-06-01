using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using static Mimir.Server.TestScripts.CommentJSON;

namespace Mimir.backend.postgres
{
    /*
    JSON Layout:
                @"{
                  ""CommentText"": ""Test Comment"",
                  ""UID"":1,
                  ""CommentOnCommentID"":0,
                  ""CommentID"":1,
                  ""Comments"":[
                        {
                        ""CommentText"": ""Test Commment Comment"",
                        ""UID"":2,
                        ""CommentOnCommentID"":1,
                        ""CommentID"":2,
                        ""Comments"":[
                            {
                            ""CommentText"": ""Test Commment Comment Comment"",
                            ""UID"":3,
                            ""CommentOnCommentID"":2,
                            ""CommentID"":4,
                            ""Comments"":[
                    
                             ]
                            }
                         ]
                        },
                    {
                        ""CommentText"": ""Test Comment 2"",
                        ""UID"": 3,
                        ""CommentOnCommentID"":1,
                        ""CommentID"":3,
                        ""Comments"":[
                        ]
                    }
            
                   ]
                }";
    */

    public class Comments
    {
        public string commentText { get; set; }
        public int uid { get; set; }
        public int commentOnCommentID { get; set; }
        public int commentID { get; set; }
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }
    }


    internal class PostgresAddComment
    {

        public static async Task AddComment(int uid, string text, int postID, int wantedCommentID)
        {
            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            using var command = new NpgsqlCommand("SELECT commentsreplies, commentamt FROM public.posts WHERE postid=($1)", conn)
            {
                Parameters =
                {
                    new () { Value = postID }
                }
            };

            string _currentCommentString = "";
            int _commentCount = 0;

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                _currentCommentString = (string)reader.GetValue(0);
                _commentCount = (int)reader.GetValue(1);
            }

            while (reader.IsClosed == false)
            {
                reader.Close();
            }

            Console.WriteLine(_currentCommentString + " " + _commentCount);
            // Update For JSON

            string _startString = "";
            string _endString = "";
            
            if (wantedCommentID != 0) 
            {
                string indexToFind = @$"""commentID"": {wantedCommentID}";
                //Console.WriteLine(indexToFind);
                int commentsIndex = _currentCommentString.IndexOf(indexToFind);
                //Console.WriteLine("Index: " + commentsIndex.ToString());
                commentsIndex = commentsIndex + 27 + wantedCommentID.ToString().Length;
                _startString = _currentCommentString.Substring(0, commentsIndex);
                Console.WriteLine("Start String: " + _startString);
                _endString = _currentCommentString.Substring(commentsIndex);
                Console.WriteLine("End String: " + _endString);
            }
            else
            {
                _startString = _currentCommentString.Substring(0, 1);
                _endString = _currentCommentString.Substring(1);
            }

            Comments newCommentJSON = new Comments
            {
                commentText = text,
                uid = uid,
                commentOnCommentID = wantedCommentID,
                commentID = _commentCount + 1
            };
            
            var _serializeOptions = new JsonSerializerOptions { WriteIndented = true };
            string _newCommentString = JsonSerializer.Serialize(newCommentJSON, _serializeOptions);

            string _indexString = @"""commentID"": " + (_commentCount + 1).ToString();

            int _index = _newCommentString.IndexOf(_indexString);
            _index = _index + 14 + (_commentCount + 1).ToString().Length;
            string[] _newCommentArray = { _newCommentString.Substring(0, _index), _newCommentString.Substring(_index) };

            string _addedCommentsSection = @",""comments"":[ ]";
            Console.WriteLine("Comment Array 0: " + _newCommentArray[0]);
            Console.WriteLine("Comment Array 1: " + _newCommentArray[1]);

            if (_currentCommentString.Contains("{"))
            {
                _newCommentString = _newCommentArray[0] + _addedCommentsSection + _newCommentArray[1] + ",";
            }

            else
            {
                _newCommentString = _newCommentArray[0] + _addedCommentsSection + _newCommentArray[1];
            }

            string _finishedJsonString = _startString + _newCommentString + _endString;
            int _commentCountInt = _commentCount + 1;

            Console.WriteLine(_finishedJsonString);

            using var command2 = new NpgsqlCommand("UPDATE public.posts SET commentsreplies=($1), commentamt=($2) WHERE postid=($3)", conn)
            {
                Parameters =
                {
                    new NpgsqlParameter() { Value = _finishedJsonString },
                    new NpgsqlParameter() { Value = _commentCountInt },
                    new () { Value = postID }
                }
            };

            await command2.ExecuteNonQueryAsync();
            await conn.CloseAsync();
        }

    }
}
