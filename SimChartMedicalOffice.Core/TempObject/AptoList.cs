using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using SimChartMedicalOffice.Common;
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
