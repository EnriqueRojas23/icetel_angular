using System;
using System.ComponentModel.DataAnnotations;

namespace CargaClic.Repository.Contracts.Mantenimiento
{
    public class TrabajadorForRegister
    {
        
        public int? id {get;set;}
        [Required]
        public string nombre_completo {get;set;}
        [Required]
        public string cargo {get;set;}
        
        public string dni {get;set;}
        public string telefono {get;set;}
        public string email {get;set;}
        public int? documento_id {get;set;}

        [Required]
        public int contratista_id {get;set;}


        
    }
    
}