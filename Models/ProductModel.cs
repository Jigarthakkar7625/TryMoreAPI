using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.ComponentModel;

namespace TryMoreAPI
{
  
    public partial class tblProduct : BaseModel
    {
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        
        public string SellerName { get; set; }
        public string FileExt { get; set; }
        public List<tblBrand> lstBrands { get; set; }
        public List<tblCategory> lstCategories { get; set; }
    }

   
}