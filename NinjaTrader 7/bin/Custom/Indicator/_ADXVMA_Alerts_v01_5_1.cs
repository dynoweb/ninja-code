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
    /// Customized for Sharky by Big Mike (www.bigmiketrading.com) 06/16/2009
	/// Updated to v1.2 06/21/2009 by Big Mike
	/// Updated originally from ADXVMA_Sharky to _ADXVMA_Alerts_v01_5_1 07/13/09 by zeller4
	/// thanks to Sharky and Tzachi for implementation of the use of this indicator
	/// testing & comparisons by the user - don't use as a trade signal but rather as a preparation for a signal
	/// also, see Todd'd comments: http://forum.bigmiketrading.com/programmers-paradise/355-consolidation-alert-la-tzachi-2.html#post2245
    /// v01_2 7/14/09 by zeller4 - added text box with choices of text position similar to ECO2New2
	/// changed diamond default color to black and neutral plot to white dots
	/// v01_3 7/18/09 by zeller4 - changed diamond colors with serialization property
	/// changed to gold / gold - changeable by the user - new color should hold after saving workspace
	/// v01_4 7/18/09  changed to version number to coincide with _White_Dot_Alerts_v01_4 which have similar characteristics / development
	/// I hope there's no confusion because of this...
	/// changed property titles
	/// 
	/// v01_5 9/28/09 big mike -- added ability to not redraw plot if using a non-line plot
	/// v01_5_1 02/06/10 - TheWizard -- added ability to play sound on formation of new price bar & ability to color the price panel background
	/// </summary>
    [Description("_ADXVMA_Alerts_v01_5_1")]
    [Gui.Design.DisplayName("_ADXVMA_Alerts_v01_5_1")]
    public class _ADXVMA_Alerts_v01_5_1 : Indicator
    {
        #region Variables
        private bool colorbars = false;
		private Color barColorUp = Color.Blue;
		private Color barColorDown = Color.Red;
		private Color barColorNeutral = Color.Yellow;
		private Color barColorOutline = Color.DimGray;
		private bool colorbackground = false;  // added by TheWizard 02/06/10
		private Color backColorUp = Color.RoyalBlue;  // added by TheWizard 02/06/10
		private Color backColorDn = Color.DeepPink;  // added by TheWizard 02/06/10
		private int					opacity = 50;  // added by TheWizard 02/06/10
		
		private bool conservativemode = true;
		
		private int aDXPeriod = 5;
        private DataSeries PDI;
		private DataSeries PDM;
		private DataSeries MDM;
		private DataSeries MDI;
		private DataSeries Out;
		private double WeightDM;
		private double WeightDI;
		private double WeightDX;
		private double ChandeEMA;
		private double HHV = double.MinValue;
		private double LLV = double.MaxValue;
		private DataSeries main;
		
		private int direction;
			private int lastDirection;
			private int lastCalcBar = -2;
			private bool drawObjects = true;

//			private ArrayList data = new ArrayList();

		// Alert Arrows	
			private bool showArrows = false;
			private int arrowDisplacement = 7;
		
		// Signal
			private DataSeries signal;
			
		// Sound	
			private bool diamondsoundon=true;
			private bool arrowsoundon=false;
			private bool newbarsound=false;  // Added by TheWizard 02/06/10
			private string diamondalertwavfilename	= "alert4.wav";
			private string longwavfilename	= "long.wav";
			private string shortwavfilename	= "short.wav";
			private string newbarwavfilename = "AutoTrail.wav";  // Added by TheWizard 02/06/10
		
		// Alert Diamonds
			private bool showDiamonds = true;
			private Color diamondColor = Color.Gold;
			private int diamondDisplacement = 7;
		
		// Text Box
			private bool		showText = true;
			private bool		textOnLeft = false;
			private bool		textOnTop = true;
			private TextPosition bPosition = TextPosition.BottomLeft;
			private TextPosition tPosition = TextPosition.TopLeft;
			private Font textFontMed		= new Font("Arial", 14, FontStyle.Bold);
		
		
		
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
//            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "Rising"));
			Add(new Plot(new Pen(Color.Blue,4), PlotStyle.Line, "Rising"));
            Add(new Plot(new Pen(Color.Red,4), PlotStyle.Line, "Falling"));
            Add(new Plot(new Pen(Color.Gold,4), PlotStyle.Dot, "Neutral"));
//            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Line, "Falling"));
//			Add(new Plot(Color.FromKnownColor(KnownColor.Yellow), PlotStyle.Line, "Neutral"));
            AutoScale = false;
			CalculateOnBarClose	= true;
			Overlay				= true;
            PriceTypeSupported	= false;
			DisplayInDataBox	= false;
			PDI = new DataSeries( this );
			PDM = new DataSeries( this );
			MDM = new DataSeries( this );
			MDI = new DataSeries( this );
			Out = new DataSeries( this );
			WeightDX = ADXPeriod;
			WeightDM = ADXPeriod;
			WeightDI = ADXPeriod;
			ChandeEMA = ADXPeriod;
			main = new DataSeries(this);
			signal = new DataSeries(this);
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (newbarsound) if (FirstTickOfBar) PlaySound(newbarwavfilename);  // Added by TheWizard 02/06/10
			if( CurrentBar < 2 )
				
			{
				PDM.Set( 0 );
				MDM.Set( 0 );
				PDI.Set( 0 );
				MDI.Set( 0 );
				Out.Set( 0 );
				Main.Set( 0 );
				Rising.Set( 0 );
				Falling.Set( 0 );
				Neutral.Set( 0 );
				return;
			}
			try
			{
				
				if( lastCalcBar != CurrentBar )
				{
					lastCalcBar = CurrentBar;
//					setData( data, 0, 0d );
//					colorBar( 1 );
					lastDirection = direction;
				}
				
				int i = 0;
				PDM.Set( 0 );
				MDM.Set( 0 );
				if(Close[i]>Close[i+1])
					PDM.Set( Close[i]-Close[i+1] );//This array is not displayed.
				else
					MDM.Set( Close[i+1]-Close[i] );//This array is not displayed.
				
				PDM.Set(((WeightDM-1)*PDM[i+1] + PDM[i])/WeightDM);//ema.
				MDM.Set(((WeightDM-1)*MDM[i+1] + MDM[i])/WeightDM);//ema.
				
				double TR=PDM[i]+MDM[i];
				
				if (TR>0)
				{
					PDI.Set(PDM[i]/TR);
					MDI.Set(MDM[i]/TR);
					
				}//Avoid division by zero. Minimum step size is one unnormalized price pip.
				else
				{
					PDI.Set(0);
					MDI.Set(0);
					
				}
				
				PDI.Set(((WeightDI-1)*PDI[i+1] + PDI[i])/WeightDI);//ema.
				MDI.Set(((WeightDI-1)*MDI[i+1] + MDI[i])/WeightDI);//ema.

				double DI_Diff=PDI[i]-MDI[i];  
				if (DI_Diff<0)
					DI_Diff= -DI_Diff;//Only positive momentum signals are used.
				double DI_Sum=PDI[i]+MDI[i];
				double DI_Factor=0;//Zero case, DI_Diff will also be zero when DI_Sum is zero.
				if (DI_Sum>0)
					Out.Set(DI_Diff/DI_Sum);//Factional, near zero when PDM==MDM (horizonal), near 1 for laddering.
				else
					Out.Set(0);
	
				  Out.Set(((WeightDX-1)*Out[i+1] + Out[i])/WeightDX);
				
				if (Out[i]>Out[i+1])
				{
					HHV=Out[i];
					LLV=Out[i+1];
				}
				else
				{
					HHV=Out[i+1];
					LLV=Out[i];
				}
	
				for(int j=1;j<Math.Min(ADXPeriod,CurrentBar);j++)
				{
					if(Out[i+j+1]>HHV)HHV=Out[i+j+1];
					if(Out[i+j+1]<LLV)LLV=Out[i+j+1];
				}
				
				
				double diff = HHV - LLV;//Veriable reference scale, adapts to recent activity level, unnormalized.
				double VI=0;//Zero case. This fixes the output at its historical level. 
				if (diff>0)
					VI=(Out[i]-LLV)/diff;//Normalized, 0-1 scale.
				
				//   if (VI_0.VIsq_1.VIsqroot_2==1)VI*=VI;
				//   if (VI_0.VIsq_1.VIsqroot_2==2)VI=MathSqrt(VI);
				//   if (VI>VImax)VI=VImax;//Used by Bemac with VImax=0.4, still used in vma1 and affects 5min trend definition.
										//All the ema weight settings, including Chande, affect 5 min trend definition.
				//   if (VI<=zeroVIbelow)VI=0;                    
										
				main.Set(((ChandeEMA-VI)*main[i+1]+VI*Close[i])/ChandeEMA);//Chande VMA formula with ema built in.
				bool rising = false;
				bool falling = false;
				
				if (conservativemode)
				{
				if (main[0] > main[1])
				{ 
					Rising.Set(main[0]); 
					if (main[1] <= main[2] && Plots[0].PlotStyle == PlotStyle.Line)
						Rising.Set(1, main[1]);
					rising = true;
					if (colorbackground)
					{
					BackColor = Color.FromArgb(opacity,BackColorUp); // added By TheWizard 02/06/10
					}
				
					if (ColorBars)
					if (ChartControl.ChartStyleType == ChartStyleType.CandleStick)  // modified by TheWizard Feb 5, 2010 to allow for hollow-style candlesticks
					{
					BarColor = Color.Transparent;
					CandleOutlineColor = barColorUp;
					}
					else
					{
					BarColor = BarColorUp;
					}
				}
				else if (main[0] < main[1])
				{ 
					Falling.Set(main[0]); 
					if (main[1] >= main[2] && Plots[1].PlotStyle == PlotStyle.Line)
						Falling.Set(1, main[1]);
					falling = true;
										
					if (colorbackground)
					{
					BackColor = Color.FromArgb(opacity,BackColorDn); // added By TheWizard 02/06/10
					}
					
					if (ColorBars)
					if (ChartControl.ChartStyleType == ChartStyleType.CandleStick)  // modified by TheWizard Feb 5, 2010 to allow for hollow-style candlesticks
					{
					BarColor = Color.Transparent;
					CandleOutlineColor = barColorDown;
					}
					else
					{
					BarColor = BarColorDown;
					}
				}
				else
				{ 
					Neutral.Set(main[0]); Neutral.Set(1, main[1]); 
					rising = false; falling = false;
					
					if (ColorBars)
					if (ChartControl.ChartStyleType == ChartStyleType.CandleStick)  // modified by TheWizard Feb 5, 2010 to allow for hollow-style candlesticks
					{
					BarColor = BarColorNeutral;
					CandleOutlineColor = BarColorOutline;
					}
					else
					{
					BarColor = BarColorNeutral;
					}
				}
				}
				
				if (!conservativemode)
				{
				if (main[0] > main[1])
				{ 
					Rising.Set(main[0]); Rising.Set(1, main[1]);
					rising = true;
					if (colorbackground)
					{
					BackColor = Color.FromArgb(opacity,BackColorUp); // added By TheWizard 02/06/10
					}
					
					if (ColorBars)
					if (ChartControl.ChartStyleType == ChartStyleType.CandleStick)  // modified by TheWizard Feb 5, 2010 to allow for hollow-style candlesticks
					{
					BarColor = Color.Transparent;
					CandleOutlineColor = barColorUp;
					}
					else
					{
					BarColor = BarColorUp;
					}
				}
				else if (main[0] < main[1])
				{ 
					Falling.Set(main[0]); Falling.Set(1, main[1]);
					falling = true;
					if (colorbackground)
					{
					BackColor = Color.FromArgb(opacity,BackColorDn); // added By TheWizard 02/06/10
					}
					
					if (ColorBars)
					if (ChartControl.ChartStyleType == ChartStyleType.CandleStick)  // modified by TheWizard Feb 5, 2010 to allow for hollow-style candlesticks
					{
					BarColor = Color.Transparent;
					CandleOutlineColor = barColorDown;
					}
					else
					{
					BarColor = BarColorDown;
					}
				}
				else if (main[0] == main[0] && Median[0] > main[0])
				{ 
					Rising.Set(main[0]); Rising.Set(1, main[1]);
					rising = true;
					if (colorbackground)
					{
					BackColor = Color.FromArgb(opacity,BackColorUp); // added By TheWizard 02/06/10
					}
					
					if (ColorBars)
					if (ChartControl.ChartStyleType == ChartStyleType.CandleStick)  // modified by TheWizard Feb 5, 2010 to allow for hollow-style candlesticks
					{
					BarColor = Color.Transparent;
					CandleOutlineColor = barColorUp;
					}
					else
					{
					BarColor = BarColorUp;
					}
				}
				else if (main[0] == main[0] && Median[0] < main[0])
				{ 
					Falling.Set(main[0]); Falling.Set(1, main[1]);
					falling = true;
					if (colorbackground)
					{
					BackColor = Color.FromArgb(opacity,BackColorDn); // added By TheWizard 02/06/10
					}
					
					if (ColorBars)
					if (ChartControl.ChartStyleType == ChartStyleType.CandleStick)  // modified by TheWizard Feb 5, 2010 to allow for hollow-style candlesticks
					{
					BarColor = Color.Transparent;
					CandleOutlineColor = barColorDown;
					}
					else
					{
					BarColor = BarColorDown;
					}
				}
				else
				{ 
					Neutral.Set(main[0]); Neutral.Set(1, main[1]); 
					rising = false; falling = false;
					
					if (ColorBars)
					if (ChartControl.ChartStyleType == ChartStyleType.CandleStick)  // modified by TheWizard Feb 5, 2010 to allow for hollow-style candlesticks
					{
					BarColor = BarColorNeutral;
					CandleOutlineColor = BarColorOutline;
					}
					else
					{
					BarColor = BarColorNeutral;
					}
				}
				}
				
				
				if(rising)
				{
					
					Signal.Set(1);
					if (colorbackground)
					{
					BackColor = Color.FromArgb(opacity,BackColorUp); // added By TheWizard 02/06/10
					}
				}
				else if(falling)
				{
					
					Signal.Set(-1);
					if (colorbackground)
					{
					BackColor = Color.FromArgb(opacity,BackColorDn); // added By TheWizard 02/06/10
					}
				}
				else
				{
					
					Signal.Set(0);
				}
				if (Signal[0] == 0 && Signal[1] != 0)
				{
					if (showDiamonds && CurrentBar >=0)
					{
						
						double val1 = Neutral[i+1] + ( TickSize * DiamondDisplacement );
						double val2 = Neutral[i+1] - ( TickSize * DiamondDisplacement );
						DrawDiamond("topdiamond"+CurrentBar.ToString(),false,1, val1 , DiamondColor );
						DrawDiamond("botdiamond"+CurrentBar.ToString(),false,1, val2 , DiamondColor );
					}
						
						if (diamondsoundon && FirstTickOfBar)
					{						
							PlaySound(diamondalertwavfilename);
					}
//					Print("Signal: " +Signal.ToString());
				}
			if( ShowArrows && CurrentBar >=0)//> 2)
				{
					//if( lastDirection < direction )
					if (Signal[0] == 1 && Signal[1] != 1)
					{
						double val = Low[i+1] - ( TickSize * ArrowDisplacement );
					if (colorbackground)
					{
					BackColor = Color.FromArgb(opacity,BackColorUp); // added By TheWizard 02/06/10
					}
						 if (FirstTickOfBar) DrawArrowUp(CurrentBar.ToString(),1, val , barColorUp );
						//data[ data.Count - 1] = val  * -1;
						//setData( data, bar, val  * -1 );
						if (arrowsoundon && FirstTickOfBar){						
							PlaySound(longwavfilename);
						}		
					}
					//else if( lastDirection > direction )
					else if (Signal[0] == -1 && Signal[1] != -1)
					{
						double val = High[i+1] + ( TickSize * ArrowDisplacement );
					if (colorbackground)
					{
					BackColor = Color.FromArgb(opacity,BackColorDn); // added By TheWizard 02/06/10
					}
						if (FirstTickOfBar) DrawArrowDown(CurrentBar.ToString(),1, val, barColorDown );
						if (arrowsoundon && FirstTickOfBar){						
							PlaySound(shortwavfilename);
						}
						//setData( data, bar, val );						
					}	
				
				}
//--Text Box Info--//
				
//			double ECOminus = Math.Abs(eco[1]-eco[0]);
//			double ECOplus = Math.Abs(eco[0]-eco[1]);	
			
			if(textOnLeft)			
			{
				if(textOnTop)
				tPosition = TextPosition.TopLeft;
				else 
				bPosition = TextPosition.BottomLeft;
				
			}
			else
			{
				if(!textOnTop)
				bPosition = TextPosition.BottomRight;
				else
				tPosition = TextPosition.TopRight;
			}
						
			if(showText)
			{
			
			if(Signal[0] ==1)
			DrawTextFixed("Rising", " RISING ", tPosition, Color.White, textFontMed, Color.Black, Color.Blue, 10);
				else
			RemoveDrawObject("Rising");
			
			if(Signal[0] == -1)
			DrawTextFixed("Falling", " FALLING ", tPosition, Color.White, textFontMed, Color.Black, Color.Red, 10);
				else
			RemoveDrawObject("Falling");
			
			if(Signal[0] == 0)
			DrawTextFixed("Neutral", " NEUTRAL ", tPosition, Color.Black, textFontMed, Color.Black, Color.Goldenrod, 3);
				else
			RemoveDrawObject("Neutral");
			}				
				
				
				
			}
			catch( Exception ex )
			{
				Print( ex.ToString() );
			}
        }
//	private void setData( ArrayList list, int index, double val )
//		{
//			if( list == null || index < 0 )
//				return;
//			if( list.Count - 1 >= CurrentBar )
//				list[ list.Count - index - 1 ] = val;
//			else if( index == 0 )
//				list.Add( val );
//		}
        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Main
        {
            get { return main; }
        }
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Rising
        {
            get { return Values[0]; }
        }
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Falling
        {
            get { return Values[1]; }
        }
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Neutral
        {
            get { return Values[2]; }
        }
        [Description("ADX Period")]
        [Category("Parameters")]
        public int ADXPeriod
        {
            get { return aDXPeriod; }
            set { aDXPeriod = Math.Max(1, value); }
        }
		[Description("Conservative mode, when disabaled, will not change the trend direction until price closes beyond the line")]
        [Category("Parameters")]
        public bool ConservativeMode
        {
            get { return conservativemode; }
            set { conservativemode = value; }
        }
		[Description("Color price bars?")]
        [Category("Visual")]
        [Gui.Design.DisplayName("01. Color Bars?")]
        public bool ColorBars
        {
            get { return colorbars; }
            set { colorbars = value; }
        }

        /// <summary>
        /// </summary>
        [XmlIgnore()]
        [Description("Color of up bars")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("02. Up color")]
        public Color BarColorUp
        {
            get { return barColorUp; }
            set { barColorUp = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string barColorUpSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(barColorUp); }
            set { barColorUp = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [XmlIgnore()]
        [Description("Color of down bars")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("03. Down color")]
        public Color BarColorDown
        {
            get { return barColorDown; }
            set { barColorDown = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string barColorDownSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(barColorDown); }
            set { barColorDown = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
        
        /// <summary>
        /// </summary>
        [XmlIgnore()]
        [Description("Color of neutral bars")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("04. Neutral color")]
        public Color BarColorNeutral
        {
            get { return barColorNeutral; }
            set { barColorNeutral = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string barColorNeutralSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(barColorNeutral); }
            set { barColorNeutral = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
		/// <summary>
        /// </summary>
        [XmlIgnore()]
        [Description("Color of bar outline")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("05. Outline color")]
        public Color BarColorOutline
        {
            get { return barColorOutline; }
            set { barColorOutline = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string barColorOutlineSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(barColorOutline); }
            set { barColorOutline = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
		[Description(" Diamond Alert Sound")]
        [Category("Alerts")]
        public bool DiamondSoundOn
        {
            get { return diamondsoundon;}
            set { diamondsoundon = value; }
        }
		[Description(" Arrow Alert Sound")]
        [Category("Alerts")]
        public bool ArrowSoundOn
        {
            get { return arrowsoundon; }
            set { arrowsoundon = value; }
        }
		[Description(" New Bar Alert Sound")]  // Added by TheWizard 02/06/10
        [Category("Alerts")]  // Added by TheWizard 02/06/10
        public bool NewBarSound  // Added by TheWizard 02/06/10
        {
            get { return newbarsound; }  // Added by TheWizard 02/06/10
            set { newbarsound = value; }  // Added by TheWizard 02/06/10
        }
//		[Browsable(true)]
       	[XmlIgnore()]
        [Description("Color of diamonds.")]
        [Category("Drawing Objects")]
        [Gui.Design.DisplayNameAttribute("Diamond Color")]
        public Color DiamondColor
        {
            get { return diamondColor; }
            set { diamondColor = value; }
        }
		[Browsable(false)]
        public string diamondColorSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(diamondColor); }
            set { diamondColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Signal
        {
            get { return signal; }
        }
		[Description("Show Diamonds")]
        [Category("Drawing Objects")]
        public bool ShowDiamonds
        {
            get { return showDiamonds; }
            set { showDiamonds = value; }
        }
		[Description("Draw Arrows above/below bars")]
        [Category("Drawing Objects")]
        public int DiamondDisplacement
        {
            get { return diamondDisplacement; }
            set { diamondDisplacement = Math.Max( 0, value ); }
        }
		[Description("Sound file to play for white dot alert.")]
		[Category("Alerts")]
		public string DiamondAlertFileName
		{
			get { return diamondalertwavfilename; }
			set { diamondalertwavfilename = value; }
		}
		[Description("Sound file to play for long alert.")]
		[Category("Alerts")]
		public string ArrowLongAlertFileName
		{
			get { return longwavfilename; }
			set { longwavfilename = value; }
		}
		[Description("Sound file to play for short alert.")]
		[Category("Alerts")]
		public string ArrowShortAlertFileName
		{
			get { return shortwavfilename; }
			set { shortwavfilename = value; }
		}
		[Description("Sound file to play for new bar alert.")]  // Added by TheWizard 02/06/10
		[Category("Alerts")]  // Added by TheWizard 02/06/10
		public string NewBarWavFileName  // Added by TheWizard 02/06/10
		{
			get { return newbarwavfilename; }  // Added by TheWizard 02/06/10
			set { newbarwavfilename = value; }  // Added by TheWizard 02/06/10
		}
		[Description("Show Arrows")]
        [Category("Drawing Objects")]
        public bool ShowArrows
        {
            get { return showArrows; }
            set { showArrows = value; }
        }
		 [Description("Draw Arrows above/below bars")]
        [Category("Drawing Objects")]
        public int ArrowDisplacement
        {
            get { return arrowDisplacement; }
            set { arrowDisplacement = Math.Max( 0, value ); }
        }
		[Description("Show Text Info.")]
        [Category("Drawing Objects")]
        public bool ShowText
        {
            get { return showText; }
            set { showText = value; }
        }
		
		[Description("Text on left of chart? For those with no right margin.")]
        [Category("Drawing Objects")]
        public bool TextOnLeft
        {
            get { return textOnLeft; }
            set { textOnLeft = value; }
        }
		[Description("Text on top of chart?")]
        [Category("Drawing Objects")]
        public bool TextOnTop
        {
            get { return textOnTop; }
            set { textOnTop = value; }
        }
		[Description("Color Background?")]
		[Category("Visual")]
		[Gui.Design.DisplayNameAttribute("6. Color Background?")]
		public bool ColorBackground
		{
			get { return colorbackground; }
			set { colorbackground = value; }
		}
		[Description("Back Up Color")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("7. Background Up Color")]
        public Color BackColorUp
        {
            get { return backColorUp; }
            set { backColorUp = value; }
        }
        [Browsable(false)]
        public string backColorUpSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(backColorUp); }
            set { backColorUp = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
		[XmlIgnore()]
        [Description("Back Down COlor")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("8. Background Down Color")]
        public Color BackColorDn
        {
            get { return backColorDn; }
            set { backColorDn = value; }
        }
        [Browsable(false)]
        public string backColorDownSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(backColorDn); }
            set { backColorDn = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
		[Description("Background Opacity")]
		[Category("Visual")]
		[Gui.Design.DisplayNameAttribute("9. Background Opacity")]
		public int Opacity
		{
			get { return opacity; }
			set { opacity = value; }
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
        private _ADXVMA_Alerts_v01_5_1[] cache_ADXVMA_Alerts_v01_5_1 = null;

        private static _ADXVMA_Alerts_v01_5_1 check_ADXVMA_Alerts_v01_5_1 = new _ADXVMA_Alerts_v01_5_1();

        /// <summary>
        /// _ADXVMA_Alerts_v01_5_1
        /// </summary>
        /// <returns></returns>
        public _ADXVMA_Alerts_v01_5_1 _ADXVMA_Alerts_v01_5_1(int aDXPeriod, bool conservativeMode)
        {
            return _ADXVMA_Alerts_v01_5_1(Input, aDXPeriod, conservativeMode);
        }

        /// <summary>
        /// _ADXVMA_Alerts_v01_5_1
        /// </summary>
        /// <returns></returns>
        public _ADXVMA_Alerts_v01_5_1 _ADXVMA_Alerts_v01_5_1(Data.IDataSeries input, int aDXPeriod, bool conservativeMode)
        {
            if (cache_ADXVMA_Alerts_v01_5_1 != null)
                for (int idx = 0; idx < cache_ADXVMA_Alerts_v01_5_1.Length; idx++)
                    if (cache_ADXVMA_Alerts_v01_5_1[idx].ADXPeriod == aDXPeriod && cache_ADXVMA_Alerts_v01_5_1[idx].ConservativeMode == conservativeMode && cache_ADXVMA_Alerts_v01_5_1[idx].EqualsInput(input))
                        return cache_ADXVMA_Alerts_v01_5_1[idx];

            lock (check_ADXVMA_Alerts_v01_5_1)
            {
                check_ADXVMA_Alerts_v01_5_1.ADXPeriod = aDXPeriod;
                aDXPeriod = check_ADXVMA_Alerts_v01_5_1.ADXPeriod;
                check_ADXVMA_Alerts_v01_5_1.ConservativeMode = conservativeMode;
                conservativeMode = check_ADXVMA_Alerts_v01_5_1.ConservativeMode;

                if (cache_ADXVMA_Alerts_v01_5_1 != null)
                    for (int idx = 0; idx < cache_ADXVMA_Alerts_v01_5_1.Length; idx++)
                        if (cache_ADXVMA_Alerts_v01_5_1[idx].ADXPeriod == aDXPeriod && cache_ADXVMA_Alerts_v01_5_1[idx].ConservativeMode == conservativeMode && cache_ADXVMA_Alerts_v01_5_1[idx].EqualsInput(input))
                            return cache_ADXVMA_Alerts_v01_5_1[idx];

                _ADXVMA_Alerts_v01_5_1 indicator = new _ADXVMA_Alerts_v01_5_1();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ADXPeriod = aDXPeriod;
                indicator.ConservativeMode = conservativeMode;
                Indicators.Add(indicator);
                indicator.SetUp();

                _ADXVMA_Alerts_v01_5_1[] tmp = new _ADXVMA_Alerts_v01_5_1[cache_ADXVMA_Alerts_v01_5_1 == null ? 1 : cache_ADXVMA_Alerts_v01_5_1.Length + 1];
                if (cache_ADXVMA_Alerts_v01_5_1 != null)
                    cache_ADXVMA_Alerts_v01_5_1.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cache_ADXVMA_Alerts_v01_5_1 = tmp;
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
        /// _ADXVMA_Alerts_v01_5_1
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator._ADXVMA_Alerts_v01_5_1 _ADXVMA_Alerts_v01_5_1(int aDXPeriod, bool conservativeMode)
        {
            return _indicator._ADXVMA_Alerts_v01_5_1(Input, aDXPeriod, conservativeMode);
        }

        /// <summary>
        /// _ADXVMA_Alerts_v01_5_1
        /// </summary>
        /// <returns></returns>
        public Indicator._ADXVMA_Alerts_v01_5_1 _ADXVMA_Alerts_v01_5_1(Data.IDataSeries input, int aDXPeriod, bool conservativeMode)
        {
            return _indicator._ADXVMA_Alerts_v01_5_1(input, aDXPeriod, conservativeMode);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// _ADXVMA_Alerts_v01_5_1
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator._ADXVMA_Alerts_v01_5_1 _ADXVMA_Alerts_v01_5_1(int aDXPeriod, bool conservativeMode)
        {
            return _indicator._ADXVMA_Alerts_v01_5_1(Input, aDXPeriod, conservativeMode);
        }

        /// <summary>
        /// _ADXVMA_Alerts_v01_5_1
        /// </summary>
        /// <returns></returns>
        public Indicator._ADXVMA_Alerts_v01_5_1 _ADXVMA_Alerts_v01_5_1(Data.IDataSeries input, int aDXPeriod, bool conservativeMode)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator._ADXVMA_Alerts_v01_5_1(input, aDXPeriod, conservativeMode);
        }
    }
}
#endregion
