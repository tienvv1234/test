using Microsoft.AspNetCore.Http;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Airgap.WebUi.Helper
{
    public class StripeHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public async Task ProcessRequestAsync(HttpContext context)
        {
            var json = await new StreamReader(context.Request.Body).ReadToEndAsync();

            var stripeEvent = StripeEventUtility.ParseEvent(json);

            switch (stripeEvent.Type)
            {
                case StripeEvents.ChargeRefunded:  // all of the types available are listed in StripeEvents
                    var stripeCharge = Stripe.Mapper<StripeCharge>.MapFromJson(stripeEvent.Data.Object.ToString());
                    break;
            }
        }
    }
}
