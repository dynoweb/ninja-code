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

// This namespace holds all indicators and is required. Do not change it
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// EhlersFilter
    /// </summary>
    [Description("EhlersFilter")]
    public class EhlersFilter : Indicator
    {
    #region Variables
    // Wizard generated variables
		
    private int length = 20; // Default setting for Length
		
    // User defined variables (add any user defined variables below)
		
	private DataSeries Smooth;
    private DataSeries Coef;        //defined as an array in EL code
    private DataSeries Distance2;    //defined as an array in EL code
    private int count		= 0;        //loop index
    private int lookback;    //loop index
    private double Num 		= 0.00;
    private double SumCoef  = 0.00;

    #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded
        /// </summary>	
        protected override void Initialize()			
        {
	     	Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "EF"));
	     	Overlay				= true;
	      	PriceTypeSupported	= true; 
			PriceType			= Data.PriceType.Median; // (H+L)/2
			Smooth 				= new DataSeries(this);
	  	    Coef	 		    = new DataSeries(this);
	       	Distance2 			= new DataSeries(this); 
        }
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>		
        protected override void OnBarUpdate()		
        {	

			 if (CurrentBar < length)
   			      return;	
			
			 Smooth.Set( 0.00);			 	
			 Smooth.Set( (Input[0] + 2*Input[1] + 2*Input[2] + Input[3]) / 6.0);	
	         for ( count = 0; count <= length -1; count++)
            {
                Distance2.Set( 0.00);				
                for ( lookback = 1; lookback <= length -1; lookback++)						
                {	
            	 Distance2.Set( Distance2[count] + (Smooth[count] - Smooth[count + lookback])*(Smooth[count] - Smooth[count + lookback]) );		
				 Coef.Set( count, Distance2[count]);                           
				}	
				Num 	= 0.00;
            	SumCoef = 0.00;	
				for ( count = 0; count <= length; count++)							
				{						
                Num = Num + Coef[count]*Smooth[count];
                SumCoef =  SumCoef + Coef[count];
				}
		 // Use this method for calculating your indicator values. Assign a value to each
         // plot below by replacing 'Close[0]' with your own formula	
			
         		if( SumCoef != 0) EF.Set( Num / SumCoef ); 
			}
		}
        
        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries EF
        {
            get { return Values[0]; }
        }

        [Description("Default setting for Length")]
        [Category("Parameters")]
        public int Length
        {
            get { return length; }
            set { length = Math.Max(1, value); }
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
        private EhlersFilter[] cacheEhlersFilter = null;

        private static EhlersFilter checkEhlersFilter = new EhlersFilter();

        /// <summary>
        /// EhlersFilter
        /// </summary>
        /// <returns></returns>
        public EhlersFilter EhlersFilter(int length)
        {
            return EhlersFilter(Input, length);
        }

        /// <summary>
        /// EhlersFilter
        /// </summary>
        /// <returns></returns>
        public EhlersFilter EhlersFilter(Data.IDataSeries input, int length)
        {
            if (cacheEhlersFilter != null)
                for (int idx = 0; idx < cacheEhlersFilter.Length; idx++)
                    if (cacheEhlersFilter[idx].Length == length && cacheEhlersFilter[idx].EqualsInput(input))
                        return cacheEhlersFilter[idx];

            lock (checkEhlersFilter)
            {
                checkEhlersFilter.Length = length;
                length = checkEhlersFilter.Length;

                if (cacheEhlersFilter != null)
                    for (int idx = 0; idx < cacheEhlersFilter.Length; idx++)
                        if (cacheEhlersFilter[idx].Length == length && cacheEhlersFilter[idx].EqualsInput(input))
                            return cacheEhlersFilter[idx];

                EhlersFilter indicator = new EhlersFilter();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Length = length;
                Indicators.Add(indicator);
                indicator.SetUp();

                EhlersFilter[] tmp = new EhlersFilter[cacheEhlersFilter == null ? 1 : cacheEhlersFilter.Length + 1];
                if (cacheEhlersFilter != null)
                    cacheEhlersFilter.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheEhlersFilter = tmp;
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
        /// EhlersFilter
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.EhlersFilter EhlersFilter(int length)
        {
            return _indicator.EhlersFilter(Input, length);
        }

        /// <summary>
        /// EhlersFilter
        /// </summary>
        /// <returns></returns>
        public Indicator.EhlersFilter EhlersFilter(Data.IDataSeries input, int length)
        {
            return _indicator.EhlersFilter(input, length);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// EhlersFilter
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.EhlersFilter EhlersFilter(int length)
        {
            return _indicator.EhlersFilter(Input, length);
        }

        /// <summary>
        /// EhlersFilter
        /// </summary>
        /// <returns></returns>
        public Indicator.EhlersFilter EhlersFilter(Data.IDataSeries input, int length)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.EhlersFilter(input, length);
        }
    }
}
#endregion
