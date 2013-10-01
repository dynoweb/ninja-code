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
	/// Written by Rick Cromer
	/// Stock Strategy based on Chapter 6 of the book Building Reliable Trading Systems
	/// Trades daily time period commodities
    /// </summary>
    [Description("Trades daily time period commodities, based on Chapter 5 of the book Building Reliable Trading Systems. Note when running live make sure to set the the 'Stop & target submission' property to ByStrategyPosition")]
    public class Chapter6Commodities : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int donchianPeriod = 20; // Default setting for DonchianPeriod
        private int exitAfterXBars = 0; // Default setting for ExitAfterXBars
        private int longerTermPeriod = 70; // Default setting for LongerTermPeriod
        //private bool longOnly = false; // Default setting for LongOnly
		//private double lowVolatilityThreshold = 3; // used in stocks for stdDev
		private double highVolatilityFilter = 0; // used to check ATR in $
        private double stopLoss = 1910; // Default setting for StopLoss
        private double profitTarget = 0; // Default setting for ProfitTarget
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			// Order Handling enforce only 1 position long or short at any time
			EntriesPerDirection = 1; 
			EntryHandling = EntryHandling.AllEntries;
			ClearOutputWindow();
			ExitOnClose = true;
			CalculateOnBarClose = true;
				
			// if this is set without a CalculationMode then it's a dollar value
			// for the entire trade (targetPrice = fillPrice + profitTarget/positionQuantity)
			//if (profitTarget != 0) SetProfitTarget(profitTarget);
			if (stopLoss != 0) SetStopLoss(stopLoss);
			if (profitTarget != 0) SetTrailStop(CalculationMode.Ticks, profitTarget);
			
			
			Add(DonchianChannelClose(donchianPeriod));			
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {			
			// Checks to make sure we have at least x or more bars
			if (CurrentBar < Math.Max(LongerTermPeriod, DonchianPeriod))
        		return;
			
			if (ExitAfterXBars != 0 && BarsSinceEntry() >= ExitAfterXBars)
			{					
				if (Position.MarketPosition == MarketPosition.Long)
					ExitLong();
				if (Position.MarketPosition == MarketPosition.Short)
					ExitShort();
			}
			
//			if (Position.MarketPosition == MarketPosition.Long)
//			{
//				if (Close[0] < MAX(Close, DonchianPeriod)[1]
//					&& Close[1] < MAX(Close, DonchianPeriod)[2])
//					ExitLong();
//			}
//			if (Position.MarketPosition == MarketPosition.Short)
//			{
//				if (Close[0] > MIN(Close, DonchianPeriod)[1]
//					&& Close[1] > MIN(Close, DonchianPeriod)[2])
//					ExitShort();
//			}
				

				if (ExitAfterXBars == 0 && DonchianPeriod != 0)
				{
					if (Close[0] < MIN(Close, DonchianPeriod)[1])
						ExitLong();								
					if (Close[0] > MAX(Close, DonchianPeriod)[1])
						ExitShort();								
				}
			
//			if (Position.MarketPosition == MarketPosition.Flat)
//			{				
				// using trend following entry signals
				if (Close[0] > MAX(Close, DonchianPeriod)[1]
					&& (LongerTermPeriod == 0 || Close[0] > Close[LongerTermPeriod]))
				{
					if (HighVolatilityFilter == 0 || calcAverageRange() < HighVolatilityFilter)
					{
						EnterLong();
						//EnterLongStop(Close[0] + TickSize);
					}
					else
						DrawDot(CurrentBar + "ATR", true, 0, High[0] + 2 * TickSize, Color.Yellow);
				}
				
				if (Close[0] < MIN(Close, DonchianPeriod)[1]
					&& (LongerTermPeriod == 0 || Close[0] < Close[LongerTermPeriod]))
				{
					if (HighVolatilityFilter == 0 || calcAverageRange() < HighVolatilityFilter)
					{
						EnterShort();
						//EnterShortStop(Close[0] - TickSize);
					}
					else
						DrawDot(CurrentBar + "ATR", true, 0, Low[0] - 2 * TickSize, Color.Yellow);
				}
				
				///
				/// Potential Errors: - Long
				/// 
				/// Buy order placed has been ignored since the stop price is less than or equal 
				/// to the close price of the current bar. This is an invalid order and subsequent 
				/// orders may also be ignored. Please fix your strategy.
				/// 
//			}
        }
		
		private double calcAverageRange()
		{
			int volPeriod = 20;
			double v = Instrument.MasterInstrument.PointValue * ATR(volPeriod)[0];
			Print(Time + " PointValue: " + Instrument.MasterInstrument.PointValue + " ATR " + ATR(volPeriod)[0] + " avgRange " + v);
			return v;
		}
		
        #region Properties
        [Description("Donchian channel period, use 0 to ignore")]
        [GridCategory("Parameters")]
        public int DonchianPeriod
        {
            get { return donchianPeriod; }
            set { donchianPeriod = Math.Max(0, value); }
        }

        [Description("Closes the trade after x bars, use 0 to ignore")]
        [GridCategory("Parameters")]
        public int ExitAfterXBars
        {
            get { return exitAfterXBars; }
            set { exitAfterXBars = Math.Max(0, value); }
        }

        [Description("ATR VolatilityFilter Average Range < $x")]
        [GridCategory("Parameters")]
        public double HighVolatilityFilter
        {
            get { return highVolatilityFilter; }
            set { highVolatilityFilter = Math.Max(0.000, value); }
        }
		
		[Description("Determines trend by price longerTermPeriod bars ago")]
        [GridCategory("Parameters")]
        public int LongerTermPeriod
        {
            get { return longerTermPeriod; }
            set { longerTermPeriod = Math.Max(0, value); }
        }
		
//        [Description("Trade only longs")]
//        [GridCategory("Parameters")]
//        public bool LongOnly
//        {
//            get { return longOnly; }
//            set { longOnly = value; }
//        }
//
        [Description("Loss in dollars of a $5000 investment")]
        [GridCategory("Parameters")]
        public double StopLoss
        {
            get { return stopLoss; }
            set { stopLoss = Math.Max(0.000, value); }
        }
		
        [Description("Profit taget, use 0 to ignore")]
        [GridCategory("Parameters")]
        public double ProfitTarget
        {
            get { return profitTarget; }
            set { profitTarget = Math.Max(0, value); }
        }
        #endregion
    }
}
