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
            
            var (text, isDone) = todoPutModel;

            return Ok(await _todoItemsRepository.EditAsync(id, text, isDone, cancellationToken));
        }

        [HttpPut("toggle_done")]
        public async Task<IActionResult> Put(
            [FromBody] TodosPutModel todosPutModel,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var todoItemsStream = _todoItemsRepository.ToggleDoneAsync(todosPutModel.Id, cancellationToken);
            return Ok(await todoItemsStream.ToListAsync(cancellationToken));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            [FromRoute] int id, 
            [FromQuery] bool restore, 
            CancellationToken cancellationToken)
        {
            return Ok(await _todoItemsRepository.SetDeletedAsync(id, !restore, cancellationToken));
        }
        
        [HttpDelete]
        public async Task<IActionResult> Delete(
            [FromBody] TodosDeleteModel todosDeleteModel, 
            CancellationToken cancellationToken)
        {
            var (ids, restore) = todosDeleteModel;

            var todoItemsIds = ids as int[] ?? ids.ToArray();
            
            if (!todoItemsIds.Any())
            {
                return BadRequest("The array of IDs is empty ('id' field)");
            }
            
            var todoItemsStream =
                _todoItemsRepository.SetDeletedAsync(todoItemsIds, !restore, cancellationToken);
            return Ok(await todoItemsStream.ToListAsync(cancellationToken));
        }
    }
}