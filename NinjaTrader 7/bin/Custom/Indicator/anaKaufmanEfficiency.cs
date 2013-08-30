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

#region Global Enums

public enum anaKaufmanEfficiencyType {Classic, Balanced}

#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Kahlman's Efficiency Ratio.
    /// </summary>
    [Description("Kaufman's Efficiency Ratio.")]
    public class anaKaufmanEfficiency : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int period 							= 10; 
            private int smoothingPeriod 				= 14; 
			private int dynPeriod						= 0;
			private int lookback						= 0;
			private double signal						= 0.0;
			private double noise						= 0.0;
			private double ratio						= 0.0;
			private anaKaufmanEfficiencyType momType	= anaKaufmanEfficiencyType.Classic; 	
		
			// User defined variables (add any user defined variables below)
			private IDataSeries Anchor;
			private Momentum Mom;
      		private DataSeries AbsMomentum;
		
		#endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(new Pen(Color.CornflowerBlue,2), PlotStyle.Line, "KaufmanEfficiency"));
            Add(new Plot(new Pen(Color.Red,2), PlotStyle.Line, "SignalLine"));
			Add(new Plot(new Pen(Color.YellowGreen, 1), PlotStyle.HLine, "ChopLine"));
          
			CalculateOnBarClose	= true;
            Overlay				= false;
            PriceTypeSupported	= true;
 			AbsMomentum 		= new DataSeries(this);
       }

 		protected override void OnStartUp()
		{
			if(momType == anaKaufmanEfficiencyType.Classic)
			{
				Anchor = SMA(Input,1);
				lookback = period;
			}
			else
			{
				Anchor = SMA(SMA(Input,period+1),period+1);
				lookback = 0;
			}
			Mom = Momentum(Input,1);
		}

		/// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			
			if (CurrentBar <= 2 * period)
			{
				if (CurrentBar < 1)
					return;
				dynPeriod = Math.Min(CurrentBar,period);
				AbsMomentum.Set(Math.Abs(Mom[0]));
				signal = Math.Abs(Input[0]-Input[dynPeriod]);
				noise = (SUM(AbsMomentum, dynPeriod))[0];
				if (noise == 0)
					ratio = 0.0;
				else	
					ratio = 100 * (signal / noise);
				EfficiencyRatio.Set(ratio);
            	Average.Set(HMA(EfficiencyRatio, smoothingPeriod)[0]); // check HMA version
				ChopLine.Set(100/Math.Sqrt(period));
				return;
			}
			AbsMomentum.Set(Math.Abs(Mom[0]));
			signal = Math.Abs(Input[0] - Anchor[lookback]);
			noise = (SUM(AbsMomentum, period))[0];
			if (noise == 0)
				ratio = 0.0;
			else	
				ratio = 100 * (signal / noise);
			EfficiencyRatio.Set(ratio);
            Average.Set(HMA(EfficiencyRatio, smoothingPeriod)[0]); // check HMA version
			ChopLine.Set(100/Math.Sqrt(period));
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries EfficiencyRatio
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Average
        {
            get { return Values[1]; }
        }

         [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ChopLine
        {
            get { return Values[2]; }
        }

       	[Description("Period for Fractal Efficiency")]
        [Category("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }

        [Description("Period for Hull Moving Average")]
        [Category("Parameters")]
        public int SmoothingPeriod
        {
            get { return smoothingPeriod; }
            set { smoothingPeriod = Math.Max(1, value); }
        }
   		
		/// <summary>
		/// </summary>
		[Description("Formula for Momentum")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Momentum Formula")]
		public anaKaufmanEfficiencyType MomType
		{
			get { return momType; }
			set { momType = value; }
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
        private anaKaufmanEfficiency[] cacheanaKaufmanEfficiency = null;

        private static anaKaufmanEfficiency checkanaKaufmanEfficiency = new anaKaufmanEfficiency();

        /// <summary>
        /// Kaufman's Efficiency Ratio.
        /// </summary>
        /// <returns></returns>
        public anaKaufmanEfficiency anaKaufmanEfficiency(anaKaufmanEfficiencyType momType, int period, int smoothingPeriod)
        {
            return anaKaufmanEfficiency(Input, momType, period, smoothingPeriod);
        }

        /// <summary>
        /// Kaufman's Efficiency Ratio.
        /// </summary>
        /// <returns></returns>
        public anaKaufmanEfficiency anaKaufmanEfficiency(Data.IDataSeries input, anaKaufmanEfficiencyType momType, int period, int smoothingPeriod)
        {
            if (cacheanaKaufmanEfficiency != null)
                for (int idx = 0; idx < cacheanaKaufmanEfficiency.Length; idx++)
                    if (cacheanaKaufmanEfficiency[idx].MomType == momType && cacheanaKaufmanEfficiency[idx].Period == period && cacheanaKaufmanEfficiency[idx].SmoothingPeriod == smoothingPeriod && cacheanaKaufmanEfficiency[idx].EqualsInput(input))
                        return cacheanaKaufmanEfficiency[idx];

            lock (checkanaKaufmanEfficiency)
            {
                checkanaKaufmanEfficiency.MomType = momType;
                momType = checkanaKaufmanEfficiency.MomType;
                checkanaKaufmanEfficiency.Period = period;
                period = checkanaKaufmanEfficiency.Period;
                checkanaKaufmanEfficiency.SmoothingPeriod = smoothingPeriod;
                smoothingPeriod = checkanaKaufmanEfficiency.SmoothingPeriod;

                if (cacheanaKaufmanEfficiency != null)
                    for (int idx = 0; idx < cacheanaKaufmanEfficiency.Length; idx++)
                        if (cacheanaKaufmanEfficiency[idx].MomType == momType && cacheanaKaufmanEfficiency[idx].Period == period && cacheanaKaufmanEfficiency[idx].SmoothingPeriod == smoothingPeriod && cacheanaKaufmanEfficiency[idx].EqualsInput(input))
                            return cacheanaKaufmanEfficiency[idx];

                anaKaufmanEfficiency indicator = new anaKaufmanEfficiency();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.MomType = momType;
                indicator.Period = period;
                indicator.SmoothingPeriod = smoothingPeriod;
                Indicators.Add(indicator);
                indicator.SetUp();

                anaKaufmanEfficiency[] tmp = new anaKaufmanEfficiency[cacheanaKaufmanEfficiency == null ? 1 : cacheanaKaufmanEfficiency.Length + 1];
                if (cacheanaKaufmanEfficiency != null)
                    cacheanaKaufmanEfficiency.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheanaKaufmanEfficiency = tmp;
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
        /// Kaufman's Efficiency Ratio.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaKaufmanEfficiency anaKaufmanEfficiency(anaKaufmanEfficiencyType momType, int period, int smoothingPeriod)
        {
            return _indicator.anaKaufmanEfficiency(Input, momType, period, smoothingPeriod);
        }

        /// <summary>
        /// Kaufman's Efficiency Ratio.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaKaufmanEfficiency anaKaufmanEfficiency(Data.IDataSeries input, anaKaufmanEfficiencyType momType, int period, int smoothingPeriod)
        {
            return _indicator.anaKaufmanEfficiency(input, momType, period, smoothingPeriod);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Kaufman's Efficiency Ratio.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaKaufmanEfficiency anaKaufmanEfficiency(anaKaufmanEfficiencyType momType, int period, int smoothingPeriod)
        {
            return _indicator.anaKaufmanEfficiency(Input, momType, period, smoothingPeriod);
        }

        /// <summary>
        /// Kaufman's Efficiency Ratio.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaKaufmanEfficiency anaKaufmanEfficiency(Data.IDataSeries input, anaKaufmanEfficiencyType momType, int period, int smoothingPeriod)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.anaKaufmanEfficiency(input, momType, period, smoothingPeriod);
        }
    }
}
#endregion
