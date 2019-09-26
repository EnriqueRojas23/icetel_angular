using System;
using System.Threading.Tasks;
using CargaClic.API.Dtos.Recepcion;
using CargaClic.Domain.Mantenimiento;


namespace CargaClic.Repository.Interface
{
    public interface IFacturacionRepository
    {
        Task<Int64> GenerarPreliquidacion(PreliquidacionForRegister oPreliquidacionForRegister);
        Task<Int64> GenerarComprobante(ComprobanteForRegister oPreliquidacionForRegister);

    }
}