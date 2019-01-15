using Common.Dtos;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IPostsRepository
    {
        /// <summary>
        /// Adds a post to the database. 
        /// </summary>
        /// <param name="posterId"></param>
        /// <param name="post"></param>
        /// <param name="tagIds"></param>
        Task Add(string posterId, PostModel post, IEnumerable<TagDto> tags);


        /// <summary>
        /// Gets the emailes containing the text specified.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        Task<IEnumerable<UserModel>> GetUsersOfEmailWith(string taggerId, string text);
    }
}
