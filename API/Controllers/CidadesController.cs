using API.Models;
using API.Repository;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{ 
    [ApiController]
    [Route("paises/{idPais}/estados/{idEstado}/cidades")]
    public class CidadesController : ControllerBase
    {
        [HttpGet]
        //public List<Cidade> GetCidades()
        //{
        //    return CidadeRepository.Cidades!;
        //}

        public ActionResult<List<Cidade>> GetCidades(
            [FromQuery] string? nome,
            [FromQuery, Range(100, 100000)] int fromPopulacao,
            [FromRoute, Required] int idPais,
            [FromRoute, Required] int idEstado
            )
        {
            var resultado = CidadeRepository.Cidades!.Where(cidade => cidade.IdPais == idPais &&
                cidade.IdEstado == idEstado).ToList();

            if(resultado.Count == 0)
            {
                return NotFound(null);
            }
            
            if (!string.IsNullOrEmpty(nome))
            {
                resultado = resultado!.Where(cidade => cidade.Nome == nome).ToList();
            }

            if (fromPopulacao > 0)
            {
                resultado = resultado!.Where(cidade => cidade.Populacao >= fromPopulacao).ToList();
            }

            return Ok(resultado!);
        }

        [HttpPost]
        public ActionResult PostCidades(
           [FromRoute, Required] int idPais,
           [FromRoute, Required] int idEstado,
           [FromBody] Cidade cidade
           )

        {
            cidade.IdEstado = idEstado;
            cidade.IdPais = idPais;
            CidadeRepository.Cidades!.Add(cidade);
            CidadeRepository.Save();

            string? locationUrl = $"/paises/{idPais}/estados/{idEstado}/cidades/{cidade.Id}";

            return Created(locationUrl, null);
        }
    };
}
