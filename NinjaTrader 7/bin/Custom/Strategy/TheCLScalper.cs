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
	/// CL Renko 3 Secret Sauce Strategy
	/// macdFast, Slow, Smooth, TrailingStopLong, TrailingStopShort
	/// Renko 3 - 1/1->9/9/12  0.0, 18,45,18,10,10 $144,185.0 9,201 Trades,   -$1,615.0 Max DD
    /// </summary>
    [Description("CL Renko 3 Secret Sauce Strategy")]
    public class TheCLScalper : Strategy
    {
        #region Variables
        // Wizard generated variables
	        private int donchianPeriod = 55;
			private bool enableDebug = false;
			private bool enableLong = true;
			private bool enableShort = true;
	        private int hullPeriod = 87; // Default setting for HullPeriod  // <-- not used
			private double mACDDiff = 0.0;
			private int macdDeltaBarsBack = 1; // <-- not used
			private int macdDeltaBarsInto = 4; // <-- not used
			private int mACDFast = 18; // Default setting for MACDFast
			private int mACDSlow = 45; // Default setting for MACDSlow
			private int mACDSmooth = 18; // Default setting for ATRPeriod
			private int profitTargetLong = 20; // <-- not used
			private int profitTargetShort = 20; // <-- not used
			private int stopLossLong = 10; // <-- not used
			private int stopLossShort = 10; // <-- not used
			private int trailingStopLong = 10;
			private int trailingStopShort = 10;
        // User defined variables (add any user defined variables below)
			private bool macdWasPos = false;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            Add(MACDrick(mACDFast, mACDSlow, mACDSmooth));
            //Add(HMA(HullPeriod));
			//Add(HMARick(25));
			// Add stop loss and profit target orders, 
			//SetStopLoss(CalculationMode.Ticks, StopLossLong);
			//SetProfitTarget(CalculationMode.Ticks, ProfitTargetLong);
            //SetTrailStop(CalculationMode.Ticks, TrailingStopLong);
			
			CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			colorBars();
			
			//if ((MACDrick(mACDFast, mACDSlow, mACDSmooth).Diff[0] 
			//		- MACDrick(mACDFast, mACDSlow, mACDSmooth).Diff[macdDeltaBarsBack] > macdDelta))
			//	DrawDiamond(CurrentBar.ToString()+"mac", true, 0, Close[0], Color.Yellow);

			
			// Open Secret Sauce Long
			if (isFlat()
				&& enableLong
				&& (MACDrick(MACDFast, MACDSlow, MACDSmooth).Diff[0] > MACDDiff)
				&& (MACDrick(MACDFast, MACDSlow, MACDSmooth).Diff[1] < 0)
				)
			{
				SetTrailStop(CalculationMode.Ticks, TrailingStopLong);
				EnterLong(DefaultQuantity);
			}
			
			// Open Secret Sauce Short
			if (isFlat()
				&& enableShort
				&& (MACDrick(MACDFast, MACDSlow, MACDSmooth).Diff[1] - MACDrick(MACDFast, MACDSlow, MACDSmooth).Diff[0] > MACDDiff)
				&& (MACDrick(MACDFast, MACDSlow, MACDSmooth).Diff[0] < 0)
				)
			{
				SetTrailStop(CalculationMode.Ticks, TrailingStopShort);
				EnterShort(DefaultQuantity);
			}
        }

		#region utils
		protected Boolean isAlgoBarPos() {
			return Rising(HMA(HullPeriod));	
		}
		
		protected Boolean isAlgoBarNeg() {
			return Falling(HMA(HullPeriod));	
		}
		
		protected Boolean isMacdPos() {
			return MACDrick(mACDFast, mACDSlow, mACDSmooth).Diff[0] > 0;	
		}
		
		protected Boolean isMacdNeg() {
			return MACDrick(mACDFast, mACDSlow, mACDSmooth).Diff[0] < 0;	
		}
		
		protected Boolean isFlat() {
			return Position.MarketPosition == MarketPosition.Flat;
		}
		protected void colorBars() 
		{
            if (Rising(HMA(HullPeriod)) == true)
            {
                BarColor = Color.Blue;
            }
            if (Falling(HMA(HullPeriod)) == true)
            {
                BarColor = Color.Red;
            }
		}
		#endregion
		
		
		
        #region Properties
        [Description("Donchian Period")]
        [GridCategory("Parameters")]
        public int DonchianPeriod
        {
            get { return donchianPeriod; }
            set { donchianPeriod = Math.Max(1, value); }
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

        [Description("MACD min difference to consider above 0")]
        [GridCategory("Parameters")]
        public double MACDDiff
        {
            get { return mACDDiff; }
            set { mACDDiff = Math.Max(-0.05, value); }
        }

        [Description("Hull Moving Average Period")]
        [GridCategory("Parameters")]
        public int HullPeriod
        {
            get { return hullPeriod; }
            set { hullPeriod = Math.Max(1, value); }
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

        [Description("MACD Slow Setting")]
        [GridCategory("Parameters")]
        public int MacdDeltaBarsBack
        {
            get { return macdDeltaBarsBack; }
            set { macdDeltaBarsBack = Math.Max(1, value); }
        }

        [Description("MACD bars into change and still take the trade, 0 means ignore this")]
        [GridCategory("Parameters")]
        public int MacdDeltaBarsInto
        {
            get { return macdDeltaBarsInto; }
            set { macdDeltaBarsInto = Math.Max(0, value); }
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
