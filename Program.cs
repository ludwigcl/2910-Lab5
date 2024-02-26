using System.Text.Json;

namespace Lab5
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string trefleToken = "?token=ZwlviJNUAXtzmpLG6AfbsQK1-HckXmkz0RGSdk2iFWY";


        static async Task Main(string[] args)
        {
            try
            {
                Console.ResetColor();
                await MainMenu(trefleToken);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            catch (JsonException e)
            {
                Console.WriteLine("\nJSON Parsing Exception Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }
        /// <summary>
        /// Displays a list of plant names fetched from an API. Allows navigation through the list and supports both pagination and search result display.
        /// </summary>
        /// <param name="trefleToken">The token used for authenticating with the Trefle API.</param>
        /// <param name="urlTail">The additional URL parameters used for pagination or search queries.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        static async Task DisplayPlantNames(string trefleToken, string urlTail = null)
        {
            // Show loading animation for fetching plant names
            for (int i = 0; i < 2; i++)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("Fetching plant names.");
                for (int j = 0; j < 3; j++)
                {
                    Thread.Sleep(250); // Wait for a quarter second
                    Console.Write("."); // Print dot for loading effect
                    Thread.Sleep(250); // Wait for another quarter second
                }
                Console.Clear(); // Clear the console to prepare for actual data display
            }

            // Fetch initial data from the API
            APICallData currentData = await GetPlants(trefleToken, urlTail);
            Console.ResetColor(); // Reset the console color to default

            // Initialize variables for navigation
            int selectedPlantIndex = 0; // Index of the selected plant in the list
            string currentPage = urlTail?.Split('=').LastOrDefault() ?? "1"; // Current page number or search term
            string totalPages = "unknown"; // Default value if the total number of pages is unknown

            while (true)
            {
                // Fetch current data again if it's null or empty
                if (currentData == null || !currentData.PlantNamesWithIDs.Any())
                {
                    currentData = await GetPlants(trefleToken, urlTail);
                }

                // Attempt to get the total number of pages from the 'last' URL
                if (currentData.Links.TryGetValue("last", out string lastUrl))
                {
                    totalPages = lastUrl.Split('=').Last();
                }

                Console.Clear(); // Clear the console for a clean slate

                // Check if the currentPage is a number to determine if we are showing a page number or a search term
                bool isPageNumber = int.TryParse(currentPage, out int pageNumber);

                // Display the title with the current page number or search query
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                if (isPageNumber)
                {
                    Console.WriteLine($"Page {currentPage} of {totalPages}\n");
                }
                else
                {
                    Console.WriteLine($"Showing search results for: {currentPage}\n");
                }
                Console.ResetColor(); // Reset the console color to default

                // Display list of plant names with their IDs
                var plantEntries = currentData.PlantNamesWithIDs.ToList();
                for (int i = 0; i < plantEntries.Count; i++)
                {
                    // Highlight the selected plant
                    if (selectedPlantIndex == i)
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    Console.WriteLine("\t" + plantEntries[i].Value); // Display the plant name

                    Console.ResetColor(); // Reset the color for the next item
                }

                // Display navigation commands
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n\nNavigation Commands:");
                Console.ResetColor();
                Console.WriteLine("Arrow Up/Down: Navigate plants");
                Console.WriteLine("Arrow Left/Right: Previous/Next page");
                Console.WriteLine("F: First page\nL: Last page\nEnter: Select plant");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\nPress Escape to return to the Main Menu.");
                Console.ResetColor();

                // Read the user input for navigation
                var key = Console.ReadKey().Key;

                // Handle navigation
                if (key == ConsoleKey.DownArrow)
                {
                    selectedPlantIndex = Math.Min(selectedPlantIndex + 1, plantEntries.Count - 1);
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    selectedPlantIndex = Math.Max(selectedPlantIndex - 1, 0);
                }
                else if (key == ConsoleKey.RightArrow)
                {
                    if (currentData.Links.TryGetValue("next", out string nextUrl) &&
                        nextUrl != lastUrl)
                    {
                        currentData = await GetPlants(trefleToken, nextUrl);
                        currentPage = nextUrl.Split('=').LastOrDefault();
                    }
                }
                else if (key == ConsoleKey.LeftArrow)
                {
                    if (currentData.Links.TryGetValue("prev", out string prevUrl))
                    {
                        currentData = await GetPlants(trefleToken, prevUrl);
                        currentPage = prevUrl.Split('=').LastOrDefault();
                    }
                }
                else if (key == ConsoleKey.F)
                {
                    if (currentData.Links.TryGetValue("first", out string firstUrl))
                    {
                        currentData = await GetPlants(trefleToken, firstUrl);
                        currentPage = firstUrl.Split('=').LastOrDefault();
                    }
                }
                else if (key == ConsoleKey.L)
                {
                    currentData = await GetPlants(trefleToken, lastUrl);
                    currentPage = lastUrl.Split('=').LastOrDefault();
                }
                else if (key == ConsoleKey.Enter)
                {
                    var selectedPlantId = Convert.ToInt32(plantEntries[selectedPlantIndex].Key);
                    await GetPlantDetails(trefleToken, selectedPlantId);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nPress any key to return...");
                    Console.ResetColor();
                    Console.ReadKey(intercept: true);
                }
                if (key == ConsoleKey.Escape)
                {
                    Console.Clear(); // Clear the console when returning to the main menu
                    return; // Exit the method
                }
            }
        }
        /// <summary>
        /// Fetches and displays detailed information about a specific plant from the Trefle API.
        /// </summary>
        /// <param name="trefleToken">The access token for the Trefle API.</param>
        /// <param name="plantId">The unique identifier for the plant whose details are to be fetched.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        static async Task GetPlantDetails(string trefleToken, int plantId)
        {
            // Hardcoded token for demonstration. In a real application, this should be securely handled and not exposed.
            trefleToken = "?token=ZwlviJNUAXtzmpLG6AfbsQK1-HckXmkz0RGSdk2iFWY";
            // Constructs the URL to fetch plant details by combining the base URL, plant ID, and the access token.
            string apiUrl = $"https://trefle.io/api/v1/plants/{plantId}{trefleToken}";

            // Executes a GET request to the Trefle API and awaits the response.
            var responseBody = await client.GetStringAsync(apiUrl);
            // Deserializes the JSON response into a dynamic data structure.
            var jsonData = JsonSerializer.Deserialize<JsonElement>(responseBody);

            // Prepares a new APICallData instance to store the fetched plant details.
            var result = new APICallData();

            // Extracts and assigns plant details from the JSON response if the "data" property exists.
            if (jsonData.TryGetProperty("data", out JsonElement data))
            {
                result.ScientificName = data.GetProperty("scientific_name").GetString();
                result.CommonName = data.TryGetProperty("common_name", out JsonElement commonNameElement) ? commonNameElement.GetString() : "Not available";
                result.FamilyCommonName = data.GetProperty("family_common_name").GetString() ?? "N/A";

                // Handles nullable boolean for "vegetable" property.
                var vegetableProperty = data.GetProperty("vegetable");
                result.Vegetable = vegetableProperty.ValueKind != JsonValueKind.Null ? vegetableProperty.GetBoolean() : false;

                result.Observations = data.GetProperty("observations").GetString() ?? "N/A";
                result.Year = data.GetProperty("year").GetInt32();
                result.Author = data.GetProperty("author").GetString() ?? "N/A";

                // Parses native distribution regions if available.
                if (data.TryGetProperty("distributions", out JsonElement distributions) && distributions.TryGetProperty("native", out JsonElement native))
                {
                    foreach (var region in native.EnumerateArray())
                    {
                        result.NativeDistribution.Add(region.GetProperty("name").GetString());
                    }
                }
            }

            // Clears the console and sets up for displaying plant details.
            Console.Clear();

            // Displaying the common name of the plant in uppercase as a title.
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{result.CommonName.ToUpper()}\n");
            Console.ResetColor();

            // Utilizes a helper method to print key/value pairs of plant details.
            PrintKeyValuePair("Scientific Name", result.ScientificName);
            PrintKeyValuePair("Family Common Name", result.FamilyCommonName);
            PrintKeyValuePair("Vegetable", result.Vegetable ? "Yes" : "No");
            PrintKeyValuePair("Observations", result.Observations);
            PrintKeyValuePair("Year of Classification", result.Year.ToString());
            PrintKeyValuePair("Author of Classification", result.Author);
            PrintKeyValuePair("Native Distribution", result.NativeDistribution.Count > 0 ? string.Join(", ", result.NativeDistribution) : "N/A");

            Console.ResetColor();
        }
        /// <summary>
        /// Prints a key-value pair in a formatted manner to the console.
        /// </summary>
        /// <param name="key">The key or label for the data to be printed.</param>
        /// <param name="value">The value associated with the key.</param>
        static void PrintKeyValuePair(string key, string value)
        {
            // Sets the color for the key text.
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{key}: ");
            // Sets the color for the value text.
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(value);
            // Resets the console color to its default.
            Console.ResetColor();
        }
        /// <summary>
        /// Fetches plant data from the Trefle API, either all plants or based on specific criteria provided in the URL tail.
        /// </summary>
        /// <param name="trefleToken">The token used for authenticating with the Trefle API. This should include the API key.</param>
        /// <param name="urlTail">Additional URL parameters for filtering or pagination. Defaults to null for fetching all plants.</param>
        /// <returns>A Task that returns an APICallData object containing the fetched plant data.</returns>
        static async Task<APICallData> GetPlants(string trefleToken, string urlTail = null)
        {
            // Construct the base URL for the API call
            string apiUrl = $"https://trefle.io/api/v1/plants{trefleToken}";

            // Append URL parameters if provided, otherwise default to ordering by common name in ascending order
            if (urlTail != null)
            {
                apiUrl += $"{urlTail}";
            }
            else
            {
                apiUrl += "&order[common_name]=asc";
            }

            // Perform the API call and await the response
            var responseBody = await client.GetStringAsync(apiUrl);

            // Deserialize the JSON response into a JsonElement object
            var jsonData = JsonSerializer.Deserialize<JsonElement>(responseBody);

            // Initialize the result object to store the fetched data
            var result = new APICallData();

            // Process the 'data' property of the JSON response if it exists and is an array
            if (jsonData.TryGetProperty("data", out JsonElement data) && data.ValueKind == JsonValueKind.Array)
            {
                foreach (var plant in data.EnumerateArray())
                {
                    // Extract the common name and ID of each plant
                    var commonName = plant.GetProperty("common_name").GetString();
                    var id = plant.GetProperty("id").GetInt32(); // Parse the ID as an integer

                    // Only add plants with a non-empty common name to the result
                    if (!string.IsNullOrEmpty(commonName))
                    {
                        result.PlantNamesWithIDs[id] = commonName;
                    }
                }
            }

            // Process the 'links' property of the JSON response for pagination
            if (jsonData.TryGetProperty("links", out JsonElement links))
            {
                foreach (var link in links.EnumerateObject())
                {
                    // Extract the URL for each pagination link and reformat it for use in subsequent API calls
                    var fullUrl = link.Value.GetString();
                    var questionMarkIndex = fullUrl?.IndexOf('?');
                    var afterQuestionMark = fullUrl.Substring(questionMarkIndex.Value + 1);
                    result.Links[link.Name] = $"&{afterQuestionMark}";
                }
            }

            // Return the populated result object
            return result;
        }
        /// <summary>
        /// Initiates a search for plants by their name using the Trefle API.
        /// </summary>
        /// <param name="trefleToken">The token used for authenticating with the Trefle API. This should include the API key.</param>
        /// <param name="plantName">The name of the plant to search for.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        static async Task PlantSearch(string trefleToken, string plantName)
        {
            int i = 1; // Initialize the page number to 1 for the search query

            // Modify the Trefle token to include the search endpoint and the API key
            trefleToken = $"/search{trefleToken}";

            // Construct the URL parameters for the search, including ascending order by common name, page number, and search query
            string tail = $"&order[common_name]=asc&page={i}&q={plantName}";

            // Call the DisplayPlantNames method to display the search results
            await DisplayPlantNames(trefleToken, tail);
        }
        /// <summary>
        /// Displays the main menu of the Plant Information System and handles user input to navigate different functionalities.
        /// </summary>
        /// <param name="trefleToken">The token used for authenticiating with the Trefle API.</param>
        /// <returns>A Task representing the asynchronous operation of the main menu.</returns>
        static async Task MainMenu(string trefleToken)
        {
            // Infinite loop to keep the menu running until the user decides to exit
            while (true)
            {
                // Display the welcome message and menu options to the user
                Console.WriteLine("Welcome to the Plant Information System!");

                // Change console text color for better visibility of the options section
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("\nPlease choose an option:");
                // Reset color to default for general text
                Console.ResetColor();

                // Options for the user to select from
                Console.WriteLine("1. Display a list of all plants");
                Console.WriteLine("2. Search for a plant by name");
                Console.WriteLine("3. Exit");

                // Change console text color for the input prompt
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write("\n\nEnter your choice (1-3): ");
                // Reset color to default after the prompt
                Console.ResetColor();

                // Read the user's choice from the console input
                var choice = Console.ReadLine();

                // Handle the user's choice using a switch statement
                switch (choice)
                {
                    case "1":
                        // Clear the console for cleaner output display
                        Console.Clear();
                        Console.ResetColor();
                        // Display names of all plants by calling the DisplayPlantNames method
                        await DisplayPlantNames(trefleToken);
                        break;
                    case "2":
                        // Clear the console and set up for a new input prompt
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("Enter a plant name to search for: ");
                        Console.ResetColor();

                        // Read the plant name to search for from the console input
                        var plantName = Console.ReadLine();
                        Console.Clear();
                        // Perform the plant search with the given plant name
                        await PlantSearch(trefleToken, plantName);
                        break;
                    case "3":
                        // Display a goodbye message and exit the loop, thus ending the program
                        Console.WriteLine("Thank you for using the Plant Information System. Goodbye!");
                        return;
                    default:
                        // Handle invalid input by prompting the user to enter a valid choice
                        Console.WriteLine("Invalid choice, please enter 1, 2, or 3.");
                        break;
                }
            }
        }

    }
}
