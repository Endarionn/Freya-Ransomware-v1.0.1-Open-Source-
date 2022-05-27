using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Freya_Ransomware_v1._0._1
{
    class Http
    {
        public static byte[] Post(string uri, NameValueCollection pairs)
        {
            using (WebClient wc = new WebClient())
            {
                return wc.UploadValues(uri, pairs);
            }

        }

    }
}
