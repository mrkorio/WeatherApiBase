using System;
using System.Collections.Generic;
using System.Text;

namespace BE.Models
{
    public class Coordenadas
    {

        public float Latitud { get; set; }

        public float Longitud { get; set; }

        public Coordenadas()
        {

        }

        public Coordenadas(float latitud, float longitud)
        {
            this.Latitud = latitud;
            this.Longitud = longitud;
        }

        public bool Empty()
        {
            return this.Latitud == 0 && this.Longitud == 0;
        }
    }
}
