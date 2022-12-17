﻿//==============================================================================
// Project:     TuringTrader, simulator core v2
// Name:        Indicators/Arithmetic
// Description: Arithmetic on time series.
// History:     2022xi02, FUB, created
//------------------------------------------------------------------------------
// Copyright:   (c) 2011-2022, Bertram Enterprises LLC dba TuringTrader.
//              https://www.turingtrader.org
// License:     This file is part of TuringTrader, an open-source backtesting
//              engine/ market simulator.
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
using System.Threading.Tasks;

namespace TuringTrader.SimulatorV2.Indicators
{
    /// <summary>
    /// Collection of indicators performing arithmetic on time series.
    /// </summary>
    public static class Arithmetic
    {
        #region Add
        public static TimeSeriesFloat Add(this TimeSeriesFloat summand1, TimeSeriesFloat summand2)
        {
            var name = string.Format("{0}.Add({1})", summand1.Name, summand2.Name);

            return summand1.Algorithm.Cache(
                name,
                () => new TimeSeriesFloat(
                    summand1.Algorithm, name,
                    Task.Run(() =>
                    {
                        var src = summand1.Data.Result;
                        var dst = new List<BarType<double>>();

                        foreach (var it in src)
                        {
                            dst.Add(new BarType<double>(
                                it.Date,
                                it.Value + summand2[it.Date]));
                        }

                        return dst;
                    })));
        }
        public static TimeSeriesFloat Add(this TimeSeriesFloat summand1, double summand2)
        {
            var name = string.Format("{0}.Add({1})", summand1.Name, summand2);

            return summand1.Algorithm.Cache(
                name,
                () => new TimeSeriesFloat(
                    summand1.Algorithm, name,
                    Task.Run(() =>
                    {
                        var src = summand1.Data.Result;
                        var dst = new List<BarType<double>>();

                        foreach (var it in src)
                        {
                            dst.Add(new BarType<double>(
                                it.Date,
                                it.Value + summand2));
                        }

                        return dst;
                    })));
        }
        #endregion
        #region Sub
        public static TimeSeriesFloat Sub(this TimeSeriesFloat minuend, TimeSeriesFloat subtrahend)
        {
            var name = string.Format("{0}.Sub({1})", minuend.Name, subtrahend.Name);

            return minuend.Algorithm.Cache(
                name,
                () => new TimeSeriesFloat(
                    minuend.Algorithm, name,
                    Task.Run(() =>
                    {
                        var src = minuend.Data.Result;
                        var dst = new List<BarType<double>>();

                        foreach (var it in src)
                        {
                            dst.Add(new BarType<double>(
                                it.Date,
                                it.Value - subtrahend[it.Date]));
                        }

                        return dst;
                    })));
        }
        public static TimeSeriesFloat Sub(this TimeSeriesFloat minuend, double subtrahend)
        {
            var name = string.Format("{0}.Sub({1})", minuend.Name, subtrahend);

            return minuend.Algorithm.Cache(
                name,
                () => new TimeSeriesFloat(
                    minuend.Algorithm, name,
                    Task.Run(() =>
                    {
                        var src = minuend.Data.Result;
                        var dst = new List<BarType<double>>();

                        foreach (var it in src)
                        {
                            dst.Add(new BarType<double>(
                                it.Date,
                                it.Value - subtrahend));
                        }

                        return dst;
                    })));
        }
        #endregion
        #region Mul
        public static TimeSeriesFloat Mul(this TimeSeriesFloat multiplicand1, TimeSeriesFloat multiplicand2)
        {
            var name = string.Format("{0}.Mul({1})", multiplicand1.Name, multiplicand2.Name);

            return multiplicand1.Algorithm.Cache(
                name,
                () => new TimeSeriesFloat(
                    multiplicand1.Algorithm, name,
                    Task.Run(() =>
                    {
                        var src = multiplicand1.Data.Result;
                        var dst = new List<BarType<double>>();

                        foreach (var it in src)
                        {
                            dst.Add(new BarType<double>(
                                it.Date,
                                it.Value * multiplicand2[it.Date]));
                        }

                        return dst;
                    })));
        }
        public static TimeSeriesFloat Mul(this TimeSeriesFloat multiplicand1, double multiplicand2)
        {
            var name = string.Format("{0}.Mul({1})", multiplicand1.Name, multiplicand2);

            return multiplicand1.Algorithm.Cache(
                name,
                () => new TimeSeriesFloat(
                    multiplicand1.Algorithm, name,
                    Task.Run(() =>
                    {
                        var src = multiplicand1.Data.Result;
                        var dst = new List<BarType<double>>();

                        foreach (var it in src)
                        {
                            dst.Add(new BarType<double>(
                                it.Date,
                                it.Value * multiplicand2));
                        }

                        return dst;
                    })));
        }
        #endregion
        #region Div
        /// <summary>
        /// Divide one time series by another.
        /// </summary>
        /// <param name="dividend">input series</param>
        /// <param name="divisor">series by which to divide</param>
        /// <returns>Div series</returns>
        public static TimeSeriesFloat Div(this TimeSeriesFloat dividend, TimeSeriesFloat divisor)
        {
            var name = string.Format("{0}.Div({1})", dividend.Name, divisor.Name);

            return dividend.Algorithm.Cache(
                name,
                () => new TimeSeriesFloat(
                    dividend.Algorithm, name,
                    Task.Run(() =>
                    {
                        var src = dividend.Data.Result;
                        var dst = new List<BarType<double>>();

                        foreach (var it in src)
                        {
                            dst.Add(new BarType<double>(
                                it.Date,
                                it.Value / divisor[it.Date]));
                        }

                        return dst;
                    })));

        }
        /// <summary>
        /// Divide time series by constant value.
        /// </summary>
        /// <param name="dividend">input series</param>
        /// <param name="divisor">series by which to divide</param>
        /// <returns>Div series</returns>
        public static TimeSeriesFloat Div(this TimeSeriesFloat dividend, double divisor)
        {
            var name = string.Format("{0}.Div({1})", dividend.Name, divisor);

            return dividend.Algorithm.Cache(
                name,
                () => new TimeSeriesFloat(
                    dividend.Algorithm, name,
                    Task.Run(() =>
                    {
                        var src = dividend.Data.Result;
                        var dst = new List<BarType<double>>();

                        foreach (var it in src)
                        {
                            dst.Add(new BarType<double>(
                                it.Date,
                                it.Value / divisor));
                        }

                        return dst;
                    })));
        }
        #endregion
        #region Min
        public static TimeSeriesFloat Min(this TimeSeriesFloat min1, TimeSeriesFloat min2)
        {
            var name = string.Format("{0}.Min({1})", min1.Name, min2.Name);

            return min1.Algorithm.Cache(
                name,
                () => new TimeSeriesFloat(
                    min1.Algorithm, name,
                    Task.Run(() =>
                    {
                        var src = min1.Data.Result;
                        var dst = new List<BarType<double>>();

                        foreach (var it in src)
                        {
                            dst.Add(new BarType<double>(
                                it.Date,
                                Math.Min(it.Value, min2[it.Date])));
                        }

                        return dst;
                    })));
        }
        public static TimeSeriesFloat Min(this TimeSeriesFloat min1, double min2)
        {
            var name = string.Format("{0}.Min({1})", min1.Name, min2);

            return min1.Algorithm.Cache(
                name,
                () => new TimeSeriesFloat(
                    min1.Algorithm, name,
                    Task.Run(() =>
                    {
                        var src = min1.Data.Result;
                        var dst = new List<BarType<double>>();

                        foreach (var it in src)
                        {
                            dst.Add(new BarType<double>(
                                it.Date,
                                Math.Min(it.Value, min2)));
                        }

                        return dst;
                    })));
        }
        #endregion
        #region Max
        public static TimeSeriesFloat Max(this TimeSeriesFloat max1, TimeSeriesFloat max2)
        {
            var name = string.Format("{0}.Max({1})", max1.Name, max2.Name);

            return max1.Algorithm.Cache(
                name,
                () => new TimeSeriesFloat(
                    max1.Algorithm, name,
                    Task.Run(() =>
                    {
                        var src = max1.Data.Result;
                        var dst = new List<BarType<double>>();

                        foreach (var it in src)
                        {
                            dst.Add(new BarType<double>(
                                it.Date,
                                Math.Max(it.Value, max2[it.Date])));
                        }

                        return dst;
                    })));
        }
        public static TimeSeriesFloat Max(this TimeSeriesFloat max1, double max2)
        {
            var name = string.Format("{0}.Max({1})", max1.Name, max2);

            return max1.Algorithm.Cache(
                name,
                () => new TimeSeriesFloat(
                    max1.Algorithm, name,
                    Task.Run(() =>
                    {
                        var src = max1.Data.Result;
                        var dst = new List<BarType<double>>();

                        foreach (var it in src)
                        {
                            dst.Add(new BarType<double>(
                                it.Date,
                                Math.Max(it.Value, max2)));
                        }

                        return dst;
                    })));
        }
        #endregion
    }
}

//==============================================================================
// end of file
