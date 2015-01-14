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
    [Description("Enter the description of your new custom indicator here")]
    public class SoderstromsCloud : Indicator
    {
        #region Variables
        
            //Parameters
			private int fastPrd = 9; 
            private int midPrd = 26; 
            private int slowPrd = 52;
      		
			//Fib
			private DataSeries fib6180;
			private DataSeries fib3820;
						
			//Ichimoku Kinko Hyo
			private DataSeries HighCloud;
			private DataSeries LowCloud;
			private DataSeries SpanA;
			private DataSeries SpanB;
		
        #endregion

       
        protected override void Initialize()
        {
            //Plots
			Add(new Plot(Color.FromKnownColor(KnownColor.ButtonShadow), PlotStyle.Line, "MaxCloud"));
            Add(new Plot(Color.FromKnownColor(KnownColor.ButtonShadow), PlotStyle.Line, "MinCloud"));
            
			//Minjatrader Parameters
			Overlay	= true;
			Displacement = 26;
			CalculateOnBarClose = false;
						
			fib6180= new DataSeries(this);
			fib3820= new DataSeries(this);
			HighCloud = new DataSeries(this);
			LowCloud = new DataSeries(this);
			SpanA = new DataSeries(this);
			SpanB = new DataSeries(this);
			
			
        }

        protected override void OnBarUpdate()
        {
			if(CurrentBar <= slowPrd)return;
			
			SpanA[0] = ( sMAX(High,fastPrd)[0] + sMIN(Low,fastPrd)[0] + sMAX(High,midPrd)[0] + sMIN(Low,midPrd)[0] ) / 4;
			SpanB[0] = ( sMAX(High,slowPrd)[0] + sMIN(Low,slowPrd)[0] ) / 2;
			fib6180[0] = ((sMAX(High,slowPrd)[0]-sMIN(Low,slowPrd)[0])*0.618)+sMIN(Low,slowPrd)[0];
			fib3820[0] = ((sMAX(High,slowPrd)[0]-sMIN(Low,slowPrd)[0])*0.382)+sMIN(Low,slowPrd)[0];
			
			HighCloud[0] = Math.Max(SpanA[0],Math.Max(SpanB[0],Math.Max(fib6180[0],fib3820[0])));
			LowCloud[0] = Math.Min(SpanA[0],Math.Min(SpanB[0],Math.Min(fib6180[0],fib3820[0])));
			
			MaxCloud.Set(HighCloud[0]);
			MinCloud.Set(LowCloud[0]);
			
			if(SpanA[0] > SpanB[0])
			{
				DrawLine(CurrentBar.ToString(),false,-26,HighCloud[0],-26,LowCloud[0],Color.Blue,DashStyle.Solid,1);
			}
			else
			{
				if(SpanA[0] < SpanB[0])
				{
					DrawLine(CurrentBar.ToString(),false,-26,HighCloud[0],-26,LowCloud[0],Color.Red,DashStyle.Solid,1);
				}
				else
				{
					DrawLine(CurrentBar.ToString(),false,-26,HighCloud[0],-26,LowCloud[0],Color.Gray,DashStyle.Solid,1);
				}
			}
				
			
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries MaxCloud
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries MinCloud
        {
            get { return Values[1]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int FastPrd
        {
            get { return fastPrd; }
            set { fastPrd = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MidPrd
        {
            get { return midPrd; }
            set { midPrd = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int SlowPrd
        {
            get { return slowPrd; }
            set { slowPrd = Math.Max(1, value); }
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
        private SoderstromsCloud[] cacheSoderstromsCloud = null;

        private static SoderstromsCloud checkSoderstromsCloud = new SoderstromsCloud();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public SoderstromsCloud SoderstromsCloud(int fastPrd, int midPrd, int slowPrd)
        {
            return SoderstromsCloud(Input, fastPrd, midPrd, slowPrd);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public SoderstromsCloud SoderstromsCloud(Data.IDataSeries input, int fastPrd, int midPrd, int slowPrd)
        {
            if (cacheSoderstromsCloud != null)
                for (int idx = 0; idx < cacheSoderstromsCloud.Length; idx++)
                    if (cacheSoderstromsCloud[idx].FastPrd == fastPrd && cacheSoderstromsCloud[idx].MidPrd == midPrd && cacheSoderstromsCloud[idx].SlowPrd == slowPrd && cacheSoderstromsCloud[idx].EqualsInput(input))
                        return cacheSoderstromsCloud[idx];

            lock (checkSoderstromsCloud)
            {
                checkSoderstromsCloud.FastPrd = fastPrd;
                fastPrd = checkSoderstromsCloud.FastPrd;
                checkSoderstromsCloud.MidPrd = midPrd;
                midPrd = checkSoderstromsCloud.MidPrd;
                checkSoderstromsCloud.SlowPrd = slowPrd;
                slowPrd = checkSoderstromsCloud.SlowPrd;

                if (cacheSoderstromsCloud != null)
                    for (int idx = 0; idx < cacheSoderstromsCloud.Length; idx++)
                        if (cacheSoderstromsCloud[idx].FastPrd == fastPrd && cacheSoderstromsCloud[idx].MidPrd == midPrd && cacheSoderstromsCloud[idx].SlowPrd == slowPrd && cacheSoderstromsCloud[idx].EqualsInput(input))
                            return cacheSoderstromsCloud[idx];

                SoderstromsCloud indicator = new SoderstromsCloud();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.FastPrd = fastPrd;
                indicator.MidPrd = midPrd;
                indicator.SlowPrd = slowPrd;
                Indicators.Add(indicator);
                indicator.SetUp();

                SoderstromsCloud[] tmp = new SoderstromsCloud[cacheSoderstromsCloud == null ? 1 : cacheSoderstromsCloud.Length + 1];
                if (cacheSoderstromsCloud != null)
                    cacheSoderstromsCloud.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSoderstromsCloud = tmp;
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
        public Indicator.SoderstromsCloud SoderstromsCloud(int fastPrd, int midPrd, int slowPrd)
        {
            return _indicator.SoderstromsCloud(Input, fastPrd, midPrd, slowPrd);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.SoderstromsCloud SoderstromsCloud(Data.IDataSeries input, int fastPrd, int midPrd, int slowPrd)
        {
            return _indicator.SoderstromsCloud(input, fastPrd, midPrd, slowPrd);
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
        public Indicator.SoderstromsCloud SoderstromsCloud(int fastPrd, int midPrd, int slowPrd)
        {
            return _indicator.SoderstromsCloud(Input, fastPrd, midPrd, slowPrd);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.SoderstromsCloud SoderstromsCloud(Data.IDataSeries input, int fastPrd, int midPrd, int slowPrd)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SoderstromsCloud(input, fastPrd, midPrd, slowPrd);
        }
    }
}
#endregion
