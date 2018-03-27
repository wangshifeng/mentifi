using System;
using Hub3c.Mentify.Service;
using Microsoft.AspNetCore.Mvc;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/Documents")]
    [ApiVersion("1")]
    public class DocumentsController : Controller
    {
        private readonly IDocumentService _documentService;

        /// <inheritdoc />
        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }


        /// <summary>
        /// Get User Photo
        /// </summary>
        /// <param name="id">System User Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public IActionResult Document([FromRoute]int id)
        {
            var data = _documentService.GetById(id);
            if (data != null)
            {
                return File(data.Content, data.Mime);
            }

            throw new ApplicationException("Document ID is invalid");
        }
    }
}