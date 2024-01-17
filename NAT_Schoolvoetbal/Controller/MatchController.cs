using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace APiTest
{
    internal class MatchController
    {
        public async Task GetMatches()
        {
            using (HttpClient httpClient = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            }))
            {
                try
                {
                    string apiUrl = "http://localhost:8000/api/matches";
                    string bearerToken = "xpLeZU8FEBM0ooL6mdQb0StHFiLy57N9gVXAWgj0b6610335";
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        // Parse the JSON response
                        List<Match> matches = JsonConvert.DeserializeObject<List<Match>>(apiResponse);

                        // Display the matches
                        foreach (Match match in matches)
                        {
                            Console.WriteLine($"{match.team1_name} vs {match.team2_name}");
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        } 
    }
}

