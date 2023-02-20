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
    [Route("api/files")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        //[HttpGet("url")]
        //public async Task<IActionResult> GetDownloadUrlFromStreamIdAsync([FromQuery] string[] streamId, CancellationToken cancellationToken = default)
        //{
        //    if (!(streamId?.Any() ?? false))
        //        return BadRequest();

        //    var result = streamId.Select(async id =>
        //    {
        //        var url = await _fileService.GetDownloadUrlFromStreamIdAsync(id, cancellationToken: cancellationToken);

        //        return KeyValuePair.Create(id, url);

        //    });

        //    return Ok(await Task.WhenAll(result));
        //}

        [HttpPost("users/{id}/types/{type}")]
        public async Task<IActionResult> UploadAsync(int id, string type, [FromForm] IFormFileCollection files, CancellationToken cancellationToken = default)
        {
            if (files.Count == 0)
            {
                files = HttpContext.Request.Form.Files;
            }

            var fileStoreDtos = await Task.WhenAll(files.Select(file => _fileService.UploadAsync(id, file, type, cancellationToken: cancellationToken)));

            return Ok(fileStoreDtos);
        }
    }
}
