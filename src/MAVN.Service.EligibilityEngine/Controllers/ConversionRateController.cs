using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.Common.Log;
using MAVN.Service.EligibilityEngine.Client;
using MAVN.Service.EligibilityEngine.Client.Enums;
using MAVN.Service.EligibilityEngine.Client.Models.ConversionRate.Requests;
using MAVN.Service.EligibilityEngine.Client.Models.ConversionRate.Responses;
using MAVN.Service.EligibilityEngine.Domain.Exceptions;
using MAVN.Service.EligibilityEngine.Domain.Models;
using MAVN.Service.EligibilityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using ConversionSource = MAVN.Service.EligibilityEngine.Client.Enums.ConversionSource;
using ConvertAmountByConditionResponse = MAVN.Service.EligibilityEngine.Client.Models.ConversionRate.Responses.ConvertAmountByConditionResponse;

namespace MAVN.Service.EligibilityEngine.Controllers
{
    [Route("api/conversionRate")]
    [ApiController]
    public class ConversionRateController : Controller, IConversionRateApi
    {
        private readonly IConversionRateService _conversionRateService;
        private readonly ILog _log;

        public ConversionRateController(IConversionRateService conversionRateService, ILogFactory logFactory)
        {
            _conversionRateService = conversionRateService ?? throw new ArgumentNullException(nameof(IConversionRateService));
            _log = logFactory.CreateLog(this);
        }

        /// <inheritdoc/>
        [HttpPost("partnerRate")]
        [ProducesResponseType(typeof(OptimalCurrencyRateByPartnerResponse), (int)HttpStatusCode.OK)]
        public async Task<OptimalCurrencyRateByPartnerResponse> GetOptimalCurrencyRateByPartnerAsync(OptimalCurrencyRateByPartnerRequest request)
        {
            if (request.FromCurrency.ToLower() == request.ToCurrency.ToLower())
            {
                return new OptimalCurrencyRateByPartnerResponse
                {
                    ConversionSource = ConversionSource.Partner,
                    Rate = 1
                };
            }

            OptimalConversionRateByPartnerResponse result;

            try
            {
                result = await _conversionRateService.GetOptimalCurrencyRateByPartnerAsync(request.PartnerId, request.CustomerId, request.FromCurrency, request.ToCurrency);
            }
            catch (PartnerNotFoundException e)
            {
                _log.Info(e.Message, context: request);

                return new OptimalCurrencyRateByPartnerResponse
                {
                    ErrorCode = EligibilityEngineErrors.PartnerNotFound,
                    ErrorMessage = e.Message
                };
            }
            catch (ConversionRateNotFoundException e)
            {
                _log.Info(e.Message, context: request);

                return new OptimalCurrencyRateByPartnerResponse
                {
                    ErrorCode = EligibilityEngineErrors.ConversionRateNotFound,
                    ErrorMessage = e.Message
                };
            }

            return new OptimalCurrencyRateByPartnerResponse
            {
                Rate = result.UsedRate,
                ConversionSource = Mapper.Map<ConversionSource>(result.ConversionSource),
                SpendRuleId = result.SpendRuleId
            };
        }

        /// <inheritdoc/>
        [HttpPost("earnRuleRate")]
        [ProducesResponseType(typeof(CurrencyRateByEarnRuleResponse), (int)HttpStatusCode.OK)]
        public async Task<CurrencyRateByEarnRuleResponse> GetCurrencyRateByEarnRuleIdAsync(CurrencyRateByEarnRuleRequest request)
        {
            if (request.FromCurrency.ToLower() == request.ToCurrency.ToLower())
            {
                return new CurrencyRateByEarnRuleResponse
                {
                    ConversionSource = ConversionSource.EarnRule,
                    Rate = 1
                };
            }

            ConversionRateByEarnRuleResponse result;

            try
            {
                result = await _conversionRateService.GetCurrencyRateByEarnRuleIdAsync(
                    request.EarnRuleId, request.CustomerId, request.FromCurrency, request.ToCurrency);
            }
            catch (EarnRuleNotFoundException e)
            {
                _log.Info(e.Message, context: request);

                return new CurrencyRateByEarnRuleResponse
                {
                    ErrorCode = EligibilityEngineErrors.EarnRuleNotFound,
                    ErrorMessage = e.Message
                };
            }
            catch (ConversionRateNotFoundException e)
            {
                _log.Info(e.Message, context: request);

                return new CurrencyRateByEarnRuleResponse
                {
                    ErrorCode = EligibilityEngineErrors.ConversionRateNotFound,
                    ErrorMessage = e.Message
                };
            }

            return new CurrencyRateByEarnRuleResponse
            {
                Rate = result.UsedRate,
                ConversionSource = Mapper.Map<ConversionSource>(result.ConversionSource),
                EarnRuleId = result.EarnRuleId
            };
        }

        /// <inheritdoc/>
        [HttpPost("spendRuleRate")]
        [ProducesResponseType(typeof(CurrencyRateBySpendRuleResponse), (int)HttpStatusCode.OK)]
        public async Task<CurrencyRateBySpendRuleResponse> GetCurrencyRateBySpendRuleIdAsync(CurrencyRateBySpendRuleRequest request)
        {
            if (request.FromCurrency.ToLower() == request.ToCurrency.ToLower())
            {
                return new CurrencyRateBySpendRuleResponse
                {
                    ConversionSource = ConversionSource.BurnRule,
                    Rate = 1
                };
            }

            ConversionRateBySpendRuleResponse result;

            try
            {
                result = await _conversionRateService.GetCurrencyRateBySpendRuleIdAsync(
                    request.SpendRuleId, request.CustomerId, request.FromCurrency, request.ToCurrency);
            }
            catch (BurnRuleNotFoundException e)
            {
                _log.Info(e.Message, context: request);

                return new CurrencyRateBySpendRuleResponse
                {
                    ErrorCode = EligibilityEngineErrors.SpendRuleNotFound,
                    ErrorMessage = e.Message
                };
            }
            catch (ConversionRateNotFoundException e)
            {
                _log.Info(e.Message, context: request);

                return new CurrencyRateBySpendRuleResponse
                {
                    ErrorCode = EligibilityEngineErrors.ConversionRateNotFound,
                    ErrorMessage = e.Message
                };
            }

            return new CurrencyRateBySpendRuleResponse
            {
                Rate = result.UsedRate,
                ConversionSource = Mapper.Map<ConversionSource>(result.ConversionSource),
                SpendRuleId = result.SpendRuleId
            };
        }

        /// <inheritdoc/>
        [HttpPost("partnerAmount")]
        [ProducesResponseType(typeof(ConvertOptimalByPartnerResponse), (int)HttpStatusCode.OK)]
        public async Task<ConvertOptimalByPartnerResponse> ConvertOptimalByPartnerAsync(ConvertOptimalByPartnerRequest request)
        {
            if (request.FromCurrency.ToLower() == request.ToCurrency.ToLower())
            {
                return new ConvertOptimalByPartnerResponse
                {
                    ConversionSource = ConversionSource.Partner,
                    Amount = request.Amount,
                    UsedRate = 1
                };
            }

            OptimalAmountConvertResponse result;

            try
            {
                result = await _conversionRateService.ConvertOptimalByPartnerAsync(
                    request.PartnerId, request.CustomerId, request.FromCurrency, request.ToCurrency, request.Amount);
            }
            catch (PartnerNotFoundException e)
            {
                _log.Info(e.Message, context: request);

                return new ConvertOptimalByPartnerResponse
                {
                    ErrorCode = EligibilityEngineErrors.PartnerNotFound,
                    ErrorMessage = e.Message
                };
            }
            catch (ConversionRateNotFoundException e)
            {
                _log.Info(e.Message, context: request);

                return new ConvertOptimalByPartnerResponse
                {
                    ErrorCode = EligibilityEngineErrors.ConversionRateNotFound,
                    ErrorMessage = e.Message
                };
            }

            return new ConvertOptimalByPartnerResponse
            {
                Amount = result.Amount,
                CurrencyCode = result.CurrencyCode,
                UsedRate = result.UsedRate,
                ConversionSource = Mapper.Map<ConversionSource>(result.ConversionSource),
                SpendRuleId = result.SpendRuleId
            };
        }

        /// <inheritdoc/>
        [HttpPost("earnRuleAmount")]
        [ProducesResponseType(typeof(ConvertAmountByEarnRuleResponse), (int)HttpStatusCode.OK)]
        public async Task<ConvertAmountByEarnRuleResponse> GetAmountByEarnRuleAsync(ConvertAmountByEarnRuleRequest request)
        {
            if (request.FromCurrency.ToLower() == request.ToCurrency.ToLower())
            {
                return new ConvertAmountByEarnRuleResponse
                {
                    ConversionSource = ConversionSource.EarnRule,
                    Amount = request.Amount,
                    UsedRate = 1
                };
            }

            AmountConvertByEarnRuleResponse result;

            try
            {
                result = await _conversionRateService.GetAmountByEarnRuleAsync(
                    request.EarnRuleId, request.CustomerId, request.FromCurrency, request.ToCurrency, request.Amount);
            }
            catch (EarnRuleNotFoundException e)
            {
                _log.Info(e.Message, context: request);

                return new ConvertAmountByEarnRuleResponse
                {
                    ErrorCode = EligibilityEngineErrors.EarnRuleNotFound,
                    ErrorMessage = e.Message
                };
            }
            catch (ConversionRateNotFoundException e)
            {
                _log.Info(e.Message, context: request);

                return new ConvertAmountByEarnRuleResponse
                {
                    ErrorCode = EligibilityEngineErrors.ConversionRateNotFound,
                    ErrorMessage = e.Message
                };
            }

            return new ConvertAmountByEarnRuleResponse
            {
                Amount = result.Amount,
                CurrencyCode = result.CurrencyCode,
                UsedRate = result.UsedRate,
                ConversionSource = Mapper.Map<ConversionSource>(result.ConversionSource),
                EarnRuleId = result.EarnRuleId
            };
        }

        /// <inheritdoc/>
        [HttpPost("spendRuleAmount")]
        [ProducesResponseType(typeof(ConvertAmountBySpendRuleResponse), (int)HttpStatusCode.OK)]
        public async Task<ConvertAmountBySpendRuleResponse> GetAmountBySpendRuleAsync(ConvertAmountBySpendRuleRequest request)
        {
            if (request.FromCurrency.ToLower() == request.ToCurrency.ToLower())
            {
                return new ConvertAmountBySpendRuleResponse
                {
                    ConversionSource = ConversionSource.BurnRule,
                    Amount = request.Amount,
                    UsedRate = 1
                };
            }

            AmountConvertBySpendRuleResponse result;

            try
            {
                result = await _conversionRateService.GetAmountBySpendRuleAsync(
                    request.SpendRuleId, request.CustomerId, request.FromCurrency, request.ToCurrency, request.Amount);
            }
            catch (BurnRuleNotFoundException e)
            {
                _log.Info(e.Message, context: request);

                return new ConvertAmountBySpendRuleResponse
                {
                    ErrorCode = EligibilityEngineErrors.SpendRuleNotFound,
                    ErrorMessage = e.Message
                };
            }
            catch (ConversionRateNotFoundException e)
            {
                _log.Info(e.Message, context: request);

                return new ConvertAmountBySpendRuleResponse
                {
                    ErrorCode = EligibilityEngineErrors.ConversionRateNotFound,
                    ErrorMessage = e.Message
                };
            }

            return new ConvertAmountBySpendRuleResponse
            {
                Amount = result.Amount,
                CurrencyCode = result.CurrencyCode,
                UsedRate = result.UsedRate,
                ConversionSource = Mapper.Map<ConversionSource>(result.ConversionSource),
                SpendRuleId = result.SpendRuleId
            };
        }

        /// <inheritdoc/>
        [HttpPost("conditionAmount")]
        [ProducesResponseType(typeof(ConvertAmountByConditionRequest), (int)HttpStatusCode.OK)]
        public async Task<ConvertAmountByConditionResponse> GetAmountByConditionAsync(ConvertAmountByConditionRequest request)
        {
            if (request.FromCurrency.ToLower() == request.ToCurrency.ToLower())
            {
                return new ConvertAmountByConditionResponse
                {
                    ConversionSource = ConversionSource.BurnRule,
                    Amount = request.Amount,
                    UsedRate = 1
                };
            }

            AmountByConditionResponse result;

            try
            {
                result = await _conversionRateService.GetAmountConditionAsync(
                    request.ConditionId,request.CustomerId, request.FromCurrency, request.ToCurrency, request.Amount);
            }
            catch (ConditionNotFoundException e)
            {
                _log.Info(e.Message, context: request);

                return new ConvertAmountByConditionResponse
                {
                    ErrorCode = EligibilityEngineErrors.ConditionNotFound,
                    ErrorMessage = e.Message
                };
            }
            catch (ConversionRateNotFoundException e)
            {
                _log.Info(e.Message, context: request);

                return new ConvertAmountByConditionResponse
                {
                    ErrorCode = EligibilityEngineErrors.ConversionRateNotFound,
                    ErrorMessage = e.Message
                };
            }

            return new ConvertAmountByConditionResponse
            {
                Amount = result.Amount,
                CurrencyCode = result.CurrencyCode,
                UsedRate = result.UsedRate,
                ConversionSource = Mapper.Map<ConversionSource>(result.ConversionSource),
                ConditionId = result.ConditionId
            };
        }

        /// <inheritdoc/>
        [HttpPost("conditionRate")]
        [ProducesResponseType(typeof(CurrencyRateByConditionResponse), (int)HttpStatusCode.OK)]
        public async Task<CurrencyRateByConditionResponse> GetCurrencyRateByConditionIdAsync(CurrencyRateByConditionRequest request)
        {
            if (request.FromCurrency.ToLower() == request.ToCurrency.ToLower())
            {
                return new CurrencyRateByConditionResponse
                {
                    ConversionSource = ConversionSource.Condition,
                    Rate = 1
                };
            }

            ConversionRateByConditionResponse result;

            try
            {
                result = await _conversionRateService.GetCurrencyRateByConditionIdAsync(
                    request.ConditionId, request.CustomerId, request.FromCurrency, request.ToCurrency);
            }
            catch (ConditionNotFoundException e)
            {
                _log.Info(e.Message, context: request);

                return new CurrencyRateByConditionResponse
                {
                    ErrorCode = EligibilityEngineErrors.ConditionNotFound,
                    ErrorMessage = e.Message
                };
            }
            catch (ConversionRateNotFoundException e)
            {
                _log.Info(e.Message, context: request);

                return new CurrencyRateByConditionResponse
                {
                    ErrorCode = EligibilityEngineErrors.ConversionRateNotFound,
                    ErrorMessage = e.Message
                };
            }

            return new CurrencyRateByConditionResponse
            {
                Rate = result.UsedRate,
                ConversionSource = Mapper.Map<ConversionSource>(result.ConversionSource),
                ConditionId = result.ConditionId
            };
        }
    }
}
