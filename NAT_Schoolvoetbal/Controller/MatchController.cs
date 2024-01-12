using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace APiTest
{
    internal class Program
    {
        static async Task Main()
        {
            // Create an instance of HttpClient with custom handler
            using (HttpClient httpClient = new HttpClient(new HttpClientHandler
            {
                // Disable SSL certificate validation
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            }))
            {
                try
                {
                    // Specify the HTTP URL without SSL (http://localhost:port)
                    string apiUrl = "http://localhost:8000/api/user";
                    string bearerToken = "1Dc2Gg7BNh8vZF5haBf1y2sffLQApbdNcwT1Pq1N933edb3d"; // Bearer Token
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                    // Send an HTTP GET request to the API URL
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    // Check if the response is successful (status code 200)
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content as a string
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        // Process the API response data
                        Console.WriteLine(apiResponse);
                    }
                    else
                    {
                        Console.WriteLine($"HTTP Error: {response.StatusCode}");
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