using System;
using System.ComponentModel.DataAnnotations;
using CargaClic.Common;

namespace CargaClic.Domain.Seguimiento
{
    public class Incidencia : Entity
    {
        [Key]
        public Int64 id	{get;set;}
        public int site_id 	 {get;set;}
        public string descripcion	 {get;set;}
        public string observacion	 {get;set;}
        public DateTime fecha_incidencia {get;set;}
        public DateTime fecha_registro {get;set;}
        public int usuario_id {get;set;}
    }
}