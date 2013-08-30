using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using d9.NinjaScript.Utility;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.Design;

namespace NinjaTrader.Indicator
{
    [Description("d9 Particle Oscillator_V2")]
    [Gui.Design.DisplayName(" d9 ParticleOscillator_V2")]
    public class d9ParticleOscillator_V2 : Indicator
    {
        private int _period = 7;  // Changed by Nano 8 June 2010
		private bool showArrows = false;  // Added by TheWizard October 25, 2009
		private bool alertonrawtrend = false;  // Added by TheWizard October 25, 2009
		private int _ArrowDisplacement = 30;  // Added by TheWizard October 25, 2009
		private int _TriangleDisplacement = 15;  // Added by TheWizard October 25, 2009
		private Color upColor = Color.Cyan;
        private Color downColor = Color.Magenta;
		private bool uptrend=false;
		private bool downtrend=false;
        private double _phase;
        private double _beta;
        private double _expFactor;
        private double _lengthParam;
        private double _windowLength;
        private double _permissivity;
		//copied next line from SMI2 indicator
		private double              offset = 1.0;
        private Dictionary<int, double[]> fbank;
        private DataSeries _vxTrend;
        private DataSeries _uSigma;
        private DataSeries _lSigma;
        private DataSeries _vx;
        private DataSeries _vxAvg;
        private DataSeries _zlr;
	
        private StringFormat stringFormat = new StringFormat();
        private SolidBrush textBrush = new SolidBrush(Color.Black);
        private System.Drawing.Font textFont = new Font("Arial", 8);
        private Color colorAboveSig = Color.White;
        private Color colorBelowSig = Color.DarkOrange;
        private Pen penDown = new Pen(Color.OrangeRed, 2f);
        private Pen penTurn = new Pen(Color.DimGray, 1f);
        private Pen penUp = new Pen(Color.Lime, 2f);
        private int opacity =75;
		private Color backgroundupcolor = Color.DodgerBlue;
		private Color backgrounddncolor = Color.HotPink;
        private bool _drawInfo = false;
        private bool _drawZLR = false;
		private bool soundOn = false;  // Added by TheWizard October 25, 2009
		private string longWavefilename = "long.wav";  // Added by TheWizard October 25, 2009
		private string shortWavefilename= "short.wav";  // Added by TheWizard October 25, 2009
		private int BarAtLastCross = 0;

        protected override void Initialize()
        {
            Add(new Plot(Color.Blue, "rawTrend")); //0
			Plots[0].Pen.Width = 2.0f; // Changed by TheWizard December 10, 2009
            
            Add(new Plot(Color.Transparent, "predictTrend")); //1
            Plots[1].PlotStyle = PlotStyle.Bar;
            Plots[1].Pen.DashStyle = DashStyle.Dot;
            
            Add(new Plot(Color.Lime, "DotU")); //2  // Changed by TheWizard December 10, 2009
            Plots[2].Pen.DashStyle = DashStyle.Dot;
            Plots[2].PlotStyle = PlotStyle.Dot;

            Add(new Plot(Color.Red, "DotL")); //3  // Changed by TheWizard December 10, 2009
            Plots[3].Pen.DashStyle = DashStyle.Dot;
            Plots[3].PlotStyle = PlotStyle.Dot;

            Add(new Plot(Color.Yellow, "DotWarn")); //4
            Plots[4].Pen.DashStyle = DashStyle.Dot;
            Plots[4].PlotStyle = PlotStyle.Dot;
            Plots[4].Pen.Width = 2.0f;

            Add(new Plot(Color.Magenta, "rawTrendSlope")); //5
            Add(new Plot(Color.Cyan, "predictSlope")); //6
           
            Add(new Line(Color.Transparent, 0, "zeroLine"));
            Lines[0].Pen.DashStyle = DashStyle.Dash;
			Add(new Line(Color.Transparent, -0.25, "Oversold"));  // Changed by TheWizard December 10, 2009
			Lines[0].Pen.Width = 2;  // Changed by TheWizard December 10, 2009
            Lines[1].Pen.DashStyle = DashStyle.Solid;  // Changed by TheWizard December 10, 2009
			Add(new Line(Color.Transparent, 0.25, "Overbought"));  // Changed by TheWizard December 10, 2009
			Lines[1].Pen.Width = 2;  // Changed by TheWizard December 10, 2009
            Lines[2].Pen.DashStyle = DashStyle.Solid;
			Add(new Line(Color.Transparent, -0.50, "VeryOversold"));
            Lines[3].Pen.DashStyle = DashStyle.Dash;  // Changed by TheWizard December 10, 2009
			Lines[3].Pen.Width = 2;
			Add(new Line(Color.Transparent, 0.50, "VeryOverbought"));
            Lines[4].Pen.DashStyle = DashStyle.Dash;  // Changed by TheWizard December 10, 2009
			Lines[4].Pen.Width = 2;
            fbank = new Dictionary<int, double[]>();

            _vxAvg = new DataSeries(this);
            _vxTrend = new DataSeries(this);
            _vx = new DataSeries(this);
            _lSigma = new DataSeries(this);
            _uSigma = new DataSeries(this);
            _zlr = new DataSeries(this);

            penUp.DashStyle = DashStyle.Dot;
            penDown.DashStyle = DashStyle.Dot;
            penTurn.DashStyle = DashStyle.Dot;

            stringFormat.Alignment = StringAlignment.Far;

            Overlay = false;
            PriceTypeSupported = true;
            DrawOnPricePanel = true;
        }
        private void setWeights()
        {
            //permissivity adjusts weight of trend component in prediction step
            _permissivity = Math.Max(.1, Math.Min(0.01 * Phase + 1.5, 3.0));

            _windowLength = 0.5 * (Period - 1);
            _lengthParam = Math.Max(0, Math.Log(Math.Sqrt(_windowLength)) / Math.Log(2.0) + 2);
            _expFactor = Math.Max(.5, _lengthParam - 2);
            _windowLength *= .9;

            double k = Math.Sqrt(_windowLength) * _lengthParam;
            _beta = k / (k + 1);
        }
        protected override void OnBarUpdate()
        {
            if (CurrentBar < 2)
            {
                _uSigma[0] = Input[0];
                _lSigma[0] = Input[0];
                return;
            }

            setWeights();

            double input = Input[0];//(CurrentBar > 0) ? Value[1] : Input[0];//SMA(Input, 1)[0];

            double uDelta = input - _uSigma[1];
            double lDelta = input - _lSigma[1];
            double uAbs = Math.Abs(uDelta);
            double lAbs = Math.Abs(lDelta);

            if (uAbs > lAbs) _vx[0] = uAbs;
            if (uAbs < lAbs) _vx[0] = lAbs;
            if (uAbs == lAbs) _vx[0] = 0;


            _vxAvg[0] = _vxAvg[1] + 0.1 * (_vx[0] - _vx[Math.Min(CurrentBar, 10)]);

            _vxTrend[0] = SMA(_vxAvg, Math.Min(CurrentBar, 65))[0];
            if (CurrentBar <= 64)
                _vxTrend[0] = _vxTrend[1] + 2 * (_vxAvg[0] - _vxTrend[1]) / (65);

            double vxCoeff = 0d;
            if (_vxTrend[0] > 0) vxCoeff = _vx[0] / _vxTrend[0];
            vxCoeff = Math.Min(Math.Pow(_lengthParam, 1 / _expFactor), vxCoeff);
            if (vxCoeff < 1) vxCoeff = 1.0;

            double vExp = Math.Pow(vxCoeff, _expFactor);
            double kV = Math.Pow(_beta, Math.Sqrt(vExp));

            double gamma = _windowLength / (_windowLength + 2);
            double alpha = Math.Pow(gamma, vExp);

            _uSigma[0] = (uDelta > 0) ? input : input - kV * uDelta;
            _lSigma[0] = (lDelta < 0) ? input : input - kV * lDelta;


            int bar = CurrentBar;
            double[] bank = new double[5];
            fbank[bar] = bank;
            if (bar == 2)
            {
                bank[0] = input;
                bank[2] = input;
                bank[4] = input;
            }
            else
            {
                bank[0] = (1 - alpha) * input + alpha * fbank[bar - 1][0]; //level component
                bank[1] = (input - bank[0]) * (1 - gamma) + gamma * fbank[bar - 1][1]; //trend component
                bank[2] = bank[0] + _permissivity * bank[1]; //apriori prediction
                bank[3] = (bank[2] - fbank[bar - 1][4]) * Math.Pow((1 - alpha), 2) + Math.Pow(alpha, 2) * fbank[bar - 1][3]; //error (innovation)
                bank[4] = fbank[bar - 1][4] + bank[3]; //posteriori mean
            }

            RawTrend.Set(bank[1]);
            Prediction.Set(bank[3]);


            DrawDots();
            if (_drawZLR)
                CatchZLR();
			if (alertonrawtrend) // Added by TheWizard October 25, 2009
			{
				if(CrossAbove(RawTrend, 0, 1)) // Added by TheWizard October 25, 2009
				{uptrend = true; downtrend = false;
//					BackColor = Color.FromArgb(50,Color.DodgerBlue);  // Added by TheWizard December 10, 2009
					if (soundOn) 
					{
					if (FirstTickOfBar) Alert("Ready",NinjaTrader.Cbi.Priority.High,"Ready",longWavefilename,4,Color.Black,Color.Yellow);  // Added by Wizard October 25, 2009
					}
					if (showArrows) DrawArrowUp(CurrentBar.ToString(), true, 0, Low[0] - (TickSize*ArrowDisplacement), Color.DarkGreen);  // Added by Wizard October 25, 2009
				}
				else if (CrossBelow(RawTrend, 0, 1))  // Added by TheWizard October 25, 2009
				{downtrend = true; uptrend = false;
//					BackColor = Color.FromArgb(50,Color.DeepPink);  // Added by TheWizard December 10, 2009
					if (soundOn) 
					{
					if (FirstTickOfBar) Alert("Ready",NinjaTrader.Cbi.Priority.High,"Ready",shortWavefilename,4,Color.Black,Color.Yellow);  // Added by Wizard October 25, 2009
					}
					if (showArrows) DrawArrowDown(CurrentBar.ToString(), true, 0, High[0] + (TickSize*ArrowDisplacement), Color.DarkRed);  // Added by Wizard October 25, 2009
				}
			}		
			else
			{
				if(CrossAbove(Prediction, 0, 1))  // Added by TheWizard October 25, 2009
				{uptrend = true; downtrend = false;
					if (soundOn) 
					{
					if (FirstTickOfBar) Alert("Ready",NinjaTrader.Cbi.Priority.High,"Ready",longWavefilename,4,Color.Black,Color.Yellow);  // Added by Wizard October 25, 200
					}
					if (showArrows) DrawArrowUp(CurrentBar.ToString(), true, 0, Low[0] - (TickSize*ArrowDisplacement), Color.DarkGreen);  // Added by Wizard October 25, 2009
				}
				else if (CrossBelow(Prediction, 0, 1))  // Added by TheWizard October 25, 2009
				{downtrend = true; uptrend = false;
					if (soundOn)
					{
					if (FirstTickOfBar) Alert("Ready",NinjaTrader.Cbi.Priority.High,"Ready",shortWavefilename,4,Color.Black,Color.Yellow);  // Added by Wizard October 25, 2009
					}
					if (showArrows) DrawArrowDown(CurrentBar.ToString(), true, 0, High[0] + (TickSize*ArrowDisplacement), Color.DarkRed);  // Added by Wizard October 25, 2009
				}
			} 
		if (uptrend) BackColor = Color.FromArgb(opacity,BackGroundUpColor);
		if (downtrend) BackColor = Color.FromArgb(opacity,BackGroundDnColor);
		}

        private void CatchZLR()
        {
            if (DotL.ContainsValue(1) || DotU.ContainsValue(1)) return;
            if (CrossAbove(RawTrend, 0, 3) && RawTrend[0] < 0)
            {
//				BackColor = Color.FromArgb(50,Color.DodgerBlue);  // Added by TheWizard December 10, 2009
                _zlr.Set(-1);
				DrawTriangleDown("ZLRs" + CurrentBar, true, 0, High[0] + (TickSize*TriangleDisplacement), DownColor);
            }
            if (CrossBelow(RawTrend, 0, 3) && RawTrend[0] > 0)
            {
//				BackColor = Color.FromArgb(50,Color.DeepPink);  // Added by TheWizard December 10, 2009
                _zlr.Set(1);
				DrawTriangleUp("ZLRL" + CurrentBar, true, 0, Low[0] - (TickSize*TriangleDisplacement), UpColor);
            }
        }

        private void DrawDots()
        {
            if (RawTrend[0] < 0 && Prediction[0] < 0)
            {
//				BackColor = Color.FromArgb(50,Color.DeepPink);  // Added by TheWizard December 10, 2009
                if (Slope(RawTrend, 2, 0) < 0 && Slope(Prediction, 2, 0) < 0 && RawTrend[0] > Prediction[0]) DotWarn.Set(0);
                if (RawTrend[0] < Prediction[0])
                {
                    DotL.Set(0);
                }
                else
                {
                    DotU.Set(0);
                }
            }
            else if (RawTrend[0] > 0 && Prediction[0] > 0)
            {
//				BackColor = Color.FromArgb(50,Color.DodgerBlue);  // Added by TheWizard December 10, 2009
                if (Slope(RawTrend, 2, 0) > 0 && Slope(Prediction, 2, 0) > 0 && RawTrend[0] < Prediction[0]) DotWarn.Set(0);
                if (RawTrend[0] > Prediction[0])
                {
                    DotU.Set(0);
                }
                else
                {
                    DotL.Set(0);
                }
            }
        }

        public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
        {
             //Default plotting in base class.
            base.Plot(graphics, bounds, min, max);

            int rawSlope = 0;
            int predictSlope = 0;
            int sigSlope = 0;
            int BoxNumber = 4;

            int index = -1;
            int bars = CalculateOnBarClose ? (ChartControl.BarsPainted - 2) : (ChartControl.BarsPainted - 1);

            if (base.Bars != null && _drawInfo)
            {
                index = ((ChartControl.LastBarPainted - ChartControl.BarsPainted) - 1) + bars;
                if (ChartControl.ShowBarsRequired || ((index - base.Displacement) >= base.BarsRequired))
                    if (Value.ContainsValue(2) && index <= CurrentBar)
                    {
                        rawSlope = getSlope(RawTrend, index);
                        predictSlope = getSlope(Prediction, index);
                        
                        Util.DrawSlopeString(this, graphics, bounds, "Detrend", 0, rawSlope, 3);
                        Util.DrawSlopeString(this, graphics, bounds, "Predict", 1, predictSlope, 2);
                        Util.drawValueString(this, graphics, bounds, "ParticleBias", -1, getBias(index), 5);
                    }
            }

            drawColorLine(graphics, bounds, min, max, Prediction, penUp, penDown);

        }

        public void drawColorLine(Graphics graphics, Rectangle bounds, double min, double max, DataSeries series,
                                  Pen upPen, Pen downPen)
        {
            if (base.Bars != null)
            {
                int barPaintWidth = ChartControl.ChartStyle.GetBarPaintWidth(ChartControl.BarWidth);
                SmoothingMode smoothingMode = graphics.SmoothingMode;
                int x1 = -1;
                int y1 = -1;
                double val;
                double old = 0.0;
                double older = 0.0;
                Pen pen = null;
                GraphicsPath path = null;
                for (int i = 0;
                     i <= (Math.Min(ChartControl.BarsPainted, series.Count-ChartControl.FirstBarPainted)-1);
                     i++)
                {
                    int index = ((ChartControl.LastBarPainted-ChartControl.BarsPainted)+1)+i;
                    if (ChartControl.ShowBarsRequired || ((index-Displacement) >= BarsRequired))
                    {
                        val = series.Get(index);
                        if (!double.IsNaN(val))
                        {
                            int x2 = (((ChartControl.CanvasRight-ChartControl.BarMarginRight)-(barPaintWidth/2))-
                                      ((ChartControl.BarsPainted-1)*ChartControl.BarSpace))+(i*ChartControl.BarSpace);
                            int y2 = (bounds.Y+bounds.Height)-
                                     ((int) (((val-min)/ChartControl.MaxMinusMin(max, min))*bounds.Height));
                            if (x1 >= 0)
                            {
                                Pen pen2;
                                pen2 = (val > 0) ? upPen : downPen;
       
                                if (pen2 != pen)
                                {
                                    if (path != null)
                                    {
                                        graphics.DrawPath(pen, path);
                                    }
                                    path = new GraphicsPath();
                                    pen = pen2;
                                }
                                path.AddLine(x1, y1, x2, y2);
                            }
                            x1 = x2;
                            y1 = y2;
                            older = old;
                            old = val;
                        }
                        if (pen != null)
                        {
                            graphics.SmoothingMode = SmoothingMode.AntiAlias;
                            graphics.DrawPath(pen, path);
                            graphics.SmoothingMode = smoothingMode;
                        }

                    }
                }
            }
        }

        private int getSlope(DataSeries series, int index)
        {
            return Util.getSlope(this, series, index);
        }

        [Browsable(false), XmlIgnore()]
        public int Bias
        {
            get
            {
                return getBias(0);
            }
        }
		
        private int getBias(int index)
        {				
            int score = 0;
            score += (RawTrend[index] > 0) ? 1 : -1;
            score += (Prediction[index] > 0) ? 1 : -1;
            return score;	
        }

        [Browsable(false), XmlIgnore()]
        public int ThresholdCross
        {
            get
            {
                if (CrossAbove(RawTrend, 0, 1) || (RawTrend[0] > 0 && CrossAbove(RawTrend, Prediction, 1))) return 1;
                if (CrossBelow(RawTrend, 0, 1)) return -1;
                return 0;
            }
        }

        [Browsable(false), XmlIgnore()]
        public DataSeries RawTrend
        {
            get { return Values[0]; }
        }
        [Browsable(false), XmlIgnore()]
        public DataSeries Prediction
        {
            get { return Values[1]; }
        }
        [Browsable(false), XmlIgnore()]
        public DataSeries DotU
        {
            get { return Values[2]; }
        }
        [Browsable(false), XmlIgnore()]
        public DataSeries DotL
        {
            get { return Values[3]; }
        }
        [Browsable(false), XmlIgnore()]
        public DataSeries DotWarn
        {
            get { return Values[4]; }
        }

        [Browsable(false), XmlIgnore()]
        public DataSeries ZLR
        {
            get { return _zlr; }
        }


        [Description("")]
        [Category("Parameters")]
        public int Period
        {
            get { return _period; }
            set { _period = value; }
        }
		
        [Description("[-150,150]")]
        [Category("Parameters")]
        public double Phase
        {
            get { return _phase; }
            set { _phase = value; }
        }
  // Added the following lines - by TheWizard October 25, 2009		        
		[Description("Show Arrows?")]
        [Category("Visual")]
        [Gui.Design.DisplayName("02. Show Arrows?")]
        public bool ShowArrows
        {
            get { return showArrows; }
            set { showArrows = value; }
        }
		[Description("ArrowDisplacement")]
        [Category("Visual")]
        public int ArrowDisplacement
        {
            get { return _ArrowDisplacement; }
            set { _ArrowDisplacement = value; }
        }
		[Description("TriangleDisplacement")]
        [Category("Visual")]
        public int TriangleDisplacement
        {
            get { return _TriangleDisplacement; }
            set { _TriangleDisplacement = value; }
        }
		/// <summary>
        /// </summary>
		/// [XmlIgnore()]
		[Description("UpColor")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("04. Up Color")]
        public Color UpColor
        {
            get { return upColor; }
            set { upColor = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string upColorSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(upColor); }
            set { upColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
		/// <summary>
        /// </summary>
        [XmlIgnore()]
        [Description("DownColor")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("05. Down Color")]
        public Color DownColor
        {
            get { return downColor; }
            set { downColor = value; }
        }
		/// <summary>
        /// </summary>
        [Browsable(false)]
        public string downColorSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(downColor); }
            set { downColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
        [XmlIgnore()]
		[Description("Region Opacity")]
		[Category("Visual")]
		[Gui.Design.DisplayNameAttribute("Region Opacity")]
		public int Opacity
		{
			get { return opacity;}
			set {opacity = value;}
		}
		[XmlIgnore()]
        [Description("BackUPColor")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Background Up Color")]
        public Color BackGroundUpColor
        {
            get { return backgroundupcolor; }
            set { backgroundupcolor = value; }
        }
        [Browsable(false)]
        public string backgroundupcolorSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(backgroundupcolor); }
            set { backgroundupcolor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
		[XmlIgnore()]
        [Description("BackDNColor")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Background Dn Color")]
        public Color BackGroundDnColor
        {
            get { return backgrounddncolor; }
            set { backgrounddncolor = value; }
        }
        [Browsable(false)]
        public string backgrounddncolorSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(backgrounddncolor); }
            set { backgrounddncolor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
		[Category("Alerts")]
		[Description("Sound")]

		public bool Soundon
		{
			get { return soundOn; }
			set { soundOn = value; }
		}
		
		[Category("Alerts")]
		[Description("AlertOnRawTrend")]

		public bool AlertOnRawTrend
		{
			get { return alertonrawtrend; }
			set { alertonrawtrend = value; }
		}
		
		[Description("Sound file for long alert.")]
		[Category("Alerts")]
		public string LongWavefilefame
		{
			get { return longWavefilename; }
			set { longWavefilename = value; }
		}
		
		[Description("Sound file for short alert.")]
		[Category("Alerts")]
		public string ShortWavefileName
		{
			get { return shortWavefilename; }
			set { shortWavefilename = value; }
		}
  // END OF ADDITIONS by TheWizard October 25, 2009		
        [NinjaTrader.Gui.Design.DisplayName("Direction Down Color"), Editor(typeof(PenEditor), typeof(UITypeEditor)),
         XmlIgnore, TypeConverter(typeof(PenConverter)), VisualizationOnly,
         Description("Pen settings for when direction is down.")]
        [Category("Visual")]
        public Pen PenDown
        {
            get { return this.penDown; }
            set { this.penDown = value; }
        }

        [Browsable(false)]
        public SerializablePen PenDownSerialize
        {
            get { return SerializablePen.FromPen(this.PenDown); }
            set { this.PenDown = SerializablePen.ToPen(value); }
        }

        [TypeConverter(typeof(PenConverter)), Description("Pen settings for when direction is up."), XmlIgnore,
         NinjaTrader.Gui.Design.DisplayName("Direction Up Color"), VisualizationOnly,
         Editor(typeof(PenEditor), typeof(UITypeEditor))]
        [Category("Visual")]
        public Pen PenUp
        {
            get { return this.penUp; }
            set { this.penUp = value; }
        }

        [Browsable(false)]
        public SerializablePen PenUpSerialize
        {
            get { return SerializablePen.FromPen(this.PenUp); }
            set { this.PenUp = SerializablePen.ToPen(value); }
        }

        [Description("")]
        [Category("Visual")]
        public bool DrawInfo
        {
            get { return _drawInfo; }
            set { _drawInfo = value; }
        }

        [Description("")]
        [Category("Visual")]
        public bool DrawZLR
        {
            get { return _drawZLR; }
            set { _drawZLR = value; }
        }
    }
}
#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private d9ParticleOscillator_V2[] cached9ParticleOscillator_V2 = null;

        private static d9ParticleOscillator_V2 checkd9ParticleOscillator_V2 = new d9ParticleOscillator_V2();

        /// <summary>
        /// d9 Particle Oscillator_V2
        /// </summary>
        /// <returns></returns>
        public d9ParticleOscillator_V2 d9ParticleOscillator_V2(int period, double phase)
        {
            return d9ParticleOscillator_V2(Input, period, phase);
        }

        /// <summary>
        /// d9 Particle Oscillator_V2
        /// </summary>
        /// <returns></returns>
        public d9ParticleOscillator_V2 d9ParticleOscillator_V2(Data.IDataSeries input, int period, double phase)
        {
            if (cached9ParticleOscillator_V2 != null)
                for (int idx = 0; idx < cached9ParticleOscillator_V2.Length; idx++)
                    if (cached9ParticleOscillator_V2[idx].Period == period && Math.Abs(cached9ParticleOscillator_V2[idx].Phase - phase) <= double.Epsilon && cached9ParticleOscillator_V2[idx].EqualsInput(input))
                        return cached9ParticleOscillator_V2[idx];

            lock (checkd9ParticleOscillator_V2)
            {
                checkd9ParticleOscillator_V2.Period = period;
                period = checkd9ParticleOscillator_V2.Period;
                checkd9ParticleOscillator_V2.Phase = phase;
                phase = checkd9ParticleOscillator_V2.Phase;

                if (cached9ParticleOscillator_V2 != null)
                    for (int idx = 0; idx < cached9ParticleOscillator_V2.Length; idx++)
                        if (cached9ParticleOscillator_V2[idx].Period == period && Math.Abs(cached9ParticleOscillator_V2[idx].Phase - phase) <= double.Epsilon && cached9ParticleOscillator_V2[idx].EqualsInput(input))
                            return cached9ParticleOscillator_V2[idx];

                d9ParticleOscillator_V2 indicator = new d9ParticleOscillator_V2();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                indicator.Phase = phase;
                Indicators.Add(indicator);
                indicator.SetUp();

                d9ParticleOscillator_V2[] tmp = new d9ParticleOscillator_V2[cached9ParticleOscillator_V2 == null ? 1 : cached9ParticleOscillator_V2.Length + 1];
                if (cached9ParticleOscillator_V2 != null)
                    cached9ParticleOscillator_V2.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cached9ParticleOscillator_V2 = tmp;
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
        /// d9 Particle Oscillator_V2
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.d9ParticleOscillator_V2 d9ParticleOscillator_V2(int period, double phase)
        {
            return _indicator.d9ParticleOscillator_V2(Input, period, phase);
        }

        /// <summary>
        /// d9 Particle Oscillator_V2
        /// </summary>
        /// <returns></returns>
        public Indicator.d9ParticleOscillator_V2 d9ParticleOscillator_V2(Data.IDataSeries input, int period, double phase)
        {
            return _indicator.d9ParticleOscillator_V2(input, period, phase);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// d9 Particle Oscillator_V2
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.d9ParticleOscillator_V2 d9ParticleOscillator_V2(int period, double phase)
        {
            return _indicator.d9ParticleOscillator_V2(Input, period, phase);
        }

        /// <summary>
        /// d9 Particle Oscillator_V2
        /// </summary>
        /// <returns></returns>
        public Indicator.d9ParticleOscillator_V2 d9ParticleOscillator_V2(Data.IDataSeries input, int period, double phase)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.d9ParticleOscillator_V2(input, period, phase);
        }
    }
}
#endregion
