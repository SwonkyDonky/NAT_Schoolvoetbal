public class UserController
{
    private const int InitialBalance = 50;
    public void CreateNewAccount(string email, string password)
    {
        Console.Clear();
        using (var dbContext = new AppDbContext())
        {
            if (!IsValidEmail(email))
            {
                Console.WriteLine("Ongeldig e-mailadres. Voer een geldig e-mailadres in.");
                return;
            }

            // Controleer of de gebruiker al bestaat
            if (dbContext.Users.Any(u => u.Email == email))
            {
                Console.WriteLine("Email is al in gebruik. Kies een andere.");
                return;
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new User
            {
                Email = email,
                Password = hashedPassword,
                Sdollars = InitialBalance
            };

            // Voeg de nieuwe gebruiker toe aan de database
            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();

            Console.WriteLine("Account succesvol aangemaakt!");
        }
    }
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    public void CreateAdminAccount(string userName, string password)
    {
        // Logica om een nieuw admin-account te maken en op te slaan in de lokale database
    }
    public bool LogInAccount(string email, string password, out bool isLoggedIn)
    {
        Console.Clear();
        using (var dbContext = new AppDbContext())
        {
            var user = dbContext.Users.SingleOrDefault(u => u.Email == email);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                // Set the console color to green
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine($"Welkom, {user.Email}! Balans in 4SDollars: {user.Sdollars}");

                // Reset the console color to default
                Console.ResetColor();

                isLoggedIn = true;
                return true;
            }
            else
            {
                Console.WriteLine("Ongeldige inloggegevens. Probeer opnieuw.");
                isLoggedIn = false;
                return false;
            }
        }
    }

}