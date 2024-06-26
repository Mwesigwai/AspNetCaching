using AspNetCaching.Models;
using AspNetCaching.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspNetCaching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController(DocumentService service)
        : ControllerBase
    {

        DocumentService _service = service;
        [HttpGet]
        public async Task<IEnumerable<Document>> Get()
        {
            return await Task.Run(()=> _service.GetAllDocuments());
        }

        [HttpGet("{id}",Name = "GetById")]
        public async Task<ActionResult<Document>> Get(int id)
        {
            var doc = await _service.GetDocument(id);
            if(doc is null)
                return BadRequest("Document not found");
            return Ok(doc);
        }

        // POST api/<DocumentController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DocumentModel doc)
        {
            var id = await _service.AddDocument(doc);
            if (id is not null)
                return CreatedAtRoute("GetById", id, doc);
            return BadRequest("try again");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.Delete(id);
            if (deleted)
                return NoContent();
            return BadRequest("document was not found");
        }
    }
}
