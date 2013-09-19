#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Strategy;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Trades the hullma on CL
    /// </summary>
    [Description("Trades the hullma on CL")]
    public class CL150THull : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int hullPeriod = 4; // Default setting for HullPeriod
        private int sHullPeriod = 5; // Default setting for SHullPeriod
        private int stopLong = 500; // Default setting for Stop
        private int stopShort = 500; // Default setting for SStop
        private int tStop = 37; // Default setting for TStop
        private int sTStop = 38; // Default setting for STStop
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            Add(HMA(SHullPeriod));
            SetTrailStop("Short", CalculationMode.Ticks, STStop, false);
            SetTrailStop("Long", CalculationMode.Ticks, TStop, false);

            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Condition set 1
            if (Low[1] < HMA(HullPeriod)[1]
                && Low[0] >= HMA(HullPeriod)[0]
                && ToTime(Time[0]) >= ToTime(8, 30, 0)
                && ToTime(Time[0]) < ToTime(12, 0, 0)
                && Position.MarketPosition == MarketPosition.Flat)
            {
                EnterLong(DefaultQuantity, "Long");
            }

            // Condition set 2
            if (High[0] < HMA(HullPeriod)[0])
            {
                ExitLong("", "");
            }

            // Condition set 3
            if (High[1] > HMA(SHullPeriod)[1]
                && High[0] <= HMA(SHullPeriod)[0]
                && ToTime(Time[0]) >= ToTime(8, 30, 0)
                && ToTime(Time[0]) < ToTime(10, 30, 0)
                && Position.MarketPosition == MarketPosition.Flat)
            {
                EnterShort(DefaultQuantity, "Short");
            }

            // Condition set 4
            if (Low[0] >= HMA(SHullPeriod)[0])
            {
                ExitShort("", "");
            }
        }

        #region Properties
        [Description("Hull Periods - Long")]
        [GridCategory("Parameters")]
        public int HullPeriod
        {
            get { return hullPeriod; }
            set { hullPeriod = Math.Max(1, value); }
        }

        [Description("Hull Periods - Short")]
        [GridCategory("Parameters")]
        private int SHullPeriod
        {
            get { return sHullPeriod; }
            set { sHullPeriod = Math.Max(1, value); }
        }

        [Description("Stop - Long")]
        [GridCategory("Parameters")]
        public int StopLong
        {
            get { return stopLong; }
            set { stopLong = Math.Max(1, value); }
        }

        [Description("Stop - Short")]
        [GridCategory("Parameters")]
        public int StopShort
        {
            get { return stopShort; }
            set { stopShort = Math.Max(0, value); }
        }

        [Description("Trailing Stop - Long")]
        [GridCategory("Parameters")]
        public int TStop
        {
            get { return tStop; }
            set { tStop = Math.Max(1, value); }
        }

        [Description("Trailing Stop - Short")]
        [GridCategory("Parameters")]
        public int STStop
        {
            get { return sTStop; }
            set { sTStop = Math.Max(1, value); }
        }
        #endregion
    }
}
