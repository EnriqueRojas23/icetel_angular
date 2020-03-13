using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CargaClic.Common;

namespace CargaClic.Domain.Seguimiento
{
    public class ActasSite : Entity
    {
        [Key]
        public int actasite_id {get;set;}
        public int site_id { get; set; }
        public int acta_id { get; set; }
   
    }
}