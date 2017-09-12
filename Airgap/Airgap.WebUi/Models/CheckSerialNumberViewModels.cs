using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airgap.WebUi.Models
{
    public class CheckSerialNumberViewModels
    {
        public string SerialNumberInput { get; set; }
        public string SerialNumberExist { get; set; }
        public string SerialNumberNotExist { get; set; }
    }
}
