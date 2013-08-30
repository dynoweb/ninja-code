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

// Copyright Big Mike Trading
// BMT Elite Only
// August 4 2012 v1.0 alpha
// August 7 2012 v1.01 alpha
// August 12 2012 changes made my Mike Winfrey
// 		added on chart PNL report
//		

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Enter the description of your strategy here
    /// </summary>
    [Description("August 4 Divergence test Big Mike Trading")]
    public class Div1v1 : Strategy
    {
		
		private int							divavgsupport		= 0;
		private int							divmatype			= 0;
		private int							divmaxbars 			= 100;
		private int							divminbars 			= 25;		
		private int							divperiod			= 30;
		private int							divsmooth			= 15;
		private int							divthresh 			= 50;
		
		private int							epd					= 10;
		
		private int							target1				= 10;
		private int							target2				= 24;
		private int							target3				= 32;
		
		private int							stop				= 48;
		private int							stopperiod			= 45;
		
		
		private DataSeries					Div;

		private	Font textFont = new Font("Arial",10,FontStyle.Bold);

        protected override void Initialize()
        {
			Add(mRSIv1(DivAvgSupport, DivMaxBars, DivMinBars, DivThresh, DIVMAType, DivPeriod, DivSmooth));
		
			IncludeCommission 	= true;			// Helps be realistic
			Slippage 			= 1;			// Market entry orders
			ExitOnClose 		= false;

			CalculateOnBarClose = true;
			Enabled				= true;
			
			
    		EntryHandling 		= EntryHandling.AllEntries;
			
			Div					= new DataSeries(this);
			
        }
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			EntriesPerDirection = EPD; 
			
			Div.Set(mRSIv1(DivAvgSupport, DivMaxBars, DivMinBars, DivThresh, DIVMAType, DivPeriod, DivSmooth).Div[0]);
			
			TimeSpan tsdiff = Time[0] - Time[1];
			int diff = (int) tsdiff.TotalMilliseconds;
			
			// MWinfrey commented the checking of fast bars because the modifications I made are
			// to accomodate use of this strategy against stocks, etfs, and funds.
		//	if (diff < 2000) return;  // skip bars if coming fast (news)
					
			/*if ((Time[0].Hour == 7 && Time[0].Minute > 58) || Time[0].Hour == 8)
			{
			
				if (Position.MarketPosition == MarketPosition.Long) ExitLong();
				if (Position.MarketPosition == MarketPosition.Short) ExitShort();
				return;
				
			}
			
			if ((Time[0].Hour == 13 && Time[0].Minute > 58) || Time[0].Hour == 14 || Time[0].Hour == 15)
			{
			
				if (Position.MarketPosition == MarketPosition.Long) ExitLong();
				if (Position.MarketPosition == MarketPosition.Short) ExitShort();
				return;
				
			}*/
			
			try
			{
			
			// MWinfrey added to show where the average price is when in a position.
			if (Position.AvgPrice != 0 && 
				Position.MarketPosition == MarketPosition.Long)
			{
				DrawTriangleDown("pt " + CurrentBar, true, Time[0], Position.AvgPrice, Color.White);
			}
			
			// MWinfrey commented the checking of the short position because it's unnecessary when only going long.
			if (Div[0] == 1) //&& Position.MarketPosition != MarketPosition.Short)
			{
				
				// MWinfrey commented the additional checking here because it's unnecessary when only going long.
				if (Position.Quantity <= EntriesPerDirection)// && 
			//		(Position.MarketPosition == MarketPosition.Flat || 
				//	Low[0] < Position.AvgPrice))
			//		Close[0] < Position.AvgPrice))
				{
					int m_FillQty = Position.Quantity + 1;
					double m_StopPrice = MIN(Low, Stopperiod)[0] - (Stop * TickSize);
					double m_TargetPrice = 0.0;
					
					switch (m_FillQty)
					{
						case 0: m_TargetPrice = Close[0] + (Target1 * TickSize); break;
						case 1: m_TargetPrice = Close[0] + (Target1 + Target2) * TickSize; break;
						default: m_TargetPrice = Close[0] + (Target1 + Target2 + Target3) * TickSize; break;
						
					}
						
					EnterLong(1, "Long " + m_FillQty);
				//	DrawSquare("pt " + CurrentBar, true, Time[0], Position.AvgPrice, Color.White);
					DrawArrowUp("up " + CurrentBar, 0, Low[0], Color.Lime);
					string avgprice = FormatPrice(Position.AvgPrice);
			
			// MWinfrey removed stops and targets because they are really unnecessary for trading stocks, 
				//	SetProfitTarget("Long " + m_FillQty, CalculationMode.Price, ((m_TargetPrice - Close[0]) / TickSize >= 10 ? m_TargetPrice : Close[0] + (10 * TickSize)));
				//	SetStopLoss("Long " + m_FillQty, CalculationMode.Price, m_StopPrice, false);
				}
				
			} 
			// MWinfrey commedted the checking of a long postition...not necessary
			if (Div[0] == -1)//&& Position.MarketPosition != MarketPosition.Long)
			{
			//	DrawText("x"+CurrentBar,Position.Quantity.ToString("0 ") + FormatPrice(Close[0]) + " " + FormatPrice(Position.AvgPrice),0,High[0]+(2*TickSize),Color.Black);
				
				if (Position.Quantity <= EntriesPerDirection && 
					(Position.MarketPosition == MarketPosition.Flat || 
				//	High[0] > Position.AvgPrice))
					Close[0] > Position.AvgPrice))
				{
					int m_FillQty = Position.Quantity + 1;
					double m_StopPrice = MAX(High, Stopperiod)[0] + (Stop * TickSize);
					double m_TargetPrice = 0.0;
					
					switch (m_FillQty)
					{
						case 0: m_TargetPrice = Close[0] - (Target1 * TickSize); break;
						case 1: m_TargetPrice = Close[0] - (Target1 + Target2) * TickSize; break;
						default: m_TargetPrice = Close[0] - (Target1 + Target2 + Target3) * TickSize; break;
						
					}
						
				//	DrawSquare("pt " + CurrentBar, true, Time[0], m_TargetPrice, Color.Red);
					DrawArrowDown("down " + CurrentBar, 0, High[0], Color.Red);
			
					//  MWinfrey this is where longs are exited.  commented the enter short because i'm only looking for longs.
					ExitLong();
				//	EnterShort(1, "Short " + m_FillQty);
				//	SetProfitTarget("Short " + m_FillQty, CalculationMode.Price, ((Close[0] - m_TargetPrice) / TickSize >= 10 ? m_TargetPrice : Close[0] - (10 * TickSize)));
				//	SetStopLoss("Short " + m_FillQty, CalculationMode.Price, m_StopPrice, false);
				}

			}
			
			}
			catch {}
			
			// MWinfrey added code to display PNL stats on the chart
			DrawTextFixed("pnl","All: " + Performance.AllTrades.TradesCount + " W:" + (Performance.AllTrades.WinningTrades.Count) + " L: " + (Performance.AllTrades.LosingTrades.Count) +
				" PNL: $" + (int) Performance.AllTrades.TradesPerformance.Currency.CumProfit +
				"\nLongs: " + Performance.LongTrades.TradesCount + " W: " + (Performance.LongTrades.WinningTrades.Count) + " L: " + (Performance.LongTrades.LosingTrades.Count) +
				" PNL: $" + (int) Performance.LongTrades.TradesPerformance.Currency.CumProfit +
				"\nShorts: " + Performance.ShortTrades.TradesCount + " W: " + (Performance.ShortTrades.WinningTrades.Count) + " L: " + (Performance.ShortTrades.LosingTrades.Count) +
				" PNL: $" + (int) Performance.ShortTrades.TradesPerformance.Currency.CumProfit
				,TextPosition.TopLeft,Color.White,textFont,Color.Black,Color.Black,10);
        }

		// added by MWinfrey
 		private string FormatPrice(double value)
        {
            return Bars.Instrument.MasterInstrument.FormatPrice(value);
        }

        private double RoundPrice(double value)
        {
            return Bars.Instrument.MasterInstrument.Round2TickSize(value);
        }

        #region Properties
        
		[Description("Avg plot in support")]
		[GridCategory("Parameters")]
		public int DivAvgSupport
		{
			get { return divavgsupport; }
			set { divavgsupport = Math.Max(0, value); }
		}
		
		[Description("Numbers of bars used for calculations")]
		[GridCategory("Parameters")]
		public int DivPeriod
		{
			get { return divperiod; }
			set { divperiod = Math.Max(1, value); }
		}

		[Description("Divergence Period")]
		[GridCategory("Parameters")]
		public int DivMaxBars
		{
			get { return divmaxbars; }
			set { divmaxbars = Math.Max(1, value); }
		}
		
		[Description("Divergence Threshold")]
		[GridCategory("Parameters")]
		public int DivThresh
		{
			get { return divthresh; }
			set { divthresh = Math.Max(1, value); }
		}
		
		[Description("Divergence Min Bars")]
		[GridCategory("Parameters")]
		public int DivMinBars
		{
			get { return divminbars; }
			set { divminbars = Math.Max(1, value); }
		}
		
		[Description("Div MA Type")]
		[GridCategory("Parameters")]
		public int DIVMAType
		{
			get { return divmatype; }
			set { divmatype = Math.Max(0, value); }
		}
		
		[Description("Div Smooth")]
		[GridCategory("Parameters")]
		public int DivSmooth
		{
			get { return divsmooth; }
			set { divsmooth = Math.Max(1, value); }
		}
		
		[Description("Profit Target 01")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayName("Profit Target 01")]
        public int Target1
        {
            get { return target1; }
            set { target1 = Math.Max(1, value); }
        }
		
		[Description("Profit Target 02")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayName("Profit Target 02")]
        public int Target2
        {
            get { return target2; }
            set { target2 = Math.Max(1, value); }
        }
		
		[Description("Profit Target 03")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayName("Profit Target 03")]
        public int Target3
        {
            get { return target3; }
            set { target3 = Math.Max(1, value); }
        }
		
		[Description("Stop Loss")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayName("Stop Loss")]
        public int Stop
        {
            get { return stop; }
            set { stop = value; }
        }
		
		[Description("Stop Loss Period")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayName("Stop Loss Period")]
        public int Stopperiod
        {
            get { return stopperiod; }
            set { stopperiod = Math.Max(1, value); }
        }
		
		[Description("Entries Per Direction")]
		[GridCategory("Parameters")]
		public int EPD
		{
			get { return epd; }
			set { epd = Math.Max(1, value); }
		}
        #endregion
	
		
    }
}
