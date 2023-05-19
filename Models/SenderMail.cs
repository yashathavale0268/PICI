using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PICI.Models
{
    public class SenderMail
    {
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Reciever { get; set; }
        public string Pid { get; set; }
        public string UpdaterName { get; set; }
        public string CreatorName { get; set; }

        public string SubjectName { get; set; }

        public DateTimeOffset Created_on { get; set; }
        public DateTimeOffset Updated_on { get; set; }
    }
}
