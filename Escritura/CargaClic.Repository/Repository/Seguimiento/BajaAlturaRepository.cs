
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CargaClic.API.Dtos.Seguimiento;
using CargaClic.Common;
using CargaClic.Data;
using CargaClic.Domain.Seguimiento;
using CargaClic.Repository.Interface.Seguimiento;
using Microsoft.Extensions.Configuration;

namespace CargaClic.Repository.Seguimiento
{
    public class BajaAlturaRepository  : IBajaAlturaRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;

        public BajaAlturaRepository()
        {
        }

        public BajaAlturaRepository(DataContext context,IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        public IDbConnection Connection
        {   
            get
            {
                var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                try
                {
                     connection.Open();
                     return connection;
                }
                catch (System.Exception)
                {
                    connection.Close();
                    connection.Dispose();
                    throw;
                }
            }
        }

        public async Task<bool> DeleteDocumento(int documentoId)
        {
             using(var transaction = _context.Database.BeginTransaction())
            {
               var documento =   _context.Documentos.Where(x=>x.id == documentoId).SingleOrDefault();
               
                try
                {
                     _context.Remove(documento);
                    await _context.SaveChangesAsync();
                    
                }
                catch(Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                transaction.Commit();   
                return true;
            }
        }

        public bool EliminarSite(long id)
        {
             var site = _context.Site.Where(x=>x.id == id).Single();
             site.activo = false;
             _context.SaveChanges();
             return true;
        

        }

        public async Task<bool> RegisterActaConformidad(ActaConformidadForRegister actaConformidadForRegister)
        {
             using(var transaction = _context.Database.BeginTransaction())
            {
                var actaConformidad = new ActaConformidad();
                actaConformidad.fecha_registro = actaConformidadForRegister.fecha_registro;
                actaConformidad.nombre = actaConformidadForRegister.nombre;
                actaConformidad.numero_acta = actaConformidadForRegister.numero_acta;
                actaConformidad.ruta = actaConformidadForRegister.ruta;
                actaConformidad.monto = actaConformidadForRegister.monto;
                actaConformidad.usuario_registro = actaConformidadForRegister.usuario_registro;
                actaConformidad.carga_id = actaConformidadForRegister.carga_id;
                actaConformidad.tipo_documento_id = actaConformidadForRegister.tipo_id;
               
                try
                {
                    await _context.AddRangeAsync(actaConformidad);
                    await _context.SaveChangesAsync();
                    
                }
                catch(Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                transaction.Commit();
                return true;
            }
        }

        public async Task<bool> RegisterDocumento(DocumentoForRegister documentoForRegister)
        {
            using(var transaction = _context.Database.BeginTransaction())
            {
                var documento = new Documento();
                documento.fecha_registro = documentoForRegister.fecha_registro;
                documento.nombre = documentoForRegister.nombre;
                documento.numero_documento = documentoForRegister.numero_documento;
                documento.ruta = documentoForRegister.ruta;
                documento.tipo_id = documentoForRegister.tipo_id;
                documento.usuario_registro = documento.usuario_registro;
                documento.carga_id = documentoForRegister.carga_id;
                documento.site_id = documentoForRegister.site_id;
               
                try
                {
                    await _context.AddRangeAsync(documento);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch(Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                //transaction.Commit();
                return true;
            }
        }

        public async Task<bool> RegisterSite(CargaMasivaForRegister cargaForRegister
        , IEnumerable<SiteForRegister> commandDetais)
        {
            string nombre_file;
            Site site ;
            CargaMasiva cargaMasiva = new CargaMasiva(); 
            cargaMasiva.estado_id =(Int32) Constantes.EstadoCarga.Creado;

            if(_context.CargaMasiva.ToList().Count<=0)
                nombre_file = "BA001-" + DateTime.Now.Year;
            else {
                var objMaximo =  _context.CargaMasiva.ToList().Max(x=>x.id);
               string[] max =  _context.CargaMasiva.Where(x=>x.id == objMaximo).Single().nombre_archivo.Split('-');
               string m = max[0].Substring(2,3) ;
               nombre_file = "BA" + (Convert.ToInt32(m) + 1).ToString().PadLeft(3,'0').ToString() + '-' + DateTime.Now.Year;
            }

           

            cargaMasiva.fecha_registro = DateTime.Now;
            cargaMasiva.usuario_id  = 1;    
            cargaMasiva.nombre_archivo = nombre_file ; 
            cargaMasiva.tipo_proyecto = cargaForRegister.tipo_proyecto;
              

            List<Site> sites = new List<Site>();

            using(var transaction = _context.Database.BeginTransaction())
            {

                await _context.AddAsync<CargaMasiva>(cargaMasiva);
                await _context.SaveChangesAsync();

                foreach (var item in commandDetais)
                {
                    site = new Site();
                    site.carga_id = cargaMasiva.id;
                    site.direccion = item.direccion;
                    site.distrito_id = item.distrito_id;
                    site.estado_id = (Int32) Constantes.Site.Creado;
                    site.fecha_registro = item.fecha_registro;
                    site.geom = item.geom;
                    site.latitud = item.latitud;
                    site.longitud = item.longitud;
                    site.nombre_site = item.nombre_site;
                    site.numero_suministro = item.numero_suministro;
                    site.presupuesto_costo = item.presupuesto_costo;
                    site.presupuesto_ingreso = item.presupuesto_ingreso;
                    site.real_costo = item.real_costo;
                    site.real_ingreso = item.real_ingreso;
                    site.usuario_id = item.usuario_id;
                    site.activo = true;
                    site.tipo_proyecto = cargaForRegister.tipo_proyecto;

                    sites.Add(site);
                }

                await _context.AddRangeAsync(sites);
                await _context.SaveChangesAsync();
                transaction.Commit();
                return true;
            }
        }

       
    }
}