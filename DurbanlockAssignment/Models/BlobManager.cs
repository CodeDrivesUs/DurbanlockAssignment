using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DurbanlockAssignment.Models
{
    public class BlobManager
    {
        private CloudBlobContainer blobContainer;

        public BlobManager(string ContainerName)
        {
            // Check if Container Name is null or empty
            if (string.IsNullOrEmpty(ContainerName))
            {
                throw new ArgumentNullException("ContainerName", "Container Name can't be empty");
            }
            try
            {
                // Get azure table storage connection string.
                string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=studentassignment;AccountKey=LTzwSYp7lc7J+K3g68s7YBUhzyV3QDT0bKNclIjjn4I1UbK5xf0XjNUdiICGqHiaCFDxS49gltlCdqnlo5dLuQ==;EndpointSuffix=core.windows.net";
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);

                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                blobContainer = cloudBlobClient.GetContainerReference(ContainerName);

                // Create the container and set the permission
                if (blobContainer.CreateIfNotExists())
                {
                    blobContainer.SetPermissions(
                        new BlobContainerPermissions
                        {
                            PublicAccess = BlobContainerPublicAccessType.Blob
                        }
                    );
                }
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }

        public string UploadFile(HttpPostedFileBase FileToUpload)
        {
            string AbsoluteUri;
            // Check HttpPostedFileBase is null or not
            if (FileToUpload == null || FileToUpload.ContentLength == 0)
                return null;
            try
            {
                string FileName = Path.GetFileName(FileToUpload.FileName);
                CloudBlockBlob blockBlob;
                // Create a block blob
                blockBlob = blobContainer.GetBlockBlobReference(FileName);
                // Set the object's content type
                blockBlob.Properties.ContentType = FileToUpload.ContentType;
                // upload to blob
                blockBlob.UploadFromStream(FileToUpload.InputStream);

                // get file uri
                AbsoluteUri = blockBlob.Uri.AbsoluteUri;
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
            return AbsoluteUri;
        }
        public bool DeleteBlob(string AbsoluteUri)
        {
            try
            {
                Uri uriObj = new Uri(AbsoluteUri);
                string BlobName = Path.GetFileName(uriObj.LocalPath);

                // get block blob refarence
                CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(BlobName);

                // delete blob from container    
                blockBlob.Delete();
                return true;
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }

        public List<string> BlobList()
        {
            List<string> _blobList = new List<string>();
            foreach (IListBlobItem item in blobContainer.ListBlobs())
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob _blobpage = (CloudBlockBlob)item;
                    _blobList.Add(_blobpage.Uri.AbsoluteUri.ToString());
                }
            }
            return _blobList;
        }
    }
}