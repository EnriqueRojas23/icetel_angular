using System;

namespace CargaClic.ReadRepository.Contracts.Seguimiento.Results
{
    public class GetSiteResult
    {
        public string id	{get;set;}
        public string carga_id	{get;set;}
        public DateTime fecha_registro	{get;set;}
        public string usuario_id	{get;set;}
        public string nombre_site	{get;set;}
        public string latitud	{get;set;}
        public string longitud	{get;set;}
        public string distrito_id	{get;set;}
        public string direccion	{get;set;}
        public string numero_suministro	{get;set;}
        public string estado_id	{get;set;}
        public string presupuesto_ingreso	{get;set;}
        public string presupuesto_costo	{get;set;}
        public string real_ingreso	{get;set;}
        public string real_costo{get;set;}
    }
}