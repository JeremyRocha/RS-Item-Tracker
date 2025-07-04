**Application Summary**
Allows user to create categories to hold items. User can then create entries for item by inputting the game id. Once item is in the database it will start tracking the prices of item. In the table for items it diplays the item name, image and price so user can see what items have value. Potential for some improvement but price tracking is implemented which is the main goal.

**Models**
- Category model is setup and should not need any changes
- Item model is setup and should only need changes if we want to add an item description
- Model for prices is working and should not need any changes

**Controllers**
- Category was setup through scaffolding and is working as intended
- Item controller was also setup through scaffolding, however some changes needed to be made in order to get name and image url from API when creating. Changes also had to be made in order to display the current price in the index view for items. Controller is working as intended right now and should only need changes if we need to add a feature that effects item controller.
- Price controller is not setup and won't be setup using scaffolding because we want it to only have one view that changes dynamically based on the item that is selected. Because we are populating the price table in the background through code we aren't adding CRUD views for this model.

**Views**
- Category views are generated and working as intended
- Item views are generated and working as intended. Made changes from original index in order to display the current price of the item. Would like to add a link to a price view but currently not setup.
- Price view not currently setup but should be a single view that will change dynamically based on the item selected. The view should display a graph showing how the price is changing for that item.

**Background service**
- Created a class that inherits from background services in order to get the current price of items automatically. The code runs every 24 hours to grab the price for items in the item table, it then save that price entry to the price table.  

**Javascript Notes**
- Want to add dynamic fill in for the name and item image url when creating an entry for items
- Minor tweaks need to be made to the datatables used.

**CSS Notes**
- Header would like to add hover to link and add logo before app name
- Footer is done, maybe some minor tweaks to the spacing
- Home page finished. Maybe some minor changes but overal its good
- Category and Items index view is finished. Need minor tweaks to other views
- Login/Register need some work. That will come soon.

**Entity Relationship Diagram**
- One Category hold many Items and one Item holds many prices
- Potential for more relationships for user or tracking but we are keeping it simple for now and seeing what is covered in class
![02cfd0c98237735b7a164e52b556a6e8](https://github.com/user-attachments/assets/d5f76047-37ae-4119-9cec-a3952df2563a)
