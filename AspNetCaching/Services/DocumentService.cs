using AspNetCaching.Data;
using AspNetCaching.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace AspNetCaching.Services
{
    public class DocumentService(IDistributedCache cache,DocumentDbContext documentDb)
    {
        IDistributedCache _cache = cache;
        DocumentDbContext _documentDb = documentDb;

        public async Task<int?> AddDocument(DocumentModel document)
        {
            if (document?.Name is null)
                return null!;
            await _documentDb.Documents.AddAsync(new() { Name = document.Name});
            return await GetDocId(document);
            
        }

        public async Task<Document> GetDocument(int id)
        {
            var documentKey = $"document:{id}";
            var result = await _cache.GetStringAsync(documentKey);

            if (result is not null)
            {
                await Console.Out.WriteLineAsync("got obj from cache");
                var document = JsonConvert.DeserializeObject<Document>(result!);
                return document!;
            }

            await Console.Out.WriteLineAsync("No doc from cache going to local db");
            var dbDocument = await GetFromDb(id);
            await _cache.SetStringAsync(documentKey, JsonConvert.SerializeObject(dbDocument));
            return dbDocument;
        }

        public IEnumerable<Document> GetAllDocuments()
        {
            var allDocs = _documentDb.Documents;
            return allDocs;
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await GetFromDb(id);
            await _cache.RemoveAsync($"document:{id}");
            if (entity is not null)
            {
                _documentDb.Documents.Remove(entity);
                return true;
            }
            return false;
        }

        private async Task<Document> GetFromDb(int id)
        {
            return await Task.Run(() =>
            {
                var dbResult = _documentDb.Documents
                    .FirstOrDefault(d => d.Id == id);
                return dbResult!;
            });
        }

        private async Task<int?> GetDocId(DocumentModel document)
        {
            var id = await _documentDb.Documents
                      .Where(d => d.Name == document.Name)
                      .Select(x => x.Id).FirstOrDefaultAsync();

            return id;
        }
    }
}
