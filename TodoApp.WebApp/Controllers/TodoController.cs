using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Infrastructure.Models.RequestModels;
using TodoApp.Infrastructure.Models.RequestModels.Todo;
using TodoApp.WebApp.Services.Repositories;

namespace TodoApp.WebApp.Controllers
{
    [ApiController]
    [Route("api/todo")]
    public class TodoController : ControllerBase
    {
        private readonly TodoItemsRepository _todoItemsRepository;

        public TodoController(TodoItemsRepository todoItemsRepository)
        {
            _todoItemsRepository = todoItemsRepository;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int page)
        {
            if (page < 0)
            {
                return BadRequest("Page index cannot be less than zero");
            }
            
            return Ok(await _todoItemsRepository.GetAsync(page));
        }

        public async Task<IActionResult> Post([FromBody] string text)
        {
            return Ok(await _todoItemsRepository.AddAsync(text));
        }

        public async Task<IActionResult> Put([FromBody] TodoPutModel todoPutModel)
        {
            if (!ModelState.IsValid)
            {
                
            }
            
            var (id, text, isDone) = todoPutModel;

            return Ok(await _todoItemsRepository.EditAsync(id.Value, text, isDone));
        }
    }
}