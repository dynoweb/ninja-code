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
    /// Tunning setings
	/// ES 5 min - 7/1/12 -> 9/13/12
	/// t, f, 128, 28, 132, 17, 9, 13, 12 %5,760.00 MDD $4,025
	/// 
	/// 
    /// </summary>
    [Description("This came from a demo on BigMikes website")]
    public class BigMikesDemo : Strategy
    {
		bool be2 = true;
		bool be3 = false;		
		
		int emalength = 128;
		int hmalength = 28;
		int smalength = 132;
		
		int stop = 17;

		int target1 = 9;
		int target2 = 13;
		int target3 = 12;
		
		
        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			Add(EMA(emalength));
			Add(HMA(emalength));
			Add(SMA(emalength));
            CalculateOnBarClose = true;
			EntryHandling = EntryHandling.UniqueEntries;		
        }
		
		private void GoLong() 
		{
			SetStopLoss("target1", CalculationMode.Price, Close[0] - (Stop*TickSize), false);
			SetStopLoss("target2", CalculationMode.Price, Close[0] - (Stop*TickSize), false);
			SetStopLoss("target3", CalculationMode.Price, Close[0] - (Stop*TickSize), false);
			
			SetProfitTarget("target1", CalculationMode.Price, Close[0] + (Target1*TickSize));
			SetProfitTarget("target2", CalculationMode.Price, Close[0] + ((Target1+Target2)*TickSize));
			SetProfitTarget("target3", CalculationMode.Price, Close[0] + ((Target1+Target2+Target3)*TickSize));
			
			EnterLong("target1");
			EnterLong("target2");
			EnterLong("target3");
		}

		private void GoShort() 
		{
			SetStopLoss("target1", CalculationMode.Price, Close[0] + (Stop*TickSize), false);
			SetStopLoss("target2", CalculationMode.Price, Close[0] + (Stop*TickSize), false);
			SetStopLoss("target3", CalculationMode.Price, Close[0] + (Stop*TickSize), false);
			
			SetProfitTarget("target1", CalculationMode.Price, Close[0] - (Target1*TickSize));
			SetProfitTarget("target2", CalculationMode.Price, Close[0] - ((Target1+Target2)*TickSize));
			SetProfitTarget("target3", CalculationMode.Price, Close[0] - ((Target1+Target2+Target3)*TickSize));
			
			EnterShort("target1");
			EnterShort("target2");
			EnterShort("target3");
		}
		
		private void ManageOrder() 
		{
			if (Position.MarketPosition == MarketPosition.Long)
			{
				if (BE2 && High[0] > Position.AvgPrice + (Target1*TickSize))
					SetStopLoss("target2", CalculationMode.Price, Position.AvgPrice, false);
					
				if (BE3 && High[0] > Position.AvgPrice + (Target1*TickSize))
					SetStopLoss("target3", CalculationMode.Price, Position.AvgPrice, false);
			}
			
			if (Position.MarketPosition == MarketPosition.Short)
			{
				if (BE2 && High[0] < Position.AvgPrice - (Target1*TickSize))
					SetStopLoss("target2", CalculationMode.Price, Position.AvgPrice, false);
					
				if (BE3 && High[0] < Position.AvgPrice - (Target1*TickSize))
					SetStopLoss("target3", CalculationMode.Price, Position.AvgPrice, false);
			}
		}

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			EntryHandling = EntryHandling.UniqueEntries;
			
			SMA smav = SMA(SMAlength);
			EMA emav = EMA(EMAlength);
			HMA hmav = HMA(HMAlength);
			
			ManageOrder();
			
			if (Position.MarketPosition != MarketPosition.Flat) return;
			
			if (Rising(smav) && Rising(emav) && Rising(hmav))
				GoLong();
			else if (Falling(smav) && Falling(emav) && Falling(hmav))
				GoShort();
        }

        #region Properties

		[GridCategory("Parameters")]
		public int SMAlength
        {
            get { return smalength; }
            set { smalength = Math.Max(1, value); }
        }
		
        [GridCategory("Parameters")]
		public int EMAlength
        {
            get { return emalength; }
            set { emalength = Math.Max(1, value); }
        }
		
        [GridCategory("Parameters")]
		public int HMAlength
        {
            get { return hmalength; }
            set { hmalength = Math.Max(1, value); }
        }
		
        [GridCategory("Parameters")]
		public int Target1
        {
            get { return target1; }
            set { target1 = Math.Max(1, value); }
        }
		
        [GridCategory("Parameters")]
		public int Target2
        {
            get { return target2; }
            set { target2 = Math.Max(1, value); }
        }
		
        [GridCategory("Parameters")]
		public int Target3
        {
            get { return target3; }
            set { target3 = Math.Max(1, value); }
        }
		
        [GridCategory("Parameters")]
		public int Stop
        {
            get { return stop; }
            set { stop = Math.Max(1, value); }
        }
		
        [GridCategory("Parameters")]
		public bool BE2
        {
            get { return be2; }
            set { be2 = value; }
        }
		
        [GridCategory("Parameters")]
		public bool BE3
        {
            get { return be3; }
            set { be3 = value; }
        }
		
        #endregion
    }
}
