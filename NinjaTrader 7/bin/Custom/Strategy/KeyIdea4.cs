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
	/// 	status, I wasn't able to determine the color of the vwap plot to know if it was up or donw.
    /// </summary>
    [Description("Script for @stef https://www.bigmiketrading.com/elite-circle/32758-want-your-ninjatrader-strategy-created-free-10.html#post437355")]
    public class KeyIdea4 : Strategy
    {
        #region Variables
        // Wizard generated variables
        private double dparm1 = 0.009; // Default setting for Dparm1
        private double dparm2 = 0.009; // Default setting for Dparm2
        private int iparm1 = 100; // Default setting for Iparm1
        private int iparm2 = 15; // Default setting for Iparm2
        private int period1 = 23; // Default setting for Period1
        private int period2 = 14; // Default setting for Period2
        // User defined variables (add any user defined variables below)
		private anaCurrentWeekVWAPV42 vwap;
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			//SetProfitTarget(CalculationMode.Ticks, iparm1);
			//SetStopLoss(CalculationMode.Ticks, iparm1);
			//SetTrailStop("", CalculationMode.Percent, dparm2, false);
			
			double multiplier1 = 1.0;
			double multiplier2 = 2.0;
			double multiplier3 = 3.0;
			string s_SessionOffset = "0:00:00";
			
			vwap = anaCurrentWeekVWAPV42(anaBandTypeVWAPW42.Variance_Price, multiplier1, multiplier2, multiplier3, s_SessionOffset, anaSessionCountVWAPW42.Auto, anaSessionTypeVWAPW42.RTH);
			
			Add(vwap);
			
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()			
        {
			if (isBull())
			{
				EnterLong();
			}
			if (isBear())
			{
				EnterShort();
			}
        }

		private bool isVwapUp()
		{
			bool isUp = false;
			//Print(Time + " plotcolor: " + vwap.Plots[0].Pen.Color);   // shows Color.Gold the original plot color
			Print(Time + " plotcolor: " + vwap.PlotColors[0][0]);		// shows Color[Empty]
			if (vwap.PlotColors[0][0] == vwap.UpColor)
			{
 				isUp = true;
			}
			return isUp;
		}
		
		private bool isBull() 
		{
			bool bull = false;
			if (isVwapUp())
			{
				bull = true;
			}
			return bull;
		}

		private bool isBear() 
		{
			bool bear = false;
			if (!isVwapUp())
			{
				bear = true;
			}
			return bear;
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
