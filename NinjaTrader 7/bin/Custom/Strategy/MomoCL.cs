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
    /// Will Trade MOMO Trades
	/// The following results are before the hour/day screening was added
	/// CL 133t - 5, 0.004, 10, 23, 38, 134, 8 1/12->9/7/12  $29,510
	/// CL 144t
	/// CL 233t - 8, 0.001, 9, 19, 18, 45, 6  1/12->9/7/12  $45,615
	/// CL 333t - 10, 0.004, 12, 23, 18, 45, 9 1/12->9/7/12  $61,695
	/// CL 400t
	/// CL 512t - 4, 0.004, 8, 17, 10, 30, 5 1/12->9/7/12  $45,265
	/// CL 1600t
	/// CL 3200t
	/// CL Renko2 - 11, 0.000, 16, 15, 18, 45, 14 1/12->9/7/12  $47,720
	/// CL Renko2 - 11, 0.000, 16, 15, 26, 198, 12 1/12->9/7/12  $46,975
	/// CL Renko5 - 10, 0.000, 9, 25, 26, 198, 12 1/12->9/7/12  $56,380  - max dd $6,240
	/// 
    /// </summary>
    [Description("Will Trade MOMO Trades")]
    public class MomoCL : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int aTRPeriod = 10; // Default setting for ATRPeriod
        private double aTRRatched = 0.004; // Default setting for ATRRatched
        private double aTRTimes = 12; // Default setting for ATRTimes
        private int donchianPeriod = 24; // Default setting for DonchianPeriod
		private bool enableDebug = false;
		private bool enableLong = true;
		private bool enableShort = true;
        private int mACDFast = 18; // Default setting for MACDFast
        private int mACDSlow = 45; // Default setting for MACDSlow
        private int mACDSmooth = 9; // Default setting for ATRPeriod

		// ATM Stop Stategy Variables
		//private int ssAbePartialAdjustment1 = 0;//8;  // Amount to move stopLoss to
		private int ssAbeProfitTriggerL = 76; // Stop Strategy, Auto breakeven longs(ticks)
		private int ssAbeProfitTriggerS = 59; // Stop Strategy, Auto breakeven shorts(ticks)
		//private int ssAbeProfitTrigger1 = 0; //7; // Stop Strategy, Partial move to breakeven (ticks)
		private int ssAtProfitTrigger = 0;//28;	// Start auto adjusting our Stop Loss if this value is reached
		private int ssAtStopLossL = 87;		// Trailing stop for longs
		private int ssAtStopLossS = 46;		// Trailing stop for shorts

		// User defined variables (add any user defined variables below)
		private int disp = 2;
		//private double approxEntry = 0.00;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            Add(DonchianRick(DonchianPeriod));
            Add(MACDrick(mACDFast, mACDSlow, mACDSmooth));
            Add(ATRTrailing(ATRTimes, ATRPeriod, ATRRatched));

			//SetStopLoss(CalculationMode.Ticks, StopLoss);
			//SetProfitTarget(CalculationMode.Ticks, ProfitTarget);
			//SetTrailStop(CalculationMode.Ticks, TrailingStop);
			
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			
			if (BarsPeriod.Id != PeriodType.Tick) {
				Print("This strategy requires Tick Bars");
				return;
			}
			if (BarsPeriod.Value != 333) {
				Print("This strategy requires 333 Ticks/Bar");
				return;
			}
			
			if (Time[0].DayOfWeek != DayOfWeek.Wednesday
				&& ToTime(Time[0]) > ToTime(2, 00, 0)				
				)
			{
				// Open MOMO Long
				if (isFlat()
					&& isMacdPos()
					&& isAlgoStopPos()
					&& Low[0] > DonchianRick(DonchianPeriod).Upper[disp]
					&& Time[0].DayOfWeek != DayOfWeek.Thursday
				//	&& ToTime(Time[0]) > ToTime(1, 00, 0)
					&& enableLong
					)
				{
					EnterLong(DefaultQuantity, "MOMO Long");
				}
				
				// Open MOMO Short
				if (isFlat()
					&& isMacdNeg()
					&& isAlgoStopNeg()
					&& High[0] < DonchianRick(DonchianPeriod).Lower[disp]
					&& Time[0].DayOfWeek != DayOfWeek.Friday
					&& enableShort
					)
				{
					EnterShort(DefaultQuantity, "MOMO Short");
				}
			}
			
            if (!isFlat())
			{
				// Close Long
				if (Position.MarketPosition == MarketPosition.Long)
				{
					// $54,420
					//ExitLongStop(ATRTrailing(ATRTimes, ATRPeriod, ATRRatched).Upper[0]);
					// $57,765 using a stop loss with a break even trigger					
					closeLong();
            	}
				
				// Close Short
				if (Position.MarketPosition == MarketPosition.Short)
				{
					//ExitShortStop(ATRTrailing(ATRTimes, ATRPeriod, ATRRatched).Lower[0]);
					closeShort();
            	}
			}
			
        }

		protected void closeLong() {
			
			if (Position.MarketPosition != MarketPosition.Long) 
				return;
			
			double stopPrice = 0.0;
			double approxEntry = Instrument.MasterInstrument.Round2TickSize(Position.AvgPrice);
			
			double highSinceOpen = High[HighestBar(High, BarsSinceEntry())];
			//double tStopPrice = highSinceOpen - (ssAtProfitTrigger * TickSize);

			// set breakeven if triggered										
			if (highSinceOpen >= (approxEntry + (ssAbeProfitTriggerL * TickSize))) {
				stopPrice = approxEntry;
				//Print("Set to B/E: " + stopPrice);
			//} else { 
			//	if (highSinceOpen >= (approxEntry + (ssAbeProfitTriggerL1 * TickSize))) {
			//		stopPrice = approxEntry - (ssAbePartialAdjustment1 * TickSize);
			//		//Print("Moved up to just below B/E: " + stopPrice);
			//	} else {
					//stopPrice = (approxEntry - (ssAtStopLossL * TickSize));
					//Print("Moved up to cover stop loss: " + stopPrice);
			//	}
			}
				
			//stopPrice = Math.Max(tStopPrice, stopPrice);
			stopPrice = Math.Max(ATRTrailing(ATRTimes, ATRPeriod, ATRRatched).Upper[0], stopPrice);
			stopPrice = Math.Max(stopPrice, (approxEntry - (ssAtStopLossL * TickSize)));		

			if (enableDebug) {
				DrawDiamond(CurrentBar.ToString()+"Stop", true, 0, stopPrice, Color.Green);

				if (BarsSinceEntry() == -10) {
					Print("----------LONGSTOP------------");
					Print(BarsSinceEntry() + ": approxEntry@" + approxEntry);
					Print("highSinceOpen: " + highSinceOpen);
					Print("ssAtProfitTrigger: " + ssAtProfitTrigger + ", TickSize: " + TickSize + " *= " + (ssAtProfitTrigger * TickSize)); // + " --> tStopPrice: " + tStopPrice);
					Print("(approxEntry - (sAtStopLossL * TickSize)" + approxEntry + "-" + ssAtStopLossL + "*" + TickSize + "=" + (approxEntry - (ssAtStopLossL * TickSize)));
					Print("Calculated stopPrice: " + stopPrice); 
				}				
			}
			if (Close[0] <= stopPrice)
				ExitLong();
			else
				ExitLongStop(stopPrice);
		}

		protected void closeShort() {

			if (Position.MarketPosition != MarketPosition.Short) 
				return;
			
			double stopPrice = Double.MaxValue;
			double approxEntry = Instrument.MasterInstrument.Round2TickSize(Position.AvgPrice);
			double lowSinceOpen = Low[LowestBar(Low, BarsSinceEntry())];
//			double tStopPrice = lowSinceOpen + (ssAtProfitTrigger * TickSize);			
			
			// set breakeven if triggered			
			if (lowSinceOpen <= (approxEntry - (ssAbeProfitTriggerS * TickSize))) {
				stopPrice = approxEntry;
			} //else { 
				// check for partial move to b/e
//				if (lowSinceOpen <= (approxEntry - (ssAbeProfitTrigger1 * TickSize))) {
//					stopPrice = approxEntry + (ssAbePartialAdjustment1 * TickSize);
//				} else {
					// worse case, starting stop loss setting
				//	stopPrice = (approxEntry + (ssAtStopLossS * TickSize));
//				}
			//}
			
//			stopPrice = Math.Min(tStopPrice, stopPrice);
			stopPrice = Math.Min(ATRTrailing(ATRTimes, ATRPeriod, ATRRatched).Lower[0], stopPrice);
			stopPrice = Math.Min(stopPrice, (approxEntry + (ssAtStopLossS * TickSize)));		
			
			if (enableDebug) {
				DrawDiamond(CurrentBar.ToString()+"sdia", true, 0, stopPrice, Color.Yellow);
				//Print (Time[0] + " --PROCESS-- " + BarsSinceEntry() + "/" + BarsSinceExit() + " ----" + stopPrice);
				
				if (BarsSinceEntry() < -1) {
					Print(Time[0] + " ----------------------");
					Print(BarsSinceEntry() + ": approxEntry@" + approxEntry);
					Print("lowSinceOpen: " + lowSinceOpen);
					Print("ssAtProfitTrigger: " + ssAtProfitTrigger + ", TickSize: " + TickSize + " *= " + (ssAbeProfitTriggerS * TickSize)); // + " --> tStopPrice: " + tStopPrice);
					Print("(approxEntry + (ssAtStopLossS * TickSize) -> " + approxEntry + " + " + ssAtStopLossS + " * " + TickSize + " = " + (approxEntry + (ssAtStopLossS * TickSize)));
					Print("Calculated stopPrice: " + stopPrice); 
				}				
			}			
			if (Close[0] >= stopPrice) {
				//if (enableDebug) Print("ExitShort");
				ExitShort();
			} else {
				//if (enableDebug) Print("ExitShortStop Close[0]: " + Close[0] + " stopPrice: " + stopPrice);
				ExitShortStop(stopPrice);
			}
		}

		protected override void OnExecution(IExecution execution)		
		{
			//if (execution.Order != null && execution.Order.OrderState == OrderState.Filled)
		    //	Print(execution.ToString());
			// execution.Name; -- orderEntryName
			if (Performance.RealtimeTrades.Count > 0)
			{
				// Get the last completed real-time trade (at index 0)
				Trade lastTrade = Performance.RealtimeTrades[Performance.RealtimeTrades.Count - 1];
		
				// Calculate the PnL for the last completed real-time trade
				double lastProfit = lastTrade.ProfitCurrency * lastTrade.Quantity;
		
				// Pring the PnL to the Output window
				Print("The last trade's profit is " + lastProfit);
			}
		}
		
		
		#region utils
		protected Boolean isAlgoStopPos() {
			return ATRTrailing(ATRTimes, ATRPeriod, ATRRatched).Upper[0] < Low[0];	
		}

		protected Boolean isAlgoStopNeg() {
			return ATRTrailing(ATRTimes, ATRPeriod, ATRRatched).Lower[0] > High[0];	
		}
		
		protected Boolean isMacdPos() {
			return MACDrick(mACDFast, mACDSlow, mACDSmooth).Diff[0] > 0;	
		}
		
		protected Boolean isMacdNeg() {
			return MACDrick(mACDFast, mACDSlow, mACDSmooth).Diff[0] <= 0;	
		}
		
		protected Boolean isFlat() {
			return Position.MarketPosition == MarketPosition.Flat;
		}

		protected void colorBars() 
		{
            //if (Rising(HMARick(HullPeriod)) == true)
            //{
            //    BarColor = Color.Blue;
            //}
            //if (Falling(HMARick(HullPeriod)) == true)
            //{
             //   BarColor = Color.Red;
            //}
		}
		#endregion
		
        #region Properties
		
		[Description("Enables or disables developer troubleshooting symbols")]
        [GridCategory("Parameters")]
        public bool EnableDebug
        {
            get { return enableDebug; }
            set { enableDebug = value; }
        }
		
		[Description("Enables or disables long trades")]
        [GridCategory("Parameters")]
        public bool EnableLong
        {
            get { return enableLong; }
            set { enableLong = value; }
        }
		
		[Description("Enables or disables short trades")]
        [GridCategory("Parameters")]
        public bool EnableShort
        {
            get { return enableShort; }
            set { enableShort = value; }
        }

		/*
		[Description("Price target of when to move stop ssAbePartialAdjustment1 ticks away from open")]
        [GridCategory("Parameters")]
        public int SsAbeProfitTrigger1
        {
            get { return ssAbeProfitTrigger1; }
            set { ssAbeProfitTrigger1  = Math.Max(1, value); }
        }

		[Description("Number of ticks from open to move on partial adjustment to b/e")]
        [GridCategory("Parameters")]
        public int SsAbePartialAdjustment1
        {
            get { return ssAbePartialAdjustment1; }
            set { ssAbePartialAdjustment1  = Math.Max(1, value); }
        }
*/
		[Description("Stop Strategy, Auto breakeven for Long trades (ticks)")]
        [GridCategory("Parameters")]
        public int SsAbeProfitTriggerL		
        {
            get { return ssAbeProfitTriggerL; }
            set { ssAbeProfitTriggerL  = Math.Max(1, value); }
        }

		[Description("Stop Strategy, Auto breakeven for Short trades (ticks)")]
        [GridCategory("Parameters")]
        public int SsAbeProfitTriggerS		
        {
            get { return ssAbeProfitTriggerS; }
            set { ssAbeProfitTriggerS  = Math.Max(1, value); }
        }

		[Description("Trailing stop, start auto adjusting our Stop Loss if this value is reached")]
        [GridCategory("Parameters")]
        public int SsAtProfitTrigger
        {
            get { return ssAtProfitTrigger; }
            set { ssAtProfitTrigger = Math.Max(1, value); }
        }

		[Description("Stop loss setting for shorts")]
        [GridCategory("Parameters")]
        public int SsAtStopLossS
        {
            get { return ssAtStopLossS; }
            set { ssAtStopLossS  = Math.Max(1, value); }
        }

		[Description("Stop loss setting for longs")]
        [GridCategory("Parameters")]
        public int SsAtStopLossL
        {
            get { return ssAtStopLossL; }
            set { ssAtStopLossL  = Math.Max(1, value); }
        }

		
		
		/*
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
        public int StopLoss
        {
            get { return stopLoss; }
            set { stopLoss = Math.Max(1, value); }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public int StopLossShort
        {
            get { return stopLossShort; }
            set { stopLossShort = Math.Max(1, value); }
        }
*/
//        [Description("")]
//        [GridCategory("Parameters")]
//        public int HullPeriod
//        {
//            get { return hullPeriod; }
//            set { hullPeriod = Math.Max(1, value); }
//        }

        [Description("")]
        [GridCategory("Parameters")]
        public int DonchianPeriod
        {
            get { return donchianPeriod; }
            set { donchianPeriod = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MACDFast
        {
            get { return mACDFast; }
            set { mACDFast = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MACDSlow
        {
            get { return mACDSlow; }
            set { mACDSlow = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MACDSmooth
        {
            get { return mACDSmooth; }
            set { mACDSmooth = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double ATRTimes
        {
            get { return aTRTimes; }
            set { aTRTimes = Math.Max(1.0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int ATRPeriod
        {
            get { return aTRPeriod; }
            set { aTRPeriod = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double ATRRatched
        {
            get { return aTRRatched; }
            set { aTRRatched = Math.Max(0.000, value); }
        }
        #endregion
    }
}
