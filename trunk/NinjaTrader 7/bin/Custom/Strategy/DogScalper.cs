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
    /// This straegy is based on the Top Dog Trading rules.
	/// I'm currently working with NQ 200, 600 and 1800 Tick ($20/pt $5/tick, 1 tick = .25)
	/// or ES 1000, 3000, 9000 ticks
    /// </summary>
    [Description("This straegy is based on the Top Dog Trading rules.")]
    public class DogScalper : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int smaPeriod = 50; // Default setting for SmaPeriod
        private int emaPeriod = 15; // Default setting for EmaPeriod
        // User defined variables (add any user defined variables below)
		int FALLING = -1;
		int RISING = 1;
		int PEAK = 1;
		int VALLEY = -1;
		int trend = 0;  // +1 = rising, -1 = falling
		int trendStartBar = 0;
		int cycleHigh = int.MinValue;  // bar away from current cycle high
		int cycleLow = int.MaxValue;
		int lastCycleHighBar = 0;		
		int previousTrend = 0;
		
		Stochastics stoc;
		Stochastics stocX3;
		int stocD = 3;
		int stocK = 5;
		int stocS = 2;
		
		MACD macd;
		int macdFast = 5;
		int macdSlow = 20;
		int macdSmooth = 30;
		
		int cycleCount = 0;
		int trendCount = 0;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			Add(PitColor(Color.Black, 83000, 25, 161500));
            
			Add(EMA(EmaPeriod));
			EMA(EmaPeriod).Plots[0].Pen.Color = Color.Gray;
			
            Add(SMARick(SmaPeriod));
			//SMARick(SmaPeriod).Plots[0].Pen.Color = Color.Red;	
			//Add(SMARick(SmaPeriod * 3));
			
            Add(Stochastics(stocD, stocK, stocS));
			Stochastics(stocD, stocK, stocS).Plots[0].Pen.Color = Color.Blue; // D color
			Stochastics(stocD, stocK, stocS).Plots[1].Pen.Color = Color.Black; // K color
			Stochastics(stocD, stocK, stocS).Plots[0].Pen.Width = 2; // D color
			Stochastics(stocD, stocK, stocS).Plots[1].Pen.Width = 1; // K color
			
			Stochastics(stocD, stocK, stocS).Lines[0].Pen.Color = Color.Black; // Lower
			Stochastics(stocD, stocK, stocS).Lines[1].Pen.Color = Color.Black; // Upper
			Stochastics(stocD, stocK, stocS).Lines[0].Pen.DashStyle = DashStyle.Dot; // Lower
			Stochastics(stocD, stocK, stocS).Lines[1].Pen.DashStyle = DashStyle.Dot; // Upper
			Stochastics(stocD, stocK, stocS).Lines[0].Pen.Width = 2; // Lower
			Stochastics(stocD, stocK, stocS).Lines[1].Pen.Width = 2; // Upper
			Stochastics(stocD, stocK, stocS).Lines[0].Value = 20; // Lower
			Stochastics(stocD, stocK, stocS).Lines[1].Value = 80; // Upper
			
			ConstantLines(45,55,0,0).Panel = 1;	// specifying to use the first indicator's panel
			Add(ConstantLines(45,55,0,0)); 
			ConstantLines(45,55,0,0).Plots[0].Pen.Color = Color.Red;
			ConstantLines(45,55,0,0).Plots[1].Pen.Color = Color.Red;			

			Add(MACD(macdFast,macdSlow,macdSmooth));
			MACD(macdFast,macdSlow,macdSmooth).CalculateOnBarClose = true;
			MACD(macdFast,macdSlow,macdSmooth).Plots[0].Pen.Color = Color.Black;	// Macd
			MACD(macdFast,macdSlow,macdSmooth).Plots[1].Pen.Color = Color.Blue;	// Avg
			MACD(macdFast,macdSlow,macdSmooth).Plots[2].Pen.Color = Color.Red;		//downBard
//			MACD(macdFast,macdSlow,macdSmooth).Plots[3].Pen.Color = Color.Transparent;	// Diff
			MACD(macdFast,macdSlow,macdSmooth).Plots[0].Pen.Width = 2;
			MACD(macdFast,macdSlow,macdSmooth).Plots[1].Pen.Width = 3;
			MACD(macdFast,macdSlow,macdSmooth).Plots[0].PlotStyle = PlotStyle.Bar;
						
			//SetProfitTarget("", CalculationMode.Ticks, 205);
            //SetTrailStop("", CalculationMode.Ticks, 5, false);
			//SetStopLoss("", CalculationMode.Ticks, 200, false);
			
            CalculateOnBarClose = true;
			ClearOutputWindow();
			TimeInForce = Cbi.TimeInForce.Day;
			ExitOnClose = true;
			ExitOnCloseSeconds = 3600;	// 60 min before session close
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			stoc = Stochastics(stocD, stocK, stocS);
			//stocX3 = Stochastics(IDataSeries, stocD, stocK, stocS);
			
			int currentTrend = Rising(SMARick(SmaPeriod)) ? RISING: FALLING;

			// is SMA trend changing
			//if ((( Rising(SMARick(SmaPeriod))&& (SMARick(SmaPeriod)[1] - SMARick(SmaPeriod)[2]) < 0 ))
			//	|| (Falling(SMARick(SmaPeriod)) && (SMARick(SmaPeriod)[1] - SMARick(SmaPeriod)[2]) > 0))
			//{

			if (currentTrend != previousTrend)
			{
			//if (Rising(SMARick(SmaPeriod)) != Rising(SMARick(SmaPeriod))[1])
			//{
			//if (currentTrend > 0 && trend < 0 ||
			//	currentTrend < 0 && trend > 0) 
				//|| (trend >= 0 && currentTrend == -1))
			//{
				// reset trend
				//trend = currentTrend;
				trendStartBar = CurrentBar;
				trendCount++;
				cycleHigh = int.MaxValue;
				cycleLow = int.MinValue;
				
				if (currentTrend == FALLING)
					DrawText(CurrentBar + " barCount", CurrentBar + " ", 0, 2 * (High[0] - Low[0]) + High[0], Color.DarkRed);
				if (currentTrend == RISING)
					DrawText(CurrentBar + " barCount", CurrentBar + " ", 0, 2 * (High[0] - Low[0]) + High[0], Color.DarkGreen);
				//	cycleCount = 0;
			}
			
			//if (((SMARick(SmaPeriod)[0] - SMARick(SmaPeriod)[1]) > 0 && (SMARick(SmaPeriod)[1] - SMARick(SmaPeriod)[2]) < 0 )
			//	|| ((SMARick(SmaPeriod)[0] - SMARick(SmaPeriod)[1]) < 0 && (SMARick(SmaPeriod)[1] - SMARick(SmaPeriod)[2]) > 0 ))
			//{
			//	cycleCount = 0;
			//}
			
			MarkCycleHigh();			
			MarkCycleLow();		
			IdentifySecondChance();
			
			Scalp();
			
			// In-Trend note:
			// multi-hook entry pattern provides a higher probability that we are making a cycle high or low, than a single hook.
			// even more so, is when the price falls with the hooks are going up (long entry)
			// example in doc on pg 24 is using Stoc D
			// for the example above, if it's down trend and you're short, it could signal time to exit
			
			/*
			uptrend conditions
			1 - new
			2 - cross above 45, look for cycle high
			3 - cross above 55
			4 - cross below 55, stop cycle high search
			*/
			if (currentTrend == 1) 
			{
				switch (trend) {
					case 1:
						if (stoc.D[0] > 45)
							trend = 2;
						if (stoc.D[0] > 55)
							trend = 3;						
						break;
					case 2:
						if (stoc.D[0] > 55)
							trend = 3;						
						if (CrossBelow(stoc.D, 45, 1))
							trend = 1;
						break;
					case 3:
						if (CrossBelow(stoc.D, 55, 1))
							trend = 4;
						break;
					default:
						
						break;
				}
			}
			if (currentTrend == -1) 
			{
				switch (trend) {
					case -1:
						if (stoc.D[0] < 55)
							trend = -2;
						if (stoc.D[0] < 45)
							trend = -3;						
						break;
					case -2:
						if (stoc.D[0] < 45)
							trend = -3;						
						if (CrossAbove(stoc.D, 55, 1))
							trend = -1;
						break;
					case -3:
						if (CrossAbove(stoc.D, 55, 1))
							trend = -4;
						break;
					default:
						
						break;
				}
			}
			
			if (trend >= 1) 
			{
				cycleHigh = HighestBar(High, CurrentBar - trendStartBar);
			}
			if (trend <= -1)
				cycleLow = LowestBar(Low, CurrentBar - trendStartBar);	
			
            if (trend < 0)
			{
				//DrawDot("Low" + CurrentBar, false, 0, Low[cycleLow], Color.Black);
			}
			
            if (trend > 0)
			{
				//DrawDot("High" + CurrentBar, false, 0, High[cycleHigh], Color.Black);
			//	Print(Time + " cycleHigh: " + cycleHigh);
				
				if (stoc.D[2] > stoc.D[1]
	                && stoc.D[1] < stoc.D[0]
					)
				{
					DrawArrowUp("My up arrow" + CurrentBar, false, 0, Low[0], Color.Lime);
					
					if (Position.MarketPosition == MarketPosition.Flat)
					{
						//EnterLongStop(High[0] + 1 * TickSize);
					}
				}            
            }
			
			previousTrend = currentTrend;
        }

		void Scalp()
		{
			if (Position.MarketPosition == MarketPosition.Flat)
			{
				// Trade in the direction of the trend
				// Long Condition
				if (Rising(SMARick(SmaPeriod)))
				{
					// retracing to the extreme
					if (stoc.D[1] <= 20)
					{
						if (DHookBottom(0))
						{
							// confirmed on the higher time frame chart that
							// stocK is leading stocD, PG 59
							if (true) 
							{
								//EnterLong("DHookUp");
								EnterLongStop(High[0] + 1 * TickSize, "DHookUp");
								double stopLoss = (High[0] - Low[0])/TickSize + 2;
								double profitTarget = stopLoss + 2;
								SetStopLoss(CalculationMode.Ticks, stopLoss);
								SetProfitTarget(CalculationMode.Ticks, profitTarget);
								Print(Time + " Close[0]: " + Close[0] + " stopLoss: " + stopLoss + " profitTarget: " + profitTarget);
							}
						}
					}
					
				}
			}
		}
		
		/// <summary>
		/// 	Identifies Second Chance Patterns
		/// 	<ul>
		/// 		<li>Mini-Divergence - divergence from the trend</li>
		/// 		<li>Mini-Convergence</li>
		/// 		<li>Triple Cross</li>
		/// 		<li>Head and Shoulders</li>
		/// 		<li>Double Tops and Bottoms</li>
		/// 		<li>The Mitten Pattern</li>
		/// 	</ul>
		/// </summary>
		void IdentifySecondChance()
		{
			//MarkKHookBottom();		
			if (Rising(SMARick(SmaPeriod)))
				MarkKDoubleBottom();
			if (Falling(SMARick(SmaPeriod)))
				MarkKDoubleTop();
		}
		
		/// <summary>
		/// A cycle high is defined starting by K CrossBelow 20 back to K CrossAbove 45, with
		/// K reasching 80 or more
		/// </summary>
		void MarkCycleHigh()
		{
			if (CrossBelow(stoc.K, 20.0, 1)) 
			{
				int barsAgo = 1;
				DrawDot(CurrentBar + "CrossedBelow", true, 0, High[0] + 1 * TickSize, Color.Yellow);
				bool failed = true;
				
				// TODO - this is where I was working on when I stopped.
				
				while (trendStartBar < CurrentBar 
					&& !CrossAbove(stoc.K, 20.0, barsAgo)
					&& !CrossAbove(stoc.K, 20.0, barsAgo))
					
				{
				//	if (stoc.K[barsAgo] >= 80)
				//	{
				//		failed = false;
				//	}
					//CrossAbove(stoc.K[barsAgo] > 45);
					if (stoc.K[barsAgo] > 80)
						failed = false;	// peak found
					barsAgo++;
				}
				//if (cycleCount % 2 == 0)
				cycleCount++;
				
				int highBarsAgo = HighestBar(Close, barsAgo);
				//if (CurrentBar > 7160)
				//	highBarsAgo = HighestBar(Close, CurrentBar - 7160);
				//if (highBarsAgo > trendStartBar)
				//{
					//	if (cycleCount % 2 == 0)
					Print(Time + " CurrentBar: " + CurrentBar + " highBarsAgo: " + highBarsAgo + " highBarNumber: " + (CurrentBar - highBarsAgo) + "  trendStartBar: " + trendStartBar);
						cycleCount++;
					DrawDot(CurrentBar + "BarsAgo", true, barsAgo, High[0] + 1 * TickSize, Color.Salmon);
					DrawDot(CurrentBar + "High", true, highBarsAgo, 2 * High[highBarsAgo] - Low[highBarsAgo], Color.Blue);
					DrawText(CurrentBar + "cycleCount", cycleCount.ToString() + " " + failed, highBarsAgo, 2 * (High[highBarsAgo] - Low[highBarsAgo]) + High[highBarsAgo], Color.Black);
					//lastCycleHighBar = highBar;
				//}//
			}
		}
		
		/// <summary>
		/// A cycle low is defined starting by K CrossAbove 80 back to K CrossAbove 55, with k
		/// reaching 20 or less
		/// </summary>
		void MarkCycleLow()
		{
			if (CrossAbove(stoc.D, 45.0, 1)) 
			{
				int barsAgo = 1;
				//DrawDot(CurrentBar + "CrossedBelow", true, 0, High[0] + 1 * TickSize, Color.Yellow);
				while (stoc.D[barsAgo] < 55)
				{
					barsAgo++;
				}
				//if (cycleCount % 2 == 0)
				//	cycleCount++;
				
				int lowBar = LowestBar(Close, barsAgo);
				if (trendStartBar > lowBar)
				{
				//	if (cycleCount % 2 == 1)
						cycleCount++;
					//DrawDot(CurrentBar + "BarsAgo", true, barsAgo, High[0] + 1 * TickSize, Color.Salmon);
					DrawDot(CurrentBar + "Low", true, lowBar, 2 * Low[lowBar] - High[lowBar], Color.Red);
					DrawText(CurrentBar + "cycleCount", cycleCount.ToString(), lowBar, Low[lowBar] - (High[lowBar] - Low[lowBar]) * 2, Color.Black);				
				}
			}
		}
		
		void MarkKHookBottom()
		{
			if (KHookBottom(0))
			{
				DrawVerticalLine(CurrentBar + "KHookBottom", 0, Color.Blue);
			}
		}
		
		bool DHookBottom(int barsAgo)
		{
			if (stoc.D[barsAgo + 2] >= stoc.D[barsAgo + 1]
				&& stoc.D[barsAgo + 1] < stoc.D[barsAgo + 0] 
				&& stoc.D[barsAgo + 1] <= 45)
			{
				return true;
			}
			return false;
		}
		
		bool KHookBottom(int barsAgo)
		{
			if (stoc.K[barsAgo + 2] >= stoc.K[barsAgo + 1]
				&& stoc.K[barsAgo + 1] < stoc.K[barsAgo + 0] 
				&& stoc.K[barsAgo + 1] <= 45)
			{
				return true;
			}
			return false;
		}
		
		bool DHookTop(int barsAgo)
		{
			if (stoc.D[barsAgo + 2] <= stoc.D[barsAgo + 1]
				&& stoc.D[barsAgo + 1] > stoc.D[barsAgo + 0] 
				&& stoc.D[barsAgo + 1] >= 55)
			{
				return true;
			}
			return false;
		}
		
		bool KHookTop(int barsAgo)
		{
			if (stoc.K[barsAgo + 2] <= stoc.K[barsAgo + 1]
				&& stoc.K[barsAgo + 1] > stoc.K[barsAgo + 0] 
				&& stoc.K[barsAgo + 1] >= 55)
			{
				return true;
			}
			return false;
		}
		
		void MarkKDoubleBottom()
		{
			if (KDoubleBottom())
			{
				DrawVerticalLine(CurrentBar + "LowerDivergence", 1, Color.Salmon);
			}
		}
			
		void MarkKDoubleTop()
		{
			if (KDoubleTop())
			{
				DrawVerticalLine(CurrentBar + "UpperDivergence", 1, Color.Cyan);
			}
		}
			
		/// <summary>
		/// The key is to ignore all of the choppy hooks on %K, and only trade hooks on %K when %D is in
		/// place for a potential Cycle High/Low and you get a divergence between price and %K with the
		/// Trend.
		/// </summary>
		bool KDoubleBottom()
		{
			if (KHookBottom(0))
			{
				int barsAgo = 2;
				while (stoc.K[barsAgo + 2] < 45)
				{
					if (KHookBottom(barsAgo))
					{
						return true;
					}			
					barsAgo++;
				}
			}
			return false;
		}
		
		bool KDoubleTop()
		{
			if (KHookTop(0))
			{
				int barsAgo = 2;
				while (stoc.K[barsAgo + 2] > 55)
				{
					if (KHookTop(barsAgo))
					{
						return true;
					}			
					barsAgo++;
				}
			}
			return false;
		}
		
		
        #region Properties
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
