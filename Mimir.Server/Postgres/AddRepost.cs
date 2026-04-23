using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mimir.backend.postgres
{
    internal class AddRepost
    {
        static public async Task AddLike(int puid, int uid)
        {
            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            await using var command = new NpgsqlCommand("SELECT (repost, repostamt) FROM public.posts WHERE puid = ($1)", conn)
            {
                Parameters =
                {
                    new () { Value = puid }
                }
            };

            string _repostString = "";
            int _repostAmt = 0;

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                _repostString = reader.GetString(0);
                _repostAmt = reader.GetInt32(1);
            }

            while (reader.IsClosed == false)
            {
                await reader.CloseAsync();
            }

            _repostString += ("," + uid.ToString());
            _repostAmt++;

            await using var transaction = await conn.BeginTransactionAsync();

            await using var command2 = new NpgsqlCommand("UPDATE public.posts SET likes = ($1), likeamt = ($2) WHERE puid = ($3)", conn, transaction)
            {
                Parameters =
                {
                    new () { Value = _repostString },
                    new () { Value = _repostAmt },
                    new () { Value = puid }
                }
            };

            await command2.ExecuteNonQueryAsync();

            // Probably need to gather all reposts, then add the current. Reformat to an array using "," so that way it can be added when getting feed

            // Sounds like a lot of work rn tbh

            //await using var command3 = new NpgsqlCommand("UPDATE public.userprofile SET reposts = ($1) WHERE uid = ($2)", conn, transaction)
            //{
            //    Parameters =
            //    {
            //        new () { Value = puid },
            //        new () { Value = uid }
            //    }
            //};

            await transaction.CommitAsync();

            await conn.CloseAsync();
        }
    }
}
