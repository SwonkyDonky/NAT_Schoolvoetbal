using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace APiTest
{
    internal class MatchController
    {
        public async Task UpdateDatabaseWithNewMatches()
        {
            using (HttpClient httpClient = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            }))
            {
                try
                {
                    string apiUrl = "http://localhost:8000/api/upcoming-matches";

                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        // Log the raw JSON response
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        // Parse the JSON response
                        List<Match> newMatches = JsonConvert.DeserializeObject<List<Match>>(apiResponse);

                        // Get existing matches from the database
                        List<Match> existingMatches = GetExistingMatchesFromDatabase();

                        // Display and save the new matches
                        foreach (Match match in newMatches)
                        {
                            match.id = match.id;

                            // Check if a match with the same team names already exists in the database
                            bool matchExists = existingMatches.Any(
                                m => m.team1_name == match.team1_name && m.team2_name == match.team2_name
                            );

                            if (!matchExists)
                            {
                                // Add the new match to the database
                                SaveMatchToDatabase(match);

                                Console.WriteLine($"New Match: {match.team1_name} vs {match.team2_name}");
                            }
                            else
                            {
                                Console.WriteLine($"{match.team1_name} vs {match.team2_name}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }


        // Method to get existing matches from the database
        private List<Match> GetExistingMatchesFromDatabase()
        {
            var dbContext = new AppDbContext();
            return dbContext.Matches.ToList();
        }

        // Method to save a match to the database
        private void SaveMatchToDatabase(Match match)
        {
            var dbContext = new AppDbContext();
            try
            {
                match.id = match.id;

                // Check if the match already exists in the database
                bool matchExists = dbContext.Matches.Any(m => m.id == match.id);

                if (!matchExists)
                {
                    // Add the new match to the database
                    dbContext.Matches.Add(match);
                    dbContext.SaveChanges();
                }
                else
                {
                    Console.WriteLine($"Match already exists in the database: {match.team1_name} vs {match.team2_name}");
                }
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Error saving match to the database: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
        }

        // Custom comparer for the Match class, comparing matches based on their ID
        public class MatchComparer : IEqualityComparer<Match>
        {
            public bool Equals(Match x, Match y)
            {
                return x.id == y.id;
            }

            public int GetHashCode(Match obj)
            {
                return obj.id.GetHashCode();
            }
        }

        public Match GetMatchByName(string chosenMatch)
        {
            var dbContext = new AppDbContext();

            // Split the chosenMatch into team names
            string[] teams = chosenMatch.Split("vs", StringSplitOptions.RemoveEmptyEntries);
            if (teams.Length != 2)
            {
                return null; // Invalid format
            }

            string team1Name = teams[0].Trim();
            string team2Name = teams[1].Trim();

            return dbContext.Matches.FirstOrDefault(m => m.team1_name == team1Name && m.team2_name == team2Name);
        }

        public void PlaceBet(User user, Match match)
        {
            var dbContext = new AppDbContext();
            user = dbContext.Users.FirstOrDefault(u => u.Id == user.Id); // Fetch the user from the database

            Console.WriteLine($"Op welk team wil je inzetten?");
            Console.WriteLine($"1. {match.team1_name}");
            Console.WriteLine($"2. {match.team2_name}");

            string teamChoice = Console.ReadLine();
            Console.WriteLine("");

            if (teamChoice == "1" || teamChoice == "2")
            {
                string teamName = (teamChoice == "1") ? match.team1_name : match.team2_name;

                Console.WriteLine($"Hoeveel 4SDollars wil je inzetten? Je balans: {user.Sdollars}");
                int betAmount;
                while (!int.TryParse(Console.ReadLine(), out betAmount) || betAmount < 0)
                {
                    Console.WriteLine("Ongeldige invoer. Voer een geldig bedrag in:");
                }

                // Check if the user has enough 4SDollars
                if (betAmount <= user.Sdollars)
                {
                    // Call a method to save the bet to the database
                    SaveBetToDatabase(user.Id, match.id, teamName, betAmount);

                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Inzet van {betAmount} 4SDollars op {teamName} in de wedstrijd tussen {match.team1_name} en {match.team2_name} geplaatst.");
                    Console.ResetColor();

                    // Update the user's SDollars after placing the bet
                    user.Sdollars -= betAmount;

                    // Save the updated user to the database
                    dbContext.SaveChanges();

                    // Update the currentUser with the latest information
                    Program.SetCurrentUser(user);
                }
                else
                {
                    Console.WriteLine("Je hebt niet genoeg 4SDollars om deze inzet te plaatsen.");
                }
            }
            else
            {
                Console.WriteLine("Ongeldige keuze. Probeer opnieuw.");
            }
        }


        public void SaveBetToDatabase(int userId, int matchId, string teamName, int betAmount)
        {
            var dbContext = new AppDbContext();
            Gamble newBet = new Gamble
            {
                user_id = userId,
                match_id = matchId,
                team_name = teamName,
                dollars = betAmount
            };

            dbContext.Gamble.Add(newBet);
            dbContext.SaveChanges();
        }

        public async Task<List<Match>> GetMatchResults()
        {
            // Fetch the raw JSON code for match results
            using (HttpClient httpClient = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            }))
            {
                try
                {
                    string apiUrl = "http://localhost:8000/api/results";

                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        // Parse the JSON response into a list of Match objects
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        List<Match> matchResults = JsonConvert.DeserializeObject<List<Match>>(apiResponse);
                        return matchResults;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return null;
        }

        public void ProcessWinningBets(Match matchResult, User currentUser)
        {
            var dbContext = new AppDbContext();

            // Check if the match is a tie
            if (GetWinningTeamName(matchResult) == "Tie")
            {
                // Get all bets for the current user on this match
                List<Gamble> userBetsOnTie = dbContext.Gamble
                    .Where(b => b.match_id == matchResult.id && b.user_id == currentUser.Id)
                    .ToList();

                foreach (Gamble tieBet in userBetsOnTie)
                {
                    // The user lost the bet
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"User {currentUser.Id} lost {tieBet.dollars} Sdollars on match {matchResult.id}. (Match is a tie)");
                    Console.ResetColor();

                    // Remove the bet from the database
                    dbContext.Gamble.Remove(tieBet);
                }

                // Save changes to the database after processing bets for a tie
                dbContext.SaveChanges();

                return;
            }

            // Get all bets for this match from the local database for the current user
            List<Gamble> userBets = dbContext.Gamble
                .Where(b => b.match_id == matchResult.id && b.user_id == currentUser.Id)
                .ToList();

            foreach (Gamble bet in userBets.ToList())
            {
                // Check if the user's bet matches the winning team
                string winningTeamName = GetWinningTeamName(matchResult);

                if (bet.team_name == winningTeamName)
                {
                    // The user won the bet, update their Sdollars
                    currentUser.Sdollars += 2 * bet.dollars; // Double the bet amount as winnings
                    dbContext.SaveChanges();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"User {currentUser.Email} won {2 * bet.dollars} 4Sdollars on match {matchResult.id}! (Betted on Team {bet.team_name})");
                    Console.ResetColor();

                    // Update the currentUser with the latest information
                    Program.SetCurrentUser(currentUser);
                }
                else
                {
                    // The user lost the bet
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"User {currentUser.Id} lost {bet.dollars} 4Sdollars on match {matchResult.id}. (Betted on Team {bet.team_name})");
                    Console.ResetColor();
                }
                // Remove the winning bet from the database
                dbContext.Gamble.Remove(bet);
            }
            // Save changes to the database after processing all bet
            dbContext.SaveChanges();
        }


        private string GetWinningTeamName(Match matchResult)
        {
            // Check if the match is a tie (both team scores are null or equal)
            if (matchResult.team1_score == null && matchResult.team2_score == null ||
                matchResult.team1_score == matchResult.team2_score)
            {
                return "Tie";
            }

            // Check if there is a winning team based on match result
            if (matchResult.team1_score > matchResult.team2_score)
            {
                return matchResult.team1_name;
            }
            else if (matchResult.team1_score < matchResult.team2_score)
            {
                return matchResult.team2_name;
            }

            // If no winner is determined, return a value indicating no winner
            return "NoWinner";
        }
    }
}
