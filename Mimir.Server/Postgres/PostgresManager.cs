using System;
using System.Collections.Generic;
using System.Text;



namespace Mimir.backend.postgres
{
    internal class PostgresManager
    {

        static int loggedInUID = 0;
        static string loginToken = "";

        static public async Task Main()
        {
            await UserActions();
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
                    Console.WriteLine("'esc' to leave, 'mu' to manage users, 'login' to log in, 'create' to create a user");
                    break;
                case "mu":
                    await PostgresManageUser.ManageUser(); ;
                    break;
                case "login":
                    var _loginInfo = await PostgresUserLogIn.UserLogin();
                    loggedInUID = _loginInfo.Item1;
                    loginToken = _loginInfo.Item2;
                    break;
                case "create":
                    Console.WriteLine("Username: ");
                    string _username = GetUserInput();
                    Console.WriteLine("Password: ");
                    string _password = GetUserInput();
                    bool _created = await PostgresCreateUser.CreateUser(_username, _password);
                    if (_created == true)
                    {
                        Console.WriteLine("Created User: " + _username);
                    }
                    else
                    {
                        Console.WriteLine("Username Taken.");
                    }
                    break;

            }

            await UserActions();
        }

        static public string[] PostgresAPICall(string request, string inputString = "", string inputString2 = "", 
            string inputString3 = "", int inputInt = 0, int inputInt2 = 0)
        {
            return null;
        }
    }
}
