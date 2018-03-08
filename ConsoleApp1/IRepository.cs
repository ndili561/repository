using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace ConsoleApp1
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity Load(params object[] id);
        TEntity Load(object id, string includeProperties);
        TEntity Load(object[] id, string includeProperties);
        TEntity Load(object id, Expression<Func<TEntity, dynamic>> includeProperties);
        TEntity Load(object[] id, Expression<Func<TEntity, dynamic>> includeProperties);

        IQueryable<TEntity> Select(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, bool, IOrderedQueryable<TEntity>> orderBy = null,
            SortDirection sortDir = SortDirection.Ascending,
            string includeProperties = "", int take = 0);

        IQueryable<TEntity> Select(List<Expression<Func<TEntity, bool>>> filters = null,
            Func<IQueryable<TEntity>, bool, IOrderedQueryable<TEntity>> orderBy = null,
            SortDirection sortDir = SortDirection.Ascending,
            string includeProperties = "", int take = 0);

        IQueryable<TEntity> Select(out int totalRecords,
            int pageSize = 10,
            int pageIndex = 0,
            List<Expression<Func<TEntity, bool>>> filters = null,
            Func<IQueryable<TEntity>, bool, IOrderedQueryable<TEntity>> orderBy = null,
            SortDirection sortDir = SortDirection.Ascending,
            string includeProperties = "");

        int Count(List<Expression<Func<TEntity, bool>>> filters = null);
        int Count(Expression<Func<TEntity, bool>> filter);
        TEntity Insert(TEntity entityToAdd);
        void Update(TEntity entityToUpdate);
        void Delete(object id);
        void Delete(TEntity entityToDelete);
        void DeleteRange(IEnumerable<TEntity> entities);
        IQueryable<TEntity> CacheControl(Expression<Func<TEntity, bool>> filter = null);
        string ConnectionString();
        void UpdateRange(IEnumerable<TEntity> entities);
        void AddRange(IEnumerable<TEntity> entities);
        void RemoveRange(IEnumerable<TEntity> entities);

        IEnumerable<TEntity> ExecWithStoreProcedure(string query, params object[] parameters);
        IEnumerable<TEntity> ExecWithStoreProcedure(string query);
        void ExecuteWithStoreProcedure(string query, params object[] parameters);

        IQueryable<TEntity> Select2(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, bool, IOrderedQueryable<TEntity>> orderBy = null,
            SortDirection sortDir = SortDirection.Ascending,
            Expression<Func<TEntity, dynamic>> includeProperties = null, int take = 0);

        IQueryable<TEntity> Select2(List<Expression<Func<TEntity, bool>>> filters = null,
            Func<IQueryable<TEntity>, bool, IOrderedQueryable<TEntity>> orderBy = null,
            SortDirection sortDir = SortDirection.Ascending,
            Expression<Func<TEntity, dynamic>> includeProperties = null, int take = 0);

        IQueryable<TEntity> Select2(out int totalRecords, int pageSize = 10, int pageIndex = 0,
            List<Expression<Func<TEntity, bool>>> filters = null,
            Func<IQueryable<TEntity>, bool, IOrderedQueryable<TEntity>> orderBy = null,
            SortDirection sortDir = SortDirection.Ascending,
            Expression<Func<TEntity, dynamic>> includeProperties = null);
    }
}
