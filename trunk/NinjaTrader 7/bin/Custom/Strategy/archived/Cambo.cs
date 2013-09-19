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
    /// Trend Trading
    /// </summary>
    [Description("Trend Trading")]
    public class Cambo : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int dMPeriod = 14; // Default setting for DMPeriod
        private int stoploss = 30; // Default setting for Stoploss
        private int trailingStop = 50; // Default setting for TrailingStop
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            Add(DM(DMPeriod));
            Add(DM(DMPeriod));
            Add(MACD(12, 26, 9));
            Add(MACD(12, 26, 9));
            Add(DM(DMPeriod));
            Add(DM(DMPeriod));
            Add(MACD(12, 26, 9));
            Add(MACD(12, 26, 9));
            Add(DM(DMPeriod));
            Add(DM(DMPeriod));
            Add(MACD(12, 26, 9));
            Add(MACD(12, 26, 9));
            Add(DM(DMPeriod));
            Add(DM(DMPeriod));
            Add(MACD(12, 26, 9));
            Add(MACD(12, 26, 9));
            SetStopLoss("", CalculationMode.Ticks, Stoploss, true);
            SetTrailStop("", CalculationMode.Ticks, TrailingStop, true);

            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Condition set 1
            if (CrossAbove(DM(DMPeriod).DiPlus, DM(DMPeriod).DiMinus, 1)
                &&  CrossAbove(MACD(12, 26, 9), MACD(12, 26, 9).Avg, 1))
            {
                EnterLong(DefaultQuantity, "");
            }

            // Condition set 2
            if (CrossBelow(DM(DMPeriod).DiPlus, DM(DMPeriod).DiMinus, 1)
                &&  CrossBelow(MACD(12, 26, 9), MACD(12, 26, 9).Avg, 1))
            {
                EnterShort(DefaultQuantity, "");
            }

            // Condition set 3
            if (CrossAbove(DM(DMPeriod).DiPlus, DM(DMPeriod).DiMinus, 1)
                &&  CrossAbove(MACD(12, 26, 9), MACD(12, 26, 9).Avg, 1))
            {
                ExitShort("", "");
            }

            // Condition set 4
            if (CrossBelow(DM(DMPeriod).DiPlus, DM(DMPeriod).DiMinus, 1)
                &&  CrossBelow(MACD(12, 26, 9), MACD(12, 26, 9).Avg, 1))
            {
                ExitLong("", "");
            }
        }

        #region Properties
        [Description("DI +/-")]
        [GridCategory("Parameters")]
        public int DMPeriod
        {
            get { return dMPeriod; }
            set { dMPeriod = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Stoploss
        {
            get { return stoploss; }
            set { stoploss = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int TrailingStop
        {
            get { return trailingStop; }
            set { trailingStop = Math.Max(1, value); }
        }
        #endregion
    }
}
