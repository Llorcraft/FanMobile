using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Essentials;
using FanApp.Models;

namespace FanApp.Services
{
    public class DataStore
    {
        HttpClient client;

        public DataStore()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://fandatamanager.com.mialias.net/");
        }

        bool IsConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;
        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            if (forceRefresh && IsConnected)
            {
                var json = await client.GetStringAsync($"api/item");
                //items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<Item>>(json));
            }

            return null;
        }

        public async Task<UserModel> Login (UserModel user)
        {
            if (IsConnected)
            {
                var serializedItem = JsonConvert.SerializeObject(user);
                var response = await client.PostAsync("login.php", new StringContent(serializedItem, Encoding.UTF8, "application/json"));
                if (!response.IsSuccessStatusCode) return null;
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserModel>(json);
            }
            return null;
        }

        public async Task<UserModel> Register(UserModel user, bool isNew = true)
        {
            if (IsConnected)
            {
                var serializedItem = JsonConvert.SerializeObject(user);
                var response = await client.PostAsync(isNew ? "register.php" : "profile.php", new StringContent(serializedItem, Encoding.UTF8, "application/json"));
                if (!response.IsSuccessStatusCode) throw new Exception(response.StatusCode == System.Net.HttpStatusCode.Conflict 
                        ? $"Ha fallado el registro porque existe un usuario con el email {user.Email}"
                        : "Ha fallado el registro porque ha ocurrido un error interno en el servidor.");
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserModel>(json);
            }
            return null;
        }

        public async Task<Item> GetItemAsync(string id)
        {
            if (id != null && IsConnected)
            {
                var json = await client.GetStringAsync($"api/item/{id}");
                return await Task.Run(() => JsonConvert.DeserializeObject<Item>(json));
            }

            return null;
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            if (item == null || !IsConnected)
                return false;

            var serializedItem = JsonConvert.SerializeObject(item);

            var response = await client.PostAsync($"api/item", new StringContent(serializedItem, Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            if (item == null || item.Id == null || !IsConnected)
                return false;

            var serializedItem = JsonConvert.SerializeObject(item);
            var buffer = Encoding.UTF8.GetBytes(serializedItem);
            var byteContent = new ByteArrayContent(buffer);

            var response = await client.PutAsync(new Uri($"api/item/{item.Id}"), byteContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            if (string.IsNullOrEmpty(id) && !IsConnected)
                return false;

            var response = await client.DeleteAsync($"api/item/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}
