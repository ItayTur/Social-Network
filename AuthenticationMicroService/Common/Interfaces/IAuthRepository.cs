using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IAuthRepository
    {
        /// <summary>
        /// Verify user by username and password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>Access token</returns>
        string LoginByUsernamePassword(string email, string password);

        /// <summary>
        /// Verify user by Facebook token
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>Access token</returns>
        string LoginByFacebookToken(string facebookToken);

        void ChangePassword(string accessToken, string oldPassword, string newPassword);

        /// <summary>
        /// Adds new username/password credentials and creates access token
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>Access token</returns>
        string AddUsernamePasswordLoginAndLogin(string email, string password);

        /// <summary>
        /// Adds new facebook token credentials and creates access token
        /// </summary>
        /// <param name="facebookToken"></param>
        /// <returns></returns>
        string AddFacebookLoginAndLogin(string facebookToken);

        void DeactivateUser(string email);

        bool ValidateAccessToken(string accessToken);
    }
}
