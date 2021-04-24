using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using TryMoreAPI.Models;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Net.Http;
using System.IO;
using System.Xml.Serialization;

namespace TryMoreAPI.DataAccess
{
    public class BookingDL
    {
        SqlConnection conn = new
                        SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        Common objCommon = new Common();


        public String ObjectToXMLGeneric<T>(T filter)
        {

            string xml = null;
            using (StringWriter sw = new StringWriter())
            {

                XmlSerializer xs = new XmlSerializer(typeof(T));
                xs.Serialize(sw, filter);
                try
                {
                    xml = sw.ToString();

                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return xml;
        }
    }
}