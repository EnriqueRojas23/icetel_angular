using System;


namespace CargaClic.API.Dtos.Seguimiento
{
   public class GetIncidencia
    {
        
        public string incidencia {get;set;}
        public DateTime fecha_incidencia {get;set;}
        public string observacion {get;set;}
        public string usuario_registro {get;set;}
        
    }
}