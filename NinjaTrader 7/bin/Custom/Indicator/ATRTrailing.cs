// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// ATRTrailing.
    /// </summary>
    [Description("Wilder’s Volatility System, developed by and named after Welles Wilder, is a volatility index made up of the ongoing calculated average, the True Range. The consideration of the True Range means that days with a low trading range (little difference between daily high and low), but still showing a clear price difference to the previous day")]
    [Gui.Design.DisplayName("ATRTrailing")]
    public class ATRTrailing : Indicator
    {
        #region Variables
		private double ATRTimes		= 4;
		private int period 			= 10;
		private int counter			= 0;
		private double ratched   = 0.005;
		private double ratchedval   = 0;

		private double SigClose		= 0;
		
		private bool shortval;
		private bool longval;
		
		private DataSeries Plotset;
		private DataSeries Sideset;  //  Short == 0  Long ==1

		
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Red, PlotStyle.Dot, "ATR Trailing Dn"));
			Add(new Plot(Color.Blue, PlotStyle.Dot, "ATR Trailing Up"));
            CalculateOnBarClose		= true;	// Call 'OnBarUpdate' only as a bar closes
            Overlay					= true;	// Plots the indicator on top of price
			Plotset = new DataSeries(this);
			Sideset = new DataSeries(this);

        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar < period) 
				return;

			if (Low[0] < Low[1] )
				{
					Sideset.Set(0);
				}
			
			if (High[0]  > High[1] )
				{	
					Sideset.Set(1);
				}

				ratchedval=(1 - ( counter * ratched));

			if (Sideset[1] == 0 )
				{
					SigClose = MIN(Low,(period))[0];
					Plotset.Set(SigClose + Bars.Instrument.MasterInstrument.Round2TickSize((ratchedval*(ATRTimes*ATR(period)[0]))));
				}
			
			if (Sideset[1] == 1)
				{
					SigClose = MAX(High,(period))[0];
					Plotset.Set(SigClose - Bars.Instrument.MasterInstrument.Round2TickSize((ratchedval*(ATRTimes*ATR(period)[0]))));
				}
			
			if(Sideset[1] == 1 && Low[1] <= Plotset[1])   
			
				{
					Sideset.Set(0);
					SigClose = High[1];
					counter = 0;
					Plotset.Set( SigClose + Bars.Instrument.MasterInstrument.Round2TickSize((ratchedval*(ATRTimes*ATR(period)[0]))));
					Lower.Set(Plotset[0]);    
				}
			
			if( Sideset[1] == 1 && Low[1] > Plotset[1])
				{
					Sideset.Set(1);
					SigClose = MAX(High,(period))[0];
					Plotset.Set(SigClose - Bars.Instrument.MasterInstrument.Round2TickSize((ratchedval*(ATRTimes*ATR(period)[0]))));
					if (Plotset[1] > Plotset[0])
						Plotset.Set(Plotset[1]);   
					Upper.Set(Plotset[0]);
				}
			
			if(Sideset[1] == 0 && High[1] >= Plotset[1])  
				{
					Sideset.Set(1);
					SigClose = High[1];
					counter = 0;
					Plotset.Set(SigClose - Bars.Instrument.MasterInstrument.Round2TickSize((ratchedval*(ATRTimes*ATR(period)[0]))));
					Upper.Set(Plotset[0]);      
				}
			if(Sideset[1] == 0  && High[1] < Plotset[1])
				{
					Sideset.Set(0);
					SigClose = MIN(Low,(period))[0];
					Plotset.Set( SigClose + Bars.Instrument.MasterInstrument.Round2TickSize((ratchedval*(ATRTimes*ATR(period)[0]))));
					if (Plotset[1] < Plotset[0])
						Plotset.Set(Plotset[1]);  
					Lower.Set(Plotset[0]);
				}
			counter++;	
			
		
		}

        #region Properties

        /// <summary>
        /// The acceleration step factor
        /// </summary>
        [Description("Number of ATR Multipliers")]
        [Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("Number of ATR (Ex. 3 Time ATR) ")]
        public double ATRTIMES
        {
            get { return ATRTimes; }
            set { ATRTimes = Math.Max(0, value); }
        }
		
		[Description("Period")]
        [Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("Periods")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(0, value); }
        }
		/// <summary>
		/// Gets the lower value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Lower
		{
			get { return Values[0]; }
		}
		
		/// <summary>
		/// Get the Upper value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Upper
		{
			get { return Values[1]; }
		}
		
		[Description("Ratchet Percent")]
        [Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("Ratchet Percent")]
        public double Ratched
        {
            get { return ratched; }
            set { ratched = Math.Max(0, value); }
        }
		
		
		#endregion

    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private ATRTrailing[] cacheATRTrailing = null;

        private static ATRTrailing checkATRTrailing = new ATRTrailing();

        /// <summary>
        /// Wilder’s Volatility System, developed by and named after Welles Wilder, is a volatility index made up of the ongoing calculated average, the True Range. The consideration of the True Range means that days with a low trading range (little difference between daily high and low), but still showing a clear price difference to the previous day
        /// </summary>
        /// <returns></returns>
        public ATRTrailing ATRTrailing(double aTRTIMES, int period, double ratched)
        {
            return ATRTrailing(Input, aTRTIMES, period, ratched);
        }

        /// <summary>
        /// Wilder’s Volatility System, developed by and named after Welles Wilder, is a volatility index made up of the ongoing calculated average, the True Range. The consideration of the True Range means that days with a low trading range (little difference between daily high and low), but still showing a clear price difference to the previous day
        /// </summary>
        /// <returns></returns>
        public ATRTrailing ATRTrailing(Data.IDataSeries input, double aTRTIMES, int period, double ratched)
        {
            if (cacheATRTrailing != null)
                for (int idx = 0; idx < cacheATRTrailing.Length; idx++)
                    if (Math.Abs(cacheATRTrailing[idx].ATRTIMES - aTRTIMES) <= double.Epsilon && cacheATRTrailing[idx].Period == period && Math.Abs(cacheATRTrailing[idx].Ratched - ratched) <= double.Epsilon && cacheATRTrailing[idx].EqualsInput(input))
                        return cacheATRTrailing[idx];

            lock (checkATRTrailing)
            {
                checkATRTrailing.ATRTIMES = aTRTIMES;
                aTRTIMES = checkATRTrailing.ATRTIMES;
                checkATRTrailing.Period = period;
                period = checkATRTrailing.Period;
                checkATRTrailing.Ratched = ratched;
                ratched = checkATRTrailing.Ratched;

                if (cacheATRTrailing != null)
                    for (int idx = 0; idx < cacheATRTrailing.Length; idx++)
                        if (Math.Abs(cacheATRTrailing[idx].ATRTIMES - aTRTIMES) <= double.Epsilon && cacheATRTrailing[idx].Period == period && Math.Abs(cacheATRTrailing[idx].Ratched - ratched) <= double.Epsilon && cacheATRTrailing[idx].EqualsInput(input))
                            return cacheATRTrailing[idx];

                ATRTrailing indicator = new ATRTrailing();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ATRTIMES = aTRTIMES;
                indicator.Period = period;
                indicator.Ratched = ratched;
                Indicators.Add(indicator);
                indicator.SetUp();

                ATRTrailing[] tmp = new ATRTrailing[cacheATRTrailing == null ? 1 : cacheATRTrailing.Length + 1];
                if (cacheATRTrailing != null)
                    cacheATRTrailing.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheATRTrailing = tmp;
                return indicator;
            }
        }
    }
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
    public partial class Column : ColumnBase
    {
        /// <summary>
        /// Wilder’s Volatility System, developed by and named after Welles Wilder, is a volatility index made up of the ongoing calculated average, the True Range. The consideration of the True Range means that days with a low trading range (little difference between daily high and low), but still showing a clear price difference to the previous day
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ATRTrailing ATRTrailing(double aTRTIMES, int period, double ratched)
        {
            return _indicator.ATRTrailing(Input, aTRTIMES, period, ratched);
        }

        /// <summary>
        /// Wilder’s Volatility System, developed by and named after Welles Wilder, is a volatility index made up of the ongoing calculated average, the True Range. The consideration of the True Range means that days with a low trading range (little difference between daily high and low), but still showing a clear price difference to the previous day
        /// </summary>
        /// <returns></returns>
        public Indicator.ATRTrailing ATRTrailing(Data.IDataSeries input, double aTRTIMES, int period, double ratched)
        {
            return _indicator.ATRTrailing(input, aTRTIMES, period, ratched);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Wilder’s Volatility System, developed by and named after Welles Wilder, is a volatility index made up of the ongoing calculated average, the True Range. The consideration of the True Range means that days with a low trading range (little difference between daily high and low), but still showing a clear price difference to the previous day
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ATRTrailing ATRTrailing(double aTRTIMES, int period, double ratched)
        {
            return _indicator.ATRTrailing(Input, aTRTIMES, period, ratched);
        }

        /// <summary>
        /// Wilder’s Volatility System, developed by and named after Welles Wilder, is a volatility index made up of the ongoing calculated average, the True Range. The consideration of the True Range means that days with a low trading range (little difference between daily high and low), but still showing a clear price difference to the previous day
        /// </summary>
        /// <returns></returns>
        public Indicator.ATRTrailing ATRTrailing(Data.IDataSeries input, double aTRTIMES, int period, double ratched)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ATRTrailing(input, aTRTIMES, period, ratched);
        }
    }
}
#endregion
