using System;


namespace CargaClic.API.Dtos.Seguimiento
{
    public class ActaConformidadForRegister
    {

        public int id	 {get;set;}
        public string ruta	 {get;set;}
        public string nombre	 {get;set;}
        public decimal monto {get;set;}
        public string numero_acta {get;set;}
        public int carga_id {get;set;}
        public DateTime fecha_registro {get;set;}
        public int usuario_registro {get;set;}
        public int tipo_id {get;set;}
    }
}