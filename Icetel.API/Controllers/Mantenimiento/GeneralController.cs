using System.Threading.Tasks;
using AutoMapper;
using CargaClic.Data.Interface;
using CargaClic.Domain.Mantenimiento;
using CargaClic.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CargaClic.ReadRepository.Interface.Mantenimiento;
using CargaClic.Repository.Contracts.Mantenimiento;
using CargaClic.Domain.Seguimiento;
using System;
using System.Linq;

namespace CargaClic.API.Controllers.Mantenimiento
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralController : ControllerBase
    {
        private readonly IMantenimientoRepository _repoMantenimiento;
        private readonly IRepository<ValorTabla> _repoValorTabla;
        private readonly IRepository<Contratista> _repoContratista;
        private readonly IRepository<Trabajador> _repoTrabajador;
        private readonly IRepository<Site> _repoSite;
        private readonly IMapper _mapper;

        public GeneralController(IRepository<Estado> repo
        ,IMantenimientoRepository repoMantenimiento
        ,IRepository<ValorTabla> repoValorTabla
        ,IRepository<Contratista> repoContratista
        ,IRepository<Trabajador> repoTrabajador
        ,IRepository<Site> repoSite
        ,IMapper mapper
        )
        {
            _repoContratista = repoContratista;
            _repoTrabajador = repoTrabajador;
            _repoSite = repoSite;
            _repoMantenimiento = repoMantenimiento;
            _repoValorTabla = repoValorTabla;
            _mapper = mapper;
        }
        [HttpGet("GetAllContratistas")]
        public async Task<IActionResult> GetAllContratistas()
        {
           var result = await _repoMantenimiento.GetAllContratistas();
           return Ok(result);
        }

        [HttpGet("GetAllTrabajadores")]
        public async Task<IActionResult> GetAllTrabajadores(int contratista_id)
        {
           var result = await _repoMantenimiento.GetAllTrabajadores(contratista_id);
           return Ok(result);
        }


        
        [HttpGet("GetContratista")]
        public async Task<IActionResult> GetContratista(int id)
        {
           var result = await _repoContratista.Get(x=>x.id == id);
           return Ok(result);
        }


        [HttpGet("GetAllValorTabla")]
        public async Task<IActionResult> GetAllValorTabla(int TablaId)
        {
           var result = await _repoValorTabla.GetAll(x=>x.TablaId == TablaId);
           
           return Ok(result);
        }
        [HttpPost("RegisterContratista")]
        public async Task<IActionResult> RegisterProveedor(ContratistaForRegister contratista)
        {
             var param = _mapper.Map<ContratistaForRegister, Contratista>(contratista);
            var createdProveedor = await _repoContratista.AddAsync(param);
            return Ok(createdProveedor);
        }

        [HttpPost("UpdateContratista")]
        public async Task<IActionResult> UpdateContratista(ContratistaForRegister contratistaForRegister)
        {
            var contratista = _repoContratista.Get(x=>x.id == contratistaForRegister.id).Result;
            contratista.direccion = contratistaForRegister.direccion;
            contratista.estado_id = contratistaForRegister.estado_id;
            contratista.nombre_corto = contratistaForRegister.nombre_corto;
            contratista.ruc = contratistaForRegister.ruc;
            

            var createdProveedor = await _repoContratista.SaveAll();
            return Ok(createdProveedor);
        }

        [HttpPost("deleteContratista")]
        public async Task<IActionResult> DeleteContratista(int ContratistaId)
        {
             var site =  _repoSite.GetAll(x=>x.contratista_id == ContratistaId).Result;
             if(site.ToList().Count > 0 )
             {
                 throw new ArgumentException("El Contratista actualmente tiene asignado proyecto(s)"); 
             }
            var param = await _repoContratista.Get(x=>x.id == ContratistaId);
            _repoContratista.Delete(param);
            return Ok(param);
        }
        [HttpPost("RegisterTrabajador")]
        public async Task<IActionResult> RegisterTrabajador(TrabajadorForRegister trabajador)
        {
             var param = _mapper.Map<TrabajadorForRegister, Trabajador>(trabajador);
            var createdTrabajador = await _repoTrabajador.AddAsync(param);
            return Ok(createdTrabajador);
        }
        [HttpGet("GetTrabajador")]
        public async Task<IActionResult> GetTrabajador(int id)
        {
           var result = await _repoTrabajador.Get(x=>x.id == id);
           return Ok(result);
        }
        [HttpPost("UpdateTrabajador")]
        public async Task<IActionResult> UpdateTrabajador(TrabajadorForRegister trabajadorForRegister)
        {
            var trabajador = _repoTrabajador.Get(x=>x.id == trabajadorForRegister.id).Result;
            trabajador.cargo = trabajadorForRegister.cargo;
            trabajador.telefono = trabajadorForRegister.telefono;
            trabajador.email = trabajadorForRegister.email;
            trabajador.dni = trabajadorForRegister.dni;
            

            var createdTrabajador = await _repoTrabajador.SaveAll();
            return Ok(createdTrabajador);
        }
        [HttpPost("deleteTrabajador")]
        public async Task<IActionResult> deleteTrabajador(int id)
        {
             //var site =  _repoTrabajador.GetAll(x=>x.contratista_id == id).Result;
            //  if(site.ToList().Count > 0 )
            //  {
            //      throw new ArgumentException("El Contratista actualmente tiene asignado proyecto(s)"); 
            //  }
            var param = await _repoTrabajador.Get(x=>x.id == id);
            _repoTrabajador.Delete(param);
            return Ok(param);
        }





    }
}