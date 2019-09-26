using System.Collections.Generic;
using System.Threading.Tasks;
using CargaClic.Domain.Despacho;
using CargaClic.ReadRepository.Contracts.Despacho.Results;
using CargaClic.ReadRepository.Contracts.Seguimiento.Results;

namespace CargaClic.ReadRepository.Interface.Seguimiento
{
    public interface IBajaAlturaReadRepository
    {
         Task<IEnumerable<GetCargaMasivaResult>> GetAllCargasMasivas();
         Task<IEnumerable<GetSiteResult>> GetAllSitesMasivas();
       
    }
}