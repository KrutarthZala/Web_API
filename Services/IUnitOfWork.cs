namespace Product_CRUD_Web_API.Services
{
    public interface IUnitOfWork
    {
        IProductService Product { get; }
        IUserService User { get; }
        void Save();
    }
}
