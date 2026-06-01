using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mimir.Server.TestScripts
{
    public class CommentJSON
    {

        static string jsonString =
                @"{
                  ""CommentText"": ""Test Comment"",
                  ""UID"": 1,
                  ""CommentOnCommentID"":0,
                  ""CommentID"":1,
                  ""Comments"":[
                        {
                        ""CommentText"": ""Test Commment Comment"",
                        ""UID"": 2,
                        ""CommentOnCommentID"":1,
                        ""CommentID"":2,
                        ""Comments"":[
                            {
                            ""CommentText"": ""Test Commment Comment Comment"",
                            ""UID"": 3,
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
        /*
           {
            "CommentText":"", 
            "UID":uid
            "Comments":
             [
              {"CommentText":"", "UID":uid}
               ...
             ]
           }
        */

        public class CommentData
        {
            public string? CommentText { get; set; }
            public string? CommentImages { get; set; }
            public int UID { get; set; }
            public int PostID { get; set; }
            [JsonExtensionData]
            public Dictionary<string, JsonElement>? ExtensionData { get; set; }
        }

        public static void TestCommentJSONDeserialize()
        {
            

            CommentData comments =
                JsonSerializer.Deserialize<CommentData>(jsonString)!;
            CommentData[] comment2 =
                JsonSerializer.Deserialize<CommentData[]>(comments.ExtensionData["Comments"])!;
            var serializeOptions = new JsonSerializerOptions { WriteIndented = true };
            string jsonString2 = JsonSerializer.Serialize(comments, serializeOptions);

            //Console.WriteLine($"JSON output:\n{jsonString}\n");

            jsonString2 = JsonSerializer.Serialize(comment2[0], serializeOptions);
            //Console.WriteLine($"JSON output 2:\n{jsonString}\n");

            TestCommentJSONSerialize();
        }

        static void TestCommentJSONSerialize() 
        {
            int postIndex = 0;
            CommentData comments =
                JsonSerializer.Deserialize<CommentData>(jsonString)!;

            var serializeOptions = new JsonSerializerOptions { WriteIndented = true };
            jsonString = JsonSerializer.Serialize(comments, serializeOptions);

            string indexToFind = @$"""CommentID"": {postIndex.ToString()}";
            Console.WriteLine(indexToFind);
            int commentsIndex = jsonString.IndexOf(indexToFind);
            Console.WriteLine("Index: " + commentsIndex.ToString());
            commentsIndex = commentsIndex + 30 + postIndex.ToString().Length;
            string endString = jsonString.Substring(commentsIndex);
            string startString = jsonString.Substring(0, commentsIndex);

            string comment = @"{
                            ""CommentText"": ""Test Commment Comment Comment"",
                            ""UID"": 3,
                            ""CommentID"":3,
                            ""Comments"":[
                    
                            ]
                            },";

            string finishedJsonString = startString + comment + endString;
            Console.WriteLine("Start String: " + startString);
            Console.WriteLine("End String: " + endString);
            Console.WriteLine($"JSON Serialize:\n{finishedJsonString}\n");

        }

    }
}
