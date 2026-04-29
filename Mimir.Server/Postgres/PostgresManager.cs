using Mimir.Server.Postgres;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;



namespace Mimir.backend.postgres
{
    internal class PostgresManager
    {
        
        interface postInterface
        {
            int postid { get; set; }
            int posterUID { get; set; }
            string? postText { get; set; }
            string? images { get; set; }
            int likeAmt { get; set; }
            int repostAmt { get; set; }
            int commentAmt { get; set; }

        }

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
                    //var _loginInfo = await PostgresUserLogIn.UserLogin();
                    //loggedInUID = _loginInfo.Item1;
                    //loginToken = _loginInfo.Item2;
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

        static public async Task<string[]> PostgresAPICall(string request, string inputString = "", string inputString2 = "", 
            string inputString3 = "", int inputInt = 0, int inputInt2 = 0)
        {
            switch (request)
            {
                case "login":
                    var tokenUID = PostgresUserLogIn.UserLogin(inputString);
                    //string tokenUIDJson = JsonSerializer.Serialize();
                    string[] tokenUIDStringArray = { tokenUID.Result.Item1.ToString(), tokenUID.Result.Item2};
                    return tokenUIDStringArray;
                case "createuser":
                    string[] userPass = inputString.Split(':');
                    await PostgresCreateUser.CreateUser(userPass[0], userPass[1]);

                    var tokenUIDCreate = await PostgresUserLogIn.UserLogin(inputString);
                    //string tokenUIDJson = JsonSerializer.Serialize();
                    string[] tokenUIDStringArrayCreate = { tokenUIDCreate.Item1.ToString(), tokenUIDCreate.Item2 };
                    return tokenUIDStringArrayCreate;
                case "getaccountdata":
                    var accountData = await PostgresGetUserAccount.GetUserAccountDetails(inputInt);
                    Console.WriteLine("Getting User Account Data");
                    string[] accountDataArray = { "true", accountData[0].ToString(), accountData[1].ToString(), accountData[2].ToString(), accountData[3].ToString(), accountData[4].ToString() };
                    return accountDataArray;
                case "getforiegnaccountdata":
                    string[] test2 = new string[1];
                    return test2;
                case "updateaccount":
                    string[] test3 = new string[1];
                    PostgresUpdateUserAccount.UpdateUserAccount(request, inputString);
                    return test3;
            }

            return null;
        }
    }
}
