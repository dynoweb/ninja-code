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
    /// Identify stocks in periods of extremes of low volatility which usually followed by big moves.
    /// </summary>
    [Description("Identify stocks in periods of extremes of low volatility which usually followed by big moves.")]
    public class VolatilityBreakoutPattern : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int period = 20; // Default setting for Period
        private double kCDeviation = 1.500; // Default setting for KCDeviation
        private double bBDeviation = 2.000; // Default setting for BBDeviation
        private int momentumPeriod = 12; // Default setting for MomentumPeriod
        private double bBWidth = 999; // Default setting for BBWidth
        private double stopLossPct = 0.020; // Default setting for StopLossPct
        private double trailingStopPct = 0.020; // Default setting for TrailingStopPct		
        // User defined variables (add any user defined variables below)
		private double trailStop = 0; // Default setting for TrailStop
		private double StopLoss = 0;
        private double prevValue = 0; // Default setting for PrevValue
        private bool justEntered = false; // Default setting for JustEntered		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {

            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Condition set 1
			
			if (VolatilityBreakout2(BBDeviation, KCDeviation, MomentumPeriod, Period).Plot0[0] == 2 &&
				BollingerBandWidth2(BBDeviation, Period)[0] < bBWidth &&
				Position.MarketPosition == MarketPosition.Flat)
            {
                EnterLong(100, "Long");
				justEntered = true;
            }
			
			if (Position.MarketPosition == MarketPosition.Long && justEntered == true)
			{
				trailStop = Position.AvgPrice - (trailingStopPct * Position.AvgPrice);
				StopLoss = Position.AvgPrice - (stopLossPct * Position.AvgPrice);
    			prevValue = Position.AvgPrice;
    			justEntered = false;
    			Print(" TRAIL STOP TRACKING: " + trailStop + " symbol " + Instrument.FullName);
			}			
			if (Position.MarketPosition == MarketPosition.Long)
			{
				if (High[0] > Position.AvgPrice && High[0] > prevValue)
				{
					trailStop = trailStop + (High[0] - prevValue);
       				prevValue = High[0];
       				Print(" TRAIL STOP RAISED: " + trailStop + " PrevValue " + prevValue + " symbol " + Instrument.FullName);
				}
				Print(Time[0] + " High " + High[0] + " PrevValue " + prevValue + " symbol " + Instrument.FullName);
			}
			if (Low[0] <= trailStop && Position.MarketPosition == MarketPosition.Long && Position.AvgPrice < trailStop)
			{
				Print(" TRAIL STOP HIT: " + trailStop + " " + Close[0] + " symbol " + Instrument.FullName);
    			// Trailing stop has been hit; do whatever you want here
				ExitLong(100,"Trailing Stop" ,"Long");
   				trailStop = 0;
				StopLoss = 0;
    			prevValue = 0;
			}
			else if (Low[0] <= StopLoss && Position.MarketPosition == MarketPosition.Long && Position.AvgPrice > StopLoss)
			{
				Print(" STOP LOSS HIT: " + StopLoss + " " + Close[0] + " symbol " + Instrument.FullName);
				// Trailing stop has been hit; do whatever you want here
				ExitLong(100,"Stop Loss" ,"Long");
				trailStop = 0;
				StopLoss = 0;
				prevValue = 0;					
			}
			
            // Condition set 2
            if (VolatilityBreakout2(BBDeviation, KCDeviation, MomentumPeriod, Period).Plot0[0] == -2 &&
				BollingerBandWidth2(BBDeviation, Period)[0] < bBWidth &&
				Position.MarketPosition == MarketPosition.Flat)
            {
                EnterShort(100, "Short");
				justEntered = true;
            }
			if (Position.MarketPosition == MarketPosition.Short && justEntered == true)
			{
				trailStop = Position.AvgPrice + (trailingStopPct * Position.AvgPrice);
				StopLoss = Position.AvgPrice + (stopLossPct * Position.AvgPrice);
    			prevValue = Position.AvgPrice;
    			justEntered = false;
    			Print(" TRAIL STOP TRACKING: " + trailStop + " symbol " + Instrument.FullName);
			}
			if (Position.MarketPosition == MarketPosition.Short)
			{
				if (Low[0] < Position.AvgPrice && Low[0] < prevValue)
				{
					trailStop = trailStop - (prevValue - Low[0]);
       				prevValue = Low[0];
       				Print(" TRAIL STOP Lowered: " + trailStop + " PrevValue " + prevValue + " symbol " + Instrument.FullName);
				}
				Print(Time[0] + " Low " + Low[0] + " PrevValue " + prevValue + " symbol " + Instrument.FullName);
			}
			if (High[0] >= trailStop && Position.MarketPosition == MarketPosition.Short && Position.AvgPrice > trailStop)
			{
				Print(" TRAIL STOP HIT: " + trailStop + " " + Close[0] + " symbol " + Instrument.FullName);
    			// Trailing stop has been hit; do whatever you want here
				ExitShort(100,"Trailing Stop" ,"Short");
   				trailStop = 0;
				StopLoss = 0;
    			prevValue = 0;
			}
			else if (High[0] >= StopLoss && Position.MarketPosition == MarketPosition.Short && Position.AvgPrice < StopLoss)
			{
				Print(" STOP LOSS HIT: " + StopLoss + " " + Close[0] + " symbol " + Instrument.FullName);
				// Trailing stop has been hit; do whatever you want here
				ExitShort(100,"Stop Loss" ,"Short");
				trailStop = 0;
				StopLoss = 0;
				prevValue = 0;					
			}			
			
        }

        #region Properties
        [Description("# of Price Bars used to calculate band/channel")]
        [Category("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }

        [Description("Used to calculate Keltner Channel")]
        [Category("Parameters")]
        public double KCDeviation
        {
            get { return kCDeviation; }
            set { kCDeviation = Math.Max(1, value); }
        }

        [Description("Used to calculate Bollinger Band")]
        [Category("Parameters")]
        public double BBDeviation
        {
            get { return bBDeviation; }
            set { bBDeviation = Math.Max(1, value); }
        }

        [Description("Used to calculate Momentum")]
        [Category("Parameters")]
        public int MomentumPeriod
        {
            get { return momentumPeriod; }
            set { momentumPeriod = Math.Max(1, value); }
        }

        [Description("Used to limit results, lower = Low Volatility & 999 to ignore")]
        [Category("Parameters")]
        public double BBWidth
        {
            get { return bBWidth; }
            set { bBWidth = Math.Max(0.000, value); }
        }
		
		[Description("Define stop loss percent")]
        [Category("Parameters")]
        public double StopLossPct
        {
            get { return stopLossPct; }
            set { stopLossPct = Math.Max(0.010, value); }
        }

        [Description("Define trailing stop percent")]
        [Category("Parameters")]
        public double TrailingStopPct
        {
            get { return trailingStopPct; }
            set { trailingStopPct = Math.Max(0.010, value); }
        }
        #endregion
    }
}
