using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface ICommonOperationsManager
    {
        /// <summary>
        /// Verifies the specified token and return the user id.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> VerifyToken(string token);

        /// <summary>
        /// Gets the value of the cookie name specified.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cookieName"></param>
        /// <returns></returns>
        string GetCookieValue(HttpRequestMessage request, string cookieName);

        /// <summary>
        /// Gets a new guid.
        /// </summary>
        /// <returns></returns>
        string GetNewGuid();


        /// <summary>
        /// Verifies the string is not null, whitespace, or undefined; 
        /// </summary>
        /// <param name="stringToVerify"></param>
        /// <returns></returns>
        void VerifyString(string stringToVerify);


        /// <summary>
        /// Verifies the string specified is an integer bigger than zero.
        /// </summary>
        /// <param name="strignToVerify"></param>
        int IntegerBiggerThanZero(string stringToVerify);


        /// <summary>
        /// Sends a notification to a specific user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendNotification(string userId, string message, string appToken);
    }
}
