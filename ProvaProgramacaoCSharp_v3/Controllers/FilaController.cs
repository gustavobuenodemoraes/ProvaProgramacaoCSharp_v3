using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ProvaProgramacaoCSharp_v3.Entities;
using System.Collections.Generic;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProvaProgramacaoCSharp_v3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilaController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly string key = "Fila";

        public FilaController(IMemoryCache cache)
        {
            _cache = cache;
        }

        // GET: api/Fila
        [HttpGet]
        public ActionResult<FilaArmazemando> GetItemFila()
        {
            var fila = GetFila();
            if (!fila.Any())
            {
                return NotFound("Lista vazia!");
            }

            return fila.LastOrDefault();
        }

        // POST api/Fila
        [HttpPost]
        public ActionResult AddItemFila([FromBody] List<FilaArmazemando> filaArmazemando)
        {
            var fila = GetFila();

            fila.AddRange(filaArmazemando);

            _cache.Set(key, fila);

            return Ok("Item adicionado a fila com sucesso!");
        }

        private List<FilaArmazemando> GetFila()
        {
            var fila = _cache.Get<List<FilaArmazemando>>(key);
            return fila == null ? new List<FilaArmazemando>() : fila;

        }
    }
}
