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
    /// Enter the description of your strategy here
    /// </summary>
    [Description("Enter the description of your strategy here")]
    public class RicksStrategy : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int eMAFast = 15; // Default setting for EMAFast
        private int sMASlow = 45; // Default setting for SMASlow
        private int profitTarget = 15; // Default setting for ProfitTarget
        private int longMAPeriod = 200; // Default setting for LongMAPeriod
        private int trailingStop = 15; // Default setting for TrailingStop
        private int hullPeriod = 20; // Default setting for HullPeriod
        private int dorChanPer = 150; // Default setting for DorChanPer
        private int pullback = 8; // Default setting for Pullback
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            Add(EMA(EMAFast));
            Add(SMA(SMASlow));
            Add(EMA(EMAFast));
            Add(SMA(SMASlow));
            Add(DonchianChannel(DorChanPer));
            SetTrailStop("", CalculationMode.Ticks, TrailingStop, true);
            SetProfitTarget("", CalculationMode.Ticks, ProfitTarget);

            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Condition set 1
            if (CrossAbove(EMA(EMAFast), SMA(SMASlow), 1)
                && LowestBar(DonchianChannel(DorChanPer).Mean, 10) > -500)
            {
                DrawArrowUp("Long Cross Over" + CurrentBar, false, 0, Low[0] + -20 * TickSize, Color.DarkCyan);
                DrawDot("My dot" + CurrentBar, true, 0, High[0], Color.Green);
            }

            // Condition set 2
            if (CrossBelow(EMA(EMAFast), SMA(SMASlow), 1))
            {
                DrawArrowDown("My down arrow" + CurrentBar, false, 0, High[0] + 20 * TickSize, Color.Red);
                DrawDot("My dot" + CurrentBar, true, 0, Low[0], Color.DarkViolet);
            }

            // Condition set 3
            if (CrossBelow(EMA(EMAFast), SMA(SMASlow), 1))
            {
            }


            // Condition set 5
            if (Position.MarketPosition == MarketPosition.Long
                && Low[0] + 1 * TickSize < Position.AvgPrice)
            {
                Variable0 = Position.AvgPrice;
            }


            // Condition set 7
            if (LowestBar(DefaultInput, 7) == 0
                && BarsSinceEntry() > 7
                && Position.MarketPosition == MarketPosition.Long)
            {
                ExitLong("BarsBackStop", "");
            }

            // Condition set 8
            if (High[0] <= High[1]
                && High[1] > DonchianChannel(DorChanPer).Upper[2]
                && Close[0] + Pullback * TickSize < DonchianChannel(DorChanPer).Upper[0])
            {
                EnterShort(DefaultQuantity, "DanchShort");
            }

            // Condition set 9
            if (High[0] < High[2]
                && High[1] < High[2]
                && High[3] < High[2]
                && High[4] < High[2])
            {
                DrawDot("Resistance" + CurrentBar, true, 2, High[2] + 3 * TickSize, Color.Blue);
            }
        }

        #region Properties
        [Description("Fast Moving Average")]
        [GridCategory("Parameters")]
        public int EMAFast
        {
            get { return eMAFast; }
            set { eMAFast = Math.Max(5, value); }
        }

        [Description("Slow Moving Average")]
        [GridCategory("Parameters")]
        public int SMASlow
        {
            get { return sMASlow; }
            set { sMASlow = Math.Max(15, value); }
        }

        [Description("Profit Target")]
        [GridCategory("Parameters")]
        public int ProfitTarget
        {
            get { return profitTarget; }
            set { profitTarget = Math.Max(5, value); }
        }

        [Description("Long Trend Period")]
        [GridCategory("Parameters")]
        public int LongMAPeriod
        {
            get { return longMAPeriod; }
            set { longMAPeriod = Math.Max(20, value); }
        }

        [Description("Trailing Sop")]
        [GridCategory("Parameters")]
        public int TrailingStop
        {
            get { return trailingStop; }
            set { trailingStop = Math.Max(5, value); }
        }

        [Description("Hull Moving Average Period")]
        [GridCategory("Parameters")]
        public int HullPeriod
        {
            get { return hullPeriod; }
            set { hullPeriod = Math.Max(5, value); }
        }

        [Description("Donchian Channel Period")]
        [GridCategory("Parameters")]
        public int DorChanPer
        {
            get { return dorChanPer; }
            set { dorChanPer = Math.Max(20, value); }
        }

        [Description("Min retrace on channel")]
        [GridCategory("Parameters")]
        public int Pullback
        {
            get { return pullback; }
            set { pullback = Math.Max(1, value); }
        }
        #endregion
    }
}
