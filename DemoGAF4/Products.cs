using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoGAF4
{
    class Products
    {
        public string Sugar { get; set; }
        public string Salt { get; set; }

        public Products(string _sugar, string _salt)
        {
            Sugar = _sugar;
            Salt = _salt;
        }

        public string GetAssociationsFromProducts(string _sugar, string _salt)
        {
            string sugarBool, saltBool;
            string d = "no";
            sugarBool = _sugar;
            saltBool = _salt;
            var result = sugarBool.Equals(saltBool);
           
            if (result) d.Equals("yes");
            return d;
        }

    }

   
}
