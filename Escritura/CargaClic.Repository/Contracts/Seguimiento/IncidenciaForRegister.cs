using System;


namespace CargaClic.API.Dtos.Seguimiento
{
    public class IncidenciaForRegister
    {
        public int site_id { get; set; }
        public int contratista_id { get; set; }
        public decimal presupuesto_ingreso {get;set;}
        public decimal presupuesto_costo {get;set;}

        

        
    }
    
}