#region Using declarations
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using PriceActionSwing.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    [Description("Swings")]
    public class PriceActionSwing : Indicator
    {
        #region Variables
        //#########################################################################################
        #region Parameters
        //=========================================================================================
        /// <summary>
        /// Represents the double top/-bottom strength for the swings.
        /// </summary>
        private int dtbStrength = 1;
        /// <summary>
        /// Represents the swing size for the swings. E.g. 1 = small and 5 = bigger swings.
        /// </summary>
        private double swingSize = 1;
        /// <summary>
        /// Represents the swing type for the swings.
        /// </summary>
        private SwingStyle swingType = SwingStyle.Gann;
        /// <summary>
        /// Indicates if high and low prices are used for the swing calculations or close values.
        /// </summary>
        private bool useCloseValues = false;
        //=========================================================================================
        #endregion

        #region Display
        //=========================================================================================
        /// <summary>
        /// Represents the swing length visualization type for the swings.
        /// </summary>
        private SwingLengthStyle swingLengthType = SwingLengthStyle.False;
        /// <summary>
        /// Represents the swing duration visualization type for the swings.
        /// </summary>
        private SwingDurationStyle swingDurationType = SwingDurationStyle.False;
        /// <summary>
        /// Indicates if the swing price is shown for the swings.
        /// </summary>
        private bool showSwingPrice = false;
        /// <summary>
        /// Indicates if the swing label is shown for the swings.
        /// </summary>
        private bool showSwingLabel = true;
        /// <summary>
        /// Indicates if the swing percentage in relation to the last swing is shown for the 
        /// swings.
        /// </summary>
        private bool showSwingPercent = false;
        /// <summary>
        /// Represents the swing time visualization type for the swings. 
        /// </summary>
        private SwingTimeStyle swingTimeType = SwingTimeStyle.False;
        /// <summary>
        /// Indicates if the swing volume is shown for the swings.
        /// </summary>
        private SwingVolumeStyle swingVolumeType = SwingVolumeStyle.False;
        //=========================================================================================
        #endregion

        #region Visualize swings
        //=========================================================================================
        /// <summary>
        /// Represents the swing visualization type for the swings.
        /// </summary>
        private VisualizationStyle visualizationType = VisualizationStyle.GannStyle;
        /// <summary>
        /// Indicates if AutoScale is used for the swings. 
        /// </summary>
        private bool useAutoScale = true;
        /// <summary>
        /// Represents the color of the zig-zag up lines for the swings.
        /// </summary>
        private Color zigZagColorUp = Color.LimeGreen;
        /// <summary>
        /// Represents the color of the zig-zag down lines for the swings.
        /// </summary>
        private Color zigZagColorDn = Color.OrangeRed;
        /// <summary>
        /// Represents the line style of the zig-zag lines for the swings.
        /// </summary>
        private DashStyle zigZagStyle = DashStyle.Solid;
        /// <summary>
        /// Represents the line width of the zig-zag lines for the swings.
        /// </summary>
        private int zigZagWidth = 2;
        /// <summary>
        /// Represents the color of the swing value output for higher highs for the swings.
        /// </summary>
        private Color textColorHigherHigh = Color.Green;
        /// <summary>
        /// Represents the color of the swing value output for lower highs for the swings.
        /// </summary>
        private Color textColorLowerHigh = Color.Red;
        /// <summary>
        /// Represents the color of the swing value output for double tops for the swings.
        /// </summary>
        private Color textColorDoubleTop = Color.Blue;
        /// <summary>
        /// Represents the color of the swing value output for higher lows for the swings.
        /// </summary>
        private Color textColorHigherLow = Color.Green;
        /// <summary>
        /// Represents the color of the swing value output for lower lows swings for the swings.
        /// </summary>
        private Color textColorLowerLow = Color.Red;
        /// <summary>
        /// Represents the color of the swing value output for double bottoms for the swings.
        /// </summary>
        private Color textColorDoubleBottom = Color.Blue;
        /// <summary>
        /// Represents the text font for the swing value output for the swings.
        /// </summary>
        private Font textFont = new Font("Courier", 12, FontStyle.Bold);
        /// <summary>
        /// Represents the text offset in pixel for the swing length for the swings.
        /// </summary>
        private int textOffsetLength = 15;
        /// <summary>
        /// Represents the text offset in pixel for the retracement value for the swings.
        /// </summary>
        private int textOffsetPercent = 90;
        /// <summary>
        /// Represents the text offset in pixel for the swing price for the swings.
        /// </summary>
        private int textOffsetPrice = 45;
        /// <summary>
        /// Represents the text offset in pixel for the swing labels for the swings.
        /// </summary>
        private int textOffsetLabel = 30;
        /// <summary>
        /// Represents the text offset in pixel for the swing time for the swings.
        /// </summary>
        private int textOffsetTime = 75;
        /// <summary>
        /// Represents the text offset in pixel for the swing volume for the swings.
        /// </summary>
        private int textOffsetVolume = 30;

        /// <summary>
        /// Indicates if the Gann swings are updated if the last swing high/low is broken. 
        /// </summary>
        private bool useBreakouts = true;
        /// <summary>
        /// Indicates if inside bars are ignored for the Gann swing calculation. If set to 
        /// true it is possible that between consecutive up/down bars are inside bars.
        /// </summary>
        private bool ignoreInsideBars = true;
        /// <summary>
        /// Represents the number of decimal places for the instrument
        /// </summary>
        private int decimalPlaces;
        //=========================================================================================
        #endregion

        #region Class objects and DataSeries
        //=========================================================================================
        /// <summary>
        /// Represents the properties for the swing.
        /// </summary>
        private SwingProperties swingProperties;
        /// <summary>
        /// Represents the values for the current swing.
        /// </summary>
        private SwingCurrent swingCurrent = new SwingCurrent();
        /// <summary>
        /// Represents the swing high values.
        /// </summary>
        private Swings swingHigh = new Swings();
        /// <summary>
        /// Represents the swing low values.
        /// </summary>
        private Swings swingLow = new Swings();
        /// <summary>
        /// Indicates if the swing direction changed form down to up swing for the swings.
        /// </summary>
        private BoolSeries upFlip;
        /// <summary>
        /// Indicates if the swing direction changed form up to down swing for the swings.
        /// </summary>
        private BoolSeries dnFlip;
        /// <summary>
        /// Represents a list of all up swings for the swings.
        /// </summary>
        private List<SwingStruct> swingHighs = new List<SwingStruct>();
        /// <summary>
        /// Represents a list of all down swings for the swings.
        /// </summary>
        private List<SwingStruct> swingLows = new List<SwingStruct>();
        //=========================================================================================
        #endregion
        //#########################################################################################
        #endregion

        #region Initialize()
        //#########################################################################################
        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy 
        /// method is called.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(new Pen(Color.Gold, 3), PlotStyle.Dot, "DoubleBottom"));
            Add(new Plot(new Pen(Color.Red, 3), PlotStyle.Dot, "LowerLow"));
            Add(new Plot(new Pen(Color.Green, 3), PlotStyle.Dot, "HigherLow"));

            Add(new Plot(new Pen(Color.Gold, 3), PlotStyle.Dot, "DoubleTop"));
            Add(new Plot(new Pen(Color.Red, 3), PlotStyle.Dot, "LowerHigh"));
            Add(new Plot(new Pen(Color.Green, 3), PlotStyle.Dot, "HigherHigh"));

            Add(new Plot(new Pen(Color.Blue, 2), PlotStyle.Square, "GannSwing"));

            AutoScale = true;
            useAutoScale = AutoScale;
            BarsRequired = 1;
            CalculateOnBarClose = false;
            Overlay = true;
            PriceTypeSupported = false;

            dnFlip = new BoolSeries(this);
            upFlip = new BoolSeries(this);
        }
        //#########################################################################################
        #endregion

        #region OnStartUp()
        //#########################################################################################
        /// <summary>
        /// This method is used to initialize any variables or resources. This method is called 
        /// only once immediately prior to the start of the script, but after the Initialize() 
        /// method.
        /// </summary>
        protected override void OnStartUp()
        {
            // Calculate decimal places
            decimal increment = Convert.ToDecimal(Instrument.MasterInstrument.TickSize);
            int incrementLength = increment.ToString().Length;
            decimalPlaces = 0;
            if (incrementLength == 1)
                decimalPlaces = 0;
            else if (incrementLength > 2)
                decimalPlaces = incrementLength - 2;

            if (swingType != SwingStyle.Percent)
            {
                swingSize = Convert.ToInt32(Math.Round(swingSize, 0,
                    MidpointRounding.AwayFromZero));
                if (swingSize <= 0)
                    swingSize = 1;
            }

            swingProperties = new SwingProperties(swingType, swingSize, dtbStrength,
                swingLengthType, swingDurationType, showSwingPrice, showSwingLabel,
                showSwingPercent, swingTimeType, swingVolumeType, visualizationType,
                useBreakouts, ignoreInsideBars, useAutoScale,
                zigZagColorUp, zigZagColorDn, zigZagStyle, zigZagWidth, textColorHigherHigh,
                textColorLowerHigh, textColorDoubleTop, textColorHigherLow,
                textColorLowerLow, textColorDoubleBottom, textFont, textOffsetLength, 
                textOffsetPercent, textOffsetPrice, textOffsetLabel, textOffsetTime, 
                textOffsetVolume, useCloseValues);
        }
        //#########################################################################################
        #endregion

        #region OnBarUpdate()
        //#########################################################################################
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Checks to ensure there are enough bars before beginning
            if (CurrentBars[BarsInProgress] <= BarsRequired 
                || CurrentBars[BarsInProgress] < swingSize)
                return;

            #region Swing calculation
            //=====================================================================================
            InitializeSwingCalculation(swingHigh, swingLow, swingCurrent, upFlip, swingHighs, 
                dnFlip, swingLows);

            switch (swingType)
	        {
                case SwingStyle.Standard:
                    CalculateSwingStandard(swingHigh, swingLow, swingCurrent, swingProperties,
                        upFlip, swingHighs, dnFlip, swingLows, decimalPlaces, useCloseValues,
                        DoubleBottom, LowerLow, HigherLow, DoubleTop, LowerHigh, HigherHigh, 
                        GannSwing);
                    break;
                case SwingStyle.Gann:
                    CalculateSwingGann(swingHigh, swingLow, swingCurrent, swingProperties, upFlip,
                        swingHighs, dnFlip, swingLows, decimalPlaces, DoubleBottom, LowerLow, 
                        HigherLow, DoubleTop, LowerHigh, HigherHigh, GannSwing);
                    break;
                case SwingStyle.Ticks:
                    CalculateSwingTicks(swingHigh, swingLow, swingCurrent, swingProperties,
                        upFlip, swingHighs, dnFlip, swingLows, decimalPlaces, useCloseValues,
                        DoubleBottom, LowerLow, HigherLow, DoubleTop, LowerHigh, HigherHigh,
                        GannSwing);
                    break;
                case SwingStyle.Percent:
                    CalculateSwingPercent(swingHigh, swingLow, swingCurrent, swingProperties,
                        upFlip, swingHighs, dnFlip, swingLows, decimalPlaces, useCloseValues,
                        DoubleBottom, LowerLow, HigherLow, DoubleTop, LowerHigh, HigherHigh,
                        GannSwing);
                    break;
            }
            //=====================================================================================
            #endregion
        }
        //#########################################################################################
        #endregion

        #region Properties
        //#########################################################################################
        #region Plots
        // Plots ==================================================================================
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries DoubleBottom
        {
            get { return Values[0]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LowerLow
        {
            get { return Values[1]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HigherLow
        {
            get { return Values[2]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries DoubleTop
        {
            get { return Values[3]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LowerHigh
        {
            get { return Values[4]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HigherHigh
        {
            get { return Values[5]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries GannSwing
        {
            get { return Values[6]; }
        }
        //=========================================================================================
        #endregion

        #region Parameters
        //=========================================================================================
        /// <summary>
        /// Represents the swing type for the swings.
        /// </summary>
        [GridCategory("Parameters")]
        [Description("Represents the swing type for the swings.")]
        [Gui.Design.DisplayName("01. Swing type")]
        public SwingStyle SwingType
        {
            get { return swingType; }
            set { swingType = value; }
        }
        /// <summary>
        /// Represents the swing size. e.g. 1 = small swings and 5 = bigger swings.
        /// </summary>
        [GridCategory("Parameters")]
        [Description("Represents the swing size. e.g. 1 = small swings and 5 = bigger swings.")]
        [Gui.Design.DisplayName("02. Swing size")]
        public double SwingSize
        {
            get { return swingSize; }
            set { swingSize = Math.Max(Double.MinValue, value); }
        }
        /// <summary>
        /// Represents the double top/-bottom strength.
        /// </summary>
        [GridCategory("Parameters")]
        [Description("Represents the double top/-bottom strength. Increase the value to get more DB/DT.")]
        [Gui.Design.DisplayName("03. Double top/-bottom strength")]
        public int DtbStrength
        {
            get { return dtbStrength; }
            set { dtbStrength = Math.Max(1, value); }
        }
        /// <summary>
        /// Indicates if high and low prices are used for the swing calculations or close values.
        /// </summary>
        [GridCategory("Parameters")]
        [Description("Indicates if high and low prices are used for the swing calculations or close values.")]
        [Gui.Design.DisplayName("04. Use close values")]
        public bool UseCloseValues
        {
            get { return useCloseValues; }
            set { useCloseValues = value; }
        }
        //=========================================================================================
        #endregion

        #region Gann Swings
        //=========================================================================================
        [Category("Gann Swings")]
        [Description("Indicates if inside bars are ignored. If set to true it is possible that between consecutive up/down bars are inside bars. Only used if calculationSize > 1.")]
        [Gui.Design.DisplayName("01. Ignore inside Bars")]
        public bool IgnoreInsideBars
        {
            get { return ignoreInsideBars; }
            set { ignoreInsideBars = value; }
        }
        [Category("Gann Swings")]
        [Description("Indicates if the swings are updated if the last swing high/low is broken. Only used if calculationSize > 1.")]
        [Gui.Design.DisplayName("02. Use breakouts")]
        public bool UseBreakouts
        {
            get { return useBreakouts; }
            set { useBreakouts = value; }
        }
        //=========================================================================================
        #endregion

        #region Swings values
        //=========================================================================================
        [Category("Swing values")]
        [Description("Represents the swing length visualization type for the swings.")]
        [Gui.Design.DisplayName("01. Length")]
        public SwingLengthStyle SwingLengthType
        {
            get { return swingLengthType; }
            set { swingLengthType = value; }
        }
        [Category("Swing values")]
        [Description("Represents the swing duration visualization type for the swings.")]
        [Gui.Design.DisplayName("02. Duration")]
        public SwingDurationStyle SwingDurationType
        {
            get { return swingDurationType; }
            set { swingDurationType = value; }
        }
        [Category("Swing values")]
        [Description("Indicates if the swing price is shown for the swings.")]
        [Gui.Design.DisplayName("03. Price")]
        public bool ShowSwingPrice
        {
            get { return showSwingPrice; }
            set { showSwingPrice = value; }
        }
        [Category("Swing values")]
        [Description("Indicates if the swing label is shown (HH, HL, LL, LH, DB, DT).")]
        [Gui.Design.DisplayName("04. Labels")]
        public bool ShowSwingLabel
        {
            get { return showSwingLabel; }
            set { showSwingLabel = value; }
        }
        [Category("Swing values")]
        [Description("Indicates if the swing percentage in relation to the last swing is shown.")]
        [Gui.Design.DisplayName("05. Percentage")]
        public bool ShowSwingPercent
        {
            get { return showSwingPercent; }
            set { showSwingPercent = value; }
        }
        [Category("Swing values")]
        [Description("Represents the swing time visualization type for the swings. ")]
        [Gui.Design.DisplayName("06. Time")]
        public SwingTimeStyle SwingTimeType
        {
            get { return swingTimeType; }
            set { swingTimeType = value; }
        }
        [Category("Swing values")]
        [Description("Represents the swing volume visualization type for the swings. ")]
        [Gui.Design.DisplayName("07. Volume")]
        public SwingVolumeStyle SwingVolumeType
        {
            get { return swingVolumeType; }
            set { swingVolumeType = value; }
        }
        //=========================================================================================
        #endregion

        #region Visualize swings
        //=========================================================================================
        [Category("Visualize swings")]
        [Description("Represents the swing visualization type for the swings.")]
        [Gui.Design.DisplayName("01. Visualization type")]
        public VisualizationStyle VisualizationType
        {
            get { return visualizationType; }
            set { visualizationType = value; }
        }
        [XmlIgnore()]
        [Category("Visualize swings")]
        [Description("Represents the text font for the swing value output.")]
        [Gui.Design.DisplayName("02. Text font")]
        public Font TextFont
        {
            get { return textFont; }
            set { textFont = value; }
        }
        [Browsable(false)]
        public string TextFontSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableFont.ToString(textFont); }
            set { textFont = NinjaTrader.Gui.Design.SerializableFont.FromString(value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the color of the swing value output for higher highs.")]
        [Gui.Design.DisplayName("03. Text color higher high")]
        public Color TextColorHigherHigh
        {
            get { return textColorHigherHigh; }
            set { textColorHigherHigh = value; }
        }
        [Browsable(false)]
        public string TextColorHigherHighSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(textColorHigherHigh); }
            set { textColorHigherHigh = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the color of the swing value output for lower highs.")]
        [Gui.Design.DisplayName("04. Text color lower high")]
        public Color TextColorLowerHigh
        {
            get { return textColorLowerHigh; }
            set { textColorLowerHigh = value; }
        }
        [Browsable(false)]
        public string TextColorLowerHighSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(textColorLowerHigh); }
            set { textColorLowerHigh = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the color of the swing value output for double tops.")]
        [Gui.Design.DisplayName("05. Text color double top")]
        public Color TextColorDoubleTop
        {
            get { return textColorDoubleTop; }
            set { textColorDoubleTop = value; }
        }
        [Browsable(false)]
        public string TextColorDoubleTopSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(textColorDoubleTop); }
            set { textColorDoubleTop = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the color of the swing value output for higher lows.")]
        [Gui.Design.DisplayName("06. Text color higher high")]
        public Color TextColorHigherLow
        {
            get { return textColorHigherLow; }
            set { textColorHigherLow = value; }
        }
        [Browsable(false)]
        public string TextColorHigherLowSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(textColorHigherLow); }
            set { textColorHigherLow = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the color of the swing value output for lower lows.")]
        [Gui.Design.DisplayName("07. Text color lower high")]
        public Color TextColorLowerLow
        {
            get { return textColorLowerLow; }
            set { textColorLowerLow = value; }
        }
        [Browsable(false)]
        public string TextColorLowerLowSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(textColorLowerLow); }
            set { textColorLowerLow = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the color of the swing value output for double bottoms.")]
        [Gui.Design.DisplayName("08. Text color double bottem")]
        public Color TextColorDoubleBottom
        {
            get { return textColorDoubleBottom; }
            set { textColorDoubleBottom = value; }
        }
        [Browsable(false)]
        public string TextColorDoubleBottomSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(textColorDoubleBottom); }
            set { textColorDoubleBottom = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the text offset in pixel for the swing length/duration.")]
        [Gui.Design.DisplayName("09. Text offset length / duration")]
        public int TextOffsetLength
        {
            get { return textOffsetLength; }
            set { textOffsetLength = Math.Max(1, value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the text offset in pixel for the swing volume.")]
        [Gui.Design.DisplayName("10. Text offset volume")]
        public int TextOffsetVolume
        {
            get { return textOffsetVolume; }
            set { textOffsetVolume = Math.Max(1, value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the text offset in pixel for the swing price for the swings.")]
        [Gui.Design.DisplayName("11. Text offset price")]
        public int TextOffsetPrice
        {
            get { return textOffsetPrice; }
            set { textOffsetPrice = Math.Max(1, value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the text offset in pixel for the swing labels.")]
        [Gui.Design.DisplayName("12. Text offset swing labels")]
        public int TextOffsetLabel
        {
            get { return textOffsetLabel; }
            set { textOffsetLabel = Math.Max(1, value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the text offset in pixel for the time value.")]
        [Gui.Design.DisplayName("13. Text offset time")]
        public int TextOffsetTime
        {
            get { return textOffsetTime; }
            set { textOffsetTime = Math.Max(1, value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the text offset in pixel for the retracement value.")]
        [Gui.Design.DisplayName("14. Text offset percent")]
        public int TextOffsetPercent
        {
            get { return textOffsetPercent; }
            set { textOffsetPercent = Math.Max(1, value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the color of the zig-zag up lines.")]
        [Gui.Design.DisplayName("15. Zig-Zag color up")]
        public Color ZigZagColorUp
        {
            get { return zigZagColorUp; }
            set { zigZagColorUp = value; }
        }
        [Browsable(false)]
        public string ZigZagColorUpSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(zigZagColorUp); }
            set { zigZagColorUp = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the color of the zig-zag down lines.")]
        [Gui.Design.DisplayName("16. Zig-Zag color down")]
        public Color ZigZagColorDn
        {
            get { return zigZagColorDn; }
            set { zigZagColorDn = value; }
        }
        [Browsable(false)]
        public string ZigZagColorDnSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(zigZagColorDn); }
            set { zigZagColorDn = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the line style of the zig-zag lines.")]
        [Gui.Design.DisplayName("17. Zig-Zag style")]
        public DashStyle ZigZagStyle
        {
            get { return zigZagStyle; }
            set { zigZagStyle = value; }
        }
        [Category("Visualize swings")]
        [Description("Represents the line width of the zig-zag lines.")]
        [Gui.Design.DisplayName("18. Zig-Zag width")]
        public int ZigZagWidth
        {
            get { return zigZagWidth; }
            set { zigZagWidth = Math.Max(1, value); }
        }
        //=====================================================================
        #endregion
        //#########################################################################################
        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private PriceActionSwing[] cachePriceActionSwing = null;

        private static PriceActionSwing checkPriceActionSwing = new PriceActionSwing();

        /// <summary>
        /// Swings
        /// </summary>
        /// <returns></returns>
        public PriceActionSwing PriceActionSwing(int dtbStrength, double swingSize, SwingStyle swingType, bool useCloseValues)
        {
            return PriceActionSwing(Input, dtbStrength, swingSize, swingType, useCloseValues);
        }

        /// <summary>
        /// Swings
        /// </summary>
        /// <returns></returns>
        public PriceActionSwing PriceActionSwing(Data.IDataSeries input, int dtbStrength, double swingSize, SwingStyle swingType, bool useCloseValues)
        {
            if (cachePriceActionSwing != null)
                for (int idx = 0; idx < cachePriceActionSwing.Length; idx++)
                    if (cachePriceActionSwing[idx].DtbStrength == dtbStrength && Math.Abs(cachePriceActionSwing[idx].SwingSize - swingSize) <= double.Epsilon && cachePriceActionSwing[idx].SwingType == swingType && cachePriceActionSwing[idx].UseCloseValues == useCloseValues && cachePriceActionSwing[idx].EqualsInput(input))
                        return cachePriceActionSwing[idx];

            lock (checkPriceActionSwing)
            {
                checkPriceActionSwing.DtbStrength = dtbStrength;
                dtbStrength = checkPriceActionSwing.DtbStrength;
                checkPriceActionSwing.SwingSize = swingSize;
                swingSize = checkPriceActionSwing.SwingSize;
                checkPriceActionSwing.SwingType = swingType;
                swingType = checkPriceActionSwing.SwingType;
                checkPriceActionSwing.UseCloseValues = useCloseValues;
                useCloseValues = checkPriceActionSwing.UseCloseValues;

                if (cachePriceActionSwing != null)
                    for (int idx = 0; idx < cachePriceActionSwing.Length; idx++)
                        if (cachePriceActionSwing[idx].DtbStrength == dtbStrength && Math.Abs(cachePriceActionSwing[idx].SwingSize - swingSize) <= double.Epsilon && cachePriceActionSwing[idx].SwingType == swingType && cachePriceActionSwing[idx].UseCloseValues == useCloseValues && cachePriceActionSwing[idx].EqualsInput(input))
                            return cachePriceActionSwing[idx];

                PriceActionSwing indicator = new PriceActionSwing();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.DtbStrength = dtbStrength;
                indicator.SwingSize = swingSize;
                indicator.SwingType = swingType;
                indicator.UseCloseValues = useCloseValues;
                Indicators.Add(indicator);
                indicator.SetUp();

                PriceActionSwing[] tmp = new PriceActionSwing[cachePriceActionSwing == null ? 1 : cachePriceActionSwing.Length + 1];
                if (cachePriceActionSwing != null)
                    cachePriceActionSwing.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachePriceActionSwing = tmp;
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
        /// Swings
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PriceActionSwing PriceActionSwing(int dtbStrength, double swingSize, SwingStyle swingType, bool useCloseValues)
        {
            return _indicator.PriceActionSwing(Input, dtbStrength, swingSize, swingType, useCloseValues);
        }

        /// <summary>
        /// Swings
        /// </summary>
        /// <returns></returns>
        public Indicator.PriceActionSwing PriceActionSwing(Data.IDataSeries input, int dtbStrength, double swingSize, SwingStyle swingType, bool useCloseValues)
        {
            return _indicator.PriceActionSwing(input, dtbStrength, swingSize, swingType, useCloseValues);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Swings
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PriceActionSwing PriceActionSwing(int dtbStrength, double swingSize, SwingStyle swingType, bool useCloseValues)
        {
            return _indicator.PriceActionSwing(Input, dtbStrength, swingSize, swingType, useCloseValues);
        }

        /// <summary>
        /// Swings
        /// </summary>
        /// <returns></returns>
        public Indicator.PriceActionSwing PriceActionSwing(Data.IDataSeries input, int dtbStrength, double swingSize, SwingStyle swingType, bool useCloseValues)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.PriceActionSwing(input, dtbStrength, swingSize, swingType, useCloseValues);
        }
    }
}
#endregion
