using System;
using System.ComponentModel.DataAnnotations;
using CargaClic.Common;

namespace CargaClic.Domain.Seguimiento
{
    public class Site : Entity
    {
        [Key]
        public int id { get; set; }
        public int? carga_id { get; set; }
        public DateTime? fecha_registro { get; set; }
        public int? usuario_id { get; set; }
        public string nombre_site { get; set; }
        public decimal? latitud { get; set; }
        public decimal? longitud { get; set; }
        public int? distrito_id { get; set; }
        public string direccion { get; set; }
        public string numero_suministro { get; set; }
        public int? estado_id { get; set; }
        public decimal? presupuesto_ingreso { get; set; }
        public decimal? presupuesto_costo { get; set; }
        public decimal? real_ingreso { get; set; }
        public decimal? real_costo { get; set; }
        public string geom { get; set; }
        public int? contratista_id {get;set;}
        public DateTime? fecha_inicio_proyecto {get;set;}
        public DateTime? fecha_inicio_instalacion {get;set;}
        public DateTime? fecha_reembolso_factura_contratista  {get;set;}
        public int? archivo_presupuesto_concesionaria_id {get;set;}
        public int? archivo_factura_concesionaria_id {get;set;}
        public bool? reembolso_factura_contratista {get;set;}
        public bool? documentacion_aprobada {get;set;}

        public int? estado_poste {get;set;}
        public bool? estado_poste_enviado {get;set;}

        public DateTime? fecha_primer_pago {get;set;}
        public DateTime? fecha_segundo_pago {get;set;}
        public DateTime? fecha_adicional_pago {get;set;}
        public DateTime? fecha_asignacion { get; set; }

        public decimal? primer_pago_monto {get;set;}
        public decimal? segundo_pago_monto {get;set;}
        public decimal? adicional_pago_monto {get;set;}

        public bool? primer_pago_contratista {get;set;}
        public bool? segundo_pago_contratista {get;set;}
        public bool? adicional_pago {get;set;}

        public bool? activo {get;set;}
        public string tipo_proyecto {get;set;}

        public string pru_model { get; set; }
        public string cell_id { get; set; }
        public string nodeb_id { get; set; }
        public string nodeb_name { get; set; }
        public string azimuth { get; set; }
        public string downtilt { get; set; }
        public string e_downtilt { get; set; }
        public string m_downtilt { get; set; }
        public string ret { get; set; }
        public string ground_height { get; set; }
        public string antenna_type { get; set; }
        public string antenna_gain { get; set; }
        public string h_beamwidth { get; set; }
        public string v_beamwidth { get; set; }
        public int?   number_antennas { get; set; }


        public DateTime? fecha_forecast {get;set;}



    }
}