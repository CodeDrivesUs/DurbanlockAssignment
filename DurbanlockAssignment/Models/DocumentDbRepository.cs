using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace DurbanlockAssignment.Models
{
    public static class DocumentDBRepository<T> where T : class
    {
        private static readonly string DatabaseId = "studentdb";
        private static readonly string CollectionId = "studentcol";
        private static DocumentClient client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["endpoint"]), ConfigurationManager.AppSettings["authKey"]);
        
        public static async Task<Item> GetItemAsync(string id)
        {
            try
            {
                Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), new RequestOptions { PartitionKey = new PartitionKey(Undefined.Value) });
                return (Item)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public static async Task<IEnumerable<Item>> GetItemsAsync(Expression<Func<Item, bool>> predicate)
        {
            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            IDocumentQuery<Item> query = client.CreateDocumentQuery<Item>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                option)
                .Where(predicate)
                .AsDocumentQuery();

            List<Item> results = new List<Item>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<Item>());
            }

            return results;
        }
        public static async Task<IEnumerable<Item>> GetItemesAsync()
        {
            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            IDocumentQuery<Item> query = client.CreateDocumentQuery<Item>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                option)
              
                .AsDocumentQuery();

            List<Item> results = new List<Item>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<Item>());
            }

            return results;
        }
        public static async Task<IEnumerable<Item>> SearchAsync(string search, string SearchBy)
        {
            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            List<Item> results = new List<Item>();
            if (SearchBy=="First")
            {
                IDocumentQuery<Item> query = client.CreateDocumentQuery<Item>(
               UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
               option)
               .Where(x => x.FirstName == search)
               .AsDocumentQuery();

                while (query.HasMoreResults)
                {
                    results.AddRange(await query.ExecuteNextAsync<Item>());
                }
            }
            if (SearchBy == "Last")
            {
                IDocumentQuery<Item> query = client.CreateDocumentQuery<Item>(
               UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
               option)
               .Where(x => x.LastName == search)
               .AsDocumentQuery();

                while (query.HasMoreResults)
                {
                    results.AddRange(await query.ExecuteNextAsync<Item>());
                }
            }
            if (SearchBy == "Email")
            {
                IDocumentQuery<Item> query = client.CreateDocumentQuery<Item>(
               UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
               option)
               .Where(x => x.EmailAddress == search)
               .AsDocumentQuery();

                while (query.HasMoreResults)
                {
                    results.AddRange(await query.ExecuteNextAsync<Item>());
                }
            }
            if (SearchBy == "StudentNo")
            {
                IDocumentQuery<Item> query = client.CreateDocumentQuery<Item>(
               UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
               option)
               .Where(x => x.StudentNum == search)
               .AsDocumentQuery();

                while (query.HasMoreResults)
                {
                    results.AddRange(await query.ExecuteNextAsync<Item>());
                }
            }
            return results;
        }

        public static async Task<Document> CreateItemAsync(Item item)
        {
            return await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item);
        }

        public static async Task<Document> UpdateItemAsync(string id, T item)
        {
            return await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), item);
        }

        public static async Task DeleteItemAsync(string id)
        {
            await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), new RequestOptions { PartitionKey = new PartitionKey(Undefined.Value) });
        }

        public static void Initialize()
        {
            client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["endpoint"]), ConfigurationManager.AppSettings["authKey"]);
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
        }

        private static async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = DatabaseId });
                }
                else
                {
                    throw;
                }
            }
        }
        //public static bool valideate(string stud)
        //{
        //    var option = new FeedOptions { EnableCrossPartitionQuery = true };
        //    List<Item> results = new List<Item>();
        //    IDocumentQuery<Item> query = client.CreateDocumentQuery<Item>(
        //     UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
        //     option)
        //     .Where(x => x.StudentNum == stud)
        //     .AsDocumentQuery();
        //    while (query.HasMoreResults)
        //    {
        //        results.AddRange( query.ExecuteNextAsync<Item>());
        //    }

        //    return true;
        //} 

        private static async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DatabaseId),
                        new DocumentCollection { Id = CollectionId },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}