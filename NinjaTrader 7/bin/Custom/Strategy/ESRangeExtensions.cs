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
    /// This will leverage Price Channels and trade with the expectations that the price will continue to extend the channel.
	/// Tested on 9/4 YTD
	/// 512T - 10, 21, 0.001, 82, 20, 13, 1, 1  +6532.50, 42.38% profitable, 3.29% max dd
	/// 333T - 12, 12, 0.000, 120, 20, 11, 1, 1 +5662.50, 39.81% profitable, 3.43% max dd
	/// 333T - 12, 12, 0.000, 120, 20, 24, 1, 1 +8120.00, 45.53% profitable, 3.58% max dd
	/// 333T - 12, 10, 0.002, 120, 20, 11, 1, 1 +4760.00, 43.13% profitable, 2.62% max dd
	///1024T - 7, 7, 0, 65, 20, 19, 1, 1 +7342.50, 43.78% profitable, 2.71% max dd
    /// </summary>
    [Description("This will leverage Price Channels and trade with the expectations that the price will continue to extend the channel.")]
    public class ESRangeExtensions : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int aTRNumber = 10; // Default setting for ATRNumber
        private int aTRPeriods = 21; // Default setting for ATRPeriods
        private double aTRRatchet = 0.001; // Default setting for ATRRatchet
        private int donChanPer = 82; // Default setting for DonChanPer
        private int hullPer = 20; // Default setting for HullPer
        private int stop = 13; // Default setting for Stop
        private int tProfit = 1; // Default setting for TProfit
        private int tStop = 1; // Default setting for TStop
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            Add(DonchianChannel(DonChanPer));
            Add(ATRTrailing(ATRNumber, ATRPeriods, ATRRatchet));
            SetStopLoss("", CalculationMode.Ticks, Stop, false);

            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Condition set 1
            if (High[0] > DonchianChannel(DonChanPer).Upper[1]
                && Low[0] > ATRTrailing(ATRNumber, ATRPeriods, ATRRatchet).Upper[0]
                && ToTime(Time[0]) > ToTime(8, 30, 0)
                && ToTime(Time[0]) < ToTime(15, 0, 0))
            {
                EnterLong(DefaultQuantity, "ChanExt");
            }

            // Condition set 2
            if (Low[0] < DonchianChannel(DonChanPer).Lower[1]
                && High[0] < ATRTrailing(ATRNumber, ATRPeriods, ATRRatchet).Lower[0]
                && ToTime(Time[0]) > ToTime(9, 0, 0)
                && ToTime(Time[0]) < ToTime(15, 0, 0))
            {
                EnterShort(DefaultQuantity, "");
            }

            // Condition set 3
            if (Low[0] < ATRTrailing(ATRNumber, ATRPeriods, ATRRatchet).Upper[0]
                && Position.MarketPosition == MarketPosition.Long)
            {
                ExitLong("", "");
            }

            // Condition set 4
            if (High[0] > ATRTrailing(ATRNumber, ATRPeriods, ATRRatchet).Lower[0]
                && Position.MarketPosition == MarketPosition.Short)
            {
                ExitShort("", "");
            }
        }

        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int ATRNumber
        {
            get { return aTRNumber; }
            set { aTRNumber = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int ATRPeriods
        {
            get { return aTRPeriods; }
            set { aTRPeriods = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double ATRRatchet
        {
            get { return aTRRatchet; }
            set { aTRRatchet = Math.Max(0.000, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int DonChanPer
        {
            get { return donChanPer; }
            set { donChanPer = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int HullPer
        {
            get { return hullPer; }
            set { hullPer = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Stop
        {
            get { return stop; }
            set { stop = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int TProfit
        {
            get { return tProfit; }
            set { tProfit = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int TStop
        {
            get { return tStop; }
            set { tStop = Math.Max(1, value); }
        }
        #endregion
    }
}
