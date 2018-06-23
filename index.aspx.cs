using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;
using RestSharp;
using System.Threading;

namespace ocr_web
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void UploadFile(object sender, EventArgs e)
        {
            string folderPath = Server.MapPath("~/Files/");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            FileUpload1.SaveAs(folderPath + Path.GetFileName(FileUpload1.FileName));
            var filename = FileUpload1.FileName;
            var url = "< domain path >" + filename;  //< domain path > https://example.com/project/upload_files/
            ocr(url);
        }

        protected void ocr(string url)
        {
            // Key
            string key = "< Subscription Key >";
            // submit image
            var client = new RestClient("https://westcentralus.api.cognitive.microsoft.com/vision/v2.0/recognizeText?mode=Printed");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Ocp-Apim-Subscription-Key", key);
            request.AddParameter("", "{\"url\":\"" + url + "\"}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                var response_header = response.Headers;
                var operation_location = response_header[1].Value;

                // Wait time to process the image in computer vision
                Thread.Sleep(5000);

                // get  response
                var client2 = new RestClient("" + operation_location + "");
                var request2 = new RestRequest(Method.GET);
                request2.AddHeader("Ocp-Apim-Subscription-Key", key);
                IRestResponse response2 = client2.Execute(request2);
                if (response2.IsSuccessful)
                {
                    var response_data = response2.Content;
                    var arr_data = JObject.Parse(response_data);
                    string status = (string)arr_data["status"];
                    if (status == "Succeeded")
                    {
                        var lines = arr_data["recognitionResult"]["lines"].Value<JArray>();
                        int lines_count = lines.Count;
                        string _data = "";
                        for (int i = 0; i < lines_count; i++)
                        {
                            var cd = arr_data["recognitionResult"]["lines"][i]["text"];
                            _data = _data + cd + " <br> ";
                        }
                        r1.InnerHtml = _data;
                    }
                    else
                    {
                        // wait again for 10 sec to process
                        Thread.Sleep(10000);
                        var client3 = new RestClient("" + operation_location + "");
                        var request3 = new RestRequest(Method.GET);
                        request3.AddHeader("Ocp-Apim-Subscription-Key", key);
                        IRestResponse response3 = client3.Execute(request3);
                        if (response3.IsSuccessful)
                        {
                            var response_data2 = response3.Content;
                            var arr_data2 = JObject.Parse(response_data2);
                            string status2 = (string)arr_data2["status"];
                            if (status2 == "Succeeded")
                            {
                                var lines2 = arr_data2["recognitionResult"]["lines"].Value<JArray>();
                                int lines_count2 = lines2.Count;
                                string _data2 = "";
                                for (int i = 0; i < lines_count2; i++)
                                {
                                    var cd2 = arr_data2["recognitionResult"]["lines"][i]["text"];
                                    _data2 = _data2 + cd2 + " <br> ";
                                }
                                r1.InnerHtml = _data2;
                            }
                        }
                        else
                        {
                            r1.InnerHtml = "Server is taking too much time to process the image, kindly increase the buffer seconds.";
                        }
                    }
                }
                else
                {
                    r1.InnerHtml = "Error in getting response from serrver";
                }
            }
            else
            {
                r1.InnerHtml = "Error in posting image to serrver";
            }
        }
    }
}
