using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Numerics;
using MAVN.Service.Campaign.Client;
using MAVN.Service.Campaign.Client.Models.BurnRule.Requests;
using MAVN.Service.Campaign.Client.Models.BurnRule.Responses;
using MAVN.Service.Campaign.Client.Models.Campaign.Responses;
using MAVN.Service.Campaign.Client.Models.Condition;
using MAVN.Service.Campaign.Client.Models.Enums;
using MAVN.Service.CurrencyConvertor.Client;
using MAVN.Service.CurrencyConvertor.Client.Models.Responses;
using MAVN.Service.EligibilityEngine.Domain.Exceptions;
using MAVN.Service.EligibilityEngine.Domain.Models;
using MAVN.Service.EligibilityEngine.Domain.Services;
using MAVN.Service.EligibilityEngine.DomainServices;
using MAVN.Service.PartnerManagement.Client;
using MAVN.Service.PartnerManagement.Client.Models.Partner;
using Moq;
using Xunit;

namespace MAVN.Service.EligibilityEngine.Tests.DomainServices
{
    public class ConversionRateServiceTests
    {
        private const string _from = "MVN";
        private const string _to = "USD";

        private readonly Mock<ICampaignClient> _campaignClientMock;
        private readonly Mock<IPartnerManagementClient> _partnerManagementClientMock;
        private readonly ConversionRateService _service;
        private readonly Mock<IRateCalculatorService> _rateCalculatorServiceMock;
        private readonly Mock<ICurrencyConvertorClient> _currencyConvertorClientMock;

        public ConversionRateServiceTests()
        {
            _campaignClientMock = new Mock<ICampaignClient>(MockBehavior.Strict);
            _partnerManagementClientMock = new Mock<IPartnerManagementClient>(MockBehavior.Strict);
            _currencyConvertorClientMock = new Mock<ICurrencyConvertorClient>(MockBehavior.Strict);
            _rateCalculatorServiceMock = new Mock<IRateCalculatorService>(MockBehavior.Strict);
            _service = new ConversionRateService(
                _campaignClientMock.Object, 
                _partnerManagementClientMock.Object,
                _currencyConvertorClientMock.Object,
                _rateCalculatorServiceMock.Object);
        }

        [Fact]
        public async Task WhenBurnRuleForPartnerFound_ReturnBurnRuleConversionRate()
        {
            // Arrange
            _campaignClientMock.Setup(m => m.BurnRules.GetAsync(It.IsAny<BurnRulePaginationRequest>()))
                .ReturnsAsync(() => new PaginatedBurnRuleListResponse
                {
                    BurnRules = new List<BurnRuleInfoResponse>
                    {
                        new BurnRuleInfoResponse {AmountInTokens = 1, AmountInCurrency = 2}
                    }
                });

            _rateCalculatorServiceMock.Setup(m => m.CalculateConversionRate(It.IsAny<decimal>(), It.IsAny<Money18>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => 15);

            // Act
            var result = await _service.GetOptimalCurrencyRateByPartnerAsync(Guid.NewGuid(), Guid.NewGuid(), _from, _to);

            // Assert
            Assert.Equal((Money18) 15, result.UsedRate);
            Assert.Equal(ConversionSource.BurnRule, result.ConversionSource);

            _rateCalculatorServiceMock.Verify(m => m.CalculateConversionRate(
                It.Is<decimal>(d => d == 2),
                It.Is<Money18>(l => l == 1),
                It.Is<string>(s => s == _from),
                It.Is<string>(s => s == _to)));
        }

        [Fact]
        public async Task WhenMultipleBurnRulesForPartnerFound_ReturnBestBurnRuleConversionRate()
        {
            // Arrange
            _campaignClientMock.Setup(m => m.BurnRules.GetAsync(It.IsAny<BurnRulePaginationRequest>()))
                .ReturnsAsync(() => new PaginatedBurnRuleListResponse
                {
                    BurnRules = new List<BurnRuleInfoResponse>
                    {
                        new BurnRuleInfoResponse {AmountInTokens = 1, AmountInCurrency = 2},
                        new BurnRuleInfoResponse {AmountInTokens = 10, AmountInCurrency = 15},
                        new BurnRuleInfoResponse {AmountInTokens = 1, AmountInCurrency = 5},
                    }
                });

            _rateCalculatorServiceMock.Setup(m => m.CalculateConversionRate(It.IsAny<decimal>(), It.IsAny<Money18>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => 25);

            // Act
            var result = await _service.GetOptimalCurrencyRateByPartnerAsync(Guid.NewGuid(), Guid.NewGuid(), _from, _to);

            // Assert
            Assert.Equal(25, result.UsedRate);
            Assert.Equal(ConversionSource.BurnRule, result.ConversionSource);

            _rateCalculatorServiceMock.Verify(m => m.CalculateConversionRate(
                It.Is<decimal>(d => d == 5),
                It.Is<Money18>(l => l == 1),
                It.Is<string>(s => s == _from),
                It.Is<string>(s => s == _to)));
        }

        [Fact]
        public async Task WhenMultipleBurnRulesForPartnerFound_ReturnBestBurnRuleAmountConversionRate()
        {
            // Arrange
            var burnRuleId = Guid.NewGuid();

            _campaignClientMock.Setup(m => m.BurnRules.GetAsync(It.IsAny<BurnRulePaginationRequest>()))
                .ReturnsAsync(() =>
                {
                    return new PaginatedBurnRuleListResponse
                    {
                        BurnRules = new List<BurnRuleInfoResponse>
                        {
                            new BurnRuleInfoResponse {AmountInTokens = 1, AmountInCurrency = 2},
                            new BurnRuleInfoResponse {AmountInTokens = 10, AmountInCurrency = 15},
                            new BurnRuleInfoResponse {AmountInTokens = 1, AmountInCurrency = 5, Id = burnRuleId},
                        }
                    };
                });

            _rateCalculatorServiceMock.Setup(m => m.CalculateConversionRate(It.IsAny<decimal>(), It.IsAny<Money18>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => 25);

            // Act
            var partnerId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var result = await _service.ConvertOptimalByPartnerAsync(partnerId, customerId, _from, _to, 100);

            // Assert
            Assert.Equal(25 * 100, result.Amount);
            Assert.Equal(25, result.UsedRate);
            Assert.Equal(ConversionSource.BurnRule, result.ConversionSource);
            Assert.Equal(burnRuleId, result.SpendRuleId);
            Assert.Equal(customerId, result.CustomerId);

            _rateCalculatorServiceMock.Verify(m => m.CalculateConversionRate(
                It.Is<decimal>(d => d == 5),
                It.Is<Money18>(l => l == 1),
                It.Is<string>(s => s == _from),
                It.Is<string>(s => s == _to)));
        }

        [Fact]
        public async Task WhenBurnRuleForPartnerNotFound_ReturnPartnerConversionRate()
        {
            // Arrange
            _campaignClientMock.Setup(m => m.BurnRules.GetAsync(It.IsAny<BurnRulePaginationRequest>()))
                .ReturnsAsync(() => new PaginatedBurnRuleListResponse
                {
                    BurnRules = new List<BurnRuleInfoResponse>()
                });

            _partnerManagementClientMock.Setup(m => m.Partners.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => new PartnerDetailsModel { AmountInTokens = 2, AmountInCurrency = 10 });

            _rateCalculatorServiceMock.Setup(m => m.CalculateConversionRate(It.IsAny<decimal>(), It.IsAny<Money18>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => 25);

            // Act
            var result = await _service.GetOptimalCurrencyRateByPartnerAsync(Guid.NewGuid(), Guid.NewGuid(), _from, _to);

            // Assert
            Assert.Equal((Money18) 25, result.UsedRate);
            Assert.Equal(ConversionSource.Partner, result.ConversionSource);

            _rateCalculatorServiceMock.Verify(m => m.CalculateConversionRate(
                It.Is<decimal>(d => d == 10),
                It.Is<Money18>(l => l == 2),
                It.Is<string>(s => s == _from),
                It.Is<string>(s => s == _to)));
        }

        [Fact]
        public async Task WhenBurnRuleForPartnerNotFoundAndPartnerUseGlobalRate_ReturnGlobalConversionRate()
        {
            // Arrange
            _campaignClientMock.Setup(m => m.BurnRules.GetAsync(It.IsAny<BurnRulePaginationRequest>()))
                .ReturnsAsync(() => new PaginatedBurnRuleListResponse
                {
                    BurnRules = new List<BurnRuleInfoResponse>()
                });

            _partnerManagementClientMock.Setup(m => m.Partners.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => new PartnerDetailsModel { UseGlobalCurrencyRate = true });

            _currencyConvertorClientMock.Setup(m => m.GlobalCurrencyRates.GetAsync())
                .ReturnsAsync(() => new GlobalCurrencyRateModel { AmountInTokens = 25, AmountInCurrency = 7 });

            _rateCalculatorServiceMock.Setup(m => m.CalculateConversionRate(It.IsAny<decimal>(), It.IsAny<Money18>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => 25);

            // Act
            var result = await _service.GetOptimalCurrencyRateByPartnerAsync(Guid.NewGuid(), Guid.NewGuid(), _from, _to);

            // Assert
            Assert.Equal((Money18)25, result.UsedRate);
            Assert.Equal(ConversionSource.Global, result.ConversionSource);

            _rateCalculatorServiceMock.Verify(m => m.CalculateConversionRate(
                It.Is<decimal>(d => d == 7),
                It.Is<Money18>(l => l == 25),
                It.Is<string>(s => s == _from),
                It.Is<string>(s => s == _to)));
        }

        [Fact]
        public async Task WhenBurnRuleForPartnerNotFoundAndNoPartnerFound_ThrowException()
        {
            // Arrange
            _campaignClientMock.Setup(m => m.BurnRules.GetAsync(It.IsAny<BurnRulePaginationRequest>()))
                .ReturnsAsync(() => new PaginatedBurnRuleListResponse
                {
                    BurnRules = new List<BurnRuleInfoResponse>()
                });

            _partnerManagementClientMock.Setup(m => m.Partners.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => null);

            // Act
            // Assert
            await Assert.ThrowsAsync<PartnerNotFoundException>(async () =>
            {
                await _service.GetOptimalCurrencyRateByPartnerAsync(Guid.NewGuid(), Guid.NewGuid(), _from, _to);
            });
        }

        [Fact]
        public async Task WhenEarnRuleFound_ReturnPerformConversionRateCalculation()
        {
            // Arrange
            var earnRuleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            _campaignClientMock.Setup(m => m.Campaigns.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(() => new CampaignDetailResponseModel
                {
                    AmountInTokens = 100,
                    AmountInCurrency = 1,
                    RewardType = RewardType.ConversionRate
                });

            _rateCalculatorServiceMock.Setup(m => m.CalculateConversionRate(It.IsAny<decimal>(), It.IsAny<Money18>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => 25);

            // Act
            var result = await _service.GetCurrencyRateByEarnRuleIdAsync(earnRuleId, customerId, _from, _to);

            // Assert
            Assert.Equal((Money18)25, result.UsedRate);
            Assert.Equal(ConversionSource.EarnRule, result.ConversionSource);
            Assert.Equal(earnRuleId, result.EarnRuleId);
            Assert.Equal(customerId, result.CustomerId);
        }

        [Fact]
        public async Task WhenEarnRuleNotFoundFound_ThrowException()
        {
            // Arrange
            _campaignClientMock.Setup(m => m.Campaigns.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            // Act
            // Assert
            await Assert.ThrowsAsync<EarnRuleNotFoundException>(async () =>
            {
                await _service.GetCurrencyRateByEarnRuleIdAsync(Guid.NewGuid(), Guid.NewGuid(), _from, _to);
            });
        }

        [Fact]
        public async Task WhenEarnRuleUsePartnerCurrencyRateAndOnePartner_ReturnPartnerCurrencyRate()
        {
            // Arrange
            var earnRuleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            _campaignClientMock.Setup(m => m.Campaigns.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(() => new CampaignDetailResponseModel
                {
                    AmountInTokens = 100,
                    AmountInCurrency = 1,
                    RewardType = RewardType.ConversionRate,
                    UsePartnerCurrencyRate = true,
                    Conditions = new List<ConditionModel>
                    {
                        new ConditionModel { PartnerIds = new []{ Guid.NewGuid() } }
                    }
                });

            _partnerManagementClientMock.Setup(m => m.Partners.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => new PartnerDetailsModel { AmountInTokens = 2, AmountInCurrency = 10 });

            _rateCalculatorServiceMock.Setup(m =>
                m.CalculateConversionRate(It.IsAny<decimal>(), It.IsAny<Money18>(), It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(() => (Money18)25);

            // Act
            var result = await _service.GetCurrencyRateByEarnRuleIdAsync(earnRuleId, customerId, _from, _to);

            // Assert
            Assert.Equal((Money18)25, result.UsedRate);
            Assert.Equal(ConversionSource.Partner, result.ConversionSource);
            Assert.Equal(earnRuleId, result.EarnRuleId);
            Assert.Equal(customerId, result.CustomerId);

            _rateCalculatorServiceMock.Verify(r => 
                r.CalculateConversionRate(
                    It.Is<decimal>(d => d == 10), 
                    It.Is<Money18>(d => d == 2), 
                    It.Is<string>(d => d == _from), 
                    It.Is<string>(d => d == _to)));
        }

        [Fact]
        public async Task WhenEarnRuleUsePartnerCurrencyRateAndOnePartnerInTwoConditions_ReturnPartnerCurrencyRate()
        {
            // Arrange
            var earnRuleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var partnerId = Guid.NewGuid();

            _campaignClientMock.Setup(m => m.Campaigns.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(() => new CampaignDetailResponseModel
                {
                    AmountInTokens = 100,
                    AmountInCurrency = 1,
                    RewardType = RewardType.ConversionRate,
                    UsePartnerCurrencyRate = true,
                    Conditions = new List<ConditionModel>
                    {
                        new ConditionModel(),
                        new ConditionModel { PartnerIds = new []{ partnerId } },
                        new ConditionModel { PartnerIds = new []{ partnerId } }
                    }
                });

            _partnerManagementClientMock.Setup(m => m.Partners.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => new PartnerDetailsModel { AmountInTokens = 2, AmountInCurrency = 10 });

            _rateCalculatorServiceMock.Setup(m =>
                m.CalculateConversionRate(It.IsAny<decimal>(), It.IsAny<Money18>(), It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(() => (Money18)25);

            // Act
            var result = await _service.GetCurrencyRateByEarnRuleIdAsync(earnRuleId, customerId, _from, _to);

            // Assert
            Assert.Equal((Money18)25, result.UsedRate);
            Assert.Equal(ConversionSource.Partner, result.ConversionSource);
            Assert.Equal(earnRuleId, result.EarnRuleId);
            Assert.Equal(customerId, result.CustomerId);

            _rateCalculatorServiceMock.Verify(r =>
                r.CalculateConversionRate(
                    It.Is<decimal>(d => d == 10),
                    It.Is<Money18>(d => d == 2),
                    It.Is<string>(d => d == _from),
                    It.Is<string>(d => d == _to)));
        }

        [Fact]
        public async Task WhenEarnRuleUsePartnerCurrencyRateAndNoPartner_ReturnGlobalCurrencyRate()
        {
            // Arrange
            var earnRuleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            _campaignClientMock.Setup(m => m.Campaigns.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(() => new CampaignDetailResponseModel
                {
                    AmountInTokens = 100,
                    AmountInCurrency = 1,
                    RewardType = RewardType.ConversionRate,
                    UsePartnerCurrencyRate = true,
                    Conditions = new List<ConditionModel>
                    {
                        new ConditionModel()
                    }
                });

            _currencyConvertorClientMock.Setup(m => m.GlobalCurrencyRates.GetAsync())
                .ReturnsAsync(() => new GlobalCurrencyRateModel { AmountInCurrency = 20, AmountInTokens = 25 });

            _rateCalculatorServiceMock.Setup(m =>
                m.CalculateConversionRate(It.IsAny<decimal>(), It.IsAny<Money18>(), It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(() => (Money18)35);

            // Act
            var result = await _service.GetCurrencyRateByEarnRuleIdAsync(earnRuleId, customerId, _from, _to);

            // Assert
            Assert.Equal((Money18)35, result.UsedRate);
            Assert.Equal(ConversionSource.Global, result.ConversionSource);
            Assert.Equal(earnRuleId, result.EarnRuleId);
            Assert.Equal(customerId, result.CustomerId);

            _rateCalculatorServiceMock.Verify(r =>
                r.CalculateConversionRate(
                    It.Is<decimal>(d => d == 20),
                    It.Is<Money18>(d => d == 25),
                    It.Is<string>(d => d == _from),
                    It.Is<string>(d => d == _to)));
        }

        [Fact]
        public async Task WhenEarnRuleUsePartnerCurrencyRateAndMultiplePartners_ReturnGlobalCurrencyRate()
        {
            // Arrange
            var earnRuleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            _campaignClientMock.Setup(m => m.Campaigns.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(() => new CampaignDetailResponseModel
                {
                    AmountInTokens = 100,
                    AmountInCurrency = 1,
                    RewardType = RewardType.ConversionRate,
                    UsePartnerCurrencyRate = true,
                    Conditions = new List<ConditionModel>
                    {
                        new ConditionModel {PartnerIds = new []{ Guid.NewGuid() }},
                        new ConditionModel {PartnerIds = new []{ Guid.NewGuid() }}
                    }
                });

            _currencyConvertorClientMock.Setup(m => m.GlobalCurrencyRates.GetAsync())
                .ReturnsAsync(() => new GlobalCurrencyRateModel { AmountInCurrency = 230, AmountInTokens = 335 });

            _rateCalculatorServiceMock.Setup(m =>
                    m.CalculateConversionRate(It.IsAny<decimal>(), It.IsAny<Money18>(), It.IsAny<string>(),
                        It.IsAny<string>()))
                .ReturnsAsync(() => (Money18)45);

            // Act
            var result = await _service.GetCurrencyRateByEarnRuleIdAsync(earnRuleId, customerId, _from, _to);

            // Assert
            Assert.Equal((Money18)45, result.UsedRate);
            Assert.Equal(ConversionSource.Global, result.ConversionSource);
            Assert.Equal(earnRuleId, result.EarnRuleId);
            Assert.Equal(customerId, result.CustomerId);

            _rateCalculatorServiceMock.Verify(r =>
                r.CalculateConversionRate(
                    It.Is<decimal>(d => d == 230),
                    It.Is<Money18>(d => d == 335),
                    It.Is<string>(d => d == _from),
                    It.Is<string>(d => d == _to)));
        }

        [Fact]
        public async Task WhenSpendRuleUsePartnerCurrencyRateAndOnePartner_ReturnPartnerCurrencyRate()
        {
            // Arrange
            var spendRuleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            _campaignClientMock.Setup(m => m.BurnRules.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => new BurnRuleResponse
                {
                    UsePartnerCurrencyRate = true,
                    AmountInTokens = 100,
                    AmountInCurrency = 1,
                    PartnerIds = new[]{ Guid.NewGuid() }
                });

            _partnerManagementClientMock.Setup(m => m.Partners.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => new PartnerDetailsModel { AmountInTokens = 2, AmountInCurrency = 10 });

            _rateCalculatorServiceMock.Setup(m =>
                m.CalculateConversionRate(It.IsAny<decimal>(), It.IsAny<Money18>(), It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(() => (Money18)25);

            // Act
            var result = await _service.GetCurrencyRateBySpendRuleIdAsync(spendRuleId, customerId, _from, _to);

            // Assert
            Assert.Equal((Money18)25, result.UsedRate);
            Assert.Equal(ConversionSource.Partner, result.ConversionSource);
            Assert.Equal(spendRuleId, result.SpendRuleId);
            Assert.Equal(customerId, result.CustomerId);

            _rateCalculatorServiceMock.Verify(r =>
                r.CalculateConversionRate(
                    It.Is<decimal>(d => d == 10),
                    It.Is<Money18>(d => d == 2),
                    It.Is<string>(d => d == _from),
                    It.Is<string>(d => d == _to)));
        }

        [Fact]
        public async Task WhenSpendRuleUsePartnerCurrencyRateAndOnePartnerWithGlobalConversion_ReturnPartnerCurrencyRate()
        {
            // Arrange
            var spendRuleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            _campaignClientMock.Setup(m => m.BurnRules.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => new BurnRuleResponse
                {
                    UsePartnerCurrencyRate = true,
                    AmountInTokens = 100,
                    AmountInCurrency = 1,
                    PartnerIds = new[] { Guid.NewGuid() }
                });

            _partnerManagementClientMock.Setup(m => m.Partners.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => new PartnerDetailsModel
                {
                    AmountInTokens = 2,
                    AmountInCurrency = 10,
                    UseGlobalCurrencyRate = true
                });

            _currencyConvertorClientMock.Setup(m => m.GlobalCurrencyRates.GetAsync())
                .ReturnsAsync(() => new GlobalCurrencyRateModel { AmountInTokens = 25, AmountInCurrency = 7 });

            _rateCalculatorServiceMock.Setup(m =>
                    m.CalculateConversionRate(It.IsAny<decimal>(), It.IsAny<Money18>(), It.IsAny<string>(),
                        It.IsAny<string>()))
                .ReturnsAsync(() => (Money18)25);

            // Act
            var result = await _service.GetCurrencyRateBySpendRuleIdAsync(spendRuleId, customerId, _from, _to);

            // Assert
            Assert.Equal((Money18)25, result.UsedRate);
            Assert.Equal(ConversionSource.Global, result.ConversionSource);
            Assert.Equal(spendRuleId, result.SpendRuleId);
            Assert.Equal(customerId, result.CustomerId);

            _rateCalculatorServiceMock.Verify(r =>
                r.CalculateConversionRate(
                    It.Is<decimal>(d => d == 7),
                    It.Is<Money18>(d => d == 25),
                    It.Is<string>(d => d == _from),
                    It.Is<string>(d => d == _to)));
        }

        [Fact]
        public async Task WhenSpendRuleUsePartnerCurrencyRateAndTwoDuplicatePartners_ReturnPartnerCurrencyRate()
        {
            // Arrange
            var spendRuleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var partnerId = Guid.NewGuid();

            _campaignClientMock.Setup(m => m.BurnRules.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => new BurnRuleResponse
                {
                    UsePartnerCurrencyRate = true,
                    AmountInTokens = 100,
                    AmountInCurrency = 1,
                    PartnerIds = new[] { 
                        partnerId,
                        partnerId
                    }
                });

            _partnerManagementClientMock.Setup(m => m.Partners.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => new PartnerDetailsModel { AmountInTokens = 2, AmountInCurrency = 10 });

            _rateCalculatorServiceMock.Setup(m =>
                m.CalculateConversionRate(It.IsAny<decimal>(), It.IsAny<Money18>(), It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(() => (Money18)25);

            // Act
            var result = await _service.GetCurrencyRateBySpendRuleIdAsync(spendRuleId, customerId, _from, _to);

            // Assert
            Assert.Equal((Money18)25, result.UsedRate);
            Assert.Equal(ConversionSource.Partner, result.ConversionSource);
            Assert.Equal(spendRuleId, result.SpendRuleId);
            Assert.Equal(customerId, result.CustomerId);

            _rateCalculatorServiceMock.Verify(r =>
                r.CalculateConversionRate(
                    It.Is<decimal>(d => d == 10),
                    It.Is<Money18>(d => d == 2),
                    It.Is<string>(d => d == _from),
                    It.Is<string>(d => d == _to)));
        }

        [Fact]
        public async Task WhenSpendRuleUsePartnerCurrencyRateAndNoPartner_ReturnGlobalCurrencyRate()
        {
            // Arrange
            var spendRuleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            _campaignClientMock.Setup(m => m.BurnRules.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => new BurnRuleResponse
                {
                    UsePartnerCurrencyRate = true,
                    AmountInTokens = 100,
                    AmountInCurrency = 1
                });

            _currencyConvertorClientMock.Setup(m => m.GlobalCurrencyRates.GetAsync())
                .ReturnsAsync(() => new GlobalCurrencyRateModel { AmountInTokens = 25, AmountInCurrency = 7 });

            _rateCalculatorServiceMock.Setup(m =>
                m.CalculateConversionRate(It.IsAny<decimal>(), It.IsAny<Money18>(), It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(() => (Money18)35);

            // Act
            var result = await _service.GetCurrencyRateBySpendRuleIdAsync(spendRuleId, customerId, _from, _to);

            // Assert
            Assert.Equal((Money18)35, result.UsedRate);
            Assert.Equal(ConversionSource.Global, result.ConversionSource);
            Assert.Equal(spendRuleId, result.SpendRuleId);
            Assert.Equal(customerId, result.CustomerId);

            _rateCalculatorServiceMock.Verify(r =>
                r.CalculateConversionRate(
                    It.Is<decimal>(d => d == 7),
                    It.Is<Money18>(d => d == 25),
                    It.Is<string>(d => d == _from),
                    It.Is<string>(d => d == _to)));
        }

        [Fact]
        public async Task WhenSpendRuleUsePartnerCurrencyRateAndMultiplePartners_ReturnGlobalCurrencyRate()
        {
            // Arrange
            var spendRuleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            _campaignClientMock.Setup(m => m.BurnRules.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => new BurnRuleResponse
                {
                    AmountInTokens = 100,
                    AmountInCurrency = 1,
                    UsePartnerCurrencyRate = true,
                    PartnerIds = new[] {
                        Guid.NewGuid(),
                        Guid.NewGuid()
                    }
                });

            _currencyConvertorClientMock.Setup(m => m.GlobalCurrencyRates.GetAsync())
                .ReturnsAsync(() => new GlobalCurrencyRateModel { AmountInCurrency = 230, AmountInTokens = 335 });

            _rateCalculatorServiceMock.Setup(m =>
                    m.CalculateConversionRate(It.IsAny<decimal>(), It.IsAny<Money18>(), It.IsAny<string>(),
                        It.IsAny<string>()))
                .ReturnsAsync(() => (Money18)45);

            // Act
            var result = await _service.GetCurrencyRateBySpendRuleIdAsync(spendRuleId, customerId, _from, _to);

            // Assert
            Assert.Equal((Money18)45, result.UsedRate);
            Assert.Equal(ConversionSource.Global, result.ConversionSource);
            Assert.Equal(spendRuleId, result.SpendRuleId);
            Assert.Equal(customerId, result.CustomerId);

            _rateCalculatorServiceMock.Verify(r =>
                r.CalculateConversionRate(
                    It.Is<decimal>(d => d == 230),
                    It.Is<Money18>(d => d == 335),
                    It.Is<string>(d => d == _from),
                    It.Is<string>(d => d == _to)));
        }

        [Fact]
        public async Task WhenSpendRuleFound_ReturnPerformConversionRateCalculation()
        {
            // Arrange
            var spendRuleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            _campaignClientMock.Setup(m => m.BurnRules.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => new BurnRuleResponse { AmountInTokens = 100, AmountInCurrency = 1 });

            _rateCalculatorServiceMock.Setup(m => m.CalculateConversionRate(It.IsAny<decimal>(), It.IsAny<Money18>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => 25);

            // Act
            var result = await _service.GetCurrencyRateBySpendRuleIdAsync(spendRuleId, customerId, _from, _to);

            // Assert
            Assert.Equal((Money18)25, result.UsedRate);
            Assert.Equal(ConversionSource.BurnRule, result.ConversionSource);
            Assert.Equal(spendRuleId, result.SpendRuleId);
            Assert.Equal(customerId, result.CustomerId);
        }

        [Fact]
        public async Task WhenBurnRuleNotFoundFound_ThrowException()
        {
            // Arrange
            _campaignClientMock.Setup(m => m.BurnRules.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => null);

            // Act
            // Assert
            await Assert.ThrowsAsync<BurnRuleNotFoundException>(async () =>
            {
                await _service.GetCurrencyRateBySpendRuleIdAsync(Guid.NewGuid(), Guid.NewGuid(), _from, _to);
            });
        }

        [Fact]
        public async Task WhenEarnRuleFound_ReturnPerformAmountCalculation()
        {
            // Arrange
            var earnRuleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            _campaignClientMock.Setup(m => m.Campaigns.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(() => new CampaignDetailResponseModel
                {
                    AmountInTokens = 100,
                    AmountInCurrency = 1,
                    RewardType = RewardType.ConversionRate
                });

            _rateCalculatorServiceMock.Setup(m => m.CalculateConversionRate(It.IsAny<decimal>(), It.IsAny<Money18>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => 25);

            // Act
            var result = await _service.GetAmountByEarnRuleAsync(earnRuleId, customerId, _from, _to, 100);

            // Assert
            Assert.Equal((Money18)25 * 100, result.Amount);
            Assert.Equal((Money18)25, result.UsedRate);
            Assert.Equal(ConversionSource.EarnRule, result.ConversionSource);
            Assert.Equal(earnRuleId, result.EarnRuleId);
            Assert.Equal(customerId, result.CustomerId);
            Assert.Equal(_to, result.CurrencyCode);
        }

        [Fact]
        public async Task WhenEarnRuleNotFoundForAmountFound_ThrowException()
        {
            // Arrange
            _campaignClientMock.Setup(m => m.Campaigns.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            // Act
            // Assert
            await Assert.ThrowsAsync<EarnRuleNotFoundException>(async () =>
            {
                await _service.GetAmountByEarnRuleAsync(Guid.NewGuid(), Guid.NewGuid(), _from, _to, 100);
            });
        }

        [Fact]
        public async Task WhenSpendRuleFound_ReturnPerformAmountCalculation()
        {
            // Arrange
            var spendRuleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            _campaignClientMock.Setup(m => m.BurnRules.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => new BurnRuleResponse { AmountInTokens = 100, AmountInCurrency = 1 });

            _rateCalculatorServiceMock.Setup(m => m.CalculateConversionRate(It.IsAny<decimal>(), It.IsAny<Money18>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => 25);

            // Act
            var result = await _service.GetAmountBySpendRuleAsync(spendRuleId, customerId, _from, _to, 100);

            // Assert
            Assert.Equal((Money18)25 * 100, result.Amount);
            Assert.Equal((Money18)25, result.UsedRate);
            Assert.Equal(ConversionSource.BurnRule, result.ConversionSource);
            Assert.Equal(spendRuleId, result.SpendRuleId);
            Assert.Equal(customerId, result.CustomerId);
        }

        [Fact]
        public async Task WhenBurnRuleNotFoundForAmountFound_ThrowException()
        {
            // Arrange
            _campaignClientMock.Setup(m => m.BurnRules.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => null);

            // Act
            // Assert
            await Assert.ThrowsAsync<BurnRuleNotFoundException>(async () =>
            {
                await _service.GetAmountBySpendRuleAsync(Guid.NewGuid(), Guid.NewGuid(), _from, _to, 100);
            });
        }
    }
}
