using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace DAL.External_Services
{
    public class ServiceHelper<T>
    {
        public T MakeRequest(string requestUrl, object JSONRequest, string method)
        {

            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
          
                //string sb = JsonConvert.SerializeObject(JSONRequest);
                //request.Method = method;               
                //Byte[] bt = Encoding.UTF8.GetBytes(sb);
                //Stream st = request.GetRequestStream();
                //st.Write(bt, 0, bt.Length);
                //st.Close();


                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {

                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));

                   
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    string strsb = sr.ReadToEnd();
                    T objResponse = JsonConvert.DeserializeObject<T>(strsb);


                    return objResponse;
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}
