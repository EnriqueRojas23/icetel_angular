using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CargaClic.Data;
using CargaClic.ReadRepository.Contracts.Seguridad.Results;
using CargaClic.ReadRepository.Interface.Seguridad;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace CargaClic.ReadRepository.Repository.Seguridad
{
    public class SeguridadReadRepository : ISeguridadReadRepository
    {
            private readonly DataContext _context;
            private readonly IConfiguration _config;

            public SeguridadReadRepository(DataContext context,IConfiguration config)
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

      
       

        public async Task<IEnumerable<GetOptionsResult>> GetAllOptions(int user_id)
        {
            var parametros = new DynamicParameters();
            parametros.Add("user_id", dbType: DbType.Int64, direction: ParameterDirection.Input, value: user_id);

            using (IDbConnection conn = Connection)
            {
                string sQuery = "[seguridad].[pa_listar_options]";
                conn.Open();
                var result = await conn.QueryAsync<GetOptionsResult>(sQuery,
                                                                    parametros
                                                                    ,commandType:CommandType.StoredProcedure
                  ); 
                return result;
            }
        }
    }
}