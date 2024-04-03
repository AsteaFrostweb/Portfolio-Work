using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static TestApp.HighScores;

namespace TestApp
{

    class NetworkManager
    {
        private string baseUri = "";
        private string Port = "";
        private string Username = "";
        private string Password = "";
        public string UriString { get { return baseUri + ":" + Port; } }

        private HttpClientHandler httpClientHandler;
        public HttpClient httpClient { get; set; }
        public NetworkManager()
        {
            httpClientHandler = new HttpClientHandler();
            httpClient = new HttpClient(httpClientHandler);
        }
        public NetworkManager(string _baseUri, string _Port)
        {
            httpClientHandler = new HttpClientHandler();
            httpClient = new HttpClient(httpClientHandler);
            baseUri = _baseUri;
            Port = _Port;
        }



        //Gets a verification token from login page and then attempts to login with username and password from networkManager
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
        //Gets a verification token from login page and then attempts to login with username and password specified
        public async Task<bool> Login(string _username, string _password)
        {
            string verificationToken = await GetVerificationToken();
            if (verificationToken == "")
            {
                return false;
            }

            FormUrlEncodedContent LoginData = GetLoginContent(verificationToken, _username, _password);

            // Send POST request with form data
            HttpResponseMessage response = await httpClient.PostAsync($"{UriString}/Identity/Account/Login", LoginData);
            string content = await response.Content.ReadAsStringAsync();
            if (!content.Contains("Invalid login attempt."))
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

        public async Task<bool> PostHighScore(string mapName, float fastestLap, int bestComboScore, float bestComboTime)
        {
            // Create the form data
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("MapName", mapName),
                new KeyValuePair<string, string>("FastestLap", fastestLap.ToString()),
                new KeyValuePair<string, string>("BestComboScore", bestComboScore.ToString()),
                new KeyValuePair<string, string>("BestComboTime", bestComboTime.ToString())
            };

            // Create FormUrlEncodedContent
            var content = new FormUrlEncodedContent(formData);

            // Send POST request to the UpdateHighScore endpoint
            HttpResponseMessage response = await httpClient.PostAsync($"{UriString}/Highscores/UpdateHighScore", content);

            Console.WriteLine("Status Code: " + response.StatusCode.ToString());

            // Check if the response is successful
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Highscore>> GetHighscores(string MapName) 
        {
            List<Highscore> highscores = new List<Highscore>();

            // Send POST request to the UpdateHighScore endpoint
            HttpResponseMessage response = await httpClient.GetAsync($"{UriString}/Highscores/GetHighscores?MapName=" + MapName);

            string json = await response.Content.ReadAsStringAsync();

            highscores = HighScores.ParseHighscores(json);

            return highscores;
        }
    }

  
    public class HighscoreUpdateModel
    {
        public string MapName { get; set; }
        public float FastestLap { get; set; }
        public int BestComboScore { get; set; }
        public float BestComboTime { get; set; }
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

        public TimeSpan BestComboTimeSpan() 
        {
           return TimeSpan.ParseExact(Best_Combo_Time, @"hh\:mm\:ss\:fff", CultureInfo.InvariantCulture);
        }
        public TimeSpan FastestLapTimeSpan()
        {
            return TimeSpan.ParseExact(Fastest_Lap, @"hh\:mm\:ss\:fff", CultureInfo.InvariantCulture);
        }        

    }

    public class HighScores 
    {
        enum Sort_Order { DESC, ASC}
        enum Sort_By {NAME, FASTEST_LAP }

        List<Highscore> highscores;



        public static List<Highscore> ParseHighscores(string text)
        {
            List<Highscore> highscores;

            try
            {
                // Deserialize the JSON string into a list of Highscore objects
                highscores = JsonConvert.DeserializeObject<List<Highscore>>(text);
            }
            catch (Exception ex)
            {
                // Handle any errors that may occur during deserialization
                Console.WriteLine("Error parsing highscores: " + ex.Message);
                highscores = new List<Highscore>(); // Return an empty list if parsing fails
            }

            return highscores;
        }

    }

   


    class Program
    {
        private static string baseUri = "https://localhost";
        private static string Port = "7093";
        private static string Username = "bill@teach.com";
        private static string Password = "Password001!";
        static async Task Main(string[] args)
        {
            NetworkManager networkManager = new NetworkManager(baseUri, Port);
            if (await networkManager.Login(Username, Password) == true)
            {
                Console.WriteLine("login Succesful");

                if (await networkManager.PostHighScore("Sandy Slalom", 32.040f, 1481, 30.54f))
                {
                    Console.WriteLine("Highscore posted");
                }
                if (await networkManager.PostHighScore("Carteena Valley", 25.040f, 11681, 73.4f))
                {
                    Console.WriteLine("Highscore posted");
                }
                else 
                {
                    Console.WriteLine("Error posting highscore");
                }

                foreach (Highscore h in await networkManager.GetHighscores("Sandy Slalom")) 
                {
                    Console.WriteLine(h.ToString());
                }
                
            }
            else 
            {
                Console.WriteLine("Login failed");
            }
            Console.Read();
        }
    }
}
