using Microsoft.AspNetCore.Mvc;
using UploadImageForProcessing.Authorization;
using UploadImageForProcessing.Models;
using UploadImageForProcessing.Repositories;

namespace UploadImageForProcessing.Controllers
{
    [ApiController]
    [ApiKey]
    [Route("[controller]")]
    public class AlertController : ControllerBase
    {
        private readonly TableRepository _tableRepository;

        public AlertController(
            ILogger<UploadFileController> logger,
            TableRepository tableRepository,
            IConfiguration configuration)
        {
            _tableRepository = tableRepository;
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(AlertRequest request)
        {
            try
            {

                var typedFieldId = long.Parse(request.FileId);
                await _tableRepository.UpdateAlertAsync(typedFieldId, request.Alert);

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
