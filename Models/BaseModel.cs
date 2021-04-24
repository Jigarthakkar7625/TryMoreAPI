using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace TryMoreAPI
{
    public class BaseModel
    {
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string EnteredBy { get; set; }
        public DateTime EnteredOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public bool success { get; set; } = false;
        public string message { get; set; }
        public int code { get; set; }
    }

}