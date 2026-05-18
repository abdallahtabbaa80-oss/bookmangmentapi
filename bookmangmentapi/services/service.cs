using bookmangmentapi.models;
using bookmangmentapi.data;
using System.Net.Http;
using System.Net;
//using System.Threading.Tasks;   
using System.Text.Json;
namespace bookmangmentapi.services

{//service layer for http client
    public class Service
    {
        private readonly HttpClient _httpClient;
        // constructor
        public Service(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        // method to get book description and yearpublishd by isbn
        public async Task<string> GetBooksdescriptionAsync(string isbn)
        {
            if (string.IsNullOrEmpty(isbn))
                return "no isbn provided";
            // try catch block
            try
            {
                // endpoint url 
                string Url = $"https://www.googleapis.com/books/v1/volumes?q=isbn:{isbn}";

                var response = await _httpClient.GetAsync(Url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(content))
                    {
                        JsonElement root = doc.RootElement;
                        JsonElement items;
                        if (root.TryGetProperty("items", out items) && items.GetArrayLength() > 0)
                        {

                            var volumeInfo = items[0].GetProperty("volumeInfo");

                            string description = volumeInfo.TryGetProperty("description", out JsonElement descElem)
                                ? descElem.GetString() ?? "No description available."
                                : "No description available.";


                            string publishedDate = volumeInfo.TryGetProperty("publishedDate", out JsonElement dateElem)
                                ? dateElem.GetString() ?? "Unknown year"
                                : "Unknown year";


                            return $"Published: {publishedDate}\nDescription: {description}";

                        }
                    }
                    return "No description found for the provided ISBN.";
                }
                else if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    return "Google Books API quota exceeded. External details are temporarily unavailable.";
                }
                else
                {
                    return $"Error fetching data: {response.ReasonPhrase}";
                }



            }
            // catching exceptions
            catch (Exception ex)
            {
                return $"Exception occurred: {ex.Message}";

            }
        }
    }
}

