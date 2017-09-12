using Airgap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Airgap.Data.DTOEntities
{
    public class ApplianceDTO
    { 

        public ApplianceDTO() { }

        public ApplianceDTO(Appliance appliance)
        {
            this.Id = appliance.Id;
            this.AccountId = appliance.AccountId;
            this.DeviceName = appliance.DeviceName;
            this.SerialNumber = appliance.SerialNumber;
            this.OsVersion = appliance.OsVersion;
            this.TimerEnabled = appliance.TimerEnabled;
            this.GeoFenceEnabled = appliance.GeoFenceEnabled;
            this.IsConnected = appliance.IsConnected;
            this.TrustLevel = appliance.TrustLevel;
            this.Power = appliance.Power;
            this.Cellular = appliance.Cellular;
            this.WiFiInternet = appliance.WiFiInternet;
            this.Wifi = appliance.Wifi;
            this.Street1 = appliance.Street1;
            this.Street2 = appliance.Street2;
            this.City = appliance.City;
            this.StateId = appliance.StateId;
            this.ZipCode = appliance.ZipCode;
            this.Lat = appliance.Lat;
            this.Lon = appliance.Lon;
            this.TriggerMile = appliance.TriggerMile;
            this.IsOn = appliance.IsOn;
            this.Status = appliance.Status;
        }

        public long Id { get; set; }

        public long? AccountId { get; set; }

        [StringLength(50)]
        public string DeviceName { get; set; }

        [StringLength(14)]
        public string SerialNumber { get; set; }

        [StringLength(50)]
        public string OsVersion { get; set; }

        public bool? TimerEnabled { get; set; }

        public bool? GeoFenceEnabled { get; set; }

        public bool? IsConnected { get; set; }

        public int? TrustLevel { get; set; }

        public bool? Power { get; set; }

        public bool? Cellular { get; set; }

        public bool? WiFiInternet { get; set; }

        public bool? Wifi { get; set; }

        [StringLength(50)]
        public string Street1 { get; set; }

        [StringLength(50)]
        public string Street2 { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        public long? StateId { get; set; }

        [StringLength(10)]
        public string ZipCode { get; set; }

        public double? Lat { get; set; }

        public double? Lon { get; set; }

        public bool? IsOn { get; set; }

        public decimal? TriggerMile { get; set; }

        public bool? HasSerialNumber { get; set; }

        public bool? IsMatching { get; set; }

        public string AirGapVersion { get; set; }

        public bool? Status { get; set; }
    }
}
