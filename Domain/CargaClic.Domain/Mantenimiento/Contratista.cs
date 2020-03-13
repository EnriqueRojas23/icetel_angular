using System.ComponentModel.DataAnnotations;
using CargaClic.Common;

namespace CargaClic.Domain.Mantenimiento
{
    public class Contratista : Entity
    {
        [Key]
        public int id {get;set;}
        public string razon_social {get;set;}
        public string nombre_corto {get;set;}
        public string ruc {get;set;}
        public string direccion {get;set;}
        public int estado_id {get;set;}
    }
}