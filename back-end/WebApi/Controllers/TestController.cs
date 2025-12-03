using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qfile.Core.Servicios;
using Exceptionless;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private ITestServicio _servicio;

        public TestController(ITestServicio servicio)
        {
            _servicio = servicio;
        }

        // GET: api/Test
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                string val = await _servicio.GetTestAsync();
                throw new Exception("Test Exceptionless");
                return Ok(val);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                // todo: enviar al log de errores
                return StatusCode(500);
            }
        }

        [HttpGet("ReadinessProbe")]
        public async Task<IActionResult> ReadinessProbe()
        {
            try
            {
                string val = await _servicio.ReadinessProbe();
                return Ok(val);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500);
            }
        }


        /* Otros ejemplos de restfull -----------------------------------------

        // GET: api/Test/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Test
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Test/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        ------------------------------------------------------------------*/
    }
}