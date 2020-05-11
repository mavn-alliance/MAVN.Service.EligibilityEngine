using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MAVN.Numerics;
using MAVN.Service.CurrencyConvertor.Client;
using MAVN.Service.CurrencyConvertor.Client.Models.Responses;
using MAVN.Service.EligibilityEngine.Domain.Exceptions;
using MAVN.Service.EligibilityEngine.DomainServices;
using Moq;
using Xunit;

namespace MAVN.Service.EligibilityEngine.Tests.DomainServices
{
    public class RateCalculatorServiceTests
    {
        private const string _token = "MVN";

        private readonly RateCalculatorService _service;
        private readonly Mock<ICurrencyConvertorClient> _currencyConvertorClientMock;

        public RateCalculatorServiceTests()
        {
            _currencyConvertorClientMock = new Mock<ICurrencyConvertorClient>(MockBehavior.Strict);
            _service = new RateCalculatorService(_currencyConvertorClientMock.Object, _token, "USD");
        }

        [Fact]
        public async Task WhenBaseRateIsPassed_NoGraphIsNeeded()
        {
            var currencyRateModels = new List<CurrencyRateModel>
            {
                new CurrencyRateModel { BaseAsset = _token, QuoteAsset = "USD", Rate = 2M },
                new CurrencyRateModel { BaseAsset = "USD", QuoteAsset = "AED", Rate = 3.67M },
                new CurrencyRateModel { BaseAsset = "USD", QuoteAsset = "NZD", Rate = 1.58M },
                new CurrencyRateModel { BaseAsset = "AED", QuoteAsset = "EUR", Rate = 0.25M },
                new CurrencyRateModel { BaseAsset = "EUR", QuoteAsset = "GBP", Rate = 0.89M },
                new CurrencyRateModel { BaseAsset = "EUR", QuoteAsset = "BGN", Rate = 1.96M }
            };

            // Arrange
            _currencyConvertorClientMock.Setup(c => c.CurrencyRates.GetAllAsync())
                .ReturnsAsync(() => currencyRateModels);

            // Act
            var forward = await _service.CalculateConversionRate(5, 100, _token, "USD");
            var backward = await _service.CalculateConversionRate(4, 100, "USD", _token);
            var forwardS = await _service.CalculateConversionRate(2, 1, _token, "USD");

            Money18 expectedF = 5M / 100M; // 0.05
            Money18 expectedB = 100M / 4M; // 25
            Money18 expectedS = 2M / 1M;   // 2

            // Assert
            Assert.Equal(expectedF, forward);   
            Assert.Equal(expectedB, backward);
            Assert.Equal(expectedS, forwardS);
        }

        [Fact]
        public async Task WhenToCurrencyIsNotBase_CalculateInGraph()
        {
            var currencyRateModels = new List<CurrencyRateModel>
            {
                new CurrencyRateModel { BaseAsset = _token, QuoteAsset = "USD", Rate = 2M },
                new CurrencyRateModel { BaseAsset = "USD", QuoteAsset = "AED", Rate = 3.67M },
                new CurrencyRateModel { BaseAsset = "USD", QuoteAsset = "NZD", Rate = 1.58M },
                new CurrencyRateModel { BaseAsset = "AED", QuoteAsset = "USD", Rate = 0.25M },
                new CurrencyRateModel { BaseAsset = "AED", QuoteAsset = "EUR", Rate = 0.25M },
                new CurrencyRateModel { BaseAsset = "EUR", QuoteAsset = "GBP", Rate = 0.89M },
                new CurrencyRateModel { BaseAsset = "EUR", QuoteAsset = "BGN", Rate = 1.96M },
            };

            // Arrange
            _currencyConvertorClientMock.Setup(c => c.CurrencyRates.GetAllAsync())
                .ReturnsAsync(() =>
                {
                    return currencyRateModels;
                });

            // Act
            var result = await _service.CalculateConversionRate(10, 2, _token, "BGN");

            Money18 expected = (Money18)currencyRateModels[1].Rate;
            expected *= (Money18)currencyRateModels[4].Rate;
            expected *= (Money18) currencyRateModels[6].Rate;
            expected *= (Money18)(10M / 2M);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WhenCurrencyNot_AvailableThrowException()
        {
            var currencyRateModels = new List<CurrencyRateModel>
            {
                new CurrencyRateModel {BaseAsset = _token, QuoteAsset = "USD", Rate = 2M},
                new CurrencyRateModel {BaseAsset = "USD", QuoteAsset = "AED", Rate = 3.67M},
                new CurrencyRateModel {BaseAsset = "USD", QuoteAsset = "NZD", Rate = 1.58M},
                new CurrencyRateModel {BaseAsset = "AED", QuoteAsset = "EUR", Rate = 0.25M},
                new CurrencyRateModel {BaseAsset = "EUR", QuoteAsset = "GBP", Rate = 0.89M},
                new CurrencyRateModel {BaseAsset = "EUR", QuoteAsset = "BGN", Rate = 1.96M}
            };

            // Arrange
            _currencyConvertorClientMock.Setup(c => c.CurrencyRates.GetAllAsync())
                .ReturnsAsync(() => { return currencyRateModels; });

            // Act
            // Assert
            await Assert.ThrowsAsync<ConversionRateNotFoundException>(() => 
                _service.CalculateConversionRate(1, 10, _token, "CAD"));

            await Assert.ThrowsAsync<ConversionRateNotFoundException>(() => 
                _service.CalculateConversionRate(1, 10, "CAD", "TRY"));
        }

        [Fact]
        public async Task WhenConversionIsBackwards_DivideTheRatesInGraph()
        {
            var currencyRateModels = new List<CurrencyRateModel>
            {
                new CurrencyRateModel { BaseAsset = _token, QuoteAsset = "USD", Rate = 2M },
                new CurrencyRateModel { BaseAsset = "USD", QuoteAsset = "AED", Rate = 3.67M },
                new CurrencyRateModel { BaseAsset = "USD", QuoteAsset = "NZD", Rate = 1.58M },
                new CurrencyRateModel { BaseAsset = "AED", QuoteAsset = "EUR", Rate = 0.25M },
                new CurrencyRateModel { BaseAsset = "EUR", QuoteAsset = "GBP", Rate = 0.89M },
                new CurrencyRateModel { BaseAsset = "EUR", QuoteAsset = "BGN", Rate = 1.96M }
            };

            // Arrange
            _currencyConvertorClientMock.Setup(c => c.CurrencyRates.GetAllAsync())
                .ReturnsAsync(() => currencyRateModels);

            // Act
            var result = await _service.CalculateConversionRate(10, 2, "BGN", _token);

            Money18 expected = (1 / (Money18)currencyRateModels[1].Rate);
            expected *= (1 / (Money18)currencyRateModels[3].Rate);
            expected *= (1 / (Money18)currencyRateModels[5].Rate);
            expected *= (2M / 10M);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WhenConversionIsFromBaseToNonBaseCurrency_DivideTheRatesInGraph()
        {
            var currencyRateModels = new List<CurrencyRateModel>
            {
                new CurrencyRateModel { BaseAsset = _token, QuoteAsset = "USD", Rate = 2M },
                new CurrencyRateModel { BaseAsset = "USD", QuoteAsset = "AED", Rate = 3.67M },
                new CurrencyRateModel { BaseAsset = "USD", QuoteAsset = "NZD", Rate = 1.58M },
                new CurrencyRateModel { BaseAsset = "AED", QuoteAsset = "EUR", Rate = 0.25M },
                new CurrencyRateModel { BaseAsset = "EUR", QuoteAsset = "GBP", Rate = 0.89M },
                new CurrencyRateModel { BaseAsset = "EUR", QuoteAsset = "BGN", Rate = 1.96M }
            };

            // Arrange
            _currencyConvertorClientMock.Setup(c => c.CurrencyRates.GetAllAsync())
                .ReturnsAsync(() => currencyRateModels);

            // Act
            var result = await _service.CalculateConversionRate(10, 2, "USD", "BGN");

            Money18 expected = (Money18)currencyRateModels[1].Rate;
            expected *= (Money18)currencyRateModels[3].Rate;
            expected *= (Money18)currencyRateModels[5].Rate;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WhenConversionIsMade_ConversionShoudBeValid()
        {
            var currencyRateModels = new List<CurrencyRateModel>
            {
                new CurrencyRateModel { BaseAsset = _token, QuoteAsset = "USD", Rate = 2M },
                new CurrencyRateModel { BaseAsset = "USD", QuoteAsset = "AED", Rate = 3.67M },
                new CurrencyRateModel { BaseAsset = "USD", QuoteAsset = "NZD", Rate = 1.58M },
                new CurrencyRateModel { BaseAsset = "AED", QuoteAsset = "USD", Rate = 0.25M },
                new CurrencyRateModel { BaseAsset = "AED", QuoteAsset = "EUR", Rate = 0.25M },
                new CurrencyRateModel { BaseAsset = "EUR", QuoteAsset = "GBP", Rate = 0.89M },
                new CurrencyRateModel { BaseAsset = "EUR", QuoteAsset = "BGN", Rate = 1.96M },
            };

            // Arrange
            _currencyConvertorClientMock.Setup(c => c.CurrencyRates.GetAllAsync())
                .ReturnsAsync(() =>
                {
                    return currencyRateModels;
                });

            // Act
            var initialAmount = Money18.Create(200);
            var forwardConversion = await _service.CalculateConversionRate(10, 2, _token, "BGN");
            var convertedAmount = forwardConversion * initialAmount;

            var backwardConversion = await _service.CalculateConversionRate(10, 2, "BGN", _token);
            var revertedAmount = backwardConversion * convertedAmount;

            Money18 expected = (Money18)currencyRateModels[1].Rate;
            expected *= (Money18)currencyRateModels[4].Rate;
            expected *= (Money18)currencyRateModels[6].Rate;
            expected *= (Money18)(10M / 2M);

            // Assert
            Assert.Equal(expected, forwardConversion);
            // Due to the amount of operations, 18 digits after the decimal point are exceeded
            // and thus rounded, which leads to 4 to 6 digits difference between the initial and 
            // the end value
            Assert.Equal(Money18.Round(initialAmount, 10), Money18.Round(revertedAmount, 10));
        }
    }
}
