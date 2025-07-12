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

TodoLists and Tasks controllers have exception handling and logging. Both of them use API client services to sent requests to API. With help of UserManager some methods can check user for existance or get current user. [TodoListsController](./TodoListApp.WebApp/Controllers/TodoListsController.cs) uses [TodoListApiClientService](./TodoListApp.ApiClient/Services/TodoListApiClientService.cs) to manipulate with to-do lists. TodoLists controller has CRUD operations and deleting/adding operations with editors. [TasksController](./TodoListApp.WebApp/Controllers/TasksController.cs) uses [TaskApiClientService](./TodoListApp.ApiClient/Services/TaskApiClientService.cs) to manipulate with tasks. Tasks controller also contains CRUD operations. Index action has filtration and pagination of all tasks, assiggned to user. There are an actions for managing comments/tags, assign of the task, searching tasks and changing status of the task.<br/>

#### WEB API
API layer provides interaction with TodoListDB and follows REST architectural style. <br/>
There are three controllers:
- [BaseController](./TodoListApp.WebApi/Controllers/BaseController.cs)
- [TodoListsController](./TodoListApp.WebApi/Controllers/TodoListsController.cs)
- [TodoListsController](./TodoListApp.WebApi/Controllers/TodoListsController.cs)

[Base](./TodoListApp.WebApi/Controllers/BaseController.cs) controller has method to operate with service successfully. [TodoLists](./TodoListApp.WebApi/Controllers/TodoListsController.cs) and [Tasks](./TodoListApp.WebApi/Controllers/TodoListsController.cs) controllers inherited from [Base](./TodoListApp.WebApi/Controllers/BaseController.cs) controller and have documentations. All actions have attributes for spacifing which status codes can be returnd as response. Also every action has attribute which difines route and http method.

In [ServiceCollectionExtension](./TodoListApp.WebApi/Extensions/ServiceCollectionExtension.cs) was configured JWT Bearer to securely exchange data between API and APP. Because of JWT Bearer Authorization, only authorizezd users can successfuly use API.

#### Databases
In this project were used Microsoft SQL Server. Application has two databases: TodoListsDB and UsersDB (identity). Connection Strings are storing in appsettings.json. In this project was used ORM Entity Framework Core for code-first approach. TodoListsDB stores to-do lists and tasks and has such entities: [TodoList](./TodoListApp.Database/Entities/TodoListEntity.cs), [Task](./TodoListApp.Database/Entities/TaskEntity.cs) and [Status](./TodoListApp.Database/Entities/StatusEntity.cs). UsersDB stores information about users and contains Identity tables, but instead of IdentityUser was used [ApplicationUser](TodoListApp.WebApp/Models/AuthenticationModels/ApplicationUser.cs) which is inherited from IdentityUser.

#### Models
Main models: 

#### Nuget packeges

## FrontEnd

#### Views

#### Java Script

#### Bootstrap
