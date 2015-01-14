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
namespace NinjaTrader.Indicator
{
   	[Description("Enter the description of your new custom indicator here")]
    public class AhrensMA : Indicator
    {
        //--------------------------------
		#region Variables
            private int MovAvgPRD = 5; 
			private DataSeries OCA;
			private DataSeries SubNum;
			private DataSeries AMA;
        #endregion

        //--------------------------------
		#region Initialize
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "AhrensHL"));
            Overlay	= true;
			OCA = new DataSeries(this);
			SubNum= new DataSeries(this);
			AMA= new DataSeries(this);
		}
		#endregion
		
        //--------------------------------
		#region OnBarUpdate
        protected override void OnBarUpdate()
        {		
			
        OCA[0] = (High[0]+Low[0])/2;
		SubNum[0]=(AMA[1]+AMA[MovAvgPRD])/2;
		AMA[0] = (CurrentBar < MovAvgPrd ? Close[0] : AMA[1]+((OCA[0]-SubNum[0])/MovAvgPRD));	
			
			AhrensHL.Set(AMA[0]);
			
        }
		#endregion
		
		//--------------------------------
        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries AhrensHL
        {
            get { return Values[0]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MovAvgPrd
        {
            get { return MovAvgPRD; }
            set { MovAvgPRD = Math.Max(1, value); }
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
        private AhrensMA[] cacheAhrensMA = null;

        private static AhrensMA checkAhrensMA = new AhrensMA();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public AhrensMA AhrensMA(int movAvgPrd)
        {
            return AhrensMA(Input, movAvgPrd);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public AhrensMA AhrensMA(Data.IDataSeries input, int movAvgPrd)
        {
            if (cacheAhrensMA != null)
                for (int idx = 0; idx < cacheAhrensMA.Length; idx++)
                    if (cacheAhrensMA[idx].MovAvgPrd == movAvgPrd && cacheAhrensMA[idx].EqualsInput(input))
                        return cacheAhrensMA[idx];

            lock (checkAhrensMA)
            {
                checkAhrensMA.MovAvgPrd = movAvgPrd;
                movAvgPrd = checkAhrensMA.MovAvgPrd;

                if (cacheAhrensMA != null)
                    for (int idx = 0; idx < cacheAhrensMA.Length; idx++)
                        if (cacheAhrensMA[idx].MovAvgPrd == movAvgPrd && cacheAhrensMA[idx].EqualsInput(input))
                            return cacheAhrensMA[idx];

                AhrensMA indicator = new AhrensMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.MovAvgPrd = movAvgPrd;
                Indicators.Add(indicator);
                indicator.SetUp();

                AhrensMA[] tmp = new AhrensMA[cacheAhrensMA == null ? 1 : cacheAhrensMA.Length + 1];
                if (cacheAhrensMA != null)
                    cacheAhrensMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheAhrensMA = tmp;
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
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AhrensMA AhrensMA(int movAvgPrd)
        {
            return _indicator.AhrensMA(Input, movAvgPrd);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.AhrensMA AhrensMA(Data.IDataSeries input, int movAvgPrd)
        {
            return _indicator.AhrensMA(input, movAvgPrd);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AhrensMA AhrensMA(int movAvgPrd)
        {
            return _indicator.AhrensMA(Input, movAvgPrd);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.AhrensMA AhrensMA(Data.IDataSeries input, int movAvgPrd)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.AhrensMA(input, movAvgPrd);
        }
    }
}
#endregion
