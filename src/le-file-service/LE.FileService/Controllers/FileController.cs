using LE.FileService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LE.FileService.Controllers
{
    [Route("api/v1/files")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<FileController> _logger;
        private readonly IFileService _fileService;

        public FileController(ILogger<FileController> logger, IFileService fileService)
        {
            _logger = logger;
            _fileService = fileService;
        }

        [HttpGet("url")]
        public async Task<IActionResult> GetDownloadUrlFromStreamIdAsync([FromQuery] string[] streamId, CancellationToken cancellationToken = default)
        {
            if (!(streamId?.Any() ?? false))
                return BadRequest();

            var result = streamId.Select(async id =>
            {
                var url = await _fileService.GetDownloadUrlFromStreamIdAsync(id, cancellationToken: cancellationToken);

                return KeyValuePair.Create(id, url);

            });

            return Ok(await Task.WhenAll(result));
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UploadAsync(int id, [FromForm] IFormFileCollection files, CancellationToken cancellationToken = default)
        {
            if (files.Count == 0)
            {
                files = HttpContext.Request.Form.Files;
            }

            var fileStoreDtos = await Task.WhenAll(files.Select(file => _fileService.UploadAsync(id, file, cancellationToken: cancellationToken)));

            return Ok(fileStoreDtos);
        }
    }
}
