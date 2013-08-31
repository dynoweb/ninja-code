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
    [Description("d9 Particle Oscillator w/Vertical Line")]
    [Gui.Design.DisplayName(" d9 ParticleOscillator w/VertLine")]
    public class d9ParticleOscillatorWVertLineR : Indicator
    {
        private int _period = 7;
        private double _phase;
        private double _beta;
        private double _expFactor;
        private double _lengthParam;
        private double _windowLength;
        private double _permissivity;
        private Dictionary<int, double[]> fbank;
        private DataSeries _vxTrend;
        private DataSeries _uSigma;
        private DataSeries _lSigma;
        private DataSeries _vx;
        private DataSeries _vxAvg;
        private DataSeries _zlr;
		
		private bool drawVerticalLineOnPricePanel = true; // Added by TheWizard December 22, 2009
		private Color vlineUpColor = Color.Green; // added by TheWizard December 23, 2009
		private Color vlineDnColor = Color.Red; // added by TheWizard December 23, 2009
//		private Color vlineWarningColor = Color.Yellow; // added by TheWizard December 28, 2009
	

        private StringFormat stringFormat = new StringFormat();
        private SolidBrush textBrush = new SolidBrush(Color.Black);
        private System.Drawing.Font textFont = new Font("Arial", 8);
        private Color colorAboveSig = Color.White;
        private Color colorBelowSig = Color.DarkOrange;
        private Pen penDown = new Pen(Color.Red, 2f);
        private Pen penTurn = new Pen(Color.DimGray, 1f);
        private Pen penUp = new Pen(Color.Blue, 2f);
        private int _regionOpacity = 2;
        private bool _drawInfo = true;
        private bool _drawZLR = false;
		private bool soundOn = true;
		private string longWavefilename = "long.wav";
		private string shortWavefilename="short.wav";
		private int BarAtLastCross = 0;
		private int vlineWidth = 2; // added by TheWizard December 23, 2009



        protected override void Initialize()
        {
            Add(new Plot(Color.Black, "rawTrend")); //0
            
            Add(new Plot(Color.Transparent, "predictTrend")); //1
            Plots[1].PlotStyle = PlotStyle.Bar;
            Plots[1].Pen.DashStyle = DashStyle.Dot;
            
            Add(new Plot(Color.Transparent, "DotU")); //2
            Plots[2].Pen.DashStyle = DashStyle.Dot;
            Plots[2].PlotStyle = PlotStyle.Dot;

            Add(new Plot(Color.Transparent, "DotL")); //3
            Plots[3].Pen.DashStyle = DashStyle.Dot;
            Plots[3].PlotStyle = PlotStyle.Dot;

            Add(new Plot(Color.Yellow, "DotWarn")); //4
            Plots[4].Pen.DashStyle = DashStyle.Dot;
            Plots[4].PlotStyle = PlotStyle.Dot;
            Plots[4].Pen.Width = 2.0f;

            Add(new Plot(Color.Magenta, "rawTrendSlope")); //5
            Add(new Plot(Color.Cyan, "predictSlope")); //6
           
            Add(new Line(Color.Black, 0, "zeroLine"));
            Lines[0].Pen.DashStyle = DashStyle.Solid;


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
			CalculateOnBarClose = false;
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
			if(drawVerticalLineOnPricePanel)
			{
				if(CrossAbove(Prediction, 0, 1))
                {
					DrawVerticalLine("MyVerticalLine", 0, vlineUpColor,DashStyle.Dash,vlineWidth); // added by TheWizard December 23, 2009
                }
                else if (CrossBelow(Prediction, 0, 1))
                {
					DrawVerticalLine("MyVerticalLine", 0, vlineDnColor,DashStyle.Dash,vlineWidth); // added by TheWizard December 23, 2009
                }
//				if(CrossAbove(RawTrend,Prediction,1))
//				{
//					DrawVerticalLine("MyVerticalLine", 0,vlineWarningColor,DashStyle.Dash,vlineWidth); // added by TheWizard December 28, 2009
//				}
//				else if (CrossBelow(RawTrend,Prediction,1))
//				{
//					DrawVerticalLine("MyVerticalLine", 0,vlineWarningColor,DashStyle.Dash,vlineWidth); // added by TheWizard December 28, 2009
//				}
			}
			if (soundOn){
// ORIGINAL CODE				
//				if(CrossAbove(Prediction, 0, 1))
//				{
//					PlaySound(longWavefilename);
//					BarAtLastCross=CurrentBar;
//					Alert("Ready",NinjaTrader.Cbi.Priority.High,"Ready","long.wav",4,Color.Black,Color.Yellow);
//				}
//				else if (CrossBelow(Prediction, 0, 1))
//				{
//					PlaySound(shortWavefilename);
//					BarAtLastCross=CurrentBar;
//					Alert("Ready",NinjaTrader.Cbi.Priority.High,"Ready","short.wav",4,Color.Black,Color.Yellow);
//				}
//  DAVIDHP's MODIFIED CODE:
				if(CrossAbove(Prediction, 0, 1))
                {
                    if(FirstTickOfBar) // added by TheWizard December 23, 2009
					{
						Alert("Ready",NinjaTrader.Cbi.Priority.High,"Ready","long.wav",4,Color.Black,Color.Yellow);
					}
//copied the line below from ADXVMA_Alerts_Rjay & modified, slightly
                    if (_drawInfo)
					{
						DrawArrowUp(CurrentBar.ToString(),0, Low[1]-TickSize,Color.FromKnownColor(KnownColor.Blue));
                	}
				}
                else if (CrossBelow(Prediction, 0, 1))
                {
                    if(FirstTickOfBar) // added by TheWizard December 23, 2009
					{
						Alert("Ready",NinjaTrader.Cbi.Priority.High,"Ready","short.wav",4,Color.Black,Color.Yellow);
					}
//copied the line below from ADXVMA_Alerts_Rjay & modified, slightly
                    if (_drawInfo)
					{
						DrawArrowDown(CurrentBar.ToString(),0,High[1]+TickSize,Color.FromKnownColor(KnownColor.Red));
					}
                }
			}			
            
        }

        private void CatchZLR()
        {
            if (DotL.ContainsValue(1) || DotU.ContainsValue(1)) return;
            if (CrossAbove(RawTrend, 0, 3) && RawTrend[0] < 0)
            {
                _zlr.Set(-1);
                DrawTriangleDown("ZLRs" + CurrentBar, true, 0, 0 + TickSize, Color.Black);
                //DrawText("ZLR"+CurrentBar, true, "ZLRS", 0, RawTrend[1], 5, Color.Yellow, textFont, StringAlignment.Center, Color.Empty, Color.Empty, 1);
            }
            if (CrossBelow(RawTrend, 0, 3) && RawTrend[0] > 0)
            {
                _zlr.Set(1);
                DrawTriangleUp("ZLRL" + CurrentBar, true, 0, 0 - TickSize, Color.GreenYellow);
                //DrawText("ZLR" + CurrentBar, true, "ZLRL", 0, RawTrend[1], -5, Color.Yellow, textFont, StringAlignment.Center, Color.Empty, Color.Empty, 1);
            }
        }

        private void DrawDots()
        {
            if (RawTrend[0] < 0 && Prediction[0] < 0)
            {
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
		[Category("Alerts")]
		[Description("Sound")]

		public bool Soundon
		{
			get { return soundOn; }
			set { soundOn = value; }
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
 // The Following lines added by TheWizard December 22, 2009 
		[Description("Draw Vertical Line on Price Panel?")]
        [Category("Visual")]
        [Gui.Design.DisplayName("02. DrawVerticalLineOnPricePanel?")]
        public bool DrawVerticalLineOnPricePanel
        {
            get { return drawVerticalLineOnPricePanel; }
            set { drawVerticalLineOnPricePanel = value; }
        }
		[Description("VLineUpColor")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("03. VLineUpColor")]
        public Color VlineUpColor
        {
            get { return vlineUpColor; }
            set { vlineUpColor = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string vlineUpColorSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(vlineUpColor); }
            set { vlineUpColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
		[Description("VLineDNColor")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("04. VLineDnColor")]
        public Color VlineDnColor
        {
            get { return vlineDnColor; }
            set { vlineDnColor = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string vlineDnColorSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(vlineDnColor); }
            set { vlineDnColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
		[Description("VLineWidth")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("05. VLineWidth")]
		    public int VLineWidth
        {
            get { return vlineWidth; }
            set { vlineWidth = value; }
        }
//END OF TheWizard's additions
    }
}
#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private d9ParticleOscillatorWVertLineR[] cached9ParticleOscillatorWVertLineR = null;

        private static d9ParticleOscillatorWVertLineR checkd9ParticleOscillatorWVertLineR = new d9ParticleOscillatorWVertLineR();

        /// <summary>
        /// d9 Particle Oscillator w/Vertical Line
        /// </summary>
        /// <returns></returns>
        public d9ParticleOscillatorWVertLineR d9ParticleOscillatorWVertLineR(int period, double phase)
        {
            return d9ParticleOscillatorWVertLineR(Input, period, phase);
        }

        /// <summary>
        /// d9 Particle Oscillator w/Vertical Line
        /// </summary>
        /// <returns></returns>
        public d9ParticleOscillatorWVertLineR d9ParticleOscillatorWVertLineR(Data.IDataSeries input, int period, double phase)
        {
            if (cached9ParticleOscillatorWVertLineR != null)
                for (int idx = 0; idx < cached9ParticleOscillatorWVertLineR.Length; idx++)
                    if (cached9ParticleOscillatorWVertLineR[idx].Period == period && Math.Abs(cached9ParticleOscillatorWVertLineR[idx].Phase - phase) <= double.Epsilon && cached9ParticleOscillatorWVertLineR[idx].EqualsInput(input))
                        return cached9ParticleOscillatorWVertLineR[idx];

            lock (checkd9ParticleOscillatorWVertLineR)
            {
                checkd9ParticleOscillatorWVertLineR.Period = period;
                period = checkd9ParticleOscillatorWVertLineR.Period;
                checkd9ParticleOscillatorWVertLineR.Phase = phase;
                phase = checkd9ParticleOscillatorWVertLineR.Phase;

                if (cached9ParticleOscillatorWVertLineR != null)
                    for (int idx = 0; idx < cached9ParticleOscillatorWVertLineR.Length; idx++)
                        if (cached9ParticleOscillatorWVertLineR[idx].Period == period && Math.Abs(cached9ParticleOscillatorWVertLineR[idx].Phase - phase) <= double.Epsilon && cached9ParticleOscillatorWVertLineR[idx].EqualsInput(input))
                            return cached9ParticleOscillatorWVertLineR[idx];

                d9ParticleOscillatorWVertLineR indicator = new d9ParticleOscillatorWVertLineR();
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

                d9ParticleOscillatorWVertLineR[] tmp = new d9ParticleOscillatorWVertLineR[cached9ParticleOscillatorWVertLineR == null ? 1 : cached9ParticleOscillatorWVertLineR.Length + 1];
                if (cached9ParticleOscillatorWVertLineR != null)
                    cached9ParticleOscillatorWVertLineR.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cached9ParticleOscillatorWVertLineR = tmp;
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
        /// d9 Particle Oscillator w/Vertical Line
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.d9ParticleOscillatorWVertLineR d9ParticleOscillatorWVertLineR(int period, double phase)
        {
            return _indicator.d9ParticleOscillatorWVertLineR(Input, period, phase);
        }

        /// <summary>
        /// d9 Particle Oscillator w/Vertical Line
        /// </summary>
        /// <returns></returns>
        public Indicator.d9ParticleOscillatorWVertLineR d9ParticleOscillatorWVertLineR(Data.IDataSeries input, int period, double phase)
        {
            return _indicator.d9ParticleOscillatorWVertLineR(input, period, phase);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// d9 Particle Oscillator w/Vertical Line
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.d9ParticleOscillatorWVertLineR d9ParticleOscillatorWVertLineR(int period, double phase)
        {
            return _indicator.d9ParticleOscillatorWVertLineR(Input, period, phase);
        }

        /// <summary>
        /// d9 Particle Oscillator w/Vertical Line
        /// </summary>
        /// <returns></returns>
        public Indicator.d9ParticleOscillatorWVertLineR d9ParticleOscillatorWVertLineR(Data.IDataSeries input, int period, double phase)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.d9ParticleOscillatorWVertLineR(input, period, phase);
        }
    }
}
#endregion
