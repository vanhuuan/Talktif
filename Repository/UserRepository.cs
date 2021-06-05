using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Talktif.Models;

namespace Talktif.Repository
{
    public class UserRepo
    {
        public User_Infor data { get; set; }
        private static UserRepo _Instance;
        public static UserRepo Instance {
            get 
            {
                if(_Instance == null)
                {
                    _Instance = new UserRepo();
                } 
                return _Instance;
            }
            private set { }
        }
        private const string UriString = "https://talktifapi.azurewebsites.net/api/Users/";
        public HttpResponseMessage Sign_Up(SignUpRequest Sr)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                var signUp = client.PostAsJsonAsync("SignUp",Sr);
                signUp.Wait();
                return signUp.Result;
            }
        }
        public HttpResponseMessage Sign_In(LoginRequest lr){
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                //client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var login = client.PostAsJsonAsync("SignIn",lr);
                login.Wait();
                return login.Result;
            }
        }
        public HttpResponseMessage ResetPass(ResetPassRequest resetPassRequest)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                var resetPass = client.PostAsJsonAsync("ResetPass",resetPassRequest);
                resetPass.Wait();
                return resetPass.Result;
            }
        }
        public HttpResponseMessage ResetPasswordEmail(ResetPassEmailRequest resetPassEmailRequest)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                var resetPasswordEmail = client.PostAsJsonAsync("ResetPasswordEmail",resetPassEmailRequest);
                resetPasswordEmail.Wait();
                return resetPasswordEmail.Result;
            }
        }
        public HttpResponseMessage GetAllCountry()
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                var getAllCountry = client.GetAsync("GetAllCountry");
                getAllCountry.Wait();
                return getAllCountry.Result;
            }
        }
        public HttpResponseMessage GetAllCityCountry(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                var getAllCityCountry = client.GetAsync("GetAllCityCountry/"+id);
                getAllCityCountry.Wait();
                return getAllCityCountry.Result;
            }
        }
        public HttpResponseMessage GetUserByID(int ID)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",UserRepo.Instance.data.token);
                var getUserByID = client.PostAsJsonAsync(ID.ToString(),ID);
                getUserByID.Wait();
                return getUserByID.Result;
            }
        }
        public HttpResponseMessage UpdateUserInfor(UpdateInfoRequest updateInfoRequest)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",UserRepo.Instance.data.token);
                var upDateUser = client.PostAsJsonAsync("UpdateInfo",updateInfoRequest);
                upDateUser.Wait();
                return upDateUser.Result;
            }
        }
        public HttpResponseMessage InActiveUser(int id)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",UserRepo.Instance.data.token);
                var inActiveUser = client.GetAsync("InActiveUser/"+ id);
                inActiveUser.Wait();
                return inActiveUser.Result;
            }
        }
        public HttpResponseMessage RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                var refreshToken = client.PostAsJsonAsync("RefreshToken",refreshTokenRequest);
                refreshToken.Wait();
                return refreshToken.Result;
            }
        }
        public HttpResponseMessage Report(ReportRequest request)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserRepo.Instance.data.token);
                var rePort = client.PostAsJsonAsync("Report", request);
                rePort.Wait();
                return rePort.Result;
            }
        }
        public void ShowInformation()
        {
            Console.WriteLine("ID= {0}\nName= {1}\nEmail= {2}\nGender= {3}\nisAdmin= {4}\nisActive= {5}\nHobbies= {6}\nToken= {7}",
            data.id,data.name,data.email,data.gender,data.isAdmin,data.isActive,data.hobbies,data.token);
        }
    }
}