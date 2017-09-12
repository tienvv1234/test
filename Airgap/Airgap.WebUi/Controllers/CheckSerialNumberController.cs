using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Airgap.WebUi.Models;
using Airgap.Constant;
using System.Net.Http;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using Airgap.Data.DTOEntities;
using Airgap.Service.Helper;

namespace Airgap.WebUi.Controllers
{
    public class CheckSerialNumberController : BaseController
    {
        private readonly AppSetting _appSettings;
        private HttpClient client;
        private IHelperService _helperService;
        public CheckSerialNumberController(IHelperService helperService, IOptions<AppSetting> appSettings)
        {
            _appSettings = appSettings.Value;
            this._helperService = helperService;
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public ActionResult Index()
        {
            CheckSerialNumberViewModels model = new CheckSerialNumberViewModels();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(CheckSerialNumberViewModels model)
        {
            string url = _appSettings.ApiUrl + "/Home/CheckSerialNumber";
            client.BaseAddress = new Uri(url);
            SerialNumberDTO dto = new SerialNumberDTO()
            {
                SerialNumberInput = model.SerialNumberInput
            };
            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(url, dto);
            var responseData = new ResponseData<SerialNumberDTO>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<SerialNumberDTO>>().Result;
                if (responseData != null && responseData.Data != null)
                {
                    ViewBag.SerialNumberExist = responseData.Data.SerialNumberExist;
                    ViewBag.SerialNumberNotExist = responseData.Data.SerialNumberNotExist;

                    return View(model);
                }
            }

            return View();
        }


        public async Task<FileResult> Export()
        {
            string url = _appSettings.ApiUrl + "/Air/GetAllThingFromTelit";
            client.BaseAddress = new Uri(url);
            HttpResponseMessage responseMessage = await client.GetAsync(url);

            var responseData = new ResponseData<SerialNumberExport>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<SerialNumberExport>>().Result;
                if (responseData != null && responseData.DataList != null && responseData.DataList.Count > 0)
                {
                    var file = _helperService.ExportExcel(responseData.DataList);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(file.FullName);
                    return File(fileBytes, "application/x-msdownload", file.Name);
                }
            }
            return null;
        }
    }

}