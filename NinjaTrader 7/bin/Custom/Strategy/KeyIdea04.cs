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
    [Description("CL 20 Range Bars - 8/16/2011 start of CL tick data")]
    public class KeyIdea04 : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int iparm1 = 0; // Default setting for Iparm1  ---- exit after x bars
        private int iparm2 = 1; // Default setting for Iparm2
        private double dparm1 = 7.0; // Default setting for Dparm1
        private double dparm2 = 0.01; // Default setting for Dparm2 --- profit target
		private double ratched = 0.02;
        private int period1 = 10; // Default setting for Period1
        private int period2 = 27; // Default setting for Period2
		private bool isMo = true;
		private bool is2nd = false;
        // User defined variables (add any user defined variables below)
		
		// ATRTrailing parms
		ATRTrailing atr = null;
		double atrTimes = 3.0; 
		int atrPeriod = 10; 
		//double ratched = 0.00;
		
		//  KeltnerChannel parms
		KeltnerChannel kc = null;
		double offsetMultiplier = 1.5;
		int keltnerPeriod = 10; 
		
		// DonchianChannel parms
		DonchianChannel dc = null;
		int donchianPeriod = 20;
		
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
			atrTimes = dparm1;
			atrPeriod = period1;
			//ratched = 0;
			
			atr = ATRTrailing(atrTimes, Period1, Ratched);
			Add(atr);
			
			kc = KeltnerChannel(offsetMultiplier, keltnerPeriod);
			Add(kc);
			
			dc = DonchianChannel(donchianPeriod);
			dc.Displacement = 2;
			dc.PaintPriceMarkers = false;
			Add(dc);
			
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
			/*
			if (ToDay(Time[0]) >= 20130226 && ToDay(Time[0]) <= 20130325)
			{
				atrTimes = 3.25;
			}
			else if (ToDay(Time[0]) <= 20130422)
			{
				atrTimes = 4;
			}
			else if (ToDay(Time[0]) <= 20130520)
			{
				atrTimes = 4;
			}
			else if (ToDay(Time[0]) <= 20130617)
			{
				atrTimes = 4;
			}
			else if (ToDay(Time[0]) <= 20130715)
			{
				atrTimes = 3.5;
			}
			else if (ToDay(Time[0]) <= 20130812)
			{
				atrTimes = 3.5;
			}
			else if (ToDay(Time[0]) <= 20130909)
			{
				atrTimes = 4;
			}
			else if (ToDay(Time[0]) <= 20131007)
			{
				atrTimes = 4;
			}
			else if (ToDay(Time[0]) <= 20131104)
			{
				atrTimes = 3;
			}
			else if (ToDay(Time[0]) <= 20131202)
			{
				atrTimes = 3;
			}

			else if (ToDay(Time[0]) <= 20140127)
			{
				atrTimes = 3.75;
			}
			else if (ToDay(Time[0]) <= 20140224)
			{
				atrTimes = 3.5;
			}
			else if (ToDay(Time[0]) <= 20140324)
			{
				atrTimes = 4;
			}
			else if (ToDay(Time[0]) <= 20140421)
			{
				atrTimes = 4;
			}
			else if (ToDay(Time[0]) <= 20140519)
			{
				atrTimes = 3;
			}
			else if (ToDay(Time[0]) <= 20140616)
			{
				atrTimes = 2.25;
			}
			else if (ToDay(Time[0]) <= 20140714)
			{
				atrTimes = 2;
			}
			else if (ToDay(Time[0]) <= 20140811)
			{
				atrTimes = 2;
			}
			else if (ToDay(Time[0]) <= 20140908)
			{
				atrTimes = 3.75;
			}
			else if (ToDay(Time[0]) <= 20141006)
			{
				atrTimes = 3.75;
			}
			else if (ToDay(Time[0]) <= 20141011)
			{
				atrTimes = 3.5;
			}
			
			atr = ATRTrailing(atrTimes, atrPeriod, ratched);
			*/
			if (Position.MarketPosition == MarketPosition.Flat) 
			{
				SetStopLoss(CalculationMode.Ticks, 1000);
				if (isBull())
				{
					if (Is2nd && Close[0] < kc.Midline[0])
					{
						//DrawDot(CurrentBar + "Bull", true, 0, kc.Midline[0], Color.Blue);
						EnterLongStop(kc.Midline[0]);
					}					
					if (IsMo && Low[0] > dc.Upper[2])
					{
						EnterLong();
					}					
				}
				if (isBear())
				{
					if (Is2nd && Close[0] > kc.Midline[0])
					{
						//DrawDot(CurrentBar + "Bear", true, 0, kc.Midline[0], Color.Red);
						EnterShortStop(kc.Midline[0]);
					}
					if (IsMo && High[0] < dc.Lower[2])
					{
						EnterShort();
					}					
				}
			} 
			else
			{
				if (Iparm1 > 0 && BarsSinceEntry() >= Iparm1)
				{
					if (isLong())
						ExitLong();
					if (isShort())
						ExitShort();
				}
				
				if (isLong())
				{
//						if (!atr.Upper.ContainsValue(0))
//						{
//							ExitLong();
//						}
//						else 
//						{
					SetStopLoss(CalculationMode.Price, atr.Upper[0]);
							//DrawDiamond(CurrentBar + "Stop", true, 0, atr.Upper[0], Color.Black);
//						}
				}
				if (isShort())// && !atr.Lower.ContainsValue(0))
				{
					SetStopLoss(CalculationMode.Price, atr.Lower[0]);
						//DrawDiamond(CurrentBar + "Stop", true, 0, atr.Lower[0], Color.Black);
						//ExitShort();
				}
			}
        }

		private bool isBull()
		{
			//if (HighestBar(High, period1) == 0 && Close[0] > Close[1])
			if (atr.Upper[0] < Low[0])
			{
				return true;
			}
			return false;
		}
		
		private bool isBear()
		{
			//if (LowestBar(Low, period2) == 0 && Close[0] < Close[1])
			if (atr.Lower.ContainsValue(0))
			{
				return true;
			}
			return false;
		}
		
		private bool isFlat()
		{
			if (Position.MarketPosition == MarketPosition.Flat)
				return true;
			else
				return false;
		}
		
		private bool isLong()
		{
			if (Position.MarketPosition == MarketPosition.Long)
				return true;
			else
				return false;
		}
		
		private bool isShort()
		{
			if (Position.MarketPosition == MarketPosition.Short)
				return true;
			else
				return false;
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

        [Description("Mo Entries")]
        [GridCategory("Parameters")]
        public bool IsMo
        {
            get { return isMo; }
            set { isMo = value; }
        }

        [Description("2nd Chance")]
        [GridCategory("Parameters")]
        public bool Is2nd
        {
            get { return is2nd; }
            set { is2nd = value; }
        }
		
        #endregion
        [Description("")]
        [GridCategory("Parameters")]
        public double Ratched
        {
            get { return ratched; }
            set { ratched = Math.Max(0.000, value); }
        }

    }
}
