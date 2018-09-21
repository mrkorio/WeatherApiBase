using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE;
using BE.Models;
using BL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace WeatherApi.Controllers
{
    [Route("api/[controller]")]
    public class WeatherController : Controller
    {
        private static WeatherConfig _configSettigs;        

       

        public WeatherController(IOptions<WeatherConfig> config)
        {
            _configSettigs = config.Value;
            WeatherHelper helper = new WeatherHelper(_configSettigs);
        }

      
        [HttpGet("GetSucursales")]
        [EnableCors("AllowSpecificOrigin")]
        public IActionResult GetSucursales()
        {
            try
            {
                List<Sucursal> list = WeatherHelper.GetSucursales();

              
                return Ok(list);
            }
            catch (Exception ex)
            {

                 return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    
        [HttpGet("GetRegiones")]
        [EnableCors("AllowSpecificOrigin")]
        public IActionResult GetRegiones()
        {
            try
            {
                List<Region> list = WeatherHelper.GetRegiones();


                return Ok(list);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
       
        [HttpGet("byCoordenadas")]
        [EnableCors("AllowSpecificOrigin")]
        public IActionResult GetClima(float latitud, float longitud)
        {
            try
            {
                Clima clima = WeatherHelper.GetClima(latitud, longitud);


                return Ok(clima);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    
        [HttpGet("byNombreRegion")]
        [EnableCors("AllowSpecificOrigin")]
        public IActionResult GetClima(string nombreRegion)
        {
            try
            {
                Clima clima = WeatherHelper.GetClima(nombreRegion);


                return Ok(clima);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }



        // POST api/values
        
        [HttpPost("AddSucursal")]
        [EnableCors("AllowSpecificOrigin")]
        public IActionResult AddSucursal([FromBody] Sucursal sucursal)
        {
            try
            {

                WeatherHelper.AddSucursal(sucursal);

                return Ok();
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
