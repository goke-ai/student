using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Goke.Students.Shared;
using Microsoft.JSInterop;

namespace Goke.Students.Client.Data
{
    // To support offline use, we use this simple local data repository
    // instead of performing data access directly against the server.
    // This would not be needed if we assumed that network access was always
    // available.

    public class LocalPeopleStore
    {
        private readonly HttpClient httpClient;
        private readonly IJSRuntime js;

        public LocalPeopleStore(HttpClient httpClient, IJSRuntime js)
        {
            this.httpClient = httpClient;
            this.js = js;
        }

        public ValueTask<Person[]> GetOutstandingLocalEditsAsync()
        {
            return js.InvokeAsync<Person[]>(
                "localPeopleStore.getAll", "localedits");
        }

        public async Task SynchronizeAsync()
        {
            // If there are local edits, always send them first
            foreach (var editedPerson in await GetOutstandingLocalEditsAsync())
            {
                (await httpClient.PutAsJsonAsync("api/people/details", editedPerson)).EnsureSuccessStatusCode();
                await DeleteAsync("localedits", editedPerson.PersonNumber);
            }

            await FetchChangesAsync();
        }

        public ValueTask SaveUserAccountAsync(ClaimsPrincipal user)
        {
            return user != null
                ? PutAsync("metadata", "userAccount", user.Claims.Select(c => new ClaimData { Type = c.Type, Value = c.Value }))
                : DeleteAsync("metadata", "userAccount");
        }

        public async Task<ClaimsPrincipal> LoadUserAccountAsync()
        {
            var storedClaims = await GetAsync<ClaimData[]>("metadata", "userAccount");
            return storedClaims != null
                ? new ClaimsPrincipal(new ClaimsIdentity(storedClaims.Select(c => new Claim(c.Type, c.Value)), "appAuth"))
                : new ClaimsPrincipal(new ClaimsIdentity());
        }

        public ValueTask<string[]> Autocomplete(string prefix)
            => js.InvokeAsync<string[]>("localPeopleStore.autocompleteKeys", "serverdata", prefix, 5);

        // If there's an outstanding local edit, use that. If not, use the server data.
        public async Task<Person> GetPerson(string personNumber)
            => await GetAsync<Person>("localedits", personNumber)
            ?? await GetAsync<Person>("serverdata", personNumber);

        public async ValueTask<DateTime?> GetLastUpdateDateAsync()
        {
            var value = await GetAsync<string>("metadata", "lastUpdateDate");
            return value == null ? (DateTime?)null : DateTime.Parse(value);
        }

        public ValueTask SavePersonAsync(Person person)
            => PutAsync("localedits", null, person);

        async Task FetchChangesAsync()
        {
            var mostRecentlyUpdated = await js.InvokeAsync<Person>("localPeopleStore.getFirstFromIndex", "serverdata", "lastUpdated", "prev");
            var since = mostRecentlyUpdated?.LastUpdated ?? DateTime.MinValue;
            var json = await httpClient.GetStringAsync($"api/people/changedpeople?since={since:o}");
            await js.InvokeVoidAsync("localPeopleStore.putAllFromJson", "serverdata", json);
            await PutAsync("metadata", "lastUpdateDate", DateTime.Now.ToString("o"));
        }

        ValueTask<T> GetAsync<T>(string storeName, object key)
            => js.InvokeAsync<T>("localPeopleStore.get", storeName, key);

        ValueTask PutAsync<T>(string storeName, object key, T value)
            => js.InvokeVoidAsync("localPeopleStore.put", storeName, key, value);

        ValueTask DeleteAsync(string storeName, object key)
            => js.InvokeVoidAsync("localPeopleStore.delete", storeName, key);

        class ClaimData
        {
            public string Type { get; set; }
            public string Value { get; set; }
        }
    }
}
