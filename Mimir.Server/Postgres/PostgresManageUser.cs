using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace Mimir.backend.postgres
{
    internal class PostgresManageUser
    {
        static private string pgsSettings = "";
        static public async Task ManageUser()
        {
            Console.WriteLine("Starting Postgres Manage User");

            pgsSettings = GetPostgres.GetPostgresSettings();
            Console.WriteLine(pgsSettings);

            await UserActions();

            Console.WriteLine("Finished Postgres Manage User");
        }

        static public async Task DeleteUserFromDB(string username)
        {
            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            await using var command = new NpgsqlCommand("DELETE FROM uinfo.users WHERE username = ($1)", conn)
            {
                Parameters =
                {
                    new () { Value = username }
                }
            };
            await command.ExecuteReaderAsync();
            await conn.CloseAsync();
        }

        static public async Task GetAllUsers()
        {
            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            await using var command = new NpgsqlCommand("SELECT * FROM uinfo.users", conn);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Console.WriteLine(reader.GetValue(0).ToString() + " " + reader.GetValue(1));
            }
            await conn.CloseAsync();
        }

        static public async Task UpdateUsername(string username, string newUsername)
        {
            await using var conn = new NpgsqlConnection(GetPostgres.GetPostgresSettings());
            await conn.OpenAsync();

            await using var command = new NpgsqlCommand("UPDATE uinfo.users SET username = ($1) WHERE username = ($2)", conn)
            {
                Parameters =
                {
                    new() { Value = newUsername },
                    new() { Value = username }
                }
            };
            await command.ExecuteReaderAsync();
            await conn.CloseAsync();
        }

        static private string GetUserInput()
        {
            return Console.ReadLine();
        }

        static async Task UserActions()
        {
            Console.WriteLine("What action you want to use? ('Help' for options)");
            string _userInput = "test";
            _userInput = GetUserInput();

            switch (_userInput.ToLower())
            {
                case "esc":
                    return;
                case "help":
                    Console.WriteLine("'esc' to leave, 'del' to delete user, 'users' to see users, 'update' to update username");
                    break;
                case "del":
                    Console.WriteLine("Delete which User?");
                    _userInput = GetUserInput();
                    await DeleteUserFromDB(_userInput);
                    break;
                case "users":
                    await GetAllUsers();
                    break;
                case "update":
                    Console.WriteLine("Username to Update");
                    _userInput = GetUserInput();
                    Console.WriteLine("New username");
                    string newUsername = Console.ReadLine().ToString();
                    await UpdateUsername(_userInput, newUsername);
                    break;
            }

            await UserActions();
        }
    }
}
