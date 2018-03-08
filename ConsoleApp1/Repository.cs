using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace ConsoleApp1
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        #region Insert

        public virtual TEntity Insert(TEntity entityToAdd)
        {
            var added = DbSet.Attach(entityToAdd);
            Context.Entry(entityToAdd).State = EntityState.Added;
            return added;
        }

        #endregion

        #region Update

        public virtual void Update(TEntity entityToUpdate)
        {

            DbSet.Attach(entityToUpdate);
            Context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        #endregion


        private static string IncludeProperties(Expression<Func<TEntity, dynamic>> property)
        {
            if (property == null) return null;
            ReadOnlyCollection<Expression> members;

            var t = property.Parameters[0].Name;
            var prefix = t + ".";

            var memberExp = property.Body as NewExpression;
            if (memberExp == null)
            {
                return GetProperty(property.Body.ToString(), prefix);
            }
            members = memberExp.Arguments;
            var result = new StringBuilder();
            var notfirst = false;
            foreach (var memberInfo in members)
            {
                if (notfirst)
                {
                    result.Append(", ");
                }
                result.Append(GetProperty(memberInfo.ToString(), prefix));
                notfirst = true;
            }
            return result.ToString();
        }

        private static string GetProperty(string info, string prefix)
        {
            if (info.StartsWith(prefix))
                info = info.Substring(prefix.Length);

            return info.Replace("First().", "")
                .Replace("FirstOrDefault().", "")
                .Replace("Single().", "")
                .Replace("SingleOrDefault().", "");
        }

        #region Declarations

        internal IDbContext Context;
        public DbSet<TEntity> DbSet;

        #endregion

        #region Ctor

        public Repository(IDbContext context)
        {
            Context = context;

            Context.Configuration.LazyLoadingEnabled = false;

            // TEST SETTING TO TRUE
            Context.Configuration.AutoDetectChangesEnabled = true;
            Context.Configuration.ValidateOnSaveEnabled = false;
            Context.Configuration.ProxyCreationEnabled = false;

            DbSet = context.Set<TEntity>();
        }

        public Repository()
        {
        }

        #endregion

        #region Select

        public TEntity Load(params object[] id)
        {
            return DbSet.Find(id);
        }

        public TEntity Load(object id, string includeProperties)
        {
            return Load(new[] { id }, includeProperties);
        }

        public TEntity Load(object[] id, string includeProperties)
        {
            var tEntity = DbSet.Find(id);
            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                var strings = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var entry = Context.Entry(tEntity);
                foreach (var includeProperty in strings)
                {
                    var x = entry.Member(includeProperty);
                    if (x.GetType() == typeof(DbCollectionEntry))
                        entry.Collection(includeProperty).Load();
                    else if (x.GetType() == typeof(DbReferenceEntry))
                        entry.Reference(includeProperty).Load();
                }
            }
            return tEntity;
        }

        public TEntity Load(object id, Expression<Func<TEntity, dynamic>> includeProperties)
        {
            return Load(new[] { id }, IncludeProperties(includeProperties));
        }

        public TEntity Load(object[] id, Expression<Func<TEntity, dynamic>> includeProperties)
        {
            return Load(id, IncludeProperties(includeProperties));
        }

        public IQueryable<TEntity> Select2(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, bool, IOrderedQueryable<TEntity>> orderBy,
            SortDirection sortDir, Expression<Func<TEntity, dynamic>> includeProperties, int take = 0)
        {
            var inclProps = IncludeProperties(includeProperties);
            return Select(filter, orderBy, sortDir, inclProps, take);
        }

        public IQueryable<TEntity> Select2(List<Expression<Func<TEntity, bool>>> filter,
            Func<IQueryable<TEntity>, bool, IOrderedQueryable<TEntity>> orderBy,
            SortDirection sortDir, Expression<Func<TEntity, dynamic>> includeProperties, int take = 0)
        {
            var inclProps = IncludeProperties(includeProperties);
            return Select(filter, orderBy, sortDir, inclProps, take);
        }

        public IQueryable<TEntity> Select2(out int totalRecords, int pageSize, int pageIndex,
            List<Expression<Func<TEntity, bool>>> filters,
            Func<IQueryable<TEntity>, bool, IOrderedQueryable<TEntity>> orderBy,
            SortDirection sortDir, Expression<Func<TEntity, dynamic>> includeProperties)
        {
            var inclProps = IncludeProperties(includeProperties);
            return Select(out totalRecords, pageSize, pageIndex, filters, orderBy, sortDir, inclProps);
        }

        public IQueryable<TEntity> Select(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, bool, IOrderedQueryable<TEntity>> orderBy = null,
            SortDirection sortDir = SortDirection.Ascending, string includeProperties = "", int take = 0)
        {
            var filters = filter != null ? new List<Expression<Func<TEntity, bool>>> { filter } : null;

            return Select(filters, orderBy, sortDir, includeProperties, take);
        }

        public IQueryable<TEntity> Select(List<Expression<Func<TEntity, bool>>> filters,
            Func<IQueryable<TEntity>, bool, IOrderedQueryable<TEntity>> orderBy,
            SortDirection sortDir, string includeProperties = "", int take = 0)
        {
            IQueryable<TEntity> query = DbSet;

            if (filters != null)
            {
                query = filters.Aggregate(query, (current, filter) => current.Where(filter));
            }
            if (!string.IsNullOrWhiteSpace(includeProperties))
                query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            if (orderBy != null)
                query = orderBy(query, sortDir == SortDirection.Ascending);
            if (take > 0)
                query = query.Take(take);
            return query;
        }

        public IQueryable<TEntity> Select(out int totalRecords, int pageSize, int pageIndex,
            List<Expression<Func<TEntity, bool>>> filters,
            Func<IQueryable<TEntity>, bool, IOrderedQueryable<TEntity>> orderBy,
            SortDirection sortDir, string includeProperties)
        {
            IQueryable<TEntity> query = DbSet;

            pageIndex = pageIndex - 1;

            if (filters != null)
            {
                query = filters.Aggregate(query, (current, filter) => current.Where(filter));
            }

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                var properties = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                query = properties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }

            if (orderBy != null)
            {
                query = orderBy(query, sortDir == SortDirection.Ascending);
            }

            if (pageSize > 0) //else All
                query = pageIndex == 0 ? query.Take(pageSize) : query.Skip(pageIndex * pageSize).Take(pageSize);

            totalRecords = Count(filters);

            return query;
        }

        #endregion

        #region Count

        public virtual int Count(List<Expression<Func<TEntity, bool>>> filters = null)
        {
            IQueryable<TEntity> query = DbSet;

            if (filters != null)
            {
                query = filters.Aggregate(query, (current, filter) => current.Where(filter));
            }

            return query.ToList().Count();
        }

        public virtual int Count(Expression<Func<TEntity, bool>> filter)
        {
            return Count(new List<Expression<Func<TEntity, bool>>> { filter });
        }

        #endregion

        #region Delete

        public virtual void Delete(object id)
        {
            var entityToDelete = DbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                DbSet.Attach(entityToDelete);
            }
            DbSet.Remove(entityToDelete);
        }
        public virtual void DeleteRange(IEnumerable<TEntity> entities)
        {
            var entitiesToDelete = entities.ToList();
            for (var ctr = 0; ctr < entitiesToDelete.Count; ctr++)// Cannot use foreach loop for collection modified exception
            {
                var entityToDelete = entitiesToDelete[ctr];
                if (Context.Entry(entityToDelete).State == EntityState.Detached)
                {
                    DbSet.Attach(entityToDelete);
                }
                DbSet.Remove(entityToDelete);
            }
        }
        #endregion

        #region CacheControl

        public virtual IQueryable<TEntity> CacheControl(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query;
        }

        public virtual string ConnectionString()
        {
            return Context.Database.Connection.ConnectionString;
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            foreach (var entityToUpdate in entities)
            {

                DbSet.Attach(entityToUpdate);
                Context.Entry(entityToUpdate).State = EntityState.Modified;
            }
        }

        #endregion

        #region bulk insert

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            foreach (var e in entities)
            {

                DbSet.Add(e);
                Context.Entry(e).State = EntityState.Added;
            }

            //Context.Set<TEntity>().AddRange(entities);            
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().RemoveRange(entities);
        }

        #endregion

        #region stored proc execution

        public virtual IEnumerable<TEntity> ExecWithStoreProcedure(string query, params object[] parameters)
        {
            return Context.Database.SqlQuery<TEntity>(query, parameters);
        }

        public IEnumerable<TEntity> ExecWithStoreProcedure(string query)
        {
            return Context.Database.SqlQuery<TEntity>(query);
        }


        public void ExecuteWithStoreProcedure(string query, params object[] parameters)
        {
            Context.Database.ExecuteSqlCommand(query, parameters);
        }

        #endregion
    }

}
