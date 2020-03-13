using System;


namespace CargaClic.API.Dtos.Seguimiento
{
    public class DocumentoForRegister
    {

        public int id	 {get;set;}
        public string ruta	 {get;set;}
        public string nombre	 {get;set;}
        public int tipo_id {get;set;}
        public string numero_documento {get;set;}
        public int? carga_id {get;set;}
        public DateTime fecha_registro {get;set;}
        public int usuario_registro {get;set;}
        public long? site_id {get;set;}

    }
}