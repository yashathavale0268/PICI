using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PICI.Models
{
    [DataContract]
    public class CustomerModel
    {
        [DataMember(Name="Custid")]
         public int Custid { get; set; }

        [DataMember(Name = "Cid")]
        public string Cid { get; set; }
        [DataMember(Name = "Name")]
        public string Name { get; set; }
        [DataMember(Name = "Custid")]
        public int CreatedBy { get; set; }
        [DataMember(Name = "UpdatedBy")]
        public int UpdatedBy { get; set; }

        [DataMember(Name = "Created_at")]
        public string Created_at { get; set; }
        [DataMember(Name = "Updated_at")]
        public string Updated_at { get; set; }
    }
}
