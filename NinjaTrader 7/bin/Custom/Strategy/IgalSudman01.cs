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
    /// CL 3 min bars
	/// 
	/// 1. ATR for stop loss rounded up even if its 7.1 we go to 8
	/// 2. profit target % of atr rounded down , even if its 7.9 we go to 7 but we are able to change the target to anyting 75% to 110%
	/// 3. BE is a % of profit rounded down can be 50 or 90 before we go to BE
	/// 4. another BE PLUS is when we get a certain % of profit 70-80-90 whatever we put to go to BE plus ??
	/// 5. trail stop when we get to % profit we trail by ( parameter input ) ex. if we get to 80-90% of profit target for every tick up we trail by 1 tick or for every 2 ticks we trail by 1
	/// 6. we may input the profit target to be 150% in order to let the trail work beacuse if we are right we may only lose 1 tick on the pullback fter hitting 80% of profit but have the opportunity to trail 3-4-5 ticks up which makes a difference on the profit PNL but does not really change what we give up ( at most 1-2 ticks )
	/// 7. paprameter for time in market , we will not trade during news or when market closes for ex in europe
	/// 
    /// </summary>
    [Description("CL 1 min bars")]
    public class IgalSudman01 : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int myInput0 = 1; // Default setting for MyInput0
		
		double channelHigh = 0;
		double channelLow = 0;
		double channelSize = 0;
		
		int startTime = 80000;	// 8 AM
		
		double upperTrigger = 0;
		double lowerTrigger = 0;
		double upperStopLoss = 0;
		double lowerStopLoss = 0;
		double atr = 0;

        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// reset variables at the start of each day
			if (Bars.BarsSinceSession == 1)
			{
				channelHigh = 0;
				channelLow = 0;
				channelSize = 0;
				upperTrigger = 0;
				lowerTrigger = 0;
			}
			
			if (channelSize == 0 && ToTime(Time[0]) == startTime + 3000)
			{
				EstablishOpeningChannel();
			}

			// Open pit session trading only
			if (ToTime(Time[0]) < startTime || ToTime(Time[0]) > 150000)
			{
				return;
			}
			
			if (channelSize != 0)
			{
				if (Time[0].Minute % 5 == 0)
				{
					if (isFlat())
					{
						atr = CalcAtr();
						Print(Time + " atr: " + atr + " getTicks: " + Instrument.MasterInstrument.GetTicks(atr));
						upperTrigger = MAX(High, 5)[0] + atr;
						lowerTrigger = MIN(Low, 5)[0] - atr;
						
						if (upperTrigger < channelLow || upperTrigger > channelHigh)
						{
							upperTrigger += atr*0.5;
						}
						if (lowerTrigger < channelLow || lowerTrigger > channelHigh)
						{
							lowerTrigger -= atr*0.5;
						}
						
						DrawLine(CurrentBar + "upperTrigger", 0, upperTrigger, -5, upperTrigger, Color.Blue);
						DrawLine(CurrentBar + "lowerTrigger", 0, lowerTrigger, -5, lowerTrigger, Color.Blue);
						
						upperStopLoss = upperTrigger + atr;
						lowerStopLoss = lowerTrigger - atr;
						
						DrawLine(CurrentBar + "upperStopLoss", 0, upperStopLoss, -5, upperStopLoss, Color.Red);
						DrawLine(CurrentBar + "lowerStopLoss", 0, lowerStopLoss, -5, lowerStopLoss, Color.Red);
					}
					else
					{
						lowerTrigger = 0;
						upperTrigger = 0;
					}
				}
			}
			
			if (isLong())
			{
				SetProfitTarget(CalculationMode.Ticks, Instrument.MasterInstrument.GetTicks(atr*0.75));
				SetStopLoss(CalculationMode.Price, lowerStopLoss);
				lowerTrigger = 0;
				upperTrigger = 0;
			}
			
			if (isShort())
			{
				SetProfitTarget(CalculationMode.Ticks, Instrument.MasterInstrument.GetTicks(atr*0.75));
				SetStopLoss(CalculationMode.Price, upperStopLoss);
				lowerTrigger = 0;
				upperTrigger = 0;
			}
			
			if (isFlat() && lowerTrigger != 0)
			{
				EnterLongLimit(lowerTrigger);
				SetProfitTarget(CalculationMode.Ticks, 200);
				SetStopLoss(CalculationMode.Ticks, 200);
				double target = lowerTrigger + atr*0.75;
				DrawLine(CurrentBar+"longTarget", 0, target, -5, target, Color.Green);
			}
			
			if (isFlat() && upperTrigger != 0)
			{
				//EnterShortLimit(upperTrigger);
				SetProfitTarget(CalculationMode.Ticks, 200);
				SetStopLoss(CalculationMode.Ticks, 200);
				double target = upperTrigger - atr*0.75;
				DrawLine(CurrentBar+"shortTarget", 0, target, -5, target, Color.Green);
			}
			
			if (isLong() && BarsSinceEntry() > 6) 
			{
				ExitLong();
			}
			
			if (isShort() && BarsSinceEntry() > 6) 
			{
				ExitShort();
			}
			
			
        }
		
		private void EstablishOpeningChannel()
		{
			channelHigh = MAX(High, 30)[0];
			channelLow = MIN(Low, 30)[0];
			channelSize = channelHigh - channelLow;
			DrawRectangle(CurrentBar + "channel", false, 30, channelHigh, -30, channelLow, Color.SeaGreen, Color.SeaGreen, 1);
		}
		
		private double CalcAtr()
		{
			double _atr = 0;
			for (int i = 0; i < 10; i++) 
			{
				_atr += ATR(10)[i];
			}
			for (int i = 0; i < 5; i++) 
			{
				_atr += ATR(5)[i];
			}
			return Instrument.MasterInstrument.Round2TickSize(_atr/15);
		}
		
		private bool isFlat()
		{
			if (Position.MarketPosition == MarketPosition.Flat)
				return true;
			else
				return false;
		}
		
		private bool isLong()
		{
			if (Position.MarketPosition == MarketPosition.Long)
				return true;
			else
				return false;
		}
		
		private bool isShort()
		{
			if (Position.MarketPosition == MarketPosition.Short)
				return true;
			else
				return false;
		}		

        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int MyInput0
        {
            get { return myInput0; }
            set { myInput0 = Math.Max(1, value); }
        }
        #endregion
    }
}
