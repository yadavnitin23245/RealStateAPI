using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using RealState.Data;
using System.Data.Entity.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;


namespace RealState.Repository.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        private DbSet<T> _entities;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _entities = context.Set<T>();


        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetAll()
        {
            return _entities.AsEnumerable();
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<T> GetAsync(long id)
        {
            return await _entities.FindAsync(id);
        }

        /// <summary>
        /// Inserts the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">entity is null</exception>
        public async Task<int?> InsertAsync(T entity)
        {
            try
            {

                if (entity == null)
                    throw new ArgumentNullException("entity is null");
                _entities.Add(entity);
                _context.Entry(entity).State = EntityState.Added;
                return await _context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                var message = "";
                if (exception is DbEntityValidationException dbException)
                {
                    message += "\nValidation Errors: ";
                    foreach (var error in dbException.EntityValidationErrors.SelectMany(entity => entity.ValidationErrors))
                    {
                        message += $"\n * Field name: {error.PropertyName}, Error message: {error.ErrorMessage}";
                    }
                }


                // use an optimistic concurrency strategy from:
                // https://docs.microsoft.com/en-us/ef/core/saving/concurrency#resolving-concurrency-conflicts
                return null;
            }
        }

        
        public int? Insert(T entity)
        {
            try
            {

                if (entity == null)
                    throw new ArgumentNullException("entity is null");
                _entities.Add(entity);
                _context.Entry(entity).State = EntityState.Added;
                return _context.SaveChanges();
            }
            catch (Exception exception)
            {
                var message = "";
                if (exception is DbEntityValidationException dbException)
                {
                    message += "\nValidation Errors: ";
                    foreach (var error in dbException.EntityValidationErrors.SelectMany(entity => entity.ValidationErrors))
                    {
                        message += $"\n * Field name: {error.PropertyName}, Error message: {error.ErrorMessage}";
                    }
                }


                // use an optimistic concurrency strategy from:
                // https://docs.microsoft.com/en-us/ef/core/saving/concurrency#resolving-concurrency-conflicts
                return null;
            }
        }

        public async Task<int?> InsertMultiAsync(List<T> entity)
        {

            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity is null");
                entity.ForEach(a => { _context.Entry(a).State = EntityState.Added; });
                return await _context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                var message = "";
                if (exception is DbEntityValidationException dbException)
                {
                    message += "\nValidation Errors: ";
                    foreach (var error in dbException.EntityValidationErrors.SelectMany(entity => entity.ValidationErrors))
                    {
                        message += $"\n * Field name: {error.PropertyName}, Error message: {error.ErrorMessage}";
                    }
                }


                // use an optimistic concurrency strategy from:
                // https://docs.microsoft.com/en-us/ef/core/saving/concurrency#resolving-concurrency-conflicts
                return null;
            }
        }


        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">entity is null</exception>
        public async Task<int?> UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity is null");
            _context.Entry(entity).State = EntityState.Modified;
            return await SaveChangesAsync();
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">entity is null</exception>
        public async Task<int?> DeleteAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity is null");
            _context.Entry(entity).State = EntityState.Deleted;
            _entities.Remove(entity);
            return await SaveChangesAsync();
        }

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <exception cref="ArgumentNullException">entity</exception>
        public void Remove(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            _context.Entry(entity).State = EntityState.Deleted;
            _entities.Remove(entity);
        }

        /// <summary>
        /// Saves the changes asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<int?> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public int? SaveChanges()
        {
            return _context.SaveChanges();
        }
        /// <summary>
        /// Wheres the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate);
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">entity is null</exception>
        public async Task<int?> UpdateAsync(List<T> entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity is null");

            entity.ForEach(a => { _context.Entry(a).State = EntityState.Modified; });
            return await SaveChangesAsync();
        }

        /// <summary>
        /// Executes the store procedure.
        /// </summary>
        /// <param name="procName">Name of the proc.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public async Task<DbDataReader> ExecStoreProcedure(string procName, string entity, params SqlParameter[] parameters)
        {
            var query = _queryBuilder(procName, parameters);
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandTimeout = 3600;
                var reader = await command.ExecuteReaderAsync();
                return reader;
            }
        }

        /// <summary>
        /// Executes the with json result asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parserString">The parser string.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public async Task<T> ExecuteWithJsonResultAsync(string name, string parserString, params SqlParameter[] parameters)
        {
            var query = _queryBuilder(name, parameters);
            T result;
            var connectionObject = _context.Database.GetDbConnection();
            connectionObject.Open();
            using (var command = connectionObject.CreateCommand())
            {
                command.CommandText = query;
                command.CommandTimeout = 3600;
                var reader = await command.ExecuteReaderAsync();

                try
                {
                    var jsonResult = new StringBuilder();
                    while (reader.Read())
                    {
                        jsonResult.Append(reader.GetValue(0).ToString());
                    }
                    JObject jsonResponse = JObject.Parse(jsonResult.ToString());
                    result = JsonConvert.DeserializeObject<T>(Convert.ToString(jsonResponse[parserString]));
                }
                catch (Exception e)
                {
                    //TODO: Add logging
                    result = default(T);
                }
                finally
                {
                    connectionObject.Close();
                }
            }
            return result;
        }







        //without json result



        /// <summary>
        /// Executes the with json result.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parserString">The parser string.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public List<T> ExecuteWithJsonResult(string name, string parserString, params SqlParameter[] parameters)
        {
            var query = _queryBuilder(name, parameters);
            List<T> result;
            var connectionObject = _context.Database.GetDbConnection();
            // Always close the connection in finally block
            if (connectionObject.State == ConnectionState.Open)
            {
                connectionObject.Close();
            }
            connectionObject.Open();

            using (var command = connectionObject.CreateCommand())
            {
                command.CommandText = query;
                command.CommandTimeout = 3600;
                var reader = command.ExecuteReader();

                try
                {
                    var jsonResult = new StringBuilder();
                    while (reader.Read())
                    {
                        jsonResult.Append(reader.GetValue(0).ToString());
                    }
                    JObject jsonResponse = JObject.Parse(jsonResult.ToString());
                    var objResponse = jsonResponse[parserString];
                    if (objResponse != null)
                    {
                        return JsonConvert.DeserializeObject<List<T>>(Convert.ToString(objResponse));
                    }
                    return (List<T>)Activator.CreateInstance(typeof(List<T>));
                }
                catch (Exception ex)
                {
                    //TODO: Add logging
                    result = default(List<T>);
                    //connectionObject.Close();
                }
                finally
                {
                    connectionObject.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// Queries the builder.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        private string _queryBuilder(string name, params SqlParameter[] parameters)
        {
            var query = string.Empty;
            if (parameters != null && parameters.Any())
            {
                var paramsString = parameters.Any(a => a.ParameterName.Contains("@")) ?
                    string.Join(",", parameters.Select(p => $" {p.ParameterName}='{p.SqlValue}'"))
                    : string.Join(",", parameters.Select(p => $" @{p.ParameterName}='{p.SqlValue}'"));
                query = $"Exec {name} {paramsString}";
            }
            return query;
        }



        /// <summary>
        /// Finds the specified key values.
        /// </summary>
        /// <param name="keyValues">The key values.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public T Find(params object[] keyValues)
        {
            return _entities.Find(keyValues);
        }


        /// <summary>
        /// Selects the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includes">The includes.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        internal IQueryable<T> Select(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            List<Expression<Func<T, object>>> includes = null,
            int? page = null,
            int? pageSize = null)
        {
            IQueryable<T> query = _entities;

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            if (orderBy != null)
                query = orderBy(query);

            //if (filter != null)
            //    query = query.AsExpandable().Where(filter);

            if (page != null && pageSize != null)
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);

            return query;
        }

        /// <summary>
        /// Selects the asynchronous.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includes">The includes.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        internal async Task<IEnumerable<T>> SelectAsync(
            Expression<Func<T, bool>> query = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            List<Expression<Func<T, object>>> includes = null,
            int? page = null,
            int? pageSize = null)
        {
            return Select(query, orderBy, includes, page, pageSize).AsEnumerable();
        }

        /// <summary>
        /// Executes the type of the store procedure without return.
        /// </summary>
        /// <param name="procName">Name of the proc.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="parameters">The parameters.</param>
        public void ExecuteStoreProcedureWithoutReturnType(string procName, string entity, params SqlParameter[] parameters)
        {
            var connectionObject = _context.Database.GetDbConnection();
            connectionObject.Open();
            try
            {
                var query = _queryBuilder(procName, parameters);
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandTimeout = 3600;
                    command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                return;
            }
            finally
            {
                connectionObject.Close();
            }
        }

        public List<T> ExecuteWithoutJsonResult<T>(string name, params SqlParameter[] parameters) where T : new()
        {
            var query = _queryBuilder(name, parameters);
            var resultList = new List<T>();
            var connectionObject = _context.Database.GetDbConnection();
            connectionObject.Open();
            using (var command = connectionObject.CreateCommand())
            {
                command.CommandText = query;
                command.CommandTimeout = 3600;

                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var obj = new T();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var prop = typeof(T).GetProperty(reader.GetName(i));
                                if (prop != null && !DBNull.Value.Equals(reader.GetValue(i)))
                                {
                                    prop.SetValue(obj, Convert.ChangeType(reader.GetValue(i), prop.PropertyType));
                                }
                            }
                            resultList.Add(obj);
                        }
                    }
                }
                catch (Exception e)
                {
                    //TODO: Add logging
                    resultList = null;
                }
                finally
                {
                    connectionObject.Close();
                }
            }
            return resultList;
        }


        public IEnumerable<T> Find_clause(Func<T, bool> predicate)
        {
            return _entities.Where(predicate).ToList();
        }
    }
}
