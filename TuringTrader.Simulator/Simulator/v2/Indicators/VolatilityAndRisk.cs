﻿//==============================================================================
// Project:     TuringTrader, simulator core v2
// Name:        VolatilityAndRisk
// Description: Volatility and risk indicators.
// History:     2022xi02, FUB, created
//------------------------------------------------------------------------------
// Copyright:   (c) 2011-2023, Bertram Enterprises LLC dba TuringTrader.
//              https://www.turingtrader.org
// License:     This file is part of TuringTrader, an open-source backtesting
//              engine/ trading simulator.
//              TuringTrader is free software: you can redistribute it and/or 
//              modify it under the terms of the GNU Affero General Public 
//              License as published by the Free Software Foundation, either 
//              version 3 of the License, or (at your option) any later version.
//              TuringTrader is distributed in the hope that it will be useful,
//              but WITHOUT ANY WARRANTY; without even the implied warranty of
//              MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//              GNU Affero General Public License for more details.
//              You should have received a copy of the GNU Affero General Public
//              License along with TuringTrader. If not, see 
//              https://www.gnu.org/licenses/agpl-3.0.
//==============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TuringTrader.SimulatorV2.Indicators
{
    /// <summary>
    /// Collection of volatility-related indictors.
    /// </summary>
    public static class VolatilityAndRisk
    {
        #region StandardDeviation
        /// <summary>
        /// Calculate historical standard deviation.
        /// </summary>
        /// <param name="series">input series</param>
        /// <param name="n">length of calculation window</param>
        /// <returns>standard deviation time series</returns>
        public static TimeSeriesFloat StandardDeviation(this TimeSeriesFloat series, int n = 10)
        {
            var name = string.Format("{0}.StandardDeviation({1})", series.Name, n);

            return series.Owner.ObjectCache.Fetch(
                name,
                () =>
                {
                    var data = series.Owner.DataCache.Fetch(
                        name,
                        () => Task.Run(() =>
                        {
                            var src = series.Data;
                            var dst = new List<BarType<double>>();

                            for (int idx = 0; idx < src.Count; idx++)
                            {
                                // see https://en.wikipedia.org/wiki/Algorithms_for_calculating_variance

                                var sum = Enumerable.Range(0, n)
                                    .Sum(t => src[Math.Max(0, idx - t)].Value);
                                var sum2 = Enumerable.Range(0, n)
                                    .Sum(t => Math.Pow(src[Math.Max(0, idx - t)].Value, 2.0));
                                var var = (sum2 - sum * sum / n) / (n - 1);

                                dst.Add(new BarType<double>(
                                    src[idx].Date, Math.Sqrt(var)));
                            }

                            return dst;
                        }));

                    return new TimeSeriesFloat(series.Owner, name, data);
                });
        }
        #endregion
        // SemiDeviation
        #region Volatility
        /// <summary>
        /// Calculate historical volatility, based on log-returns.
        /// </summary>
        /// <param name="series"></param>
        /// <param name="n"></param>
        /// <returns>volatility time series</returns>
        public static TimeSeriesFloat Volatility(this TimeSeriesFloat series, int n = 10)
            => series.LogReturn().StandardDeviation(n);
        #endregion
        #region TrueRange
        /// <summary>
        /// Calculate True Range, non averaged, as described here:
        /// <see href="https://en.wikipedia.org/wiki/Average_true_range"/>.
        /// </summary>
        public static TimeSeriesFloat TrueRange(this TimeSeriesAsset series)
        {
            var name = string.Format("{0}.TrueRange", series.Name);

            return series.Owner.ObjectCache.Fetch(
                name,
                () =>
                {
                    var data = series.Owner.DataCache.Fetch(
                        name,
                        () => Task.Run(() =>
                        {
                            var src = series.Data;
                            var dst = new List<BarType<double>>();

                            for (int idx = 0; idx < src.Count; idx++)
                            {
                                var idxPrev = Math.Max(0, idx - 1);
                                var high = Math.Max(src[idxPrev].Value.Close, src[idx].Value.High);
                                var low = Math.Min(src[idxPrev].Value.Close, src[idx].Value.Low);
                                dst.Add(new BarType<double>(src[idx].Date, high - low));
                            }

                            return dst;
                        }));

                    return new TimeSeriesFloat(series.Owner, name, data);
                });
        }
        #endregion
        #region AverageTrueRange
        /// <summary>
        /// Calculate Averaged True Range, as described here:
        /// <see href="https://en.wikipedia.org/wiki/Average_true_range"/>.
        /// </summary>
        /// <param name="series"></param>
        /// <param name="n"></param>
        /// <returns>average true range time series</returns>
        public static TimeSeriesFloat AverageTrueRange(this TimeSeriesAsset series, int n)
            => series.TrueRange().SMA(n);
        #endregion

        #region Drawdown
        /// <summary>
        /// Calculate current drawdown from  highest high in period.
        /// </summary>
        /// <param name="series">input series</param>
        /// <param name="period">period for highest high</param>
        /// <returns>drawdown time series</returns>
        public static TimeSeriesFloat Drawdown(this TimeSeriesFloat series, int period)
        {
            var name = string.Format("{0}.Drawdown({1})", series.Name, period);

            return series.Owner.ObjectCache.Fetch(
                name,
                () =>
                {
                    var data = series.Owner.DataCache.Fetch(
                        name,
                        () => Task.Run(() =>
                        {
                            var src = series.Data;
                            var dst = new List<BarType<double>>();

                            for (int idx = 0; idx < src.Count; idx++)
                            {
                                var highestHigh = Enumerable.Range(0, period)
                                    .Max(t => src[Math.Max(0, idx - t)].Value);

                                dst.Add(new BarType<double>(src[idx].Date, 1.0 - src[idx].Value / highestHigh));
                            }

                            return dst;
                        }));

                    return new TimeSeriesFloat(series.Owner, name, data);
                });
        }
        #endregion
        #region UlcerIndex
        /// <summary>
        /// Calculate Ulcer Index over period.
        /// </summary>
        /// <param name="series">input series</param>
        /// <param name="period">period for highest high</param>
        /// <returns>ulcer index time series</returns>
        public static TimeSeriesFloat UlcerIndex(this TimeSeriesFloat series, int period)
            => series.Drawdown(period).Square().SMA(period).Sqrt();
        #endregion

        #region BollingerBands
        public class BollingerT
        {
            /// <summary>
            /// Middle line, calculated as moving average.
            /// </summary>
            public readonly TimeSeriesFloat Middle;
            /// <summary>
            /// Upper line, calculated as middle line plus standard deviation.
            /// </summary>
            public readonly TimeSeriesFloat Upper;
            /// <summary>
            /// Lower line, calculated as middle line minus standard deviation.
            /// </summary>
            public readonly TimeSeriesFloat Lower;
            //public readonly TimeSeriesFloat PercentB;

            /// <summary>
            /// Create new container for Bollinger Band
            /// </summary>
            /// <param name="middle"></param>
            /// <param name="upper"></param>
            /// <param name="lower"></param>
            public BollingerT(TimeSeriesFloat middle, TimeSeriesFloat upper, TimeSeriesFloat lower)
            {
                Middle = middle;
                Upper = upper;
                Lower = lower;
            }
        }
        /// <summary>
        /// Calculate Bollinger Bands, as described here:
        /// <see href="https://en.wikipedia.org/wiki/Bollinger_Bands"/>.
        /// </summary>
        /// <param name="series">input series</param>
        /// <param name="n">calculation period</param>
        /// <param name="stdev">width in standard deviations</param>
        /// <returns>Bollinger Band container of time series</returns>
        public static BollingerT BollingerBands(this TimeSeriesFloat series, int n = 20, double stdev = 2.0)
        {
            var name = string.Format("{0}.BollingerBands({1},{2})", series.Name, n, stdev);

            return series.Owner.ObjectCache.Fetch(
                name,
                () =>
                {
                    /*var data = series.Owner.DataCache.Fetch(
                        name,
                        () => Task.Run(() =>
                        {
                            var src = series.Data;
                            var dst = new List<BarType<double>>();
                            ...
                            return dst;
                        }));*/

                    var std = series.StandardDeviation(n).Mul(stdev);
                    var mid = series.SMA(n);
                    var upper = mid.Add(std);
                    var lower = mid.Sub(std);

                    return new BollingerT(mid, upper, lower);
                });
        }
        #endregion

        #region ValueAtRisk
        /// <summary>
        /// Calculate Value-at-Risk at given percentile. The result is 
        /// expressed as a fraction of the asset's value. A value of 
        /// 0.1 equates to 10% risk.
        /// </summary>
        /// <param name="series">input time series</param>
        /// <param name="days"># of days to sample</param>
        /// <param name="percentile">percentile of value at risk. A value of 0.99 represents the 99-th percentile</param>
        /// <returns>value at risk, expresed as a fraction of the asset value. A value of 0.1 equates to 10% risk</returns>
        public static TimeSeriesFloat ValueAtRisk(this TimeSeriesAsset series, int days = 21, double percentile = 0.95)
        {
            var name = string.Format("{0}.ValueAtRisk({1}, {2}%)", series.Name, days, percentile);

            return series.Owner.ObjectCache.Fetch(
                name,
                () =>
                {
                    var data = series.Owner.DataCache.Fetch(
                        name,
                        () => Task.Run(() =>
                        {
                            var RESOLUTION = 1000; // TODO: tie this to percentile
                            var upsamplingSteps = (int)Math.Ceiling(Math.Log(RESOLUTION) / Math.Log(days));

                            var src = series.Close.LogReturn().Data;
                            var dst = new List<BarType<double>>();

                            for (int idx = 0; idx < src.Count; idx++)
                            {
                                //----- create return distribution with required resolution
                                var upsampledDistribution = new List<double> { 0.0 };
                                for (int step = 0; step < upsamplingSteps; step++)
                                {
                                    var nextStep = new List<double>();
                                    foreach (var s1 in upsampledDistribution)
                                        for (var s2 = 0; s2 < days; s2++)
                                            nextStep.Add(s1 + src[Math.Max(0, idx - s2)].Value);
                                    upsampledDistribution = nextStep;
                                }

                                upsampledDistribution = upsampledDistribution
                                    .Select(lr => lr / Math.Sqrt(upsamplingSteps))
                                    .OrderBy(lr => lr)
                                    .ToList();

                                //----- find value at risk and scale to required period
                                var distributionAtPercentile = upsampledDistribution[(int)Math.Round(upsampledDistribution.Count * (1.0 - percentile))];
                                var valueAtRisk = 1.0 - Math.Exp(Math.Sqrt(1.0 / upsamplingSteps) * distributionAtPercentile);

                                dst.Add(new BarType<double>(src[idx].Date, Math.Min(1.0, Math.Max(1e-99, valueAtRisk))));
                            }

                            return dst;
                        }));

                    return new TimeSeriesFloat(series.Owner, name, data);
                });
        }
        #endregion
    }
}

//==============================================================================
// end of file

