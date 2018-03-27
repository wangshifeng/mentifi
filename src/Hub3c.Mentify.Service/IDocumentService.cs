using Hub3c.Mentify.Service.Models;

namespace Hub3c.Mentify.Service
{
    public interface IDocumentService
    {
        DocumentModel GetById(int id);
    }
}
