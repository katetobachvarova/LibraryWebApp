# LibraryWebApp

Application is based on Asp Net Core, EntityFramework 7 and MVC 6.
It uses LocalDb, which is seeded on first startup.(DbContextSeedData.Seed())
You have 3 types of users - Admin, Librarian and RegUser.
RegUser can browse Items and Add to favourites list.
Librarian can create Sections,Titles, Items, ItemMovement + RegUser functionality.
Admin can addd and edit users + RegUser and Librarian functionality.
Roles are implemented on UI and method level.
Concurrency issues - if user edits data that has been modified in the meantime, UI notifies what the changes are and user can decide whether to 
appply new changes or abort operation.
