using APiTest;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Net.Http.Headers;
class Program
{
    private static bool isLoggedIn = false;
    private static User currentUser;
    static void Main(string[] args)
    {
        int currentSessionId = 0;

        while (true)
        {
            if (isLoggedIn)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Welkom, {currentUser.Email}! Balans in 4SDollars: {currentUser.Sdollars}");
                Console.ResetColor();

                Console.WriteLine("Kies een optie:");
                Console.WriteLine("1. 4SDollars inzetten");
                Console.WriteLine("2. Check of je gewonnen hebt!");
                Console.WriteLine("3. Uitloggen");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        Matches(SessionManager.GetSessionUser(currentSessionId), currentSessionId);
                        break;
                    case "2":
                        Console.Clear();
                        Check(currentUser);
                        break;
                    case "3":
                        Console.Clear();
                        isLoggedIn = false;
                        SessionManager.RemoveSession(currentSessionId);  // Verwijder de huidige sessie bij uitloggen
                        Console.WriteLine("Uitgelogd.");
                        break;
                    default:
                        Console.WriteLine("Ongeldige keuze. Probeer opnieuw.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Kies een optie:");
                Console.WriteLine("1. Inloggen");
                Console.WriteLine("2. Registreren");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Login(out currentSessionId);
                        break;
                    case "2":
                        Register();
                        break;
                    default:
                        Console.WriteLine("Ongeldige keuze. Probeer opnieuw.");
                        break;
                }
            }
        }
    }

    static void Login(out int sessionId)
    {
        Console.Clear();
        Console.WriteLine("Inloggen - Voer je gegevens in.");
        Console.WriteLine("");
        Console.WriteLine("Email: ");
        string email = Console.ReadLine();

        Console.WriteLine("Wachtwoord: ");
        string pass = ReadPassword();

        UserController userController = new UserController();
        userController.LogInAccount(email, pass, out isLoggedIn, out sessionId);  // Geef de sessie-ID terug
    }

    static void Register()
    {
        Console.Clear();
        Console.WriteLine("Registreren - Voer je gegevens in.");
        Console.WriteLine("");
        Console.WriteLine("Email: ");
        string email = Console.ReadLine();

        Console.WriteLine("Wachtwoord: ");
        string pass = ReadPassword();

        UserController userController = new UserController();
        userController.CreateNewAccount(email, pass);
    }

    static void Matches(User currentUser, int sessionId)
    {
        Console.Clear();

        // Call the function in MatchController.cs
        MatchController matchController = new MatchController();
        matchController.UpdateDatabaseWithNewMatches().Wait();

        // Choose the match you want to bet on and call the according method in the match controller
        Console.WriteLine("");
        Console.WriteLine("Op welke wedstrijd wil je 4SDollars inzetten? (typ het exact uit zoals hierboven staat)");
        string chosenMatch = Console.ReadLine();
        Console.WriteLine("");

        // Get match details from the local database
        Match selectedMatch = matchController.GetMatchByName(chosenMatch);

        if (selectedMatch != null)
        {
            // Call the method to place a bet
            matchController.PlaceBet(SessionManager.GetSessionUser(sessionId), selectedMatch);
        }
        else
        {
            Console.WriteLine("Ongeldige wedstrijdkeuze. Probeer opnieuw.");
        }
    }

    static void Check(User currentUser)
    {
        Console.Clear();

        MatchController matchController = new MatchController();

        // Call the function to get the results of the matches
        List<Match> matchResults = matchController.GetMatchResults().Result;

        if (matchResults != null)
        {
            // Iterate through the match results and process winning bets for the current user
            foreach (Match matchResult in matchResults)
            {
                // Call a method to check and process winning bets for this match result
                matchController.ProcessWinningBets(matchResult, currentUser);
            }

            Console.WriteLine("Winning bets processed.");
        }
        else
        {
            Console.WriteLine("Failed to retrieve match results.");
        }
    }



    public static void SetCurrentUser(User user)
    {
        currentUser = user;
    }

    static string ReadPassword()
    {
        string password = "";
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                // Verwijder het laatst ingevoerde karakter
                password = password.Remove(password.Length - 1, 1);
                Console.Write("\b \b"); // Wis het karakter op het scherm
            }
            else if (key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();
        return password;
    }

}