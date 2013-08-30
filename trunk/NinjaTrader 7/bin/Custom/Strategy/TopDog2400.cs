// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

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
using NinjaTrader.Strategy;
#endregion


// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Multi-time frame strategy.
	/// 
	/// wav files from http://www2.research.att.com/~ttsweb/tts/demo.php
	/// 
	/// Medium Term : Short Term charts 3:1
	/// 
	/// 6E -   284k - 250   620k/w   266:800
	/// CL -   200k - 250   600k/w   266:800
	/// ES - 1,570k - 1500 7500k/w 1000:3000
	/// NQ -   185k - 250   620k/w   200:600
	/// TF -    95k - 100   420k/w	  50:150
	/// YM -   131k - 100   400k/w	 100:300
	/// GC -    50k - 100   300k/w    50:150
	/// 
	/// Would need a 150, 300, 600. 800. 1200. 2400
	/// 
    /// </summary>
    [Description("Multi-time frame strategy.")]
    public class TopDog2400 : Strategy
    {
        #region Variables
        // Wizard generated variables
			private int smaPeriod = 50; // Default setting for SmaPeriod
			private int emaPeriod = 15; // Default setting for EmaPeriod
			private string instrument = "CL";
		    private int secondPeriod = 100;
        // User defined variables (add any user defined variables below)
			//Stochastics stoc;
			//Stochastics stocX3;
			int stocD = 3;
			int stocK = 5;
			int stocS = 2;
		
			bool cycleNeutral = false;
			bool cycleHigh = false;
			bool cycleLow = false;
		
			bool isContinueationShort = false;
			bool isContinueationLong = false;
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			ClearOutputWindow();

			// Add a 5 minute Bars object to the strategy
			//Add(PeriodType.Minute, 5);		// just for test
			//Add(PeriodType.Tick, 800);	// this is the real medium term chart for CL
			Add(PeriodType.Tick, secondPeriod);	// using for testing and off hours
			
			// Add a 15 minute Bars object to the strategy
			//Add(PeriodType.Minute, 15);
			
			// Note: Bars are added to the BarsArray and can be accessed via an index value
			// E.G. BarsArray[1] ---> Accesses the 5 minute Bars object added above
			
			Add(PitColor(Color.Black, 83000, 25, 161500));
            
			// These only display for the primary Bars object on the chart BarsArray[0]
			Add(EMA(EmaPeriod));
			EMA(EmaPeriod).Plots[0].Pen.Color = Color.Gray;
			
            Add(SMARick(SmaPeriod));
			
            Add(Stochastics(stocD, stocK, stocS));
			Stochastics(stocD, stocK, stocS).Plots[0].Pen.Color = Color.Blue; // D color
			Stochastics(stocD, stocK, stocS).Plots[0].Pen.Width = 2;
			Stochastics(stocD, stocK, stocS).Lines[0].Pen.Color = Color.Black; // Lower
			Stochastics(stocD, stocK, stocS).Lines[0].Pen.DashStyle = DashStyle.Dot;
			Stochastics(stocD, stocK, stocS).Lines[0].Pen.Width = 2;
			Stochastics(stocD, stocK, stocS).Plots[1].Pen.Color = Color.Black; // K color
			Stochastics(stocD, stocK, stocS).Lines[1].Pen.Color = Color.Black; // Upper
			Stochastics(stocD, stocK, stocS).Lines[1].Pen.DashStyle = DashStyle.Dot;
			Stochastics(stocD, stocK, stocS).Lines[1].Pen.Width = 2;
			
			Add(ConstantLines(45,55,0,0)); 
			ConstantLines(45,55,0,0).Plots[0].Pen.Color = Color.Red;
			ConstantLines(45,55,0,0).Plots[1].Pen.Color = Color.Red;						
			
			Stochastics(stocD, stocK, stocS).Panel = 1;
			ConstantLines(45,55,0,0).Panel = 1;	
			
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// OnBarUpdate() will be called on incoming tick events on all Bars objects added to the strategy
			// We only want to process events on our primary Bars object (index = 0) which is set when adding
			// the strategy to a chart
			if (BarsInProgress != 0)
				return;
			
			// Stochastics stoc = Stochastics(BarsArray[0],3,5,2);
			
			// Here’s how we define Cycle highs and lows:
			// - A Cycle high is the highest high in price after %D goes above 45 and before it gets back below 55.
			//   
			// - A Cycle low is the lowest low in price after %D goes below 55 and before it gets back above 45.
			//   
			// - The 45-55 (middle) range on the stochastic indicator is a neutral zone.
			
			// Start of cycle high?
			if (CrossAbove(Stochastics(BarsArray[0],3,5,2).D, 45, 1))
			{
				cycleHigh = true;
				isContinueationShort = false;
				//DrawDiamond(CurrentBar + "crossAbove40", true, 0, High[0] + 2*TickSize, Color.White);
			}
			
			// Start of cycle low?
			if (CrossBelow(Stochastics(BarsArray[0],3,5,2).D, 55, 1))
			{
				cycleLow = true;
				isContinueationLong = false;
				//DrawDiamond(CurrentBar + "crossBelow60", true, 0, Low[0] - 2*TickSize, Color.Black);
			}
			
			if (CrossAbove(Stochastics(BarsArray[0],3,5,2).D, 55, 1))
			{
				cycleLow = false;
			}
			
			if (CrossBelow(Stochastics(BarsArray[0],3,5,2).D, 45, 1))
			{
				cycleHigh = false;
			}
			
			//Print(Time + " cycleLow: " + cycleLow + " cycleHigh: " + cycleHigh);
			
			// check for potential long position
			if (Rising(SMARick(BarsArray[0], 50)))
			{
				if (cycleLow)
				{
					if (Stochastics(BarsArray[0],3,5,2).K[0] <= 20)
					{
						isContinueationLong = true;
						DrawDot(CurrentBar + "potLong", true, 0, High[0] + 3 * TickSize, Color.Purple);
					}
					
					if (Stochastics(BarsArray[1],3,5,2).K[0] >= Stochastics(BarsArray[1],3,5,2).K[1])
					{
						if (isContinueationLong == true)
						{
							DrawArrowUp(CurrentBar + "validLong", 0, Low[0] - 3 * TickSize, Color.Green);
							PlaySound(@"C:\Program Files (x86)\NinjaTrader 7\sounds\Potential long.wav");
							//PlaySound(@"C:\Program Files (x86)\NinjaTrader 7\sounds\" + instrument + ".wav");
						}
						else
						{
							// medium trend validates direction
							DrawArrowUp(CurrentBar + "mtLong", 0, Low[0] - 3 * TickSize, Color.Yellow);
						}
					}
				}
			}
			else
			{
				if (cycleHigh)
				{
					if (Stochastics(BarsArray[0],3,5,2).K[0] >= 80)
					{
						isContinueationShort = true;
						DrawDot(CurrentBar + "potShort", true, 0, Low[0] - 3 * TickSize, Color.Pink);
					}
					
					if (Stochastics(BarsArray[1],3,5,2).K[0] <= Stochastics(BarsArray[1],3,5,2).K[1])
					{
						if (isContinueationShort == true)
						{
							DrawArrowDown(CurrentBar + "validShort", 0, High[0] + 3 * TickSize, Color.Red);
							PlaySound(@"C:\Program Files (x86)\NinjaTrader 7\sounds\Potential short.wav");
							//PlaySound(@"C:\Program Files (x86)\NinjaTrader 7\sounds\" + instrument + ".wav");
						}
						else
						{
							// medium trend validates direction
							DrawArrowDown(CurrentBar + "mtShort", 0, High[0] + 3 * TickSize, Color.Yellow);
						}
					}
				}
			}
			
			/*
			
				int crossedBelow60BarsAgo = crossedBelow60(0, 1, 50);
				int crossedAbove20BarsAgo = crossedAbove20(0, 1, 50);
			//Print(Time + " crossedBelow60BarsAgo: " + crossedBelow60BarsAgo + " crossedAbove20BarsAgo: " + crossedAbove20BarsAgo);
				if (crossedBelow60BarsAgo > crossedAbove20BarsAgo 
					//&& Stochastics(BarsArray[0],3,5,2).K[0] < 60
					) 
				{
					DrawDot(CurrentBar + "potentialLong", true, 0, High[0] + 3 * TickSize, Color.Lime);
				}
				
				
			// testing alert functionality
			if (Rising(SMARick(BarsArray[0], 50)) 
				&& Rising(Stochastics(BarsArray[0],3,5,2))
				&& Rising(Stochastics(BarsArray[1],3,5,2))
				)
			{				
				int longEnabledBarsAgo = mostRecentStocKHigherThan(0, 1, 50, 60);
				int longTriggerBarsAgo = mostRecentStocKLowerThan(0, 1, 50, 20);
				
				if (longEnabledBarsAgo > longTriggerBarsAgo)	// stoc 20 or less?
				{
					//Print(Time + " CL Bull Alert SMA50, Stoc ST, MT Rising");
					//Alert(CurrentBar + " Bull Alert", Priority.High, "SMA50, Stoc ST, MT Rising", @"C:\Program Files (x86)\NinjaTrader 7\sounds\Alert1.wav", 60, Color.White, Color.Black);
					//Log("CL Bull Alert SMA50, Stoc ST, MT Rising", NinjaTrader.Cbi.LogLevel.Information);
					PlaySound(@"C:\Program Files (x86)\NinjaTrader 7\sounds\Potential long.wav");
					DrawDot(CurrentBar + "potLong", true, 0, High[0] + 2 * TickSize, Color.Green);
					Print(Time + " longTriggerBarsAgo: " + longTriggerBarsAgo);										
				}
				
				
			}
			if (Falling(SMARick(BarsArray[0], 50)) 
				&& Falling(Stochastics(BarsArray[0],3,5,2))
				&& Falling(Stochastics(BarsArray[1],3,5,2))
				)
			{		
				Print(Time + " first step short");
				int shortEnabledBarsAgo = mostRecentStocKHigherThan(0, 1, 50, 40);
				int shortTriggerBarsAgo = mostRecentStocKLowerThan(0, 1, 50, 80);
				
				if (shortEnabledBarsAgo > shortTriggerBarsAgo)	// stoc 80 or more?
				{					
					Print(Time + " CL Bear Alert SMA50, Stoc ST, MT Falling");
					PlaySound(@"C:\Program Files (x86)\NinjaTrader 7\sounds\Potential short.wav");
					DrawDot(CurrentBar + "potShort", true, 0, Low[0] -  2 * TickSize, Color.Red);
					Print(Time + " shortTriggerBarsAgo: " + shortTriggerBarsAgo);
				}
				
				/*
				int crossedBelow60BarsAgo = crossedBelow60(0, 1, 50);
				if (crossedBelow60 > crossedAbove20 && !crossedAbove60) 
				{
					DrawDot(CurrentBar + "potentialShort", true, 0, High[0] + 3 * TickSize, Color.Pink);
				}
				
			}
			*/

        }
		
		/// <summary>
		/// 	Looks for the Stochastic value of K lower than the argument passed.
		/// </summary>
		/// <param name="bars">barsArray subscript</param>
		/// <param name="start">usually set to 1 (barsAgo)</param>
		/// <param name="end">max bars to look back</param>
		/// <param name="lowerThan">less than this value</param>
		/// <returns></returns>
		private int mostRecentStocKLowerThan(int bars, int start, int end, int lowerThan)
		{
			int barsAgo = start;
			while (Stochastics(BarsArray[bars],3,5,2).K[barsAgo] <= lowerThan && barsAgo < end)
			{
				barsAgo++;
			}
			return barsAgo;	
		}

		private int mostRecentStocKHigherThan(int bars, int start, int end, int higherThan)
		{
			int barsAgo = start;
			while (Stochastics(BarsArray[bars],3,5,2).K[barsAgo] >= higherThan && barsAgo < end)
			{
				barsAgo++;
			}
			return barsAgo;	
		}
		
		private int crossedAbove20(int bars, int start, int end)
		{
			int barsAgo = start;
			while (CrossAbove(Stochastics(BarsArray[bars],3,5,2).K, 20.0, 50) && barsAgo < end)
			{
				barsAgo++;
			}
			return barsAgo;
		}

		private int crossedAbove60(int bars, int start, int end)
		{
			int barsAgo = start;
			while (CrossAbove(Stochastics(BarsArray[bars],3,5,2).K, 60, 50) && barsAgo < end)
			{
				barsAgo++;
			}
			return barsAgo;
		}

		private int crossedBelow60(int bars, int start, int end)
		{
			int barsAgo = start;
			while (CrossBelow(Stochastics(BarsArray[bars],3,5,2).K, 60, barsAgo) && barsAgo < end)
			{
				barsAgo++;
			}
			return barsAgo;
		}

		private int crossedBelow80(int bars, int start, int end)
		{
			int barsAgo = start;
			while (CrossBelow(Stochastics(BarsArray[bars],3,5,2).K, 80, 50) && barsAgo < end)
			{
				barsAgo++;
			}
			return barsAgo;
		}


        #region Properties
		
        [Description("Instrument")]
        [GridCategory("Parameters")]
        public string Instrument
        {
            get { return instrument; }
            set { instrument = value; }
        }

        [Description("Second Period")]
        [GridCategory("Parameters")]
        public int SecondPeriod
        {
            get { return secondPeriod; }
            set { secondPeriod = value; }
        }

        [Description("SMA Period")]
        [GridCategory("Parameters")]
        public int SmaPeriod
        {
            get { return smaPeriod; }
            set { smaPeriod = Math.Max(1, value); }
        }

        [Description("EMA Period")]
        [GridCategory("Parameters")]
        public int EmaPeriod
        {
            get { return emaPeriod; }
            set { emaPeriod = Math.Max(1, value); }
        }
		
        #endregion
    }
}
