using LuisTrainer.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuisTrainer.Services
{
    public class MongoService : IDisposable
    {
        //private MongoServer mongoServer = null;
        private bool disposed = false;

        // To do: update the connection string with the DNS name
        // or IP address of your server. 
        //For example, "mongodb://testlinux.cloudapp.net"
        private string connectionString = "mongodb://policybot:mApr6FkqBwXvijKNJpMo7oh7wGiDQFFTTleqJ7qlGUS1N5RERmVxyo9HmuNpk1E7bcuDSPLhJuSNzcd2cMJjOg==@policybot.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";

        // This sample uses a database named "Tasks" and a 
        //collection named "TasksList".  The database and collection 
        //will be automatically created if they don't already exist.
        private string dbName = "TrainerAudit";
        private string collectionName = "TrainedEntry";
        private string untrainedCollectionName = "UnTrainedEntry";

        // Default constructor.        
        public MongoService()
        {
        }

        // Gets all Task items from the MongoDB server.        
        public List<Entry> GetAllTasks()
        {
            try
            {
                var collection = GetTasksCollection();
                return collection.Find(new BsonDocument()).ToList();
            }
            catch (MongoConnectionException)
            {
                return new List<Entry>();
            }
        }

        // Creates a Task and inserts it into the collection in MongoDB.
        public async Task CreateEntry(Entry task, bool isTrained = true)
        {
            var cName = isTrained ? collectionName : untrainedCollectionName;
            var collection = GetEntriesCollectionForEdit(cName);
            try
            {
                await collection.InsertOneAsync(task);
            }
            catch (MongoCommandException ex)
            {
                string msg = ex.Message;
            }
        }

        private IMongoCollection<Entry> GetTasksCollection()
        {
            MongoClient client = new MongoClient(connectionString);
            var database = client.GetDatabase(dbName);
            var todoTaskCollection = database.GetCollection<Entry>(collectionName);
            return todoTaskCollection;
        }

        private IMongoCollection<Entry> GetEntriesCollectionForEdit(string cName)
        {
            MongoClient client = new MongoClient(connectionString);
            var database = client.GetDatabase(dbName);
            var todoTaskCollection = database.GetCollection<Entry>(cName);
            return todoTaskCollection;
        }

        # region IDisposable

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                   
                }
            }

            this.disposed = true;
        }

        # endregion
    }
}
