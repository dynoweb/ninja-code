using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.Design;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace NinjaTrader.Indicator
{
  [Description(".")]
  public class rcVolatilityBreakout : Indicator
  {
	#region variables
    private int length = 20;
    private double number_2_0 = 2.0;
    private double number_1_5 = 1.5;
    private int number_20 = 20;
    private int number_2 = 2;
    private int number_5 = 5;
    private List<IRectangle> list_IRectangle = new List<IRectangle>();
    private Dictionary<ILine, Color> dictionary1 = new Dictionary<ILine, Color>();
    private Dictionary<IText, Color> dictionary2 = new Dictionary<IText, Color>();
    private int[] intArray8_1 = new int[8];
    private int[] intArray8_2 = new int[8];
    [XmlIgnore]
    private Plot boxOutline = new Plot(new Pen(Color.Orchid, 1f), "boxOutline");
    private string email = "";
    private int barsLookback = 30;
    private bool extendedVisible = true;
    private bool showLines = true;
    [XmlIgnore]
    private Plot extendedHigh = new Plot(new Pen(Color.Green, 1f), "extendedHigh");
    private bool extendedHighVisible = true;
    [XmlIgnore]
    private Plot extendedLow = new Plot(new Pen(Color.Red, 1f), "extendedLow");
    private bool extendedLowVisible = true;
    [XmlIgnore]
    private Plot extendedMiddle = new Plot(new Pen(Color.Gray, 1f), "extendedMiddle");
    private int extendedMiddleLineLength = 6;
    private int extendedLineLength = 15;
    private Color boxAreaColor = Color.Blue;
    private int boxAreaOpacity = 1;
    private bool tickHeightVisible = true;
    private string tickHeightPosition = RIGHT;
    private Color tickHeightColor = Color.Black;
    private string fontSize = "8";
    private bool extendedLevel1Visible = true;
    private bool extendedLevel2Visible = true;
    private bool extendedLevel3Visible = true;
    private int extendedLevel1 = 50;
    private int extendedLevel2 = 100;
    private int extendedLevel3 = 200;
    [XmlIgnore]
    private Plot extendedLineLevel1 = new Plot(new Pen(Color.Gray, 1f), "extendedLineLevel1");
    [XmlIgnore]
    private Plot extendedLineLevel2 = new Plot(new Pen(Color.Gray, 1f), "extendedLineLevel2");
    [XmlIgnore]
    private Plot extendedLineLevel3 = new Plot(new Pen(Color.Gray, 1f), "extendedLineLevel3");
    private string soundHigh = "Alert1.wav";
    private string soundLow = "Alert1.wav";
    private string soundLevel1 = "Alert1.wav";
    private string soundLevel2 = "Alert1.wav";
    private string soundLevel3 = "Alert1.wav";
    private DataSeries dataSeries1;
    private DataSeries dataSeries2;
    private int barsSinceSqueeze;
    private Font font;
    private Color color1;
    private Color color2;
    private Image image;
    private int breakoutRecNumb = 0;
    private int startBarNumber = 0;
    private int endBarNumber = 0;
    private double squeezeHigh;
    private double squeezeLow;
    private IRectangle iRectangle;
    private ILine iLine;
    private IText iText;
    private int squeezeLength;
    private double squeezeMidPoint;
    private double squeezeRange;
    private int tempCurrentBar2;
    private bool extendedMiddleVisible = true;
    private bool multiAlert;
	private static string LEFT = "Left";
	private static string RIGHT = "Right";
	private static string BOTH = "Both";
	private static string DISABLED = "Disabled";
	#endregion

	protected override void Initialize()      
	{
		Add(new Plot(Color.Transparent, PlotStyle.Dot, "Squeeze"));  
		Add(new Plot(Color.Transparent, PlotStyle.Dot, "SqueezeOn")); 
		Add(new Plot(Color.Green, PlotStyle.Dot, "PMomentumUp"));
		Add(new Plot(Color.Red, PlotStyle.Dot, "PMomentumDown"));
		Add(new Plot(Color.Blue, PlotStyle.Dot, "NMomentumUp"));
		Add(new Plot(Color.Pink, PlotStyle.Dot, "NMomentumDown"));
		Add(new Plot(Color.Transparent, PlotStyle.Dot, "CurrentValue"));
		Add(new Plot(Color.Transparent, PlotStyle.Dot, "BarsSinceSqueeze"));
		Add(new Plot(Color.Transparent, PlotStyle.Dot, "SqueezeLength"));
		Add(new Plot(Color.Transparent, PlotStyle.Dot, "SqueezeHigh"));
		Add(new Plot(Color.Transparent, PlotStyle.Dot, "SqueezeLow"));
		
		dataSeries1 = new DataSeries((IndicatorBase) this);
		dataSeries2 = new DataSeries((IndicatorBase) this);
		
		boxOutline.Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
		
		CalculateOnBarClose = true;
		Overlay = true;
		PlotsConfigurable = true;	// Hides the plots from the indicator configuration diagram
	}
	
	protected override void OnBarUpdate()
	{
		// Are there enough bars
		if (CurrentBar < length) return;
		try {
			bool debug = false; //CurrentBar == 33;
			
		if (CurrentBar != 0)
		{
		
			double num1 = ATR(length)[0];
			double num2 = StdDev(Close, length)[0];
			double num3;
			
			if (number_1_5 * num1 == 0.0)
			{
				num3 = 1.0;
			}
			else
				num3 = number_2_0 * num2 / (number_1_5 * num1);
			
			if (num3 <= 1.0)
			{
				if (Squeeze[1] != 0.0)
				{
					startBarNumber = CurrentBar;
					squeezeHigh = High[0];
					squeezeLow = Low[0];
					++breakoutRecNumb;
					if (debug) Print(Time + " in != 0.0 - Squeeze[1] " + Squeeze[1] + "  breakoutRecNumb: " + breakoutRecNumb);
				}
				
				if (Squeeze[1] == 0.0)
				{
					//Print(Time + " in == 0.0 - Squeeze[1] " + Squeeze[1] + " ");
					if (High[0] > squeezeHigh)
					{
						squeezeHigh = High[0];
					}
					if (Low[0] < squeezeLow)
					{
						squeezeLow = Low[0];
					}
					
					endBarNumber = CurrentBar;
					
					if (startBarNumber != endBarNumber)
					{
						squeezeMidPoint = (squeezeHigh + squeezeLow) / 2.0;
						squeezeRange = squeezeHigh - squeezeLow;
						if (debug) Print("breakoutRecNumb: " + breakoutRecNumb + " startBarNumber: " + startBarNumber + " endBarNumber: " + endBarNumber + " squeezeHigh: " + squeezeHigh + " squeezeLow: " + squeezeLow );

						iRectangle = DrawRectangle("rectangle" + breakoutRecNumb, AutoScale, CurrentBar - startBarNumber, squeezeHigh, CurrentBar - endBarNumber, squeezeLow, boxOutline.Pen.Color, boxAreaColor, boxAreaOpacity);
						//iRectangle = DrawRectangle("r" +breakoutRecNumb, 5, squeezeHigh, 0, squeezeLow, Color.Green);

						if (debug) Print("307");
						if (iRectangle == null)
						{
							if (debug) Print("iRectangle is null");
						}
						else
							{
							iRectangle.Pen.DashStyle = boxOutline.Pen.DashStyle;
							iRectangle.Pen.Width = boxOutline.Pen.Width;
							}
						
					if (debug) Print("309");
						if (extendedHighVisible)
						{
							iLine = DrawLine("extendedHigh" + breakoutRecNumb, AutoScale, CurrentBar - endBarNumber, squeezeHigh, CurrentBar - endBarNumber - extendedLineLength, squeezeHigh, extendedHigh.Pen.Color, extendedHigh.Pen.DashStyle, (int) extendedHigh.Pen.Width);
							if (iLine != null)
							{
							if (!dictionary1.ContainsKey(iLine))
								dictionary1.Add(iLine, iLine.Pen.Color);
							if (!showLines)
							{
								iLine.Pen.Color = Color.Transparent;
							}
							}
						}
				if (debug) Print("51");
						
						if (extendedLowVisible)
						{
							iLine = DrawLine("extendedLow" + breakoutRecNumb, AutoScale, CurrentBar - endBarNumber, squeezeLow, CurrentBar - endBarNumber - extendedLineLength, squeezeLow, extendedLow.Pen.Color, extendedLow.Pen.DashStyle, (int) extendedLow.Pen.Width);
							if (iLine != null)
							{
							if (!dictionary1.ContainsKey(iLine))
							{
								dictionary1.Add(iLine, iLine.Pen.Color);
							}
							if (!showLines)
							{
								iLine.Pen.Color = Color.Transparent;
							}
							}
						}
						
						if (extendedMiddleVisible)
						{
							iLine = DrawLine("extendedMiddle" + breakoutRecNumb, AutoScale, CurrentBar - endBarNumber, squeezeMidPoint, CurrentBar - endBarNumber - extendedMiddleLineLength, squeezeMidPoint, extendedMiddle.Pen.Color, extendedMiddle.Pen.DashStyle, (int) extendedMiddle.Pen.Width);
							if (iLine != null)
							{
							if (!dictionary1.ContainsKey(iLine))
							{
								dictionary1.Add(iLine, iLine.Pen.Color);
							}
							if (!showLines)
							{
								iLine.Pen.Color = Color.Transparent;
							}
							}
						}
						
				if (debug) Print("32");
						if (extendedLevel1Visible)
						{
							double num4 = squeezeRange * (double) extendedLevel1 / 100.0;
							iLine = DrawLine("extendedLevel1High" + breakoutRecNumb, AutoScale, CurrentBar - endBarNumber, squeezeHigh + num4, CurrentBar - endBarNumber - extendedLineLength, squeezeHigh + num4, extendedLineLevel1.Pen.Color, extendedLineLevel1.Pen.DashStyle, (int) extendedLineLevel1.Pen.Width);
							if (iLine != null)
							{
							if (!dictionary1.ContainsKey(iLine))
							{
								dictionary1.Add(iLine, iLine.Pen.Color);
							}
							if (!showLines)
							{
								iLine.Pen.Color = Color.Transparent;
							}
							}
							iLine = DrawLine("extendedLevel1Low" + breakoutRecNumb, AutoScale, CurrentBar - endBarNumber, squeezeLow - num4, CurrentBar - endBarNumber - extendedLineLength, squeezeLow - num4, extendedLineLevel1.Pen.Color, extendedLineLevel1.Pen.DashStyle, (int) extendedLineLevel1.Pen.Width);
							if (iLine != null)
							{
							if (!dictionary1.ContainsKey(iLine))
							{
								dictionary1.Add(iLine, iLine.Pen.Color);
							}
							if (!showLines)
							{
								iLine.Pen.Color = Color.Transparent;
							}                            
							}
						}
					
						if (extendedLevel2Visible)
						{
							double num4 = squeezeRange * (double) extendedLevel2 / 100.0;
							iLine = DrawLine("extendedLevel2High" + breakoutRecNumb, AutoScale, CurrentBar - endBarNumber, squeezeHigh + num4, CurrentBar - endBarNumber - extendedLineLength, squeezeHigh + num4, extendedLineLevel2.Pen.Color, extendedLineLevel2.Pen.DashStyle, (int) extendedLineLevel2.Pen.Width);
							if (iLine != null)
							{
							if (!dictionary1.ContainsKey(iLine))
							{
								dictionary1.Add(iLine, iLine.Pen.Color);
							}
							if (!showLines)
							{
								iLine.Pen.Color = Color.Transparent;
							}
							}
							iLine = DrawLine("extendedLevel2Low" + breakoutRecNumb, AutoScale, CurrentBar - endBarNumber, squeezeLow - num4, CurrentBar - endBarNumber - extendedLineLength, squeezeLow - num4, extendedLineLevel2.Pen.Color, extendedLineLevel2.Pen.DashStyle, (int) extendedLineLevel2.Pen.Width);
							if (iLine != null)
							{
							if (!dictionary1.ContainsKey(iLine))
							{
								dictionary1.Add(iLine, iLine.Pen.Color);
							}
							if (!showLines)
							{
								iLine.Pen.Color = Color.Transparent;
							}                            
							}
						}
					
				if (debug) Print("33");
						if (extendedLevel3Visible)
						{
							double num4 = squeezeRange * (double) extendedLevel3 / 100.0;
							Plot plot = extendedLineLevel3;
							iLine = DrawLine("extendedLevel3High" + breakoutRecNumb, AutoScale, CurrentBar - endBarNumber, squeezeHigh + num4, CurrentBar - endBarNumber - extendedLineLength, squeezeHigh + num4, plot.Pen.Color, plot.Pen.DashStyle, (int) plot.Pen.Width);
							if (iLine != null)
							{
							if (!dictionary1.ContainsKey(iLine))
							{
								dictionary1.Add(iLine, iLine.Pen.Color);
							}
							if (!showLines)
							{
								iLine.Pen.Color = Color.Transparent;
							}
							}
							iLine = DrawLine("extendedLevel3Low" + breakoutRecNumb, AutoScale, CurrentBar - endBarNumber, squeezeLow - num4, CurrentBar - endBarNumber - extendedLineLength, squeezeLow - num4, plot.Pen.Color, plot.Pen.DashStyle, (int) plot.Pen.Width);
							if (iLine != null)
							{
							if (!dictionary1.ContainsKey(iLine))
							{
								dictionary1.Add(iLine, iLine.Pen.Color);
							}
							if (!showLines)
							{
								iLine.Pen.Color = Color.Transparent;
							}
							}
						}
					
						if (tickHeightVisible)
						{
							int num4 = Convert.ToInt32((squeezeHigh - squeezeLow) / TickSize);
							if (tickHeightPosition == RIGHT || tickHeightPosition == BOTH)
							{
								iText = DrawText("LText" + breakoutRecNumb, AutoScale, num4.ToString(), CurrentBar - endBarNumber, squeezeMidPoint, 0, tickHeightColor, font, StringAlignment.Near, Color.Transparent, Color.Transparent, 1);
							if (iLine != null)
							{
								if (!dictionary2.ContainsKey(iText))
								{
									dictionary2.Add(iText, iText.TextColor);
								}
								if (!extendedVisible)
								{
									iText.TextColor = Color.Transparent;
								}
							}
							}
							
							if (tickHeightPosition == LEFT || tickHeightPosition == BOTH)
							{
								iText = DrawText("RText" + breakoutRecNumb, AutoScale, num4.ToString(), CurrentBar - startBarNumber, squeezeMidPoint, 0, tickHeightColor, font, StringAlignment.Far, Color.Transparent, Color.Transparent, 1);
							if (iLine != null)
							{
								if (!dictionary2.ContainsKey(iText))
								{
									dictionary2.Add(iText, iText.TextColor);
								}
								if (!extendedVisible)
								{
									iText.TextColor = Color.Transparent;
								}
							}
							}
						}
				if (debug) Print("4");

						if (!extendedVisible)
						{
							color1 = ((IShape) iRectangle).AreaColor;
							color2 = ((IShape) iRectangle).Pen.Color;
							((IShape) iRectangle).AreaColor = Color.Transparent;
							((IShape) iRectangle).Pen.Color = Color.Transparent;
						}
							if (iLine != null)
							{
						if (!this.list_IRectangle.Contains(this.iRectangle))
						{
							list_IRectangle.Add(iRectangle);
						}
							}
					}
				}
				Squeeze.Set(0.0);
				SqueezeOn.Reset();
			}
			else
			{
				if (debug) Print("5");
				Squeeze.Reset();
				SqueezeOn.Set(0.0);
			}
		
			dataSeries2.Set(Input[0] - (DonchianChannel(Input, number_20).Mean[CurrentBar] + EMA(Input, number_20)[0]) / 2.0);
			dataSeries1.Set(LinReg((IDataSeries) dataSeries2, number_20)[0]);
			
			PMomentumUp.Set(0.0);
			PMomentumDown.Set(0.0);
			NMomentumUp.Set(0.0);
			NMomentumDown.Set(0.0);
			
			if (dataSeries1[0] > 0.0)
			{
				if (dataSeries1[0] > dataSeries1[1])
				{
					PMomentumUp.Set(dataSeries1[0]);
				}
				else
				{
					PMomentumDown.Set(dataSeries1[0]);
				}
			}
			else if (dataSeries1[0] > dataSeries1[1])
			{
				NMomentumUp.Set(dataSeries1[0]);
			}
			else
			{
				NMomentumDown.Set(dataSeries1[0]);
			}
		}
      				if (debug) Print("6");

		processLongAlert(soundHigh, 0, squeezeHigh, "squeezeHigh");
		processLongAlert(soundLevel1, 1, squeezeHigh + squeezeRange * (double) extendedLevel1 / 100.0, "extendedLevel1");
		processLongAlert(soundLevel2, 2, squeezeHigh + squeezeRange * (double) extendedLevel2 / 100.0, "extendedLevel2");
		processLongAlert(soundLevel3, 3, squeezeHigh + squeezeRange * (double) extendedLevel3 / 100.0, "extendedLevel3");
		processShortAlert(soundLow, 4, squeezeLow, "squeezeLow");
		processShortAlert(soundLevel1, 5, squeezeLow - squeezeRange * (double) extendedLevel1 / 100.0, "extendedLevel1");
		processShortAlert(soundLevel2, 6, squeezeLow - squeezeRange * (double) extendedLevel2 / 100.0, "extendedLevel2");
		processShortAlert(soundLevel3, 7, squeezeLow - squeezeRange * (double) extendedLevel3 / 100.0, "extendedLevel3");
			
		CurrentValue.Set(dataSeries1[0]);
			
		if (tempCurrentBar2 != CurrentBar)
		{
			tempCurrentBar2 = CurrentBar;
			if (SqueezeOn[0] == 0.0)
			{
				++barsSinceSqueeze;
			}
			else
				barsSinceSqueeze = 0;
			
			if (Squeeze[0] == 0.0)
			{
				++squeezeLength;
			}
			else
			{
				squeezeLength = 0;
			}
		}
				if (debug) Print("7");

		if (barsSinceSqueeze > barsLookback)
		{
			BarsSinceSqueeze.Set(0.0);
		}
		else
			BarsSinceSqueeze.Set((double) barsSinceSqueeze);
		
		SqueezeLength.Set((double) squeezeLength);
		SqueezeHigh.Set(squeezeHigh);
		SqueezeLow.Set(squeezeLow);
		} catch (Exception ex) 
			{
				Print("Exception thrown " + ex.Message);
				Print(ex.StackTrace);
			}

	}

	
	
	#region UnusedCode

    private void sendEmail(string message)
    {
//      if (this.email.Length <= 0)
//        return;
//	
//      this.SendMail("from rcVolatilityBreakout Indicator", this.email, this.get_Name() + ", ", message + (object) ", " + (string) (object) DateTime.Now + ", " + this.get_Instrument().get_MasterInstrument().get_Name() + ", " + (string) (object) Input[0]);
    }

    private void processShortAlert(string alertWaveFile, int cefea6044484c3257df671c52a39f09b2, double ca2bb1244f73ff9a012a77d9ca61f445d, string message)
    {
//      if (!(alertWaveFile != DISABLED))
//        return;
//      if (this.intArray8_1[cefea6044484c3257df671c52a39f09b2] == CurrentBar)
//      {
//          if (this.intArray8_2[cefea6044484c3257df671c52a39f09b2] == this.endBarNumber)
//            return;
//          if (Close[1] <= ca2bb1244f73ff9a012a77d9ca61f445d || Close[0] > ca2bb1244f73ff9a012a77d9ca61f445d)
//            return;
//          if (CurrentBar < this.endBarNumber || CurrentBar >= this.endBarNumber + this.extendedLineLength)
//            return;
//          if (!this.multiAlert)
//          {
//                this.intArray8_2[cefea6044484c3257df671c52a39f09b2] = this.endBarNumber;
//          }
//          this.intArray8_1[cefea6044484c3257df671c52a39f09b2] = CurrentBar;
//          this.sendEmail(message);
//          this.PlaySound(Core.get_InstallDir() +"sounds" + alertWaveFile);
//          return;
//      }
    }
	
    private void processLongAlert(string alertWaveFile, int cefea6044484c3257df671c52a39f09b2, double ca2bb1244f73ff9a012a77d9ca61f445d, string message)
    {
//      if (!(alertWaveFile != DISABLED))
//        return;
//	
//      if (this.intArray8_1[cefea6044484c3257df671c52a39f09b2] == CurrentBar || this.intArray8_2[cefea6044484c3257df671c52a39f09b2] == this.endBarNumber)
//          if (Close[1] >= ca2bb1244f73ff9a012a77d9ca61f445d || Close[0] < ca2bb1244f73ff9a012a77d9ca61f445d || CurrentBar < this.endBarNumber)
//            return;
//      if (CurrentBar >= this.endBarNumber + this.extendedLineLength)
//        return;
//      if (!this.multiAlert)
//      {
//            this.intArray8_2[cefea6044484c3257df671c52a39f09b2] = this.endBarNumber;
//      }
//      this.intArray8_1[cefea6044484c3257df671c52a39f09b2] = CurrentBar;
//      this.sendEmail(message);
//      this.PlaySound(Core.get_InstallDir() + "sounds" + alertWaveFile);
//      return;
    }
	#endregion

	#region SupportingClasses

	// class used to load wave files
    public class WaveConverter : TypeConverter
    {
      private static string[] stringArray;

      static WaveConverter()
      {
      }

      public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
      {
        return true;
      }

      public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
      {
        return true;
      }

		// returns a list of alert wav files
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			string[] array;
			if (stringArray == null)
			{
				string[] files = Directory.GetFiles(Core.InstallDir + "sounds", "*.wav"); //SearchOption.TopDirectoryOnly);
				StringCollection stringCollection = new StringCollection();
				for (int index = 0; index < files.Length; ++index)
					stringCollection.Add(Path.GetFileName(files[index]));
				array = new string[stringCollection.Count + 1];
				stringCollection.CopyTo(array, 1);
				array[0] = DISABLED;
				stringArray = array;
			}
			else
			array = stringArray;
			return new TypeConverter.StandardValuesCollection((ICollection) array);
		}
	}

    private class cb7fd7594b63cf924b3e645a1b96661fe : UITypeEditor
    {
      public override bool GetPaintValueSupported(ITypeDescriptorContext context)
      {
        return true;
      }

      public override void PaintValue(PaintValueEventArgs e)
      {
        if (e.Value == null)
          return;
		
        SolidBrush solidBrush = new SolidBrush(((Plot) e.Value).Pen.Color);
        e.Graphics.FillRectangle((Brush) solidBrush, e.Bounds);
        solidBrush.Dispose();
      }
	}

	// for possible positions for the height of the squeeze
    public class positionModeConverter : TypeConverter
    {
      public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
      {
        return true;
      }

      public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
      {
        return true;
      }

      public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
      {
        return new TypeConverter.StandardValuesCollection((ICollection) new string[4] { LEFT, RIGHT, BOTH, DISABLED });
      }
    }

	// used for font size 
    public class sizeConverter : TypeConverter
    {
      public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
      {
        return true;
      }

      public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
      {
        return true;
      }

      public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
      {
        return new TypeConverter.StandardValuesCollection((ICollection) new string[6] { "5","6","7","8","9","10" });
      }
    }
	#endregion
	
	#region Properties

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries Squeeze
    {
      get { return Values[0]; }
    }

    [Browsable(false)]
    [XmlIgnore]
    public DataSeries SqueezeOn
    {
      get { return Values[1]; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries PMomentumUp
    {
      get { return Values[2]; }
    }

    [Browsable(false)]
    [XmlIgnore]
    public DataSeries PMomentumDown
    {
      get { return Values[3]; }
    }

    [Browsable(false)]
    [XmlIgnore]
    public DataSeries NMomentumUp
    {
      get { return Values[4]; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries NMomentumDown
    {
      get { return Values[5]; }
    }

    [Browsable(false)]
    [XmlIgnore]
    public DataSeries CurrentValue
    {
      get { return Values[6]; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries BarsSinceSqueeze
    {
      get { return Values[7]; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries SqueezeLength
    {
      get { return Values[8]; }
    }

    [XmlIgnore]
    [Browsable(true)]
    public DataSeries SqueezeHigh
    {
      get { return Values[9]; }
    }

    [XmlIgnore]
    [Browsable(true)]
    public DataSeries SqueezeLow
    {
      get { return Values[10]; }
    }

//    [Editor(typeof (rcVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
//    [Category("Visual")]
//    [Description("")]
//    public Plot BoxOutline
//    {
//      get { return this.boxOutline; }
//      set { this.boxOutline = value; }
//    }
//
    [Description("Volatility Strength")]
    [GridCategory("Consolidation box")]
    public int Length
    {
      get { return this.length; }
      set { this.length = Math.Max(1, value); }
    }

//    [Description("")]
//    [Category("Alert")]
//    public string Email
//    {
//      get { return this.email; }
//      set { this.email = value; }
//    }
//
//    [Description("")]
//    [Category("Global")]
//    public int BarsLookback
//    {
//      get { return this.barsLookback; }
//      set { this.barsLookback = value; }
//    }
//
//    [Category("Global")]
//    [Description("ShowBoxes")]
//    public bool ExtendedVisible
//    {
//      get { return this.extendedVisible; }
//      set { this.extendedVisible = value; }
//    }
//
//    [Description("ShowLines")]
//    [Category("Global")]
//    public bool ShowLines
//    {
//      get { return this.showLines; }
//      set { this.showLines = value; }
//    }
//
//    [Category("Visual")]
//    [Editor(typeof (rcVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
//    [Description("")]
//    public Plot ExtendedHigh
//    {
//      get { return this.extendedHigh; }
//      set { this.extendedHigh = value; }
//    }
//
//    [Category("Settings")]
//    public bool ExtendedHighVisible
//    {
//      get { return this.extendedHighVisible; }
//      set { this.extendedHighVisible = value; }
//    }
//
//    [Editor(typeof (rcVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
//    [Category("Visual")]
//    [Description("")]
//    public Plot ExtendedLow
//    {
//      get { return this.extendedLow; }
//      set { this.extendedLow = value; }
//    }
//
//    [Category("Settings")]
//    public bool ExtendedLowVisible
//    {
//      get { return this.extendedLowVisible; }
//      set { this.extendedLowVisible = value; }
//    }
//
//    [Category("Visual")]
//    [Editor(typeof (rcVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
//    [Description("")]
//    public Plot ExtendedMiddle
//    {
//      get { return this.extendedMiddle; }
//      set { this.extendedMiddle = value; }
//    }
//
//    [Category("Settings")]
//    public bool ExtendedMiddleVisible
//    {
//      get { return this.extendedMiddleVisible; }
//      set { this.extendedMiddleVisible = value; }
//    }
//
//    [Category("Settings")]
//    [Description("")]
//    public int ExtendedMiddleLineLength
//    {
//      get { return this.extendedMiddleLineLength; }
//      set { this.extendedMiddleLineLength = value; }
//    }
//
//    [Description("")]
//    [Category("Settings")]
//    public int ExtendedLineLength
//    {
//      get { return this.extendedLineLength; }
//      set { this.extendedLineLength = value; }
//    }
//
//    [Description("Background color of the low volatility box")]
//    [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
//    [GridCategory("Visual")]
//    [Gui.Design.DisplayNameAttribute("Box Background")]
//    public Color BoxAreaColor
//    {
//        get { return boxAreaColor; }
//        set { boxAreaColor = value; }
//    }
//	
//    [Description("")]
//    [Category("Visual")]
//    public int BoxAreaOpacity
//    {
//      get { return this.boxAreaOpacity; }
//      set { this.boxAreaOpacity = value; }
//    }
//
//    [Category("Visual")]
//    [Description("TickHeightVisible")]
//    public bool TickHeightVisible
//    {
//      get { return this.tickHeightVisible; }
//      set { this.tickHeightVisible = value; }
//    }
//
//    [Category("Visual")]
//    [TypeConverter(typeof (rcVolatilityBreakout.positionModeConverter))]
//    [Description("TickHeightPosition")]
//    public string TickHeightPosition
//    {
//      get { return this.tickHeightPosition; }
//      set { this.tickHeightPosition = value; }
//    }
//
//    [Description("Font color of the low volatility box height")]
//    [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
//    [GridCategory("Visual")]
//    [Gui.Design.DisplayNameAttribute("Box height color")]
//    public Color TickHeightColor
//    {
//        get { return tickHeightColor; }
//        set { tickHeightColor = value; }
//    }
//	
//    [Description("FontSize")]
//    [Category("Settings")]
//    [TypeConverter(typeof (rcVolatilityBreakout.sizeConverter))]
//    public string FontSize
//    {
//      get { return this.fontSize; }
//      set { this.fontSize = value; }
//    }
//
//    [Category("Settings")]
//    public bool ExtendedLevel1Visible
//    {
//      get { return this.extendedLevel1Visible; }
//      set { this.extendedLevel1Visible = value; }
//    }
//
//    [Category("Settings")]
//    public bool ExtendedLevel2Visible
//    {
//      get { return this.extendedLevel2Visible; }
//      set { this.extendedLevel2Visible = value; }
//    }
//
//    [Category("Settings")]
//    public bool ExtendedLevel3Visible
//    {
//      get { return this.extendedLevel3Visible; }
//      set { this.extendedLevel3Visible = value; }
//    }
//
//    [Category("Settings")]
//    [Description("")]
//    public int ExtendedLevel1
//    {
//      get { return this.extendedLevel1; }
//      set { this.extendedLevel1 = value; }
//    }
//
//    [Category("Settings")]
//    [Description("")]
//    public int ExtendedLevel2
//    {
//      get { return this.extendedLevel2; }
//      set { this.extendedLevel2 = value; }
//    }
//
//    [Category("Settings")]
//    [Description("")]
//    public int ExtendedLevel3
//    {
//      get { return this.extendedLevel3; }
//      set { this.extendedLevel3 = value; }
//    }
//
//    [Category("Visual")]
//    [Editor(typeof (rcVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
//    [Description("")]
//    public Plot ExtendedLineLevel1
//    {
//      get { return this.extendedLineLevel1; }
//      set { this.extendedLineLevel1 = value; }
//    }
//
//    [Category("Visual")]
//    [Editor(typeof (rcVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
//    [Description("")]
//    public Plot ExtendedLineLevel2
//    {
//      get { return this.extendedLineLevel2; }
//      set { this.extendedLineLevel2 = value; }
//    }
//
//    [Editor(typeof (rcVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
//    [Description("")]
//    [Category("Visual")]
//    public Plot ExtendedLineLevel3
//    {
//      get { return this.extendedLineLevel3; }
//      set { this.extendedLineLevel3 = value; }
//    }
//
//    [Category("Alert")]
//    [TypeConverter(typeof (rcVolatilityBreakout.WaveConverter))]
//    [Description("")]
//    public string SoundHigh
//    {
//      get { return this.soundHigh; }
//      set { this.soundHigh = value; }
//    }
//
//    [Description("")]
//    [Category("Alert")]
//    [TypeConverter(typeof (rcVolatilityBreakout.WaveConverter))]
//    public string SoundLow
//    {
//      get { return this.soundLow; }
//      set { this.soundLow = value; }
//    }
//
//    [Description("")]
//    [TypeConverter(typeof (rcVolatilityBreakout.WaveConverter))]
//    [Category("Alert")]
//    public string SoundLevel1
//    {
//      get { return this.soundLevel1; }
//      set { this.soundLevel1 = value; }
//    }
//
//    [TypeConverter(typeof (rcVolatilityBreakout.WaveConverter))]
//    [Category("Alert")]
//    [Description("")]
//    public string SoundLevel2
//    {
//      get { return this.soundLevel2; }
//      set { this.soundLevel2 = value; }
//    }
//
//    [Description("")]
//    [TypeConverter(typeof (rcVolatilityBreakout.WaveConverter))]
//    [Category("Alert")]
//    public string SoundLevel3
//    {
//      get { return this.soundLevel3; }
//      set { this.soundLevel3 = value; }
//    }
//
//    [Description("")]
//    [Category("Alert")]
//    public bool MultiAlert
//    {
//      get { return this.multiAlert; }
//      set { this.multiAlert = value; }
//    }
	#endregion
  }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private rcVolatilityBreakout[] cachercVolatilityBreakout = null;

        private static rcVolatilityBreakout checkrcVolatilityBreakout = new rcVolatilityBreakout();

        /// <summary>
        /// .
        /// </summary>
        /// <returns></returns>
        public rcVolatilityBreakout rcVolatilityBreakout(int length)
        {
            return rcVolatilityBreakout(Input, length);
        }

        /// <summary>
        /// .
        /// </summary>
        /// <returns></returns>
        public rcVolatilityBreakout rcVolatilityBreakout(Data.IDataSeries input, int length)
        {
            if (cachercVolatilityBreakout != null)
                for (int idx = 0; idx < cachercVolatilityBreakout.Length; idx++)
                    if (cachercVolatilityBreakout[idx].Length == length && cachercVolatilityBreakout[idx].EqualsInput(input))
                        return cachercVolatilityBreakout[idx];

            lock (checkrcVolatilityBreakout)
            {
                checkrcVolatilityBreakout.Length = length;
                length = checkrcVolatilityBreakout.Length;

                if (cachercVolatilityBreakout != null)
                    for (int idx = 0; idx < cachercVolatilityBreakout.Length; idx++)
                        if (cachercVolatilityBreakout[idx].Length == length && cachercVolatilityBreakout[idx].EqualsInput(input))
                            return cachercVolatilityBreakout[idx];

                rcVolatilityBreakout indicator = new rcVolatilityBreakout();
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

                rcVolatilityBreakout[] tmp = new rcVolatilityBreakout[cachercVolatilityBreakout == null ? 1 : cachercVolatilityBreakout.Length + 1];
                if (cachercVolatilityBreakout != null)
                    cachercVolatilityBreakout.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachercVolatilityBreakout = tmp;
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
        /// .
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.rcVolatilityBreakout rcVolatilityBreakout(int length)
        {
            return _indicator.rcVolatilityBreakout(Input, length);
        }

        /// <summary>
        /// .
        /// </summary>
        /// <returns></returns>
        public Indicator.rcVolatilityBreakout rcVolatilityBreakout(Data.IDataSeries input, int length)
        {
            return _indicator.rcVolatilityBreakout(input, length);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// .
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.rcVolatilityBreakout rcVolatilityBreakout(int length)
        {
            return _indicator.rcVolatilityBreakout(Input, length);
        }

        /// <summary>
        /// .
        /// </summary>
        /// <returns></returns>
        public Indicator.rcVolatilityBreakout rcVolatilityBreakout(Data.IDataSeries input, int length)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.rcVolatilityBreakout(input, length);
        }
    }
}
#endregion
