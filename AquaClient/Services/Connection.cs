using AquaClient.Classes.Responses;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Controls;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace AquaClient.Services
{
    public class Connection
    {
        private HttpClient _client = new();
        public string UriApi { get; set; } = "";
        public string Token { get; set; } = "";
        public int UserId { get; set; } = -1;

        private Dictionary<string, string> _routes = new Dictionary<string, string>()
        {
            ["login"] = "auth",
            ["signup"] = "signup",
            ["controls"] = "status",
            ["video"] = "video_feed",
            ["user"] = "user",
            ["logout"] ="logout",
        };
        
        public Connection(string uriApi="")
        {
            UriApi = uriApi;
        }

        ~Connection()
        {
            _client?.Dispose();
        }

        public string? GetUriByName(string action)
        {
            if (_routes.ContainsKey(action))
                return $"{UriApi}{_routes[action]}";
            return null;
        }

        private void AddToken(string token)
        {
            Token = token;
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<ResponseBase<ConfirmCodeDataResponse?>?> Login(string phone, string password)
        {
            try
            {
                var user = new { phone, password };
                HttpContent content = JsonContent.Create(user);
                // устанавливаем заголовок 
                var response = await _client.PostAsync(UriApi + _routes["login"], content);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return new ResponseBase<ConfirmCodeDataResponse?> { Error = -2, Message = "Ошибка сервера", Data = null };
                }
                var data = await response.Content.ReadFromJsonAsync<ResponseBase<ConfirmCodeDataResponse?>>();
                if (data != null && data.Error == 0)
                {
                    AddToken(data.Data.Token);
                    UserId = data.Data.UserId;
                }
                return data;
            }
            catch (Exception ex)
            {
                return new ResponseBase<ConfirmCodeDataResponse?> { Error = -1, Message = $"Сетевая ошибка {ex.Message}", Data = null };
            }
        }

        public async Task<ResponseBase<object?>?> Logout()
        {
            try
            {
                var response = await _client.PostAsync(UriApi + _routes["logout"], null);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return new ResponseBase<object?> { Error = -2, Message = "Ошибка сервера", Data = null };
                }
                var data = await response.Content.ReadFromJsonAsync<ResponseBase<object?>?>();
                if (data?.Error == 0)
                {
                    Token = "";
                    UserId = -1;
                }
                return data;
            }
            catch (Exception ex)
            {
                return new ResponseBase<object?> { Error = -1, Message = $"Сетевая ошибка {ex.Message}", Data = null };
            }
        }

        public async Task<ResponseBase<object?>?> Signup(string phone, string email, string name, string password, string? surname, string? middlename)
        {
            try
            {
                var regData = new { phone, email, password, name, surname, middlename };
                HttpContent content = JsonContent.Create(regData);
                // устанавливаем заголовок 
                var response = await _client.PostAsync(UriApi + _routes["signup"], content);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return new ResponseBase<object?> { Error = -2, Message = "Ошибка сервера", Data = null };
                }
                var data = await response.Content.ReadFromJsonAsync<ResponseBase<object?>?>();
                return data;
            }
            catch (Exception ex)
            {
                return new ResponseBase<object?> { Error = -1, Message = $"Сетевая ошибка {ex.Message}", Data = null };
            }
        }

        public async Task<ResponseBase<ConfirmCodeDataResponse?>?> SendConfirmCode(string phone, string code)
        {
            try
            {
                var response = await _client.GetAsync($"{UriApi}{_routes["signup"]}?phone={Uri.EscapeDataString(phone)}&code={Uri.EscapeDataString(code)}");
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return new ResponseBase<ConfirmCodeDataResponse?> { Error = -2, Message = "Ошибка сервера", Data = null };
                }
                var data = await response.Content.ReadFromJsonAsync<ResponseBase<ConfirmCodeDataResponse?>?>();
                if (data != null && data.Error == 0)
                {
                    AddToken(data.Data.Token);
                    UserId = data.Data.UserId;
                }
                
                return data;
            }
            catch (Exception ex)
            {
                return new ResponseBase<ConfirmCodeDataResponse?> { Error = -1, Message = $"Сетевая ошибка {ex.Message}", Data = null };
            }
        }

        public async Task<ResponseBase<List<AquaDeviceInfo>?>?> GetDevicesInfo(int? deviceId = null)
        {
            try
            {
                var routeRequest = $"{UriApi}{_routes["controls"]}{(deviceId != null ? $"?deviceId={deviceId}" : "")}";
                var response = await _client.GetAsync(routeRequest);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return new ResponseBase<List<AquaDeviceInfo>?> { Error = -2, Message = "Ошибка сервера", Data = null };
                }
                var data = await response.Content.ReadFromJsonAsync<ResponseBase<List<AquaDeviceInfo>?>?>();
                return data;
            }
            catch (Exception ex)
            {
                return new ResponseBase<List<AquaDeviceInfo>?> { Error = -1, Message = $"Сетевая ошибка {ex.Message}", Data = null };
            }
        }

        public async Task<ResponseBase<AquaDeviceInfo>?>? SetDeviceStatus(int deviceId, int value)
        {
            try
            {
                var deviceInfo = new { deviceId, value};
                HttpContent content = JsonContent.Create(deviceInfo);
                // устанавливаем заголовок 
                var response = await _client.PostAsync(UriApi + _routes["controls"], content);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return new ResponseBase<AquaDeviceInfo> { Error = -2, Message = "Ошибка сервера", Data = null };
                }
                var data = await response.Content.ReadFromJsonAsync<ResponseBase<AquaDeviceInfo>?>();
                return data;
            }
            catch (Exception ex)
            {
                return new ResponseBase<AquaDeviceInfo> { Error = -1, Message = $"Сетевая ошибка {ex.Message}", Data = null };
            }
        }

        public async Task<byte[]> LoadUserAvatar()
        {
            try
            {
                var uri = $"{UriApi}{_routes["user"]}/{UserId}?image=1";
                var response = await _client.GetByteArrayAsync(uri);
                return response;
            }
            catch (Exception ex)
            {
                return [];
            }
        }

        public async Task<ResponseBase<User?>?> GetUserInfo()
        {
            try
            {
                var routeRequest = $"{UriApi}{_routes["user"]}/{UserId}";
                var response = await _client.GetAsync(routeRequest);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return new ResponseBase<User?> { Error = -2, Message = "Ошибка сервера", Data = null };
                }
                var data = await response.Content.ReadFromJsonAsync<ResponseBase<User?>?>();
                return data;
            }
            catch (Exception ex)
            {
                return new ResponseBase<User?> { Error = -1, Message = $"Сетевая ошибка {ex.Message}", Data = null };
            }
        }

        public async Task<ResponseBase<User?>?> SaveUserInfo(string firstName, string middlename, string surname, string phone,
            string email, string password)
        {
            try
            {
                var userData = new {
                    firstName,
                    middlename,
                    surname,
                    phone,
                    email,
                    password
                };
                HttpContent content = JsonContent.Create(userData);
                // устанавливаем заголовок 
                var response = await _client.PostAsync(UriApi + _routes["user"], content);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return new ResponseBase<User?> { Error = -2, Message = $"Ошибка. Код http {response.StatusCode}", Data = null };
                }
                var data = await response.Content.ReadFromJsonAsync<ResponseBase<User?>?>();
                return data;
            }
            catch (Exception ex)
            {
                return new ResponseBase<User?> { Error = -1, Message = $"Сетевая ошибка {ex.Message}", Data = null };
            }
        }


    }
}
