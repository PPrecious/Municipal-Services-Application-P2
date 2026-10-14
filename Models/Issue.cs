using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServicesApp.Models
{
    public class Issue
    {
        public string ReferenceNumber { get; set; }

        public string Location { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public string AttachmentPath { get; set; }

        public DateTime DateReported { get; set; }

        public Issue()
        {
            ReferenceNumber = string.Empty;
            Location = string.Empty;
            Category = string.Empty;
            Description = string.Empty;
            AttachmentPath = string.Empty;
            DateReported = DateTime.Now;
        }
    }
}
