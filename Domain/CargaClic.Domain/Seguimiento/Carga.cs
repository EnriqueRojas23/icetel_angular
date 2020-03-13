using System;
using System.ComponentModel.DataAnnotations;
using CargaClic.Common;

namespace CargaClic.Domain.Seguimiento
{
    public class CargaMasiva : Entity
    {
        [Key]
        public int id { get; set; }
        public DateTime? fecha_registro { get; set; }
        public int? usuario_id { get; set; }
        public int? estado_id { get; set; }
        public decimal? presupuesto { get; set; }
        public decimal? real { get; set; }
        public string nombre_archivo {get;set;}
        public string tipo_proyecto {get;set;}
    }
}