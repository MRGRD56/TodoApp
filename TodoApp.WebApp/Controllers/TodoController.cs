using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
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
        public async Task<IActionResult> Get(
            [FromQuery] int page, 
            CancellationToken cancellationToken)
        {
            if (page < 0)
            {
                return BadRequest("Page index cannot be less than zero");
            }

            return Ok(await _todoItemsRepository.GetAsync(page, cancellationToken: cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> Post(
            [FromBody] TodoPostModel todoPostModel, 
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            return Ok(await _todoItemsRepository.AddAsync(todoPostModel.Text, cancellationToken));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(
            [Required(ErrorMessage = "Specify ID")]
            [Range(1, int.MaxValue, ErrorMessage = "Invalid ID")]
            [FromRoute] int id, 
            [FromBody] TodoPutModel todoPutModel, 
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            todoPutModel.Deconstruct(out var text, out var isDone);

            return Ok(await _todoItemsRepository.EditAsync(id, text, isDone, cancellationToken));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            [FromRoute] int id, 
            [FromQuery] bool restore, 
            CancellationToken cancellationToken)
        {
            return Ok(await _todoItemsRepository.SetDeletedAsync(id, !restore, cancellationToken));
        }
    }
}