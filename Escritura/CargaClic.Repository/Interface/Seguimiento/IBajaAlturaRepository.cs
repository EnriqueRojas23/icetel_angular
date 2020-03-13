using System.Collections.Generic;
using System.Threading.Tasks;
using CargaClic.API.Dtos.Seguimiento;


namespace CargaClic.Repository.Interface.Seguimiento
{
    public interface IBajaAlturaRepository
    {
        Task<bool> RegisterSite(CargaMasivaForRegister cargaForRegister, IEnumerable<SiteForRegister>  siteForRegisterDto );
        Task<bool> RegisterDocumento(DocumentoForRegister documentoForRegister );
        Task<bool> RegisterActaConformidad(ActaConformidadForRegister actaConformidadForRegister );
        bool EliminarSite(long id );
        Task<bool> DeleteDocumento(int documentoId);
        
    }
}