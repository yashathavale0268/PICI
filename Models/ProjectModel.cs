﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PICI.Models
{
    [DataContract]
    public class ProjectModel
    {
        [DataMember(Name = "Pmid")]
        public Int64 Pmid { get; set; }

        [DataMember(Name = "Pid")]
        public string Pid { get; set; }

        [DataMember(Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [DataMember(Name = "UpdatedDate")]
        public DateTime UpdatedDate { get; set; }

        [DataMember(Name = "CID")]
        public Int64 CID { get; set; }

        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "Techstack")]
        public int Techstack { get; set; }

        [DataMember(Name = "PMName")]
        public Int64 PMName { get; set; }

        [DataMember(Name = "Type")]
        public int Type { get; set; }

        [DataMember(Name = "CreatedBy")]
        public int CreatedBy { get; set; }

        [DataMember(Name = "UpdatedBy")]
        public int UpdatedBy { get; set; }

        [DataMember(Name = "TechStackName")]
        public string TechStackName { get; set; }

    }
}
