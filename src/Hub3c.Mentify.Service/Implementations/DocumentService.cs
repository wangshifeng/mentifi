using Hub3c.Mentify.Repository.Models;
using Hub3c.Mentify.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Hub3c.Mentify.Service.Implementations
{
    public class DocumentService : IDocumentService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IRepository<DocumentRegister> _documentRepository;

        public DocumentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _documentRepository = unitOfWork.GetRepository<DocumentRegister>();
        }

        public DocumentModel GetById(int id)
        {
            return _documentRepository.GetFirstOrDefault(selector: a => new DocumentModel()
            {
                Name = a.DocumentName,
                Id = a.DocumentId,
                Content = a.DocumentContent,
                Mime = a.DocumentMimeType
            }, predicate: a => a.DocumentId == id);
        }
    }
}
