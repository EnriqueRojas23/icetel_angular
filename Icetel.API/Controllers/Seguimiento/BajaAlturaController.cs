
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CargaClic.API.Dtos.Seguimiento;
using CargaClic.ReadRepository.Interface.Seguimiento;
using CargaClic.Repository.Interface;
using CargaClic.Repository.Interface.Seguimiento;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

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

        public BajaAlturaController (
             IBajaAlturaRepository repoBajaAltura,
             IBajaAlturaReadRepository repoReadBajaAltura
            ,IMapper mapper
            ,IConfiguration config
            ) {
            _repoBajaAltura = repoBajaAltura;
            _repoReadBajaAltura = repoReadBajaAltura;
            _mapper = mapper;
             _config = config;
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

                             bool resp = await  registar_cargamasiva(total,fileName);
                         
                        }

                    }

                
                }




            }
            catch(Exception ex)
            {
                    return Ok();
            }
                return Ok();
             
        }
        public List<SiteForRegister> ObtenerEntidades(List<List<string>> data) 
        {
            var totales = new List<SiteForRegister>();
            var linea = new SiteForRegister();

            foreach (var item in data.Skip(1))
            {
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
                totales.Add(linea);
            }
            return totales;
        }


        private async Task<bool> registar_cargamasiva(List<SiteForRegister> items, string file_name)
        {
            var carga = new CargaMasivaForRegister();
            carga.estado_id = 1;
            carga.fecha_registro = DateTime.Now;
            carga.nombre_archivo = file_name;
            
            return await _repoBajaAltura.RegisterSite(carga, items);
        }

        [HttpGet("GetAllCargas")]
        public async Task<IActionResult> GetAllCargas(string sitio)
        { 
            var resp  = await _repoReadBajaAltura.GetAllCargasMasivas();
            return Ok (resp);
        }


        [HttpGet("GetAllSites")]
        public async Task<IActionResult> GetAllSites(string sitio)
        { 
            var resp  = await _repoReadBajaAltura.GetAllSitesMasivas();
            return Ok (resp);
        }




   


   
    }
}