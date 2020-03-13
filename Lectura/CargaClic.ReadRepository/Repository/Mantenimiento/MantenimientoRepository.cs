using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CargaClic.Common;
using CargaClic.Data;

using CargaClic.Domain.Mantenimiento;
using CargaClic.ReadRepository.Contracts.Mantenimiento.Results;
using CargaClic.ReadRepository.Interface.Mantenimiento;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CargaClic.Handlers.Mantenimiento
{
    public class MantenimientoRepository : IMantenimientoRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;

        public MantenimientoRepository(DataContext context,IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        public IDbConnection Connection
        {   
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public async Task<IEnumerable<GetContratistaResult>> GetAllContratistas()
        {
            var parametros = new DynamicParameters();
            using (IDbConnection conn = Connection)
            {
                string sQuery = "[Mantenimiento].[pa_listar_contratistas]";
                conn.Open();
                var result = await conn.QueryAsync<GetContratistaResult>(sQuery
                            , parametros 
                            ,commandType:CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<IEnumerable<GetTrabajadoresResult>> GetAllTrabajadores(int contratista_id)
        {
              var parametros = new DynamicParameters();
            parametros.Add("contratista_id", dbType: DbType.String, direction: ParameterDirection.Input, value: contratista_id);
            using (IDbConnection conn = Connection)
            {
                string sQuery = "[Mantenimiento].[pa_listartrabajadores]";
                conn.Open();
                var result = await conn.QueryAsync<GetTrabajadoresResult>(sQuery
                            , parametros 
                            ,commandType:CommandType.StoredProcedure);
                return result;
            }
        }
    }
}