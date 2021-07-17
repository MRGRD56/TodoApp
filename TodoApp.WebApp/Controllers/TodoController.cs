using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoApp.WebApp.Services.Repositories;

namespace TodoApp.WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            
            var todoItems = await _todoItemsRepository.GetAsync(page);
            return todoItems.Any()
                ? Ok(todoItems)
                : NotFound("The specified page contains no elements");
        }
    }
}