using Airgap.Data.DTOEntities;
using Airgap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Airgap.Service.Helper
{
    public interface IHelperService
    {
        void ExecuteSendMail(string sendTo, string subject, string path, string url, Account acc, string content);
        FileInfo ExportExcel(List<SerialNumberExport> serialNumbers);
    }
}
