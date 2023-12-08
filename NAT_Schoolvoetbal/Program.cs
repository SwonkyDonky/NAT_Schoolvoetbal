using System;

Console.WriteLine("Welkom bij de Schoolvoetbal App!");

while (true)
{
    Console.WriteLine("Kies een optie:");
    Console.WriteLine("1. Inloggen");
    Console.WriteLine("2. Registreren");

    string choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            Login(); // Inloggen
            break;
        case "2":
            Register(); // Registreren
            break;
        default:
            Console.WriteLine("Ongeldige keuze. Probeer opnieuw.");
            break;
    }
}

void Login()
{
    Console.Clear();
    Console.WriteLine("Inloggen - Voer je gegevens in.");
    Console.WriteLine("");
    Console.WriteLine("Email: ");
    string email = Console.ReadLine();
    Console.WriteLine("Wachtwoord: ");
    string pass = Console.ReadLine();

    UserController userController = new UserController();
    userController.LogInAccount(email, pass);
}

void Register()
{
    Console.Clear();
    Console.WriteLine("Registreren - Voer je gegevens in.");
    Console.WriteLine("");
    Console.WriteLine("Email: ");
    string email = Console.ReadLine();
    Console.WriteLine("Wachtwoord: ");
    string pass = Console.ReadLine();

    UserController userController = new UserController();
    userController.CreateNewAccount(email, pass);
}
