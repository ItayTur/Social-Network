using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BL.Managers
{
    public class AWSStorageManager: IStorageManager
    {

        private readonly string _bucketName;
        private readonly string _imgBaseUrl;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AWSStorageManager()
        {
            _bucketName = ConfigurationManager.AppSettings["BucketName"];
            _imgBaseUrl = ConfigurationManager.AppSettings["ImgBaseUrl"];
        }

        /// <summary>
        /// Addes picture to the storage.
        /// </summary>
        /// <param name="picFile"></param>
        /// <returns>The picture url.</returns>
        public async Task<string> AddPicToStorage(HttpPostedFile picFile, string path)
        {
            picFile.SaveAs(path);
            RegionEndpoint bucketRegion = RegionEndpoint.EUWest1;
            IAmazonS3 s3Client = new AmazonS3Client(bucketRegion);
            try
            {
                string key = Guid.NewGuid().ToString();
                var fileTrnsferUtility = new TransferUtility(s3Client);
                await fileTrnsferUtility.UploadAsync(path, _bucketName, key).ConfigureAwait(false);
                return _imgBaseUrl+ key;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }
    }
}
