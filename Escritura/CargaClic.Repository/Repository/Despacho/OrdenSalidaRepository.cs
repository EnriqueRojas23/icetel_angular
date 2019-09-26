
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CargaClic.API.Dtos.Despacho;
using CargaClic.API.Dtos.Recepcion;
using CargaClic.Common;
using CargaClic.Data;
using CargaClic.Domain.Despacho;
using CargaClic.Domain.Inventario;
using CargaClic.Repository.Contracts.Despacho;
using CargaClic.Repository.Contracts.Inventario;
using CargaClic.Repository.Interface;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CargaClic.Repository
{
    public class OrdenSalidaRepository : IOrdenSalidaRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;

        public OrdenSalidaRepository(DataContext context,IConfiguration config)
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

       
        public async Task<long> MovimientoSalida(InventarioForStorage command)
        {
            KardexGeneral kardex ;
           
            using(var transaction = _context.Database.BeginTransaction())
            {
                var pckrk = await _context.Pckwrk.Where(x=>x.Id == command.Id).SingleOrDefaultAsync();
                var wrk = await _context.Wrk.Where(x=>x.Id == pckrk.WrkId).SingleOrDefaultAsync();
                var dominio = await _context.InventarioGeneral.Where(x=>x.Id == pckrk.InventarioId).Include(z=>z.InvLod).SingleOrDefaultAsync();
               
                try
                {
                       
                         pckrk.Confirmado = true;
                         dominio.InvLod.UbicacionId =   wrk.DestinoId.Value;
                         dominio.InvLod.UbicacionProxId = null;

                         pckrk.DestinoId = null;
                         pckrk.UbicacionId =  wrk.DestinoId.Value;
                         

                        /// Lógica para cerrar pedido
                        var detalles = await _context.Pckwrk.Where(x=>x.WrkId == wrk.Id).ToListAsync();
                                    
                        if(detalles.Where(x=>x.Confirmado == true).ToList().Count > 0)
                            wrk.FechaInicio = DateTime.Now;

                            foreach (var item in detalles)
                            {
                                if(item.Confirmado == false){
                                    wrk.EstadoId  = (Int32)Constantes.EstadoWrk.Iniciado;
                                }
                                else wrk.EstadoId = (Int32)Constantes.EstadoWrk.Terminado;
                            }

                         await _context.SaveChangesAsync();


                         //Registrar el movimiento en el kardex
                        kardex = new KardexGeneral();
                        kardex.Almacenado = false;
                        kardex.EstadoId = dominio.EstadoId;
                        kardex.FechaExpire = dominio.FechaExpire;
                        kardex.FechaManufactura = dominio.FechaManufactura;
                        kardex.FechaRegistro = DateTime.Now;
                        kardex.HuellaId = dominio.HuellaId;
                        kardex.LineaId = dominio.LineaId;
                        kardex.LodId = dominio.LodId;
                        kardex.LotNum = dominio.LotNum;
                        kardex.Movimiento = "S";
                        kardex.OrdenReciboId = dominio.OrdenReciboId;
                        kardex.Peso = dominio.Peso;
                        kardex.ProductoId = dominio.ProductoId;
                        kardex.PropietarioId = dominio.ClienteId;
                        kardex.ShipmentLine = pckrk.ShipmentLineId;
                        kardex.UntQty = pckrk.CantidadRetiro * -1 ; //dominio.UntQty * -1;
                        kardex.UsuarioIngreso = 1;
                        kardex.InventarioId = dominio.Id;
                        
                        _context.KardexGeneral.Add(kardex);

                         //Eliminar el inventario

                          if(pckrk.CantidadRetiro == pckrk.CantidadPallet)
                          {
                                var eliminar = await _context.InventarioGeneral.Where(x=>x.Id == dominio.Id).SingleAsync();
                                _context.InventarioGeneral.Remove(eliminar);
                          }
                          else
                          { 
                                dominio.Almacenado = true;
                                dominio.UntQty = pckrk.CantidadPallet - pckrk.CantidadRetiro;

                          }

                         await _context.SaveChangesAsync();


                        transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();  
                    throw ex; 
                }
                return command.Id;
            }

            
        }
       
        public async Task<long> RegisterOrdenSalida(OrdenSalidaForRegister ordenSalidaForRegister)
        {

            OrdenSalida ordensalida  ;
            
            
            ordensalida = new OrdenSalida();
            ordensalida.Activo = true;
            ordensalida.AlmacenId = 1;
            ordensalida.EquipoTransporteId = null;
            ordensalida.EstadoId = (Int32) Constantes.EstadoOrdenSalida.Creado;
            ordensalida.FechaRegistro = DateTime.Now;
            ordensalida.FechaRequerida = Convert.ToDateTime(ordenSalidaForRegister.FechaRequerida);
            ordensalida.GuiaRemision = ordenSalidaForRegister.GuiaRemision;
            ordensalida.HoraRequerida = ordenSalidaForRegister.HoraRequerida;
            ordensalida.NumOrden = ordenSalidaForRegister.NumOrden;
            ordensalida.Propietario = ordenSalidaForRegister.Propietario;
            ordensalida.PropietarioId = ordenSalidaForRegister.PropietarioId;
            ordensalida.ClienteId = ordenSalidaForRegister.ClienteId;
            ordensalida.UbicacionId = null;
            ordensalida.UsuarioRegistro = 1;
            ordensalida.DireccionId = ordenSalidaForRegister.IdDireccion;
            ordensalida.NumOrden = "";
            ordensalida.OrdenCompraCliente = ordenSalidaForRegister.OrdenCompraCliente;


            using(var transaction = _context.Database.BeginTransaction())
            {
        

                await _context.OrdenSalida.AddAsync(ordensalida);
                await _context.SaveChangesAsync();

                ordensalida.NumOrden = (ordensalida.Id).ToString().PadLeft(7,'0');
                await _context.SaveChangesAsync();

                transaction.Commit();
                return ordensalida.Id;
            }
        }

        public async Task<long> RegisterOrdenSalidaDetalle(OrdenSalidaDetalleForRegister command)
        {
            OrdenSalidaDetalle dominio ;
            string linea = "";
            //int cantidadTotal = 0;

            var detalles =  _context.OrdenSalidaDetalle.Where(x=>x.OrdenSalidaId == command.OrdenSalidaId);

            if(detalles.Count() == 0)
                linea = "0001";
            else {
                 linea = detalles.Max(x=>x.Linea).ToString();
                 linea = (Convert.ToInt32(linea) + 1).ToString().PadLeft(4,'0');
            }

            int total = 0;

            var inventario =  _context.InventarioGeneral.Where(x=>x.ProductoId ==  command.ProductoId).ToList();

            inventario.ForEach(x=> total = total + x.UntQty );

            if(total < command.Cantidad){
                throw new ArgumentException("No existen productos sufientes en el inventario");
            }
            
            total = 0;
            if(command.Lote != null){
                var existen = inventario.Where(x=>x.LotNum == command.Lote).ToList();
                existen.ForEach(x=> total = total + x.UntQty );

                if(total < command.Cantidad){
                    throw new ArgumentException("No existen productos sufientes en el inventario");
                }
            }
            


            dominio = new OrdenSalidaDetalle();
            dominio.Cantidad = command.Cantidad;
            dominio.Completo = command.Completo;
            dominio.EstadoId = command.EstadoID; 
            dominio.HuellaId = command.HuellaId;
            dominio.Linea = linea;
            dominio.Lote = command.Lote;
            dominio.OrdenSalidaId = command.OrdenSalidaId;
            dominio.ProductoId = command.ProductoId;
            dominio.UnidadMedidaId = command.UnidadMedidaId;

            
            using(var transaction = _context.Database.BeginTransaction())
            {
        

                await _context.OrdenSalidaDetalle.AddAsync(dominio);
                await _context.SaveChangesAsync();

                transaction.Commit();
                return dominio.Id;
            }


        }
        public async Task<long> assignmentOfDoor(AsignarPuertaSalida asignarPuertaSalida)
        {

            string[] prm = asignarPuertaSalida.ids.Split(',');
            Wrk wrk ;

            using(var transaction = _context.Database.BeginTransaction())
            {
                foreach (var item in prm)
                {
                    wrk = await _context.Wrk.SingleOrDefaultAsync(x=>x.Id == Convert.ToInt64(item));
                    wrk.DestinoId = asignarPuertaSalida.PuertaId;
                    var WrkDetail = await _context.Pckwrk.Where(x=>x.WrkId == wrk.Id).ToListAsync();
                    foreach (var detail in WrkDetail)
                    {
                          var dominio = await _context.InventarioGeneral.Where(x=>x.Id == detail.InventarioId).Include(z=>z.InvLod).SingleOrDefaultAsync();
                          dominio.InvLod.UbicacionProxId = wrk.DestinoId.Value;

                          detail.DestinoId =  wrk.DestinoId.Value;
                    }
                    await _context.SaveChangesAsync();
                }
                var ubicacionDb = await _context.Ubicacion.SingleOrDefaultAsync(x=>x.Id == asignarPuertaSalida.PuertaId);
                ubicacionDb.EstadoId =  9; //Lleno
                await _context.SaveChangesAsync();

                transaction.Commit();

                return ubicacionDb.Id;
            }
        }

        public async Task<long> assignmentOfUser(AsignarUsuarioSalida asignarPuertaSalida)
        {
            string[] prm = asignarPuertaSalida.ids.Split(',');
             Wrk wrk ;

            using(var transaction = _context.Database.BeginTransaction())
            {
                foreach (var item in prm)
                {
                    wrk = await _context.Wrk.SingleOrDefaultAsync(x=>x.Id == Convert.ToInt64(item));
                    
                    wrk.UsuarioId = asignarPuertaSalida.UserId;
                    wrk.FechaAsignacion = DateTime.Now;
                    wrk.EstadoId = (int) Constantes.EstadoWrk.Asignado;
                    await _context.SaveChangesAsync();


                    var detalles  =  await _context.Pckwrk.Where(x=>x.WrkId == wrk.Id).ToListAsync();
                    foreach (var item2 in detalles)
                    {
                        var orden =  await _context.OrdenSalida.Where(x=>x.Id == item2.OrdenSalidaId).SingleOrDefaultAsync();
                        orden.EstadoId = (Int16) Constantes.EstadoOrdenSalida.Asignado;
                         await _context.SaveChangesAsync();
                    }

                }
                transaction.Commit();

                return 1;
            }
        }

     

        
    }
}