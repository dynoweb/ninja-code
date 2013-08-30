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
    /// This will try to jump in on pit open in the direction of the market open
	/// 
	/// Note:
	/// CL Pit trading - Monday – Friday 9:00 AM to 2:30 PM (8:00 AM to 1:30 PM CT)
    /// </summary>
    [Description("Optimized for CL - 5 min - This will try to jump in on pit open in the direction of the market open")]
    public class PitOpen : Strategy
    {
        #region Variables

		private int period = 3; // Default setting for MyInput0
		private int hour = 8;
		private int minute = 0;
		private bool allowReversal = false;
		
		// ATM Strategy Variables
        private int abeProfitTrigger = 20; // Sets stop loss to B/E when price moves to this target
        
		private int atFrequency1 = 1; // Auto Trail update Frequency
        private int atFrequency2 = 1; // Auto Trail update Frequency
		private int atFrequency3 = 1; // Auto Trail update Frequency

		private int atProfitTrigger1 = 40; // Auto Trail AtProfitTrigger
        private int atProfitTrigger2 = 0; // Auto Trail AtProfitTrigger
        private int atProfitTrigger3 = 0; // Auto Trail AtProfitTrigger

		private int atStopLoss1 = 30; // Auto Trail AtStopLoss		
        private int atStopLoss2 = 1; // Auto Trail AtStopLoss        
        private int atStopLoss3 = 1; // Auto Trail AtStopLoss
        
		private int profitTarget = 109; // Closes once reached, if 0 will be ignored ProfitTarget
        private int qty = 1; // Order Qty
        private int stopLoss = 10; // StopLoss

        // User defined variables (add any user defined variables below)
		double high = 0;
		double low = 0;
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (BarsPeriod.Id != PeriodType.Minute) 
				return;

			if (Bars.BarsSinceSession == 1)
			{
				high = 0;
				low = 0;
			}
			
			if (Position.MarketPosition == MarketPosition.Long || Position.MarketPosition == MarketPosition.Short)
			{
				high = 0;
				low = 0;
				AtmStrategyHandler();
			}
			
			if (ToTime(Time[0]) >= ToTime(hour, minute, 0)
				&& ToTime(Time[0]) < ToTime(hour, minute + BarsPeriod.Value, 0))
			{
				high = High[HighestBar(High, period)];
				low = Low[LowestBar(Low, period)];
				BackColor = Color.Beige;
				//Print(Time + " high: " + high + " low: " + low);
				//DrawDot(CurrentBar + "high", false, 0, high, Color.Blue);
				//DrawDot(CurrentBar + "low", false, 0, low, Color.Red);
				DrawHorizontalLine("high", high, Color.Blue);
				DrawHorizontalLine("low", low, Color.Red);
			}
			
			if (high != 0 && low != 0) {
				if (Close[0] > Open[0]) {
					//Print(Time + " LongStop: " + Math.Max(high + 1*TickSize, Close[0] + 1*TickSize));
					if (Close[0] >= high) 
					{
						EnterLong();
					} else {
						EnterLongStop(Math.Max(high + 1*TickSize, Close[0] + 1*TickSize));
					}
					ExitLongStop(Close[0] - stopLoss * TickSize);
				} else {
					if (Close[0] <= low) 
					{
						EnterShort();
					} else {
						EnterShortStop(Math.Min(low - 1*TickSize, Close[0] - 1*TickSize));
					}
					ExitShortStop(Close[0] + stopLoss * TickSize);
				}
			}
			
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
				if (AbeProfitTrigger > 0 && highSinceOpen >= approxEntry + AbeProfitTrigger * TickSize)
					stop = approxEntry;

				//private int atFrequency1 = 1; // Auto Trail update Frequency
				//private int atProfitTrigger1 = 1; // Auto Trail AtProfitTrigger
				//private int atStopLoss1 = 1; // Auto Trail AtStopLoss
				
				// Trailing Stop
				if (AtProfitTrigger1 > 0 && highSinceOpen >= approxEntry + AtProfitTrigger1 * TickSize) 
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
				if (AbeProfitTrigger > 0 && lowSinceOpen <= approxEntry - AbeProfitTrigger * TickSize)
					stop = approxEntry;

				// Trailing Stop
				if (AtProfitTrigger1 > 0 && lowSinceOpen <= approxEntry - AtProfitTrigger1 * TickSize) 
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
		
        [Description("The number of bars to include before opening pit session to build the range")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }
		
        [Description("Hour pit opens for this instrument")]
        [GridCategory("Parameters")]
        public int Hour
        {
            get { return hour; }
            set { hour = Math.Max(0, value); }
        }
		
        [Description("Minute pit opens for this instrument")]
        [GridCategory("Parameters")]
        public int Minute
        {
            get { return minute; }
            set { minute = Math.Max(0, value); }
        }
		
        [Description("Reverse the trade if it hits the opposite limit")]
        [GridCategory("Parameters")]
        public bool AllowReversal
        {
            get { return allowReversal; }
            set { allowReversal = value; }
        }
		
		// ===========================================================
		// Begin ATM Stop Strategy Settings
		// ===========================================================
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
		// ===========================================================
		// End ATM Stop Strategy Settings
		// ===========================================================
		
        #endregion
    }
}
