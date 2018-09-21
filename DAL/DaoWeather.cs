using BE;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using BE.Models;
using System.Data;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using DAL.External_Services;
using BE.Requests;

namespace DAL
{
    public class DaoWeather
    {


        private readonly WeatherConfig _config;

        private const string END_POINT_WEATHHER = "http://api.openweathermap.org/data/2.5";

        private const string APPID = "7687647345808bec2d7976a2c2b911a2";


        public DaoWeather(WeatherConfig config)
        {
            this._config = config;
        }


       public List<Region> GetRegion(int idRegion = 0)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(" SELECT * FROM dbo.Region ");
            if (idRegion > 0)
            {
                sb.Append(" WHERE id = " + idRegion);
            }

            using (IDbConnection conn = Connection.GetConnection(_config.ConnectionString))
            {
                conn.Open();
                var list = conn.Query<Region>(sb.ToString()).AsList();

                return list;
                
            }

        }

        public List<Sucursal> GetSucursal(int idSucursal)
        {
            StringBuilder sb = new StringBuilder();
            List<Sucursal> list = new List<Sucursal>();

            sb.Append(" SELECT s.id, s.descripcion, r.id, r.descripcion, s.latitud, s.longitud  ");
            sb.Append(" FROM dbo.Sucursal s INNER JOIN dbo.Region r ON r.id = s.id_region  ");
            if (idSucursal > 0)
            {
                sb.Append(" WHERE s.id = " + idSucursal);
            }

            using (IDbConnection conn = Connection.GetConnection(_config.ConnectionString))
            {
                conn.Open();
                list = conn.Query<Sucursal, Region, Sucursal>(sb.ToString(),(sucursal, region) =>
                {
                    sucursal.Region = region;
                    return sucursal;
                }).AsList();

                foreach (var item in list)
                {
                    if (item.Coordenadas.Empty())
                    {
                        Clima clima = GetClima(item);
                        item.Climas = new List<Clima>();
                        item.Climas.Add(clima);
                        GuardarSucursalClima(clima, item);
                    }
                    else
                    {
                        Clima clima = GetClima(item.Coordenadas);
                        item.Climas.AsList().Add(clima);
                        GuardarSucursalClima(clima, item);
                    }
                   
                }

            }

            return list;
            
        }


        private int GuardarClima(Clima clima)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(" SELECT * FROM dbo.Clima ");
            if (clima.Id > 0)
            {
                sb.Append(" WHERE id = " + clima.Id);
            }

            using (IDbConnection conn = Connection.GetConnection(_config.ConnectionString))
            {

                if (!conn.State.Equals(ConnectionState.Open))
                {
                    conn.Open();
                }

                var result = conn.QueryFirstOrDefault<Clima>(sb.ToString());

                if (result == null) //El clima no existe en la base de datos
                {
                    sb.Clear();
                    sb.Append(" INSERT INTO dbo.Clima (id, descripcion) ");
                    sb.Append(" VALUES (@id, @descripcion) ");

                    var rowsAffected = conn.Execute(sb.ToString(), new { id= clima.Id , descripcion = clima.Descripcion });

                    return rowsAffected;
                }

            }

            return 0;

        }


        private int GuardarSucursalClima(Clima clima, Sucursal sucursal)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                GuardarClima(clima);

                sb.Append(" INSERT INTO dbo.Sucursal_Clima (id_sucursal, id_clima, fecha) ");
                sb.Append(" VALUES (@idSucursal, @idClima, @fecha)  ");


                using (IDbConnection conn = Connection.GetConnection(_config.ConnectionString))
                {
                    if (!conn.State.Equals(ConnectionState.Open))
                    {
                        conn.Open();
                    }
                   
                    var rowsAffected = conn.Execute(sb.ToString(), new { idSucursal = sucursal.Id, idClima = clima.Id, fecha = DateTime.Now.ToString() });

                    return rowsAffected;

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Clima GetClima(Coordenadas coordenadas)
        {
            Clima clima = new Clima();
            try
            {
                ServiceHelper<WeatherRequest> serviceHelper = new ServiceHelper<WeatherRequest>();

                var result = serviceHelper.MakeRequest(string.Format("{0}/weather?lat={1}&lon={2}&appid={3}", END_POINT_WEATHHER, coordenadas.Latitud, coordenadas.Latitud, APPID),
                    null,
                    "GET"
                    );
             
                if (result != null)
                {
                    clima.Descripcion = result.weather[0].description;
                    clima.Id = result.weather[0].id;
                    clima.Main = result.weather[0].main;
                }

               
                
            }
            catch (Exception e)
            {
                clima.Id = 9999;
                clima.Main = "No se puede obtener informacion";
                clima.Descripcion = "No se puede obtener informacion";
            }
            return clima;
        }

        public Clima GetClima(Sucursal sucursal)
        {
            Clima clima = new Clima();
            try
            {
                ServiceHelper<WeatherRequest> serviceHelper = new ServiceHelper<WeatherRequest>();

                var result = serviceHelper.MakeRequest(string.Format("{0}/weather?q={1}&appid={2}", END_POINT_WEATHHER, sucursal.Region.Descripcion, APPID),
                    null,
                    "GET"
                    );
                
                if (result != null)
                {
                    clima.Descripcion = result.weather[0].description;
                    clima.Id = result.weather[0].id;
                    clima.Main = result.weather[0].main;
                }

            

            }
            catch (Exception e)
            {
                clima.Id = 9999;
                clima.Main = "No se puede obtener informacion";
                clima.Descripcion = "No se puede obtener informacion";
            }
            return clima;

        }

        public int CreateSucursal(Sucursal sucursal)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(" INSERT INTO dbo.Sucursal (descripcion,codigo_sucursal,id_region, latitud, longitud) ");
                sb.Append(" VALUES (@descripcion, @codigoSucursal, @idRegion, @latitud, @longitud) ");


                using (IDbConnection conn = Connection.GetConnection(_config.ConnectionString))
                {
                    conn.Open();
                    var rowsAffected = conn.Execute(sb.ToString(), new
                    {
                        descripcion = sucursal.Descripcion,
                        codigoSucursal = sucursal.Descripcion + sucursal.Region.Id,
                        idRegion = sucursal.Region.Id,
                        latitud =  sucursal.Coordenadas.Latitud,
                        longitud = sucursal.Coordenadas.Longitud
                    });

                    return rowsAffected;

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
          
        }


      

    }
}
