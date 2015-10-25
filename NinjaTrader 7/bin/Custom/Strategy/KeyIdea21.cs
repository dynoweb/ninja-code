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
	/// 
	/// Buy next bar at the highest high of last X bars
	/// 
	/// 	
    /// </summary>
    [Description("Daily ZS bars - Kevin Davey's Soybean Trade")]
    public class KeyIdea21 : Strategy
    {
        #region Variables
			private int profitTarget = 200;
			private int stopLoss = 60; // Note: over the long run, moving this out to 300 gives the best return
			private int donchianPeriod = 40; // Default setting for Period1

			// DonchianChannel parms
			DonchianChannel dc = null;
			// Average True Range
			ATR atr = null;
			double multiplier = 2.125;
			
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
		/// 
        /// </summary>
        protected override void Initialize()
        {
			atr = ATR(14);
			Add(atr);

			dc = DonchianChannel(DonchianPeriod);
			dc.Displacement = 1;
			dc.PaintPriceMarkers = false;
			Add(dc);
			
//			// CL settings CST
//			Add(PitColor(Color.Blue, 80000, 15, 143000));
			
            SetProfitTarget(CalculationMode.Ticks, ProfitTarget);
            SetStopLoss(CalculationMode.Ticks, StopLoss);
            //SetTrailStop(CalculationMode.Ticks, Iparm2);

			Unmanaged = false;
			BarsRequired = 10;
            CalculateOnBarClose = true;
			ExitOnClose = false;
			IncludeCommission = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()			
        {
//			double sl = StopLoss;
//			if (multiplier != 1.0) {				
//				sl = multiplier * StopLoss * ATR(14)[0] / 50;
//			}
//            SetProfitTarget(CalculationMode.Ticks, ProfitTarget);
//            SetStopLoss(CalculationMode.Ticks, StopLoss);

			int p = DonchianPeriod;
			if (multiplier != 1.0) {
				p = Convert.ToInt32(ATR(14)[0] * multiplier);
			}
			dc = DonchianChannel(p);
			
			if (Close[0] > dc.Upper[1])
				this.EnterLong();
			else if (Close[0] < dc.Lower[1])
				this.EnterShort();
        }
		
		protected override void OnTermination()
		{
		}

		/// <summary>
		/// The OnOrderUpdate() method is called everytime an order managed by a strategy changes state. 
		/// An order will change state when a change in order quantity, price or state (working to 
		/// filled) occurs.
		/// </summary>
		/// <param name="order"></param>
		protected override void OnOrderUpdate(IOrder order)
		{
		}
		
		/// <summary>
		/// The OnExecution() method is called on an incoming execution. An execution 
		/// is another name for a fill of an order.
		/// </summary>
		/// <param name="execution"></param>
		protected override void OnExecution(IExecution execution)
		{

		}

		
		
        #region Properties

        [Description("")]
        [GridCategory("Parameters")]
        public double Multiplier
        {
            get { return multiplier; }
            set { multiplier = Math.Max(0.000, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int ProfitTarget
        {
            get { return profitTarget; }
            set { profitTarget = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int StopLoss
        {
            get { return stopLoss; }
            set { stopLoss = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int DonchianPeriod
        {
            get { return donchianPeriod; }
            set { donchianPeriod = Math.Max(1, value); }
        }

        #endregion
    }
}
