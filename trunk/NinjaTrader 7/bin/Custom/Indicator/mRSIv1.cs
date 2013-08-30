// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

// Big Mike Trading Elite Only
// v1.0 alpha Aug 7 2012
// Aug 12 2012, made one change to eliminate the additional arrows produced when calling this 
//     indicator from a strategy. Mike Winfrey

#region Using declarations
using System;
using System.ComponentModel;
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
	/// The mRSIv1 (Relative Strength Index) is a price-following oscillator that ranges between 0 and 100.
	/// </summary>
	[Description("mRSIv1 Big Mike Trading RSI Divergence")]
	public class mRSIv1 : Indicator
	{
		private DataSeries					avgUp;
		private DataSeries					avgDown;
		private DataSeries					down;
		private int							period		= 7;
		private int							smooth		= 7;
		private int							divmaxbars 	= 50;
		private int							divthresh 	= 50;
		private int							divminbars	= 10;
		private int							divavgsupport= 1;
		private int							matype		= 9;
		private DataSeries					up;
		private DataSeries					ds1;
		private DataSeries					ds2;
		private DataSeries					div;
		
		protected override void Initialize()
		{
			Add(new Plot(Color.White, "mRSIv1"));
			Add(new Plot(Color.Gray, PlotStyle.Line, "Avg"));
			
			Plots[0].Pen.Width = 2;
			Plots[1].Pen.Width = 3;
			Plots[1].Pen.DashStyle = DashStyle.Dash;

			Add(new Line(System.Drawing.Color.DarkGray, 30, "Lower"));
			Add(new Line(System.Drawing.Color.DarkGray, 70, "Upper"));
			
			DrawOnPricePanel	= false;

			avgUp				= new DataSeries(this);
			avgDown				= new DataSeries(this);
			down				= new DataSeries(this);
			up					= new DataSeries(this);
			ds1					= new DataSeries(this);
			ds2					= new DataSeries(this);
			div					= new DataSeries(this);
		}

		/// <summary>
		/// Calculates the indicator value(s) at the current index.
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				down.Set(0);
				up.Set(0);

                if (Period < 3)
                    Avg.Set(50);
				return;
			}

			down.Set(Math.Max(Input[1] - Input[0], 0));
			up.Set(Math.Max(Input[0] - Input[1], 0));

			if ((CurrentBar + 1) < Period) 
			{
				if ((CurrentBar + 1) == (Period - 1))
					Avg.Set(50);
				return;
			}

			if ((CurrentBar + 1) == Period) 
			{
				// First averages 
				avgDown.Set(anaSuperSmootherFilter(down, Period, 3)[0]);
				avgUp.Set(anaSuperSmootherFilter(up, Period, 3)[0]);
			}  
			else 
			{
				// Rest of averages are smoothed
				avgDown.Set((avgDown[1] * (Period - 1) + down[0]) / Period);
				avgUp.Set((avgUp[1] * (Period - 1) + up[0]) / Period);
			}

			double rsi	  = avgDown[0] == 0 ? 100 : 100 - 100 / (1 + avgUp[0] / avgDown[0]);
			double rsiAvg = (2.0 / (1 + Smooth)) * rsi + (1 - (2.0 / (1 + Smooth))) * Avg[1];
			
			ds1.Set(rsi);
			ds2.Set(rsiAvg);

			switch (MAType)
			{
			
				case 0:
					Avg.Set(ds2[0]);
					Value.Set(ds1[0]);
					break;
				
				case 1:
					Avg.Set(SMA(ds2, Smooth)[0]);
					Value.Set(SMA(ds1, Period)[0]);
					break;	
					
				case 2:
					Avg.Set(EMA(ds2, Smooth)[0]);
					Value.Set(EMA(ds1, Period)[0]);
					break;	
					
				case 3:
					Avg.Set(VWMA(ds2, Smooth)[0]);
					Value.Set(VWMA(ds1, Period)[0]);
					break;	
							
				case 4:
					Avg.Set(anaSuperSmootherFilter(ds2, Smooth, 3)[0]);
					Value.Set(anaSuperSmootherFilter(ds1, Period, 3)[0]);
					break;
					
				case 5:
					Avg.Set(anaADXVMA(ds2, Smooth)[0]);
					Value.Set(anaADXVMA(ds1, Period)[0]);
					break;
					
				case 6:
					Avg.Set(anaButterworthFilter(ds2, Smooth, 3)[0]);
					Value.Set(anaButterworthFilter(ds1, Period, 3)[0]);
					break;
					
				case 7:
					Avg.Set(anaGaussianFilter(ds2, Smooth, 3)[0]);
					Value.Set(anaGaussianFilter(ds1, Period, 3)[0]);
					break;
					
				case 8:
					Avg.Set(anaEhlersFilter(ds2, Smooth)[0]);
					Value.Set(anaEhlersFilter(ds1, Period)[0]);
					break;
					
				case 9:
					Avg.Set(anaHoltEMA(ds2, Smooth, Convert.ToInt16(Smooth * 2.618))[0]);
					Value.Set(anaHoltEMA(ds1, Period, Convert.ToInt16(Period * 2.618))[0]);
					break;
					
				case 10:
					Avg.Set(anaMovingMedian(ds2, Smooth)[0]);
					Value.Set(anaMovingMedian(ds1, Period)[0]);
					break;
			}
			
			
			if (CurrentBar < DivMaxBars) return;
					
			div.Set(0);
			
			// Bearish divergence
			try
			{
				int mrohh = MRO(delegate {return (Input[0] > MAX(Input, DivMinBars)[1] && Value[0] > DivThresh && Value[0] >= MAX(Value, DivMaxBars)[0]);}, 1, DivMaxBars);
				int mrohh1 = (CurrentBar > mrohh && mrohh > DivMinBars ? MRO(delegate {return ((MIN(Value, mrohh)[0] < DivThresh) || (Value[0] < Value[mrohh] && High[0] >= High[mrohh] && (DivAvgSupport == 1 ? Avg[0] < DivThresh : true)));}, 1, DivMaxBars) : -1);
				if (mrohh > -1 && mrohh1 == 0 && MAX(Value, mrohh1)[1] < Value[mrohh] && Value[1] > Value[2] && Value[0] < Value[1] && High[mrohh1] > High[mrohh] && MIN(Value, mrohh1)[0] > DivThresh)
				{
					DrawOnPricePanel = false;
					DrawLine("HH " + (CurrentBar - mrohh), true, mrohh, Value[mrohh], 0, MAX(Value, mrohh1)[1], Color.Lime, DashStyle.Solid, 2);
					DrawOnPricePanel = true;
					DrawLine("HHPP " + (CurrentBar - mrohh), true, mrohh, High[mrohh], 0, High[mrohh1], Color.Lime, DashStyle.Solid, 2);
					div.Set(-1);
				//	DrawArrowDown("down " + CurrentBar, 0, High[0]+(4*TickSize), Color.Black);
				}

				
			} catch {}
			
			// Bullish divergence
			try
			{
				int mroll = MRO(delegate {return (Input[0] < MIN(Input, DivMinBars)[1] && Value[0] < (100-DivThresh) && Value[0] <= MIN(Value, DivMaxBars)[0]);}, 1, DivMaxBars);
				int mroll1 = (CurrentBar > mroll && mroll > DivMinBars ? MRO(delegate {return ((MAX(Value, mroll)[0] > (100-DivThresh)) || (Value[0] > Value[mroll] && Low[0] <= Low[mroll] && (DivAvgSupport == 1 ? Avg[0] > (100-DivThresh) : true)));}, 1, DivMaxBars) : -1);
				if (mroll > -1 && mroll1 == 0 && MIN(Value, mroll1)[1] > Value[mroll] && Value[1] < Value[2] && Value[0] > Value[1] && Low[mroll1] < Low[mroll] && MAX(Value, mroll1)[0] < (100-DivThresh)) 
				{
					DrawOnPricePanel = false;
					DrawLine("LL " + (CurrentBar - mroll), true, mroll, Value[mroll], 0, MIN(Value, mroll1)[1], Color.Red, DashStyle.Solid, 2);
					DrawOnPricePanel = true;
					DrawLine("LLPP " + (CurrentBar - mroll), true, mroll, Low[mroll], 0, Low[mroll1], Color.Red, DashStyle.Solid, 2);
					div.Set(1);
				//	DrawArrowUp("up " + CurrentBar, 0, Low[0]-(4*TickSize), Color.White);
				}

				
			} catch {}
			
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Avg
		{
			get { return Values[1]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Default
		{
			get { return Values[0]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Div
		{
			get { return div; }
		}
		
		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculations")]
		[GridCategory("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Number of bars for smoothing")]
		[GridCategory("Parameters")]
		public int Smooth
		{
			get { return smooth; }
			set { smooth = Math.Max(1, value); }
		}
		
		[Description("Divergence Max Bars")]
		[GridCategory("Parameters")]
		public int DivMaxBars
		{
			get { return divmaxbars; }
			set { divmaxbars = Math.Max(1, value); }
		}
		
		[Description("Divergence Cross Threshold")]
		[GridCategory("Parameters")]
		public int DivThresh
		{
			get { return divthresh; }
			set { divthresh = Math.Max(1, value); }
		}
		
		[Description("Divergence Min Bars")]
		[GridCategory("Parameters")]
		public int DivMinBars
		{
			get { return divminbars; }
			set { divminbars = Math.Max(1, value); }
		}
		
		[Description("Require Avg plot to support divergence 0..1")]
		[GridCategory("Parameters")]
		public int DivAvgSupport
		{
			get { return divavgsupport; }
			set { divavgsupport = Math.Max(0, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("MA Type 0..10")]
		[GridCategory("Parameters")]
		public int MAType
		{
			get { return matype; }
			set { matype = Math.Max(0, value); }
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
        private mRSIv1[] cachemRSIv1 = null;

        private static mRSIv1 checkmRSIv1 = new mRSIv1();

        /// <summary>
        /// mRSIv1 Big Mike Trading RSI Divergence
        /// </summary>
        /// <returns></returns>
        public mRSIv1 mRSIv1(int divAvgSupport, int divMaxBars, int divMinBars, int divThresh, int mAType, int period, int smooth)
        {
            return mRSIv1(Input, divAvgSupport, divMaxBars, divMinBars, divThresh, mAType, period, smooth);
        }

        /// <summary>
        /// mRSIv1 Big Mike Trading RSI Divergence
        /// </summary>
        /// <returns></returns>
        public mRSIv1 mRSIv1(Data.IDataSeries input, int divAvgSupport, int divMaxBars, int divMinBars, int divThresh, int mAType, int period, int smooth)
        {
            if (cachemRSIv1 != null)
                for (int idx = 0; idx < cachemRSIv1.Length; idx++)
                    if (cachemRSIv1[idx].DivAvgSupport == divAvgSupport && cachemRSIv1[idx].DivMaxBars == divMaxBars && cachemRSIv1[idx].DivMinBars == divMinBars && cachemRSIv1[idx].DivThresh == divThresh && cachemRSIv1[idx].MAType == mAType && cachemRSIv1[idx].Period == period && cachemRSIv1[idx].Smooth == smooth && cachemRSIv1[idx].EqualsInput(input))
                        return cachemRSIv1[idx];

            lock (checkmRSIv1)
            {
                checkmRSIv1.DivAvgSupport = divAvgSupport;
                divAvgSupport = checkmRSIv1.DivAvgSupport;
                checkmRSIv1.DivMaxBars = divMaxBars;
                divMaxBars = checkmRSIv1.DivMaxBars;
                checkmRSIv1.DivMinBars = divMinBars;
                divMinBars = checkmRSIv1.DivMinBars;
                checkmRSIv1.DivThresh = divThresh;
                divThresh = checkmRSIv1.DivThresh;
                checkmRSIv1.MAType = mAType;
                mAType = checkmRSIv1.MAType;
                checkmRSIv1.Period = period;
                period = checkmRSIv1.Period;
                checkmRSIv1.Smooth = smooth;
                smooth = checkmRSIv1.Smooth;

                if (cachemRSIv1 != null)
                    for (int idx = 0; idx < cachemRSIv1.Length; idx++)
                        if (cachemRSIv1[idx].DivAvgSupport == divAvgSupport && cachemRSIv1[idx].DivMaxBars == divMaxBars && cachemRSIv1[idx].DivMinBars == divMinBars && cachemRSIv1[idx].DivThresh == divThresh && cachemRSIv1[idx].MAType == mAType && cachemRSIv1[idx].Period == period && cachemRSIv1[idx].Smooth == smooth && cachemRSIv1[idx].EqualsInput(input))
                            return cachemRSIv1[idx];

                mRSIv1 indicator = new mRSIv1();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.DivAvgSupport = divAvgSupport;
                indicator.DivMaxBars = divMaxBars;
                indicator.DivMinBars = divMinBars;
                indicator.DivThresh = divThresh;
                indicator.MAType = mAType;
                indicator.Period = period;
                indicator.Smooth = smooth;
                Indicators.Add(indicator);
                indicator.SetUp();

                mRSIv1[] tmp = new mRSIv1[cachemRSIv1 == null ? 1 : cachemRSIv1.Length + 1];
                if (cachemRSIv1 != null)
                    cachemRSIv1.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachemRSIv1 = tmp;
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
        /// mRSIv1 Big Mike Trading RSI Divergence
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.mRSIv1 mRSIv1(int divAvgSupport, int divMaxBars, int divMinBars, int divThresh, int mAType, int period, int smooth)
        {
            return _indicator.mRSIv1(Input, divAvgSupport, divMaxBars, divMinBars, divThresh, mAType, period, smooth);
        }

        /// <summary>
        /// mRSIv1 Big Mike Trading RSI Divergence
        /// </summary>
        /// <returns></returns>
        public Indicator.mRSIv1 mRSIv1(Data.IDataSeries input, int divAvgSupport, int divMaxBars, int divMinBars, int divThresh, int mAType, int period, int smooth)
        {
            return _indicator.mRSIv1(input, divAvgSupport, divMaxBars, divMinBars, divThresh, mAType, period, smooth);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// mRSIv1 Big Mike Trading RSI Divergence
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.mRSIv1 mRSIv1(int divAvgSupport, int divMaxBars, int divMinBars, int divThresh, int mAType, int period, int smooth)
        {
            return _indicator.mRSIv1(Input, divAvgSupport, divMaxBars, divMinBars, divThresh, mAType, period, smooth);
        }

        /// <summary>
        /// mRSIv1 Big Mike Trading RSI Divergence
        /// </summary>
        /// <returns></returns>
        public Indicator.mRSIv1 mRSIv1(Data.IDataSeries input, int divAvgSupport, int divMaxBars, int divMinBars, int divThresh, int mAType, int period, int smooth)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.mRSIv1(input, divAvgSupport, divMaxBars, divMinBars, divThresh, mAType, period, smooth);
        }
    }
}
#endregion
