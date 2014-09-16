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
	/// Traded CL on 10-20 range bars - performs pretty well
    /// </summary>
    [Description("CL using Range Bar")]
    public class Dynoweb3 : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int iparm1 = 1; // Default setting for Iparm1
        private int iparm2 = 1; // Default setting for Iparm2
        private double dparm1 = 0.014; // Default setting for Dparm1
        private double dparm2 = 0.023; // Default setting for Dparm2
        private int period1 = 30; // Default setting for Period1
        private int period2 = 27; // Default setting for Period2
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            SetProfitTarget("", CalculationMode.Percent, dparm1);
            //SetStopLoss("", CalculationMode.Percent, dparm1, false);
            SetTrailStop("", CalculationMode.Percent, dparm2, false);

            CalculateOnBarClose = true;
			ExitOnClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()			
        {

			if (Position.MarketPosition == MarketPosition.Flat) 
			{
				if (HighestBar(High, period1) == 0 && Close[0] > Close[1])
				{
					EnterLong();
				}
				if (LowestBar(Low, period2) == 0 && Close[0] < Close[1])
				{
					EnterShort();
				}
			} 

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
