using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Constant
{
    public class ResponseData<T>
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<T> DataList { get; set; }
        public T Data { get; set; }
        public int TotalPage { get; set; }

        public ResponseData()
        {

        }
        public ResponseData(string status, string message, T data, List<T> dataList, int pages)
        {
            this.Status = status;
            this.Message = message;
            this.Data = data;
            this.DataList = dataList;
            this.TotalPage = pages;
        }
    }
}
