using Microsoft.AspNetCore.Mvc;
using UploadImageForProcessing.Authorization;
using UploadImageForProcessing.Models;
using UploadImageForProcessing.Repositories;

namespace UploadImageForProcessing.Controllers
{
    [ApiController]
    [ApiKey]
    [Route("[controller]")]
    public class CounterController : ControllerBase
    {
        private readonly CountersRepository _counterRepository;

        public CounterController(
            CountersRepository counterRepository)
        {
            _counterRepository = counterRepository;
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(string counterId, int count)
        {
            try
            {
                _counterRepository.SetCounter(counterId, count);

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

        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            try
            {
                var result = await _counterRepository.GetCountersAsync();
                return new OkObjectResult(new CounterQueryResult
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
