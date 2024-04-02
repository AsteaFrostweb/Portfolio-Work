using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestApp
{

    class NetworkManager
    {
        private string baseUri = "https://localhost";
        private string Port = "7093";
        private string Username = "serina@groenewald.com";
        private string Password = "Iona001!";
        public string UriString { get { return baseUri + ":" + Port; } }

        private HttpClientHandler httpClientHandler;
        public HttpClient httpClient { get; set; }
        public NetworkManager()
        {
            httpClientHandler = new HttpClientHandler();
            httpClient = new HttpClient(httpClientHandler);
        }

        public async Task<bool> Login()
        {
            string verificationToken = await GetVerificationToken();
            if (verificationToken == "")
            {
                return false;
            }
            FormUrlEncodedContent LoginData = GetLoginContent(verificationToken, Username, Password);

            // Send POST request with form data
            HttpResponseMessage response = await httpClient.PostAsync($"{UriString}/Identity/Account/Login", LoginData);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        private FormUrlEncodedContent GetLoginContent(string verificationToken, string username, string password) 
        {
            return new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Input.Email", username),
                new KeyValuePair<string, string>("Input.Password", password),
                new KeyValuePair<string, string>("Input.RememberMe", "true"),
                new KeyValuePair<string, string>("__RequestVerificationToken", verificationToken)
            });
        }
        private async Task<string> GetVerificationToken() 
        {
            // Send GET request to retrieve the login page
            var loginPageResponse = await httpClient.GetAsync($"{UriString}/Identity/Account/Login");
            try
            {
                loginPageResponse.EnsureSuccessStatusCode(); // Throw an exception if the response is not successful
            }
            catch (Exception e) { Console.WriteLine("Error retreiving login page: " + e.Message); }
            
            string loginPageContent = await loginPageResponse.Content.ReadAsStringAsync();
          
            // Extract the verification token from the login page content
            string verificationToken = ExtractVerificationToken(loginPageContent);
            return verificationToken;
        }
        private string ExtractVerificationToken(string htmlContent)
        {
            // Define the regular expression pattern to match the verification token
            string pattern = "<input[^>]+name=\"__RequestVerificationToken\"[^>]+value=\"([^\"]+)\"[^>]*>";          
            Match match = Regex.Match(htmlContent, pattern);
          
            if (match.Success)
            {                
                return match.Groups[1].Value;
            }
            else
            {              
                Console.WriteLine("Verification token not found.");
                return "";
            }
        }

    }

   
    class HighScores 
    {
        enum Sort_Order { DESC, ASC}
        enum Sort_By {NAME, FASTEST_LAP }
        List<Highscore> highscores;

    }

    public class Highscore
    {
        public string Name { get; set; }
        public string Map { get; set; }
        public string Fastest_Lap { get; set; }
        public int Best_Combo_Score { get; set; }
        public string Best_Combo_Time { get; set; }

        public override string ToString()
        {
            return "Name:" + Name + ", Map:" + Map + ", Fastest Lap:" + Fastest_Lap + ", Best Combo Score:" + Best_Combo_Score + ", Best Combo Time:" + Best_Combo_Time; 
        }
    }

    public class HighscoreParser
    {
        public static List<Highscore> ParseHighscores(string text)
        {
            List<Highscore> highscores = new List<Highscore>();

            // Define the regex pattern to match key-value pairs
            string pattern = @"\{Name:(?<name>.*?),Map:(?<map>.*?),Fastest_Lap:(?<fastestLap>.*?),BestComboScore:(?<bestComboScore>.*?),Best_Combo_Time:(?<bestComboTime>.*?)\}";

            // Use Regex to find all matches of the pattern in the input text
            MatchCollection matches = Regex.Matches(text, pattern);

            // Iterate through each match and create a Highscore object
            foreach (Match match in matches)
            {
                Highscore highscore = new Highscore
                {
                    Name = match.Groups["name"].Value,
                    Map = match.Groups["map"].Value,
                    Fastest_Lap = match.Groups["fastestLap"].Value,
                    Best_Combo_Score = int.Parse(match.Groups["bestComboScore"].Value),
                    Best_Combo_Time = match.Groups["bestComboTime"].Value
                };

                highscores.Add(highscore);
            }

            return highscores;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            string baseUrl = "https://localhost:7093"; // Replace with your website URL

            var httpClientHandler = new HttpClientHandler();
            var httpClient = new HttpClient(httpClientHandler);

            // Send GET request to retrieve the login page
            var loginPageResponse = await httpClient.GetAsync($"{baseUrl}/Identity/Account/Login");
            loginPageResponse.EnsureSuccessStatusCode(); // Throw an exception if the response is not successful
            string loginPageContent = await loginPageResponse.Content.ReadAsStringAsync();

            // Extract the verification token from the login page content
            string verificationToken = ExtractVerificationToken(loginPageContent);
            Console.WriteLine("Verification Token: " + verificationToken + "\n");

            // Prepare the form data
            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Input.Email", "serina@groenewald.com"),
                new KeyValuePair<string, string>("Input.Password", "Iona001!"),
                new KeyValuePair<string, string>("Input.RememberMe", "true"),
                new KeyValuePair<string, string>("__RequestVerificationToken", verificationToken)
            });
           // Console.WriteLine("before Login:" + httpClient.DefaultRequestHeaders.Authorization.ToString());
            // Send POST request with form data
            var response = await httpClient.PostAsync($"{baseUrl}/Identity/Account/Login", formData);

            if (response.IsSuccessStatusCode)
            {
                // Extract cookies from the response headers        
                foreach (Cookie cookie in httpClientHandler.CookieContainer.GetCookies(new Uri(baseUrl)))
                {
                    Console.WriteLine("Cookie Name: " + cookie.Name); 
                    Console.WriteLine("Cookie value: " + cookie.Value);
                    Console.WriteLine("Cookie Comment: " + cookie.Comment + "\n");
                }

               
             

                Console.WriteLine("Login successful!");
                var repondcontent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(repondcontent);


                var managePageResponse = await httpClient.GetAsync($"{baseUrl}/Highscores");

                repondcontent = await managePageResponse.Content.ReadAsStringAsync();

                
                foreach (Highscore h in HighscoreParser.ParseHighscores(repondcontent)) 
                {
                    Console.WriteLine(h.ToString());
                }
               // Console.WriteLine(repondcontent);
            }
            else
            {
                var repondcontent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(repondcontent);
                Console.WriteLine("Login failed. " + response.ReasonPhrase);
            }

            Console.ReadLine();
        }

        static string ExtractVerificationToken(string htmlContent)
        {
            // Define the regular expression pattern to match the verification token
            string pattern = "<input[^>]+name=\"__RequestVerificationToken\"[^>]+value=\"([^\"]+)\"[^>]*>";

            // Use Regex.Match to find the first match of the pattern in the HTML content
            Match match = Regex.Match(htmlContent, pattern);

            // Check if a match is found
            if (match.Success)
            {
                // Extract the value of the verification token from the matched group
                return match.Groups[1].Value;
            }
            else
            {
                // If no match is found, return null or throw an exception
                throw new Exception("Verification token not found.");
            }
        }
    }
}
