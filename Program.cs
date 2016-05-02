using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace HelloWorld
{
    public class Program
    {
        private const string ProcessedMoviePattern = @".*?_(\d*)_(\d.\d|\d)"; 
        
        public static void Main(string[] args)
        {
            var targetDirectory = args[0];
            if(string.IsNullOrEmpty(targetDirectory)) throw new ArgumentException("targetDirectory could not found!");
            
            Console.WriteLine($"Iterating {targetDirectory}...\n");
            
            string [] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach(string subdirectory in subdirectoryEntries)
            {
                var rawMovieName = subdirectory.Substring(subdirectory.LastIndexOf("/") + 1);
                
                // Instantiate the regular expression object.
                Regex r = new Regex(ProcessedMoviePattern, RegexOptions.IgnoreCase);

                // Match the regular expression pattern against a text string.
                Match match = r.Match(rawMovieName);
                if(match.Success) 
                {
                    continue;
                };
                
                var movieServiceResult = GetAsync($"http://www.omdbapi.com/?t={rawMovieName}&r=json").Result;
                Console.WriteLine($"Getting result for: {rawMovieName}");
                
                if(movieServiceResult != null)
                {
                    try
                    {
                        var newMovieName = $"{movieServiceResult.Title}_{movieServiceResult.Year}_{movieServiceResult.ImdbRating}";
                        var targetDir = string.Format("{0}{1}",targetDirectory, newMovieName);
                        if(Directory.Exists(targetDir))
                        {
                            Console.WriteLine($"{targetDir} does exits!");
                            continue;
                        }
                        Directory.Move(subdirectory, targetDir);  
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                    
            }
                
            
            //Console.WriteLine(d.Title);
            Console.Read();
        }
        
        public static async Task<Movie> GetAsync(string uri)
        {
            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(uri);
            
                //will throw an exception if not successful
                response.EnsureSuccessStatusCode();
            
                string content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Movie>(content);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Movie could not be found! Exception: {ex.Message}");                
            }
            return null;
        }
    }


    public class Movie
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string Released { get; set; }
        public string Runtime { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public string Writer { get; set; }
        public string Actors { get; set; }
        public string Plot { get; set; }    
        public string Language { get; set; }
        public string Country { get; set; }
        public string Awards { get; set; }
        public string Poster { get; set; }
        public string ImdbRating { get; set; }
    }
}
