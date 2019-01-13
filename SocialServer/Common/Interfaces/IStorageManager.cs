using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common.Interfaces
{
    public interface IStorageManager
    {
        /// <summary>
        /// Addes picture to the storage.
        /// </summary>
        /// <param name="picFile"></param>
        /// <returns>The picture url.</returns>
        Task<string> AddPicToStorage(HttpPostedFile picFile, string path);
    }
}
