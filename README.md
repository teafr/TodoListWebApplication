# To-do List Web Application

## Web Application demonstration

#### CRUD operations with to-do lists and tasks

#### Tasks filtration, pagination and searching

#### Registration and Login (with validation)

#### Forgot password

#### Assign task to another user

#### Add/remove editor to list

## Backend

Application has four layers:
- [UI](./TodoListApp.WebApp) (ASP.NET WEB APP)
- [API](./TodoListApp.WebApi) (ASP.NET WEB API)
- [Database](./TodoListApp.Database) (Class Libarary)
- [API Client](./TodoListApp.ApiClient) (Class Libarary)

<br/>All custom services and ASP.NET services were used in project with help of DI container. So services were injected into controllers and were used as interfaces to follow dependency inversion. Dependencise were added to service collection with help of extension methods in classes [ServiceCollectionExtension (API)](./TodoListApp.WebApi/Extensions/ServiceCollectionExtension.cs) and [ServiceCollectionExtension (APP)](./TodoListApp.WebApp/Extensions/ServiceCollectionExtension.cs)<br/>

In this project Serialog were used for logging to track user's actions in console. Serialog were configured to host of API and APP in extension method of the classes [HostExtension (API)](./TodoListApp.WebApp/Extensions/HostExtension.cs) and [HostExtension (APP)](./TodoListApp.WebApi/Extensions/HostExtension.cs).<br/>

Since in the project four layers, there are different models for all of them. So it was necessery to provide [ModelsExtension (API)](./TodoListApp.WebApi/Extensions/ModelsExtension.cs) and [ModelsExtension (APP)](./TodoListApp.WebApp/Extensions/ModelsExtension.cs) to easily and without dublication convert objects from one type to another.<br/><br/>

#### WEB Application (UI)
UI layer has three main Controllers:
- [Account](./TodoListApp.WebApp/Controllers/AccountController.cs)
- [TodoLists](./TodoListApp.WebApp/Controllers/TodoListsController.cs)
- [Tasks](./TodoListApp.WebApp/Controllers/TasksController.cs)

[AccountController](./TodoListApp.WebApp/Controllers/AccountController.cs) gives user possibility to register, login, change password and log out. All of it were made with help of ASP.NET Entity Framework Identity tools such as UserManager and SignInManager. Token is generated after authorization and is stored in cookies during hour or seven days (in case user check "remember me"). For making requests to API through API Client, token is necessery, because without valid token in header, response would be with status code 401. [AuthHeaderHandler](./TodoListApp.WebApp/Handlers/AuthHeaderHandler.cs) (inherited from DelegatingHandler) checks if token is valid and put it to header of request. It uses [JwtTokenGenerator](./TodoListApp.WebApp/Helpers/JwtTokenGenerator.cs) to generate new token in case token in cookies expired, but in the same time user checked "Remember me" checkbox. Also Account controller works with [EmailSender](./TodoListApp.WebApp/Services/EmailSender.cs) to sent messages after registration (user can confirm email) and also to use functionality of Forgot Password (change password after email confirmation). All forms have validation of inputs, so user instantly sees an error if there is a mistake. With help of Identity, when user tries to use method which has Authorize attribute - user is redirected to Login.<br/>

TodoLists and Tasks controllers have exception handling and logging. With help of UserManager some methods can check user for existance or get current user. [TodoListsController](./TodoListApp.WebApp/Controllers/TodoListsController.cs) uses [TodoListApiClientService](./TodoListApp.ApiClient/Services/TodoListApiClientService.cs) to manipulate with to-do lists. Controller has CRUD operations and deleting/adding operations with editors. [TasksController](./TodoListApp.WebApp/Controllers/TasksController.cs) uses [TaskApiClientService](./TodoListApp.ApiClient/Services/TaskApiClientService.cs) to manipulate with tasks. Controller contains CRUD operations. Index action has filtration and pagination of all tasks, assiggned to user. There are an actions for managing comment/tag, assign of the task, searching tasks and changing status of the task.<br/>

#### WEB API

#### Database
In this project were used Microsoft SQL Server. Application has two databases: TodoListsDB and UsersDB (identity). Connection Strings are storing in appsettings.json.
TodoListsDB stores to-do lists and tasks. UsersDB stores information about users.

#### API Client
This project was created to send requests to API

#### Models

#### Nuget packeges

## FrontEnd

#### Views

#### Java Script

#### Bootstrap
