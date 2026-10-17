using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppForSEII.API.DTOs;
using AppForSEII.API.Data;
using System.Net;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibroController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LibroController> _logger;

        public LibroController(ApplicationDbContext context, ILogger<LibroController> logger)
        {
            _context = context;
            _logger = logger;
        }
        
      [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<LibroParaResenarDTO>), (int) HttpStatusCode.OK)]
        public async Task<ActionResult> GetLibrosParaResenar()
        {
        
            var libros = await _context.Libros
                .Select(l => new LibroParaResenarDTO
                {
                    Id = l.Id,
                    Titulo = l.Titulo
                })
                .ToListAsync();

            return Ok(libros);
        }
    }
}