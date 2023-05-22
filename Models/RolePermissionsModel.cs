﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PICI.Models
{
    public class RolePermissionsModel
    {
        public int Rpid { get; set; }

        public int Roleid { get; set; }
        public string RoleName { get; set; }
        public int Menuid { get; set; }
        public string MenuName { get; set; }
        public bool View { get; set; }
        public bool Add { get; set; }
        public bool Update { get; set; }

        public bool Delete { get; set; }

        public int totalrecords { get; set; }
    }
}
