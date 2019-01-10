using Common.Dtos;
using Common.Exceptions;
using Common.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BL.Managers
{
    public class FacebookAuthManager : IFacebookAuthManager
    {
        private readonly IFacebookAuthRepository _facebookAuthRepository;
        private readonly ILoginTokenManager _loginTokenManager;
        private readonly string _identityUrl;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="facebookAuthRepository"></param>
        /// <param name="loginTokenManager"></param>
        public FacebookAuthManager(IFacebookAuthRepository facebookAuthRepository, ILoginTokenManager loginTokenManager)
        {
            _facebookAuthRepository = facebookAuthRepository;
            _loginTokenManager = loginTokenManager;
            _identityUrl = ConfigurationManager.AppSettings["IdentityUrl"];
        }


        public async Task<string> SignIn(string facebookToken)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = GetUserByFacebookToken(facebookToken, httpClient);
                if (response.IsSuccessStatusCode)
                {
                    var facebookUserDto = await response.Content.ReadAsAsync<FacebookUserDto>();
                    var facebookId = facebookUserDto.id;
                    string appToken;
                    if (_facebookAuthRepository.IsFacebookIdFree(facebookId))
                    {
                        var userId = GenerateUserId();
                        appToken = _loginTokenManager.Add(userId, LoginTokenModel.LoginTypes.Facebook);
                        await AddUserToDatabase(facebookUserDto, userId, appToken);
                    }
                    else
                    {
                        var facebookAuth = _facebookAuthRepository.GetAuthByFacebookId(facebookId);
                        appToken = _loginTokenManager.Add(facebookAuth.UserId, LoginTokenModel.LoginTypes.Facebook);
                    }

                    return appToken;
                }
                else
                {
                    throw new ArgumentException("Access token is not valid");
                }
            }
        }
               
        /// <summary>
        /// Gets the user details associated with the specified facebook token.
        /// </summary>
        /// <param name="facebookToken"></param>
        /// <param name="httpClient"></param>
        /// <returns>An HttpResponseMessage with the user details or the reason why it failed.</returns>
        private HttpResponseMessage GetUserByFacebookToken(string facebookToken, HttpClient httpClient)
        {

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", facebookToken);
            httpClient.BaseAddress = new Uri("https://graph.facebook.com/v3.2/");
            httpClient.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient.GetAsync($"me?fields=id,name,email,first_name,last_name").Result;
        }

        /// <summary>
        /// Addes mail to the auth table in the database.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        private Task AddUserToFacebookAuthDb(string facebookId)
        {
            try
            {
                string userId = GenerateUserId();
                return Task.Run(() => _facebookAuthRepository.Add(new FacebookAuthModel(facebookId, userId)));
            }
            catch (Exception e)
            {
                //TODO: log
                throw new AddAuthToDbException(e.Message);
            }

        }


        /// <summary>
        /// Addes the user to the Users table and the email to the Auth table.
        /// </summary>
        /// <param name="facebookUserDto"></param>
        /// <param name="userEmail"></param>
        /// <param name="appToken"></param>
        private async Task AddUserToDatabase(FacebookUserDto facebookUserDto, string userId, string appToken)
        {
            try
            {
                Task addUserTask = AddUserToUsersDb(appToken, facebookUserDto, userId);
                Task addAuthTask = AddUserToFacebookAuthDb(facebookUserDto.id, userId);
                Task.WaitAll(addUserTask, addAuthTask);
            }
            catch (AggregateException ae)
            {
                bool isAddUserFail = false, isAddAuthFail = false;
                foreach (var exception in ae.InnerExceptions)
                {
                    if (exception is AddAuthToDbException)
                    {
                        isAddAuthFail = true;
                    }
                    if (exception is AddUserToDbException)
                    {
                        isAddUserFail = true;
                    }
                }
                await RollbackSuccededTask(isAddAuthFail, isAddUserFail, userId, facebookUserDto.id);
                throw new Exception("Internal server error");
            }

        }


        /// <summary>
        /// If only one of the specified tasks failed a rollback is preformed on the other.
        /// </summary>
        /// <param name="isAddAuthFail"></param>
        /// <param name="isAddUserFail"></param>
        private async Task RollbackSuccededTask(bool isAddAuthFail, bool isAddUserFail, string userId, string facebookId)
        {
            if (isAddAuthFail && !isAddUserFail)
            {
                await RemoveUserFromDb(userId);
            }
            else if (!isAddAuthFail && isAddUserFail)
            {
                RemoveFacebookAuthFromDb(facebookId);
            }
        }

        /// <summary>
        /// Removes the user associated with the specified email from the database.
        /// </summary>
        /// <param name="userEmail"></param>
        private async Task RemoveUserFromDb(string userId)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.DeleteAsync(_identityUrl + $"/{userId}");
                if (!response.IsSuccessStatusCode)
                {
                    throw new ArgumentException("Identity server could not remove the user");
                }
            }
        }



        /// <summary>
        /// Removes the auth associated with the specified email from the database.
        /// </summary>
        /// <param name="userEmail"></param>
        private void RemoveFacebookAuthFromDb(string facebookId)
        {
            _facebookAuthRepository.Delete(facebookId);
        }

        /// <summary>
        /// Addes mail to the auth table in the database.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        private Task AddUserToFacebookAuthDb(string facebookId, string userId)
        {
            try
            {
                return Task.Run(() => _facebookAuthRepository.Add(new FacebookAuthModel(facebookId, userId)));
            }
            catch (Exception e)
            {
                //TODO: log
                throw new AddAuthToDbException(e.Message);
            }
        }

        /// <summary>
        /// Adds a user entity to the users database through the identity service.
        /// </summary>
        /// <param name="appToken"></param>
        /// <param name="user"></param>
        private async Task AddUserToUsersDb(string appToken, FacebookUserDto facebookUser, string userId)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var user = new UserModel()
                                {
                                    Id = userId,
                                    FirstName = facebookUser.first_name,
                                    LastName = facebookUser.last_name,
                                    Email = facebookUser.email
                                };
                    var response = await httpClient.PostAsJsonAsync(_identityUrl, user).ConfigureAwait(continueOnCapturedContext: false);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Identity server could not add the user");
                    }
                }
            }
            catch (Exception e)
            {
                //TODO: log
                throw new AddUserToDbException(e.Message);
            }

        }

        private string GenerateUserId()
        {
            return Guid.NewGuid().ToString();
        }

    }
}
