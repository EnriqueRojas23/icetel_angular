
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CargaClic.API.Dtos.Seguimiento;
using CargaClic.Common;
using CargaClic.Data.Interface;
using CargaClic.Domain.Facturacion;
using CargaClic.Domain.Mantenimiento;
using CargaClic.Domain.Seguimiento;
using CargaClic.ReadRepository.Contracts.Seguimiento.Results;
using CargaClic.ReadRepository.Interface.Seguimiento;
using CargaClic.Repository.Interface;
using CargaClic.Repository.Interface.Seguimiento;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace CargaClic.API.Controllers.Seguimiento
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BajaAlturaController : ControllerBase
    {
        
        private readonly IBajaAlturaRepository _repoBajaAltura;
        private readonly IBajaAlturaReadRepository _repoReadBajaAltura;
        private readonly IConfiguration _config;    
        private readonly IMapper _mapper;
        private readonly IRepository<Site> _repo_site;
        private readonly IRepository<Documento> _repo_Documento;
        private readonly IRepository<Incidencia> _repo_Incidencia;
        private readonly IRepository<Estado> _repo_Estado;
        private readonly IRepository<Contratista> _repo_Contratista;
        private readonly IRepository<ActaConformidad> _repo_ActaConformidad;
        private readonly IRepository<ActasSite> _repo_ActasSite;

        public BajaAlturaController (
             IBajaAlturaRepository repoBajaAltura,
             IBajaAlturaReadRepository repoReadBajaAltura
            ,IMapper mapper
            ,IConfiguration config
            ,IRepository<Site> repo_site
            ,IRepository<Documento> repo_documento
            ,IRepository<Incidencia> repo_incidencia
            ,IRepository<Estado> repo_estado
            ,IRepository<ActasSite> repo_ActasSite
            ,IRepository<Contratista> repo_contratista
            ,IRepository<ActaConformidad> repo_ActaConformidad
            ) {
            _repoBajaAltura = repoBajaAltura;
            _repoReadBajaAltura = repoReadBajaAltura;
            _mapper = mapper;
             _config = config;
             _repo_site = repo_site;
            _repo_Documento = repo_documento;
            _repo_Incidencia = repo_incidencia;
            _repo_Estado = repo_estado;
            _repo_Contratista = repo_contratista;
            _repo_ActaConformidad = repo_ActaConformidad;
            _repo_ActasSite = repo_ActasSite;
        }
        
        [HttpPost("UploadFile")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFile(int usrid)
        {
            List<List<string>> valores = new List<List<string>>();

            try
            {
                var ruta =  _config.GetSection("AppSettings:Uploads").Value;

                var file = Request.Form.Files[0];
                var idOrden = usrid;//Request.Form["idOrden"];

                string folderName = idOrden.ToString();
                string webRootPath = ruta ;
                string newPath = Path.Combine(webRootPath, folderName);

                byte[] fileData = null;
                using (var binaryReader = new BinaryReader(Request.Form.Files[0].OpenReadStream()))
                {
                    fileData = binaryReader.ReadBytes(Request.Form.Files[0].ContentDisposition.Length);
                }

                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                if (file.Length > 0)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);

                    var checkextension = Path.GetExtension(fileName).ToLower();
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {

                        file.CopyTo(stream);
                        
                    }
                  
                    using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fullPath, false))
                    {
                     
                        
                        WorkbookPart workbookPart = doc.WorkbookPart;
                        Sheets thesheetcollection = workbookPart.Workbook.GetFirstChild<Sheets>();
                        StringBuilder excelResult = new StringBuilder();
                            //using for each loop to get the sheet from the sheetcollection  
                        foreach (Sheet thesheet in thesheetcollection)
                        {
                            Worksheet theWorksheet = ((WorksheetPart)workbookPart.GetPartById(thesheet.Id)).Worksheet;

                            SheetData thesheetdata = (SheetData)theWorksheet.GetFirstChild<SheetData>();

                            #region read_excel 
                            foreach (Row thecurrentrow in thesheetdata)
                            {
                                List<string> linea = new List<string>();
                                int inicio = 0;
                                foreach (Cell thecurrentcell in thecurrentrow)
                                {
                                    
                                    //statement to take the integer value  
                                    string currentcellvalue = string.Empty;
                                    if (thecurrentcell.DataType != null)
                                    {
                                        if (thecurrentcell.DataType == CellValues.SharedString)
                                        {
                                            int id;
                                            if (int.TryParse(thecurrentcell.InnerText, out id))
                                            {
                                                var item = 
                                                workbookPart.SharedStringTablePart.SharedStringTable
                                                .Elements<SharedStringItem>().ElementAt(id);

                                                if (item.Text != null)
                                                {
                                                    
                                                    excelResult.Append(item.Text.Text + " ");
                                                    linea.Add(item.Text.Text);
                                                }
                                                else if (item.InnerText != null)
                                                {
                                                    currentcellvalue = item.InnerText;
                                                }
                                                else if (item.InnerXml != null)
                                                {
                                                    currentcellvalue = item.InnerXml;
                                                }
                                              
                                            }
                                           
                                        }
                                    }
                                    else
                                    {   
                                         
                                                excelResult.Append(thecurrentcell.InnerText + " ");
                                                linea.Add(thecurrentcell.InnerText);
                                    }
                                    inicio ++;
                                }
                                valores.Add(linea);
                                // Obtener Entiedades
                               

                            }
                            #endregion

                             var total = ObtenerEntidades(valores);
                            string proyecto = total[0].proyecto;
                             bool resp = await  registar_cargamasiva(total,fileName,proyecto);
                         
                        }

                    }

                
                }




            }
            catch(Exception ex)
            {
                 return Ok(ex.Message);
                throw ex;
            }
                return Ok();
             
        }
        public List<SiteForRegister> ObtenerEntidades(List<List<string>> data) 
        {
            var totales = new List<SiteForRegister>();
            var linea = new SiteForRegister();

            foreach (var item in data.Skip(1))
            {
                // if(item[0] ==  "0102246_LM_Samaniego_Paz")
                // {
                //     int i = 1;
                // }
                linea = new SiteForRegister();
                linea.nombre_site = item[0];
                linea.latitud =  Decimal.Parse(item[1], System.Globalization.NumberStyles.Float);
                linea.longitud = Decimal.Parse(item[2], System.Globalization.NumberStyles.Float);
                linea.direccion = item[3];
               // linea.distrito_id = item[4] ; Rescargar el distrito 
                linea.estado_id =1 ;
                linea.fecha_registro = DateTime.Now;
                
                if(item[6] != "")
                linea.presupuesto_costo = Decimal.Parse(item[6], System.Globalization.NumberStyles.Float);

                if(item[7] != null)
                linea.presupuesto_ingreso  = Decimal.Parse(item[7], System.Globalization.NumberStyles.Float);


                if(item[8] != null)
                linea.proyecto  =item[8];

                totales.Add(linea);
            }
            return totales;
        }


        private async Task<bool> registar_cargamasiva(List<SiteForRegister> items, string file_name, string tipo_proyecto)
        {
            var carga = new CargaMasivaForRegister();
            carga.estado_id = 1;
            carga.fecha_registro = DateTime.Now;
            carga.nombre_archivo = file_name;
            carga.tipo_proyecto = tipo_proyecto;
            
            return await _repoBajaAltura.RegisterSite(carga, items);
        }
        [HttpDelete("DeleteActaConformidad")]
        public async Task<ActionResult> DeleteActaConformidad(int id)
        {
           
             try
             {
                var conformidad = await _repo_ActaConformidad.Get(x=>x.id == id);
                _repo_ActaConformidad.Delete(conformidad);    
             }
             catch (System.Exception)
             {
                 
                 throw new ArgumentException("Tiene sites asociados");
                
             }
             
            return Ok ();
        }

        [HttpGet("GetAllCargas")]
        public async Task<IActionResult> GetAllCargas(string sitio)
        { 
            var resp  = await _repoReadBajaAltura.GetAllCargasMasivas();
            return Ok (resp);
        }
        [HttpGet("GetAllSites")]
        public async Task<IActionResult> GetAllSites(int carga_id)
        { 
            var resp  = await _repoReadBajaAltura.GetAllSitesMasivas(carga_id);
            return Ok (resp);
        }
        
        [HttpGet("GetSite")]
        public async Task<IActionResult> GetSite(int siteId)
        { 
            var resp  = await _repoReadBajaAltura.GetSite(siteId);
            return Ok (resp);
        }

        [HttpGet("GetAllDocumentos")]
        public async Task<IActionResult> GetAllDocumentos(int carga_id)
        { 
            var resp  = await _repoReadBajaAltura.GetAllDocumentos(carga_id);
            return Ok (resp);
        }
        
        [HttpGet("getAllDocumentosSites")]
        public async Task<IActionResult> getAllDocumentosSites(int siteId, string tipoId)
        { 
            var resp  = await _repoReadBajaAltura.GetAllDocumentosSites(siteId);

            List<GetDocumentoResult> final  = new List<GetDocumentoResult>();

             string[] prm = tipoId.Split(',');
             foreach (var item in prm)
             {
                final.AddRange(resp.Where(x=>x.tipo_id == int.Parse(item)).ToList());
             }
             
            return Ok (final);
        }
        
        [HttpDelete("deleteDocumento")]
        public async Task<IActionResult> deleteDocumento(int documentoId)
        { 
            var resp  = await _repoBajaAltura.DeleteDocumento(documentoId);
            return Ok (resp);
        }

        [HttpGet("GetAllActasConformidad")]
        public async Task<IActionResult> GetAllActasConformidad(int carga_id)
        { 
            var resp  = await _repoReadBajaAltura.GetAllActasConformidad(carga_id);
            return Ok (resp);
        }

        [HttpPost("UploadDocumento")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadDocumento(int usuario_id, int carga_id, int tipo_id
        , string numero_documento )
        {
      
            List<List<string>> valores = new List<List<string>>();
            try
            {
                var ruta =  _config.GetSection("AppSettings:UploadsDocuments").Value;
                
                var file = Request.Form.Files[0];
                string name = file.FileName.Split('.')[0];



                string folderName = carga_id.ToString();

                string webRootPath = ruta ;
                string newPath = Path.Combine(webRootPath, folderName);

                
                  if(name.Length > 26){
                    throw new ArgumentException("El nombre del file excede al límite de caracteres permitidos (24 caracteres)");
                }


                var documento = new DocumentoForRegister{
                    carga_id = carga_id
                    , tipo_id = tipo_id
                    , numero_documento = numero_documento
                    , ruta = newPath
                    , nombre = name + "." + file.FileName.Split('.')[1]
                    , fecha_registro = DateTime.Now
                    , usuario_registro = 1
                 };


                byte[] fileData = null;

                using (var binaryReader = new BinaryReader(Request.Form.Files[0].OpenReadStream()))
                    fileData = binaryReader.ReadBytes(Request.Form.Files[0].ContentDisposition.Length);

                if (!Directory.Exists(newPath))
                    Directory.CreateDirectory(newPath);

                if (file.Length > 0)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);

                    var checkextension = Path.GetExtension(fileName).ToLower();
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }


                }

                var resp  = await _repoBajaAltura.RegisterDocumento(documento);
                return Ok(resp);

            }
            catch(Exception ex)
            {
                  return Ok(ex.Message);
                   throw ex;
            }
                
             
        }
        [HttpPost("UploadActaConformidad")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadActaConformidad(int usuario_id, int carga_id
        ,decimal  monto , string numero_acta, int tipo_id )
        {
      
            List<List<string>> valores = new List<List<string>>();
            try
            {
                var ruta =  _config.GetSection("AppSettings:UploadsDocuments").Value;
                
                var file = Request.Form.Files[0];
                string name = file.FileName.Split('.')[0];



                string folderName = carga_id.ToString();

                string webRootPath = ruta ;
                string newPath = Path.Combine(webRootPath, folderName);

                if(name.Length > 26){
                    throw new ArgumentException("El nombre del file excede al límite de caracteres permitidos (24 caracteres)");
                }

                var acta = new ActaConformidadForRegister{
                     carga_id = carga_id
                    , monto = monto
                    , ruta = newPath
                    , nombre = name + "." + file.FileName.Split('.')[1]
                    , fecha_registro = DateTime.Now
                    , usuario_registro = 1
                    , numero_acta = numero_acta 
                    , tipo_id = tipo_id
                 };


                byte[] fileData = null;

                using (var binaryReader = new BinaryReader(Request.Form.Files[0].OpenReadStream()))
                    fileData = binaryReader.ReadBytes(Request.Form.Files[0].ContentDisposition.Length);

                if (!Directory.Exists(newPath))
                    Directory.CreateDirectory(newPath);

                if (file.Length > 0)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);

                    var checkextension = Path.GetExtension(fileName).ToLower();
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }


                }

                var resp  = await _repoBajaAltura.RegisterActaConformidad(acta);
                return Ok(resp);

            }
            catch(Exception ex)
            {
                return Ok(ex.Message);
                throw ex;
            }
                
             
        }
        [HttpPost("EliminarSite")]
        public IActionResult EliminarSite(int id)
        {
            var resp = _repoBajaAltura.EliminarSite(id);
            return Ok(resp);
        }
        [HttpPost("ActualizarSite")]
        public IActionResult UpdateSite(SiteForUpdate siteForAssignment)
        {
            var site =  _repo_site.Get(x=>x.id == siteForAssignment.id).Result;

            if(site.tipo_proyecto != "Nodos")
            {
                site.presupuesto_costo = siteForAssignment.presupuesto_costo;
                site.presupuesto_ingreso = siteForAssignment.presupuesto_ingreso;
                site.nombre_site = siteForAssignment.nombre_site;
                site.latitud = siteForAssignment.latitud;
                site.longitud = siteForAssignment.longitud;
            }
            else {
                site.presupuesto_costo = siteForAssignment.presupuesto_costo;
                site.presupuesto_ingreso = siteForAssignment.presupuesto_ingreso;
                site.nombre_site = siteForAssignment.nombre_site;
                site.latitud = siteForAssignment.latitud;
                site.longitud = siteForAssignment.longitud;

                
                site.pru_model= siteForAssignment.pru_model;
                site.cell_id = siteForAssignment.cell_id;
                site.nodeb_id= siteForAssignment.nodeb_id;
                site.nodeb_name = siteForAssignment.nodeb_name;
                site.azimuth  = siteForAssignment.azimuth; 
                site.downtilt  = siteForAssignment.downtilt;
                site.e_downtilt  = siteForAssignment.e_downtilt;
                site.m_downtilt  = siteForAssignment.m_downtilt;
                site.ret  = siteForAssignment.ret;
                site.ground_height  = siteForAssignment.ground_height;
                site.antenna_type  = siteForAssignment.antenna_type;
                site.antenna_gain  = siteForAssignment.antenna_gain;
                site.h_beamwidth  = siteForAssignment.h_beamwidth;
                site.v_beamwidth  = siteForAssignment.v_beamwidth;
                site.number_antennas = siteForAssignment.number_antennas;

            }



            var resp = _repo_site.SaveAll();
            return Ok();
        }
        [HttpPost("AsignarContratista")]
        public async Task<IActionResult> AsignarContratista(SiteForAssignment siteForAssignment)
        {
            var site =  _repo_site.Get(x=>x.id == siteForAssignment.site_id).Result;
            site.contratista_id = siteForAssignment.contratista_id;
            site.estado_id = (int) Constantes.Site.Asignado;
            site.presupuesto_costo = siteForAssignment.presupuesto_costo;
            site.presupuesto_ingreso = siteForAssignment.presupuesto_ingreso;
            site.fecha_asignacion = siteForAssignment.fecha_asignacion;

           
            
           var contratista =   _repo_Contratista.Get(x=>x.id == siteForAssignment.contratista_id).Result.razon_social;
           var estado =   _repo_Estado.Get(x=>x.Id == site.estado_id).Result.NombreEstado;

            var incidencia = new Incidencia();

            incidencia.descripcion = estado;
            

            incidencia.observacion = "Se ha asignado al contratista " +  contratista + " el " + site.fecha_asignacion.Value.ToShortDateString();

            incidencia.site_id = siteForAssignment.site_id;
            incidencia.fecha_incidencia = DateTime.Now;
            incidencia.fecha_registro = DateTime.Now;
            incidencia.usuario_id = 1;



            await _repo_Incidencia.AddAsync(incidencia);
            await _repo_Incidencia.SaveAll();


            return Ok();
        
        }
        [HttpPost("AsignarContratistaNodos")]
        public async Task<IActionResult> AsignarContratistaNodos(SiteForAssignment siteForAssignment)
        {
            var site =  _repo_site.Get(x=>x.id == siteForAssignment.site_id).Result;
            site.contratista_id = siteForAssignment.contratista_id;
            site.estado_id = (int) Constantes.Site.Asignado;
            site.presupuesto_costo = siteForAssignment.presupuesto_costo;
            site.presupuesto_ingreso = siteForAssignment.presupuesto_ingreso;
            site.fecha_asignacion = siteForAssignment.fecha_asignacion;
            //await _repo_site.SaveAll();
            if(site.tipo_proyecto == "Nodos")
            {
                site.fecha_forecast = siteForAssignment.fecha_forecast;
            }


            
           var contratista =   _repo_Contratista.Get(x=>x.id == siteForAssignment.contratista_id).Result.razon_social;
           var estado =   _repo_Estado.Get(x=>x.Id == site.estado_id).Result.NombreEstado;

            var incidencia = new Incidencia();

            incidencia.descripcion = estado;
            
            incidencia.observacion = "Se ha asignado al contratista " +  contratista + " el " + site.fecha_asignacion.Value.ToShortDateString() + " & " +
            "Fecha Forecast " + site.fecha_forecast.Value.ToShortDateString();

            incidencia.site_id = siteForAssignment.site_id;
            incidencia.fecha_incidencia = DateTime.Now;
            incidencia.fecha_registro = DateTime.Now;
            incidencia.usuario_id = 1;
            await _repo_Incidencia.AddAsync(incidencia);
            await _repo_Incidencia.SaveAll();


            return Ok();
        
        }
        [HttpPost("IniciarProyecto")]
        public async Task<IActionResult> IniciarProyecto(SiteForStartProyect siteForAssignment)
        {
            var site =  _repo_site.Get(x=>x.id == siteForAssignment.site_id).Result;
            site.estado_id = (int) Constantes.Site.ProyectoIniciado;
            site.fecha_inicio_proyecto = siteForAssignment.fecha_incio_proyecto; 

            //await _repo_site.SaveAll();
            var estado =   _repo_Estado.Get(x=>x.Id == site.estado_id).Result.NombreEstado;

            var incidencia = new Incidencia();

            incidencia.descripcion = estado;
            incidencia.observacion = "Se ha iniciado el proyecto el  " +  site.fecha_inicio_proyecto.Value.ToShortDateString();
            incidencia.site_id = siteForAssignment.site_id;
            incidencia.fecha_incidencia = DateTime.Now;
            incidencia.fecha_registro = DateTime.Now;
            incidencia.usuario_id = 1;
            await _repo_Incidencia.AddAsync(incidencia);
            await _repo_Incidencia.SaveAll();

            return Ok();
        
        }
        [HttpPost("PagoConcesionaria")]
        public async Task<IActionResult> PagoConcesionaria(SiteForPagoConcesionaria siteForAssignment)
        {
            var site =  _repo_site.Get(x=>x.id == siteForAssignment.site_id).Result;
            site.estado_id = (int) Constantes.Site.PagoConcesionariaRealizado;
            site.reembolso_factura_contratista = siteForAssignment.reembolso_factura_contratista;
            site.fecha_reembolso_factura_contratista = siteForAssignment.fecha_reembolso_factura_contratista;

            var incidencia = new Incidencia();
            var estado =   _repo_Estado.Get(x=>x.Id == site.estado_id).Result.NombreEstado;

            incidencia.descripcion = estado;
            incidencia.observacion = "Se ha realizado el pago a la concesionaria el  " +  site.fecha_reembolso_factura_contratista.Value.ToShortDateString();;
            incidencia.site_id = siteForAssignment.site_id;
            incidencia.fecha_incidencia = DateTime.Now;
            incidencia.fecha_registro = DateTime.Now;
            incidencia.usuario_id = 1;
            
            await _repo_Incidencia.AddAsync(incidencia);
            await _repo_Incidencia.SaveAll();

            return Ok();
        
        }
        [HttpPost("PagoPrimero")]
        public async Task<IActionResult> PagoPrimero(PagoPrimeroForRegister pagoPrimeroForRegister)
        {
            var site =  _repo_site.Get(x=>x.id == pagoPrimeroForRegister.site_id).Result;

            site.fecha_primer_pago =   pagoPrimeroForRegister.fecha_primer_pago;
            site.primer_pago_monto = pagoPrimeroForRegister.primer_pago_monto;


            if(site.primer_pago_contratista != null)
            {
                if(site.primer_pago_contratista.Value)
                {
                    var incidencia = new Incidencia();
                    var estado =   _repo_Estado.Get(x=>x.Id == site.estado_id).Result.NombreEstado;
                    incidencia.descripcion =  site.primer_pago_monto + " Nuevos Soles.";
                    incidencia.observacion = "Se ha actualizado el pago por el inicio del proyecto (30%)  " +  site.fecha_primer_pago.Value.ToShortDateString();;
                    incidencia.site_id = pagoPrimeroForRegister.site_id;
                    incidencia.fecha_incidencia = DateTime.Now;
                    incidencia.fecha_registro = DateTime.Now;
                    incidencia.usuario_id = 1;
                    await _repo_Incidencia.AddAsync(incidencia);   
                }
                else
                {
                    if(site.fecha_primer_pago != null && site.primer_pago_monto != null)
                    {
                        
                        site.primer_pago_contratista = true;
                            
                        var incidencia = new Incidencia();
                        var estado =   _repo_Estado.Get(x=>x.Id == site.estado_id).Result.NombreEstado;
                        incidencia.descripcion =  site.primer_pago_monto + " Nuevos Soles.";
                        incidencia.observacion = "Se ha realizado el pago por el inicio del proyecto (30%)  " +  site.fecha_primer_pago.Value.ToShortDateString();;
                        incidencia.site_id = pagoPrimeroForRegister.site_id;
                        incidencia.fecha_incidencia = DateTime.Now;
                        incidencia.fecha_registro = DateTime.Now;
                        incidencia.usuario_id = 1;
                        await _repo_Incidencia.AddAsync(incidencia);
                    }
                    else
                    {
                       site.primer_pago_contratista = false; 
                    }
                }
            }
            else 
            {
                    if(site.fecha_primer_pago != null && site.primer_pago_monto != null)
                    {
                        
                        site.primer_pago_contratista = true;
                            
                        var incidencia = new Incidencia();
                        var estado =   _repo_Estado.Get(x=>x.Id == site.estado_id).Result.NombreEstado;
                        incidencia.descripcion =  site.primer_pago_monto + " Nuevos Soles.";
                        incidencia.observacion = "Se ha realizado el pago por el inicio del proyecto (30%)  " +  site.fecha_primer_pago.Value.ToShortDateString();;
                        incidencia.site_id = pagoPrimeroForRegister.site_id;
                        incidencia.fecha_incidencia = DateTime.Now;
                        incidencia.fecha_registro = DateTime.Now;
                        incidencia.usuario_id = 1;
                        await _repo_Incidencia.AddAsync(incidencia);
                    }
                    else 
                    {
                        site.primer_pago_contratista = false;
                    }
            }
            
            await _repo_Incidencia.SaveAll();
            return Ok();
        
        }
         [HttpPost("PagoSegundo")]
        public async Task<IActionResult> PagoSegundo(PagoSegundoForRegister pagoSegundoForRegister)
        {
            var site =  _repo_site.Get(x=>x.id == pagoSegundoForRegister.site_id).Result;

            site.fecha_segundo_pago =   pagoSegundoForRegister.fecha_segundo_pago;
            site.segundo_pago_monto = pagoSegundoForRegister.segundo_pago_monto;


            if(site.segundo_pago_contratista != null)
            {
                if(site.segundo_pago_contratista.Value)
                {
                    var incidencia = new Incidencia();
                    var estado =   _repo_Estado.Get(x=>x.Id == site.estado_id).Result.NombreEstado;
                    incidencia.descripcion =  site.primer_pago_monto + " Nuevos Soles.";
                    incidencia.observacion = "Se ha actualizado el pago por el fin del proyecto (70%)  " +  site.fecha_primer_pago.Value.ToShortDateString();;
                    incidencia.site_id = pagoSegundoForRegister.site_id;
                    incidencia.fecha_incidencia = DateTime.Now;
                    incidencia.fecha_registro = DateTime.Now;
                    incidencia.usuario_id = 1;
                    await _repo_Incidencia.AddAsync(incidencia);   
                }
                else
                {
                    if(site.fecha_segundo_pago != null && site.segundo_pago_monto != null)
                    {
                        
                        site.primer_pago_contratista = true;
                            
                        var incidencia = new Incidencia();
                        var estado =   _repo_Estado.Get(x=>x.Id == site.estado_id).Result.NombreEstado;
                        incidencia.descripcion =  site.primer_pago_monto + " Nuevos Soles.";
                        incidencia.observacion = "Se ha realizado el pago por el fin del proyecto (70%)  " +  site.fecha_segundo_pago.Value.ToShortDateString();;
                        incidencia.site_id = pagoSegundoForRegister.site_id;
                        incidencia.fecha_incidencia = DateTime.Now;
                        incidencia.fecha_registro = DateTime.Now;
                        incidencia.usuario_id = 1;
                        await _repo_Incidencia.AddAsync(incidencia);
                    }
                    else
                    {
                       site.segundo_pago_contratista = false; 
                    }
                }
            }
            else 
            {
                    if(site.fecha_segundo_pago != null && site.segundo_pago_monto != null)
                    {
                        
                        site.segundo_pago_contratista = true;
                            
                        var incidencia = new Incidencia();
                        var estado =   _repo_Estado.Get(x=>x.Id == site.estado_id).Result.NombreEstado;
                        incidencia.descripcion =  site.primer_pago_monto + " Nuevos Soles.";
                        incidencia.observacion = "Se ha realizado el pago por el fin del proyecto (70%)  " +  site.fecha_segundo_pago.Value.ToShortDateString();;
                        incidencia.site_id = pagoSegundoForRegister.site_id;
                        incidencia.fecha_incidencia = DateTime.Now;
                        incidencia.fecha_registro = DateTime.Now;
                        incidencia.usuario_id = 1;
                        await _repo_Incidencia.AddAsync(incidencia);
                    }
                    else 
                    {
                        site.segundo_pago_contratista = false;
                    }
            }
            
            await _repo_Incidencia.SaveAll();
            return Ok();
        
        }
        [HttpPost("PagoAdicional")]
        public async Task<IActionResult> PagoAdicional(PagoAdicionalForRegister pagoAdicionalForRegister)
        {
            var site =  _repo_site.Get(x=>x.id == pagoAdicionalForRegister.site_id).Result;

            site.fecha_adicional_pago =   pagoAdicionalForRegister.fecha_adicional_pago;
            site.adicional_pago_monto = pagoAdicionalForRegister.adicional_pago_monto;
            


            if(site.adicional_pago != null)
            {
                if(site.adicional_pago.Value)
                {
                    var incidencia = new Incidencia();
                    var estado =   _repo_Estado.Get(x=>x.Id == site.estado_id).Result.NombreEstado;
                    incidencia.descripcion =  site.adicional_pago_monto + " Nuevos Soles.";
                    incidencia.observacion = "Se ha actualizado el pago adicional al proyecto  " +  site.fecha_adicional_pago.Value.ToShortDateString();;
                    incidencia.site_id = pagoAdicionalForRegister.site_id;
                    incidencia.fecha_incidencia = DateTime.Now;
                    incidencia.fecha_registro = DateTime.Now;
                    incidencia.usuario_id = 1;
                    await _repo_Incidencia.AddAsync(incidencia);   
                }
                else
                {
                    
                        site.adicional_pago = true;
                            
                        var incidencia = new Incidencia();
                        var estado =   _repo_Estado.Get(x=>x.Id == site.estado_id).Result.NombreEstado;
                        incidencia.descripcion =  site.adicional_pago_monto + " Nuevos Soles.";
                        incidencia.observacion = "Se ha realizado el pago pago adicional  del proyecto   " +  site.fecha_adicional_pago.Value.ToShortDateString();;
                        incidencia.site_id = pagoAdicionalForRegister.site_id;
                        incidencia.fecha_incidencia = DateTime.Now;
                        incidencia.fecha_registro = DateTime.Now;
                        incidencia.usuario_id = 1;
                        await _repo_Incidencia.AddAsync(incidencia);
                    
                  
                }
            }
            else 
            {
                      site.adicional_pago = true;
                            
                        var incidencia = new Incidencia();
                        var estado =   _repo_Estado.Get(x=>x.Id == site.estado_id).Result.NombreEstado;
                        incidencia.descripcion =  site.adicional_pago_monto + " Nuevos Soles.";
                        incidencia.observacion = "Se ha realizado el pago pago adicional  del proyecto   " +  site.fecha_adicional_pago.Value.ToShortDateString();;
                        incidencia.site_id = pagoAdicionalForRegister.site_id;
                        incidencia.fecha_incidencia = DateTime.Now;
                        incidencia.fecha_registro = DateTime.Now;
                        incidencia.usuario_id = 1;
                        await _repo_Incidencia.AddAsync(incidencia);
                    
            }
            
            await _repo_Incidencia.SaveAll();
            return Ok();
        
        }
        [HttpPost("IniciarInstalacion")]
        public async Task<IActionResult> IniciarInstalacion(SiteForIniciarInstalacion siteForAssignment)
        {
            var site =  _repo_site.Get(x=>x.id == siteForAssignment.site_id).Result;
            site.estado_id = (int) Constantes.Site.InstalaciónIniciada;
            site.fecha_inicio_instalacion = siteForAssignment.fecha_inicio_instalacion;

            var incidencia = new Incidencia();
            var estado =   _repo_Estado.Get(x=>x.Id == site.estado_id).Result.NombreEstado;

            incidencia.descripcion = estado;
            incidencia.observacion = "Se iniciado la instalación  " +  site.fecha_inicio_instalacion.Value.ToShortDateString();
            incidencia.site_id = siteForAssignment.site_id;
            incidencia.fecha_incidencia = DateTime.Now;
            incidencia.fecha_registro = DateTime.Now;
            incidencia.usuario_id = 1;
            await _repo_Incidencia.AddAsync(incidencia);
            await _repo_Incidencia.SaveAll();

            return Ok();
        
        }
        [HttpPost("RevisionDocumentaria")]
        public async Task<IActionResult> RevisionDocumentaria(SiteForRevisionDocumentaria siteForAssignment)
        {
            var site =  _repo_site.Get(x=>x.id == siteForAssignment.site_id).Result;
            site.estado_id = (int) Constantes.Site.RevisiónDocFinalizada;
            site.documentacion_aprobada = siteForAssignment.documentacion_aprobada;

               var incidencia = new Incidencia();
            var estado =   _repo_Estado.Get(x=>x.Id == site.estado_id).Result.NombreEstado;

            incidencia.descripcion = estado;
            incidencia.observacion = "Se ha finalizado la revisión documentaria " +  DateTime.Now.ToShortDateString();
            incidencia.site_id = siteForAssignment.site_id;
            incidencia.fecha_incidencia = DateTime.Now;
            incidencia.fecha_registro = DateTime.Now;
            incidencia.usuario_id = 1;
            await _repo_Incidencia.AddAsync(incidencia);
            await _repo_Incidencia.SaveAll();

            return Ok();
        
        }
        [HttpGet("GetAllIncidencias")]
        public async Task<IActionResult> GetAllIncidencias(long OrdenTransporteId)
        {
             var result  = await _repoReadBajaAltura.GetAllOrdenIncidencias(OrdenTransporteId);
             return Ok(result);
        }
        
        [HttpPost("StatusPoste")]
        public async Task<IActionResult> StatusPoste(StatusPoste siteForStatusPoste)
        {
            var site =  _repo_site.Get(x=>x.id == siteForStatusPoste.site_id).Result;
            site.estado_poste  = siteForStatusPoste.estado_poste;
            site.estado_poste_enviado  = siteForStatusPoste.estado_poste_enviado;

            await _repo_site.SaveAll();
            return Ok();
        
        }

        [HttpPost("Liquidacion")]
        public async Task<IActionResult> Liquidacion(SiteForLiquidar siteForAssignment)
        {
            var site =  _repo_site.Get(x=>x.id == siteForAssignment.site_id).Result;
            site.estado_id = (int) Constantes.Site.Liquidado;
            site.real_costo = siteForAssignment.real_costo;
            site.real_ingreso = siteForAssignment.real_ingreso;

            var incidencia = new Incidencia();
            var estado =   _repo_Estado.Get(x=>x.Id == site.estado_id).Result.NombreEstado;

            incidencia.descripcion = estado;
            incidencia.observacion = "Se ha finalizado la liquidación " +  DateTime.Now.ToShortDateString();
            incidencia.site_id = siteForAssignment.site_id;
            incidencia.fecha_incidencia = DateTime.Now;
            incidencia.fecha_registro = DateTime.Now;
            incidencia.usuario_id = 1;
            await _repo_Incidencia.AddAsync(incidencia);
            await _repo_Incidencia.SaveAll();


            return Ok();
        
        }

        // [HttpPost("RegisterIncidencia")]
        // public async Task<IActionResult> RegisterIncidencia(IncidenciaForRegister incidenciaForRegister)
        // {
        //    // _repo_Estado.Get(x=>x.Id == incidenciaForRegister.estado_id )
            
          
        //     return Ok();
        
        // }



        [HttpGet("DownloadArchivo")]
        public FileResult DownloadArchivo(long documentoId)
        {
            string filePath =   _config.GetSection("AppSettings:UploadsDocuments").Value;
            
            
             var documento = _repo_Documento.Get(x=>x.id == documentoId).Result;

            IFileProvider provider = new PhysicalFileProvider(documento.ruta );
            IFileInfo fileInfo = provider.GetFileInfo(documento.nombre);
            var readStream = fileInfo.CreateReadStream();
            //var mimeType = "application/vnd.ms-excel";
            return File(readStream,GetContentType(documento.ruta + "//" + documento.nombre) , documento.nombre);
            
        }
        private string GetContentType(string path)
        {
           var provider = new FileExtensionContentTypeProvider();
           string contentType;
           if(!provider.TryGetContentType(path, out contentType))
           {
               contentType = "application/octet-stream";
           }
           return contentType;
        }

        [HttpPost("UploadDocumentoSite")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadDocumentoSite(long siteId,int idtipo)
        {
      
            List<List<string>> valores = new List<List<string>>();
            try
            {
                var ruta =  _config.GetSection("AppSettings:UploadsDocuments").Value;
                
                var file = Request.Form.Files[0];
                string name = file.FileName.Split('.')[0];



                string folderName = siteId.ToString();

                string webRootPath = ruta ;
                string newPath = Path.Combine(webRootPath, folderName);

                
                if(name.Length > 26){
                    throw new ArgumentException("El nombre del file excede al límite de caracteres permitidos (24 caracteres)");
                }
                // name = name.Substring(0,25).ToString() ;
                


                var documento = new DocumentoForRegister{
                     site_id = siteId
                    , tipo_id = idtipo
                    , numero_documento = ""
                    , ruta = newPath
                    , nombre = name + "." + file.FileName.Split('.')[1]
                    , fecha_registro = DateTime.Now
                    , usuario_registro = 1
                 };


                byte[] fileData = null;


                using (var binaryReader = new BinaryReader(Request.Form.Files[0].OpenReadStream())){
                    fileData = binaryReader.ReadBytes(Request.Form.Files[0].ContentDisposition.Length);
                
                    if (!Directory.Exists(newPath))
                        Directory.CreateDirectory(newPath);

                    if (file.Length > 0)
                    {
                        string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        string fullPath = Path.Combine(newPath, fileName);

                        var checkextension = Path.GetExtension(fileName).ToLower();
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }


                    }
                }

                var resp  = await _repoBajaAltura.RegisterDocumento(documento);
                return Ok(resp);
            }
            catch(Exception ex)
            {
                   return Ok(ex.Message);
                   throw ex;
            }
                
             
        }
        [HttpGet("GetCantidadSitios_Estados")]
        public async Task<IActionResult> GetCantidadSitios_Estados()
        { 
             var result  = await _repoReadBajaAltura.getCantidadSitios_Estados();
             return Ok(result);
        }
        [HttpGet("GetCantidad_actas")]
        public async Task<IActionResult> GetCantidad_actas()
        { 
             var result  = await _repoReadBajaAltura.GetCantidad_actas();
             return Ok(result);
        }
        [HttpGet("GetPresupuestadoCosto")]
        public async Task<IActionResult> GetPresupuestadoCosto()
        { 
             var result  = await _repoReadBajaAltura.GetPresupuestadoCosto();
             return Ok(result);
        }
        [HttpGet("GetPresupuestoPagoPendientes")]
        public async Task<IActionResult> GetPresupuestoPagoPendientes(int site_id)
        { 
             var result  = await _repoReadBajaAltura.GetPresupuestoPagoPendientes(site_id);
             return Ok(result);
        }
        [HttpGet("GetSitesxDocumento")]
        public async Task<IActionResult> GetSitesxDocumento(int acta_id)
        { 
             var result  = await _repoReadBajaAltura.getSitesxDocumento(acta_id);
             return Ok(result);
        }
        
        [HttpPost("VincularSiteDocumento")]
        public async Task<IActionResult> VincularSiteDocumento(IEnumerable<SiteForDocumento> siteForDocumento
        ,int acta_id)
        {
            Site site;
            ActasSite actasSite;

            
            var todo = _repo_ActasSite.GetAll(x=>x.acta_id == acta_id).Result;
            foreach (var item in todo)
            {
                _repo_ActasSite.Delete(item);
            }
            //_repo_ActasSite.DeleteAll(todo);


            foreach (var item in siteForDocumento)
            {
                 site =   await _repo_site.Get(x=>x.nombre_site == item.nombre_site);
                 
                 actasSite = new ActasSite();
                 actasSite.acta_id = acta_id;
                 actasSite.site_id = site.id;
                 await _repo_ActasSite.AddAsync(actasSite);

            }
             var result  = await _repoReadBajaAltura.getSitesxDocumento(acta_id);
             return Ok(result);  
        }

        [HttpGet("DownloadPlantilla")]
        public FileResult DownloadPlantilla()
        {
            string filePath =   _config.GetSection("AppSettings:UploadsDocuments").Value;
            
            IFileProvider provider = new PhysicalFileProvider(filePath );
            IFileInfo fileInfo = provider.GetFileInfo("Plantilla_Carga_Sites(MULTI_PROYECTO).xlsx");
            var readStream = fileInfo.CreateReadStream();
            //var mimeType = "application/vnd.ms-excel";
            return File(readStream,GetContentType(filePath + "//" + "Plantilla_Carga_Sites(MULTI_PROYECTO).xlsx") , "Plantilla_Carga_Sites(MULTI_PROYECTO).xlsx");
            
        }
    }
}