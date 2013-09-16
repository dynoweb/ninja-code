//
// Copyright (C) 2006, NinjaTrader LLC <ninjatrader@ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private ncatTrendHeatMap[] cachencatTrendHeatMap = null;
        private Trend[] cacheTrend = null;

        private static ncatTrendHeatMap checkncatTrendHeatMap = new ncatTrendHeatMap();
        private static Trend checkTrend = new Trend();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public ncatTrendHeatMap ncatTrendHeatMap(Color longStrong, Color longWeak, Color neutral, bool shortLogo, Color shortStrong, Color shortWeak, int trend1Period, PeriodType trend1PeriodType, int trend1Strength, int trend2Period, PeriodType trend2PeriodType, int trend2Strength, int trend3Period, PeriodType trend3PeriodType, int trend3Strength, int trend4Period, PeriodType trend4PeriodType, int trend4Strength)
        {
            return ncatTrendHeatMap(Input, longStrong, longWeak, neutral, shortLogo, shortStrong, shortWeak, trend1Period, trend1PeriodType, trend1Strength, trend2Period, trend2PeriodType, trend2Strength, trend3Period, trend3PeriodType, trend3Strength, trend4Period, trend4PeriodType, trend4Strength);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public ncatTrendHeatMap ncatTrendHeatMap(Data.IDataSeries input, Color longStrong, Color longWeak, Color neutral, bool shortLogo, Color shortStrong, Color shortWeak, int trend1Period, PeriodType trend1PeriodType, int trend1Strength, int trend2Period, PeriodType trend2PeriodType, int trend2Strength, int trend3Period, PeriodType trend3PeriodType, int trend3Strength, int trend4Period, PeriodType trend4PeriodType, int trend4Strength)
        {
            if (cachencatTrendHeatMap != null)
                for (int idx = 0; idx < cachencatTrendHeatMap.Length; idx++)
                    if (cachencatTrendHeatMap[idx].LongStrong == longStrong && cachencatTrendHeatMap[idx].LongWeak == longWeak && cachencatTrendHeatMap[idx].Neutral == neutral && cachencatTrendHeatMap[idx].ShortLogo == shortLogo && cachencatTrendHeatMap[idx].ShortStrong == shortStrong && cachencatTrendHeatMap[idx].ShortWeak == shortWeak && cachencatTrendHeatMap[idx].Trend1Period == trend1Period && cachencatTrendHeatMap[idx].Trend1PeriodType == trend1PeriodType && cachencatTrendHeatMap[idx].Trend1Strength == trend1Strength && cachencatTrendHeatMap[idx].Trend2Period == trend2Period && cachencatTrendHeatMap[idx].Trend2PeriodType == trend2PeriodType && cachencatTrendHeatMap[idx].Trend2Strength == trend2Strength && cachencatTrendHeatMap[idx].Trend3Period == trend3Period && cachencatTrendHeatMap[idx].Trend3PeriodType == trend3PeriodType && cachencatTrendHeatMap[idx].Trend3Strength == trend3Strength && cachencatTrendHeatMap[idx].Trend4Period == trend4Period && cachencatTrendHeatMap[idx].Trend4PeriodType == trend4PeriodType && cachencatTrendHeatMap[idx].Trend4Strength == trend4Strength && cachencatTrendHeatMap[idx].EqualsInput(input))
                        return cachencatTrendHeatMap[idx];

            lock (checkncatTrendHeatMap)
            {
                checkncatTrendHeatMap.LongStrong = longStrong;
                longStrong = checkncatTrendHeatMap.LongStrong;
                checkncatTrendHeatMap.LongWeak = longWeak;
                longWeak = checkncatTrendHeatMap.LongWeak;
                checkncatTrendHeatMap.Neutral = neutral;
                neutral = checkncatTrendHeatMap.Neutral;
                checkncatTrendHeatMap.ShortLogo = shortLogo;
                shortLogo = checkncatTrendHeatMap.ShortLogo;
                checkncatTrendHeatMap.ShortStrong = shortStrong;
                shortStrong = checkncatTrendHeatMap.ShortStrong;
                checkncatTrendHeatMap.ShortWeak = shortWeak;
                shortWeak = checkncatTrendHeatMap.ShortWeak;
                checkncatTrendHeatMap.Trend1Period = trend1Period;
                trend1Period = checkncatTrendHeatMap.Trend1Period;
                checkncatTrendHeatMap.Trend1PeriodType = trend1PeriodType;
                trend1PeriodType = checkncatTrendHeatMap.Trend1PeriodType;
                checkncatTrendHeatMap.Trend1Strength = trend1Strength;
                trend1Strength = checkncatTrendHeatMap.Trend1Strength;
                checkncatTrendHeatMap.Trend2Period = trend2Period;
                trend2Period = checkncatTrendHeatMap.Trend2Period;
                checkncatTrendHeatMap.Trend2PeriodType = trend2PeriodType;
                trend2PeriodType = checkncatTrendHeatMap.Trend2PeriodType;
                checkncatTrendHeatMap.Trend2Strength = trend2Strength;
                trend2Strength = checkncatTrendHeatMap.Trend2Strength;
                checkncatTrendHeatMap.Trend3Period = trend3Period;
                trend3Period = checkncatTrendHeatMap.Trend3Period;
                checkncatTrendHeatMap.Trend3PeriodType = trend3PeriodType;
                trend3PeriodType = checkncatTrendHeatMap.Trend3PeriodType;
                checkncatTrendHeatMap.Trend3Strength = trend3Strength;
                trend3Strength = checkncatTrendHeatMap.Trend3Strength;
                checkncatTrendHeatMap.Trend4Period = trend4Period;
                trend4Period = checkncatTrendHeatMap.Trend4Period;
                checkncatTrendHeatMap.Trend4PeriodType = trend4PeriodType;
                trend4PeriodType = checkncatTrendHeatMap.Trend4PeriodType;
                checkncatTrendHeatMap.Trend4Strength = trend4Strength;
                trend4Strength = checkncatTrendHeatMap.Trend4Strength;

                if (cachencatTrendHeatMap != null)
                    for (int idx = 0; idx < cachencatTrendHeatMap.Length; idx++)
                        if (cachencatTrendHeatMap[idx].LongStrong == longStrong && cachencatTrendHeatMap[idx].LongWeak == longWeak && cachencatTrendHeatMap[idx].Neutral == neutral && cachencatTrendHeatMap[idx].ShortLogo == shortLogo && cachencatTrendHeatMap[idx].ShortStrong == shortStrong && cachencatTrendHeatMap[idx].ShortWeak == shortWeak && cachencatTrendHeatMap[idx].Trend1Period == trend1Period && cachencatTrendHeatMap[idx].Trend1PeriodType == trend1PeriodType && cachencatTrendHeatMap[idx].Trend1Strength == trend1Strength && cachencatTrendHeatMap[idx].Trend2Period == trend2Period && cachencatTrendHeatMap[idx].Trend2PeriodType == trend2PeriodType && cachencatTrendHeatMap[idx].Trend2Strength == trend2Strength && cachencatTrendHeatMap[idx].Trend3Period == trend3Period && cachencatTrendHeatMap[idx].Trend3PeriodType == trend3PeriodType && cachencatTrendHeatMap[idx].Trend3Strength == trend3Strength && cachencatTrendHeatMap[idx].Trend4Period == trend4Period && cachencatTrendHeatMap[idx].Trend4PeriodType == trend4PeriodType && cachencatTrendHeatMap[idx].Trend4Strength == trend4Strength && cachencatTrendHeatMap[idx].EqualsInput(input))
                            return cachencatTrendHeatMap[idx];

                ncatTrendHeatMap indicator = new ncatTrendHeatMap();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.LongStrong = longStrong;
                indicator.LongWeak = longWeak;
                indicator.Neutral = neutral;
                indicator.ShortLogo = shortLogo;
                indicator.ShortStrong = shortStrong;
                indicator.ShortWeak = shortWeak;
                indicator.Trend1Period = trend1Period;
                indicator.Trend1PeriodType = trend1PeriodType;
                indicator.Trend1Strength = trend1Strength;
                indicator.Trend2Period = trend2Period;
                indicator.Trend2PeriodType = trend2PeriodType;
                indicator.Trend2Strength = trend2Strength;
                indicator.Trend3Period = trend3Period;
                indicator.Trend3PeriodType = trend3PeriodType;
                indicator.Trend3Strength = trend3Strength;
                indicator.Trend4Period = trend4Period;
                indicator.Trend4PeriodType = trend4PeriodType;
                indicator.Trend4Strength = trend4Strength;
                Indicators.Add(indicator);
                indicator.SetUp();

                ncatTrendHeatMap[] tmp = new ncatTrendHeatMap[cachencatTrendHeatMap == null ? 1 : cachencatTrendHeatMap.Length + 1];
                if (cachencatTrendHeatMap != null)
                    cachencatTrendHeatMap.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachencatTrendHeatMap = tmp;
                return indicator;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Trend Trend(int strength)
        {
            return Trend(Input, strength);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Trend Trend(Data.IDataSeries input, int strength)
        {
            if (cacheTrend != null)
                for (int idx = 0; idx < cacheTrend.Length; idx++)
                    if (cacheTrend[idx].Strength == strength && cacheTrend[idx].EqualsInput(input))
                        return cacheTrend[idx];

            lock (checkTrend)
            {
                checkTrend.Strength = strength;
                strength = checkTrend.Strength;

                if (cacheTrend != null)
                    for (int idx = 0; idx < cacheTrend.Length; idx++)
                        if (cacheTrend[idx].Strength == strength && cacheTrend[idx].EqualsInput(input))
                            return cacheTrend[idx];

                Trend indicator = new Trend();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Strength = strength;
                Indicators.Add(indicator);
                indicator.SetUp();

                Trend[] tmp = new Trend[cacheTrend == null ? 1 : cacheTrend.Length + 1];
                if (cacheTrend != null)
                    cacheTrend.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTrend = tmp;
                return indicator;
            }
        }
    }
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
    public partial class Column : ColumnBase
    {
        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ncatTrendHeatMap ncatTrendHeatMap(Color longStrong, Color longWeak, Color neutral, bool shortLogo, Color shortStrong, Color shortWeak, int trend1Period, PeriodType trend1PeriodType, int trend1Strength, int trend2Period, PeriodType trend2PeriodType, int trend2Strength, int trend3Period, PeriodType trend3PeriodType, int trend3Strength, int trend4Period, PeriodType trend4PeriodType, int trend4Strength)
        {
            return _indicator.ncatTrendHeatMap(Input, longStrong, longWeak, neutral, shortLogo, shortStrong, shortWeak, trend1Period, trend1PeriodType, trend1Strength, trend2Period, trend2PeriodType, trend2Strength, trend3Period, trend3PeriodType, trend3Strength, trend4Period, trend4PeriodType, trend4Strength);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.ncatTrendHeatMap ncatTrendHeatMap(Data.IDataSeries input, Color longStrong, Color longWeak, Color neutral, bool shortLogo, Color shortStrong, Color shortWeak, int trend1Period, PeriodType trend1PeriodType, int trend1Strength, int trend2Period, PeriodType trend2PeriodType, int trend2Strength, int trend3Period, PeriodType trend3PeriodType, int trend3Strength, int trend4Period, PeriodType trend4PeriodType, int trend4Strength)
        {
            return _indicator.ncatTrendHeatMap(input, longStrong, longWeak, neutral, shortLogo, shortStrong, shortWeak, trend1Period, trend1PeriodType, trend1Strength, trend2Period, trend2PeriodType, trend2Strength, trend3Period, trend3PeriodType, trend3Strength, trend4Period, trend4PeriodType, trend4Strength);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Trend Trend(int strength)
        {
            return _indicator.Trend(Input, strength);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Indicator.Trend Trend(Data.IDataSeries input, int strength)
        {
            return _indicator.Trend(input, strength);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ncatTrendHeatMap ncatTrendHeatMap(Color longStrong, Color longWeak, Color neutral, bool shortLogo, Color shortStrong, Color shortWeak, int trend1Period, PeriodType trend1PeriodType, int trend1Strength, int trend2Period, PeriodType trend2PeriodType, int trend2Strength, int trend3Period, PeriodType trend3PeriodType, int trend3Strength, int trend4Period, PeriodType trend4PeriodType, int trend4Strength)
        {
            return _indicator.ncatTrendHeatMap(Input, longStrong, longWeak, neutral, shortLogo, shortStrong, shortWeak, trend1Period, trend1PeriodType, trend1Strength, trend2Period, trend2PeriodType, trend2Strength, trend3Period, trend3PeriodType, trend3Strength, trend4Period, trend4PeriodType, trend4Strength);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.ncatTrendHeatMap ncatTrendHeatMap(Data.IDataSeries input, Color longStrong, Color longWeak, Color neutral, bool shortLogo, Color shortStrong, Color shortWeak, int trend1Period, PeriodType trend1PeriodType, int trend1Strength, int trend2Period, PeriodType trend2PeriodType, int trend2Strength, int trend3Period, PeriodType trend3PeriodType, int trend3Strength, int trend4Period, PeriodType trend4PeriodType, int trend4Strength)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ncatTrendHeatMap(input, longStrong, longWeak, neutral, shortLogo, shortStrong, shortWeak, trend1Period, trend1PeriodType, trend1Strength, trend2Period, trend2PeriodType, trend2Strength, trend3Period, trend3PeriodType, trend3Strength, trend4Period, trend4PeriodType, trend4Strength);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Trend Trend(int strength)
        {
            return _indicator.Trend(Input, strength);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Indicator.Trend Trend(Data.IDataSeries input, int strength)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Trend(input, strength);
        }
    }
}
#endregion
