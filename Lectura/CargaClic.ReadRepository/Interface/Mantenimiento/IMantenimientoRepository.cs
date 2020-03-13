using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CargaClic.Common;
using CargaClic.Data;
using CargaClic.Domain.Mantenimiento;
using CargaClic.ReadRepository.Contracts.Mantenimiento.Results;


namespace CargaClic.ReadRepository.Interface.Mantenimiento
{
    public interface IMantenimientoRepository
    {
         Task<IEnumerable<GetContratistaResult>> GetAllContratistas();
         Task<IEnumerable<GetTrabajadoresResult>> GetAllTrabajadores(int contratista_id);


    }
}