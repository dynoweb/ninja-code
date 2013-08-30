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
    /// Trades 6E using the Renko 3 Bars and MACD
	/// Renko 3 - 1/1->9/9/12  17,65,17, 4, 4,  $58,097.0 2,798 Trades,   -$415.0 Max DD
	/// Renko 3 - 1/1->9/9/12  21,65, 8, 7, 7,  $43,407.0 1,874 Trades, -$1,127.5 Max DD
	/// Renko 3 - 1/1->9/9/12   3, 7,22, 7, 7, $184,920.0 9,106 Trades, -$1,535.0 Max DD
	/// Renko 3 - 1/1->9/9/12   3, 7,22,10,10, $105,897.5 5,728 Trades, -$3,422.5 Max DD 
	/// 
	/// Note: Had lots of problems with the Renko bars, the Strategy Analyzer didn't match the Market or Martket Reply.  Close and open
	/// was happening on the same bar and prices were ticking way out of the Renko bars.
	/// 
	/// Range Bar approach:
	/// Trades 6E using the Range 4 Bars and MACD
	/// 
	/// 
    /// </summary>
    [Description("Trades 6E")]
    public class The6EScalper : Strategy
    {
        #region Variables
        // Wizard generated variables
			private bool enableDebug = false;
			private bool enableLong = true;
			private bool enableShort = true;
			private double mACDDiff = 0.0001;  // not used
		/* Not sure what range this is tuned to
		// (9,22,14)(6,32,15), (7,40,18)
			private int mACDFast = 9; // Default setting for MACDFast
			private int mACDSlow = 22; // Default setting for MACDSlow
			private int mACDSmooth = 14; // Default setting for ATRPeriod
			private int profitTargetLong = 39;
			private int profitTargetShort = 40;
			private int stopLossLong = 11;
			private int stopLossShort = 12;
			private int trailingStopLong = 13;
			private int trailingStopShort = 25;
		*/
		// These settings were used with a 10 point range with marginal results.
			private int mACDFast = 9; // Default setting for MACDFast
			private int mACDSlow = 34; // Default setting for MACDSlow
			private int mACDSmooth = 22; // Default setting for ATRPeriod
			private int profitTargetLong = 30;
			private int profitTargetShort = 47;
			private int stopLossLong = 20;
			private int stopLossShort = 30;
			private int trailingStopLong = 13;
			private int trailingStopShort = 25;
		
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            Add(MACDrick(mACDFast, mACDSlow, mACDSmooth));
			CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// Open Secret Sauce Long
			if (isFlat()
				&& enableLong
				&& (MACDrick(MACDFast, MACDSlow, MACDSmooth).Diff[0] > 0)
				&& ((MACDrick(MACDFast, MACDSlow, MACDSmooth).Diff[1] < 0)
					|| (MACDrick(MACDFast, MACDSlow, MACDSmooth).Diff[2] < 0))
				)
			{
				//SetProfitTarget(CalculationMode.Ticks, ProfitTargetLong);
				//SetStopLoss(CalculationMode.Ticks, StopLossLong);
				SetTrailStop(CalculationMode.Ticks, TrailingStopLong);
				EnterLong(DefaultQuantity);
				//EnterLongLimit(DefaultQuantity, Close[0] - TickSize * 10);
			}
			
			// Open Secret Sauce Short
			if (isFlat()
				&& enableShort
				&& (MACDrick(MACDFast, MACDSlow, MACDSmooth).Diff[0] < 0)
				&& ((MACDrick(MACDFast, MACDSlow, MACDSmooth).Diff[1] > 0)
					|| (MACDrick(MACDFast, MACDSlow, MACDSmooth).Diff[2] > 0))
				)
			{
				//SetProfitTarget(CalculationMode.Ticks, ProfitTargetShort);
				//SetStopLoss(CalculationMode.Ticks, StopLossShort);
				SetTrailStop(CalculationMode.Ticks, TrailingStopShort);
				EnterShort(DefaultQuantity);
			}
        }

		#region utils
		
			protected Boolean isFlat() {
				return Position.MarketPosition == MarketPosition.Flat;
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
