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

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// </summary>
    /// This is an implementation of the 2-pole and 3-pole Butterworth Filters, as published by 
	/// John F. Ehlers in his book "Cybernetic Analysis for Stocks and Futures".
	[Description("Butterworth Filter")]
    [Gui.Design.DisplayName("anaButterworthFilter")]
    public class anaButterworthFilter : Indicator
    {
        #region Variables
        // Wizard generated variables
        private int period = 20;
        private int poles = 3;
        private double a1 = 0;
        private double b1 = 0;
        private double c1 = 0;
		private double coeff1 = 0;
 		private double coeff2 = 0;
		private double coeff3 = 0;
		private double coeff4 = 0;
        private double recursive = 0;
       #endregion

        protected override void Initialize()
        {
            Add(new Plot(Color.DeepSkyBlue, PlotStyle.Line, "Butterworth"));
            CalculateOnBarClose = false;
            Overlay = true;
            PriceTypeSupported = true;
        }

        protected override void OnStartUp()
        {
			double pi = Math.PI;
			double sq2 = Math.Sqrt(2.0);
			double sq3 = Math.Sqrt(3.0);
			if (Poles == 2)
			{
				a1 = Math.Exp(-sq2*pi/period);
				b1 = 2*a1*Math.Cos(sq2*pi/period);
				coeff1 = (1-b1 + a1*a1)/4.0;
				coeff2 = b1;
				coeff3 = -a1*a1;
			}	
			else if (Poles == 3)
			{
				a1 = Math.Exp(-pi/Period);
            	b1 = 2 * a1 * Math.Cos(sq3*pi/period);
				c1 = a1 * a1;
				coeff1 = (1- b1 + c1)*(1-c1)/8.0;
				coeff2 =  b1 + c1;
				coeff3 = -(c1 + b1*c1);
				coeff4 = c1*c1;
			}
        }
		
		/// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            if (CurrentBar < Math.Max(Period,Poles))
            {
                Butterworth.Set(Input[0]);
				return;
            }
          	if (FirstTickOfBar)
			{
				if ( Poles == 2)
					recursive = coeff1*(2*Input[1] + Input[2]) + coeff2*Value[1] + coeff3*Value[2];
				else if (Poles == 3)
					recursive = coeff1*(3*Input[1] + 3*Input[2] + Input[3]) + coeff2*Value[1] + coeff3*Value[2] + coeff4*Value[3];
			}
			Butterworth.Set(recursive + coeff1*Input[0]);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Butterworth
        {
            get { return Values[0]; }
        }

        [Description("")]
        [Category("Parameters")]
        public int Poles
        {
            get { return poles; }
            set { poles = Math.Min(Math.Max(2,value),3); }
        }

        [Description("")]
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
        private anaButterworthFilter[] cacheanaButterworthFilter = null;

        private static anaButterworthFilter checkanaButterworthFilter = new anaButterworthFilter();

        /// <summary>
        /// Butterworth Filter
        /// </summary>
        /// <returns></returns>
        public anaButterworthFilter anaButterworthFilter(int period, int poles)
        {
            return anaButterworthFilter(Input, period, poles);
        }

        /// <summary>
        /// Butterworth Filter
        /// </summary>
        /// <returns></returns>
        public anaButterworthFilter anaButterworthFilter(Data.IDataSeries input, int period, int poles)
        {
            if (cacheanaButterworthFilter != null)
                for (int idx = 0; idx < cacheanaButterworthFilter.Length; idx++)
                    if (cacheanaButterworthFilter[idx].Period == period && cacheanaButterworthFilter[idx].Poles == poles && cacheanaButterworthFilter[idx].EqualsInput(input))
                        return cacheanaButterworthFilter[idx];

            lock (checkanaButterworthFilter)
            {
                checkanaButterworthFilter.Period = period;
                period = checkanaButterworthFilter.Period;
                checkanaButterworthFilter.Poles = poles;
                poles = checkanaButterworthFilter.Poles;

                if (cacheanaButterworthFilter != null)
                    for (int idx = 0; idx < cacheanaButterworthFilter.Length; idx++)
                        if (cacheanaButterworthFilter[idx].Period == period && cacheanaButterworthFilter[idx].Poles == poles && cacheanaButterworthFilter[idx].EqualsInput(input))
                            return cacheanaButterworthFilter[idx];

                anaButterworthFilter indicator = new anaButterworthFilter();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                indicator.Poles = poles;
                Indicators.Add(indicator);
                indicator.SetUp();

                anaButterworthFilter[] tmp = new anaButterworthFilter[cacheanaButterworthFilter == null ? 1 : cacheanaButterworthFilter.Length + 1];
                if (cacheanaButterworthFilter != null)
                    cacheanaButterworthFilter.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheanaButterworthFilter = tmp;
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
        /// Butterworth Filter
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaButterworthFilter anaButterworthFilter(int period, int poles)
        {
            return _indicator.anaButterworthFilter(Input, period, poles);
        }

        /// <summary>
        /// Butterworth Filter
        /// </summary>
        /// <returns></returns>
        public Indicator.anaButterworthFilter anaButterworthFilter(Data.IDataSeries input, int period, int poles)
        {
            return _indicator.anaButterworthFilter(input, period, poles);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Butterworth Filter
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaButterworthFilter anaButterworthFilter(int period, int poles)
        {
            return _indicator.anaButterworthFilter(Input, period, poles);
        }

        /// <summary>
        /// Butterworth Filter
        /// </summary>
        /// <returns></returns>
        public Indicator.anaButterworthFilter anaButterworthFilter(Data.IDataSeries input, int period, int poles)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.anaButterworthFilter(input, period, poles);
        }
    }
}
#endregion
