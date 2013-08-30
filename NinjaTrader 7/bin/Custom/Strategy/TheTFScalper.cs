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
	/// Note: I found out that the strategy tester showed significally different results 
	/// from the live trades.  This had to do with the fact that the tick range is wider live
	/// 
    /// Trades TF using the Renko Bars and MACD
	/// Renko 2 - 1/1->9/9/12  ??, 3,11, 4,3,3 $369,255.0 43,723 Trades, -$1,230.0 Max DD 
	/// Renko 3 - 1/1->9/9/12  ??, 4,10, 7,4,4 $300,310.0 14,852 Trades,   -$750.0 Max DD
	/// Renko 4 - 1/1->9/9/12  ??, 7,15, 3,5,5 $259,875.0  8,275 Trades,   -$535.0 Max DD
	/// 
	/// Renko 4 - 1/1->9/9/12  0.04, 5,15, 3,9,9 $175,955.0  5,705 Trades,  -$1,005.0 Max DD - wider tick stops
    /// </summary>
    [Description("Trades TF - R4")]
    public class TheTFScalper : Strategy
    {
        #region Variables
        // Wizard generated variables
			// settings are current optimized for Renko 4
			private bool enableDebug = false;
			private bool enableLong = true;
			private bool enableShort = true;
			private double mACDDiff = 0.04;
			private int mACDFast = 5; // Default setting for MACDFast
			private int mACDSlow = 15; // Default setting for MACDSlow
			private int mACDSmooth = 3; // Default setting for ATRPeriod
			private int trailingStopLong = 9;
			private int trailingStopShort = 9;
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
				&& (MACDrick(MACDFast, MACDSlow, MACDSmooth).Diff[0] < MACDDiff)
				&& (MACDrick(MACDFast, MACDSlow, MACDSmooth).Diff[1] > 0)
				)
			{
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
