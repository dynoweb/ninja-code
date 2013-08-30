#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Enter the description of your new custom indicator here
    /// </summary>
    [Description("Trend-Quality Indicator by David Sepiashvili as described in Stocks & Commodities V. 22: 4 (14-20)")]
    public class TrendQuality : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int fastEMA = 7; // Default setting for FastEMA
            private int noisePeriod = 250; // Default setting for NoisePeriod
            private int slowEMA = 15; // Default setting for SlowEMA
            private int trendPeriod = 4; // Default setting for TrendPeriod
        // User defined variables (add any user defined variables below)
		
		private DataSeries _rev;
			private DataSeries _ema7;
			private DataSeries _ema15;
			private DataSeries _pds;
			private DataSeries _dc;
			private DataSeries _cpc;
			private DataSeries _trend;
			private DataSeries _smf;
			private DataSeries _dt;
			private DataSeries _noise;
			private DataSeries _madiff;
			private DataSeries _TQ;
			private DataSeries _ABSdt;
			private DataSeries Temp1;
		
		
		
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Bar, "TQI"));
            CalculateOnBarClose	= true;
            Overlay				= false;
            PriceTypeSupported	= false;
			
			
			_rev = new DataSeries(this);
			_ema7 = new DataSeries(this);
			_ema15 = new DataSeries(this);
			_pds = new DataSeries(this);
			_dc = new DataSeries(this);
			_cpc = new DataSeries(this);
			_trend = new DataSeries(this);
			_smf = new DataSeries(this);
			_dt = new DataSeries(this);
			_noise = new DataSeries(this);
			_madiff = new DataSeries(this);
			
			_ABSdt = new DataSeries(this);
			Temp1 = new DataSeries(this);
			
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
            
			
			if (CurrentBar < NoisePeriod)
				return;
			
			
			_ema7.Set(EMA(Close,fastEMA)[0]);
			
			_ema15.Set(EMA(Close,slowEMA)[0]);
			
			
			_rev.Set(_ema7[0] - _ema15[0]);
			
			Temp1[0] = 1 + TrendPeriod;
			
			_smf[0] = (2 / Temp1[0]);
			
			
			_dc[0] = Close[0] - Close[1];
			
		
				
			if (_rev[0] > 0)
	
				{
		
				_pds[0] = 1;	
			
		
				}
	
			else
	
				{
		
				_pds[0] = -1;
		
				}
			
			
		
			if (_pds[0] != _pds[1])
			
				{
			
				_cpc[0] = 0;
			
				}
			
			else
		
				{
			
				_cpc[0] = _cpc[1] + _dc[0];
			
				}
			

			
			if (_pds[0] != _pds[1])
			
				{
			
				_trend[0] = 0;
			
				}
			
			else
		
				{
			
				_trend[0] = _trend[1] * (1- _smf[0]) + (_cpc[0] * _smf[0]);
	
				}
				
			
			
			_dt[0] = _cpc[0] - _trend[0];	
				
			
				
			if (_dt[0]<0)
			
				{
				
				_ABSdt[0] = _dt[0] * -1;
				
				}
				
			else
			
				
				{
				
				_ABSdt[0] = _dt[0];
				
				}
			
			
			_madiff[0] = SMA(_ABSdt,noisePeriod)[0];
				
			
			
			_noise[0] = (2 * _madiff[0]);
			
			
			
			
			if (_noise[0] == 0)
		
				{
	
					TQI[0] = 0;

				}
			
			
			else
		
				{
			
					TQI[0] = _trend[0]/_noise[0];

				}	
			
			
				
			
				if(TQI[0]>0)
			
			
			PlotColors[0][0] = Color.DarkGreen;
			
			
			else
			
			PlotColors[0][0] = Color.Red;
				
				
				
			
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries TQI
        {
            get { return Values[0]; }
        }

        [Description("")]
        [Category("Parameters")]
        public int FastEMA
        {
            get { return fastEMA; }
            set { fastEMA = Math.Max(1, value); }
        }

        [Description("")]
        [Category("Parameters")]
        public int NoisePeriod
        {
            get { return noisePeriod; }
            set { noisePeriod = Math.Max(1, value); }
        }

        [Description("")]
        [Category("Parameters")]
        public int SlowEMA
        {
            get { return slowEMA; }
            set { slowEMA = Math.Max(1, value); }
        }

        [Description("")]
        [Category("Parameters")]
        public int TrendPeriod
        {
            get { return trendPeriod; }
            set { trendPeriod = Math.Max(1, value); }
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
        private TrendQuality[] cacheTrendQuality = null;

        private static TrendQuality checkTrendQuality = new TrendQuality();

        /// <summary>
        /// Trend-Quality Indicator by David Sepiashvili as described in Stocks & Commodities V. 22: 4 (14-20)
        /// </summary>
        /// <returns></returns>
        public TrendQuality TrendQuality(int fastEMA, int noisePeriod, int slowEMA, int trendPeriod)
        {
            return TrendQuality(Input, fastEMA, noisePeriod, slowEMA, trendPeriod);
        }

        /// <summary>
        /// Trend-Quality Indicator by David Sepiashvili as described in Stocks & Commodities V. 22: 4 (14-20)
        /// </summary>
        /// <returns></returns>
        public TrendQuality TrendQuality(Data.IDataSeries input, int fastEMA, int noisePeriod, int slowEMA, int trendPeriod)
        {
            if (cacheTrendQuality != null)
                for (int idx = 0; idx < cacheTrendQuality.Length; idx++)
                    if (cacheTrendQuality[idx].FastEMA == fastEMA && cacheTrendQuality[idx].NoisePeriod == noisePeriod && cacheTrendQuality[idx].SlowEMA == slowEMA && cacheTrendQuality[idx].TrendPeriod == trendPeriod && cacheTrendQuality[idx].EqualsInput(input))
                        return cacheTrendQuality[idx];

            lock (checkTrendQuality)
            {
                checkTrendQuality.FastEMA = fastEMA;
                fastEMA = checkTrendQuality.FastEMA;
                checkTrendQuality.NoisePeriod = noisePeriod;
                noisePeriod = checkTrendQuality.NoisePeriod;
                checkTrendQuality.SlowEMA = slowEMA;
                slowEMA = checkTrendQuality.SlowEMA;
                checkTrendQuality.TrendPeriod = trendPeriod;
                trendPeriod = checkTrendQuality.TrendPeriod;

                if (cacheTrendQuality != null)
                    for (int idx = 0; idx < cacheTrendQuality.Length; idx++)
                        if (cacheTrendQuality[idx].FastEMA == fastEMA && cacheTrendQuality[idx].NoisePeriod == noisePeriod && cacheTrendQuality[idx].SlowEMA == slowEMA && cacheTrendQuality[idx].TrendPeriod == trendPeriod && cacheTrendQuality[idx].EqualsInput(input))
                            return cacheTrendQuality[idx];

                TrendQuality indicator = new TrendQuality();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.FastEMA = fastEMA;
                indicator.NoisePeriod = noisePeriod;
                indicator.SlowEMA = slowEMA;
                indicator.TrendPeriod = trendPeriod;
                Indicators.Add(indicator);
                indicator.SetUp();

                TrendQuality[] tmp = new TrendQuality[cacheTrendQuality == null ? 1 : cacheTrendQuality.Length + 1];
                if (cacheTrendQuality != null)
                    cacheTrendQuality.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTrendQuality = tmp;
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
        /// Trend-Quality Indicator by David Sepiashvili as described in Stocks & Commodities V. 22: 4 (14-20)
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TrendQuality TrendQuality(int fastEMA, int noisePeriod, int slowEMA, int trendPeriod)
        {
            return _indicator.TrendQuality(Input, fastEMA, noisePeriod, slowEMA, trendPeriod);
        }

        /// <summary>
        /// Trend-Quality Indicator by David Sepiashvili as described in Stocks & Commodities V. 22: 4 (14-20)
        /// </summary>
        /// <returns></returns>
        public Indicator.TrendQuality TrendQuality(Data.IDataSeries input, int fastEMA, int noisePeriod, int slowEMA, int trendPeriod)
        {
            return _indicator.TrendQuality(input, fastEMA, noisePeriod, slowEMA, trendPeriod);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Trend-Quality Indicator by David Sepiashvili as described in Stocks & Commodities V. 22: 4 (14-20)
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TrendQuality TrendQuality(int fastEMA, int noisePeriod, int slowEMA, int trendPeriod)
        {
            return _indicator.TrendQuality(Input, fastEMA, noisePeriod, slowEMA, trendPeriod);
        }

        /// <summary>
        /// Trend-Quality Indicator by David Sepiashvili as described in Stocks & Commodities V. 22: 4 (14-20)
        /// </summary>
        /// <returns></returns>
        public Indicator.TrendQuality TrendQuality(Data.IDataSeries input, int fastEMA, int noisePeriod, int slowEMA, int trendPeriod)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TrendQuality(input, fastEMA, noisePeriod, slowEMA, trendPeriod);
        }
    }
}
#endregion
