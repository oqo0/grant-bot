namespace GrantBot.Data.Repositories;

public interface IRepository<TId, TType>
{
    public TId Create(TType data);
    
    public TType? GetById(TId id);
    
    public IList<TType> GetAll();
    
    public bool Update(TType data);
    
    public bool Delete(TId id);
}