using System;

namespace CargaClic.ReadRepository.Contracts.Seguimiento.Results
{
    public class GetActasConformidadResult
    {
        public int id	 {get;set;}
        public string ruta	 {get;set;}
        public string nombre	 {get;set;}
        public decimal  monto {get;set;}
        public string numero_acta {get;set;}
        public int carga_id {get;set;}
        public DateTime fecha_registro {get;set;}
        public int usuario_registro {get;set;}
        public string nombre_carga {get;set;}
        public string tipo_documento {get;set;}
    }
    
}