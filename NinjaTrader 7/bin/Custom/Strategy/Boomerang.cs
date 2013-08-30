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
    /// Strategy based on ideas from the boomerangtrader.com Writen by Rick Cromer 2013
	/// Trading based on NQ
    /// </summary>
    [Description("Strategy based on ideas from the boomerangtrader.com Writen by Rick Cromer 2013")]
    public class Boomerang : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int lookBackPeriod = 6; // Default setting for LookBackPeriod
        private int signalLinePeriod = 10; // Default setting for SignalLinePeriod
		private int threshold = 60;
		
		int profitTarget = 12;
		int stopLoss = 10;

		
        // User defined variables (add any user defined variables below)
		int priceTriggerPeriod = 5;		// entry price trigger
		int slowSignalPeriod = 14;	// slow signal
		int HmaPeriod2 = 50;
		
		// used for long term trend, when signal crosses look to open
		// long band is when signal is above red slowSignalPeriod - slow signal
		int trendBand = 120;
		
		IDataSeries signal;
		HMARick dynamicTrend;
		IDataSeries priceTrigger;
		IDataSeries slowSignal;
		
		int maxTradesPerTrend = 1;
		int tradesInTrend = 0;
		int trend = 0;	// -1 - short, 0 - neutral, 1 - long		
		bool onlyLongs = true;
		int trendStartBar = 0;
		
        #endregion

		protected override void OnStartUp()
		{
			signal = HMA(signalLinePeriod);
			dynamicTrend = HMARick(trendBand, Threshold);
			priceTrigger = SMARick(priceTriggerPeriod);
			slowSignal = SMA(slowSignalPeriod);
			
			dynamicTrend.DownColor = Color.Purple;
			dynamicTrend.UpColor = Color.DarkCyan;
			dynamicTrend.PaintPriceMarkers = false;

		}
		
		/// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = false;
			
			Add(PitColor(Color.Black, 83000, 25, 161500));
			Add(TickCounter(true, false));
			
			Add(SMA(slowSignalPeriod));
			SMA(slowSignalPeriod).Plots[0].Pen.Color = Color.Red;
			SMA(slowSignalPeriod).PaintPriceMarkers = false;
			
			// TMA(40) may be an interesting alternative
			// DWMA(70) is also interesting
			Add(HMARick(trendBand, threshold));

			Add(SMARick(priceTriggerPeriod));			
			SMARick(priceTriggerPeriod).DownColor = Color.Firebrick;
			SMARick(priceTriggerPeriod).UpColor = Color.RoyalBlue;
			SMARick(priceTriggerPeriod).Dash0Style = DashStyle.Dash;
			SMARick(priceTriggerPeriod).PaintPriceMarkers = false;
			
			Add(HMA(signalLinePeriod));
			HMA(signalLinePeriod).Plots[0].Pen.Color = Color.DimGray;
			HMA(signalLinePeriod).Plots[0].Pen.DashStyle = DashStyle.Solid;
			HMA(signalLinePeriod).Plots[0].Pen.Width = 2;
			HMA(signalLinePeriod).PaintPriceMarkers = false;
			
			Add(HallMaColored(HmaPeriod2));
			HallMaColored(HmaPeriod2).DownColor = Color.HotPink;
			HallMaColored(HmaPeriod2).UpColor = Color.MediumAquamarine;
			HallMaColored(HmaPeriod2).Dash0Style = DashStyle.DashDot;
			HallMaColored(HmaPeriod2).Plot0Width = 2;
			HallMaColored(HmaPeriod2).PaintPriceMarkers = false;
			
			SetProfitTarget(CalculationMode.Ticks, ProfitTarget);
			SetStopLoss(CalculationMode.Ticks, StopLoss);
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			
			// Neutral Trend == 0
			if (dynamicTrend.TrendSet[0] != 0 && dynamicTrend.TrendSet[0] != trend)
			{
				//tradesInTrend = 0;
				//trend = (int) dynamicTrend.TrendSet[0];
			}
			
			if (signal[0] > dynamicTrend[0] && trend != 1)
			{
				tradesInTrend = 0;
				trend = 1;
				trendStartBar = CurrentBar;
			}
			if (signal[0] < dynamicTrend[0] && trend != -1)
			{
				tradesInTrend = 0;
				trend = -1;
				trendStartBar = CurrentBar;
			}

			
			// Only process entry signals on a bar by bar basis (not tick by tick)
			if (FirstTickOfBar)
			{
				//DrawText(CurrentBar + "x", "C", 0, High[0] + 4 * TickSize, Color.Red);
			}

			
			bool tradingEnabled = true;
			if (false)
			{
				if (ToTime(Time[0]) >= 83000 && ToTime(Time[0]) <= 143000) 
				//if (ToTime(Time[0]) >= 103000 && ToTime(Time[0]) <= 143000) 	// worse time
				{
					tradingEnabled = true;
				} else
				{
					tradingEnabled = false;
				}
			}	

			
			if (CrossAbove(signal, slowSignal, 1))
			{
				//BackColorAll = Color.PaleTurquoise;
				//DrawSquare(CurrentBar + "sq1", false, 0, slowSignal[0], Color.Black);
				DrawArrowUp(CurrentBar + "Up", 0, slowSignal[0] - 2 * TickSize, Color.Green);
				onlyLongs = true;
				tradesInTrend = 0;
			}
			if (CrossAbove(slowSignal, signal, 1))
			{
				//BackColorAll = Color.Plum;
				//DrawSquare(CurrentBar + "sq1", false, 0, slowSignal[0], Color.Black);
				DrawArrowDown(CurrentBar + "Down", 0, slowSignal[0] + 2 * TickSize, Color.Red);
				onlyLongs = false;
				tradesInTrend = 0;
			}
			
			if (tradingEnabled && Position.MarketPosition == MarketPosition.Flat) 
			{
				//Print(Time + " Position.MarketPosition: Flat");
				if (onlyLongs)
				{
					if (FirstTickOfBar)
					{
						//DrawText(CurrentBar + "L", "L", 0, LowestBar(Low, 10) - 2 * TickSize, Color.Green);
						//Print(Time + " onlyLongs");
						//DrawText(CurrentBar + "L", "L", 0, Low[0] - 4 * TickSize, Color.Green);
					}
					
					if (CrossAbove(signal, dynamicTrend, lookBackPeriod))
					{
						if (FirstTickOfBar)
						{
							//Print(Time + " Cross Above");
							//DrawText(CurrentBar + "L", "C", 0, Low[0] - 2 * TickSize, Color.Green);
						}
						
						if (tradesInTrend < maxTradesPerTrend)
						{
							// has been ignored since the stop price is 
							// less than or equal to the close price of the current bar. 
							if (priceTrigger[0] > dynamicTrend[0])
							{
								DrawDot(CurrentBar + "dot1", true, 0, priceTrigger[0], Color.Green);
								if (Close[0] > priceTrigger[0])
								{
									EnterLongLimit(priceTrigger[0]);
								}
								else
								{
									EnterLongStop(priceTrigger[0]);
								}
							//	tradesInTrend++;
							//	DrawText(CurrentBar + (" " + tradesInTrend), " " + tradesInTrend, 0, Low[0] - 2 * TickSize, Color.Black);
							}
						}
						else
						{					
							if (FirstTickOfBar)
								DrawText(CurrentBar + "M", "M", 0, Low[0] - 2 * TickSize, Color.Green);
						}
					}
				}
				
				if (!onlyLongs)
				{
					//DrawText(CurrentBar + "S", "S", 0, HighestBar(High, 10) + 2 * TickSize, Color.Red);
					//Print(Time + " not onlyLongs");
					if (CrossAbove(dynamicTrend, signal, lookBackPeriod))
					{
						//Print(Time + " Cross Below");
						//DrawText(CurrentBar + "S", "C", 0, HighestBar(High, 10) + 2 * TickSize, Color.Red);
						if (tradesInTrend < maxTradesPerTrend)
						{
							DrawDot(CurrentBar + "dot2", false, 0, priceTrigger[0], Color.Red);
							if (tradingEnabled && Close[0] >= priceTrigger[0])
							{
								EnterShort();
							//	tradesInTrend++;
							}
						}
						else
						{					
							DrawText(CurrentBar + "S", "M", 0, High[HighestBar(High, 10)] + 2 * TickSize, Color.Red);
						}
					}
						/*
					else 
					{
						if (signal[0] < dynamicTrend[0])
							Print(CurrentBar + " below dynamic trend band");
						else
							Print(CurrentBar + " above dynamic trend band");
					}
						*/
				}
			}
			else
			{
				//Print(Time + " Position.MarketPosition: " + Position.MarketPosition);
			}
			
			
        }

		protected override void OnExecution(IExecution execution)
		{

			// Remember to check the underlying IOrder object for null before trying to access its properties
			if (execution.Order != null && execution.Order.OrderState == OrderState.Filled)
			{
				Print(Time + " - " + execution.ToString());
			}
		}
		
        #region Properties
		/// </summary>
		[Description("Slope as percentage of average true range")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Neutral Threshold")]
		public int Threshold 
		{
			get { return threshold; }
			set { threshold = Math.Max(0, value); }
		}

        [Description("Bars to look back to signal crossing trend")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Look Back Period")]
        public int LookBackPeriod
        {
            get { return lookBackPeriod; }
            set { lookBackPeriod = Math.Max(1, value); }
        }        
		
        [Description("Profit Target in ticks")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Profit Target")]
        public int ProfitTarget
        {
            get { return profitTarget; }
            set { profitTarget = Math.Max(4, value); }
        }        
		
        [Description("Stop Loss in ticks")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Stop Loss")]
        public int StopLoss
        {
            get { return stopLoss; }
            set { stopLoss = Math.Max(4, value); }
        }        
		
		[Description("HMA Period")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Signal Line Period")]
        public int SignalLinePeriod
        {
            get { return signalLinePeriod; }
            set { signalLinePeriod = Math.Max(1, value); }
        }
        #endregion
    }
}
