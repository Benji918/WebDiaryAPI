using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;
using WebDiaryAPI.Data;
using WebDiaryAPI.Models;

namespace WebDiaryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiaryEntriesController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;

        public DiaryEntriesController(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiaryModel>>> GetDiaryEntries()
        {
            return await _dbContext.DiaryModels.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DiaryModel>> GetDiaryEntryById (int id)
        {
            var diaryEntry = await _dbContext.DiaryModels.FindAsync(id);

            if (diaryEntry == null)
            {
                return NotFound();
            }

            return diaryEntry;
        }

        [HttpPost]
        public async Task<ActionResult<DiaryModel>> CreateDiaryEntry(DiaryModel diaryModel)
        {
            await _dbContext.DiaryModels.AddAsync(diaryModel);

            var resourseUrl = Url.Action(nameof(GetDiaryEntryById), new { id = diaryModel.Id });

            return Created(resourseUrl, diaryModel);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DiaryModel>> PutDiaryEntry(int id, [FromBody] DiaryModel diaryModel)
        {
            if (id != diaryModel.Id)
            {
                // Returns a 400 status code
                return BadRequest();
            }

            _dbContext.Entry(diaryModel).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiaryModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();

        }

        private bool DiaryModelExists(int id)
        {
            return _dbContext.DiaryModels.Any(d => d.Id == id);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<DiaryModel>> DeleteDiaryEntry(int id)
        {
            // Find the specific diary entry
            var diaryEntry = await _dbContext.DiaryModels.FindAsync(id);

            if (diaryEntry == null)
            {
                return NotFound();
            }

            _dbContext.DiaryModels.Remove(diaryEntry);

            await _dbContext.SaveChangesAsync();

            return NoContent();

        }
    }
}
