using System.Collections.Generic;
using System.Threading.Tasks;
using CargaClic.ReadRepository.Contracts.Seguimiento.Results;

namespace CargaClic.ReadRepository.Interface.Seguimiento
{
    public interface IBajaAlturaReadRepository
    {
         Task<IEnumerable<GetCargaMasivaResult>> GetAllCargasMasivas();
         Task<IEnumerable<GetSiteResult>> GetAllSitesMasivas(int carga_id);
         Task<IEnumerable<GetDocumentoResult>> GetAllDocumentos(int carga_id);
         Task<IEnumerable<GetDocumentoResult>> GetAllDocumentosSites(int site_id);
         Task<IEnumerable<GetIncidencia>> GetAllOrdenIncidencias(long OrdenTransporteId);
         Task<GetSiteResult> GetSite(int siteId);
         Task<IEnumerable<GetActasConformidadResult>> GetAllActasConformidad(int carga_id);
         Task<IEnumerable<GetCantidadSitiosEstado>> getCantidadSitios_Estados();
         Task<IEnumerable<GetSiteResult>> getSitesxDocumento(int documento_id);
         Task<IEnumerable<GetCantidadActas>>   GetCantidad_actas();
         Task<IEnumerable<GetPrespuestadoCosto>>   GetPresupuestadoCosto();
         Task<IEnumerable<GetPresupuestoPagoPendienteDto>>   GetPresupuestoPagoPendientes(int site_id);

         


    }
}