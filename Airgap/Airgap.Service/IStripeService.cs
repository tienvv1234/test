using Airgap.Data.ApiEntities;
using Airgap.Entity.Entities;
using Stripe;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Service
{
    public interface IStripeService
    {
        List<StripePlan> GetPlans();
        StripePlan GetPlansByPlanId(string planId);
        StripeCustomer CreateUser(PurchaseInformation info, string email, string coupon = null);
        StripeSubscription CreateSubscription(PurchaseInformation info, string customerIdOfStripe);
        StripeSubscription UpdateSubscription(PurchaseInformation info, string customerIdOfStripe, string subscriptionId);
        StripeCoupon RetrieveCoupon(string coupon);
        StripeSubscription RetrieveSubscription(string subscriptionId);
        StripeSubscriptionService CancelSubscription(string subscriptionId);
    }
}
