**Application Summary**


**Scaffolding, Model, Controller**
- Category Controller and model is working and view were scaffolded. This should need no changes.
- Item Controller and model is working right now and views are scaffolded. Table view will need editing once prices is working.
- Item controller might need editing once we understand how api's working. Might be easier just to have ID get filled in and have api get the rest of the info using api's.
- Price only has model currently because we would like to get prices every so often through api and then add them to the table.
- Price should only be added through code so it shouldn't require any views.

**CSS Notes**
- Header would like to add hover to link and add logo before app name
- Footer is done, maybe some minor tweaks to the spacing
- Home page finished. Maybe some minor changes but overal its good
- Category and Items index view is finished. Need minor tweaks to other views
- Login/Register need some work. That will come soon.

**Entity Relationship Diagram**
- One Category hold many Items and one Item holds many prices
- Price only has model right now because we want to populate it using api so we are waiting to see if that gets covered in class
![02cfd0c98237735b7a164e52b556a6e8](https://github.com/user-attachments/assets/d5f76047-37ae-4119-9cec-a3952df2563a)
