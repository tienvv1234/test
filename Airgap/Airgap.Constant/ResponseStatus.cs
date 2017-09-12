using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airgap.Constant
{
    public class ResponseStatus
    {
        public const int Success = 1;
        public const int Error = 2;
        public const int AccessDenied = 3;
        public const int Existed = 4;
    }

    public class EventType
    {
        public const int NetWorkStatusChange = 1;
        public const int HomePowerLoss = 2;
        public const int ISPOutage = 3;
        public const int AccountAssoWithAppliance = 4;
        public const int AccountRemoveFromAppliance = 5;
        public const int AccountVerifyForAppliance = 6;
        public const int ApplianceQRCodeScanned = 7;
        public const int StatusChangeFromTimer = 8;
        public const int StatusChangeFromGeoFence = 9;
        public const int ManualConnectedStatusChange = 10;  
        public const int AlwaysOnStatusChange = 11;
        public const int SigninSignoutOfWebPortal = 12;
        public const int PasswordReset = 13;
        public const int TrustLevelChange = 14;
        public const int ConnectionChange = 15;
        public const int ScheduleTimerChange = 16;
    }

    public class TimerType
    {
        public const int Weekdays = 1;
        public const int Weekends = 2;
    }

    public class ResponseMessage
    {
        public const string Success = "Success";
        public const string Error = "Error occurred during process";
        public const string ForgotPassword = "Forgot Password";
        public const string EmailNotValid = "Email or Password is wrong";
        public const string Signin = "Signin Signout Of WebPortal";
        public const string SignUp = "Sign Up";
        public const string EmailExist = "This Email is already existed";
        public const string EmailIsNotExist = "This Email is not exist";
        public const string ManualConnectedStatusChange = "Manual Connected Status Change";
        public const string AlwaysOnStatusChange = "Always On Status Change";
        public const string HaveNoAppliance = "The Account have no appliance";
        public const string SerialNumberInCorrect = "Invalid serial number";
        public const string SerialNumberIsExist = "This serial number are already entered";
        public const string Network = "Network";
        public const string Wifi = "Wifi";
        public const string HomePower = "Home Power";
        public const string TrustLevel = "TrustLevel";
        public const string ScheduleTimerChange = "Schedule Timer Change";
        public const string AirGapAlwaysOn = "AirGap Always";
        public const string ConnectionChange = "Connection Change";
        public const string ISPOutage = "ISP";
        public const string PhoneNumberExist = "Phone Number is already existed";
        public const string CoupOnInvalid = "CoupOn is Invalid";
        public const string CancelSubscriptionSuccess = "You've selected to discontinue the AirGap service. Remote power control, geofence and mobile client function will no longer function when the credits currently applied to the account are used up. <br><br>" +
            "You can cancel the disconntinue of service at any time by adding credits to the account, (clink Purchase Service). if you do this before the existing credits are expired there will be no disruption in service.<br><br>" +
            "To reactive an account you can simply clink Purchase Service any time. Note: it can be take up 48 hours for your AirGap Appliance to become live on the cellular network again after a lapse in service.";
    }

    public class Configuration
    {
        public const string MailSubjectForResetPassword = "AirGap Password Reset";
        public const string MailSubjectForSignUp = "AirGap Email Verification";
        public const string ContentForResetPassword = "If you forgot the password.<br>Click on the following link to to set your password:";
        public const string ContentForSignUp = "Thank you for using the AirGap system.<br>Click on the following link to to set your password:";
        public const string TurnOn = "on";
        public const string TurnOff = "off";
        public const string DayOfWeek = "M-F";
        public const string Saturday = "Saturday";
        public const string Sunday = "Sunday";
        public const string Env = "env";
        public const string Name = "name";
        public const string Trust = "trust";
        public const string TimmerState = "timer-state";
        public const string TimerSchedule = "timer-schedule";
        public const int TrustLevel_0 = 0;
        public const int TrustLevel_1 = 1;
        public const int TrustLevel_2 = 2;
        public const int TrustLevel_3 = 3;
        public const string Administrator = "Administrator";
        public const int TimerDisable = 0;
        public const int TimerEnable = 1;
        public const int TimerEnableAirGapOff = 1;
        public const int TimerEnableAirgapOn = 2;
        public const string Unknown = "Unknown";
        public const string PasswordNotMatch = "Exist Password not match";
        public const string UpdateFromAppliance = "Update from appliance.";
        public const string TickerText = "example test GCM";
        public const string ContentTitle = "content title GCM";
        public const string Android = "Android";
        public const string Ios = "Ios";
        public const int On = 1;
        public const int Off = 0;
        public const string Disable = "Disable";
        public const string Enable = "Enable";
        public const string Connected = "Connected";
        public const string Disconnected = "Disconnected ";
        public const int Weekdays = 1;
        public const int Weekends = 2;
    }
}
