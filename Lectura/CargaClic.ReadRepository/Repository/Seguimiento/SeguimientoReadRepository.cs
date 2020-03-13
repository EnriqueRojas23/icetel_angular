using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CargaClic.Data;
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

      
        public async Task<IEnumerable<GetActasConformidadResult>> GetAllActasConformidad(int carga_id)
        {
            var parametros = new DynamicParameters();
            parametros.Add("carga_id", dbType: DbType.Int64, direction: ParameterDirection.Input, value: carga_id);

            using (IDbConnection conn = Connection)
            {
                string sQuery = "[Seguimiento].[pa_listar_actasconformidad]";
                conn.Open();
                var result = await conn.QueryAsync<GetActasConformidadResult>(sQuery,
                                                                    parametros
                                                                    ,commandType:CommandType.StoredProcedure
                  ); 
                return result;
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

        public async Task<IEnumerable<GetDocumentoResult>> GetAllDocumentos(int carga_id)
        {
            var parametros = new DynamicParameters();
            parametros.Add("carga_id", dbType: DbType.Int64, direction: ParameterDirection.Input, value: carga_id);

            using (IDbConnection conn = Connection)
            {
                string sQuery = "[Seguimiento].[pa_listar_documentos]";
                conn.Open();
                var result = await conn.QueryAsync<GetDocumentoResult>(sQuery,
                                                                    parametros
                                                                    ,commandType:CommandType.StoredProcedure
                  ); 
                return result;
            }
        }

        public async Task<IEnumerable<GetDocumentoResult>> GetAllDocumentosSites(int site_id)
        {
            var parametros = new DynamicParameters();
            parametros.Add("site_id", dbType: DbType.Int64, direction: ParameterDirection.Input, value: site_id);

            using (IDbConnection conn = Connection)
            {
                string sQuery = "[Seguimiento].[pa_listar_documentos_site]";
                conn.Open();
                var result = await conn.QueryAsync<GetDocumentoResult>(sQuery,
                                                                    parametros
                                                                    ,commandType:CommandType.StoredProcedure
                  ); 
                return result;
            }
        }

       public async Task<IEnumerable<GetIncidencia>> GetAllOrdenIncidencias(long OrdenTransporteId)
        {
             var parametros = new DynamicParameters();
             parametros.Add("site_id", dbType: DbType.Int64, direction: ParameterDirection.Input, value: OrdenTransporteId);

            using (IDbConnection conn = Connection)
            {
                string sQuery = "[seguimiento].[pa_listar_incidencias]";
                conn.Open();
                var result = await conn.QueryAsync<GetIncidencia>(sQuery,
                                                                    parametros
                                                                    ,commandType:CommandType.StoredProcedure
                  ); 
                return result;
            }
        }

        public async Task<IEnumerable<GetSiteResult>> GetAllSitesMasivas(int carga_id)
        {
            var parametros = new DynamicParameters();
            parametros.Add("carga_id", dbType: DbType.Int64, direction: ParameterDirection.Input, value: carga_id);

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

        public async Task<IEnumerable<GetCantidadSitiosEstado>> getCantidadSitios_Estados()
        {
            var parametros = new DynamicParameters();

            using (IDbConnection conn = Connection)
            {
                string sQuery = "[Seguimiento].[pa_CantidadSitios_Estados]";
                conn.Open();
                var result = await conn.QueryAsync<GetCantidadSitiosEstado>(sQuery,
                                                                    parametros
                                                                    ,commandType:CommandType.StoredProcedure
                  ); 
                return result;
            }
        }
        public async Task<IEnumerable<GetPrespuestadoCosto>> GetPresupuestadoCosto()
        {
            var parametros = new DynamicParameters();

            using (IDbConnection conn = Connection)
            {
                string sQuery = "[Seguimiento].[pa_listar_porcobrar]";
                conn.Open();
                var result = await conn.QueryAsync<GetPrespuestadoCosto>(sQuery,
                                                                    parametros
                                                                    ,commandType:CommandType.StoredProcedure
                  ); 
                return result;
            }
        }

        public async Task<IEnumerable<GetCantidadActas>> GetCantidad_actas()
        {
            var parametros = new DynamicParameters();

            using (IDbConnection conn = Connection)
            {
                string sQuery = "[Seguimiento].[pa_listar_cantactas]";
                conn.Open();
                var result = await conn.QueryAsync<GetCantidadActas>(sQuery,
                                                                    parametros
                                                                    ,commandType:CommandType.StoredProcedure
                  ); 
                return result;
            }
        }

        public async Task<IEnumerable<GetPresupuestoPagoPendienteDto>> GetPresupuestoPagoPendientes(int site_id)
        {
            var parametros = new DynamicParameters();
            parametros.Add("siteid", dbType: DbType.Int64, direction: ParameterDirection.Input, value: site_id);

            using (IDbConnection conn = Connection)
            {
                string sQuery = "[Seguimiento].[pa_obtener_presupuesto_pago_pendiente]";
                conn.Open();
                var result = await conn.QueryAsync<GetPresupuestoPagoPendienteDto>(sQuery,
                                                                    parametros
                                                                    ,commandType:CommandType.StoredProcedure
                  ); 
                return result;
            }
        }

        public async Task<GetSiteResult> GetSite(int siteId)
        {
              var parametros = new DynamicParameters();
              parametros.Add("site_id", dbType: DbType.Int64, direction: ParameterDirection.Input, value: siteId);

            using (IDbConnection conn = Connection)
            {
                string sQuery = "[Seguimiento].[pa_obtener_site]";
                conn.Open();
                var result = await conn.QueryAsync<GetSiteResult>(sQuery,
                                                                    parametros
                                                                    ,commandType:CommandType.StoredProcedure
                  ); 
                return result.SingleOrDefault();
            }
        }

        public async Task<IEnumerable<GetSiteResult>> getSitesxDocumento(int documento_id)
        {
            var parametros = new DynamicParameters();
            parametros.Add("@acta_id", dbType: DbType.Int32, direction: ParameterDirection.Input, value: documento_id);

            using (IDbConnection conn = Connection)
            {
                string sQuery = "[Seguimiento].[pa_listarsitesxacta]";
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