using Api.Helper;
using Core.Contracts;
using Core.DataTransferObjects;

using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    /// <summary>
    /// Controller to manage measurements
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MeasurementsController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        public IUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitOfWork"></param>
        public MeasurementsController(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        /// <summary>
        /// get all measurements ordered by time desc
        /// optional filtered by itemname
        /// </summary>
        /// <response code="200">Die Abfrage war erfolgreich.</response>
        [HttpGet]
        [ProducesResponseType(typeof(MeasurementGetDto[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery]string? itemname)
        {
            var measurements = await UnitOfWork.MeasurementRepository
                .GetFilteredAsync(itemname);
            return Ok(measurements);
        }

    }
}
