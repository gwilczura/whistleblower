using Microsoft.AspNetCore.Mvc;
using UploadImageForProcessing.Authorization;
using UploadImageForProcessing.Models;
using UploadImageForProcessing.Repositories;

namespace UploadImageForProcessing.Controllers
{
    [ApiController]
    [ApiKey]
    [Route("[controller]")]
    public class ResolveAlertController : ControllerBase
    {
        private readonly TableRepository _tableRepository;

        public ResolveAlertController(
            TableRepository tableRepository)
        {
            _tableRepository = tableRepository;
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(string fileId)
        {
            try
            {

                var typedCutoff = long.Parse(fileId);
                await _tableRepository.ResolveAlertAsync(typedCutoff);

                return new OkObjectResult(new MessageResult
                {
                    Message = "OK"
                });
            }
            catch (Exception ex)
            {

                return new OkObjectResult(new MessageResult
                {
                    Message = ex.Message
                }); ;
            }
        }
    }
}
