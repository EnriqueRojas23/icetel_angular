using System;
using System.ComponentModel.DataAnnotations;
using CargaClic.Common;

namespace CargaClic.Domain.Seguimiento
{
    public class Site : Entity
    {
        [Key]
        public int id { get; set; }
        public int? carga_id { get; set; }
        public DateTime? fecha_registro { get; set; }
        public int? usuario_id { get; set; }
        public string nombre_site { get; set; }
        public decimal? latitud { get; set; }
        public decimal? longitud { get; set; }
        public int? distrito_id { get; set; }
        public string direccion { get; set; }
        public string numero_suministro { get; set; }
        public int? estado_id { get; set; }
        public decimal? presupuesto_ingreso { get; set; }
        public decimal? presupuesto_costo { get; set; }
        public decimal? real_ingreso { get; set; }
        public decimal? real_costo { get; set; }
        public string geom { get; set; }
    }
}