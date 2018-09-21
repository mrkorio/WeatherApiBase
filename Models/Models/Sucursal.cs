using System;
using System.Collections.Generic;
using System.Text;

namespace BE.Models
{
   public class Sucursal
    {
        public int Id { get; set; }

        public Region Region { get; set; }

        public List<Clima> Climas { get; set; }

        public Coordenadas Coordenadas { get; set; }

        public string Descripcion { get; set; }


        public Sucursal()
        {
            this.Region = new Region();
            this.Coordenadas = new Coordenadas();
            this.Climas = new List<Clima>();
        }


        public Sucursal(string nombreRegion)
        {
            this.Region = new Region();
            this.Region.Descripcion = nombreRegion;
        }
    }
}
