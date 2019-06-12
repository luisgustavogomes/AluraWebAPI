using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AluraWebAPI.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class LivrosController : ControllerBase
    {
        //Created() --> 201
        //Ok() --> 200
        //BadRequest() --> 400
        //NoContent() --> 204
        // --EXEMPLO DE GET COM UM ARRAY NO QUER
        //[AllowAnonymous]
        //[HttpGet]
        //public ActionResult GetPowerPlants([FromQuery(Name = "id"]int[] id)
        //{
        //    ///condigo...
        //    return Ok(ids);
        //}

        private readonly IRepository<Livro> _repo;

        public LivrosController(IRepository<Livro> repository) => _repo = repository;

        [HttpGet]
        public IActionResult RecuperarListaDeLivros()
        {
            var lista = _repo.All.Select(l => l.ToApi()).ToList();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public IActionResult Recuperar(int id)
        {
            var model = _repo.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model.ToApi());
        }

        [HttpGet("{id}/capa")]
        public IActionResult ImagemCapa(int id)
        {
            byte[] img = _repo.All
                .Where(l => l.Id == id)
                .Select(l => l.ImagemCapa)
                .FirstOrDefault();
            if (img != null)
                return File(img, "image/png");
            return NotFound();
        }


        [HttpPost]
        public IActionResult Incluir([FromBody] LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                _repo.Incluir(livro);
                var uri = Url.Action("Recuperar", new { id = livro.Id });
                return Created(uri, livro);//201
            }
            return BadRequest();//404
        }

        [HttpPut]
        public IActionResult Alterar([FromBody] LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                if (model.Capa == null)
                {
                    livro.ImagemCapa = _repo.All
                        .Where(l => l.Id == livro.Id)
                        .Select(l => l.ImagemCapa)
                        .FirstOrDefault();
                }
                _repo.Alterar(livro);
                return Ok();//200
            }
            return BadRequest();//404
        }


        [HttpDelete("{id}")]
        public IActionResult Remover(int id)
        {
            var model = _repo.Find(id);
            if (model == null)
                return NotFound();
            _repo.Excluir(model);
            return NoContent();//204
        }

    }
}
