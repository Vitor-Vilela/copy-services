using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnimalCatalog.API.Data.Context;
using AnimalCatalog.API.DTOs.Requests;
using AnimalCatalog.API.DTOs.Responses;
using AnimalCatalog.API.Entities;

namespace AnimalCatalog.API.Controllers
{
    /// <summary>
    /// Controlador responsável por gerenciar as operações relacionadas aos animais.
    /// </summary>
    [ApiController]
    [Route("api/animals")]
    public class AnimalController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public AnimalController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtém a lista de todos os animais ativos.
        /// </summary>
        /// <returns>Retorna uma lista de animais ativos no formato AnimalResponse.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var animals = await _context.Animals.Where(a => a.Status).ToListAsync();
            var response = _mapper.Map<IEnumerable<AnimalResponse>>(animals);
            return Ok(response);
        }

        /// <summary>
        /// Obtém um animal pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador do animal.</param>
        /// <returns>Retorna o animal correspondente ao id informado no formato AnimalResponse ou NotFound se não encontrado.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id && a.Status);
            if (animal == null)
            {
                return NotFound("Animal não encontrado.");
            }
            var response = _mapper.Map<AnimalResponse>(animal);
            return Ok(response);
        }

        /// <summary>
        /// Cria um novo animal com os dados fornecidos.
        /// </summary>
        /// <param name="request">Dados do animal para criação.</param>
        /// <param name="animalPic">Imagem do animal.</param>
        /// <returns>Retorna o animal criado no formato AnimalResponse.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] AnimalRequest request, IFormFile animalPic)
        {
            var animal = _mapper.Map<Animal>(request);
            animal.Status = true;

            if (animalPic != null)
            {
                using (var ms = new MemoryStream())
                {
                    await animalPic.CopyToAsync(ms);
                    animal.AnimalPic = ms.ToArray();
                }
            }

            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();

            var response = _mapper.Map<AnimalResponse>(animal);
            return CreatedAtAction(nameof(GetById), new { id = animal.Id }, response);
        }
    }
}