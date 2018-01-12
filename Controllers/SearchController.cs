namespace processMsg.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using RestSharp;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    public class SearchController : Controller
    {
        public IActionResult Index() => View();
 
        [HttpPost]
        public string Download()
        {
            string rec = "";
            string searchtype = "星系";
            string downloadnum = "20";
            string savedir = "";
            int reqtimes = 1;
            int reqtimessta = 0;

            if(Request.Body != null)
            {
                StreamReader reader = new StreamReader(Request.Body);
                rec = reader.ReadToEnd();
                string[] arr = rec.Split(';');
                searchtype = arr[0];
                downloadnum = arr[1];
                savedir = arr[2];

                reqtimes = Convert.ToInt32(downloadnum)/20;
            }
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                {"0", "7"},{"1", "d"},{"2", "g"},{"3", "j"},{"4", "m"},{"5", "o"},{"6", "r"},{"7", "u"},
                {"8", "1"},{"9", "4"},{"a", "0"},{"b", "8"},{"c", "5"},{"d", "2"},{"e", "v"},{"f", "s"},
                {"g", "n"},{"h", "k"},{"i", "h"},{"j", "e"},{"k", "b"},{"l", "9"},{"m", "6"},{"n", "3"},
                {"o", "w"},{"p", "t"},{"q", "q"},{"r", "p"},{"s", "l"},{"t", "i"},{"u", "f"},{"v", "c"},
                {"w", "a"}
            };
            Dictionary<string, string> dicmap = new Dictionary<string, string>
            {{"_z2C$q", ":"}, {"_z&e3B", "."}, {"AzdH3F", "/"}};

            var client = new RestClient("http://image.baidu.com");

            int count = 0;

            while (reqtimessta<reqtimes)
            {
                reqtimessta++;
                var request = new RestRequest("search/acjson");
                request.AddParameter("tn", "resultjson_com");
                request.AddParameter("ipn", "rj");
                request.AddParameter("ct", "201326592");
                request.AddParameter("is", "");
                request.AddParameter("fp", "result");
                request.AddParameter("queryWord", searchtype);
                request.AddParameter("cl", "2");
                request.AddParameter("lm", "-1");
                request.AddParameter("ie", "utf-8");
                request.AddParameter("oe", "utf-8");
                request.AddParameter("adpicid", "");
                request.AddParameter("st", "-1");
                request.AddParameter("z", "");
                request.AddParameter("ic", "0");
                request.AddParameter("word", searchtype);
                request.AddParameter("s", "");
                request.AddParameter("se", "");
                request.AddParameter("tab", "");
                request.AddParameter("width", "");
                request.AddParameter("height", "");
                request.AddParameter("face", "0");
                request.AddParameter("istype", "2");
                request.AddParameter("qc", "");
                request.AddParameter("nc", "1");
                request.AddParameter("fr", "");
                Console.WriteLine((reqtimessta*20).ToString());
                request.AddParameter("pn", (reqtimessta*20).ToString());
                request.AddParameter("rn", "20");
                request.AddParameter("gsm", "3c");

                IRestResponse response = client.Execute(request);
                var content = response.Content;
                JObject obj = (JObject)JsonConvert.DeserializeObject(content);
                var data = obj["data"].ToString();
                JArray imgdata = (JArray)JsonConvert.DeserializeObject(data);

                if(!Directory.Exists(savedir))
                {
                    DirectoryInfo di = Directory.CreateDirectory(savedir);
                }

                string str = "";
                
                foreach(var item in imgdata)
                {
                    str += item.ToString();
                    count++;
                    string imgurl = "";

                    var objURL = item["objURL"];

                    if(objURL != null)
                    {
                        string objURLstr = objURL.ToString(); 

                        foreach(var itemmap in dicmap)
                        {
                            objURLstr = objURLstr.Replace(itemmap.Key, itemmap.Value);
                        }

                        StringBuilder newstr = new StringBuilder();

                        for(int i=0; i<objURLstr.Length; i++)
                        {
                            if(Regex.Match(objURLstr[i].ToString(),@"^[a-w\d]$").Success)
                            {
                            newstr.Append(objURLstr[i].ToString().Replace(objURLstr[i].ToString(), dic[objURLstr[i].ToString()]));
                            }
                            else if(objURLstr[i].ToString() == ":")
                            {
                                newstr.Append(":");
                            }
                            else if(objURLstr[i].ToString() == "/")
                            {
                                newstr.Append("/");
                            }
                            else if(objURLstr[i].ToString() == ".")
                            {
                                newstr.Append(".");
                            };
                        }
                        imgurl = newstr.ToString();
                        Console.WriteLine(imgurl);
                    }

                    else if(item["replaceUrl"] == null)
                    {
                            continue;
                    }

                    Match matchjpg = Regex.Match(imgurl, @".jpg$");
                    Match matchjepg = Regex.Match(imgurl, @".jpeg$");
                    Match matchpng = Regex.Match(imgurl, @".png$");

                    if(matchjpg.Success)
                    {                     
                        var clientimg = new RestClient(imgurl);
                        var requestjpg = new RestRequest();
                        requestjpg.AddHeader("Referer", "http://image.baidu.com");
                        IRestResponse resjpg =  clientimg.Execute(requestjpg);
                        if(resjpg.StatusCode.ToString() == "OK")
                        {
                            string local = savedir + "\\" + searchtype + count + ".jpg";
                            MemoryStream streamjpg = new MemoryStream(resjpg.RawBytes);
                            using(FileStream filestream = new FileStream(local, FileMode.Create))
                            {
                                streamjpg.CopyTo(filestream);
                            }
                        }
                        else
                        {
                            if(item["replaceUrl"] != null)
                            {
                                var replace  = item["replaceUrl"].ToString();
                                JArray replaceUrlArr = (JArray)JsonConvert.DeserializeObject(replace);
                                foreach(var itemr in replaceUrlArr)
                                {
                                    imgurl = itemr["ObjURL"].ToString();
                                    Console.WriteLine(imgurl);
                                    clientimg = new RestClient(imgurl);
                                    requestjpg = new RestRequest();
                                    requestjpg.AddHeader("Referer", "http://image.baidu.com");
                                    resjpg =  clientimg.Execute(requestjpg);
                                    if(resjpg.StatusCode.ToString() == "OK")
                                    {
                                        string local = savedir + "\\" + searchtype + count + ".jpg";
                                        MemoryStream streamjpg = new MemoryStream(resjpg.RawBytes);
                                        using(FileStream filestream = new FileStream(local, FileMode.Create))
                                        {
                                            streamjpg.CopyTo(filestream);
                                        }
                                        break;
                                    }
                                }
                            }
                        } 
                    }
                    else if(matchjepg.Success)
                    {
                        var clientimg = new RestClient(imgurl);
                        var requestjpeg = new RestRequest();
                        requestjpeg.AddHeader("Referer", "http://image.baidu.com");
                        IRestResponse resjpeg =  clientimg.Execute(requestjpeg); 
                        if(resjpeg.StatusCode.ToString() == "OK")
                        {
                            string local = savedir + "\\" + searchtype + count + ".jpeg";

                            MemoryStream streamjpeg = new MemoryStream(resjpeg.RawBytes);
                            using(FileStream filestream = new FileStream(local, FileMode.Create))
                            {
                                streamjpeg.CopyTo(filestream);             
                            }
                        }
                        else
                        {
                            if(item["replaceUrl"] != null)
                            {
                                var replace  = item["replaceUrl"].ToString();
                                JArray replaceUrlArr = (JArray)JsonConvert.DeserializeObject(replace);
                                foreach(var itemr in replaceUrlArr)
                                {
                                    imgurl = itemr["ObjURL"].ToString();
                                    Console.WriteLine(imgurl);
                                    clientimg = new RestClient(imgurl);
                                    requestjpeg = new RestRequest();
                                    requestjpeg.AddHeader("Referer", "http://image.baidu.com");
                                    resjpeg =  clientimg.Execute(requestjpeg);
                                    if(resjpeg.StatusCode.ToString() == "OK")
                                    {
                                        string local = savedir + "\\" + searchtype + count + ".jpg";
                                        MemoryStream streamjpeg = new MemoryStream(resjpeg.RawBytes);
                                        using(FileStream filestream = new FileStream(local, FileMode.Create))
                                        {
                                            streamjpeg.CopyTo(filestream);
                                        }
                                        break;
                                    }
                                }
                            }
                        } 
                    }
                    else if(matchpng.Success)
                    {
                        var clientimg = new RestClient(imgurl);
                        var requestpng = new RestRequest();
                        requestpng.AddHeader("Referer", "http://image.baidu.com");
                        IRestResponse respng =  clientimg.Execute(requestpng); 
                        if(respng.StatusCode.ToString() == "OK")
                        {
                            string local = savedir + "\\" + searchtype + count + ".png";
                        
                            MemoryStream streampng = new MemoryStream(respng.RawBytes);
                            using(FileStream filestream = new FileStream(local, FileMode.Create))
                            {
                                streampng.CopyTo(filestream);
                            }
                        }
                        else
                        {
                            if(item["replaceUrl"] != null)
                            {                           
                                var replace  = item["replaceUrl"].ToString();
                                JArray replaceUrlArr = (JArray)JsonConvert.DeserializeObject(replace);
                                foreach(var itemr in replaceUrlArr)
                                {
                                    imgurl = itemr["ObjURL"].ToString();
                                    Console.WriteLine(imgurl);
                                    clientimg = new RestClient(imgurl);
                                    requestpng = new RestRequest();
                                    requestpng.AddHeader("Referer", "http://image.baidu.com");
                                    respng =  clientimg.Execute(requestpng);
                                    if(respng.StatusCode.ToString() == "OK")
                                    {
                                        string local = savedir + "\\" + searchtype + count + ".jpg";
                                        MemoryStream streampng = new MemoryStream(respng.RawBytes);
                                        using(FileStream filestream = new FileStream(local, FileMode.Create))
                                        {
                                            streampng.CopyTo(filestream);
                                        }
                                        break;
                                    }
                                }
                            }
                        } 
                    }
                }
            }
            return "download pic success";
        }
    }
}