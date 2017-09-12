using Airgap.Constant;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Airgap.Telit
{
    public static class TelitApi
    {
        public static bool IsSslUsed {get; set;}
        public static string TelitHttpsUrl { get; set;}
        public static string TelitHttpUrl { get; set; }
        private static string SessionRef { get; set; }
        public static string Email { get; set; }
        public static string Password { get; set; }
        public static bool IsConnected { get; set; }

        private static HttpClient client;
        private static string TelitUrl
        {
            get
            {
                return IsSslUsed ? TelitHttpsUrl : TelitHttpUrl;
            }
        }

        //Authentication with Telit
        private static async Task<bool> Connect()
        {
            try
            {
                client = new HttpClient();
                client.BaseAddress = new Uri(TelitUrl);

                var connectionRequest = new
                {
                    auth = new
                    {
                        command = "api.authenticate",
                        @params = new
                        {
                            username = Email,
                            password = Password
                        }
                    }
                };

                HttpResponseMessage responseMessage = await client.PostAsJsonAsync(TelitUrl, connectionRequest);
                Connection.RootObject returnedData = new Connection.RootObject();
                if (responseMessage.IsSuccessStatusCode)
                {
                    returnedData = responseMessage.Content.ReadAsAsync<Connection.RootObject>().Result;
                    if (returnedData.auth != null && returnedData.auth.success)
                    {
                        IsConnected = true;
                        SessionRef = returnedData.auth.@params.sessionId;
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public static async Task<ThingList.RootObject> ThingList()
        {
            //Get authentication of not
            if (!IsConnected) await Connect();

            client = new HttpClient();
            client.BaseAddress = new Uri(TelitUrl);
            var getThingsRequest = new
            {
                auth = new
                {
                    sessionId = SessionRef
                },
                things = new
                {
                    command = "thing.list",
                    @params = new
                    {
                        offset = 0,
                        limit = 10
                    }
                }
            };
           
            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(TelitUrl, getThingsRequest);
            ThingList.RootObject sessionRef = new Telit.ThingList.RootObject();
            if (responseMessage.IsSuccessStatusCode)
            {
                sessionRef = responseMessage.Content.ReadAsAsync<ThingList.RootObject>().Result;
            }
            return sessionRef;
        }

        public static async Task<ThingFind.RootObject> ThingFind(string key)
        {
            //Get authentication of not
            if (!IsConnected) await Connect();

            client = new HttpClient();
            client.BaseAddress = new Uri(TelitUrl);
            var getThingsRequest = new
            {
                auth = new
                {
                    sessionId = SessionRef
                },
                things = new
                {
                    command = "thing.find",
                    @params = new
                    {
                        key = key
                    }
                }
            };


            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(TelitUrl, getThingsRequest);
            ThingFind.RootObject sessionRef = new Telit.ThingFind.RootObject();
            if (responseMessage.IsSuccessStatusCode)
            {
                sessionRef = responseMessage.Content.ReadAsAsync<ThingFind.RootObject>().Result;
            }
            return sessionRef;
        }

        public static async Task<bool> CheckIOTConnection()
        {
            //Get authentication of not
            if (!IsConnected) await Connect();
            return IsConnected;
        }

        public static async Task<bool> UpdateAlarmState(string key, AlarmStatus alarmStatus, string msg)
        {
            //Get authentication of not
            bool isSuccess = false;
            if (!IsConnected) await Connect();

            client = new HttpClient();
            client.BaseAddress = new Uri(TelitUrl);
            var getThingsRequest = new
            {
                auth = new
                {
                    sessionId = SessionRef
                },
                things = new
                {
                    command = "alarm.publish",
                    @params = new
                    {
                        thingKey = key,
                        key = "on",
                        state = alarmStatus,
                        msg = msg
                    }
                }
            };

            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(TelitUrl, getThingsRequest);
            if (responseMessage.IsSuccessStatusCode)
            {
                var status = responseMessage.Content.ReadAsAsync<Alarm.RootObject>().Result;

                if (status != null && status.things != null && status.things.success)
                {
                    isSuccess = status.things.success;
                }
                else
                {
                    isSuccess = status.things.success;
                }
            }

            return isSuccess;
        }

        public static async Task<bool> UpdateTimerState(string key, int value, bool statusAirgap)
        {
            //Get authentication of not
            bool isSuccess = false;
            if (!IsConnected) await Connect();

            client = new HttpClient();
            client.BaseAddress = new Uri(TelitUrl);
            var getThingsRequest = new
            {
                auth = new
                {
                    sessionId = SessionRef
                },
                things = new
                {
                    command = "alarm.publish",
                    @params = new
                    {
                        thingKey = key,
                        key = "timer-state",
                        state = value != 0 ? statusAirgap ? Configuration.TimerEnableAirgapOn : Configuration.TimerEnableAirGapOff : value
                    }
                }
            };

            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(TelitUrl, getThingsRequest);
            if (responseMessage.IsSuccessStatusCode)
            {
                var status = responseMessage.Content.ReadAsAsync<Alarm.RootObject>().Result;

                if (status != null && status.things != null && status.things.success)
                {
                    isSuccess = status.things.success;
                }
                else
                {
                    isSuccess = status.things.success;
                }
            }

            return isSuccess;
        }

        public static async Task<bool> UpdateStatusOfAppliance(string key, bool isDisable)
        {
            //Get authentication of not
            bool isSuccess = false;
            if (!IsConnected) await Connect();

            client = new HttpClient();
            client.BaseAddress = new Uri(TelitUrl);
            var getThingsRequest = new
            {
                auth = new
                {
                    sessionId = SessionRef
                },
                things = new
                {
                    command = "cdp.connection.update",
                    @params = new
                    {
                        iccid = key,
                        status = isDisable ? "deactivated" : "activated"
                    }
                }
            };

            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(TelitUrl, getThingsRequest);
            if (responseMessage.IsSuccessStatusCode)
            {
                var status = responseMessage.Content.ReadAsAsync<Alarm.RootObject>().Result;

                if (status != null && status.things != null && status.things.success)
                {
                    isSuccess = status.things.success;
                }
                else
                {
                    isSuccess = status.things.success;
                }
            }

            return isSuccess;
        }

        public static async Task<bool> UpdateEnvironment(string key, int value, string msg = null)
        {
            //Get authentication of not
            bool isSuccess = false;
            if (!IsConnected) await Connect();

            client = new HttpClient();
            client.BaseAddress = new Uri(TelitUrl);
            var getThingsRequest = new
            {
                auth = new
                {
                    sessionId = SessionRef
                },
                things = new
                {
                    command = "alarm.publish",
                    @params = new
                    {
                        thingKey = key,
                        key = "env",
                        state = value,
                        msg = msg
                    }
                }
            };

            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(TelitUrl, getThingsRequest);
            if (responseMessage.IsSuccessStatusCode)
            {
                var status = responseMessage.Content.ReadAsAsync<Alarm.RootObject>().Result;

                if (status != null && status.things != null && status.things.success)
                {
                    isSuccess = status.things.success;
                }
                else
                {
                    isSuccess = status.things.success;
                }
            }

            return isSuccess;
        }

    }
}
