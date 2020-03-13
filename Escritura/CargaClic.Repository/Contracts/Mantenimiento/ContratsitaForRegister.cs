using System;
using System.ComponentModel.DataAnnotations;

namespace CargaClic.Repository.Contracts.Mantenimiento
{
    public class ContratistaForRegister
    {
        
        public int? id {get;set;}
        [Required]
        public string razon_social {get;set;}
        [Required]
        public string nombre_corto {get;set;}
        [Required]
        public string direccion {get;set;}
        [Required]
        public string ruc {get;set;}

        [Required]
        public int estado_id {get;set;}


        
    }
    
}