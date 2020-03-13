namespace CargaClic.ReadRepository.Contracts.Mantenimiento.Results
{
    public class GetTrabajadoresResult
    {
        public int Id	{get;set;}
        public string nombre_completo	{get;set;}
        public string cargo	{get;set;}
        public string  dni	{get;set;}
        public string telefono	{get;set;}
        public string  email	{get;set;}
        public int documento_id	{get;set;}
        public int UnidadMedidaId	{get;set;}
        public int contratista_id	{get;set;}
        
    }
}