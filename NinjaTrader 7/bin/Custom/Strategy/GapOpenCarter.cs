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
    /// /ES and /YM Gap open trades on 15 min chart looking for mean reversion to fill
    /// </summary>
    [Description("/ES and /YM 15 min Gap Open Trades")]
    public class GapOpenCarter : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int minGap = 1; // Default setting for MinGap
        private int midGap = 40; // Default setting for MidGap
        private int maxGap = 200; // Default setting for MaxGap
        // User defined variables (add any user defined variables below)
		double priorDayClose = 0;
		double targetPrice = 0;
		double openingGap = 0;
		double startPrice = 0;
		double pitHigh = 0;
		double pitLow = 0;
		int barsInSessionStart = 0;
		int barsInSessionEnd = 0;

        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			Add(PitColor(Color.Black, 83000, 40, 151501));
			

            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (Bars.BarsSinceSession == 1)
			{
//				if (priorDayClose == 0.0) {
//					priorDayClose = Close[1];
//				}
			}
    		// If it's Monday, do not trade.
//    		if (Time[0].DayOfWeek == DayOfWeek.Monday)
//        		return;			
			
			// open time, typical end of pit session is the 61st bar
			int hour = 8;
			int minute = 30;
			if (ToTime(Time[0]) == ToTime(hour, minute, 0)) {
				startPrice = Close[0];
				openingGap = (startPrice - priorDayClose);
				barsInSessionStart = Bars.BarsSinceSession;

				DrawDot(CurrentBar + "close", false, 0, Close[0], Color.Blue);
				DrawText(CurrentBar + "txt", barsInSessionStart.ToString(), 0, High[0] + 2 * TickSize, Color.Black); 
				DrawText(CurrentBar + "opnGp", " " + openingGap, 0, Low[0] - 5 * TickSize, Color.Black); 
				
				if (priorDayClose != 0) {
						double target = Math.Abs(openingGap) <= 100 ? Close[0] - openingGap : Close[0] - openingGap * 0.5;
						double stop = Math.Abs(openingGap) <= 100 ? Close[0] + (openingGap * 2) : Close[0] + (openingGap * 2 * 0.5);
						
						DrawDot(CurrentBar + "targ", false, 0, target, Color.Green);
						DrawDot(CurrentBar + "stop", false, 0, stop, Color.Red);
						
//						SetProfitTarget("", CalculationMode.Price, target);
//						SetStopLoss("", CalculationMode.Price, stop, false);

					if (openingGap < 0) {
						EnterShort();
						//EnterLong();
					}
					if (openingGap > 0) {
						//EnterShort();
						EnterLong();
					}
				}
			}
			
			// close time, typical end of pit session is the 88th bar
			hour = 15;
			minute = 15;
			if ((ToTime(Time[0]) == ToTime(hour, minute, 0)) && Bars.BarsSinceSession == 88) {
				if (Position.MarketPosition == MarketPosition.Long) {
					//ExitLong();
				}
				
				barsInSessionEnd = Bars.BarsSinceSession;
				pitLow = MIN(Low, 27)[0];
				pitHigh = MAX(High, 27)[0];
				
//				if  (priorDayClose != 0) {
//					Print(Time + ", " + openingGap + ", " + startPrice + ", " + targetPrice + ", " + 
//								pitLow + ", " + pitHigh + ", " + 
//								barsInSessionStart + ", " + barsInSessionEnd);
//				}
					
				DrawDot(CurrentBar + "close", false, 0, Close[0], Color.Blue);
				DrawLine(CurrentBar + "closeHL", 0, Close[0], -75, Close[0], Color.Blue);
				DrawText(CurrentBar + "barsIn", barsInSessionEnd.ToString(), 0, High[0] + 2 * TickSize, Color.Black); 				

				if (openingGap > 0) {	// looking for a mean reversion down
					if (pitLow < targetPrice) {
						DrawText(CurrentBar + "result", "Fill", 0, High[0] + 20 * TickSize, Color.Green);
					} else {
						DrawText(CurrentBar + "result", "noFill", 0, High[0] + 20 * TickSize, Color.Red);
					}
				} else {
					if (pitHigh > targetPrice) {
						DrawText(CurrentBar + "result", "Fill", 0, High[0] + 20 * TickSize, Color.Green); 				
					} else {
						DrawText(CurrentBar + "result", "noFill", 0, High[0] + 20 * TickSize, Color.Red);
					}
				}
				priorDayClose = Close[0];
				targetPrice = Close[0];
			}
        }

        #region Properties
        [Description("Gap in ticks")]
        [GridCategory("Parameters")]
        public int MinGap
        {
            get { return minGap; }
            set { minGap = Math.Max(0, value); }
        }

        [Description("Gap taking only partial reversal")]
        [GridCategory("Parameters")]
        public int MidGap
        {
            get { return midGap; }
            set { midGap = Math.Max(1, value); }
        }

        [Description("Gaps bigger are not taken")]
        [GridCategory("Parameters")]
        public int MaxGap
        {
            get { return maxGap; }
            set { maxGap = Math.Max(1, value); }
        }
        #endregion
    }
}
