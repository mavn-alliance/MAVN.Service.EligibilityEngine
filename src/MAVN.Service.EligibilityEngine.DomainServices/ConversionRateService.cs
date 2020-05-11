using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MAVN.Service.Campaign.Client;
using MAVN.Service.Campaign.Client.Models.BurnRule.Requests;
using MAVN.Service.EligibilityEngine.Domain.Exceptions;
using MAVN.Service.EligibilityEngine.Domain.Models;
using MAVN.Service.EligibilityEngine.Domain.Services;
using MAVN.Service.PartnerManagement.Client;
using MAVN.Numerics;
using MAVN.Service.Campaign.Client.Models.BurnRule.Responses;
using MAVN.Service.Campaign.Client.Models.Campaign.Responses;
using MAVN.Service.Campaign.Client.Models.Condition;
using MAVN.Service.Campaign.Client.Models.Enums;
using MAVN.Service.CurrencyConvertor.Client;

namespace MAVN.Service.EligibilityEngine.DomainServices
{
    public class ConversionRateService : IConversionRateService
    {
        private readonly ICampaignClient _campaignClient;
        private readonly IPartnerManagementClient _partnerManagementClient;
        private readonly IRateCalculatorService _rateCalculatorService;
        private readonly ICurrencyConvertorClient _currencyConvertorClient;

        public ConversionRateService(
            ICampaignClient campaignClient,
            IPartnerManagementClient partnerManagementClient,
            ICurrencyConvertorClient currencyConvertorClient,
            IRateCalculatorService rateCalculatorService)
        {
            _campaignClient = campaignClient ?? throw new ArgumentNullException(nameof(ICampaignClient));
            _partnerManagementClient = partnerManagementClient ?? throw new ArgumentNullException(nameof(IPartnerManagementClient));
            _currencyConvertorClient = currencyConvertorClient ?? throw new ArgumentNullException(nameof(ICurrencyConvertorClient));
            _rateCalculatorService = rateCalculatorService ?? throw new ArgumentNullException(nameof(IRateCalculatorService));
        }

        public async Task<OptimalConversionRateByPartnerResponse> GetOptimalCurrencyRateByPartnerAsync(Guid partnerId, Guid customerId, string from, string to)
        {
            var burnRules = await _campaignClient.BurnRules.GetAsync(new BurnRulePaginationRequest
            {
                CurrentPage = 1,
                PageSize = 500,
                PartnerId = partnerId
            });

            if (burnRules.BurnRules.Any())
            {
                var bestRule = burnRules.BurnRules
                    .Where(r => !r.UsePartnerCurrencyRate)
                    .OrderByDescending(r => r.AmountInCurrency / r.AmountInTokens)
                    .FirstOrDefault();

                if (bestRule != null)
                {
                    return new OptimalConversionRateByPartnerResponse
                    {
                        UsedRate = await _rateCalculatorService.CalculateConversionRate(bestRule.AmountInCurrency.Value, bestRule.AmountInTokens.Value, from, to),
                        ConversionSource = ConversionSource.BurnRule,
                        SpendRuleId = bestRule.Id,
                        CustomerId = customerId
                    };
                }
            }

            var partner = await _partnerManagementClient.Partners.GetByIdAsync(partnerId);

            if (partner == null)
            {
                throw new PartnerNotFoundException($"Partner with id '{partnerId}' cannot be found.");
            }

            var conversionSource = ConversionSource.Partner;

            decimal amountInCurrency = 0;
            Money18 amountInTokens = 0;

            if (partner.UseGlobalCurrencyRate)
            {
                conversionSource = ConversionSource.Global;
                var globalRate = await _currencyConvertorClient.GlobalCurrencyRates.GetAsync();
                amountInCurrency = globalRate.AmountInCurrency;
                amountInTokens = globalRate.AmountInTokens;
            }
            else
            {
                amountInCurrency = partner.AmountInCurrency.Value;
                amountInTokens = partner.AmountInTokens.Value;
            }

            return new OptimalConversionRateByPartnerResponse
            {
                UsedRate = await _rateCalculatorService.CalculateConversionRate(amountInCurrency, amountInTokens, from, to),
                ConversionSource = conversionSource,
                SpendRuleId = null,
                CustomerId = customerId
            };
        }

        public async Task<ConversionRateByEarnRuleResponse> GetCurrencyRateByEarnRuleIdAsync(Guid requestEarnRuleId, Guid requestCustomerId, string from, string to)
        {
            var earnRule = await _campaignClient.Campaigns.GetByIdAsync(requestEarnRuleId.ToString());

            if (earnRule == null || earnRule.ErrorCode == CampaignServiceErrorCodes.EntityNotFound)
            {
                throw new EarnRuleNotFoundException($"Earn rule with id '{requestEarnRuleId}' cannot be found.");
            }

            var rate = await GetEarnRuleConversionRate(earnRule);

            return new ConversionRateByEarnRuleResponse
            {
                UsedRate = await _rateCalculatorService.CalculateConversionRate(rate.AmountInCurrency, rate.AmountInTokens, from, to),
                ConversionSource = rate.ConversionSource,
                EarnRuleId = requestEarnRuleId,
                CustomerId = requestCustomerId
            };
        }

        public async Task<ConversionRateBySpendRuleResponse> GetCurrencyRateBySpendRuleIdAsync(Guid spendRuleId, Guid customerId, string from, string to)
        {
            var burnRule = await _campaignClient.BurnRules.GetByIdAsync(spendRuleId);

            if (burnRule == null || burnRule.ErrorCode == CampaignServiceErrorCodes.EntityNotFound)
            {
                throw new BurnRuleNotFoundException($"Spend rule with id '{spendRuleId}' cannot be found.");
            }

            var rate = await GetBurnRuleConversionRate(burnRule);

            return new ConversionRateBySpendRuleResponse
            {
                UsedRate = await _rateCalculatorService.CalculateConversionRate(rate.AmountInCurrency, rate.AmountInTokens, from, to),
                ConversionSource = rate.ConversionSource,
                SpendRuleId = spendRuleId,
                CustomerId = customerId
            };
        }

        public async Task<OptimalAmountConvertResponse> ConvertOptimalByPartnerAsync(Guid partnerId, Guid customerId, string from, string to, Money18 amount)
        {
            var conversionRate = await GetOptimalCurrencyRateByPartnerAsync(partnerId, customerId, from, to);

            return new OptimalAmountConvertResponse
            {
                Amount = Money18.Multiply(amount, conversionRate.UsedRate),
                ConversionSource = conversionRate.ConversionSource,
                CurrencyCode = to,
                UsedRate = conversionRate.UsedRate,
                SpendRuleId = conversionRate.SpendRuleId,
                CustomerId = customerId
            };
        }

        public async Task<AmountConvertByEarnRuleResponse> GetAmountByEarnRuleAsync(Guid earnRuleId, Guid customerId, string from, string to, Money18 amount)
        {
            var conversionRate = await GetCurrencyRateByEarnRuleIdAsync(earnRuleId, customerId, from, to);

            return new AmountConvertByEarnRuleResponse()
            {
                Amount = Money18.Multiply(amount, conversionRate.UsedRate),
                CurrencyCode = to,
                UsedRate = conversionRate.UsedRate,
                ConversionSource = ConversionSource.EarnRule,
                EarnRuleId = earnRuleId,
                CustomerId = customerId
            };
        }

        public async Task<AmountConvertBySpendRuleResponse> GetAmountBySpendRuleAsync(Guid spendRuleId, Guid customerId, string from, string to, Money18 amount)
        {
            var conversionRate = await GetCurrencyRateBySpendRuleIdAsync(spendRuleId, customerId, from, to);

            return new AmountConvertBySpendRuleResponse()
            {
                Amount = Money18.Multiply(amount, conversionRate.UsedRate),
                CurrencyCode = to,
                UsedRate = conversionRate.UsedRate,
                ConversionSource = ConversionSource.BurnRule,
                SpendRuleId = spendRuleId,
                CustomerId = customerId
            };
        }

        public async Task<AmountByConditionResponse> GetAmountConditionAsync(Guid conditionId, Guid customerId, string fromCurrency, string toCurrency,
            Money18 amount)
        {
            var conversionRate = await GetCurrencyRateByConditionIdAsync(conditionId, customerId, fromCurrency, toCurrency);

            return new AmountByConditionResponse()
            {
                Amount = Money18.Multiply(amount, conversionRate.UsedRate),
                CurrencyCode = toCurrency,
                UsedRate = conversionRate.UsedRate,
                ConversionSource = ConversionSource.Condition,
                ConditionId = conditionId,
                CustomerId = customerId
            };
        }

        public async Task<ConversionRateByConditionResponse> GetCurrencyRateByConditionIdAsync(Guid conditionId, Guid customerId, string fromCurrency, string toCurrency)
        {
            var conditionResponse = await _campaignClient.Conditions.GetByIdAsync(conditionId.ToString());

            if (conditionResponse.Condition == null || conditionResponse.ErrorCode == CampaignServiceErrorCodes.EntityNotFound)
            {
                throw new ConditionNotFoundException($"Condition with id '{conditionId}' cannot be found.");
            }

            var rate = await GetConditionConversionRate(conditionResponse.Condition);

            return new ConversionRateByConditionResponse
            {
                UsedRate = await _rateCalculatorService.CalculateConversionRate(rate.AmountInCurrency, rate.AmountInTokens, fromCurrency, toCurrency),
                ConversionSource = rate.ConversionSource,
                ConditionId = conditionId,
                CustomerId = customerId
            };
        }

        private async Task<(decimal AmountInCurrency, Money18 AmountInTokens, ConversionSource ConversionSource)> GetConditionConversionRate(ConditionModel condition)
        {
            if (condition.UsePartnerCurrencyRate)
            {
                var partnerIds = condition.PartnerIds;

                return await GetConversionRateForPartnerRules(partnerIds != null ? partnerIds.Distinct().ToList() : new List<Guid>());
            }

            if (condition.RewardType == RewardType.ConversionRate && condition.AmountInCurrency.HasValue && condition.AmountInTokens.HasValue)
            {
                return (condition.AmountInCurrency.Value, condition.AmountInTokens.Value, ConversionSource.Condition);
            }
            else
            {
                return await GetGlobalConversionRate();
            }
        }

        private async Task<(decimal AmountInCurrency, Money18 AmountInTokens, ConversionSource ConversionSource)> GetBurnRuleConversionRate(BurnRuleResponse burnRule)
        {
            if (burnRule.UsePartnerCurrencyRate)
            {
                var partnerIds = burnRule.PartnerIds;

                return await GetConversionRateForPartnerRules(partnerIds != null ? partnerIds.Distinct().ToList() : new List<Guid>());
            }

            return (burnRule.AmountInCurrency.Value, burnRule.AmountInTokens.Value, ConversionSource.BurnRule);
        }

        private async Task<(decimal AmountInCurrency, Money18 AmountInTokens, ConversionSource ConversionSource)> GetEarnRuleConversionRate(CampaignDetailResponseModel earnRule)
        {
            if (earnRule.UsePartnerCurrencyRate)
            {
                var partnerIds = earnRule.Conditions
                    .Where(c => c.PartnerIds != null)
                    .SelectMany(c => c.PartnerIds)
                    .Distinct()
                    .ToList();

                return await GetConversionRateForPartnerRules(partnerIds);
            }

            if (earnRule.RewardType == RewardType.ConversionRate && earnRule.AmountInCurrency.HasValue && earnRule.AmountInTokens.HasValue)
            {
                return (earnRule.AmountInCurrency.Value, earnRule.AmountInTokens.Value, ConversionSource.EarnRule);
            }
            else
            {
                return await GetGlobalConversionRate();
            }
        }

        private async Task<(decimal AmountInCurrency, Money18 AmountInTokens, ConversionSource ConversionSource)> GetConversionRateForPartnerRules(List<Guid> partnerIds)
        {
            if (partnerIds == null || !partnerIds.Any() || partnerIds.Count() > 1)
            {
                return await GetGlobalConversionRate();
            }

            var partner = await _partnerManagementClient.Partners.GetByIdAsync(partnerIds[0]);
            if (partner == null || partner.UseGlobalCurrencyRate)
            {
                return await GetGlobalConversionRate();
            }

            return (partner.AmountInCurrency.Value, partner.AmountInTokens.Value, ConversionSource.Partner);
        }

        private async Task<(decimal AmountInCurrency, Money18 AmountInTokens, ConversionSource ConversionSource)> GetGlobalConversionRate()
        {
            var globalRate = await _currencyConvertorClient.GlobalCurrencyRates.GetAsync();

            return (globalRate.AmountInCurrency, globalRate.AmountInTokens, ConversionSource.Global);
        }
    }
}
