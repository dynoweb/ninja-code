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
	/// Designed for Better Renko 10 ticks
	/// 1/12-9/18/12 27,390.00
	/// 
	/// Update, when I ran it today, 9/24/12 for the same time period as before
	/// 1/12-9/18/12 17,610.00 - Better Renko 10 ticks
	///
	/// 
	/// I've been running this for 4 BetterRenko and it's looking really good, 
	/// it needs to be back tested to see if that's valid. 
	///
    /// </summary>
    [Description("Enter the description of your strategy here")]
    public class BetterRenkoCL : Strategy
    {
        #region Variables
			// The following vars are from my ATM Strategy Simulator
			// currently only 1 profit trigger is supported
			private int abeProfitTrigger = 9; // Sets stop loss to B/E when price moves to this target
			
			private int atFrequency1 = 1; // Auto Trail update Frequency
			private int atFrequency2 = 1; // Auto Trail update Frequency
			private int atFrequency3 = 1; // Auto Trail update Frequency

			private int atProfitTrigger1 = 10; // Auto Trail AtProfitTrigger
			private int atProfitTrigger2 = 0; // Auto Trail AtProfitTrigger
			private int atProfitTrigger3 = 0; // Auto Trail AtProfitTrigger

			private int atStopLoss1 = 5; // Auto Trail AtStopLoss		
			private int atStopLoss2 = 1; // Auto Trail AtStopLoss        
			private int atStopLoss3 = 1; // Auto Trail AtStopLoss
			
			private int profitTarget = 19; // Closes once reached, if 0 will be ignored ProfitTarget
			private int qty = 1; // Order Qty
			private int stopLoss = 14; // StopLoss
						
			//int trailStop = 56;
			//int profitTarget = 43;
			static int LONG = 1;
			static int SHORT = -1;
			static int NUTRAL = 0;
			int lastTrade = NUTRAL;
			double atrTimes = 5.0;
			int atrPeriod = 8;
			double atrRatched = 0.0;
			ATRTrailing atr;
			int hmaFastPeriod = 10;
			int hmaMedPeriod = 25;
			int hmaSlowPeriod = 50;
			HMA hmaFast;
			HMA hmaMed;
			HMA hmaSlow;
			//ADXVMA adx;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			//Add(ATRTrailing(atrTimes, atrPeriod, atrRatched));
			
			Add(HMA(hmaFastPeriod));
			HMA(hmaFastPeriod).Plots[0].Pen.Color = Color.Yellow;
			Add(HMA(hmaMedPeriod));
			HMA(hmaMedPeriod).Plots[0].Pen.Color = Color.Blue;
			Add(HMA(hmaSlowPeriod));
			HMA(hmaSlowPeriod).Plots[0].Pen.Color = Color.Lime;
			Add(ADXVMA(hmaFastPeriod));
			//ADXVMA(hmaFastPeriod).Plots[0].Pen.Color = Color.White;
			
            CalculateOnBarClose = true;			
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			atr = ATRTrailing(atrTimes, atrPeriod, atrRatched);
			hmaFast = HMA(hmaFastPeriod);
			hmaMed = HMA(hmaMedPeriod);
			hmaSlow = HMA(hmaSlowPeriod);
			//adx = ADXVMA(hmaFastPeriod);
			
			if (Bars.FirstBarOfSession) lastTrade = NUTRAL;
				
			if (Position.MarketPosition != MarketPosition.Flat) {
				//ManageTrade();	
				AtmStrategyHandler();
				return;
			}
			
			//resetLastTrade();
			//DrawDot(CurrentBar + "adx", false, 0, adx.ADXVMAPlot[0], Color.Black);
						
			if (Close[0] > Close[1] 
				&& Close[1] > Close[2] 
				&& Close[2] > Close[3] 
				//&& lastTrade != LONG
				&& Rising(hmaSlow)
				&& (BarsSinceExit() > 4 || BarsSinceExit() == -1)
			//	&& Close[1] < adx.ADXVMAPlot[0]
				)  
			{
				//Print(Time + " - Slope: " + Slope(adx.ADXVMAPlot, 1, 0));
				GoLong();
			} else if (Close[0] < Close[1] 
				&& Close[1] < Close[2] 
				&& Close[2] < Close[3] 
				//&& lastTrade != SHORT
				&& Falling(hmaSlow)
				&& (BarsSinceExit() > 4 || BarsSinceExit() == -1)
			//	&& Close[1] > adx.ADXVMAPlot[0]
				)
			{
				GoShort();
			}	
        }
		
		private void ManageTrade() 
		{
			/*
			if (Position.MarketPosition == MarketPosition.Short) {
				if (Low[0] > hmaFast[0] || Rising(hmaFast))
					ExitShort("Cover Short", "Short");
			}
			
			if (Position.MarketPosition == MarketPosition.Long) {
				if (High[0] < hmaFast[0] || Falling(hmaFast))
					ExitLong("Close Long", "Long");
			}
			*/
		}

		private void GoLong() 
		{
//			SetTrailStop("Long", CalculationMode.Ticks, TrailStop, false);
//			SetProfitTarget("Long", CalculationMode.Price, Close[0] + (ProfitTarget*TickSize));
			
			EnterLong("Long");
			lastTrade = LONG;
		}

		private void GoShort() 
		{
//			SetTrailStop("Short", CalculationMode.Ticks, TrailStop, false);
//			SetProfitTarget("Short", CalculationMode.Price, Close[0] - (ProfitTarget*TickSize));

			//EnterShortLimit(Close[0] + 2 * TickSize);  // - $4,270
			EnterShortStopLimit(Close[0], Close[0], "StopLimit");
			//EnterShort("Short");   // + $995
			lastTrade = SHORT;
		}
		
		private void resetLastTrade() {
		
			if (lastTrade == LONG 
				&& atr.Upper[1] < Low[1]
				&& atr.Lower[0] > High[0]
				)
			{
				//lastTrade = NUTRAL;
				//this.BarColor = Color.Black;
			}
			
			if (lastTrade == SHORT 
				&& isAlgoStopPos()
				)
			{
				//lastTrade = NUTRAL;
				//this.BackColor = Color.Red;
			}
		}
		
		protected Boolean isAlgoStopPos() {
			return atr.Upper[0] < Low[0];	
		}

		protected Boolean isAlgoStopNeg() {
			return atr.Lower[0] >  High[0];
			//return ATRTrailing(ATRTimes, ATRPeriod, ATRRatched).Lower[0] > High[0];	
		}

		private void AtmStrategyHandler() 
		{
			double approxEntry = Instrument.MasterInstrument.Round2TickSize(Position.AvgPrice);
			double highSinceOpen = High[HighestBar(High, BarsSinceEntry())];
			double lowSinceOpen = Low[LowestBar(Low, BarsSinceEntry())];
			double stop = 0;
			double limit = 0;
			
			if (Position.MarketPosition == MarketPosition.Long)
			{
				// Stoploss
				stop = approxEntry - StopLoss * TickSize;
				
				// Break Even Trigger
				if (AbeProfitTrigger > 0 && Close[0] > approxEntry + AbeProfitTrigger * TickSize)
					stop = approxEntry;

				//private int atFrequency1 = 1; // Auto Trail update Frequency
				//private int atProfitTrigger1 = 1; // Auto Trail AtProfitTrigger
				//private int atStopLoss1 = 1; // Auto Trail AtStopLoss
				
				// Trailing Stop
				if (AtProfitTrigger1 > 0 && highSinceOpen > approxEntry + AtProfitTrigger1 * TickSize) 
					stop = Math.Max(stop, highSinceOpen - (atStopLoss1 * TickSize));
				
				// Profit Target
				if (ProfitTarget > 0)
					limit = approxEntry  + ProfitTarget * TickSize;
								
				//Print(Time + ", stop: " + stop + " Close[0]: " + Close[0]);
				if (stop < Close[0])
				{
					DrawDot(CurrentBar+"closeL", true, 0, stop, Color.Pink);
					ExitLongStop(qty, stop);					
					if (limit != 0)
					{
						if (Close[0] > limit)
							ExitLong(qty);
						else 
							ExitLongLimit(qty, limit);
					}
				} else {
					ExitLong(qty);
				}
			}				
				
			if (Position.MarketPosition == MarketPosition.Short)
			{
				// Stoploss
				stop = approxEntry + StopLoss * TickSize;
				
				// Break Even Trigger
				if (AbeProfitTrigger > 0 && lowSinceOpen < approxEntry - AbeProfitTrigger * TickSize)
					stop = approxEntry;

				// Trailing Stop
				if (AtProfitTrigger1 > 0 && lowSinceOpen < approxEntry - AtProfitTrigger1 * TickSize) 
					stop = Math.Min(stop, lowSinceOpen + (atStopLoss1 * TickSize));
				
				// Profit Target
				if (ProfitTarget > 0)
					limit = approxEntry - ProfitTarget * TickSize;
								
				//DrawText("cc", "XX", 0, bolGreen.Lower[0], Color.White);
				
				if (stop > Close[0])
				{
					DrawDot(CurrentBar+"closeS", true, 0, stop, Color.Purple);
					ExitShortStop(qty, stop);
					if (limit != 0)
					{
						if (Close[0] < limit)
							ExitShort(qty);
						else
							ExitShortLimit(qty, limit);
					}
				} else {
					ExitShort(qty);
				}
			}
		}
		
        #region Properties

		/*
        [Description("")]
        [GridCategory("Parameters")]
		public int ProfitTarget
        {
            get { return profitTarget; }
            set { profitTarget = Math.Max(1, value); }
        }
		
        [GridCategory("Parameters")]
		public int TrailStop
        {
            get { return trailStop; }
            set { trailStop = Math.Max(1, value); }
        }
		*/
		
		// ==================================================================
		// The following Properties are part of Rick's ATM Strategy Simulator
		// ==================================================================
        [Description("Auto Break Even Profit Trigger")]
        [GridCategory("Parameters")]
        public int AbeProfitTrigger
        {
            get { return abeProfitTrigger; }
            set { abeProfitTrigger = Math.Max(1, value); }
        }

        [Description("Profit Target")]
        [GridCategory("Parameters")]
        public int ProfitTarget
        {
            get { return profitTarget; }
            set { profitTarget = Math.Max(0, value); }
        }

        [Description("Order Quantity")]
        [GridCategory("Parameters")]
        public int Qty
        {
            get { return qty; }
            set { qty = Math.Max(1, value); }
        }

        [Description("Stop Loss")]
        [GridCategory("Parameters")]
        public int StopLoss
        {
            get { return stopLoss; }
            set { stopLoss = Math.Max(1, value); }
        }

        [Description("Auto trail Stop loss")]
        [GridCategory("Parameters")]
        public int AtStopLoss1
        {
            get { return atStopLoss1; }
            set { atStopLoss1 = Math.Max(1, value); }
        }

        [Description("Auto trail Frequency")]
        [GridCategory("Parameters")]
        public int AtFrequency1
        {
            get { return atFrequency1; }
            set { atFrequency1 = Math.Max(1, value); }
        }

        [Description("Auto trail Profit Trigger")]
        [GridCategory("Parameters")]
        public int AtProfitTrigger1
        {
            get { return atProfitTrigger1; }
            set { atProfitTrigger1 = Math.Max(0, value); }
        }

        [Description("Auto trail Stop loss")]
        [GridCategory("Parameters")]
        public int AtStopLoss2
        {
            get { return atStopLoss2; }
            set { atStopLoss2 = Math.Max(1, value); }
        }

        [Description("Auto trail Frequency")]
        [GridCategory("Parameters")]
        public int AtFrequency2
        {
            get { return atFrequency2; }
            set { atFrequency2 = Math.Max(1, value); }
        }

        [Description("Auto trail Profit Trigger")]
        [GridCategory("Parameters")]
        public int AtProfitTrigger2
        {
            get { return atProfitTrigger2; }
            set { atProfitTrigger2 = Math.Max(0, value); }
        }

        [Description("Auto trail Stop loss")]
        [GridCategory("Parameters")]
        public int AtStopLoss3
        {
            get { return atStopLoss3; }
            set { atStopLoss3 = Math.Max(1, value); }
        }

        [Description("Auto trail Frequency")]
        [GridCategory("Parameters")]
        public int AtFrequency3
        {
            get { return atFrequency3; }
            set { atFrequency3 = Math.Max(1, value); }
        }

        [Description("Auto trail Profit Trigger")]
        [GridCategory("Parameters")]
        public int AtProfitTrigger3
        {
            get { return atProfitTrigger3; }
            set { atProfitTrigger3 = Math.Max(0, value); }
        }
		
        #endregion
    }
}
