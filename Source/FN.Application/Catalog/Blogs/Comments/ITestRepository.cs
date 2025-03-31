namespace FN.Application.Catalog.Blogs.Comments
{
    public interface ITestRepository
    {
        Task<TestProduct?> GetByIdAsync(string id);
        Task<IEnumerable<TestProduct>> GetAllAsync();
        Task<string> AddAsync(TestProduct product);
        Task UpdateAsync(TestProduct product);
        Task DeleteAsync(string id);
    }
}
