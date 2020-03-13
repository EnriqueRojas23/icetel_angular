namespace CargaClic.Common
{
    public sealed class Constantes
    {
        public enum EstadoOrdenIngreso : int
        {
            Planeado = 4,
            Asignado = 5,
            Recibiendo = 6,
            PendienteAcomodo = 19,
            PendienteAlmacenamiento = 20,
            Terminado = 12,
        }
        
        public enum EstadoOrdenSalida : int
        {
            Creado = 21,
            Planificado = 22,
            Asignado = 23,
            Despachado = 24,
        }
        public enum EstadoWrk : int
        {
            Pendiente = 30,
            Asignado = 31,
            Iniciado = 32,
            Terminado = 33,
        }
        public enum EstadoPreliquidacion: int
        {
            Pendiente = 28,
            Facturado = 29,
        }
        public enum EstadoCarga : int
        {
            Creado = 4,
            EnProceso = 5,
            Cerrado = 6,
        }
        
        public enum StatusPoste : int
        {
   
            EnGestion   = 14,
            EnEjecucion   = 15,
            Instalado   = 16,
        }

        public enum Site : int
        {
   
            Creado   = 7,
            Asignado   = 8,
            ProyectoIniciado   = 9,
            PagoConcesionariaRealizado   = 10 ,
            InstalaciónIniciada   =  11 ,
            RevisiónDocFinalizada   = 12,
            Liquidado  = 13
        }
        public enum EstadoInventario : int
        {
            Disponible = 7,
            NoDisponible = 8,
            Merma = 18,
            Eliminado = 21
        }

        public enum Motivo : int
        {
            Devolucion = 146,
            ImportacionAduanera = 147,
        }
        public enum Movimiento : int
        {
            Entrada = 148,
            Salida = 149,
        }
    }
}