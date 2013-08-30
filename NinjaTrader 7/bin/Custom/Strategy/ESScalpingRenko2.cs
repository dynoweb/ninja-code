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
    /// ES 2 Renko to identify entry points using jwdixon's ES scalping technique
	/// 
	/// Jan 1st - Sep 26 2012 - $4,680.00 profit - longs only
	/// 
    /// </summary>
    [Description("ES 2 Renko to identify entry points using jwdixon's ES scalping technique")]
    public class ESScalpingRenko2 : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int donChanPeriod1 = 10; // Default setting for DonChanPeriod1
        private int donChanPeriod2 = 21; // Default setting for DonChanPeriod2
        private int minBarsInPriorTrend = 5; // Default setting for MinBarsInPriorTrend
        private int maxPullBack = 1; // Default setting for MaxPullBack
        // User defined variables (add any user defined variables below)
		private int status = 0;
		private int barsInPriorTrendCount = 0;
		private int barsInTrend = 0;
		
		private DonchianChannel dcFast;
		private DonchianChannel dcSlow;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			Add(DonchianChannel(DonChanPeriod1));
			Add(DonchianChannel(DonChanPeriod2));
			
			DonchianChannel(DonChanPeriod1).Plots[0].Pen.Color = Color.Brown;
			DonchianChannel(DonChanPeriod1).Plots[1].Pen.Color = Color.Transparent;
			DonchianChannel(DonChanPeriod1).Plots[2].Pen.Color = Color.Transparent;
			DonchianChannel(DonChanPeriod1).Plots[0].Pen.Width = 2;
			
			DonchianChannel(DonChanPeriod2).Plots[0].Pen.Color = Color.Yellow;
			DonchianChannel(DonChanPeriod2).Plots[1].Pen.Color = Color.Transparent;
			DonchianChannel(DonChanPeriod2).Plots[2].Pen.Color = Color.Transparent;
			
			//SetStopLoss(CalculationMode.Ticks, 4);
			//SetProfitTarget(CalculationMode.Ticks, 4);
			SetTrailStop(CalculationMode.Ticks, 5);
			
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			dcFast = DonchianChannel(DonChanPeriod1);
			dcSlow = DonchianChannel(DonChanPeriod2);
			
			if (Position.MarketPosition != MarketPosition.Flat) {
				barsInTrend = 0;
				return;
			}

			switch (status) {
				case 0:
					if (barsInTrend != 0) 
					{
						barsInPriorTrendCount = barsInTrend;
						barsInTrend = 1;
					}
					status = GetBarLocation(0);
					break;
				case 1:
					if (GetBarLocation(0) != 1) {
						status = 0;
						break;
					}					
					barsInTrend++;
					
					if (barsInTrend > 2) // start to look for a pullback to get in
					{
						if (!UpBar(0)) 
						{
							DrawDot(CurrentBar + "L", false, 0, High[0] + 3 * TickSize, Color.Blue);
							//DrawDot(CurrentBar + "!=", false, 0, High[0] + 3 * TickSize, Color.Black);						
							double limit = High[0] + 1 * TickSize;
							//DrawText(CurrentBar + "LT", limit + "", 0, Low[0] - 8 * TickSize, Color.Black);
							EnterLongStopLimit(limit, limit);
						}
					} 					
					break;
				case -1:
					status = 0;
					break;
				default:
					
					break;
			}
        }
		
		private bool UpBar(int idx)
		{
			return Close[idx + 1] < Close[idx];
		}
		
		/// <summary>
		/// Returns an integer representing the location of the specfied bar.
		/// </summary>
		/// <param name="idx"></param>
		/// <returns>
		/// -1 - below both DC means
		///  0 - the bar is touching one of the means
		///  1 - the bar is above the mean
		/// </returns>
		private int GetBarLocation(int idx)
		{
			if (//Low[idx] > dcFast.Mean[idx] && 
				Low[idx] > dcSlow.Mean[idx])
				return 1;
			if (//High[idx] < dcFast.Mean[idx] && 
				High[idx] < dcSlow.Mean[idx])
				return -1;
			return 0;				
		}

        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int DonChanPeriod1
        {
            get { return donChanPeriod1; }
            set { donChanPeriod1 = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int DonChanPeriod2
        {
            get { return donChanPeriod2; }
            set { donChanPeriod2 = Math.Max(1, value); }
        }

        [Description("How long in previous trend before considering it a trend change")]
        [GridCategory("Parameters")]
        public int MinBarsInPriorTrend
        {
            get { return minBarsInPriorTrend; }
            set { minBarsInPriorTrend = Math.Max(0, value); }
        }

        [Description("Opp")]
        [GridCategory("Parameters")]
        public int MaxPullBack
        {
            get { return maxPullBack; }
            set { maxPullBack = Math.Max(1, value); }
        }
        #endregion
    }
}
