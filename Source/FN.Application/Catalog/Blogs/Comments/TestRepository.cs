using Google.Cloud.Firestore;

namespace FN.Application.Catalog.Blogs.Comments
{
    public class TestRepository : ITestRepository
    {
        private readonly FirestoreDb _db;
        private const string CollectionName = "products";
        public TestRepository(FirestoreDb db)
        {
            _db = db;
        }

        public async Task<TestProduct?> GetByIdAsync(string id)
        {
            var snapshot = await _db.Collection(CollectionName).Document(id).GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ConvertTo<TestProduct?>() : null;
        }

        public async Task<IEnumerable<TestProduct>> GetAllAsync()
        {
            var snapshot = await _db.Collection(CollectionName).GetSnapshotAsync();
            return snapshot.Documents.Select(doc =>
            {
                var product = doc.ConvertTo<TestProduct>();
                product.Id = doc.Id;
                return product;
            });
        }

        public async Task<string> AddAsync(TestProduct product)
        {
            product.CreatedAt = DateTime.UtcNow;

            var docRef = await _db.Collection(CollectionName).AddAsync(product);
            return docRef.Id;
        }

        public async Task UpdateAsync(TestProduct product)
        {
            var docRef = _db.Collection(CollectionName).Document(product.Id);
            await docRef.SetAsync(product, SetOptions.MergeAll);
        }

        public async Task DeleteAsync(string id)
        {
            await _db.Collection(CollectionName).Document(id).DeleteAsync();
        }
    }
}
