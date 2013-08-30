#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

namespace NinjaTrader.Indicator
{
    [Description("The anaSMMA (Smoothed Moving Average) is an indicator that shows the average value of a security's price over a period of time.")]
    [Gui.Design.DisplayName("anaSMMA (Smoothed Moving Average)")]
    public class anaSMMA : Indicator
    {
        #region Variables
		private int		period		= 14;
		private double 	recursive 	= 0.0; 
		#endregion

        protected override void Initialize()
        {
			Add(new Plot(Color.DeepSkyBlue, "anaSMMA"));
			
            CalculateOnBarClose	= true;
            Overlay				= true;
            PriceTypeSupported	= true;
        }
		
		protected override void OnBarUpdate()
        {		
			if (Period == 1)
				Value.Set(Input[0]);
			else if (CurrentBar <= Period)
				Value.Set(SMA(Period)[0]);
			else
			{	
				if (FirstTickOfBar)	
					recursive = (1 - 2.0/Period)*Value[1] + Value[2]/Period;
				Value.Set(recursive + Input[0]/Period);
			}
		}

        #region Properties
		[Description("Numbers of bars used for calculations")]
		[Category("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
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
        private anaSMMA[] cacheanaSMMA = null;

        private static anaSMMA checkanaSMMA = new anaSMMA();

        /// <summary>
        /// The anaSMMA (Smoothed Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public anaSMMA anaSMMA(int period)
        {
            return anaSMMA(Input, period);
        }

        /// <summary>
        /// The anaSMMA (Smoothed Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public anaSMMA anaSMMA(Data.IDataSeries input, int period)
        {
            if (cacheanaSMMA != null)
                for (int idx = 0; idx < cacheanaSMMA.Length; idx++)
                    if (cacheanaSMMA[idx].Period == period && cacheanaSMMA[idx].EqualsInput(input))
                        return cacheanaSMMA[idx];

            lock (checkanaSMMA)
            {
                checkanaSMMA.Period = period;
                period = checkanaSMMA.Period;

                if (cacheanaSMMA != null)
                    for (int idx = 0; idx < cacheanaSMMA.Length; idx++)
                        if (cacheanaSMMA[idx].Period == period && cacheanaSMMA[idx].EqualsInput(input))
                            return cacheanaSMMA[idx];

                anaSMMA indicator = new anaSMMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                anaSMMA[] tmp = new anaSMMA[cacheanaSMMA == null ? 1 : cacheanaSMMA.Length + 1];
                if (cacheanaSMMA != null)
                    cacheanaSMMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheanaSMMA = tmp;
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
        /// The anaSMMA (Smoothed Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaSMMA anaSMMA(int period)
        {
            return _indicator.anaSMMA(Input, period);
        }

        /// <summary>
        /// The anaSMMA (Smoothed Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaSMMA anaSMMA(Data.IDataSeries input, int period)
        {
            return _indicator.anaSMMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The anaSMMA (Smoothed Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaSMMA anaSMMA(int period)
        {
            return _indicator.anaSMMA(Input, period);
        }

        /// <summary>
        /// The anaSMMA (Smoothed Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaSMMA anaSMMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.anaSMMA(input, period);
        }
    }
}
#endregion
