using Image.Grpc.Service.Repository;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Image.Grpc.Service
{
    public static class Extensions
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services, bool isDevelopmentEnv)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

            services.AddSingleton(serviceProvider =>
            {
                string connectionString;
                string databaseName;
                if (isDevelopmentEnv)
                {
                    var configuration = serviceProvider.GetService<IConfiguration>();
                    connectionString = configuration.GetConnectionString("DevMongoConnection");
                    databaseName = configuration.GetSection("Database").Value;
                }
                else
                {
                    connectionString = Environment.GetEnvironmentVariable("MongoDbConnection");
                    databaseName = Environment.GetEnvironmentVariable("Database");
                }
                var mongoClient = new MongoClient(connectionString);
                return mongoClient.GetDatabase(databaseName);
            });
            return services;
        }

        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName)
            where T : class
        {
            services.AddSingleton<IRepository<T>>(serviceProvider =>
            {
                var database = serviceProvider.GetService<IMongoDatabase>();
                return new MongoRepository<T>(database, collectionName);
            });
            return services;
        }
    }
}
