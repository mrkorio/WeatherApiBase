using System;
using System.Collections.Generic;
using BE;
using BE.Models;
using DAL;
using Microsoft.Extensions.Options;

namespace BL
{
    public class WeatherHelper
    {
        private static WeatherConfig config;



        public WeatherHelper(WeatherConfig _configSettigs)
        {
            config = _configSettigs;
        }



        public static List<Sucursal> GetSucursales()
        {
            try
            {
                var dao = new DaoWeather(config);
                List<Sucursal> list  = dao.GetSucursal(0);

                return list;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static int AddSucursal(Sucursal sucursal)
        {
            try
            {
                var dao = new DaoWeather(config);

                return dao.CreateSucursal(sucursal);            

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static Clima GetClima(float latitud, float longitud)
        {
            try
            {
                var dao = new DaoWeather(config);

                return dao.GetClima(new Coordenadas(latitud, longitud));
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static List<Region> GetRegiones()
        {
            try
            {
                var dao = new DaoWeather(config);
                return dao.GetRegion();
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public static Clima GetClima(string nombreRegion)
        {
            try
            {
                var dao = new DaoWeather(config);

                return dao.GetClima(new Sucursal(nombreRegion));
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
