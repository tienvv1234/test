using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Airgap.Telit;
using Airgap.Constant;
using System.Net;
using System.Xml.Linq;

namespace Airgap.WebApi.Controllers
{
    public class BaseController : Controller
    {
        public readonly AppSetting _appSettings;

        public BaseController(AppSetting appSettings)
        {
            _appSettings = appSettings;
            TelitApi.IsSslUsed = true;
            TelitApi.TelitHttpsUrl = _appSettings.TelitHttpsUrl;
            TelitApi.TelitHttpUrl = _appSettings.TelitHttpUrl;
            TelitApi.Email = _appSettings.TelitUsername;
            TelitApi.Password = _appSettings.TelitPassword;
        }

        public async Task<XElement> GetLatLngByAddressAsync(string street, string city, string state, string zipCode)
        {
            try
            {
                string address = street.Trim() + " " + city.Trim() + " " + state + " " + zipCode;
                var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=true", Uri.EscapeDataString(address));

                var request = WebRequest.Create(requestUri);
                var response = await request.GetResponseAsync();
                var xdoc = XDocument.Load(response.GetResponseStream());

                if (xdoc.Element("GeocodeResponse").Element("status").Value == "OK")
                {
                    var result = xdoc.Element("GeocodeResponse").Element("result");
                    foreach (XElement element in result.Descendants())
                    {
                        if (element.Element("type") != null && element.Element("type").Value == "locality")
                        {
                            if (!(element.Element("long_name").Value == city))
                            {
                                return null;
                            }
                        }

                        if (element.Element("type") != null && element.Element("type").Value == "administrative_area_level_1")
                        {
                            if (!(element.Element("long_name").Value == state))
                            {
                                return null;
                            }
                        }

                        if (element.Element("type") != null && element.Element("type").Value == "postal_code")
                        {
                            if (!(element.Element("long_name").Value == zipCode))
                            {
                                return null;
                            }
                        }

                    }

                    var locationElement = result.Element("geometry").Element("location");
                    return locationElement;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<bool> calculateAddressAsync(string street, string city, string state, string zipCode, double lat, double lng)
        {
            try
            {
                string address = street.Trim() + " " + city.Trim() + " " + state + " " + zipCode;
                var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=true", Uri.EscapeDataString(address));

                var request = WebRequest.Create(requestUri);
                var response = await request.GetResponseAsync();
                var xdoc = XDocument.Load(response.GetResponseStream());

                if(xdoc.Element("GeocodeResponse").Element("status").Value == "OK")
                {
                    var result = xdoc.Element("GeocodeResponse").Element("result");
                    foreach (XElement element in result.Descendants())
                    {
                        if (element.Element("type") != null && element.Element("type").Value == "locality")
                        {
                            if(!(element.Element("long_name").Value == city))
                            {
                                return false;
                            }
                        }

                        if (element.Element("type") != null && element.Element("type").Value == "administrative_area_level_1")
                        {
                            if (!(element.Element("long_name").Value == state))
                            {
                                return false;
                            }
                        }

                        if (element.Element("type") != null && element.Element("type").Value == "postal_code")
                        {
                            if (!(element.Element("long_name").Value == zipCode))
                            {
                                return false;
                            }
                        }

                    }

                    var locationElement = result.Element("geometry").Element("location");
                    var lat1 = locationElement.Element("lat").Value;
                    var lng1 = locationElement.Element("lng").Value;
                    var distance = DistanceBetweenPlaces(lat, lng, Convert.ToDouble(lat1), Convert.ToDouble(lng1));
                    return distance <= Convert.ToDouble(_appSettings.Miles) ? true : false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

        }

        public string ReplaceAt(string input, int index, char newChar)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            char[] chars = input.ToCharArray();
            chars[index] = newChar;
            return new string(chars);
        }

        public double DistanceBetweenPlaces(double lon1, double lat1, double lon2, double lat2)
        {
            double R = 6371; // km

            double sLat1 = Math.Sin(Radians(lat1));
            double sLat2 = Math.Sin(Radians(lat2));
            double cLat1 = Math.Cos(Radians(lat1));
            double cLat2 = Math.Cos(Radians(lat2));
            double cLon = Math.Cos(Radians(lon1) - Radians(lon2));

            double cosD = sLat1 * sLat2 + cLat1 * cLat2 * cLon;

            double d = Math.Acos(cosD);

            double dist = R * d * 0.621371192; // convert to mile;

            return dist ;
        }

        private double Radians(double x)
        {
            return x * Math.PI / 180;
        }
    }

}
