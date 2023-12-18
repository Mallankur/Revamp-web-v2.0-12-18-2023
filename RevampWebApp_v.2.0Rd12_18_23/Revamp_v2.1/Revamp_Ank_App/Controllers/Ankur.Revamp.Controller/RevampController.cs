using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver.Core.Configuration;
using Revamp_Ank_App.DomainEntites.Repositores.Entites;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;
using Revamp_Ank_App.Domain.Repositores.Entites;
using System.Diagnostics.Contracts;
using Revamp_Ank_App.Contract.Entites.RevampMongoCollection;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Revamp_Ank_App.Controllers.Ankur.Revamp.Controller
{
    [Route("api/[controller]/")]
    [ApiController]
    public class RevampController : ControllerBase
    {
        private readonly ISQLconnecterAnkur _repository;
        //private readonly IConfiguration _config;

        public RevampController(ISQLconnecterAnkur repository)
        {
            _repository = repository;
           
        }
        

        [HttpPost]
        [Route("PostBatchProcessingAsync")]
        // [ProducesResponseType(typeof(RevampMongoEntity , StatusCodes.Status201Created)]
        public async Task<IActionResult> PostBatchProcessingAsync(
             string storeProcedureName, int CycleId,
             [FromBody] string rdids)
        {


            var result = await _repository.CreateData_Using_SQL_SP_ConnectorAsync(storeProcedureName,CycleId,rdids);
            if (result)
            {

                 return Ok("Data received and processed");
            }

            return BadRequest(" This exception was originally thrown at this call stack:\r\n    " +
                "System.Text.Json.ThrowHelper.ThrowJsonReaderException(ref System.Text.Json.Utf8JsonReader, " +
                "System.Text.Json.ExceptionResource, byte, System.ReadOnlySpan<byte>)\r\n   " +
                " System.Text.Json.Utf8JsonReader.ConsumeValue(byte)\r\n    " +
                "System.Text.Json.Utf8JsonReader.ConsumeNextTokenUntilAfterAllCommentsAreSkipped(byte)\r\n  " +
                "  System.Text.Json.Utf8JsonReader.ConsumeNextToken(byte)  "); 
        }

        [HttpPost]
        [Route("GetDataByrdids")]

        public async Task<IActionResult> GetDataByrdidsAsync( 
            [FromBody] string  ridis   )

        {
            return Ok(ridis.ToString());
        }








    }
}
