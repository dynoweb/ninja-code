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
    /// Trades Trading Advantage Trades for ES using 333 tick chart
    /// </summary>
    [Description("Trades Trading Advantage Trades for ES using 333 tick chart")]
    public class ES333T : Strategy
    {
		// To match TA's charts, these settings are required
		// ATR Per 10, Rat 0, Times 6, Channel 40, Hull 100, mFast 40, mSlow 120, mSmooth 30
		// Optimized settings for long
		// ATR Per 10, Rat 0, Times 12, Channel 60, Hull 100, mFast 40, mSlow 120, mSmooth 30
		// stopLoss 4, trailingStop 30
        #region Variables
        private int aTRPeriod = 8; 
        private double aTRRatched = 0.000;
        private int aTRTimes = 12;
        private int donchianPeriod = 40; 
		private bool enableLong = true;
		private bool enableMomo = false;
		private bool enableRtm = false;
		private bool enableShort = true;
        private int hullPeriod = 25; 
        private int mACDFast = 40; 
        private int mACDSlow = 120;
		private int mACDSmooth = 30;
		private int maxRiskRTM = 0;
		private int minFlatMeanBars = 0;
		private int profitTarget = 0;
		private int stopLossShort = 0;
		private int trailingStop = 29;
		private int rcPeriods = 147;
		private double rcWidth = 2.0;
		// Stop Stategy Variables
		private int stopLoss = 11;			// Max lost on a single trade (ticks)
		private int ssAbeProfitTrigger1 = 7; // Stop Strategy, Partial move to breakeven (ticks)
		private int ssAbePartialAdjustment1 = 4;  // Amount to move stopLoss to
		private int ssAbeProfitTrigger = 12; // Stop Strategy, Auto breakeven (ticks)
		private int ssAtStopLoss = 12;		// Trailing stop
		private int ssAtProfitTrigger = 16;	// Start auto adjusting our Stop Loss if this value is reached
        // User defined variables (add any user defined variables below)
        private int algoPeriod = 10;
        private double algoRatched = 0.0;
        private int algoTimes = 8;
		private int disp = 2;
		private double approxEntry = 0.00;
		private int rtmFlag = 0;
		private int barNumberOfOrder = 0;	
		private double stopPrice = 0;		// calculated price to place stop order
		private int level = 0;  // -1 indicates price is below, 0 in or overlapping, and 1 above
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            //Add(DonchianRick(DonchianPeriod));
            //Add(MACDrick(mACDFast, mACDSlow, mACDSmooth));
            //Add(ATRTrailing(ATRTimes, ATRPeriod, ATRRatched));
			Add(HMARick(HullPeriod, 30));
			Add(HeikenAshi());
			//Add(ATRTrailing(algoTimes, algoPeriod, algoRatched));
            //Add(RegressionChannel(rcPeriods, rcWidth));
			
			
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
			colorBars();
						
			DrawDot(this.CurrentBar + "Upper", false, 0, RegressionChannel(rcPeriods, rcWidth).Upper[0], Color.Blue);
			DrawDot(this.CurrentBar + "Lower", false, 0, RegressionChannel(rcPeriods, rcWidth).Lower[0], Color.Red);

			if (Low[0] > RegressionChannel(rcPeriods, rcWidth).Upper[0]
				|| (High[0] > RegressionChannel(rcPeriods, rcWidth).Upper[0] && level == 1)) {
				//DrawDot(this.CurrentBar + "hull", false, 0, HMARick(HullPeriod)[0], Color.White);
				level = 1;
			} else if (High[0] < RegressionChannel(rcPeriods, rcWidth).Lower[0] && level == 0
				|| Low[0] < RegressionChannel(rcPeriods, rcWidth).Lower[0] && level == -1) {
				//DrawDot(this.CurrentBar + "hull", false, 0, HMARick(HullPeriod)[0], Color.Black);
				level = -1;
			} else if (Low[0] > RegressionChannel(rcPeriods, rcWidth).Lower[0]
				&& High[0] < RegressionChannel(rcPeriods, rcWidth).Upper[0]) {
				level = 0;
				//DrawDot(this.CurrentBar + "hullx", false, 0, HMARick(HullPeriod)[0], Color.Green);
 			} else {
				level = 0;
				//DrawDot(this.CurrentBar + "hull", false, 0, HMARick(HullPeriod)[0], Color.Yellow);
			}
			
			if (isFlat()
				&& level == -1
				&& Rising(HMARick(HullPeriod, 30))
				&& High[0] >= RegressionChannel(rcPeriods, rcWidth).Lower[0]
				&& Low[0] < RegressionChannel(rcPeriods, rcWidth).Lower[0]
				)
			{
				DrawArrowUp("My up arrow" + CurrentBar, false, 0, RegressionChannel(rcPeriods, rcWidth).Lower[0] - TickSize * 2, Color.Red);
				EnterLongLimit(DefaultQuantity, RegressionChannel(rcPeriods, rcWidth).Lower[0], "RC Long");
				approxEntry = Close[0];
			}
			
			if (isFlat()
				&& level == 1
				&& Falling(HMARick(HullPeriod, 30))
				&& Low[0] <= RegressionChannel(rcPeriods, rcWidth).Upper[0]
				&& High[0] > RegressionChannel(rcPeriods, rcWidth).Upper[0])
			{
				DrawArrowDown("My down arrow" + CurrentBar, false, 0, RegressionChannel(rcPeriods, rcWidth).Upper[0] + TickSize * 2, Color.Red);
				EnterShortLimit(DefaultQuantity, RegressionChannel(rcPeriods, rcWidth).Upper[0], "RC Short");
				approxEntry = Close[0];
			}
			
			if (Position.MarketPosition == MarketPosition.Long)
			{
				if (Falling(HMARick(HullPeriod, 30)) && Low[0] < approxEntry - (stopLoss * TickSize))
				{
					ExitLong("Hull Flip", "RC Long");
				} else {				
					ExitLongLimit(RegressionChannel(rcPeriods, rcWidth).Upper[0], "RC Long");
				}
			}
			
			if (Position.MarketPosition == MarketPosition.Short)
			{
				if (Rising(HMARick(HullPeriod, 30)) && High[0] > approxEntry + (stopLoss * TickSize))
				{
					ExitShort("Hull Flip", "RC Short");
				} else {				
					ExitShortLimit(RegressionChannel(rcPeriods, rcWidth).Lower[0], "RC Short");
				}
			}

			
			
			#region Momo
			if (EnableMomo) 
			{
				// ===========================================================================
				// Condition set - Open MOMO Long
				// ===========================================================================
				if (isFlat()
					&& EnableLong
					&& isAlgoBarPos()
					&& isAlgoStopPos()
					&& isMacdPos()
					&& Low[0] > DonchianRick(DonchianPeriod).Upper[disp]
					)
				{
					EnterLong(DefaultQuantity, "MOMO Long");
					approxEntry = Close[0];
					Print("------> Entered MOMO Long @ " + approxEntry + " " + Position.AvgPrice);
				}
				
				// ===========================================================================
				// Condition set - Open MOMO Short
				// ===========================================================================
				if (isFlat()
					&& EnableShort
					&& isAlgoBarNeg()
					&& isAlgoStopNeg()
					&& isMacdNeg()
					&& High[0] < DonchianRick(DonchianPeriod).Lower[disp]
					)
				{
					EnterShort(DefaultQuantity, "MOMO Short");
					approxEntry = Close[0];
					Print("------> Entered MOMO Short @ " + approxEntry + " " + Position.AvgPrice);
				}
				
				// ===========================================================================
				// Condition set - Close MOMO Long Trade
				// ===========================================================================
				if (Position.MarketPosition == MarketPosition.Long)
				{
					double highSinceOpen = High[HighestBar(High, BarsSinceEntry("MOMO Long"))];
					double tStopPrice = highSinceOpen - (TrailingStop * TickSize);

					// set breakeven if triggered										
					if (highSinceOpen >= (approxEntry + (ssAbeProfitTrigger * TickSize))) 
						stopPrice = approxEntry;
					else if (highSinceOpen >= (approxEntry + (ssAbeProfitTrigger1 * TickSize)))
						stopPrice = approxEntry - (ssAbePartialAdjustment1 * TickSize);
					else
						stopPrice = (approxEntry - (StopLoss * TickSize));
						
					stopPrice = Math.Max(tStopPrice, stopPrice);
					stopPrice = Math.Max(ATRTrailing(ATRTimes, ATRPeriod, ATRRatched).Upper[0], stopPrice);
					stopPrice = Math.Max(stopPrice, (approxEntry - (StopLoss * TickSize)));		
					
					if (BarsSinceEntry("MOMO Long") < 0) {
						Print("highSinceOpen: " + highSinceOpen);
						Print("trailingStop: " + TrailingStop + ", TickSize: " + TickSize + " *= " + (TrailingStop * TickSize));
						Print("tStopPrice: " + tStopPrice);
						Print("stopPrice: " + stopPrice);
						Print("(approxEntry - (StopLoss * TickSize)" + approxEntry + "-" + StopLoss + "*" + TickSize + "=" + (approxEntry - (StopLoss * TickSize)));
						Print(BarsSinceEntry("MOMO Long") + ": " + stopPrice); 
					}				
					ExitLongStop(stopPrice, "MOMO Long");
				}
				
				// ===========================================================================
				// Condition set - Close MOMO Short Trade
				// ===========================================================================
				if (Position.MarketPosition == MarketPosition.Short)
				{
					double lowSinceOpen = Low[LowestBar(Low, BarsSinceEntry("MOMO Short"))];
					double tStopPrice = lowSinceOpen + (TrailingStop * TickSize);

					// set breakeven if triggered										
					if (lowSinceOpen <= (approxEntry - (ssAbeProfitTrigger * TickSize))) 
						stopPrice = approxEntry;
					else if (lowSinceOpen <= (approxEntry - (ssAbeProfitTrigger1 * TickSize)))
						stopPrice = approxEntry + (ssAbePartialAdjustment1 * TickSize);
					else
						stopPrice = (approxEntry + (StopLoss * TickSize));
						
					stopPrice = Math.Min(tStopPrice, stopPrice);
					stopPrice = Math.Min(ATRTrailing(ATRTimes, ATRPeriod, ATRRatched).Lower[0], stopPrice);
					stopPrice = Math.Min(stopPrice, (approxEntry + (StopLoss * TickSize)));					

					ExitShortStop(stopPrice, "MOMO Short");
				}
				// ===========================================================================
				// ===========================================================================
			}			
			#endregion
			
        }

		#region utils
			// the following is used for short stops
			//DrawDiamond("myDot" + Low[0], true, 0, 
			//	ATRTrailing(ATRTimes, ATRPeriod, ATRRatched).Lower[0] - 2 * TickSize, Color.Yellow);
			// the following is used for long stops
			//DrawDiamond("otherDot" + Low[0], true, 0, 
			//	ATRTrailing(ATRTimes, ATRPeriod, ATRRatched).Upper[0] + 2 * TickSize, Color.Pink);

		protected Boolean isAlgoStopPos() {
			return ATRTrailing(ATRTimes, ATRPeriod, ATRRatched).Upper[0] < Low[0];	
		}

		protected Boolean isAlgoStopNeg() {
			return ATRTrailing(ATRTimes, ATRPeriod, ATRRatched).Lower[0] > High[0];	
		}

		protected Boolean isAlgoBarPos() {
			return (ATRTrailing(algoTimes, algoPeriod, algoRatched).Upper[0]) < Low[0];	
		}
		
		protected Boolean isAlgoBarNeg() {
			return (ATRTrailing(algoTimes, algoPeriod, algoRatched).Lower[0]) > High[0];	
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

		protected Boolean isLong() {
			return Position.MarketPosition == MarketPosition.Long;
		}

		protected Boolean isShort() {
			return Position.MarketPosition == MarketPosition.Short;
		}

		protected void colorBars() 
		{
            if (isAlgoBarPos())
            {
                BarColor = Color.Blue;
            }
            if (isAlgoBarNeg())
            {
                BarColor = Color.Red;
            }
		}
		#endregion		
		
        #region Properties
		[Description("Regression Channel periods")]
        [GridCategory("Parameters")]
        public int RcPeriods
        {
            get { return rcPeriods; }
            set { rcPeriods  = value; }
        }

		[Description("Regression Channel width")]
        [GridCategory("Parameters")]
        public double RcWidth
        {
            get { return rcWidth; }
            set { rcWidth  = value; }
        }

		[Description("Stop Strategy, Auto breakeven (ticks)")]
        [GridCategory("Parameters")]
        public int SsAbeProfitTrigger		
        {
            get { return ssAbeProfitTrigger; }
            set { ssAbeProfitTrigger  = Math.Max(0, value); }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public int MinFlatMeanBars		
        {
            get { return minFlatMeanBars; }
            set { minFlatMeanBars  = Math.Min(20, value); }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public int MaxRiskRTM		
        {
            get { return maxRiskRTM; }
            set { maxRiskRTM  = Math.Min(60, value); }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public bool EnableLong
        {
            get { return enableLong; }
            set { enableLong = value; }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public bool EnableShort
        {
            get { return enableShort; }
            set { enableShort = value; }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public bool EnableRtm
        {
            get { return enableRtm; }
            set { enableRtm = value; }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public bool EnableMomo
        {
            get { return enableMomo; }
            set { enableMomo = value; }
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

        [Description("")]
        [GridCategory("Parameters")]
        public int HullPeriod
        {
            get { return hullPeriod; }
            set { hullPeriod = Math.Max(1, value); }
        }

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
        public int ATRTimes
        {
            get { return aTRTimes; }
            set { aTRTimes = Math.Max(1, value); }
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
