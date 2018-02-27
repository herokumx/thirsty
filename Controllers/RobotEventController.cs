using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using DemoDFRobot.Models;
using System.Dynamic;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;

namespace DemoDFRobot.Controllers
{
    public class RobotEventController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> SendData(bool isRobotFault, bool isCtrlFault)
        {
            try
            {
                using (var httpClient1 = new HttpClient())
                {
                    IronMQMessage ironMQMessage = null;
                    IOTEventRequest eventRequest = new IOTEventRequest();
					eventRequest.robotID = ConfigVars.Instance.RobotID;
                    eventRequest.robotEvent = isRobotFault ? "Fault" : "Operating";
                    eventRequest.controllerEvent = isCtrlFault ? "Fault" : "Operating";
                    httpClient1.DefaultRequestHeaders.Accept.Clear();
                    httpClient1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient1.DefaultRequestHeaders.Add("Authorization", ConfigVars.Instance.IOTToken);
                    HttpRequestMessage request = new HttpRequestMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(eventRequest), Encoding.UTF8, "application/json"),
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(ConfigVars.Instance.EnpointUrl)
                    };
                    var response = await httpClient1.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        using (var httpClient2 = new HttpClient())
                        {
                            dynamic jsonData = new ExpandoObject();
                            jsonData.n = 1;
                            jsonData.timeout = 60000;
                            jsonData.wait = 0;
                            jsonData.delete = false;
                            request = new HttpRequestMessage
                            {
                                Content = new StringContent(JsonConvert.SerializeObject(jsonData), Encoding.UTF8, "application/json"),
                                Method = HttpMethod.Post,
                                RequestUri = new Uri(string.Format("{0}/reservations?oauth={1}", ConfigVars.Instance.IronMQUrl, ConfigVars.Instance.IronMQToken))
                            };

                            //reserve message in iron message queue
                            response = await httpClient2.SendAsync(request);
                            if (response.IsSuccessStatusCode)
                            {
                                IList<IronMQMessage> messagesLst = null;
                                var ironMQResponse = await response.Content.ReadAsStringAsync();
                                var iron_MQMessage = JsonConvert.DeserializeObject<IDictionary<string, object>>(ironMQResponse);
                                if (iron_MQMessage.Count > 0 && iron_MQMessage.ContainsKey("messages"))
                                    messagesLst = JsonConvert.DeserializeObject<IList<IronMQMessage>>(iron_MQMessage["messages"].ToString());
                                if (messagesLst != null && messagesLst.Where(p => !string.IsNullOrEmpty(p.body)).Count() > 0)
                                {
                                    ironMQMessage = messagesLst.Where(p => !string.IsNullOrEmpty(p.body)).LastOrDefault();
                                    Parallel.Invoke(() =>
                                    {
                                        if (ironMQMessage != null)
                                        {
                                            //delete iron queue message after reading
                                            DeleteIronMessageByID(ironMQMessage.id, ironMQMessage.reservation_id);
                                        }
                                    });
                                }
                            }
                        }
                    }

                    if (ironMQMessage == null)
                        return Json(null);
                    else
                        return Json(ironMQMessage.body);
                }
            }
            catch (Exception ex)
            {
                return Json(null);
            }
        }

        public IActionResult Error()
        {
            return View();
        }

        private async void DeleteIronMessageByID(string pMessageID, string pReservationID)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    dynamic jsonData = new ExpandoObject();
                    jsonData.reservation_id = pReservationID;
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpRequestMessage request = new HttpRequestMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(jsonData), Encoding.UTF8, "application/json"),
                        Method = HttpMethod.Delete,
                        RequestUri = new Uri(string.Format("{0}/messages/{1}?oauth={2}", ConfigVars.Instance.IronMQUrl, pMessageID, ConfigVars.Instance.IronMQToken))
                    };
                    var response = await httpClient.SendAsync(request);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
