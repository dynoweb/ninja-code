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
    /// anaADXVMA
    /// </summary>
    [Description("anaADXVMA")]
    [Gui.Design.DisplayName("anaADXVMA")]
    public class anaADXVMA : Indicator
    {
        #region Variables
		private int 		period 			= 6;
		private double		k				= 0.0;
		private double		hhp				= 0.0;
		private double		llp				= 0.0;
		private double		hhv				= 0.0;
		private double		llv				= 0.0;
		private double 		epsilon			= 0.0;
		private bool		showPlot		= true;
		private bool		showPaintBars	= true;
		private bool		candles			= true;
		private int			opacity			= 4;
		private int			alpha			= 0;
		private int 		plot0Width 		= 2;
		private DashStyle	dash0Style		= DashStyle.Solid;
		private PlotStyle	plot0Style		= PlotStyle.Line;
		private Color		upColor			= Color.CornflowerBlue;
		private Color		neutralColor	= Color.Yellow;
		private Color		downColor		= Color.Salmon;
		private DataSeries 	up;
		private DataSeries 	down;
		private DataSeries 	ups;
		private DataSeries 	downs;
		private DataSeries 	index;
		private IntSeries	trend;
		private ATR volatility; 
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			Add(new Plot(Color.Gray, PlotStyle.Line, "ADXVMA"));
            Overlay				= true;
            PriceTypeSupported	= false;
			PlotsConfigurable 	= false;
			up = new DataSeries(this);
			down = new DataSeries(this);
			ups = new DataSeries(this);
			downs = new DataSeries(this);
			index = new DataSeries(this);
			trend = new IntSeries(this);
        }
        
		protected override void OnStartUp()
		{
			k = 1.0/(double)period;
			volatility = ATR(200);
			Plots[0].PlotStyle = plot0Style;
			Plots[0].Pen.Width = plot0Width;
			Plots[0].Pen.DashStyle = dash0Style;
			if(showPlot)
				Plots[0].Pen.Color = Color.Gray;
			else	
				Plots[0].Pen.Color = Color.Transparent;
			if (ChartControl != null && ChartControl.ChartStyleType == ChartStyleType.CandleStick)
			{
				candles = true;
				alpha = 25*opacity;
			}
			else
				candles = false;
		}	
		
		/// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if( CurrentBar < 1 )
			{
				up.Set(0);
				down.Set(0);
				ups.Set(0);
				downs.Set(0);
				index.Set(0);
				ADXVMA.Set(Input[0]);
			}
			else
			{
				double currentUp = Math.Max(Input[0] - Input[1], 0);
             	double currentDown = Math.Max(Input[1] - Input[0], 0);
			    up.Set ((1-k)*up[1] + k*currentUp);
				down.Set((1-k)*down[1] + k*currentDown);
				
				double sum = up[0] + down[0];
				double fractionUp = 0.0;
				double fractionDown = 0.0;
				if(sum > double.Epsilon)
				{
					fractionUp = up[0]/sum;
					fractionDown = down[0]/sum;
				}
				ups.Set((1-k)*ups[1] + k*fractionUp);
				downs.Set((1-k)*downs[1] + k*fractionDown);
				
				double normDiff = Math.Abs(ups[0] - downs[0]);
				double normSum = ups[0] + downs[0];
				double normFraction = 0.0;
				if(normSum > double.Epsilon)
					normFraction = normDiff/normSum;
				index.Set((1-k)*index[1] + k*normFraction);
				
            	if(FirstTickOfBar)
				{
					epsilon = 0.1 * volatility[1];
					hhp = MAX(index,period)[1];
					llp = MIN(index,period)[1];
				}	
				hhv = Math.Max(index[0],hhp);
				llv = Math.Min(index[0],llp);
				
				double vDiff = hhv-llv;
				double vIndex = 0;
				if(vDiff > double.Epsilon)
					vIndex = (index[0] - llv)/vDiff;
				
				ADXVMA.Set((1 - k*vIndex)*ADXVMA[1] + k*vIndex*Input[0]);
				
				if(trend[1] > -1 && ADXVMA[0] > ADXVMA[1] + epsilon)
				{
					trend.Set(1);
					PlotColors[0][0] = upColor;
				}
				else if(trend[1] < 1 && ADXVMA[0] < ADXVMA[1] - epsilon)
				{
					trend.Set(-1);
					PlotColors[0][0] = downColor;
				}
				else
				{
					trend.Set(0);
					PlotColors[0][0] = neutralColor;
				}
				
				if(showPaintBars)
				{
					if(trend[0] == 1)
					{
						BarColor = upColor;
						CandleOutlineColor = upColor;
					}	
					else if(trend[0] == -1)
					{
						BarColor = downColor;
						CandleOutlineColor = downColor;
					}	
					else
					{
						BarColor = neutralColor;
						CandleOutlineColor = neutralColor;
					}	
					if(candles && Close[0] > Open[0])
						BarColor = Color.FromArgb(alpha, BarColor);
				}
			}
        }

        #region Properties
        [Browsable(false)]	
        [XmlIgnore()]		
        public DataSeries ADXVMA
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	
        [XmlIgnore()]		
        public IntSeries Trend
        {
            get { return trend; }
        }
		
        [Description("ADXVMA Period")]
        [Category("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }
		
		[Description("Show paint bars on price panel")]
        [Category("Options")]
		[Gui.Design.DisplayName ("Show PaintBars")]
        public bool ShowPaintBars
        {
            get { return showPaintBars; }
            set { showPaintBars = value; }
        }

		[Description("Show plot of the ADXVMA average")]
        [Category("Options")]
		[Gui.Design.DisplayName ("Show Plot")]
        public bool ShowPlot
        {
            get { return showPlot; }
            set { showPlot = value; }
        }

		/// <summary>
		/// </summary>
		[Description("Width for ADXVMA plot")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Line Width")]
		public int Plot0Width
		{
			get { return plot0Width; }
			set { plot0Width = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for ADXVMA plot")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Dash Style")]
		public DashStyle Dash0Style
		{
			get { return dash0Style; }
			set { dash0Style = value; }
		} 
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for ADXVMA plot")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Plot Style")]
		public PlotStyle Plot0Style
		{
			get { return plot0Style; }
			set { plot0Style = value; }
		} 
		
		[Description("When paint bars are activated, this parameter sets the opacity of the upclose bars")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Upclose Opacity")]
		public int Opacity
		{
		get { return opacity; }
		set { opacity = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("Select color for rising average")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Average Rising")]
		public Color UpColor
		{
			get { return upColor; }
			set { upColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string UpColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(upColor); }
			set { upColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("Select color for falling average")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Average Falling")]
		public Color DownColor
		{
			get { return downColor; }
			set { downColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string DownColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(downColor); }
			set { downColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("Select color for neutral average")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Average Chop Mode")]
		public Color NeutralColor
		{
			get { return neutralColor; }
			set { neutralColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string NeutralColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(neutralColor); }
			set { neutralColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
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
        private anaADXVMA[] cacheanaADXVMA = null;

        private static anaADXVMA checkanaADXVMA = new anaADXVMA();

        /// <summary>
        /// anaADXVMA
        /// </summary>
        /// <returns></returns>
        public anaADXVMA anaADXVMA(int period)
        {
            return anaADXVMA(Input, period);
        }

        /// <summary>
        /// anaADXVMA
        /// </summary>
        /// <returns></returns>
        public anaADXVMA anaADXVMA(Data.IDataSeries input, int period)
        {
            if (cacheanaADXVMA != null)
                for (int idx = 0; idx < cacheanaADXVMA.Length; idx++)
                    if (cacheanaADXVMA[idx].Period == period && cacheanaADXVMA[idx].EqualsInput(input))
                        return cacheanaADXVMA[idx];

            lock (checkanaADXVMA)
            {
                checkanaADXVMA.Period = period;
                period = checkanaADXVMA.Period;

                if (cacheanaADXVMA != null)
                    for (int idx = 0; idx < cacheanaADXVMA.Length; idx++)
                        if (cacheanaADXVMA[idx].Period == period && cacheanaADXVMA[idx].EqualsInput(input))
                            return cacheanaADXVMA[idx];

                anaADXVMA indicator = new anaADXVMA();
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

                anaADXVMA[] tmp = new anaADXVMA[cacheanaADXVMA == null ? 1 : cacheanaADXVMA.Length + 1];
                if (cacheanaADXVMA != null)
                    cacheanaADXVMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheanaADXVMA = tmp;
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
        /// anaADXVMA
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaADXVMA anaADXVMA(int period)
        {
            return _indicator.anaADXVMA(Input, period);
        }

        /// <summary>
        /// anaADXVMA
        /// </summary>
        /// <returns></returns>
        public Indicator.anaADXVMA anaADXVMA(Data.IDataSeries input, int period)
        {
            return _indicator.anaADXVMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// anaADXVMA
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaADXVMA anaADXVMA(int period)
        {
            return _indicator.anaADXVMA(Input, period);
        }

        /// <summary>
        /// anaADXVMA
        /// </summary>
        /// <returns></returns>
        public Indicator.anaADXVMA anaADXVMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.anaADXVMA(input, period);
        }
    }
}
#endregion
