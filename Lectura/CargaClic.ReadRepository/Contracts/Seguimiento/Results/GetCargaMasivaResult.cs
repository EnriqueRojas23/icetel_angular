using System;

namespace CargaClic.ReadRepository.Contracts.Seguimiento.Results
{
    public class GetCargaMasivaResult
    {
        public int id {get;set;}
        public DateTime fecha_registro	{get;set; }
        public string nombre_archivo { get; set; }
        public string NombreCompleto	{get;set;}
        public string CantidadSitios	{get;set;}
        public string NombreEstado	{get;set;}
        public string documentacion	{get;set;}
        public string actas_conformidad	{get;set;}
        public decimal facturas	{get;set;}
        public decimal presupuesto	{get;set;}
        public decimal real{get;set;}
        public decimal bolsa {get;set;}
        public string tipo_proyecto {get;set;}
        

    }
}