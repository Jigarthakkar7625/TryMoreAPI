using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TryMoreAPI.Models
{
    public class ResponseData
    {
        public bool success { get; set; } = false;
        public string message { get; set; } 
        public Object data { get; set; }
        public int code { get; set; } = 500 ;
    }
}