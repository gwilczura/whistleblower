using Microsoft.AspNetCore.Mvc;
using UploadImageForProcessing.Authorization;
using UploadImageForProcessing.Models;
using UploadImageForProcessing.Repositories;

namespace UploadImageForProcessing.Controllers
{
    [ApiController]
    [ApiKey]
    [Route("[controller]")]
    public class FileDataController : ControllerBase
    {
        private readonly TableRepository _tableRepository;

        public FileDataController(
            TableRepository tableRepository)
        {
            _tableRepository = tableRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync(string? cutoffId, int? pageSize, bool? hasAlert, bool? resolvedAlert)
        {
            try
            {
                var typedCutoff = cutoffId != null
                    ? long.Parse(cutoffId)
                    : 0;
                var result = await _tableRepository.GetFromTableAsync(typedCutoff, pageSize, hasAlert, resolvedAlert);
                return new OkObjectResult(new QueryResult
                {
                    Message = "OK",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new MessageResult
                {
                    Message = ex.Message
                });
            }
        }
    }
}
