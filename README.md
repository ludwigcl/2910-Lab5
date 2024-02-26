# 2910-Lab5

Well, I clearly need more practice with Github, as all of the notations I had documented are no longer here because I failed to commit them. 

I will try and summarize the process for this console app in order to document my challenges. 

First, I had to get the API key working, which wasn't difficult. The Developers have done an excellent job with their documentation and getting the result to return was not difficult. They even have built in return limits and pagination functions so I didn't really think I would have to do all that much.

Once I got the return, I started with my formatting, concentrating on the Common Name of the plant as the only thing to be display, but catching the plantID as a key  so I could use it later. I also wrote a little method to grab the links that are return with each JSON. These links allow me to pass through the correct link for the next page based on the current page, again, REALLY handy. 

I had some frsutrationg with the next page functionality. The Links that are returned make little sense to me in where they chose to truncate. I essentially had to trim it based a character in the link so that I could pass it back through as the tail of the URL, but once I figured this out I was really pleased with the results. 

Now that I had a list of 30 plant names and the ability to paginate through multiple pages reliably I needed to make the method to display information about a selected plant. This gave me far more trouble that I originally had thought it would because of the various multivalue types being returned in the JSON. THe calls were never an issue, just getting the data to present correctly. However, after about an hour of fiddling with it I managed to get it to print out the few values I wanted to see on the screen. Then put in an escape button for the menu. 

Now I needed to make the search method, which was really just redefining a couple of variables to capture a user input and then pass it through to the methods for listing that I had already built. This allows a user to search for a common name like "Daisy" for example, and get a list of results that match it based on a few different fields in the JSON. Really good stuff. Getting the API formatting down took me about an hour and a half to ensure the pagination functions were still good without having to rewrite everything. 

Now I am finishing up, just putting in a couple of menu options, cleaning up formatting, making some colors here and there, and escape functionalities as well. 

All in all it was a fun project!
