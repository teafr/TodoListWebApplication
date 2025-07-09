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
