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
            await Console.Out.WriteLineAsync($"Received sequence.");

            if (segment.Segment.Length > MaxSegmentByteSize * BitsInByte)
            {
                await Console.Out.WriteLineAsync("Error: Segment Overflow");
                return BadRequest(new { Error = "Segment size overflow." });
            }

            foreach (var bit in segment.Segment)
            {
                if (!ValidBits.Contains(bit))
                {
                    await Console.Out.WriteLineAsync("Error: Invalid Sequence");
                    return BadRequest(new { Error = "Invalid byte sequence." });
                }
            }
            
            await Console.Out.WriteLineAsync($"Sequence length: {segment.Segment.Length} bits");

            try
            {
                var coded = CycleEncoder.Encode(segment.Segment);
                await Console.Out.WriteLineAsync($"> Encode Finished. Total length: {coded.Length}.");

                coded = CycleEncoder.TryCorrupt(coded);
                await Console.Out.WriteLineAsync("> Corrupt Finished. ");

                coded = CycleEncoder.Decode(coded);
                await Console.Out.WriteLineAsync($"> Decode Finished. Total length: {coded.Length}.");
                
                await Console.Out.WriteLineAsync($"Equal Flag: {segment.Segment == coded}");
                segment.Segment = coded;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
            
            if (Random.Shared.Next(100) >= 2)
            {
                var jsonBody = JsonConvert.SerializeObject(segment);

                var host = "http://127.0.0.1:8000/transfer";
                // var host = "http://192.168.207.124:8000/transfer";

                await Console.Out.WriteLineAsync($"Calling {host}...");
                var client = new RestClient(host);
                var request = new RestRequest()
                {
                    Method = Method.Post
                };
                request.AddHeader("Content-Type", "application/json");
                request.AddBody(jsonBody);

                var response = await client.ExecuteAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    await Console.Out.WriteLineAsync("Segment sent.");
                }
                else
                {
                    await Console.Out.WriteLineAsync($"Segment was not received by the host ({host}) ({response.Content ?? "No More Data"})");
                }
                return Ok(segment);
            }
            else
            {
                await Console.Out.WriteLineAsync("Segment lost.");
            }
            await Console.Out.WriteLineAsync();

            return StatusCode(500, new { Message = "Internal Server Error" });
            //return Ok(new { Message = "Success" });
        }
    }
}