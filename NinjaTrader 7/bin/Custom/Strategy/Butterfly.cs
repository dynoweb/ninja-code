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

namespace NinjaTrader.Strategy
{
    /// <summary>
    /// This strategy is based on a TastyTrade episode which compared calendars and butterfly trades.
	/// Options expiration on Friday between 15th and the 21st
	/// Entry will be 3 weeks before expiration
	/// Assumptions without option data, cost of butterfly with $3 wide strikes will be $1.00
	/// Max profit will be $2, max loss $1. Price is incremented by $0.50 butterfly will be positioned
	/// on first strike OTM.
	/// 
	/// Tested ETFs - 5 years on daily bars
	/// IWM and EWW
	/// 
    /// </summary>
    [Description("This strategy is based on a TastyTrade episode which compared calendars and butterfly trades use daily period")]
    public class Butterfly : Strategy
    {
        #region Variables
			private int eMA1Len = 15; //50; // Default setting for Trailing Stop
			private int eMA2Len = 200; // Default setting for Trend Filter

			bool flat = true;
			double entryPrice = 0;
			double exitPrice = 0;
			int sinceEntry = 0;
			int wins = 0;
			int losses = 0;
			double profit = 0;
		    double loss = 0;
		    string ws;
			double stdDevMax = 3.5;
        #endregion		
		
        protected override void Initialize()
        {
			Add(EMA(EMA1Len));
			Add(EMA(EMA2Len));
			
			EMA(EMA1Len).Plots[0].Pen.Color = Color.DarkGray;
			EMA(EMA2Len).Plots[0].Pen.Color = Color.DarkBlue;
			
			Add(StdDev(20));

            CalculateOnBarClose = true;
        }

        protected override void OnBarUpdate()
        {
			if (CurrentBars[0] < 15)
			{
				return;
			}
			
			
			if (StdDev(20)[15] < stdDevMax)
			{
				
				if (Math.Abs(Close[15] - Close[0]) < 2.0)
				{
					wins++;
					profit = profit + 2 - Math.Abs(Close[15] - Close[0]);
					ws="WWWWW";
				}
				else
				{
					losses++;
					loss = loss + 2 - Math.Min(Math.Abs(Close[15] - Close[0]), 3);
					ws="lllll";
				}
				
				DateTime exp = new DateTime(2014, 7, 12);
				//if (ToDay(Time[0]) > ToDay(exp))
				//{

				Print(Time[15] + ", " + Close[15] + ", " +
					  Time[0] + ", " + Close[0] + ", " + 
					 (Close[15] - Close[0]) + ", " + 
					" wins: " + wins + " losses: " + losses + ", " + 
					" profit: " + profit + " loss: " + loss// + ", " +
					//" stddev: " + StdDev(20)[15]
					);
				//}
			}
			
		   // if (Time[0].DayOfWeek == DayOfWeek.Friday)
				
			//if (ToDay(Time[0]) > ToDay(exp))
			//	return;
			//if (Position.MarketPosition == MarketPosition.Flat)
			//	&& BarsSinceEntry() >= SinceEntry
			
			/*
			if (flat) 
			{
				entryPrice = Open[0];
				sinceEntry = 0;
				exitPrice = 0;
				flat = false;
			} 
			else 
			{
				sinceEntry++;

				if (sinceEntry >= 20) 
				{
					exitPrice = Close[0];
					flat = true;
					Print(Time[sinceEntry] + ", " + entryPrice + ", " +
						Time[0] + ", " + exitPrice + ", " + (entryPrice - exitPrice));
				}
			}
			*/
        }
		
		
		

        #region Properties
        [Description("Trailing Stop")]
        [GridCategory("Parameters")]
        public int EMA1Len
        {
            get { return eMA1Len; }
            set { eMA1Len = Math.Max(1, value); }
        }

        [Description("Trend Filter")]
        [GridCategory("Parameters")]
        public int EMA2Len
        {
            get { return eMA2Len; }
            set { eMA2Len = Math.Max(1, value); }
        }

        [Description("Max Standard Deviation Filter")]
        [GridCategory("Parameters")]
        public double StdDevMax
        {
            get { return stdDevMax; }
            set { stdDevMax = value; }
        }

        #endregion
    }
}
