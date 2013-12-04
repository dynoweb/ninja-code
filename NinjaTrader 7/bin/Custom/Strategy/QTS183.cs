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
    /// Enter the description of your strategy here
    /// </summary>
    [Description("Enter the description of your strategy here")]
    public class QTS183 : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int investment = 10000; // Default setting for Investment
		private double num = 0.5;
        private int period = 20; // Default setting for ChannelPeriod
        private int periodShort = 14; // Default setting for MAShort
        private int periodMedium = 40; // Default setting for MAMedium
        private int periodLong = 80; // Default setting for MALong
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			Aggregated = true;
			CalculateOnBarClose = true;
			//ClearOutputWindow();
			EntriesPerDirection = 1; // Order Handling enforce only 1 position long or short at any time
			EntryHandling = EntryHandling.AllEntries;
			ExitOnClose = false;
			
			Add(HMARick(PeriodLong, 10));
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// Checks to make sure we have at least x or more bars
			if (CurrentBar < PeriodLong)
        		return;	
			
			
			double std = StdDev(Close, PeriodLong)[0];
			double sqrt = Math.Sqrt(PeriodShort);
			double threshold = Num * sqrt * std;
			Print(Time + " std: " + std + "  sqrt: " + sqrt + "  threshold: " + threshold + "  Close[0] - Close[PeriodShort]: " + (Close[0] - Close[PeriodShort]));
						
			if (Close[0] - HMA(PeriodLong)[PeriodShort] > threshold)
			{
				EnterLong(calcShares(Investment));
			}
				
			if (Close[0] - HMA(PeriodLong)[PeriodShort] < -threshold)
			{
				EnterShort(calcShares(Investment));
			}
        }

		private int calcShares(double investment)
		{
			return (int) Math.Floor(investment/Close[0]);
		}
		

        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int PeriodShort
        {
            get { return periodShort; }
            set { periodShort = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int PeriodMedium
        {
            get { return periodMedium; }
            set { periodMedium = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int PeriodLong
        {
            get { return periodLong; }
            set { periodLong = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Investment
        {
            get { return investment; }
            set { investment = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double Num
        {
            get { return num; }
            set { num = Math.Max(0, value); }
        }
        #endregion
    }
}
