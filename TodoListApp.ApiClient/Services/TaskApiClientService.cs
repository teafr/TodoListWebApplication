using System.Net.Http.Json;
using TodoListApp.ApiClient.Models;
using TodoListApp.WebApi.Models;

namespace TodoListApp.ApiClient.Services
{
    public class TaskApiClientService : IDisposable
    {
        private readonly string url = "api/tasks/";
        private readonly HttpClient httpClient;
        private bool disposed;

        public TaskApiClientService(ApiClientOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);
            this.httpClient = new HttpClient();
            this.httpClient.BaseAddress = new Uri(options.ApiBaseAdress);
        }

        public async Task<List<TaskApiModel>?> GetTasksAsync()
        {
            return await this.httpClient.GetFromJsonAsync<List<TaskApiModel>>(this.url);
        }

        public async Task<TaskApiModel?> GetTaskByIdAsync(int taskId)
        {
            return await this.httpClient.GetFromJsonAsync<TaskApiModel>(this.url + $"{taskId}");
        }

        public async Task<List<TaskApiModel>?> GetTasksByUserIdAsync(string userId)
        {
            return await this.httpClient.GetFromJsonAsync<List<TaskApiModel>>(this.url + $"user/{userId}");
        }

        public async Task<List<string>?> GetCommentsAsync(int taskId)
        {
            return await this.httpClient.GetFromJsonAsync<List<string>>(this.url + $"{taskId}/comments");
        }

        public async Task<List<string>?> GetTagsAsync(int taskId)
        {
            return await this.httpClient.GetFromJsonAsync<List<string>>(this.url + $"{taskId}/tags");
        }

        public async Task AddTagAsync(int taskId, string tag)
        {
            _ = await this.httpClient.PostAsJsonAsync(this.url + $"{taskId}/tag", tag);
        }

        public async Task AddCommentAsync(int taskId, string comment)
        {
            _ = await this.httpClient.PostAsJsonAsync(this.url + $"{taskId}/comment", comment);
        }

        public async Task UpdateTaskAsync(int taskId, TaskApiModel task)
        {
            _ = await this.httpClient.PutAsJsonAsync(this.url + $"{taskId}", task);
        }

        public async Task UpdateStatusOfTask(int taskId, StatusApiModel status)
        {
            _ = await this.httpClient.PutAsJsonAsync(this.url + $"{taskId}/status", status);
        }

        public async Task UpdateCommentInTask(int taskId, string oldcomment, string newComment)
        {
            _ = await this.httpClient.PutAsJsonAsync(this.url + $"{taskId}/comment/{oldcomment}", newComment);
        }

        public async Task DeleteTaskAsync(int taskId)
        {
            _ = await this.httpClient.DeleteAsync(new Uri(this.url + $"{taskId}"));
        }

        public async Task RemoveTagFromTaskAsync(int taskId, string tag)
        {
            _ = await this.httpClient.DeleteAsync(new Uri(this.url + $"{taskId}/tag/{tag}"));
        }

        public async Task RemoveCommentFromTaskAsync(int taskId, string comment)
        {
            _ = await this.httpClient.DeleteAsync(new Uri(this.url + $"{taskId}/comments/{comment}"));
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.httpClient.Dispose();
                }

                this.disposed = true;
            }
        }
    }
}
