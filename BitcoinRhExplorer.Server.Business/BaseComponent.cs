namespace BitcoinRhExplorer.Server.Business
{
    public  class BaseComponent
    {
      /*  public virtual TEntity Create()
        {
            var entity = (TEntity)typeof(TEntity).GetConstructor(new Type[0]).Invoke(new object[0]);

            return entity; 
        }*/

      /*  public virtual IEnumerable<TEntity> GetAll(bool includeDeleted = false)
        {
            return BitcoinRhExplorerServer.Current.DbContext.Set<TEntity>()
                .Where(e => (!e.IsDeleted) || (e.IsDeleted && includeDeleted));
        }

        public virtual TEntity GetById(long id, bool includeDeleted = false)
        {
            return BitcoinRhExplorerServer.Current.DbContext.Set<TEntity>()
                .FirstOrDefault(e => (e.Id == id) && ((!e.IsDeleted) || (e.IsDeleted && includeDeleted)));
        }

        public virtual IEnumerable<TEntity> GetByIds(IEnumerable<long> ids, bool includeDeleted = false)
        {
            return BitcoinRhExplorerServer.Current.DbContext.Set<TEntity>().AsQueryable()
                .Where(e => ids.Contains(e.Id) && ((!e.IsDeleted) || (e.IsDeleted && includeDeleted)));
        }

        public virtual int Add(TEntity entity)
        {
            entity.IfDefined(richEntity =>
            {
                richEntity.CreatorId = GetCurrentActorId();
            });

            BitcoinRhExplorerServer.Current.DbContext.Entry(entity).State = System.Data.Entity.EntityState.Added;
            return Save();
        }

        public virtual int Update(TEntity entity)
        {
            entity.IfDefined(richEntity =>
            {
                richEntity.UpdatedUtc = DateTime.UtcNow;
                richEntity.UpdaterId = GetCurrentActorId();
            });

            BitcoinRhExplorerServer.Current.DbContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            return Save();
        }

        public virtual int Delete(TEntity entity)
        {
            entity.IfDefined(richEntity =>
            {
                richEntity.UpdatedUtc = DateTime.UtcNow;
                richEntity.UpdaterId = GetCurrentActorId();
                richEntity.IsDeleted = true;
            });

            BitcoinRhExplorerServer.Current.DbContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            return Save();
        }

        public virtual IQueryable<TEntity> FindBy(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate, bool includeDeleted = false)
        {
            return BitcoinRhExplorerServer.Current.DbContext.Set<TEntity>().Where(predicate).Where(e => (!e.IsDeleted) || (e.IsDeleted && includeDeleted));
        }

        private int Save()
        {
            BitcoinRhExplorerServer.Current.DbContext.ChangeTracker.DetectChanges();
            return BitcoinRhExplorerServer.Current.DbContext.SaveChanges();
        }

        private long? GetCurrentActorId()
        {
            var context = HttpContext.Current;

            if (context.User != null && context.User.Identity.IsAuthenticated)
            {
                const string key = "UserIdRequest";

                if (HttpContext.Current.Session[key] == null)
                {
                    var membershipUser = Membership.GetUser(context.User.Identity.Name);
                    if (membershipUser != null)
                    {
                        var providerUserKey = membershipUser.ProviderUserKey;
                        if (providerUserKey != null)
                        {
                            HttpContext.Current.Session[key] = providerUserKey;
                            return (int) providerUserKey;
                        }
                    }
                }
                else
                {
                    return (int)HttpContext.Current.Session[key];         
                }            
            }

            return null;
        }*/
    }
}
