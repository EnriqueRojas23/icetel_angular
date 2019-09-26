
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using CargaClic.API.Dtos.Seguimiento;
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
 
    

        public async Task<bool> RegisterSite(CargaMasivaForRegister cargaForRegister
        , IEnumerable<SiteForRegister> commandDetais)
        {
            Site site ;
            CargaMasiva cargaMasiva = new CargaMasiva(); 
            cargaMasiva.estado_id = 1;
            cargaMasiva.fecha_registro = DateTime.Now;
            cargaMasiva.usuario_id  = 1;    
            cargaMasiva.nombre_archivo = cargaForRegister.nombre_archivo;

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
                    site.estado_id = item.estado_id;
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