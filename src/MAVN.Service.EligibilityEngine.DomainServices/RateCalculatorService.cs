using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MAVN.Numerics;
using MAVN.Service.CurrencyConvertor.Client;
using MAVN.Service.CurrencyConvertor.Client.Models.Responses;
using MAVN.Service.EligibilityEngine.Domain.Exceptions;
using MAVN.Service.EligibilityEngine.Domain.Services;

namespace MAVN.Service.EligibilityEngine.DomainServices
{
    public class RateCalculatorService: IRateCalculatorService
    {
        private readonly ICurrencyConvertorClient _currencyConvertorClient;
        private readonly string _tokenCurrency;
        private readonly string _baseFiatCurrency;

        public RateCalculatorService(ICurrencyConvertorClient currencyConvertorClient, string tokenCurrency, string baseFiatCurrency)
        {
            _currencyConvertorClient = currencyConvertorClient ?? throw new ArgumentNullException(nameof(ICurrencyConvertorClient));
            _tokenCurrency = tokenCurrency ?? throw new ArgumentNullException(nameof(tokenCurrency));
            _baseFiatCurrency = baseFiatCurrency ?? throw new ArgumentNullException(nameof(baseFiatCurrency));
        }

        public async Task<Money18> CalculateConversionRate(decimal amountInCurrency, Money18 amountInTokens, string from, string to)
        {
            // Find whatever we convert from tokens to fiat or from fiat to tokens
            var withTokens = from == _tokenCurrency || to == _tokenCurrency;
            var forward = withTokens ? from == _tokenCurrency : from == _baseFiatCurrency;

            // If we are looking for the USD its enough to return the burn rule conversion rate
            var initialRate = (Money18) GetInitialRate(amountInCurrency, amountInTokens, from, to);

            if ((from == _tokenCurrency && to == _baseFiatCurrency) || (from == _baseFiatCurrency && to == _tokenCurrency))
            {
                return initialRate;
            }

            var startingCurrency = forward ? _baseFiatCurrency : from;
            var endingCurrency = forward ? to : _baseFiatCurrency;

            var rateToFind = await GetRate(startingCurrency, endingCurrency, forward);

            if (rateToFind == null)
            {
                throw new ConversionRateNotFoundException($"Currency rate from '{from}' to '{to}' cannot be found.");
            }

            rateToFind = rateToFind.Value * initialRate;

            return rateToFind.Value;
        }

        private Money18 GetInitialRate(decimal amountInCurrency, Money18 amountInTokens, string from, string to)
        {
            if (from == _tokenCurrency)
            {
                return amountInCurrency / amountInTokens;
            }

            if (to == _tokenCurrency)
            {
                return amountInTokens / amountInCurrency;
            }

            // When we don't have the token currency in the conversion
            // we just return 1 to keep other calculation right
            return 1;
        }

        private async Task<Money18?> GetRate(string fromCurrency, string toCurrency, bool forward)
        {
            var rates = await _currencyConvertorClient.CurrencyRates.GetAllAsync();

            var graph = BuildGraph(rates, forward);

            var startingNode = graph.TryGetValue(fromCurrency, out var nodes);

            if (!startingNode)
            {
                throw new ConversionRateNotFoundException($"Currency rate from '{fromCurrency}' to '{toCurrency}' cannot be found.");
            }

            var visited = new HashSet<string>();

            var rateToFind = FindCurrency(fromCurrency, toCurrency, graph, visited, forward);

            return rateToFind;
        }

        private Money18? FindCurrency(string node, string currencyToFind, Dictionary<string, List<CurrencyRate>> graph, HashSet<string> visited, bool forward)
        {
            if (!graph.ContainsKey(node))
            {
                return null;
            }

            foreach (var child in graph[node])
            {
                if (visited.Contains(child.Currency))
                {
                    continue;
                }

                if (child.Currency == currencyToFind)
                {
                    return forward
                        ? child.Rate
                        : 1 / child.Rate;
                }

                visited.Add(child.Currency);
                // In order to understand the recursion you first have to understand the recursion
                var rate = FindCurrency(child.Currency, currencyToFind, graph, visited, forward);

                if (rate != null)
                {
                    // Depending of the direction of the conversion
                    // we either multiply or divide
                    return forward
                        ? rate * child.Rate
                        : rate * (1 / child.Rate);
                }
            }

            return null;
        }

        private static Dictionary<string, List<CurrencyRate>> BuildGraph(IReadOnlyList<CurrencyRateModel> rates, bool forward)
        {
            string GetDirectedCurrency(string from, string to) => forward ? from : to;

            var graph = rates
                .GroupBy(
                    o => GetDirectedCurrency(o.BaseAsset, o.QuoteAsset))
                .ToDictionary(
                    o => GetDirectedCurrency(o.First().BaseAsset, o.First().QuoteAsset),
                    o => new List<CurrencyRate>());

            foreach (var rate in rates)
            {
                // depending on the direction we need to build the tree based on either quote or base
                var baseAsset = GetDirectedCurrency(rate.BaseAsset, rate.QuoteAsset);
                var quoteAsset = GetDirectedCurrency(rate.QuoteAsset, rate.BaseAsset);
                var children = graph[baseAsset];

                children.Add(new CurrencyRate
                {
                    Currency = quoteAsset,
                    Rate = rate.Rate
                });
            }

            return graph;
        }

        private class CurrencyRate
        {
            public string Currency { get; set; }

            public Money18 Rate { get; set; }
        }
    }
}
