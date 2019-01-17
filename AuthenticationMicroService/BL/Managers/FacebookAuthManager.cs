using Common.Dtos;
using Common.Exceptions;
using Common.Interfaces;
using Common.Models;
using Newtonsoft.Json.Linq;
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
        private readonly string _socialUrl;

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
            _socialUrl = ConfigurationManager.AppSettings["SocialUrl"];
        }


        public async Task<string> SignIn(string facebookToken)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
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
                            appToken = await _loginTokenManager.Add(userId, LoginTokenModel.LoginTypes.Facebook);
                            await AddUserToDatabases(facebookUserDto, userId, appToken);
                        }
                        else
                        {
                            var facebookAuth = _facebookAuthRepository.GetAuthByFacebookId(facebookId);
                            appToken = await _loginTokenManager.Add(facebookAuth.UserId, LoginTokenModel.LoginTypes.Facebook);
                        }

                        return appToken;
                    }
                    else
                    {
                        throw new ArgumentException("Access token is not valid");
                    }
                }
                catch (Exception e)
                {

                    throw e;
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
        private async Task AddUserToDatabases(FacebookUserDto facebookUserDto, string userId, string appToken)
        {
            try
            {
                Task addUserTask = AddUserToUsersDb(appToken, facebookUserDto, userId);
                Task addAuthTask = AddUserToFacebookAuthDb(facebookUserDto.id, userId);
                Task addUserToGraphTask = AddUserToGraphDb(appToken, facebookUserDto);
                Task.WaitAll(addUserTask, addAuthTask, addUserToGraphTask);
            }
            catch (AggregateException ae)
            {
                bool isAddUserFail = false, isAddAuthFail = false, isAddToGraphFail = false;
                foreach (var exception in ae.InnerExceptions)
                {
                    if (exception is AddAuthToDbException)
                    {
                        isAddAuthFail = true;
                    }
                    else if (exception is AddUserToDbException)
                    {
                        isAddUserFail = true;
                    }
                    else if (exception is AddUserToGraphException)
                    {
                        isAddToGraphFail = true;
                    }
                }
                await RollbackSuccededTasks(isAddAuthFail, isAddUserFail, isAddToGraphFail, appToken, facebookUserDto.id);
                throw new Exception("Internal server error");
            }

        }

        /// <summary>
        /// Addes user to graph database.
        /// </summary>
        /// <param name="appToken"></param>
        /// <param name="facebookUserDto"></param>
        /// <returns></returns>
        private async Task AddUserToGraphDb(string appToken, FacebookUserDto facebookUserDto)
        {
            try
            {
                using(HttpClient httpClient = new HttpClient())
                {
                    var dataToSend = new JObject();
                    dataToSend.Add("token", JToken.FromObject(appToken));
                    dataToSend.Add("email", JToken.FromObject(facebookUserDto.email));
                    var response = await httpClient.PostAsJsonAsync(_socialUrl+"Users/AddUser",dataToSend).ConfigureAwait(continueOnCapturedContext: false);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Couldn't connect to social server");
                    }
                }
            }
            catch (HttpRequestException e)
            {
                throw new AddUserToGraphException(e.Message);
            }
            catch (Exception e)
            {
                throw new AddUserToGraphException(e.Message);
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
                    UserModel user = GetUserFromFacebookData(facebookUser, userId);
                    var data = new JObject();
                    data.Add("user", JToken.FromObject(user));
                    data.Add("token", JToken.FromObject(appToken));
                    var response = await httpClient.PostAsJsonAsync(_identityUrl, data).ConfigureAwait(continueOnCapturedContext: false);
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

        /// <summary>
        /// Gets UserModel instance from the recieved facebook data.
        /// </summary>
        /// <param name="facebookUser"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private static UserModel GetUserFromFacebookData(FacebookUserDto facebookUser, string userId)
        {
            return new UserModel()
            {
                Id = userId,
                FirstName = facebookUser.first_name,
                LastName = facebookUser.last_name,
                Email = facebookUser.email
            };
        }

        /// <summary>
        /// If only one of the specified tasks failed a rollback is preformed on the other.
        /// </summary>
        /// <param name="isAddAuthFail"></param>
        /// <param name="isAddUserFail"></param>
        private async Task RollbackSuccededTasks(bool isAddAuthFail, bool isAddUserFail, bool isAddToGraphFail, string token, string facebookId)
        {
            if (isAddAuthFail && !isAddUserFail && !isAddToGraphFail)
            {
                await RemoveIdentity(token);
                await RemoveGraphNode(token);
            }
            else if (!isAddAuthFail && isAddUserFail && !isAddToGraphFail)
            {
                await RemoveFBAuth(facebookId);
                await RemoveGraphNode(token);

            }
            else if (!isAddAuthFail && !isAddUserFail && isAddToGraphFail)
            {
                await RemoveFBAuth(facebookId);
                await RemoveIdentity(token);

            }

            else
            {
                await RollbackOnTwoAdditionsFail(isAddAuthFail, isAddUserFail, isAddToGraphFail, token,facebookId);
            }
        }


        /// <summary>
        /// Rollbacks when two database additions failed.
        /// </summary>
        /// <param name="isAddAuthFail"></param>
        /// <param name="isAddUserFail"></param>
        /// <param name="isAddToGraghFail"></param>
        /// <param name="token"></param>
        /// <param name="facebookId"></param>
        /// <returns></returns>
        private async Task RollbackOnTwoAdditionsFail(bool isAddAuthFail, bool isAddUserFail, bool isAddToGraghFail, string token, string facebookId)
        {
            if(!isAddAuthFail && isAddUserFail && isAddToGraghFail)
            {
                await RemoveFBAuth(facebookId);
            }
            else if(isAddAuthFail && !isAddUserFail && isAddToGraghFail)
            {
                await RemoveIdentity(token);
            }
            else if (isAddAuthFail && isAddUserFail && !isAddToGraghFail)
            {
                await RemoveGraphNode(token);
            }
        }


        /// <summary>
        /// Removes the user associated with the specified id from the graph DB.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task RemoveGraphNode(string token)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var response = await httpClient.DeleteAsync(_socialUrl + $"users/DeleteUserByToken/{token}");
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Error with removing from the graph");
                    }
                }
            }
            catch (Exception e)
            {

                throw e;
            }
           
        }

        /// <summary>
        /// Removes the user associated with the specified token from the database.
        /// </summary>
        /// <param name="token"></param>
        private async Task RemoveIdentity(string token)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.DeleteAsync(_identityUrl + $"/DeleteUser/{token}");
                if (!response.IsSuccessStatusCode)
                {
                    throw new ArgumentException("Identity server could not remove the user");
                }
            }
        }



        /// <summary>
        /// Removes the auth associated with the specified email from the database.
        /// </summary>
        /// <param name="facebookId"></param>
        private async Task RemoveFBAuth(string facebookId)
        {
            await _facebookAuthRepository.Delete(facebookId);
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

        

        private string GenerateUserId()
        {
            return Guid.NewGuid().ToString();
        }

    }
}
