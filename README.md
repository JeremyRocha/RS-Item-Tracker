**Azure Link**: https://geflipper20250625155408-b0ckegd0cufte0au.canadacentral-01.azurewebsites.net/

**Application Summary**
The application tracks prices for items from osrs. Items are added into the database by authorized users. Once the item is added to the database, the application will grab a new price for the item everday. Anyone can see the current item price in the item table but only authorized users can view the price history of an item. The application helps user determine the price of an item and helps them make informed decision on when to sell by looking at the item trend.

**Models**
- Category model holds all data related to a category such as category id, name and description
- Item model holds all data related to an item such as item id, name, image, game id and category id
- Model for prices holds all data related to a price entry such as price id, current price, date, and item id

**Controllers**
- Category controller implemented through scaffolding. Performs all necessary CRUD operations in order to make, edit or delete a category. User must be admin to perform operations.
- Item controller implmented through scaffolding. Perform all necessary CRUD operations in order to make, edit or delete an item. Changed Create method to implement api so user only has to put in the game id for an item and the method will grab the api. Changed index method to also return the current price of the item for display in table. Added method to the bottom to convert api to json for Javascript due to CORS issue with the api. User must be admin to perform crud operations.
- Price controller is setup so it has one index method that takes a parameter for game id which in then uses to get the item name for title in view and then gets all prices stored in the db that are related to the item to be used in a graph. User must be authorized to view price history.

**Views**
- Category views are created through scaffolding for each CRUD operation. Minor changes where made to implement datatables into the index so the table is more user friendly. Minor changes where also made to a couple under view to hide links from user who not authorized to use them.
- Item views are created through scaffolding for each CRUD operation. Changes where made to index to implment datatables here as well for the item table. Changes where also made to index view to implement current price of an item and also a function to change how the numbers are formatted. Changes were also made to create to make name and image fields read only and then added an event to game id field using Javascript to show the user what the other two fields are populated with. Minor changes where made to view where need to hide links for user who aren't authorized or admin.
- Price has a single view (index) it uses Javascript to populate a graph from chart js. The Javascript parse the db to get all the prices associated to an item and then creates a graph with that data to show user pricing trends.

**Background service**
- Created a class that inherits from background services in order to get the current price of items automatically. The service runs every 24 hours to grab the price. It has two method one for getting scoping the database and http client and storring them in variables in order to be able to munipulate them. It then calls the other method passing those two variable so that method can fetch the price and add that price to the database.  

**Login/Registration**
- Created crud operations for registration using scaffolding, registration is working as intended and users are able to make accounts.
- Login is working as intended and users are able to login so long as they use a registered account. User are also able to sign in with github, facebook, and google.
- Set role of admin and customer. Users who are admin are able to access and perform crud operations are logged in. Customers is the default role of using if they login they can see price history.

**CSS Notes**
- Tired to make UI user friendly for both normal user and admins
- Site should be responsive on almost all ends
- Tried to keep colors used to be similar to the ones used by OSRS

**Entity Relationship Diagram**
- One Category hold many Items and one Item holds many prices
![02cfd0c98237735b7a164e52b556a6e8](https://github.com/user-attachments/assets/d5f76047-37ae-4119-9cec-a3952df2563a)

**API and JS Libraries Used**
- https://www.chartjs.org/
- https://datatables.net/
- https://runescape.wiki/w/Application_programming_interface
