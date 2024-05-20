using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using RestSharp;
using WebApi_KR.Helpers;
using WebApi_KR.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApi_KR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SegmentsController : ControllerBase
    {
        public const int BitsInByte = 8;
        private char[] ValidBits = ['0', '1'];

        public int MaxSegmentByteSize { get; set; } = 130;

        /// <summary>
        /// Codes incoming <paramref name="segment"/> with cycle [15;11] code, corrupts it, decodes and tries to recover using error syndrome.
        /// </summary>
        /// <param name="segment">The segment data.</param>
        /// <response code="400">Request violates requirements.</response>
        /// <response code="500">Internal native C# error.</response>
        [HttpPost]
        [Route("Code")]
        public async Task<IActionResult> Code([FromBody] SegmentDto segment)
        {
            if (segment.Segment.Length > MaxSegmentByteSize * BitsInByte)
                return BadRequest(new { Error = "Segment size overflow." });

            foreach (var bit in segment.Segment)
                if (!ValidBits.Contains(bit))
                    return BadRequest(new { Error = "Invalid byte sequence." });

            try
            {
                var coded = CycleEncoder.Encode(segment.Segment);
                coded = CycleEncoder.TryCorrupt(coded);
                coded = CycleEncoder.Decode(coded);
                //coded = CycleEncoder.TryHeal(coded);
                segment.Segment = coded;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
            
            if (Random.Shared.Next(100) >= 2)
            {
                var jsonBody = JsonConvert.SerializeObject(segment);

                //var client = new RestClient("http://127.0.0.1:8000/transfer");
                var client = new RestClient("http://192.168.207.124:8000/transfer");
                var request = new RestRequest()
                {
                    Method = Method.Post
                };
                request.AddHeader("Content-Type", "application/json");
                request.AddBody(jsonBody);

                var response = await client.ExecuteAsync(request);
                return Ok(segment);
            }

            return StatusCode(500, new { Message = "Internal Server Error" });
            //return Ok(new { Message = "Success" });
        }
    }
}