﻿using CapstoneIdeaGenerator.Client.Services.Interfaces;
using Blazored.LocalStorage;
using System.Net.Http.Json;
using CapstoneIdeaGenerator.Client.Models.DTO;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Win32;
using System.Net.Http;

namespace CapstoneIdeaGenerator.Client.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthenticationService(HttpClient httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }


        public async Task<Response> LoginAsync(AdminLoginDTO request)
        {
            try
            {
                var result = await _httpClient.PostAsJsonAsync("/api/Authentication/login", request);
                var token = await result.Content.ReadAsStringAsync();

                if (result.IsSuccessStatusCode)
                {
                    await _localStorage.SetItemAsync("token", token);
                    await _authenticationStateProvider.GetAuthenticationStateAsync();
                    return new Response { IsSuccess = true, Message = "Login successful" };
                }
                else
                {
                    return new Response { IsSuccess = false, Message = "Login failed. Please check your credentials." };
                }
            }
            catch (Exception ex)
            {
                return new Response { IsSuccess = false, Message = "An error occurred: " + ex.Message };
            }
        }



        public async Task<bool> RegisterAsync(AdminRegisterDTO request)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Authentication/register", request);
            return response.IsSuccessStatusCode;
        }


        public async Task<Response> ForgotPassword(AdminForgotPasswordDTO request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/Authentication/forgot-password", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<Response>();
                    return result;
                }

                throw new Exception("Failed To Send Forgot Password Request");
            }
            catch (Exception ex)
            {
                throw new Exception("An Error Occurred: " + ex.Message);
            }
        }


        public async Task<string> ResetPassword(AdminPasswordResetDTO request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/Authentication/reset-password", request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }

                throw new Exception("Failed To Send Password Reset Request");
            }
            catch (Exception ex)
            {
                throw new Exception("An Error Occurred: " + ex.Message);
            }
        }


        public async Task<AdminDTO> EditAdminAsync(string email, AdminEditAccountDTO updatedAdmin)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"/api/Authentication/edit/{email}", updatedAdmin);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<AdminDTO>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error editing admin: {ex.Message}");
            }
        }

        public async Task RemoveAdminAsync(string email)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/Authentication/remove/{email}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error removing admin: {ex.Message}");
            }
        }


        public async Task<string> GetAdminNameAsync()
        {
            return await _httpClient.GetFromJsonAsync<string>("/api/Authentication");
        }


        public async Task<IEnumerable<AdminAccountDTO>> GetAllAccountsAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<AdminAccountDTO>>("/api/Authentication/accounts");
        }
    }
}
