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

[AccountController](./TodoListApp.WebApp/Controllers/AccountController.cs) gives user possibility to register, login, change password and log out. All of it were made with help of ASP.NET Entity Framework Identity tools such as UserManager and SignInManager. Token generates after authorization and stores in cookies during hour or seven days (in case user check "remember me"). For making requests to API through API Client, token is necessery, because without valid token in header, response would be with status code 401. [AuthHeaderHandler](./TodoListApp.WebApp/Handlers/AuthHeaderHandler.cs) (inherited from DelegatingHandler) checks if token is valid and put it to header of request. It uses [JwtTokenGenerator](./TodoListApp.WebApp/Helpers/JwtTokenGenerator.cs) to generate new token in case token in cookies expired, but in the same time user checked "Remember me" checkbox. If some errors accur, handler deletes token from cookies to not store old information. Also Account controller works with [EmailSender](./TodoListApp.WebApp/Services/EmailSender.cs) to sent messages after registration (user can confirm email) and also to use functionality of Forgot Password (change password after email confirmation). All forms have validation of inputs, so user instantly sees an error if there is a mistake. With help of Identity, If user tries to use method which has Authorize attribute - user is redirected to Login.

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
