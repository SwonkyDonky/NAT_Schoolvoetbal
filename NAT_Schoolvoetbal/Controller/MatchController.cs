using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace APiTest
{
    internal class MatchController
    {
        public string GetApiResponse()
        {
            using (HttpClient httpClient = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            }))
            {
                try
                {
                    string apiUrl = "http://localhost:8000/api/user";
                    string bearerToken = "495e095a1de24cb728b8548c46a0ddd0e392cda1ffa3357bd92051d4e6da5eb5";
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        return $"HTTP Error: {response.StatusCode}";
                    }
                }
                catch (Exception ex)
                {
                    return $"Error: {ex.Message}";
                }
            }
        }

        public string[] ParseApiResponse(string apiResponse)
        {
            try
            {
                // Deserialize the JSON response
                dynamic jsonData = JsonConvert.DeserializeObject(apiResponse);

                // Assuming jsonData contains an array of matches with team1_id and team2_id properties
                // Adjust this part based on the actual structure of your API response

                // Create a list to store the match strings
                var matches = new List<string>();

                foreach (var matchData in jsonData.matches)
                {
                    string team1_id = matchData.team1_id;
                    string team2_id = matchData.team2_id;

                    // Format the match string
                    string matchString = $"{team1_id} vs {team2_id}";

                    // Add the match string to the list
                    matches.Add(matchString);
                }

                // Return the array of match strings
                return matches.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing API response: {ex.Message}");
                return new string[0]; // Return an empty array in case of an error
            }
        }
    }
}
