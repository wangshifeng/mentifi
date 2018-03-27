using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hub3c.Mentify.MongoRepo.Config;
using Hub3c.Mentify.MongoRepo.Model;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Hub3c.Mentify.MongoRepo.Repository
{
    public class MongoRepository<T> : IMongoRepository<T> where T : Resource 
    {
        public MongoRepository(IConfiguration config, IMongoClient mongoClient)
        {
            var mongoDatabase = mongoClient.GetDatabase(config.GetSection("MongoDbConfig")["DbName"]);
            Collection = mongoDatabase.GetCollection<T>(typeof(T).Name);
        }
        
        public IMongoCollection<T> Collection { get; }

        public Type ElementType => Collection.AsQueryable<T>().ElementType;

        public Expression Expression => Collection.AsQueryable<T>().Expression;

        public IQueryProvider Provider => Collection.AsQueryable<T>().Provider;

        public Task Add(IEnumerable<T> entities)
        {
            return Collection.InsertManyAsync(entities);
        }

        public Task Add(T entity)
        {
            return Collection.InsertOneAsync(entity);
        }

        public async Task<long> Count()
        {
            return await Collection.CountAsync(Builders<T>.Filter.Empty, null);
        }

        public async Task Delete(Expression<Func<T, bool>> predicate)
        {
            foreach (var entity in Collection.AsQueryable<T>().Where(predicate))
            {
                await Delete(entity);
            }

        }

        public async Task Delete(T entity)
        {

            await Collection.DeleteOneAsync(Builders<T>.Filter.Where(a => a.Id == entity.Id));            
        }

        public async Task Delete(string id)
        {
            await Collection.DeleteOneAsync(Builders<T>.Filter.AnyEq("Id", id));
        }

        public async Task DeleteAll()
        {
            await Collection.DeleteManyAsync(Builders<T>.Filter.Empty);
        }

        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            return Collection.AsQueryable<T>().Any(predicate);
        }

        public async Task<T> GetById(string id)
        {
            return await Collection.Find(e => e.Id.ToString() == id).FirstOrDefaultAsync();
        }


        public void Update(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                this.Update(entity);
            }
        }

        public T Update(T entity)
        {
            Collection.ReplaceOne(a => a.Id == entity.Id, entity);
            return entity;
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> predicate)
        {
            return await this.Collection.Find(predicate).ToListAsync();
        }

        Task IMongoRepository<T>.Update(T entity)
        {
            throw new NotImplementedException();
        }

        Task IMongoRepository<T>.Update(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Collection.AsQueryable<T>().GetEnumerator();
        }
    }
}