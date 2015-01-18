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
	/// 
	/// This idea is to leverage the EMA_Colors_Paint_v01 indicator and scalp ticks from it on change of slope.
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
    [Description("EMA_Colors_Paint_v01")]
    public class KeyIdea12 : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int iparm1 = 7; // Default setting for Iparm1
        private int iparm2 = 5; // Default setting for Iparm2
        private double dparm1 = 10; // Default setting for Dparm1
        private double dparm2 = 10; // Default setting for Dparm2
		private double ratched = 0.05;
        private int period1 = 10; // Default setting for Period1
        private int period2 = 27; // Default setting for Period2
		private bool isMo = true;
		private bool is2nd = false;
        // User defined variables (add any user defined variables below)
		
		// ATRTrailing parms
		ATRTrailing atr = null;
		double atrTimes = 3.5; 
		int atrPeriod = 10; 
		
		//  KeltnerChannel parms
		KeltnerChannel kc = null;
		double offsetMultiplier = 1.5;
		int keltnerPeriod = 10; 
		
		// DonchianChannel parms
		DonchianChannel dc = null;
		int donchianPeriod = 20;
		
		// MACD
		MACDrick macd = null;
		
		// SMA
		SMA sma = null;
		
//		EMA_Colors_Paint_v01 ema = null;
		
		// FiveBarPattern
		FiveBarPattern fbp;
		double lastFiveBarPatternLow = 0;
		double lastFiveBarPatternHigh = 0;
		
		double range = 0;
		double lineLevel = 0;
		double bullLimit = 0;
		double bullStop = 0;
		int lastLow = 0;  // bar number where the last lwo occurred
		int lastHigh = 0; // bar number where the last high occurred
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
		/// 
        /// </summary>
        protected override void Initialize()
        {
			ClearOutputWindow();

//			ema = EMA_Colors_Paint_v01(30, 30, Period1);
//			Add(ema);
			
			sma = SMA(50);
			sma.Plots[0].Pen.Color = Color.Blue;
			sma.PaintPriceMarkers = false;
			sma.AutoScale = false;
			Add(sma);
			
			//macd = MACDrick(5, 20, 30);
			//Add(macd);
			
//			fbp = FiveBarPattern();
//			Add(fbp);
			
			//Add(FiveBarPattern());
			Add(FiveBarSample());
			
			Add(PitColor(Color.Black, 83000, 25, 161500));
			
            SetProfitTarget(CalculationMode.Ticks, dparm1);
            //SetStopLoss("", CalculationMode.Ticks, dparm2, false);
            //SetTrailStop("", CalculationMode.Ticks, dparm2, false);

            CalculateOnBarClose = true;
			ExitOnClose = true;
			IncludeCommission = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()			
        {
			// If it's Monday, do not trade.
    		//if (Time[0].DayOfWeek == DayOfWeek.Thursday)

//			if (ToTime(Time[0]) < 900 * 100
//				|| ToTime(Time[0]) > 1500 * 100)
//			{
//				if (isShort())
//				{
//					ExitShort();
//				}
//				if (isLong())
//				{
//					ExitLong();
//				}
//				return;
//			}

			
			//Print(Time + " fbp.lower: " + FiveBarPattern().FiveBarLower.ContainsValue(2));
			
//			if (FiveBarPattern().FiveBarLower.ContainsValue(2))
//			{
//				lastFiveBarPatternLow = FiveBarPattern().FiveBarLower[2];
//				Print(Time + " lastFiveBarPatternLow: " + lastFiveBarPatternLow);
//			} else {
//				Print(Time + " ---------------------- " + FiveBarPattern().FiveBarLower[2]);
//			}
//			if (FiveBarPattern().FiveBarLower.ContainsValue(2))
//				Print(Time + " 2 - CurrentBar: " + CurrentBar + " FiveBarLower: " + FiveBarPattern().FiveBarLower[2]);
			
//			if (FiveBarSample().FiveBarLower.ContainsValue(0))
//				Print(Time + " 0 - CurrentBar: " + CurrentBar + " FiveBarLower: " + FiveBarSample().FiveBarLower[0]);
//			if (FiveBarSample().FiveBarLower.ContainsValue(2))
//				Print(Time + " 2 - CurrentBar: " + CurrentBar + " FiveBarLower: " + FiveBarSample().FiveBarLower[2]);
			
			// -------------------------------------------------------------------------
			// For some reason, this has to be here for the other ContainsValues to work
			// -------------------------------------------------------------------------
			if (FiveBarSample().FiveBarLower.ContainsValue(0))
				Print(Time + " 0 - CurrentBar: " + CurrentBar + " FiveBarLower: " + FiveBarSample().FiveBarLower[0]);
			
			if (FiveBarSample().FiveBarLower.ContainsValue(2))
			{
				if (lastHigh > lastLow || lastLow == 0)
				{
					lastFiveBarPatternLow = Low[2];
					lastLow = CurrentBar - 2;
					DrawDot(CurrentBar + "ll", false, 2, Low[2] - 5 * TickSize, Color.Black);
					//Print(Time + " 2 - CurrentBar: " + CurrentBar + " FiveBarLower: " + lastFiveBarSampleLow);
					
					if (isBear())
					{
						range = lastFiveBarPatternHigh - lastFiveBarPatternLow;
						if (range >= 6)
						{
							//Print(Time + " adding bear rectangle " + lastFiveBarPatternLow + " - " + lastFiveBarPatternHigh);
							DrawRectangle(CurrentBar + " 50% " + lastFiveBarPatternHigh, false, 2, range * 0.47 + lastFiveBarPatternLow, -13, 
									range * 0.53 + lastFiveBarPatternLow, Color.LightPink, Color.LightPink, 2);
							// Limit
							lineLevel = Instrument.MasterInstrument.Round2TickSize(lastFiveBarPatternLow - range * 0.68);
							DrawLine(CurrentBar + " limit", 2, lineLevel, -13, lineLevel, Color.Green);
							// Stop
							lineLevel = Instrument.MasterInstrument.Round2TickSize(lastFiveBarPatternLow + range * 0.85);
							DrawLine(CurrentBar + " stop", 2, lineLevel, -13, lineLevel, Color.Red);
						}
					}
				}
			}
			if (FiveBarSample().FiveBarUpper.ContainsValue(2))
			{
//				if (lastHigh == 0)
//				{
					lastFiveBarPatternHigh = High[2];
					lastHigh = CurrentBar - 2;
					DrawDot(CurrentBar + "lh", false, 2, High[2] + 5 * TickSize, Color.Black);
					//Print(Time + " FiveBarUpper: " + lastFiveBarPatternHigh);
					
					if (isBullTrend())
					{
						range = lastFiveBarPatternHigh - lastFiveBarPatternLow;
						if (range >= 6)
						{
							Double bullMidPoint = range * 0.50 + lastFiveBarPatternLow;
							bullLimit = Instrument.MasterInstrument.Round2TickSize(lastFiveBarPatternHigh + range * 0.68);
							bullStop = Instrument.MasterInstrument.Round2TickSize(lastFiveBarPatternLow + range * 0.15);
							
							DrawRectangle(CurrentBar + " 50% " + lastFiveBarPatternHigh, false, 2, range * 0.47 + lastFiveBarPatternLow, -13, 
									range * 0.53 + lastFiveBarPatternLow, Color.PaleGreen, Color.PaleGreen, 2);
							
							double rewardToRiskRatio = Math.Round((bullLimit - bullMidPoint)/(bullMidPoint - bullStop), 1);
							// Trade details
							DrawText(CurrentBar + "txt", "R:R " + rewardToRiskRatio + ":1", -2, bullMidPoint, Color.Black);
							// Limit
							DrawLine(CurrentBar + " limit", 2, bullLimit, -13, bullLimit, Color.Green);
							// Stop
							DrawLine(CurrentBar + " stop", 2, bullStop, -13, bullStop, Color.Red);
						}
					}
//				}
			}
			
			
//			if (ema.EMAslopeup.ContainsValue(0))
//			{
//				//Print(Time + " EMAslopeup: " + ema.EMAslopeup[0]);
//			} else {
//				//Print(Time + " ++++++++++ " + ema.EMAslopeup[0]);
//			}
			
//			if (Position.MarketPosition == MarketPosition.Flat) 
//			{
//				//SetStopLoss(CalculationMode.Ticks, 1000);
//				if (isBullTrend())
//				{
//					if (isShort())
//					{
//						ExitShort();
//					}
//					
//					if (isFlat()) 
//					{
//						//EnterLong();
//						EnterLongStop(High[0] + 2 * TickSize);
//						SetStopLoss(CalculationMode.Ticks, 16); //lastFiveBarPatternLow);
//					}
//					
//				}
//				if (isBear())
//				{
//					if (isLong())
//					{
//						ExitLong();
//					}
//					
//				    //DrawDot(CurrentBar + "Bear", true, 0, Low[0] - 2 * TickSize, Color.Red);
//					if (isFlat()) 
//					{
//						EnterShort();
//					}
//				}
//			}
        }
		
		private bool isBullTrend()
		{
			if (Rising(sma))
				return true;
			else
				return false;
		}
		
		private bool isBear()
		{
			if (Falling(sma))
				return true;
			else
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
		
		protected override void OnOrderUpdate(IOrder order)
		{
//			if (entryOrder != null && entryOrder == order)
//			{
//				Print(order.ToString());
//				if (order.OrderState == OrderState.Filled)
//					entryOrder = null;
//			}
		}
		
		protected override void OnTermination()
		{
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
		
        [Description("")]
        [GridCategory("Parameters")]
        public double Ratched
        {
            get { return ratched; }
            set { ratched = Math.Max(0.000, value); }
        }

        #endregion
    }
}
