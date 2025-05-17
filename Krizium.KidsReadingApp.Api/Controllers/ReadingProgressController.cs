using Krizium.KidsReadingApp.Api.Models;
using Krizium.KidsReadingApp.Api.Models.Requests;
using Krizium.KidsReadingApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Krizium.KidsReadingApp.Api.Models;

namespace Krizium.KidsReadingApp.Api.Controllers
{
    [ApiController]
    [Route("api/progress")]
    public class ReadingProgressController : ControllerBase
    {
        private readonly IReadingProgressService _progressService;

        public ReadingProgressController(IReadingProgressService progressService)
        {
            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));
        }

        /// <summary>
        /// Get reading progress for a specific book and user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="bookId">Book ID</param>
        /// <returns>Reading progress details</returns>
        [HttpGet("{userId}/{bookId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReadingProgressDto>> GetReadingProgress(int userId, int bookId)
        {
            var progress = await _progressService.GetReadingProgressAsync(userId, bookId);
            
            if (progress == null)
            {
                return NotFound($"Reading progress for user {userId} and book {bookId} not found");
            }
            
            return Ok(progress);
        }
        
        /// <summary>
        /// Get all reading progress for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of reading progress records</returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ReadingProgressDto>>> GetUserReadingProgress(int userId)
        {
            var progressRecords = await _progressService.GetUserReadingProgressAsync(userId);
            return Ok(progressRecords);
        }
        
        /// <summary>
        /// Get the most recently read books for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="count">Number of books to return</param>
        /// <returns>List of recent books with progress</returns>
        [HttpGet("{userId}/recent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<BookProgressDto>>> GetRecentBooks(int userId, [FromQuery] int count = 5)
        {
            if (count < 1) count = 1;
            if (count > 20) count = 20;
            
            var recentBooks = await _progressService.GetRecentBooksAsync(userId, count);
            return Ok(recentBooks);
        }
        
        /// <summary>
        /// Update reading progress
        /// </summary>
        /// <param name="request">Reading progress update details</param>
        /// <returns>Updated reading progress</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReadingProgressDto>> UpdateReadingProgress(ReadingProgressRequest request)
        {
            if (request == null)
            {
                return BadRequest("Reading progress data is required");
            }
            
            if (request.UserId <= 0)
            {
                return BadRequest("Valid user ID is required");
            }
            
            if (request.BookId <= 0)
            {
                return BadRequest("Valid book ID is required");
            }
            
            if (request.PageNumber <= 0)
            {
                return BadRequest("Page number must be greater than 0");
            }
            
            try
            {
                var progress = await _progressService.UpdateReadingProgressAsync(request);
                return Ok(progress);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        /// <summary>
        /// Reset reading progress for a book
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="bookId">Book ID</param>
        /// <returns>No content</returns>
        [HttpPost("{userId}/{bookId}/reset")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResetReadingProgress(int userId, int bookId)
        {
            var success = await _progressService.ResetReadingProgressAsync(userId, bookId);
            
            if (!success)
            {
                return NotFound($"Reading progress for user {userId} and book {bookId} not found");
            }
            
            return NoContent();
        }
        
        /// <summary>
        /// Delete reading progress
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="bookId">Book ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{userId}/{bookId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteReadingProgress(int userId, int bookId)
        {
            var success = await _progressService.DeleteReadingProgressAsync(userId, bookId);
            
            if (!success)
            {
                return NotFound($"Reading progress for user {userId} and book {bookId} not found");
            }
            
            return NoContent();
        }
    }
}
