using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using CargaClic.Data;
using CargaClic.ReadRepository.Contracts.Despacho.Results;
using CargaClic.ReadRepository.Contracts.Seguimiento.Results;
using CargaClic.ReadRepository.Interface.Seguimiento;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace CargaClic.ReadRepository.Repository.Despacho
{
    public class BajaAlturaReadRepository : IBajaAlturaReadRepository
    {
            private readonly DataContext _context;
            private readonly IConfiguration _config;

            public BajaAlturaReadRepository(DataContext context,IConfiguration config)
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

        public async Task<IEnumerable<GetCargaMasivaResult>> GetAllCargasMasivas()
        {
             var parametros = new DynamicParameters();
            // parametros.Add("PropietarioId", dbType: DbType.Int64, direction: ParameterDirection.Input, value: PropietarioId);
            // parametros.Add("EstadoId", dbType: DbType.Int64, direction: ParameterDirection.Input, value: EstadoId);
            // parametros.Add("DaysAgo", dbType: DbType.Int64, direction: ParameterDirection.Input, value: DaysAgo);

            using (IDbConnection conn = Connection)
            {
                string sQuery = "[Seguimiento].[pa_listar_cargasmasivas]";
                conn.Open();
                var result = await conn.QueryAsync<GetCargaMasivaResult>(sQuery,
                                                                    parametros
                                                                    ,commandType:CommandType.StoredProcedure
                  ); 
                return result;
            }
        }

        public async Task<IEnumerable<GetSiteResult>> GetAllSitesMasivas()
        {
              var parametros = new DynamicParameters();
            // parametros.Add("PropietarioId", dbType: DbType.Int64, direction: ParameterDirection.Input, value: PropietarioId);
            // parametros.Add("EstadoId", dbType: DbType.Int64, direction: ParameterDirection.Input, value: EstadoId);
            // parametros.Add("DaysAgo", dbType: DbType.Int64, direction: ParameterDirection.Input, value: DaysAgo);

            using (IDbConnection conn = Connection)
            {
                string sQuery = "[Seguimiento].[pa_listar_sites]";
                conn.Open();
                var result = await conn.QueryAsync<GetSiteResult>(sQuery,
                                                                    parametros
                                                                    ,commandType:CommandType.StoredProcedure
                  ); 
                return result;
            }
        }
    }
}