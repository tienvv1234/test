using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Data.DTOEntities
{
    public class SerialNumberDTO
    {
        public string SerialNumberInput { get; set; }
        public string[] SerialNumberExist { get; set; }
        public string[] SerialNumberNotExist { get; set; }
    }
}
