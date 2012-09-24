using System.Collections.Generic;
namespace SimChartMedicalOffice.Core.TempObject
{
    public class AptoList<T> where T:DocumentEntity
    {
        //public string Url { 
        //    get{
        //        return (typeof(T).Name + "/" + Guid.NewGuid());
        //    } 
            
        //}
        public Dictionary<string, T> Collections { get; set; }
    }    
}
