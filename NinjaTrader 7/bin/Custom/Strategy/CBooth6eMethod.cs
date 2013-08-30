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
    /// Trades 6E using a 1508 and 377 tick chart, select 6E - 377 tick for this strategy
	/// This strategy uses Charles Booth's, from BigMikes elite forum, methodology for 
	/// trading 6E using trends based on higher highs and higher lows for establishing 
	/// long positions.
	/// 
    /// </summary>
    [Description("Trades 6E using a 1508 and 377 tick chart")]
    public class CBooth6eMethod : Strategy
    {
        #region Variables
        // Wizard generated variables
        // User defined variables (add any user defined variables below)
		IchiCloud ic;
		EMA ema;
		ZigZag zz;
		Swing swing;
		bool hh = false;	// Higher Highs
		bool lh = false;	// Lower Highs
		bool hl = false;	// Higher Highs
		bool ll = false;	// Lower Highs
		bool blueCloud = false;
		
		// IchiCloud parms
		int periodFast = 9;
		int periodMedium = 26;
		int periodSlow = 52;
		
		// ZigZag parms
		double deviationValue = 0.001;
		bool useHighLow = true;
		
		// Swing parms
		int strength = 4;
		
		double swingHigh = -2;
		double swingLow = -2;
		double prevSwingHigh = -1;
		double prevSwingLow = -1;		
		bool newLowPending = true;
		bool newHighPending = true;
		double sellTrigger;
		int stopLoss = 12;
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			// Add a 377 tick Bars object to the strategy
			// Add(PeriodType.Tick, 377); - This is added from the Strategy Analyzer
			
            // Add a 1508 tick Bars object to the strategy
			Add(PeriodType.Tick, 1508);			
			
			// This only displays the Indicators's for the primary Bars object on the chart
			
			Add(IchiCloud(periodFast, periodMedium, periodSlow));
			Add(EMA(13));
			//Add(ZigZag(DeviationType.Points, deviationValue, useHighLow));
			//Add(ZigZagFib(DeviationType.Points, 0.001, true));
			
			Add(Swing(strength));

			SetProfitTarget(CalculationMode.Ticks, 22);
			SetStopLoss(CalculationMode.Ticks, stopLoss);
			
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
		    // Ignore bar update events for the supplementary Bars object added above 
		    if (BarsInProgress == 0)
        		return; 
			
			/*
			if (ToTime(Time[0]) < ToTime(3, 00, 0))
				return;
			
			if (ToTime(Time[0]) >= ToTime(8, 00, 0))
				return;
			*/
			
			ic = IchiCloud(BarsArray[1], periodFast, periodMedium, periodSlow);
			ema = EMA(BarsArray[1], 13);
			//zz = ZigZag(BarsArray[1], DeviationType.Points, deviationValue, useHighLow);
			swing = Swing(BarsArray[1], strength);
			
			//SetHigherHighsAndLowerLowers(1);
			if (Position.MarketPosition == MarketPosition.Long)
			{
				// check for sell trigger on a down bar
				if (Close[0] < Open[0])
				{
					sellTrigger = Math.Max(sellTrigger, Low[0] - 1 * TickSize);
					//DrawDot(CurrentBar + "sell", false, 0, ic[0], Color.Pink);
				} else {
					double stop = Math.Max(Position.AvgPrice - stopLoss * TickSize, sellTrigger);
					DrawDot(CurrentBar + "stop", false, 0, stop, Color.Pink);
					SetStopLoss(CalculationMode.Price, stop);
				}
				
				if (sellTrigger < (Position.AvgPrice - stopLoss * TickSize))
				{
					//SetStopLoss(CalculationMode.Price, sellTrigger);
					//DrawDot(CurrentBar + "ouch", false, 0, sellTrigger, Color.Yellow);
				}
				
			}
			
			//hh = High[swing.SwingHighBar(0, 1, 1000)] > High[swing.SwingHighBar(0, 2, 1000)];
			//ll = Low[swing.SwingLowBar(swing.SwingHighBar(0, 1, 1000), 1, 1000)]
			
			SetSwingHighAndLows();
			
			if (//!newHighPending 
				 High[swing.SwingHighBar(0, 1, 500)] > High[swing.SwingHighBar(0, 2, 500)]
				&& Low[swing.SwingLowBar(0, 1, 500)] > Low[swing.SwingLowBar(0, 2, 500)]
				//&& Low[swing.SwingLowBar(swing.SwingHighBar(0, 1, 500), 1, 500)] < Low[0]
				&& High[0] > High[swing.SwingHighBar(20, 1, 500)]
				&& (High[0] - stopLoss * TickSize < ic[0] || High[0] - stopLoss * TickSize < ema[0])
				&& Close[0] > ic[0]
				//&& hl
				)
			{
				BackColor = Color.Beige;
				//DrawDot(CurrentBar + "ic", false, 0, ic[0], Color.Pink);
				if (isValidLongTrigger())
				{
					if (Position.MarketPosition == MarketPosition.Flat)
					{
						EnterLongStop(High[0] + 1 * TickSize);
						// set to some real high value
						sellTrigger = 0;
						SetStopLoss(CalculationMode.Price, High[0] + 1 * TickSize - stopLoss * TickSize);
						DrawDot(CurrentBar + "estop", false, 0, High[0] + 1 * TickSize - stopLoss * TickSize, Color.Blue);
					}
				}
			}
		}

		private bool isValidLongTrigger()
		{
			if (Close[0] >= Open[0])
			{
				// doji
				if (Open[0] == Close[0])
					return true;
				
				// spinner
				if (Close[0] < High[0] && Open[0] > Low[0])
					return true;
					
				// hammer
				if (Close[0] == High[0])
					return true;
			}
			return false;
		}
		
		private void SetSwingHighAndLows()
		{
			if (swingHigh != swing.SwingHigh[0]) 
			{
				prevSwingHigh = swingHigh;
				swingHigh = swing.SwingHigh[0];
				newHighPending = false;
				DrawText(CurrentBar + "SH", "SH", 0, High[0] + 15 * TickSize, Color.White);
			}
			
			if (swingLow != swing.SwingLow[0]) 
			{
				prevSwingLow = swingLow;
				swingLow = swing.SwingLow[0];
				newLowPending = false;
				DrawText(CurrentBar + "SL", "SL", 0, Low[0] - 15 * TickSize, Color.White);
			}
			
			if (High[0] > swingHigh)
				newHighPending = true;
			
			if (Low[0] < swingLow)
				newLowPending = true;
			
			//if (newHighPending)
			//	DrawText(CurrentBar + "SHB", swing.SwingHighBar(0,1,50)+"", 0, High[0] + 8 * TickSize, Color.Black);
			
			//if (newLowPending)
			//	DrawText(CurrentBar + "SLB", swing.SwingLowBar(0,1,50)+"", 0, Low[0] - 8 * TickSize, Color.Black);
		}
		
		
		
		
		/// <summary>
		/// This should work from the 1508 tick chart
		/// </summary>
		private void SetHigherHighsAndLowerLowers(int barArrayIndex)
		{
			// The occurrence to check for (1 is the most recent, 2 is the 2nd most recent etc...)
			int instance = 1;
			int lookBackPeriod = 100;
			int barsAgoHigh = 0;			
			int barsAgoLow = 0;			
			barsAgoHigh = zz.HighBar(barsAgoHigh, instance, lookBackPeriod);
			barsAgoLow = zz.LowBar(barsAgoLow, instance, lookBackPeriod);
			
			instance = 2;
			int previousBarsAgoHigh = zz.HighBar(0, instance, lookBackPeriod);
			int previousBarsAgoLow = zz.LowBar(0, instance, lookBackPeriod);
			
			//double zzHigh = Instrument.MasterInstrument.Round2TickSize(zz.ZigZagHigh[barsAgoHigh]);
			//DrawText(CurrentBar + "xx", barsAgoHigh+","+previousBarsAgoHigh, 0, High[0] + 7 * TickSize, Color.Black);
			//DrawText(CurrentBar + "yy", barsAgoLow+","+previousBarsAgoLow, 0, Low[0] - 7 * TickSize, Color.Black);
			
			if (barsAgoHigh == 1 && High[0] <= High[barsAgoHigh])
			{
				if (High[barsAgoHigh] > High[previousBarsAgoHigh])
				{
					DrawText(CurrentBar + "HH", "HH", barsAgoHigh, High[barsAgoHigh] + 5 * TickSize, Color.Black);
					hh = true;
					lh = false;
				}
				else if (High[barsAgoHigh] < High[previousBarsAgoHigh])
				{
					DrawText(CurrentBar + "LH", "LH", barsAgoHigh, High[barsAgoHigh] + 5 * TickSize, Color.Black);
					hh = false;
					lh = true;
				}
				else
					DrawText(CurrentBar + "DH", "DH", barsAgoHigh, High[barsAgoHigh] + 5 * TickSize, Color.Black);
			}
				
			if (barsAgoLow == 1 && Low[0] >= Low[barsAgoLow])
			{
				if (Low[barsAgoLow] > Low[previousBarsAgoLow])
				{
					DrawText(CurrentBar + "HL", "HL", barsAgoLow, Low[barsAgoLow] - 5 * TickSize, Color.Black);
					hl = true;
					ll = false;
					if (hh)
						BackColorAll = Color.Green;
				}
				else if (Low[barsAgoLow] < Low[previousBarsAgoLow])
				{
					DrawText(CurrentBar + "LL", "LL", barsAgoLow, Low[barsAgoLow] - 5 * TickSize, Color.Black);
					hl = false;
					ll = true;
				}
				else
					DrawText(CurrentBar + "DL", "DL", barsAgoLow, Low[barsAgoLow] - 5 * TickSize, Color.Black);
			}
			
			// SpanB is the base, where SpanA is the more active value.
			if (ic.SenkouSpanA[0] > ic.SenkouSpanB[0])
				blueCloud = true;
			
			if (hh && hl && (Low[0] > ic.SenkouSpanA[0]))
				//DrawDot(CurrentBar + "DL", false, 0, Low[barsAgoLow] - 2 * TickSize, Color.Green);
				BackColorAll = Color.LightGreen;
			
		}			

        #region Properties
        #endregion
    }
}
