using Npgsql;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Mimir.backend.postgres
{
    internal class PostgresChangeLike
    {

        static public async Task<bool> ChangeLike(int puid, int uid)
        {
            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            await using var command = new NpgsqlCommand("SELECT likes, likeamt FROM public.posts WHERE postid = ($1)", conn)
            {
                Parameters =
                {
                    new () { Value = puid }
                }
            };

            string _likeString = "";
            int _likeAmt = 0;

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                _likeString = reader.GetString(0);
                _likeAmt = reader.GetInt32(1);
            }

            while (reader.IsClosed == false)
            {
                await reader.CloseAsync();
            }

            string[] uidArray = _likeString.Split(',');

            Console.WriteLine("Got this like string: " + _likeString + " This like amount: " + _likeAmt.ToString());

            if (!uidArray.Contains(uid.ToString()))
            {
                _likeString += (uid.ToString() + ",");
                _likeAmt++;

                Console.WriteLine("Made this like string: " + _likeString + " Added like amount: " + _likeAmt.ToString() + " To post: " + puid.ToString());

                await using var command2 = new NpgsqlCommand("UPDATE public.posts SET likes = ($1), likeamt = ($2) WHERE postid = ($3)", conn)
                {
                    Parameters =
                    {
                        new () { Value = _likeString },
                        new () { Value = _likeAmt },
                        new () { Value = puid }
                    }
                };

                await command2.ExecuteNonQueryAsync();
                await conn.CloseAsync();

                return true;
            }
            else
            {
                _likeAmt--;

                Console.WriteLine("Made this removing like string: " + _likeString + " Subtracted like amount: " + _likeAmt.ToString());

                uidArray[uidArray.IndexOf(uid.ToString())] = "";
                _likeString = "";
                bool firstString = true;
                foreach(string s in uidArray)
                {
                    if(s != "")
                    {
                        if(firstString == true)
                        {
                            firstString = false;
                            _likeString += s;
                        }
                        else
                        {
                            _likeString += "," + s;
                        }
                    }
                }

                await using var command2 = new NpgsqlCommand("UPDATE public.posts SET likes = ($1), likeamt = ($2) WHERE postid = ($3)", conn)
                {
                    Parameters =
                {
                    new () { Value = _likeString },
                    new () { Value = _likeAmt },
                    new () { Value = puid }
                }
                };

                await command2.ExecuteNonQueryAsync();
                await conn.CloseAsync();

                Console.WriteLine("Finished changing like.");

                return false;
            }

            
        }
    }
}
