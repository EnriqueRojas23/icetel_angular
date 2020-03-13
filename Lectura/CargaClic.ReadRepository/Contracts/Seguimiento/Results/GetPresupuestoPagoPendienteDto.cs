using System;

namespace CargaClic.ReadRepository.Contracts.Seguimiento.Results
{
    public class GetPresupuestoPagoPendienteDto
    {
        public decimal presupuesto_costo	{get;set;}
        public decimal pagado	{get;set;}
        public decimal pendiente	{get;set;}
        public decimal presupuesto_ingreso	{get;set;}
        public decimal real_costo	{get;set;}
        public decimal real_ingreso	{get;set;}
        
    }
}