using System;
using System.Collections.Generic;
using System.Text;
using Airgap.Entity.Entities;
using Stripe;
using Airgap.Constant;
using Microsoft.Extensions.Options;
using System.Linq;
using Airgap.Data.ApiEntities;

namespace Airgap.Service
{
    public class StripeService : IStripeService
    {

        private readonly AppSetting _appSettings;

        public StripeService(IOptions<AppSetting> appSettings)
        {
            _appSettings = appSettings.Value;
            StripeConfiguration.SetApiKey(_appSettings.APIKeyOfStripe);
        }

        public List<StripePlan> GetPlans()
        {
            try
            {
                List<StripePlan> plans = new List<StripePlan>();
                var planService = new StripePlanService();
                IEnumerable<StripePlan> response = planService.List(); // optional StripeListOptions
                foreach (var item in response)
                {
                    plans.Add(item);
                }
                return plans;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public StripePlan GetPlansByPlanId(string planId)
        {
            try
            {

                var planService = new StripePlanService();
                StripePlan plan = planService.Get(planId);
                return plan;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public StripeCustomer CreateUser(PurchaseInformation info, string email, string coupon = null)
        {
            try
            {
                var customer = new StripeCustomerCreateOptions();
                customer.Email = email;

                customer.SourceCard = new SourceCard()
                {
                    Number = info.CC_number,
                    ExpirationYear = Convert.ToInt32(info.ExpireYear),
                    ExpirationMonth = Convert.ToInt32(info.ExpireMonth),
                    Cvc = info.CCVCode,
                    Name = info.NameOnCard
                };
                //customer.PlanId = info.PlanId;                          // only if you have a plan
                //if (!string.IsNullOrEmpty(coupon))
                //    customer.CouponId = coupon;
                var customerService = new StripeCustomerService();
                StripeCustomer stripeCustomer = customerService.Create(customer);

                StripeSubscriptionCreateOptions options = new StripeSubscriptionCreateOptions()
                {
                    PlanId = info.PlanId,
                    Quantity = 1,
                    CouponId = !string.IsNullOrEmpty(info.Coupon) ? info.Coupon : null
                };


                var subscriptionService = new StripeSubscriptionService();
                StripeSubscription stripeSubscription = subscriptionService.Create(stripeCustomer.Id, options); // optional StripeSubscriptionCreateOptions
                stripeCustomer.Subscriptions.Data.Add(stripeSubscription);
                return stripeCustomer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public StripeSubscription CreateSubscription(PurchaseInformation info, string customerIdOfStripe)
        {
            try
            {
                var customer = new StripeCustomerUpdateOptions();

                customer.SourceCard = new SourceCard()
                {
                    Number = info.CC_number,
                    ExpirationYear = Convert.ToInt32(info.ExpireYear),
                    ExpirationMonth = Convert.ToInt32(info.ExpireMonth),
                    Cvc = info.CCVCode,
                    Name = info.NameOnCard
                };


                if (!string.IsNullOrEmpty(info.Coupon))
                    customer.Coupon = info.Coupon;


                var customerService = new StripeCustomerService();
                StripeCustomer stripeCustomer = customerService.Update(customerIdOfStripe, customer);

                var subscriptionService = new StripeSubscriptionService();
                StripeSubscription stripeSubscription = subscriptionService.Create(customerIdOfStripe, info.PlanId); // optional StripeSubscriptionCreateOptions
                return stripeSubscription;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public StripeSubscription UpdateSubscription(PurchaseInformation info, string customerIdOfStripe, string subscriptionId)
        {
            try
            {
                var customer = new StripeCustomerUpdateOptions();

                customer.SourceCard = new SourceCard()
                {
                    Number = info.CC_number,
                    ExpirationYear = Convert.ToInt32(info.ExpireYear),
                    ExpirationMonth = Convert.ToInt32(info.ExpireMonth),
                    Cvc = info.CCVCode,
                    Name = info.NameOnCard
                };

                if (!string.IsNullOrEmpty(info.Coupon))
                    customer.Coupon = info.Coupon;


                var customerService = new StripeCustomerService();
                StripeCustomer stripeCustomer = customerService.Update(customerIdOfStripe, customer);

                StripeSubscriptionUpdateOptions options = new StripeSubscriptionUpdateOptions()
                {
                    PlanId = info.PlanId,
                    Quantity = 1,
                    CouponId = !string.IsNullOrEmpty(info.Coupon) ? info.Coupon : null
                };

                var subscriptionService = new StripeSubscriptionService();
                StripeSubscription stripeSubscription = subscriptionService.Update(subscriptionId, options); // optional StripeSubscriptionCreateOptions
                return stripeSubscription;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public StripeCoupon RetrieveCoupon(string coupon)
        {
            try
            {
                var couponService = new StripeCouponService();
                StripeCoupon response = couponService.Get(coupon);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public StripeSubscription RetrieveSubscription(string subscriptionId)
        {
            try
            {
                var subscriptionService = new StripeSubscriptionService();
                StripeSubscription stripeSubscription = subscriptionService.Get(subscriptionId);
                return stripeSubscription;
            }
            catch(Exception ex) {
                throw ex;
            }
        }

        public StripeSubscriptionService CancelSubscription(string subscriptionId)
        {
            try
            {
                var subscriptionService = new StripeSubscriptionService();
                subscriptionService.Cancel(subscriptionId, true);
                return subscriptionService;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
