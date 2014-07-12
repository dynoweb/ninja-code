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
    /// Strategy which can be used to experiment ideas
    /// </summary>
    [Description("Strategy which can be used to experiment ideas")]
    public class TestStrategy : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int myInput0 = 1; // Default setting for MyInput0

		double[,] dowGain = new double[50,8];
//		= {{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
//							{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
//							{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
//							{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
//							{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
//							{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
//							{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}};
		
		int day = 99; // trade day of the month
		int month = 99; // current month being traded
		int year = 9999;
		int startYear = 2008;
		#endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			SetStopLoss(1800.00);
			ExitOnClose = true;
			BarsRequired = 4;

			CalculateOnBarClose = false;
        }
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// find the gain based on day of the month
			/*
			if (month != Time[0].Month)
			{
				if (year != Time[0].Year)
				{
					year = Time[0].Year - startYear;
				}
				month = Time[0].Month;
				day = 0;
			}
			dowGain[day, year] += Close[0] - Open[0];
			day++;
			*/
			
			// find the gain based on day of week
			/*
			if (year != Time[0].Year)
			{
				year = Time[0].Year - startYear;
			}
			day = (int) Time[0].DayOfWeek;
			dowGain[day, year] += Close[0] - Open[0];
			*/
			
			// find the gain based on the prior day being down
			if (year != Time[0].Year)
			{
				year = Time[0].Year - startYear;
			}
			day = (int) Time[0].DayOfWeek;

			if (Close[2] < Close[3])
			{
				dowGain[day, year] += (Open[0] - Open[1]);
			}
			
			
			
			
			// Print Report
			DateTime exp = new DateTime(2014, 2, 28);
			if (ToDay(Time[0]) == ToDay(exp))
			//if (Time[0].Year == 2014)
			{
				Print("Trying to print report");
				for (int i = 0; i < 10; i++) 
				{
					string s = i.ToString("00");
					double dayTotal = 0;
					for (int y = 0; y < 2014-startYear; y++) 
					{
						//s +=  ", " + String.Format("{0,10:0.0}", dowGain[i,y]*1000);
						s +=  ", " + (dowGain[i,y]*1000).ToString("00");
						dayTotal += dowGain[i,y]*1000;
					}
					Print(s + ",  " + dayTotal.ToString("00"));
				}
			}
			
			
			// If it's Friday, do not trade.
		    //if (Time[0].DayOfWeek == DayOfWeek.Friday)
        	//	return;
			
			// Start 1/1/2012 end 11/1/2013 - CL
			//11/1/2013 4:15:00 PM 3.46, 4.61, 4.40, 7.48, -0.5, 0
			//if (Close[1] < Open[1])
			//11/1/2013 4:15:00 PM 4.24, 3.23, 7, 6.80, 3.98, 0
			//if (Close[1] < Close[2])
			//11/1/2013 4:15:00 PM 4.26, -3.52, -5.08, 0.14, 6.13, 0
			//if (Close[1] > Open[1])
			//11/1/2013 4:15:00 PM 3.48, -2.14, -7.68, 0.69, 1.65, 0
			//if (Close[1] > Close[2])
			//11/1/2013 4:15:00 PM -3.32, -6.02, -13.47, 1.22, 1.79, 0.00
//			if (Close[1] > Close[2])
//			{
//				double diff = Close[0] - Open[0];
//				switch (Time[0].DayOfWeek) {
//					case DayOfWeek.Monday:
//						dowGain[0] += diff;
//						break;
//					case DayOfWeek.Tuesday:
//						dowGain[1] += diff;
//						break;
//					case DayOfWeek.Wednesday:
//						dowGain[2] += diff;
//						break;
//					case DayOfWeek.Thursday:
//						dowGain[3] += diff;
//						break;
//					case DayOfWeek.Friday:
//						dowGain[4] += diff;
//						break;
//					default:
//						dowGain[5] += diff;
//						break;
//				}
//			}
//			
//			if (Time[0].Day == 1)
//			{
//				Print(Time + " " + dowGain[0].ToString("N2") + ", " + dowGain[1].ToString("N2") 
//					+ ", " + dowGain[2].ToString("N2") + ", " + dowGain[3].ToString("N2") 
//					+ ", " + dowGain[4].ToString("N2") + ", " + dowGain[5].ToString("N2"));
//				
//			}
			
//			if (Position.MarketPosition == MarketPosition.Long)
//				ExitLong();
//			if (Position.MarketPosition == MarketPosition.Short)
//				ExitShort();
//
//			if (Close[0] > Open[0])
//				upBars++;
//			
//			if (Close[1] > Open[1]) 
//			{
//				if (Close[0] > Open[0])
//					nextOneUp++;
//				//EnterShort();
//			} 
//			if (Close[0] < Open[0])
//				downBars++;
//			
//			if (Close[1] < Open[1]) 
//			{
//				if (Close[0] < Open[0])
//					nextOneDown++;
//				//EnterShort();
//			} 
//			//if (Close[0] < Open[0]) 
//			//
//			//if (Time[0].DayOfWeek != DayOfWeek.Wednesday)
//				//EnterLong();
////			if (Time[0].DayOfWeek == DayOfWeek.Sunday || Time[0].DayOfWeek == DayOfWeek.Thursday)
////				EnterShort();
//			//}
//			Print(CurrentBar + " upBars: " + upBars + " downBars: " + downBars + " -- " + nextOneUp + " " + nextOneDown);
        }

		#region
        #endregion
    }
}
