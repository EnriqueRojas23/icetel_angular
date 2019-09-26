using System.Collections.Generic;
using System.Threading.Tasks;
using CargaClic.API.Dtos.Seguimiento;


namespace CargaClic.Repository.Interface.Seguimiento
{
    public interface IBajaAlturaRepository
    {
        Task<bool> RegisterSite(CargaMasivaForRegister cargaForRegister, IEnumerable<SiteForRegister>  siteForRegisterDto );
        
    }
}