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
    /// This strategy looks for channel expansion and takes a trade in the opposite direction.
	/// This strategy depends on CL @ 150 tick 
    /// </summary>
    [Description("This strategy looks for channel expansion and takes a trade in the opposite direction. ")]
    public class ChannelReversion : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int channelBars = 6; // Default setting for ChannelBars
        private int channelPeriod = 200; // Default setting for ChannelPeriod
        private int hullPeriod = 28; // Default setting for HullPeriod
        private int profitTarget = 10; // Default setting for ProfitTarget
		private int stop = 17;
        private double stopPoints = -0.10; // Default setting for Stop
        private int trailingStop = 25; // Default setting for TrailingStop
        // User defined variables (add any user defined variables below)
		Boolean stopMoved = false;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            Add(DonchianChannel(ChannelPeriod));
            Add(HMA(HullPeriod));
            //SetProfitTarget("DonShort", CalculationMode.Ticks, ProfitTarget);
            SetTrailStop("DonShort", CalculationMode.Ticks, TrailingStop, false);
            SetStopLoss("DonShort", CalculationMode.Ticks, Stop, false);

            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Condition set 1
            if (DonchianChannel(ChannelPeriod).Upper[0] > 
					DonchianChannel(ChannelPeriod).Upper[ChannelBars]
                && Falling(HMA(HullPeriod)) == true)
            {
                EnterShort(DefaultQuantity, "DonShort");
				Print("Position Qty: " + Position.Quantity + " @ " + Position.AvgPrice);
				stopMoved = false;
            }

            if (Position.MarketPosition == MarketPosition.Short
                && Rising(HMA(HullPeriod)) == true)
            {
                ExitShort(DefaultQuantity, "HullUp", "DonShort");
            }

			
            // Condition set 2
            if (Position.MarketPosition == MarketPosition.Short && !stopMoved)
            {
				// unrealized gain
				// Position.GetProfitLoss(Close[0], PerformanceUnit.Points)
				//Print("Low[0]: " + Low[0] + " > StopPoints: " + StopPoints);
                //ExitShort("UnrealizedMax", "");
				//Print("Open PnL: " + Position.GetProfitLoss(Close[0], PerformanceUnit.Points));

				// Raise stop loss to breakeven when there is at least 10 ticks in profit
//    			if (Close[0] >= Position.AvgPrice + 10 * TickSize) {
//					ExitLongStop(Position.Quantity, Position.AvgPrice);
//					stopMoved = true;
//					Print("Stopped moved to b/e");
//				}


            }
			
        }

        #region Properties
        [Description("Donchian Channel Period")]
        [GridCategory("Parameters")]
        public int ChannelPeriod
        {
            get { return channelPeriod; }
            set { channelPeriod = Math.Max(50, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int HullPeriod
        {
            get { return hullPeriod; }
            set { hullPeriod = Math.Max(10, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int TrailingStop
        {
            get { return trailingStop; }
            set { trailingStop = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int ProfitTarget
        {
            get { return profitTarget; }
            set { profitTarget = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int ChannelBars
        {
            get { return channelBars; }
            set { channelBars = Math.Max(1, value); }
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
        public double StopPoints
        {
            get { return stopPoints; }
            set { stopPoints = Math.Max(1, value); }
        }
        #endregion
    }
}
