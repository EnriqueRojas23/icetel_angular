using System;


namespace CargaClic.API.Dtos.Seguimiento
{
    public class CargaMasivaForRegister
    {

        public int id { get; set; }
        public DateTime? fecha_registro { get; set; }
        public int? usuario_id { get; set; }
        public int? estado_id { get; set; }
        public decimal? presupuesto { get; set; }
        public decimal? real { get; set; }  
        public string nombre_archivo {get;set;}

    }
}