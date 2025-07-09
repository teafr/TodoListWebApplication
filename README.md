# To-do List Web Application

### Web Application demonstration

### Backend

Application has four layers:
- [UI](./TodoListApp.WebApp) (ASP.NET WEB APP)
- [API](./TodoListApp.WebApi) (ASP.NET WEB API)
- [Database](./TodoListApp.Database) (Class Libarary)
- [API Client](./TodoListApp.ApiClient) (Class Libarary)

##### WEB Application (UI)
UI layer has three main Controllers:
- [Account](./TodoListApp.WebApp/Controllers/AccountController.cs)
- [TodoLists](./TodoListApp.WebApp/Controllers/TodoListsController.cs)
- [Tasks](./TodoListApp.WebApp/Controllers/TasksController.cs)

Account controller gives user possibility to registrate, login, change password and log out. All of that were made with help of ASP.NET Entity Framework Identity. After authorisation token is storing in cookies during hour or during seven days (if user tick "remember me"). This token is necessery to make requests to API through API Client. [AuthHeaderHandler](./TodoListApp.WebApp/Handlers/AuthHeaderHandler.cs) checks if token is valid and put it to header. 

##### WEB API

##### Database
In this project were used Microsoft SQL Server. Application has two databases: TodoListsDB and UsersDB (identity). Connection Strings are storing in appsettings.json.
TodoListsDB stores to-do lists and tasks. UsersDB stores information about users.

##### API Client
This project was created to send requests to API

##### Nuget packeges

### FrontEnd

##### Views

##### Java Script

##### Bootstrap
