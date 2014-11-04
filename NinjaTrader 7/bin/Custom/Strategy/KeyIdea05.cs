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
	/// This strategy establishes 3 ETF positions based on TrendStregth.  It appears to use about 
	/// a 15% trailing stop and three exit levels.
	/// 
	/// Use SSO as the default ETF
	/// 
    /// Used to test new key trading ideas - this should be used only as a template.
	/// 
	/// Step 1: KeyIdea or trade idea 
	/// 		pseudo code entry
	/// 		example entry: if close > highest(close, x) then buy next bar at market
	/// 		exit: reverse position when entry reverses
	/// 		   or technical-based - support/resistance, moving average 
	///                 needs to work with entry to not give conflicting signals
	///            breakeven stops, stop-loss, profit targets, trailing stops 
	/// 		market selection
	/// 			oil, index etc
	/// 			over night multi-day 
	/// 			day trading
	/// 		time frame/bar size
	/// 			range
	/// 			tick
	/// 			time
	/// 		data considerations 
	/// 			not much can be done with ninja - continuous adjusted historical
	/// 	limited texting - check if there's any merit to the entry idea
	/// 		test against only 10-20% of data
	/// 			Entry Testing
	/// 				usefullness test - without commission/slippage goal > 52% profitable
	/// 					Fixed-Stop and Target Exit
	/// 						SetProfitTarget
	/// 						SetStopLoss
	/// 					Fixed-Bar Exit
	/// 					Random Exit
	/// 	
    /// </summary>
    [Description("Daily ETFs - Based on MarketGuage Country ETFs")]
    public class KeyIdea05 : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int iparm1 = 0; // Default setting for Iparm1  ---- exit after x bars
        private int iparm2 = 1; // Default setting for Iparm2
        private double dparm1 = 7.0; // Default setting for Dparm1
        private double dparm2 = 0.15; // Default setting for Dparm2 --- trailing stop
		private double ratched = 0.02;
        private int period1 = 10; // Default setting for Period1
        private int period2 = 27; // Default setting for Period2
		private bool isMo = true;
		private bool is2nd = false;
        // User defined variables (add any user defined variables below)
		
		// ATRTrailing parms
//		ATRTrailing atr = null;
//		double atrTimes = 3.0; 
//		int atrPeriod = 10; 
//		//double ratched = 0.00;
//		
//		//  KeltnerChannel parms
//		KeltnerChannel kc = null;
//		double offsetMultiplier = 1.5;
//		int keltnerPeriod = 10; 
//		
//		// DonchianChannel parms
//		DonchianChannel dc = null;
//		int donchianPeriod = 20;
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
		/// 
		/// I was thinking that MOMO could have not atr ratched but use a stop place where the
		/// prior atr stop was, for example, if a short momo, use the lower blue level as the 
		/// max high to trigger a sell.
        /// </summary>
        protected override void Initialize()
        {
			// Americas
			Add("EWZ", PeriodType.Day, 1);
			Add("EWW", PeriodType.Day, 1);
//			
//			// Europe
			Add("EWU", PeriodType.Day, 1);
			Add("EWG", PeriodType.Day, 1);
//	
//			// Middle East
//			Add("EGPT", PeriodType.Day, 1);
//			
//			// Asia
//			Add("FXI", PeriodType.Day, 1);
//			Add("EWJ", PeriodType.Day, 1);
//			
//			// Alternative
//			Add("EFZ", PeriodType.Day, 1);
//			Add("TMF", PeriodType.Day, 1);
			
//			atrTimes = dparm1;
//			atrPeriod = period1;
			//ratched = 0;
			
			Add(TSI(3, 14));
			
//			atr = ATRTrailing(atrTimes, Period1, Ratched);
//			Add(atr);
//			
//			kc = KeltnerChannel(offsetMultiplier, keltnerPeriod);
//			Add(kc);
//			
//			dc = DonchianChannel(donchianPeriod);
//			dc.Displacement = 2;
//			dc.PaintPriceMarkers = false;
//			Add(dc);
			
            SetProfitTarget("", CalculationMode.Percent, dparm2);
            SetStopLoss("", CalculationMode.Percent, dparm2, false);
            //SetTrailStop("", CalculationMode.Percent, dparm2, false);

            CalculateOnBarClose = true;
			ExitOnClose = false;
			IncludeCommission = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()			
        {
		    // Checks if OnBarUpdate() is called from an update on the primary Bars
			//if (BarsInProgress == 0)
			
				if (isBestLong(BarsInProgress))
				{
					if (Position.MarketPosition == MarketPosition.Short)
					{
						ExitShort();
					}					
					if (Position.MarketPosition == MarketPosition.Flat)
					{
						EnterLong("Long " + Instrument.FullName);						
					} 
				}
				
				if (isBestShort(BarsInProgress))
				{
					if (Position.MarketPosition == MarketPosition.Long)
					{
						ExitLong();
					}
					if (Position.MarketPosition == MarketPosition.Flat)
					{
						EnterShort("Short " + Instrument.FullName);						
					}
				} 
			
				
        }

		private bool isBestLong(int i) 
		{
			bool best = true;
			if (i != 0 && TSI(BarsArray[i], 3, 14)[0] < TSI(BarsArray[0], 3, 14)[0])
			{
				best = false;
			}
			if (i != 1 && TSI(BarsArray[i], 3, 14)[0] < TSI(BarsArray[1], 3, 14)[0])
			{
				best = false;
			}
			if (i != 2 && TSI(BarsArray[i], 3, 14)[0] < TSI(BarsArray[2], 3, 14)[0])
			{
				best = false;			
			}
			if (i > 2)
				return false;
				
			return best;
		}
		
		private bool isBestShort(int i) 
		{
			bool best = true;
			if (i != 0 && TSI(BarsArray[i], 3, 14)[0] > TSI(BarsArray[0], 3, 14)[0])
				best = false;
			if (i != 1 && TSI(BarsArray[i], 3, 14)[0] > TSI(BarsArray[1], 3, 14)[0])
				best = false;
			if (i != 2 && TSI(BarsArray[i], 3, 14)[0] > TSI(BarsArray[2], 3, 14)[0])
			{
				best = false;			
			}
			if (i > 2)
				return false;
			return best;
		}
		
        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int Iparm1
        {
            get { return iparm1; }
            set { iparm1 = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Iparm2
        {
            get { return iparm2; }
            set { iparm2 = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Period1
        {
            get { return period1; }
            set { period1 = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Period2
        {
            get { return period2; }
            set { period2 = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double Dparm1
        {
            get { return dparm1; }
            set { dparm1 = Math.Max(0.000, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double Dparm2
        {
            get { return dparm2; }
            set { dparm2 = Math.Max(0.000, value); }
        }

		
        #endregion

    }
    
}
