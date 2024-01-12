using APiTest;
using System;

class Program
{
    private static bool isLoggedIn = false;

    static void Main(string[] args)
    {
        while (true)
        {
            if (isLoggedIn)
            {
                // Opties voor ingelogde gebruikers
                Console.WriteLine("Kies een optie:");
                Console.WriteLine("1. 4SDollars inzetten");
                Console.WriteLine("2. Wedstrijden bekijken");
                Console.WriteLine("3. Uitloggen");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        // 4SDollars inzetten
                        break;
                    case "2":
                        Console.Clear();
                        Matches();
                        break;
                    case "3":
                        Console.Clear();
                        isLoggedIn = false;
                        Console.WriteLine("Uitgelogd.");
                        break;
                    default:
                        Console.WriteLine("Ongeldige keuze. Probeer opnieuw.");
                        break;
                }
            }
            else
            {
                // Opties voor niet-ingelogde gebruikers
                Console.WriteLine("Kies een optie:");
                Console.WriteLine("1. Inloggen");
                Console.WriteLine("2. Registreren");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        // Inloggen
                        Login();
                        break;
                    case "2":
                        // Registreren
                        Register();
                        break;
                    default:
                        Console.WriteLine("Ongeldige keuze. Probeer opnieuw.");
                        break;
                }
            }
        }
    }

    static void Login()
    {
        Console.Clear();
        Console.WriteLine("Inloggen - Voer je gegevens in.");
        Console.WriteLine("");
        Console.WriteLine("Email: ");
        string email = Console.ReadLine();

        Console.WriteLine("Wachtwoord: ");
        string pass = ReadPassword();

        UserController userController = new UserController();
        userController.LogInAccount(email, pass, out isLoggedIn);
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

    static void Matches()
    {
        // Add your API call here
        // Example code:
        MatchController matchController = new MatchController();
        string apiResponse = matchController.GetApiResponse();

        Console.WriteLine("API Response:");
        Console.WriteLine(apiResponse);
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    // Methode om het wachtwoord in te voeren zonder het weer te geven
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