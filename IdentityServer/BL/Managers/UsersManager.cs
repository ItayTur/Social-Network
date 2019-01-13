﻿using Common.Dtos;
using Common.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace BL.Managers
{
    public class UsersManager : IUsersManager
    {

        private readonly IUsersRepository _usersRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="usersRepository"></param>
        public UsersManager(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        /// <summary>
        /// Adds new user record to the db.
        /// </summary>
        /// <param name="user"></param>
        public async Task Add(UserModel user, string token)
        {
            try
            {
                await VerfiyToken(token);
                _usersRepository.Add(user);
            }
            catch(AuthenticationException)
            {
                throw new AuthenticationException();
            }
            catch (Exception)
            {

                throw new Exception();
            }
        }


        /// <summary>
        /// Verifies the token validity.
        /// </summary>
        /// <param name="token"></param>
        private async Task VerfiyToken(string token)
        {
            using(HttpClient httpClient = new HttpClient())
            {
                try
                {
                    var tokentDto = new TokenDto(token);
                    var response = await httpClient.PostAsJsonAsync("", tokentDto);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new AuthenticationException();
                    }
                }
                catch (Exception e)
                {

                    throw new Exception(e.Message);
                }
                
            }
        }

        /// <summary>
        /// Deletes the user associated with the specified id.
        /// </summary>
        /// <param name="id"></param>
        public async Task Delete(string id, string token)
        {
            try
            {
                await VerfiyToken(token);
                _usersRepository.Delete(id);

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}