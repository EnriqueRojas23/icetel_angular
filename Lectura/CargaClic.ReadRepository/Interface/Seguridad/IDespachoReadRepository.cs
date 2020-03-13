using System.Collections.Generic;
using System.Threading.Tasks;
using CargaClic.ReadRepository.Contracts.Seguridad.Results;

namespace CargaClic.ReadRepository.Interface.Seguridad
{
    public interface ISeguridadReadRepository
    {
         Task<IEnumerable<GetOptionsResult>> GetAllOptions(int user_id);
       
    }
}