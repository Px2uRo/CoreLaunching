﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using CoreLaunching.ObjectTemplates;
using System.IO;

namespace CoreLaunching.Auth
{
    public class Auther
    {
        public Auther()
        {

        }
        public void auth(string Account,string Password)
        {
            try
            {
                string Data = "{" + "\"agent\": {\"name\": \"Minecraft\",\"version\": 1},\"username\":\"" + Account + "\",\"password\":\"" + Password + "\",\"requestUser\":true" + "}";
                Console.WriteLine(Data);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://authserver.mojang.com/authenticate");
                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(Data);
                request.ContentLength = byteArray.Length;
                request.ContentType = "application/json";
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse response = request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                Console.WriteLine(responseFromServer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void auth(MSAInfo Info)
        {

        }
    }
    public class MSAInfo
    {

    }
}