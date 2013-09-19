// 
// MADisplaced
// Written by Mike Winfrey for participation in the BigMikeTrading Battle of the BOTS.
// Not to be shared outside of the BMT Elite membership without permission from me.  
//
// mike.winfrey@gmail.com or skype id is mikwinf
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
using NinjaTrader.Gui.Chart;
using NinjaTrader.Strategy;
using System.Collections;
#endregion

namespace NinjaTrader.Strategy
{
    [Description("Trades CL using BetterRenko 4 bars")]
    public class BKMADisplacementBMT : Strategy
    {
        #region Variables
        private int starttime = 80000;
        private int endtime = 110000;
		private int entrydisplacement = 6;
        private int period = 45;
        private int smooth = 34;
        private int period1 = 35;
        private int smooth1 = 29;
		private int direction = 0;
		private int trailstoptrigger = 31;
		private int stoploss = 31;
		private int numcontracts = 1;
		private double entrypricelong = 0;
		private double entrypriceshort = 0;
		private double stoppricelong = 0;
		private int entrybarlong = 0;
		private double stoppriceshort = 0;
		private int entrybarshort = 0;
		
		private int previousPNL = 0;
		
		private DataSeries ma;
		
		//BK Variable adds
		private int sessioncount = 0;
		//Array n stuff to calculate 7 day high low values
		private double[] d_high = new double[8];
		private double[] d_low = new double[8];
		private double d_high_7 = 0;
		private double d_low_7 = 999;

		private	Font textFont = new Font("Arial",10,FontStyle.Bold);
        #endregion

        protected override void Initialize()
        {
			ma = new DataSeries(this);
			
			// RC add **************************
			Add(EMA(SMA(Median,period),smooth));
			EMA(SMA(Median,period),smooth).Displacement = entrydisplacement;
			EMA(SMA(Median,period),smooth).Plots[0].Pen.Color = Color.Blue;
			Add(EMA(SMA(Median,period),smooth+1));
			EMA(SMA(Median,period),smooth+1).Plots[0].Pen.Color = Color.Green;
			Add(HallMaColored(300));
			// RC add end ***********************
			
            CalculateOnBarClose = true;
			ExitOnClose = true;
		//	TraceOrders = true;
        }

		protected override void OnStartUp()
		{
			if (!CalculateOnBarClose) CalculateOnBarClose = true;
		}

        protected override void OnBarUpdate()
        {
			//BK add ****************************
			if (Bars.FirstBarOfSession && FirstTickOfBar) sessioncount = sessioncount + 1;
			if (sessioncount < 8) return; // Need 7 days of bars
			
			
			if (Bars.FirstBarOfSession) 
			{	
				//Build Array for 7 days back data
				for (int i = 1; i < 8; i++)
				{
					// Get Day data for previous days 1-7
					if (Bars.GetDayBar(i) != null)
					{
						d_high[i] = Bars.GetDayBar(i).High;
						d_low[i] = Bars.GetDayBar(i).Low;
					}
				}				
			}	
			
			//*******************************************************
			// Get 7 Day High and Low
			//*******************************************************
			d_high_7 = 0;
			d_low_7 =999;
			for (int i = 1; i< 8; i++) // 	
			{
				if (d_high[i] > d_high_7)
					d_high_7 = d_high[i];
				if (d_low[i] < d_low_7)
				{
					d_low_7 = d_low[i];
				}
			}
			if (CurrentDayOHL().CurrentHigh[0] > d_high_7)
				d_high_7 = CurrentDayOHL().CurrentHigh[0];
			if (CurrentDayOHL().CurrentLow[0] < d_low_7)
				d_low_7 = CurrentDayOHL().CurrentLow[0];
			
			//*************************************
			
			ma.Set(EMA(SMA(Median,period),smooth)[0]);
			
			if (CurrentBar > 0 &&
				Volume[0] == 0 || Volume[1] == 0) return;

			//**************
			// Long Orders
			//**************
			if (ToTime(Time[0]) > starttime && 
				ToTime(Time[0]) < endtime &&
				Position.MarketPosition != MarketPosition.Long &&
				ma[0] > ma[entrydisplacement] &&
				ma[1] <= ma[entrydisplacement+1]
				// BK Add
				&& CurrentDayOHL().CurrentLow[0] > d_low_7)
			{
				EnterLong(numcontracts,"Long");
				if (Close[0] < Open[0])
				//	SetStopLoss("Long",CalculationMode.Price,Close[0]-((stoploss-4)*TickSize),false);
					stoppricelong = Close[0]-((stoploss-4)*TickSize);
				else
				//	SetStopLoss("Long",CalculationMode.Price,Close[0]-(stoploss*TickSize),false);
					stoppricelong = Close[0]-(stoploss*TickSize);
				entrypricelong = Close[0];
				entrybarlong = CurrentBar;
			}
			
			//*****************
			// Short Orders
			//*****************
			else if ((direction == 0 || direction == -1) &&
				ToTime(Time[0]) > starttime && 
				ToTime(Time[0]) < endtime &&
				Position.MarketPosition != MarketPosition.Short &&
				ma[0] < ma[entrydisplacement] &&
				ma[1] >= ma[entrydisplacement+1]
				// BK Add
				&& CurrentDayOHL().CurrentHigh[0] < d_high_7)
			{
				EnterShort(numcontracts,"Short");
				if (Close[0] > Open[0])
				//	SetStopLoss("Short",CalculationMode.Price,Close[0]+((stoploss-4)*TickSize),false);
					stoppriceshort = Close[0]+((stoploss-4)*TickSize);
				else 
				//	SetStopLoss("Short",CalculationMode.Price,Close[0]+(stoploss*TickSize),false);	
					stoppriceshort = Close[0]+(stoploss*TickSize);
				entrypriceshort = Close[0];
				entrybarshort = CurrentBar;
			}
			
			//******************
			// Long Exits
			//******************
			else if (Position.MarketPosition == MarketPosition.Long)
			{
				DrawLine("entry",true,CurrentBar-entrybarlong+3,entrypricelong,0,entrypricelong,Color.Black,DashStyle.Solid,1);
				DrawLine("stop",true,CurrentBar-entrybarlong+3,stoppricelong,0,stoppricelong,Color.Black,DashStyle.Solid,1);
				DrawTextFixed("positionlong","Current PNL: " + RoundPrice((Close[0] - entrypricelong)/TickSize),TextPosition.Center,Color.White,textFont,Color.Black,Color.Black,10);

				if (ma[0] < ma[entrydisplacement] ||
				Close[0] <= stoppricelong)		
				{
					ExitLong("Long");
				//	DrawTriangleUp("long"+CurrentBar,true,0,Channel(43).Lower[1],Color.Black);
				}
			}
			
			//****************
			// Short Exits
			//****************
			else if (Position.MarketPosition == MarketPosition.Short)
			{
				DrawLine("entry",true,CurrentBar-entrybarshort+3,entrypriceshort,0,entrypriceshort,Color.Black,DashStyle.Solid,1);
				DrawLine("stop",true,CurrentBar-entrybarshort+3,stoppriceshort,0,stoppriceshort,Color.Black,DashStyle.Solid,1);
				DrawTextFixed("positionshort","Current PNL: " + RoundPrice((entrypriceshort - Close[0])/TickSize),TextPosition.Center,Color.White,textFont,Color.Black,Color.Black,10);
			
				if (ma[0] > ma[entrydisplacement] ||
				Close[0] >= stoppriceshort)
				{
					ExitShort("Short");
				}
			}
			
			//**************************
			// Remove Drawing Objects
			//**************************
			if (Position.MarketPosition != MarketPosition.Long) 
				RemoveDrawObject("positionlong");
			if (Position.MarketPosition != MarketPosition.Short)
				RemoveDrawObject("positionshort");

			if (Position.MarketPosition == MarketPosition.Flat) 
			{
				RemoveDrawObject("entry");
				RemoveDrawObject("stop");
			}

			//***********************************
			// Plot PnL/ Stats ... on Chart
			//***********************************
		//	if ((int) Performance.AllTrades.TradesPerformance.Currency.CumProfit != previousPNL)
		//		Print(Time[0] + " " + Performance.LongTrades.TradesPerformance.Currency.CumProfit.ToString("0") + " + " + Performance.ShortTrades.TradesPerformance.Currency.CumProfit.ToString("0") + " = " + Performance.AllTrades.TradesPerformance.Currency.CumProfit.ToString("0"));
			
			DrawTextFixed("pnl","All: " + Performance.AllTrades.TradesCount + " W:" + (Performance.AllTrades.WinningTrades.Count) + " L: " + (Performance.AllTrades.LosingTrades.Count) +
				" PNL: $" + (int) Performance.AllTrades.TradesPerformance.Currency.CumProfit +
				"\nLongs: " + Performance.LongTrades.TradesCount + " W: " + (Performance.LongTrades.WinningTrades.Count) + " L: " + (Performance.LongTrades.LosingTrades.Count) +
				" PNL: $" + (int) Performance.LongTrades.TradesPerformance.Currency.CumProfit +
				"\nShorts: " + Performance.ShortTrades.TradesCount + " W: " + (Performance.ShortTrades.WinningTrades.Count) + " L: " + (Performance.ShortTrades.LosingTrades.Count) +
				" PNL: $" + (int) Performance.ShortTrades.TradesPerformance.Currency.CumProfit +
				"\nAccount: " + this.Account.Name + 
				"\nAccountBalance: " + this.GetAccountValue(AccountItem.CashValue).ToString("0")
				,TextPosition.TopLeft,Color.White,textFont,Color.Black,Color.Black,10);
			
			previousPNL = (int) Performance.AllTrades.TradesPerformance.Currency.CumProfit;
        }

 		private string FormatPrice(double value)
        {
            return Bars.Instrument.MasterInstrument.FormatPrice(value);
        }

        private double RoundPrice(double value)
        {
            return Bars.Instrument.MasterInstrument.Round2TickSize(value);
        }

       #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int StartTime
        {
            get { return starttime; }
            set { starttime = value; }
        }
        [Description("")]
        [GridCategory("Parameters")]
        public int EndTime
        {
            get { return endtime; }
            set { endtime = value; }
        }
        [Description("")]
        [GridCategory("Parameters")]
        public int Stoploss
        {
            get { return stoploss; }
            set { stoploss = value; }
        }
        [Description("")]
        [GridCategory("Parameters")]
        public int NumContracts
        {
            get { return numcontracts; }
            set { numcontracts = value; }
        }
        [Description("")]
        [GridCategory("Parameters")]
        public int Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        [Description("0. Period")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }
        [Description("0. Smooth")]
        [GridCategory("Parameters")]
        public int Smooth
        {
            get { return smooth; }
            set { smooth = Math.Max(1, value); }
        }
        #endregion
    }
}
