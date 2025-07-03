using GE_Flipper.Data;
using GE_Flipper.Models;
using System.Text.Json;

namespace GE_Flipper
{
    //Class for getting prices inherits background services
    public class PriceGetter : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory; //Variable for holding scope service to use DB
        private readonly ILogger<PriceGetter> _logger; //Variable for holding logging service to record logs

        //Constructor for the class. Require scope and logger as parameters to intialize the services
        public PriceGetter(IServiceScopeFactory scopeFactory, ILogger<PriceGetter> logger)
        {
            _scopeFactory = scopeFactory; //Sets variable value with parameter in constructor in order to use scope service
            _logger = logger; //Sets variable value with parameter in constructor in order to use logging service
        }

        //Overrides the abstract method from background. Passes the parameter for cancellation token
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Program started. Trying to get price."); //Logs that the program is starting

            while(!stoppingToken.IsCancellationRequested) //While stopping token cancellation is false executes the following
            {
                try //Try the following
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var database = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); //Variable for manipulating database using scope services
                        var httpClient = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>(); //Varaible for getting http client to use for API
                        await addPrice(database, httpClient); //Calls method for adding prices to db, passing parameter for database and httpClient
                    }
                }
                catch (Exception e) //Catches exception
                {
                    _logger.LogInformation($"{e} \n Error occured!"); //Store error in log
                }
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); //Delay the task so it only runs once a day
            }

     
        }
        private async Task addPrice(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            var client = httpClientFactory.CreateClient();
            var itemEntries = context.Items.ToList();

            foreach (var item in itemEntries) 
            {
                try //Try the following
                {
                    var apiLink = await client.GetAsync($"https://secure.runescape.com/m=itemdb_oldschool/api/graph/{item.GameId}.json"); //Makes request using the link to get API values
                    if (apiLink.IsSuccessStatusCode) 
                    {
                        var getAPI = await apiLink.Content.ReadAsStringAsync(); //Gets data from API as string
                        using var parseAPI = JsonDocument.Parse(getAPI); //Parse the string from API
                        var osrsItemPrice = parseAPI.RootElement.GetProperty("daily"); //Gets data associated with item 
                        var currentPriceString = osrsItemPrice.EnumerateObject().Last(); //Gets the latest price as string
                        int currentPrice = currentPriceString.Value.GetInt32(); //Converts string to int
                        bool dayExists = context.Prices.Any(p => p.ItemId == item.ItemId && p.Date.Date == DateTime.UtcNow.Date); //Boolean to check if item already has a price for the day
                        if (!dayExists) //If price does not exists for the day does the following
                        {
                            var price = new Price //Variable for new price entry
                            {
                                ItemId = item.ItemId, //Set item id for FK
                                CurrentPrice = currentPrice, //Set current price
                                Date = DateTime.UtcNow //Sets the date with current date 
                            };
                            context.Prices.Add(price); //Adds entry to the price table
                            await context.SaveChangesAsync(); //save changes to db
                        }
                    }
                }
                catch (Exception e) //Catches exception
                {
                    _logger.LogInformation($"{e} \n Error occured!"); //Store error in log
                }
            }
        }
    }
}
