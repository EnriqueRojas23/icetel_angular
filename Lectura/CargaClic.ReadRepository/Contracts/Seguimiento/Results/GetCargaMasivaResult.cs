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
        public string facturas	{get;set;}
        public string presupuesto	{get;set;}
        public string real{get;set;}

    }
}