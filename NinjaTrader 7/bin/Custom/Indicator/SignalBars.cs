#region Using declarations
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion
// Signal Bars v6 10/2007 ported from MetaTrader
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Signal Bars
    /// </summary>
    [Description("Signal Bars")]
    [Gui.Design.DisplayName("Signal Bars")]
    public class SignalBars : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int emaFastPeriod = 5; // Default setting for EmaFastPeriod
            private int emaSlowPeriod = 9; // Default setting for EmaSlowPeriod
            private int mACDFast = 8; // Default setting for MACDFast
            private int mACDSlow = 17; // Default setting for MACDSlow
            private int mACDSmooth = 9; // Default setting for MACDSmooth
			private int rSIPeriod = 15; // Default setting for RSIPeriod
			private int rSISmooth = 9; // Default setting for RSISmooth
			private int cCIPeriod = 13; // Default setting for CCIPeriod
			private int stochasticsD = 5; // Default setting for StochasticsD
			private int stochasticsK = 3; // Default setting for StochasticsK
			private int stochasticsSmooth = 3; // Default setting for StochasticsSmooth
        // User defined variables (add any user defined variables below)
        // User defined variables (add any user defined variables below)
		private MyEMA[] emasSlow;
		private MyEMA[] emasFast;
   		private MyMACD[] macds;
   		private MyCCI[] ccis;
   		private MyRSI[] rsis;
		private MyStochastics[] stochastics;
		private int indicators = 6;
		private int lastBar;
		private int lastMinute = 0;
		private int[] timeFrames;
		private int[] lastMin;
		private double[] tfHigh;
		private double[] tfLow;
		
		/*
		private static int testPeriod = 9;
		private MySMA mySma = new MySMA( testPeriod );
		private MySUM mySum = new MySUM( testPeriod );
		private MyMAX myMax = new MyMAX( testPeriod );
		private MyMIN myMin = new MyMIN( testPeriod );
		*/
		#endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose	= true;
            Overlay				= true;
            PriceTypeSupported	= false;
			
			lastBar = 0;
			try
			{
				emasSlow = new MyEMA[indicators];
				emasFast = new MyEMA[indicators];
				macds = new MyMACD[indicators];
				rsis = new MyRSI[indicators];
				ccis = new MyCCI[indicators];
				stochastics = new MyStochastics[indicators];
				lastMin = new int[ indicators - 1 ];
				tfHigh = new double[ indicators - 1 ];
				tfLow = new double[ indicators - 1 ];
				timeFrames = new int[]{ 5, 15, 30, 60, 240 };
				
				for( int i = 0; i < indicators; i++ )
				{
					emasSlow[ i ] = new MyEMA( EmaSlowPeriod );
					emasFast[ i ] = new MyEMA( EmaFastPeriod );
					macds[ i ] = new MyMACD(MACDFast, MACDSlow, MACDSmooth );
					rsis[ i ] = new MyRSI(RSIPeriod, RSISmooth);
					ccis[ i ] = new MyCCI(CCIPeriod );
					stochastics[ i ] = new MyStochastics(StochasticsD, StochasticsK, StochasticsSmooth );
				}
			}
			catch( Exception ex )
			{
				Print(ex.ToString());
			}
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {

			int minOffset = 0;
			try
			{
				int minute = Time[ 0 ].Minute + Time[ 0 ].Hour * 60;
				if( CurrentBar == 0 )
				{
					for( int i = 0; i < indicators - 1; i++ )
						lastMin[ i ] = minute;
					for( int i = 0; i < indicators - 1; i++ )
						tfHigh[ i ] = 0;
					for( int i = 0; i < indicators - 1; i++ )
						tfLow[ i ] = 0;
				}
				if( minute < lastMinute )
					minute += 1440;
				lastMinute  = minute;
				if( lastMinute > 1440 )
					lastMinute %= 1440;

				SetData( 5 );
				for( int i = 0; i < indicators - 1; i++ )
				{
					if( tfHigh[ i ] < High[ 0 ] )
						tfHigh[ i ] = High[ 0 ];
					if( tfLow[ i ] == 0 || tfLow[ i ] > Low[ 0 ] )
						tfLow[ i ] = Low[ 0 ];
				}
				
				for( int i = 0; i < indicators - 1; i++ )
				{
					if( ( minute + minOffset ) % timeFrames[ i ] == 0 || minute - minOffset - lastMin[ i ] > timeFrames[ i ] )
					{
						SetData( indicators - i - 2 );
						lastMin[ i ] = minute;
						if( lastMin[ i ] >= 1440 )
							lastMin[ i ] %= 1440;
						if( ( lastMin[ i ] + minOffset ) % timeFrames[ i ] != 0 )
						{
							lastMin[ i ] -= ( lastMin[ i ] + minOffset ) % timeFrames[ i ];
							if( lastMin[ i ] < 0 )
								lastMin[ i ] = 0;
						}
						//Print( minute + " " + ( indicators - i - 2 ) + " " + timeFrames[ i ] + " "  + lastMin[ i ] );
						tfLow[ i ] = 0;
						tfHigh[ i ] = 0;
					}
				}
				
			}
			catch( Exception exe )
			{
				Print( exe.ToString() );
			}
        }
		
		private void SetData( int tf )
		{
			double high;
			double low;
			double typical;
			if( tf == 5 )
			{
				high = High[ 0 ];
				low = Low[ 0 ];
			}
			else
			{
				high = tfHigh[ indicators - tf - 2 ];
				low = tfLow[ indicators - tf - 2 ];
			}
			typical = ( high + low + Close[ 0 ] ) / 3;
			/*if( tf == 0 )
			{
				mySma.Set( Input[0] );
				mySum.Set( Input[0] );
				myMax.Set( Input[0] );
				myMin.Set( Input[0] );
			}*/
			emasSlow[ tf ].Set( Input[ 0 ] );
			emasFast[ tf ].Set( Input[ 0 ] );
			macds[ tf ].Set( Input[ 0 ] );
			rsis[ tf ].Set( Input[ 0 ] );
			ccis[ tf ].Set( typical );
			stochastics[ tf ].Set( high, low, Close[ 0 ] );
		}

		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{
			// Default plotting in base class. 
			base.Plot(graphics, bounds, min, max);
			if (base.Bars == null)return;
				Brush strBrush = new SolidBrush(Color.DodgerBlue);

			Exception  caughtException;
			MyMACD macd;
			MyEMA emaS;
			MyEMA emaF;
			MyRSI rsi;
			MyCCI cci;
			MyStochastics stoch;
						
				try
				{
					int x1 = 10;
					int xGap = 4;
					int yGap = 4;
					int yGapHeading = 8;
					int y1 = 20;
					int width = 17;
					int height = 6;
					int indTextCorrection = 3;
					SolidBrush brush = new SolidBrush(Color.Red);
					Font font = new Font( "Arial", 7);
					
				
					int curX = x1;
					int curY = y1;
					Rectangle rect;
					string str = "";
	
					//Print( "SMA " + mySma.Get() + " SUM " + mySum.Get() + " MAX " + myMax.Get() + " MIN " + myMin.Get()  );
					for( int i = 0; i < 28; i++ )
					{
						if( i % 7 == 0 && i > 0 )
						{
							if( i / 7 == 0 )
								curY = y1 + i / 7 * ( height + yGapHeading );
							else
								curY = y1 + i / 7 * ( height + yGap ) + yGapHeading - yGap;
						}
						curX = x1 + i % 7 * ( width + xGap );
						if( i < 7 )
						{
							if( i == 0 )
								str = "H4";
							else if( i == 1 )
								str = "H1";
							else if( i == 2 )
								str = "M30";
							else if( i == 3 )
								str = "M15";
							else if( i == 4 )
								str = "M5";
							else if( i == 5 )
								str = "M1";

							if( i < 6 )
								graphics.DrawString(str, font, strBrush, curX, curY );
						}
						else
						{
							if( i % 7 < 6 )
							{
								Color col = Color.Purple;
								double val1 = 0;
								double val2 = 0;
								double val3 = 0;
								if( i / 7 == 1 )
								{
									macd = macds[ i % 7 ];
										val1 = macd.Get();
										val2 = macd.GetAvg();
										//Print( "MACD " + val1 + " " + val2 );
										if( val1 > val2 )
										{
											if( val1 > 0 )
												col = Color.Lime;
											else
												col = Color.Green;
										}
										if( val1 <= val2 )
										{
											if( val1 < 0 )
												col = Color.Red;
											else
												col = Color.Tomato;
										}
										/*if( val1 >= 0 && val2 >= 0 )
											col = Color.Green;
										if( val1 <= 0 && val2 >= 0 )
											col = Color.Tomato;
										if( val1 > 0 && val1 < 0 )
											col = Color.Lime;
										if( val1 < 0 && val1 < 0 )
											col = Color.Red;
										*/
								}
								else if( i / 7 == 2 )
								{
									rsi = rsis[ i % 7 ];
									cci = ccis[ i % 7 ];
									stoch = stochastics[ i % 7];
										val1 = rsi.GetRsi();
										val2 = stoch.GetD();
										val3 = cci.Get();
										//Print( "STR RSI " + val1 + " Stoch " + val2 + " CCI " + val3 + " MAX " + stoch.maxk() + " " + stoch.mink() );
										if( val1 > 50 && val2 > 40 && val3 > 0 )
											col = Color.Lime;
										if( val1 < 50 && val2 < 60 && val3 < 0 )
											col = Color.Red;
										if( val1 < 50 && val2 > 40 && val3 > 0 )
											col = Color.Orange;
										if( val1 > 50 && val2 < 60 && val3 < 0 )
											col = Color.Orange;
										if( val1 < 50 && val2 > 40 && val3 < 0 )
											col = Color.Orange;
										if( val1 > 50 && val2 < 60 && val3 > 0 )
											col = Color.Orange;
										if( val1 > 50 && val2 > 40 && val3 < 0 )
											col = Color.Orange;
										if( val1 > 50 && val2 < 60 && val3 < 0 )
											col = Color.Orange;										
								}
								else if( i / 7 == 3 )
								{
									emaS = emasSlow[ i % 7 ];
									emaF = emasFast[ i % 7 ];
									val1 = emaF.Get();
									val2 = emaS.Get();
									//Print( "EMA " + val1 + " " + val2 );
									if( val1 > val2 )
										col = Color.Lime;
									else
										col = Color.Red;
								}
								brush.Color = col;
								rect = new Rectangle( curX, curY, width, height );
								graphics.FillRectangle(brush, rect );
							}
							else
							{
								curY -= indTextCorrection;
								if( i / 7 == 1 )
									str = "MACD";
								else if( i / 7 == 2 )
									str = "STR";
								else if( i / 7 == 3 )
									str = "EMA";
								graphics.DrawString(str, font, strBrush, curX, curY );
							}
						}
					}

				}
				catch (Exception exception) { caughtException = exception; Print( exception.ToString());}
				}

        #region Properties

        [Description("Period for the fast EMA")]
        [Category("Parameters")]
        public int EmaFastPeriod
        {
            get { return emaFastPeriod; }
            set { emaFastPeriod = Math.Max(1, value); }
        }

        [Description("Period for the slow EMA")]
        [Category("Parameters")]
        public int EmaSlowPeriod
        {
            get { return emaSlowPeriod; }
            set { emaSlowPeriod = Math.Max(2, value); }
        }

        [Description("Fast MACD Period")]
        [Category("Parameters")]
        public int MACDFast
        {
            get { return mACDFast; }
            set { mACDFast = Math.Max(1, value); }
        }

        [Description("Slow MACD Period")]
        [Category("Parameters")]
        public int MACDSlow
        {
            get { return mACDSlow; }
            set { mACDSlow = Math.Max(1, value); }
        }


        [Description("MACD Smooth")]
        [Category("Parameters")]
        public int MACDSmooth
        {
            get { return mACDSmooth; }
            set { mACDSmooth = Math.Max(1, value); }

        }

        [Description("RSI Period")]
        [Category("Parameters")]
        public int RSIPeriod
        {
            get { return rSIPeriod; }
            set { rSIPeriod = Math.Max(1, value); }
        }

        [Description("RSI Smooth")]
        [Category("Parameters")]
        public int RSISmooth
        {
            get { return rSISmooth; }
            set { rSISmooth = Math.Max(1, value); }
        }

        [Description("CCI Period")]
        [Category("Parameters")]
        public int CCIPeriod
        {
            get { return cCIPeriod; }
            set { cCIPeriod = Math.Max(1, value); }
        }

        [Description("Stochastics %D Period")]
        [Category("Parameters")]
        public int StochasticsD
        {
            get { return stochasticsD; }
            set { stochasticsD = Math.Max(1, value); }
        }

        [Description("Stochastics %K Period")]
        [Category("Parameters")]
        public int StochasticsK
        {
            get { return stochasticsK; }
            set { stochasticsK = Math.Max(1, value); }
        }

        [Description("Stochastics Smooth")]
        [Category("Parameters")]
        public int StochasticsSmooth
        {
            get { return stochasticsSmooth; }
            set { stochasticsSmooth = Math.Max(1, value); }
        }
		#endregion
	}
    

	partial class MyEMA
	{
		private int period;
		private double val;
		private int count = 0;
		public MyEMA( int period )
		{
			this.period = period;
		}
		
		public void Set( double curVal )
		{
			if( count == 0 )
			{
				val = curVal;
			}
			else
			{
				val = curVal * (2.0 / (1 + period)) + (1 - (2.0 / (1 + period))) * val;
			}
			count++;
		}
		
		public double Get()
		{
			return val;
		}

	}


	partial class MySMA
	{
		private int Period;
		private double val;
		private ArrayList buff;
		public MySMA( int period )
		{
			this.Period = period;
			this.buff = new ArrayList( Period + 1 );
		}
		
		public void Set( double curVal )
		{
			buff.Insert( 0, curVal );

			if( buff.Count == 1 )
				val = curVal;
			else
			{
				double last = val * Math.Min(buff.Count-1, Period);
				if (buff.Count-1 >= Period)
					val =(last + (double)buff[0] - (double)buff[Period]) / Math.Min(buff.Count-1, Period);
				else
					val = (last + (double)buff[0]) / (Math.Min(buff.Count-1, Period) + 1);
			}
			if( buff.Count > Period )
				buff.RemoveAt( Period );
		}
		
		public double Get()
		{
			return val;
		}

	}

	partial class MyMACD
	{
		private int Fast;
		private int Slow;
		private int Smooth;
		private double macd;
		private double avg;
		private double diff;
		private ArrayList buff;
		private MyEMA slowEma;
		private MyEMA fastEma;
		private int count = 0;
		public MyMACD( int Fast, int Slow, int Smooth  )
		{
			avg = 0;
			diff = 0;
			this.Fast = Fast;
			this.Slow = Slow;
			this.Smooth = Smooth;
			fastEma = new MyEMA( Fast );
			slowEma = new MyEMA( Slow );
		}
		
		public void Set( double curVal )
		{
			if( count == 0 )
			{
				macd = 0;
				avg = 0;
				diff = 0;
			}
			else
			{
				fastEma.Set( curVal );
				slowEma.Set( curVal );
				macd = fastEma.Get() - slowEma.Get();
				avg	= (2.0 / (1 + Smooth)) * macd + (1 - (2.0 / (1 + Smooth))) * avg;
				diff = macd - avg;
			}
			count++;
		}
		
		public double Get()
		{
			return macd;
		}

		public double GetAvg()
		{
			return avg;
		}

		public double GetDiff()
		{
			return diff;
		}
	}

	partial class MyCCI
	{
		private int Period;
		private double val;
		private int count = 0;
		private ArrayList buff = null;
		private MySMA sma;
		public MyCCI( int Period )
		{
			this.Period = Period;
			buff = new ArrayList( Period + 1 );
			sma = new MySMA( Period );
		}
		
		public void Set( double curVal )
		{
			buff.Insert( 0, curVal );
			if (count == 0)
				val = 0;
			else
			{
				double mean = 0;
				sma.Set(curVal);
				for (int idx = Math.Min(buff.Count - 1, Period - 1); idx >= 0; idx--)
					mean += Math.Abs((double)buff[idx] - sma.Get());
				val = (curVal - sma.Get()) / (mean == 0 ? 1 : (0.015 * (mean / Math.Min(Period, count + 1))));
			}
			
			if( buff.Count == Period + 1 )
				buff.RemoveAt( Period );
			count++;
		}
		
		public double Get()
		{
			return val;
		}
	}

	partial class MyRSI
	{
		private int Period;
		private int Smooth;
		private double val;
		private double lastVal;
		private double avgUp;
		private double avgDown;
		private double down;
		private double up;
		private double avg;
		private MySMA downSMA;
		private MySMA upSMA;
		private int count = 0;
		public MyRSI( int period, int smooth )
		{
			this.Period = period;
			this.Smooth = smooth;
			downSMA = new MySMA( period );
			upSMA = new MySMA( period );
			val = 0;
			avg = 0;
		}
		
		public void Set( double curVal )
		{
			if( count == 0 )
			{
				up = 0;
				down = 0;
			}
			else
			{
				down = Math.Max(lastVal - curVal, 0);
				up = Math.Max(curVal - lastVal, 0);

				if ((count + 1) < Period) 
				{
					if ((count + 1) == (Period - 1))
						avg = 50;
				}
				else
				{
	
					if ((count + 1) == Period) 
					{
						avgDown = downSMA.Get();
						avgUp = upSMA.Get();
					}  
					else 
					{
						// Rest of averages are smoothed
						avgDown = (avgDown * (Period - 1) + down) / Period;
						avgUp = (avgUp * (Period - 1) + up) / Period;
					}
		
					double rs	  = avgUp / (avgDown == 0 ? 1 : avgDown);
					double rsi	  = 100 - (100 / (1 + rs));
					double rsiAvg = (2.0 / (1 + Smooth)) * rsi + (1 - (2.0 / (1 + Smooth))) * avg;
		
					avg = rsiAvg;
					val = rsi;
				}
			}
			if ((count + 1) <= Period) 
			{
				downSMA.Set( down );
				upSMA.Set( up );
			}
			lastVal = curVal;
			count++;
		}
		
		public double GetRsi()
		{
			return val;
		}

		public double GetAvg()
		{
			return avg;
		}
	}

	partial class MySUM
	{
		private int Period;
		private double val;
		private ArrayList buff;
		public MySUM( int period )
		{
			this.Period = period;
			this.buff = new ArrayList( Period + 1 );
			val = 0;
		}
		
		public void Set( double curVal )
		{
			buff.Insert( 0, curVal );

			if( buff.Count > Period )
			{
				val = val - (double)buff[Period] + curVal;
				buff.RemoveAt( Period );
			}
			else
				val += curVal;
		}
		
		public double Get()
		{
			return val;
		}
	}

	partial class MyMIN
	{
		private int Period;
		private double val;
		private ArrayList buff;
		public MyMIN( int period )
		{
			this.Period = period;
			this.buff = new ArrayList( Period );
			val = 0;
		}
		
		public void Set( double curVal )
		{
			if( buff.Count >= Period )
				buff.RemoveAt( Period - 1 );

			buff.Insert( 0, curVal );

			val = int.MaxValue;
			for( int i = 0; i < buff.Count; i++ )
			{
				if( i == 0 )
					val = (double)buff[ i ];
				else if( val > (double)buff[ i ] )
					val = (double)buff[ i ];
			}
		}
		
		public double Get()
		{
			return val;
		}
	}

	partial class MyMAX
	{
		private int Period;
		private double val;
		private ArrayList buff;
		public MyMAX( int period )
		{
			this.Period = period;
			this.buff = new ArrayList( Period );
			val = 0;
		}
		
		public void Set( double curVal )
		{
			if( buff.Count >= Period )
				buff.RemoveAt( Period - 1 );

			buff.Insert( 0, curVal );

			val = int.MinValue;
			for( int i = 0; i < buff.Count; i++ )
			{
				if( i == 0 )
					val = (double)buff[ i ];
				else if( val < (double)buff[ i ] )
					val = (double)buff[ i ];
			}
		}
		
		public double Get()
		{
			return val;
		}
	}

	partial class MyStochastics
	{
		private int Smooth;
		private int count = 0;
		private int PeriodD = 0;
		private int PeriodK = 0;
		private double den;
		private double nom;
		private double K;
		private double D;
		MyMIN minK;
		MyMAX maxK;
		MySUM sumD;
		MySUM sumNom;
		MySMA kSMA;
		public MyStochastics( int d, int k, int smooth )
		{
			this.Smooth = smooth;
			this.PeriodD = d;
			this.PeriodK = k;
			minK = new MyMIN( PeriodK );
			maxK = new MyMAX( PeriodK );
			sumD = new MySUM( PeriodD );
			sumNom = new MySUM( PeriodD );
			kSMA = new MySMA( Smooth );
		}
		
		public void Set( double high, double low, double close )
		{
			maxK.Set( high );
			minK.Set( low );
			den = maxK.Get() - minK.Get();
			nom = close - minK.Get();
			sumD.Set( den );
			sumNom.Set( nom );
			K = 100 * sumNom.Get() / (sumD.Get() == 0 ? 1.0 : sumD.Get());
			kSMA.Set( K );
			D = kSMA.Get();
		}
		
		public double GetD()
		{
			return D;
		}

		public double GetK()
		{
			return K;
		}		
	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private SignalBars[] cacheSignalBars = null;

        private static SignalBars checkSignalBars = new SignalBars();

        /// <summary>
        /// Signal Bars
        /// </summary>
        /// <returns></returns>
        public SignalBars SignalBars(int cCIPeriod, int emaFastPeriod, int emaSlowPeriod, int mACDFast, int mACDSlow, int mACDSmooth, int rSIPeriod, int rSISmooth, int stochasticsD, int stochasticsK, int stochasticsSmooth)
        {
            return SignalBars(Input, cCIPeriod, emaFastPeriod, emaSlowPeriod, mACDFast, mACDSlow, mACDSmooth, rSIPeriod, rSISmooth, stochasticsD, stochasticsK, stochasticsSmooth);
        }

        /// <summary>
        /// Signal Bars
        /// </summary>
        /// <returns></returns>
        public SignalBars SignalBars(Data.IDataSeries input, int cCIPeriod, int emaFastPeriod, int emaSlowPeriod, int mACDFast, int mACDSlow, int mACDSmooth, int rSIPeriod, int rSISmooth, int stochasticsD, int stochasticsK, int stochasticsSmooth)
        {
            if (cacheSignalBars != null)
                for (int idx = 0; idx < cacheSignalBars.Length; idx++)
                    if (cacheSignalBars[idx].CCIPeriod == cCIPeriod && cacheSignalBars[idx].EmaFastPeriod == emaFastPeriod && cacheSignalBars[idx].EmaSlowPeriod == emaSlowPeriod && cacheSignalBars[idx].MACDFast == mACDFast && cacheSignalBars[idx].MACDSlow == mACDSlow && cacheSignalBars[idx].MACDSmooth == mACDSmooth && cacheSignalBars[idx].RSIPeriod == rSIPeriod && cacheSignalBars[idx].RSISmooth == rSISmooth && cacheSignalBars[idx].StochasticsD == stochasticsD && cacheSignalBars[idx].StochasticsK == stochasticsK && cacheSignalBars[idx].StochasticsSmooth == stochasticsSmooth && cacheSignalBars[idx].EqualsInput(input))
                        return cacheSignalBars[idx];

            lock (checkSignalBars)
            {
                checkSignalBars.CCIPeriod = cCIPeriod;
                cCIPeriod = checkSignalBars.CCIPeriod;
                checkSignalBars.EmaFastPeriod = emaFastPeriod;
                emaFastPeriod = checkSignalBars.EmaFastPeriod;
                checkSignalBars.EmaSlowPeriod = emaSlowPeriod;
                emaSlowPeriod = checkSignalBars.EmaSlowPeriod;
                checkSignalBars.MACDFast = mACDFast;
                mACDFast = checkSignalBars.MACDFast;
                checkSignalBars.MACDSlow = mACDSlow;
                mACDSlow = checkSignalBars.MACDSlow;
                checkSignalBars.MACDSmooth = mACDSmooth;
                mACDSmooth = checkSignalBars.MACDSmooth;
                checkSignalBars.RSIPeriod = rSIPeriod;
                rSIPeriod = checkSignalBars.RSIPeriod;
                checkSignalBars.RSISmooth = rSISmooth;
                rSISmooth = checkSignalBars.RSISmooth;
                checkSignalBars.StochasticsD = stochasticsD;
                stochasticsD = checkSignalBars.StochasticsD;
                checkSignalBars.StochasticsK = stochasticsK;
                stochasticsK = checkSignalBars.StochasticsK;
                checkSignalBars.StochasticsSmooth = stochasticsSmooth;
                stochasticsSmooth = checkSignalBars.StochasticsSmooth;

                if (cacheSignalBars != null)
                    for (int idx = 0; idx < cacheSignalBars.Length; idx++)
                        if (cacheSignalBars[idx].CCIPeriod == cCIPeriod && cacheSignalBars[idx].EmaFastPeriod == emaFastPeriod && cacheSignalBars[idx].EmaSlowPeriod == emaSlowPeriod && cacheSignalBars[idx].MACDFast == mACDFast && cacheSignalBars[idx].MACDSlow == mACDSlow && cacheSignalBars[idx].MACDSmooth == mACDSmooth && cacheSignalBars[idx].RSIPeriod == rSIPeriod && cacheSignalBars[idx].RSISmooth == rSISmooth && cacheSignalBars[idx].StochasticsD == stochasticsD && cacheSignalBars[idx].StochasticsK == stochasticsK && cacheSignalBars[idx].StochasticsSmooth == stochasticsSmooth && cacheSignalBars[idx].EqualsInput(input))
                            return cacheSignalBars[idx];

                SignalBars indicator = new SignalBars();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.CCIPeriod = cCIPeriod;
                indicator.EmaFastPeriod = emaFastPeriod;
                indicator.EmaSlowPeriod = emaSlowPeriod;
                indicator.MACDFast = mACDFast;
                indicator.MACDSlow = mACDSlow;
                indicator.MACDSmooth = mACDSmooth;
                indicator.RSIPeriod = rSIPeriod;
                indicator.RSISmooth = rSISmooth;
                indicator.StochasticsD = stochasticsD;
                indicator.StochasticsK = stochasticsK;
                indicator.StochasticsSmooth = stochasticsSmooth;
                Indicators.Add(indicator);
                indicator.SetUp();

                SignalBars[] tmp = new SignalBars[cacheSignalBars == null ? 1 : cacheSignalBars.Length + 1];
                if (cacheSignalBars != null)
                    cacheSignalBars.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSignalBars = tmp;
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
        /// Signal Bars
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SignalBars SignalBars(int cCIPeriod, int emaFastPeriod, int emaSlowPeriod, int mACDFast, int mACDSlow, int mACDSmooth, int rSIPeriod, int rSISmooth, int stochasticsD, int stochasticsK, int stochasticsSmooth)
        {
            return _indicator.SignalBars(Input, cCIPeriod, emaFastPeriod, emaSlowPeriod, mACDFast, mACDSlow, mACDSmooth, rSIPeriod, rSISmooth, stochasticsD, stochasticsK, stochasticsSmooth);
        }

        /// <summary>
        /// Signal Bars
        /// </summary>
        /// <returns></returns>
        public Indicator.SignalBars SignalBars(Data.IDataSeries input, int cCIPeriod, int emaFastPeriod, int emaSlowPeriod, int mACDFast, int mACDSlow, int mACDSmooth, int rSIPeriod, int rSISmooth, int stochasticsD, int stochasticsK, int stochasticsSmooth)
        {
            return _indicator.SignalBars(input, cCIPeriod, emaFastPeriod, emaSlowPeriod, mACDFast, mACDSlow, mACDSmooth, rSIPeriod, rSISmooth, stochasticsD, stochasticsK, stochasticsSmooth);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Signal Bars
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SignalBars SignalBars(int cCIPeriod, int emaFastPeriod, int emaSlowPeriod, int mACDFast, int mACDSlow, int mACDSmooth, int rSIPeriod, int rSISmooth, int stochasticsD, int stochasticsK, int stochasticsSmooth)
        {
            return _indicator.SignalBars(Input, cCIPeriod, emaFastPeriod, emaSlowPeriod, mACDFast, mACDSlow, mACDSmooth, rSIPeriod, rSISmooth, stochasticsD, stochasticsK, stochasticsSmooth);
        }

        /// <summary>
        /// Signal Bars
        /// </summary>
        /// <returns></returns>
        public Indicator.SignalBars SignalBars(Data.IDataSeries input, int cCIPeriod, int emaFastPeriod, int emaSlowPeriod, int mACDFast, int mACDSlow, int mACDSmooth, int rSIPeriod, int rSISmooth, int stochasticsD, int stochasticsK, int stochasticsSmooth)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SignalBars(input, cCIPeriod, emaFastPeriod, emaSlowPeriod, mACDFast, mACDSlow, mACDSmooth, rSIPeriod, rSISmooth, stochasticsD, stochasticsK, stochasticsSmooth);
        }
    }
}
#endregion
