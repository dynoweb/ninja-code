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
	/// The EMA_Colors_Paint_v01 (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
	/// </summary>
	[Description("The SMA_Colors_Paint_v01 (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.")]
	public class EMA_Colors_Paint_v01 : Indicator
	{
		#region Variables
		private int			period		= 14;
		private int			ema34slope	= 0;
		private double		radToDegrees		= 180/Math.PI; // to convert Radians to Degrees for slope calc
		private int			angle1		= 30;
		private int			angle2		= 60;
		
		// Paint Bars
			private bool colorbars = true;
			private Color barColorUp = Color.Green;
			private Color barColorDown = Color.Maroon;
			private Color barColorSlopeUp = Color.LimeGreen;
			private Color barColorSlopeDown = Color.Red;
			private Color barColorNeutral = Color.Gold;
			private Color barColorOutline = Color.Black;
		
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Green, "EMAup"));
			Add(new Plot(Color.Maroon, "EMAdown"));
			Add(new Plot(Color.Red, "EMAslopedown"));
			Add(new Plot(Color.LimeGreen, "EMAslopeup"));
			Add(new Plot(Color.Gray, "EMAflat"));
			Plots[0].Pen.Width = 2;
			Plots[1].Pen.Width = 2;
			Plots[2].Pen.Width = 2;
			Plots[3].Pen.Width = 2;
			Plots[4].Pen.Width = 2;
			Plots[0].Pen.DashStyle = DashStyle.Dash;
			Plots[1].Pen.DashStyle = DashStyle.Dash;
			Plots[2].Pen.DashStyle = DashStyle.Dash;
			Plots[3].Pen.DashStyle = DashStyle.Dash;
			Plots[4].Pen.DashStyle = DashStyle.Dash;
			Overlay				= true;
			PriceTypeSupported	= true;
		}
		
		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if(CurrentBar<Period) return;
				ema34slope = (int)(radToDegrees*(Math.Atan((EMA(Period)[0]-(EMA(Period)[1]+EMA(Period)[2])/2)/1.5/TickSize)));
			if(Rising(EMA(Period)))
			{
				if(ema34slope>=angle2)
				{
					EMAslopeup.Set(1,EMA(Period)[1]);
					EMAslopeup.Set(EMA(Period)[0]);//LimeGreen
					if (ColorBars)
					{ 
						CandleOutlineColor = BarColorOutline; 
						BarColor = BarColorSlopeUp; 
					}
				}
				else
				if (ema34slope>=angle1)
				{
					EMAup.Set(1,EMA(Period)[1]);
					EMAup.Set(EMA(Period)[0]);//Green
					if (ColorBars)
					{ 
						CandleOutlineColor = BarColorOutline; 
						BarColor = BarColorUp; 
					}
				}
					else
					{	EMAflat.Set(1,EMA(Period)[1]);
						EMAflat.Set(EMA(Period)[0]);//Gray
						if (ColorBars)
						{ 
							CandleOutlineColor = BarColorOutline; 
							BarColor = BarColorNeutral; 
						}
					}
			}
			else
			{	if(ema34slope<=-angle2)
				{
					EMAslopedown.Set(1,EMA(Period)[1]);
					EMAslopedown.Set(EMA(Period)[0]);//Red
					if (ColorBars)
					{ 
						CandleOutlineColor = BarColorOutline; 
						BarColor = BarColorSlopeDown; 
					}
				}
				else
				if (ema34slope<=-angle1)
				{
					EMAdown.Set(1,EMA(Period)[1]);
					EMAdown.Set(EMA(Period)[0]);//Maroon
					if (ColorBars)
					{ 
						CandleOutlineColor = BarColorOutline; 
						BarColor = BarColorDown; 
					}
				}
					else
					{	EMAflat.Set(1,EMA(Period)[1]);
						EMAflat.Set(EMA(Period)[0]);//Gray
						if (ColorBars)
						{ 
							CandleOutlineColor = BarColorOutline; 
							BarColor = BarColorNeutral; 
						}
					}
			}
		}

		#region Properties
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries EMAup
        {
            get { return Values[0]; }
        }
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries EMAdown
        {
            get { return Values[1]; }
        }
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries EMAslopedown
        {
            get { return Values[2]; }
        }
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries EMAslopeup
        {
            get { return Values[3]; }
        }
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries EMAflat
        {
            get { return Values[4]; }
        }
		[Description("Numbers of bars used for calculations")]
		[Category("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
		}
		
		[Description("EMA angle between Flat and Sloping zones.")]
		[Gui.Design.DisplayName("Angle 1")]
		[Category("Parameters")]
		public int Angle1
		{
			get { return angle1; }
			set { angle1 = Math.Max(1, value); }
		}
		
		[Description("EMA angle between Sloping and Steep zones.")]
		[Gui.Design.DisplayName("Angle 2")]
		[Category("Parameters")]
		public int Angle2
		{
			get { return angle2; }
			set { angle2 = Math.Max(1, value); }
		}
		
/////////////////////////////////////////////////////////////////////////////////////		
		[Description("Color price bars?")]
        [Category("Visual")]
        [Gui.Design.DisplayName("01. Color Bars?")]
        public bool ColorBars
        {
            get { return colorbars; }
            set { colorbars = value; }
        }
		
        [XmlIgnore()]
        [Description("Color of up bars")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("02. Up color")]
        public Color BarColorUp
        {
            get { return barColorUp; }
            set { barColorUp = value; }
        }
		
        [Browsable(false)]
        public string barColorUpSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(barColorUp); }
            set { barColorUp = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
		
        [XmlIgnore()]
        [Description("Color of down bars")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("03. Down color")]
        public Color BarColorDown
        {
            get { return barColorDown; }
            set { barColorDown = value; }
        }
		
        [Browsable(false)]
        public string barColorDownSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(barColorDown); }
            set { barColorDown = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
        
        
        [XmlIgnore()]
        [Description("Color of slope up bars")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("04. Slope Up color")]
        public Color BarColorSlopeUp
        {
            get { return barColorSlopeUp; }
            set { barColorSlopeUp = value; }
        }
		
        [Browsable(false)]
        public string barColorSlopeUpSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(barColorSlopeUp); }
            set { barColorSlopeUp = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
		
        [XmlIgnore()]
        [Description("Color of slope down bars")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("05. Slope Down color")]
        public Color BarColorSlopeDown
        {
            get { return barColorSlopeDown; }
            set { barColorSlopeDown = value; }
        }
		
        [Browsable(false)]
        public string barColorSlopeDownSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(barColorSlopeDown); }
            set { barColorSlopeDown = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
        
        [XmlIgnore()]
        [Description("Color of neutral bars")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("06. Neutral color")]
        public Color BarColorNeutral
        {
            get { return barColorNeutral; }
            set { barColorNeutral = value; }
        }
		
        [Browsable(false)]
        public string barColorNeutralSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(barColorNeutral); }
            set { barColorNeutral = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
		
        [XmlIgnore()]
        [Description("Color of bar outline")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("07. Outline color")]
        public Color BarColorOutline
        {
            get { return barColorOutline; }
            set { barColorOutline = value; }
        }
		
        [Browsable(false)]
        public string barColorOutlineSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(barColorOutline); }
            set { barColorOutline = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
		
////////////////////////////////////////////////////////////////////////////////////////		
		
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private EMA_Colors_Paint_v01[] cacheEMA_Colors_Paint_v01 = null;

        private static EMA_Colors_Paint_v01 checkEMA_Colors_Paint_v01 = new EMA_Colors_Paint_v01();

        /// <summary>
        /// The SMA_Colors_Paint_v01 (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public EMA_Colors_Paint_v01 EMA_Colors_Paint_v01(int angle1, int angle2, int period)
        {
            return EMA_Colors_Paint_v01(Input, angle1, angle2, period);
        }

        /// <summary>
        /// The SMA_Colors_Paint_v01 (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public EMA_Colors_Paint_v01 EMA_Colors_Paint_v01(Data.IDataSeries input, int angle1, int angle2, int period)
        {
            if (cacheEMA_Colors_Paint_v01 != null)
                for (int idx = 0; idx < cacheEMA_Colors_Paint_v01.Length; idx++)
                    if (cacheEMA_Colors_Paint_v01[idx].Angle1 == angle1 && cacheEMA_Colors_Paint_v01[idx].Angle2 == angle2 && cacheEMA_Colors_Paint_v01[idx].Period == period && cacheEMA_Colors_Paint_v01[idx].EqualsInput(input))
                        return cacheEMA_Colors_Paint_v01[idx];

            lock (checkEMA_Colors_Paint_v01)
            {
                checkEMA_Colors_Paint_v01.Angle1 = angle1;
                angle1 = checkEMA_Colors_Paint_v01.Angle1;
                checkEMA_Colors_Paint_v01.Angle2 = angle2;
                angle2 = checkEMA_Colors_Paint_v01.Angle2;
                checkEMA_Colors_Paint_v01.Period = period;
                period = checkEMA_Colors_Paint_v01.Period;

                if (cacheEMA_Colors_Paint_v01 != null)
                    for (int idx = 0; idx < cacheEMA_Colors_Paint_v01.Length; idx++)
                        if (cacheEMA_Colors_Paint_v01[idx].Angle1 == angle1 && cacheEMA_Colors_Paint_v01[idx].Angle2 == angle2 && cacheEMA_Colors_Paint_v01[idx].Period == period && cacheEMA_Colors_Paint_v01[idx].EqualsInput(input))
                            return cacheEMA_Colors_Paint_v01[idx];

                EMA_Colors_Paint_v01 indicator = new EMA_Colors_Paint_v01();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Angle1 = angle1;
                indicator.Angle2 = angle2;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                EMA_Colors_Paint_v01[] tmp = new EMA_Colors_Paint_v01[cacheEMA_Colors_Paint_v01 == null ? 1 : cacheEMA_Colors_Paint_v01.Length + 1];
                if (cacheEMA_Colors_Paint_v01 != null)
                    cacheEMA_Colors_Paint_v01.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheEMA_Colors_Paint_v01 = tmp;
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
        /// The SMA_Colors_Paint_v01 (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.EMA_Colors_Paint_v01 EMA_Colors_Paint_v01(int angle1, int angle2, int period)
        {
            return _indicator.EMA_Colors_Paint_v01(Input, angle1, angle2, period);
        }

        /// <summary>
        /// The SMA_Colors_Paint_v01 (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public Indicator.EMA_Colors_Paint_v01 EMA_Colors_Paint_v01(Data.IDataSeries input, int angle1, int angle2, int period)
        {
            return _indicator.EMA_Colors_Paint_v01(input, angle1, angle2, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The SMA_Colors_Paint_v01 (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.EMA_Colors_Paint_v01 EMA_Colors_Paint_v01(int angle1, int angle2, int period)
        {
            return _indicator.EMA_Colors_Paint_v01(Input, angle1, angle2, period);
        }

        /// <summary>
        /// The SMA_Colors_Paint_v01 (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public Indicator.EMA_Colors_Paint_v01 EMA_Colors_Paint_v01(Data.IDataSeries input, int angle1, int angle2, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.EMA_Colors_Paint_v01(input, angle1, angle2, period);
        }
    }
}
#endregion
