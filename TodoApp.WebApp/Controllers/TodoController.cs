using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Infrastructure.Models.RequestModels.Todo;
using TodoApp.WebApp.Extensions;
using TodoApp.WebApp.Services.Repositories;

namespace TodoApp.WebApp.Controllers
{
    [ApiController]
    [Route("api/todo")]
    [Authorize]
    public class TodoController : ControllerBase
    {
        private int CurrentUserId => User.GetUserId();
        
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

            return Ok(await _todoItemsRepository.GetAsync(CurrentUserId, page, cancellationToken: cancellationToken));
        }

        [HttpGet("get_after")]
        public async Task<IActionResult> GetAfter(
            [FromQuery] int afterId,
            CancellationToken cancellationToken)
        {
            if (afterId < 0)
            {
                return BadRequest("Invalid ID");
            }

            int? nullableAfterId = afterId == default ? null : afterId;

            return Ok(await _todoItemsRepository.GetAfterAsync(CurrentUserId, nullableAfterId, cancellationToken: cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> Post(
            [FromBody] TodoPostModel todoPostModel, 
            CancellationToken cancellationToken)
        {
            return Ok(await _todoItemsRepository.AddAsync(CurrentUserId, todoPostModel.Text, cancellationToken));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(
            [Required(ErrorMessage = "Specify ID")]
            [Range(1, int.MaxValue, ErrorMessage = "Invalid ID")]
            [FromRoute] int id, 
            [FromBody] TodoPutModel todoPutModel, 
            CancellationToken cancellationToken)
        {
            var (text, isDone) = todoPutModel;

            return Ok(await _todoItemsRepository.EditAsync(CurrentUserId, id, text, isDone, cancellationToken));
        }

        [HttpPut("toggle_done")]
        public async Task<IActionResult> Put(
            [FromBody] TodosPutModel todosPutModel,
            CancellationToken cancellationToken)
        {
            var todoItemsStream = _todoItemsRepository.ToggleDoneAsync(CurrentUserId, todosPutModel.Id, cancellationToken);
            return Ok(await todoItemsStream.ToListAsync(cancellationToken));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            [FromRoute] int id, 
            [FromQuery] bool restore, 
            CancellationToken cancellationToken)
        {
            return Ok(await _todoItemsRepository.SetDeletedAsync(CurrentUserId, id, !restore, cancellationToken));
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
                _todoItemsRepository.SetDeletedAsync(CurrentUserId, todoItemsIds, !restore, cancellationToken);
            return Ok(await todoItemsStream.ToListAsync(cancellationToken));
        }
    }
}