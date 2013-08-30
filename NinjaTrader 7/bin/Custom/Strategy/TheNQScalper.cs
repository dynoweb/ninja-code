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
    /// Trades NQ using the Renko Bars and MACD
	/// Tight stops cause problems since ticks exceed Renko bars with Live Data
	/// 
	/// Status update, I've gotten frustrated with this since it doesn't seem to product good results.
	/// 
	/// Renko 4 - 1/1->8/31/12  0.01, 3,7,22,9,9 $114,100.0  6,283 Trades,   -$490.0 Max DD -- Wider Stops
	/// 
	/// Now trying with Range 4
	/// Range 4 - 1/1->8/31/12  x, x, x, 0.005, 9,16,9,21,17 $5,235.0  3,575 Trades, -$4,955.0 Max DD -- Wider Stops
    /// </summary>
    [Description("Trades NQ - R4")]
    public class TheNQScalper : Strategy
    {
        #region Variables
        // Wizard generated variables
			// settings are current optimized for Renko 4
	        private int aTRPeriod = 10; // 10 Default setting for ATRPeriod Optimized MOMO = 14		
			private double aTRRatched = 0.008; // 0.000 Default setting for ATRRatched
			private double aTRTimes = 5.5; // 4  Default setting for ATRTimes Optimized MOMO = 3
			private bool enableDebug = false;
			private bool enableLong = true;
			private bool enableShort = true;
	        private int hullPeriod = 8; // Default setting for HullPeriod Optimized MOMO = Didn't really matter
			private double mACDDiff = 0.005;
			private int mACDFast = 9; // Default setting for MACDFast
			private int mACDSlow = 16; // Default setting for MACDSlow
			private int mACDSmooth = 9; // Default setting for ATRPeriod

			private int profitTargetLong = 9; // <-- not used
			private int profitTargetShort = 26; // <-- not used

			private int setupBars = 5;
			
			private int stopLossLong = 2; 
			private int stopLossShort = 14;		
		
			private int trailingStopLong = 16;
			private int trailingStopShort = 8;
		
        // User defined variables (add any user defined variables below)
			private int upBars = 0;
			private int downBars = 0;
			private bool isCurrentUpTrend = true;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            //Add(MACDrick(mACDFast, mACDSlow, mACDSmooth));
            Add(ATRTrailing(ATRTimes, ATRPeriod, ATRRatched));
			//Add(HMARick(HullPeriod));
			
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			//DrawDiamond(CurrentBar.ToString()+"down", true, 0, HMA(hullPeriod)[0] - TickSize*2, Color.Black);
			
			if (isAlgoBarPos()) {
				if (!isCurrentUpTrend) {
					upBars = 1;		
					isCurrentUpTrend = true;
					//DrawArrowUp(CurrentBar.ToString()+"up", 0, HMA(hullPeriod)[0] - TickSize*2, Color.Blue);
				} else {
					upBars += 1;		
				}
			}
			
			if (isAlgoBarNeg()) {
				if (isCurrentUpTrend) {
					downBars = 1;		
					isCurrentUpTrend = false;
					//DrawArrowDown(CurrentBar.ToString()+"down", 0, HMA(hullPeriod)[0] + TickSize*2, Color.Red);
				} else {
					downBars += 1;		
				}
			}
			
			if (Position.MarketPosition == MarketPosition.Long) {
				isCurrentUpTrend = true;
				upBars = 0;
				downBars = 0;
			}
			if (Position.MarketPosition == MarketPosition.Short) {
				isCurrentUpTrend = false;
				upBars = 0;
				downBars = 0;
			}
			
			// Open Secret Sauce Long
			if (isFlat()
				&& enableLong
				//&& isAlgoStopPos()
				&& isAlgoBarPos()
				&& downBars > setupBars
				//&& (MACDrick(MACDFast, MACDSlow, MACDSmooth).Diff[0] > MACDDiff)
				//&& (MACDrick(MACDFast, MACDSlow, MACDSmooth).Diff[1] < 0)
				)
			{
				//SetTrailStop(CalculationMode.Ticks, TrailingStopLong);
				EnterLong(DefaultQuantity);
			}
			
			// Open Secret Sauce Short
			if (isFlat()
				&& enableShort
				&& isAlgoStopNeg()
				&& isAlgoBarNeg()
				&& upBars > setupBars
				//&& (MACDrick(MACDFast, MACDSlow, MACDSmooth).Diff[0] < 0)
				//&& (MACDrick(MACDFast, MACDSlow, MACDSmooth).Diff[1] > 0)
				)
			{
				//SetTrailStop(CalculationMode.Ticks, TrailingStopShort);
				EnterShort(DefaultQuantity);
			}
			closeLong();
			closeShort();
        }

		protected void closeLong() {
			
			if (Position.MarketPosition != MarketPosition.Long) 
				return;
			
			double entryPrice = Instrument.MasterInstrument.Round2TickSize(Position.AvgPrice);
			ExitLongStop(entryPrice - StopLossLong * TickSize);
			//ExitLongLimit(entryPrice + StopLossLong * TickSize);			
			ExitLongLimit(entryPrice + ProfitTargetLong * TickSize);			
			//DrawDiamond(CurrentBar.ToString()+"logStop", true, 0, entryPrice - StopLossLong * TickSize, Color.Yellow);
		}

		protected void closeShort() {
			
			if (Position.MarketPosition != MarketPosition.Short) 
				return;
			
			double entryPrice = Instrument.MasterInstrument.Round2TickSize(Position.AvgPrice);
			ExitShortStop(entryPrice + StopLossShort * TickSize);
			//ExitLongLimit(entryPrice + StopLossLong * TickSize);			
			ExitShortLimit(entryPrice - ProfitTargetShort * TickSize);			
			//DrawDiamond(CurrentBar.ToString()+"logStop", true, 0, entryPrice - StopLossLong * TickSize, Color.Yellow);
		}

		
		#region utils
		
			protected Boolean isAlgoBarPos() {
				return Rising(HMARick(HullPeriod, 30));	
			}
			
			protected Boolean isAlgoBarNeg() {
				return Falling(HMARick(HullPeriod, 30));	
			}
			
			protected Boolean isAlgoStopPos() {
				return ATRTrailing(ATRTimes, ATRPeriod, ATRRatched).Upper[0] < Low[0];	
			}

			protected Boolean isAlgoStopNeg() {
				return ATRTrailing(ATRTimes, ATRPeriod, ATRRatched).Lower[0] > High[0];	
			}

			protected Boolean isFlat() {
				return Position.MarketPosition == MarketPosition.Flat;
			}

		#endregion
		
		
		
        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public double ATRTimes
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

        [Description("")]
        [GridCategory("Parameters")]
        public int HullPeriod
        {
            get { return hullPeriod; }
            set { hullPeriod = Math.Max(1, value); }
        }

        [Description("MACD min difference to consider above 0")]
        [GridCategory("Parameters")]
        public double MACDDiff
        {
            get { return mACDDiff; }
            set { mACDDiff = Math.Max(-0.05, value); }
        }

		[Description("MACD Fast Setting")]
        [GridCategory("Parameters")]
        public int MACDFast
        {
            get { return mACDFast; }
            set { mACDFast = Math.Max(1, value); }
        }

        [Description("MACD Slow Setting")]
        [GridCategory("Parameters")]
        public int MACDSlow
        {
            get { return mACDSlow; }
            set { mACDSlow = Math.Max(1, value); }
        }

        [Description("MACD Smoothing Factor")]
        [GridCategory("Parameters")]
        public int MACDSmooth
        {
            get { return mACDSmooth; }
            set { mACDSmooth = Math.Max(1, value); }
        }

		[Description("Number of bars on previous opposite short term trend")]
        [GridCategory("Parameters")]
        public int SetupBars
        {
            get { return setupBars; }
            set { setupBars = Math.Max(1, value); }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public int ProfitTargetLong
        {
            get { return profitTargetLong; }
            set { profitTargetLong = Math.Max(1, value); }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public int ProfitTargetShort
        {
            get { return profitTargetShort; }
            set { profitTargetShort = Math.Max(1, value); }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public int StopLossLong
        {
            get { return stopLossLong; }
            set { stopLossLong = Math.Max(1, value); }
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
        public int TrailingStopLong
        {
            get { return trailingStopLong; }
            set { trailingStopLong = Math.Max(1, value); }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public int TrailingStopShort
        {
            get { return trailingStopShort; }
            set { trailingStopShort = Math.Max(1, value); }
        }

        #endregion

    }
}
