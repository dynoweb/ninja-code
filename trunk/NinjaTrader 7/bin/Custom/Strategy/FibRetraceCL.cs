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
    /// Uses previous High and Low and sets a range, as it moves into the range, 
	/// it enters a position at 38.2% retracement into the range.  Staying in that 
	/// trade until a trailing 38.2% trailing stop closes the trade., Where at that 
	/// time it reverses the trade.
	/// 
	/// Based on reference:
	/// http://www.swingtradingfordummies.com/swingtradingstrategy.htm
	/// 
	///  Based on CL - 4 BetterRenko Bars
	/// 
    /// </summary>
    [Description("CL - 4 BetterRenko Bars, Uses previous High and Low and sets a range, as it moves into the range, it enters a position at 38.2% retracement into the range.  Staying in that trade until a trailing 38.2% trailing stop closes the trade., Where at that time it reverses the trade.")]
    public class FibRetraceCL : Strategy
    {
        #region Variables
			bool enableDebug = false;
		
	        int minRange = 300;
			int lookBack = 30;
			int lastTrade = 0;

			double openHigh;	// what the high was when the trade was opened
			double openLow;
			//double tradeHigh;	// the highest value the trade has gotten to since it was open
			//double tradeLow;
		
			// ZigZag Settings
			DeviationType deviationType = DeviationType.Points;
			double deviationValue = 0.42;
			bool useHighLow = true;
			int hmaPeriod = 26;

			double currentZigZagHigh = 0;
			double currentZigZagLow = 0;
			//int barNumberLow = 0;
		 	//int barNumberHigh = 0;
		    bool newHighFound = false;
		    bool newLowFound = false;
			int startingBarsAgoHigh = 0;
			int startingBarsAgoLow = 0;
	
			bool triggeredStopMove = false;
		
			int barsAgoHigh = 0;			
			int barsAgoLow = 0;			
			double swingHigh = 0;
			double swingLow = 0;
			ZigZag zz;
			ZigZagFib zzFib;
			HMA hma;
		
			int step = 0;
		#endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			BarsRequired = lookBack;
			
			Add(ZigZag(deviationType, deviationValue, useHighLow));			
			Add(ZigZagFib(deviationType, deviationValue, useHighLow));			
			Add(HMA(hmaPeriod));
			//Add(DonchianChannel(20));		
			ZigZag(deviationType, deviationValue, useHighLow).Plots[0].Pen.Color = Color.Yellow;
			ZigZagFib(deviationType, deviationValue, useHighLow).AutoScale = true;						
			ZigZag(deviationType, deviationValue, useHighLow).AutoScale = false;
			
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			//DonchianChannel dc = DonchianChannel(20);
			hma = HMA(hmaPeriod);
			zz = ZigZag(deviationType, deviationValue, useHighLow);
			zzFib = ZigZagFib(deviationType, deviationValue, useHighLow);
//			ReadZigZag();
			
			swingHigh = zzFib.Upper[0];
			swingLow = zzFib.Lower[0];
			double upperTrade = zzFib.LowerFib[0];
			double lowerTrade = zzFib.LowerFib[0];

			
			//if (enableDebug) DrawText(CurrentBar + "currentHigh", barsAgoHigh+"", 0, High[0] + 5 * TickSize, Color.White);
			//if (enableDebug) DrawText(CurrentBar +  "currentLow",  barsAgoLow+"", 0,  Low[0] - 5 * TickSize, Color.White);
			
			if (swingHigh <= 0 || swingLow <= 0)
				return;

			//DrawDot(CurrentBar + "zz", true, 0, zz.ZigZagHigh[0], Color.Purple);	

			//if (swingLow > currentZigZagLow && currentZigZagLow > 0)
			//	swingLow = currentZigZagLow;
			
			//RemoveDrawObjects();
			//Print("HighestBar(High, period): " + HighestBar(High, 30) + " swingHigh: " + swingHigh);
			//DrawHorizontalLine("SwingHigh", swingHigh, Color.White);
			//DrawHorizontalLine("SwingLow", swingLow, Color.White);
			//DrawHorizontalLine("UpperTrade", false, upperTrade, Color.Blue, DashStyle.Dash, 1);
			//DrawHorizontalLine("LowerTrade", false, lowerTrade, Color.Red, DashStyle.Dash, 1);
			
			//if (ToTime(Time[0]) > ToTime(7, 04, 0)) {
			if (Position.MarketPosition == MarketPosition.Flat) 
			{
				//if (barsAgoHigh > barsAgoLow) // && Rising(hma)) 
				if (Rising(hma)
					&& Close[0] <= zzFib.LowerFib[0]
					//&& (zzFib.Lower[20] - zzFib.Lower[0]) <= 0 * TickSize
					// may potentially need to ignore new lows due to expired old lows
					)
				{
					triggeredStopMove = false;
					GoLong(zzFib.LowerFib[0], zzFib.LowerFib[0]);
					DrawDot(CurrentBar + "tLong", true, 0, zzFib.LowerFib[0], Color.Green);	
					DrawDot(CurrentBar + "tStop", true, 0, zzFib.UpperFib[0], Color.LightGreen);	
				} 
 
					
				if (Falling(hma) && AtTop(Close[0]))
				{
					openHigh = swingHigh;
					openLow = swingLow;

					GoShort(upperTrade, upperTrade);
					
					DrawDot(CurrentBar + "tShort", true, 0, zzFib.UpperFib[0], Color.Yellow);			
					DrawDot(CurrentBar + "tStop", true, 0, zzFib.LowerFib[0], Color.LightYellow);	
				}
			}

			ManageTrade();
			
			
        }
		
		bool AtTop(double price) 
		{
			return (price > zzFib.Upper[0]);
		}
				
		private void GoLong(double limitPrice, double stopPrice) 
		{
			string signalName = "Long";
			// has been ignored since the stop price is less than or equal to the close price of the current bar
			if (stopPrice <= Close[0]) {
				stopPrice = Close[0] + 1 * TickSize;
			}
			EnterLongStopLimit(DefaultQuantity, limitPrice, stopPrice, signalName); 
			//EnterLongStop(stopPrice, signalName);
		}

		private void GoShort(double limitPrice, double stopPrice) 
		{
			string signalName = "Short";
			// has been ignored since the stop price is greater than or equal to the close price of the current bar
			if (stopPrice >= Close[0]) {
				stopPrice = Close[0] - 1 * TickSize;
			}
			EnterShortStopLimit(DefaultQuantity, limitPrice, stopPrice, signalName); 
		}
		
		private void ManageTrade() 
		{
			//double tStopHigh = 0;
			double stop = 0;
			
			if (Position.MarketPosition == MarketPosition.Long) 
			{
				double highSinceOpen = High[HighestBar(High, BarsSinceEntry())];
				
				// Contains either the opening low or the current low, which ever is greater
				// we are not going to move the opening low down
				double lower = Math.Max(zzFib.Lower[0], zzFib.Lower[BarsSinceEntry()]);
				
				// openHigh is the Last High that this trade was based on, if it moves above
				// it during the trade, we need to adjust to the new higher high
				//if (highSinceOpen > openHigh) 
				//	openHigh = highSinceOpen;								
				
				// don't trigger the stop at the upper fib line unless the price
				// has reached above the 23.6% below high
				if (High[0] > (zzFib.Upper[0] - (zzFib.Upper[0] - zzFib.Lower[0]) * 0.236))
					triggeredStopMove = true;
				
				if (triggeredStopMove)
					stop = zzFib.Upper[0] 
					- (zzFib.Upper[0] - lower) * 0.382;
				else
					stop = lower;
				
				// Trying a trailing fibinocci stop
		//		if (highSinceOpen > zzFib.UpperFib[0])
		//			stop = highSinceOpen - (highSinceOpen - openLow) * 0.382;
				
				// max amount of retrace on trade
		//		stop = Math.Max(highSinceOpen - 20*TickSize, stop);
				
				// never go below the price channel bottom
		//		stop = Math.Max(openLow, stop);
				
				// has been ignored since the stop price is greater than or equal to close price of the current bar
				if (stop >= Close[0]) 
				{
		//			stop = Close[0] - 1 * TickSize;
				}
				
				if (enableDebug) 
					Print("openHigh: " + openHigh + " openLow: " + openLow + " highSinceOpen: " + highSinceOpen
				 	 + " stop: " + stop);
				
				DrawDiamond(CurrentBar+"Stop", false, 0, stop, Color.Black);
				ExitLongStop(stop, "Long");
				ExitLongStop(stop, "Channel Switch");
				return;
			}

			if (Position.MarketPosition == MarketPosition.Short) 
			{
				double lowSinceOpen = Low[LowestBar(Low, BarsSinceEntry())];

				if (lowSinceOpen < openLow)
					openLow = lowSinceOpen;
				
				stop = lowSinceOpen + (openHigh - openLow) * 0.38;
				// has been ignored since the stop price is less than or equal to close price of the current bar
				if (stop <= Close[0]) {
					stop = Close[0] + 1 * TickSize;
				}
				this.ExitShortStop(stop, "Short");
				return;
			}
		}
		
		string Format(double price) 
		{			
			return Instrument.MasterInstrument.FormatPrice(price);
		}

        #region Properties
		
        [Description("")]
        [GridCategory("Parameters")]
        public double DeviationValue
        {
            get { return deviationValue; }
            set { deviationValue = Math.Max(0.0001, value); }
        }
		
        [Description("")]
        [GridCategory("Parameters")]
        public int HmaPeriod
        {
            get { return hmaPeriod; }
            set { hmaPeriod = Math.Max(5, value); }
        }
		
        #endregion
    }
}
