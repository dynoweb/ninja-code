#region Using declarations
using NinjaTrader.Data;
using PriceActionSwing.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
#endregion

namespace PriceActionSwing.Base
{
    #region public class SwingValues
    //=============================================================================================
    public class Swings
    {
        #region Current values
        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Represents the price of the current swing.
        /// </summary>
        public double CurPrice { get; set; }
        /// <summary>
        /// Represents the bar number of the highest/lowest bar of the current swing.
        /// </summary>
        public int CurBar { get; set; }
        /// <summary>
        /// Represents the duration as time values of the current swing.
        /// </summary>
        public DateTime CurDateTime { get; set; }
        /// <summary>
        /// Represents the duration in bars of the current swing.
        /// </summary>
        public int CurDuration { get; set; }
        /// <summary>
        /// Represents the swing length in ticks of the current swing.
        /// </summary>
        public int CurLength { get; set; }
        /// <summary>
        /// Represents the percentage in relation between the last swing and the current swing. 
        /// E. g. 61.8% fib retracement.
        /// </summary>
        public double CurPercent { get; set; }
        /// <summary>
        /// Represents the duration as integer in HHMMSS of the current swing.
        /// </summary>
        public int CurTime { get; set; }
        /// <summary>
        /// Represents the entire volume of the current swing.
        /// </summary>
        public long CurVolume { get; set; }
        /// <summary>
        /// Represents the relation to the previous swing.
        /// -1 = Lower High | 0 = Double Top | 1 = Higher High
        /// </summary>
        public int CurRelation { get; set; }
        //-----------------------------------------------------------------------------------------
        #endregion

        #region Last values
        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Represents the price of the last swing.
        /// </summary>
        public double LastPrice { get; set; }
        /// <summary>
        /// Represents the bar number of the highest/lowest bar of the last swing.
        /// </summary>
        public int LastBar { get; set; }
        /// <summary>
        /// Represents the duration as time values of the last swing.
        /// </summary>
        public DateTime LastDateTime { get; set; }
        /// <summary>
        /// Represents the duration in bars of the last swing.
        /// </summary>
        public int LastDuration { get; set; }
        /// <summary>
        /// Represents the swing length in ticks of the last swing.
        /// </summary>
        public int LastLength { get; set; }
        /// <summary>
        /// Represents the percentage in relation between the previous swing and the last swing. 
        /// E. g. 61.8% fib retracement.
        /// </summary>
        public double LastPercent { get; set; }
        /// <summary>
        /// Represents the duration as integer in HHMMSS of the last swing.
        /// </summary>
        public int LastTime { get; set; }
        /// <summary>
        /// Represents the entire volume of the last swing.
        /// </summary>
        public long LastVolume { get; set; }
        /// <summary>
        /// Represents the relation to the previous swing.
        /// -1 = Lower High | 0 = Double Top | 1 = Higher High
        /// </summary>
        public int LastRelation { get; set; }
        //-----------------------------------------------------------------------------------------
        #endregion

        #region Other values
        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Represents the number of swings.
        /// </summary>
        public int Counter { get; set; }
        /// <summary>
        /// Indicates if a new swing is found.
        /// </summary>
        public bool New { get; set; }
        /// <summary>
        /// Indicates if a the current swing is updated.
        /// </summary>
        public bool Update { get; set; }
        /// <summary>
        /// Represents the volume of the signal bar for the swing.
        /// </summary>
        public double SignalBarVolume { get; set; }
        /// <summary>
        /// Represents the number of the last swing in the swing list.
        /// </summary>
        public int ListCount { get; set; }
        //-----------------------------------------------------------------------------------------
        #endregion
    }
    //=============================================================================================
    #endregion

    #region public class CurrentSwing
    //=============================================================================================
    public class SwingCurrent
    {
        /// <summary>
        /// Represents the swing slope direction. -1 = down | 0 = init | 1 = up.
        /// </summary>
        public int SwingSlope { get; set; }
        /// <summary>
        /// Represents the bar number of the swing slope change bar.
        /// </summary>
        public int SwingSlopeChangeBar { get; set; }
        /// <summary>
        /// Indicates if a new swing is found. And whether it is a swing high or a swing low.
        /// Used to control, that either a swing high or a swing low is set for each bar.
        /// 0 = no swing | -1 = down swing | 1 = up swing
        /// </summary>
        public int NewSwing { get; set; }
        /// <summary>
        /// Represents the number of consecutives up/down bars.
        /// </summary>
        public int ConsecutiveBars { get; set; }
        /// <summary>
        /// Represents the bar number of the last bar which was counted to the 
        /// consecutives up/down bars.
        /// </summary>
        public int ConsecutiveBarNumber { get; set; }
        /// <summary>
        /// Represents the high/low of the last consecutive bar.
        /// </summary>
        public double ConsecutiveBarValue { get; set; }
        /// <summary>
        /// Indicates if the outside bar calculation is stopped. Used to avoid an up swing and 
        /// a down swing in one bar.
        /// </summary>
        public bool StopOutsideBarCalc { get; set; }
    }
    //=============================================================================================
    #endregion

    #region public class SwingProperties
    //=============================================================================================
    public class SwingProperties
    {
        public SwingProperties(double swingSize, int dtbStrength)
        {
            SwingSize = swingSize;
            DtbStrength = dtbStrength;
        }

        public SwingProperties(SwingStyle swingType, double swingSize, int dtbStrength,
            SwingLengthStyle swingLengthType, SwingDurationStyle swingDurationType, 
            bool showSwingPrice, bool showSwingLabel, bool showSwingPercent,
            SwingTimeStyle swingTimeType, SwingVolumeStyle swingVolumeType, 
            VisualizationStyle visualizationType, bool useBreakouts, bool ignoreInsideBars, 
            bool useAutoScale, Color zigZagColorUp, Color zigZagColorDn,
            DashStyle zigZagStyle, int zigZagWidth, Color textColorHigherHigh,
            Color textColorLowerHigh, Color textColorDoubleTop, Color textColorHigherLow,
            Color textColorLowerLow, Color textColorDoubleBottom, Font textFont, 
            int textOffsetLength, int textOffsetPercent, int textOffsetPrice, int textOffsetLabel,
            int textOffsetTime, int textOffsetVolume, bool useCloseValues)
        {
            SwingType = swingType;
            SwingSize = swingSize;
            DtbStrength = dtbStrength;
            SwingLengthType = swingLengthType;
            SwingDurationType = swingDurationType;
            ShowSwingPrice = showSwingPrice;
            ShowSwingLabel = showSwingLabel;
            ShowSwingPercent = showSwingPercent;
            SwingTimeType = swingTimeType;
            SwingVolumeType = swingVolumeType;
            VisualizationType = visualizationType;
            UseBreakouts = useBreakouts;
            IgnoreInsideBars = ignoreInsideBars;
            UseAutoScale = useAutoScale;
            ZigZagColorUp = zigZagColorUp;
            ZigZagColorDn = zigZagColorDn;
            ZigZagStyle = zigZagStyle;
            ZigZagWidth = zigZagWidth;
            TextColorHigherHigh = textColorHigherHigh;
            TextColorLowerHigh = textColorLowerHigh;
            TextColorDoubleTop = textColorDoubleTop;
            TextColorHigherLow = textColorHigherLow;
            TextColorLowerLow = textColorLowerLow;
            TextColorDoubleBottom = textColorDoubleBottom;
            TextFont = textFont;
            TextOffsetLength = textOffsetLength;
            TextOffsetPercent = textOffsetPercent;
            TextOffsetPrice = textOffsetPrice;
            TextOffsetLabel = textOffsetLabel;
            TextOffsetTime = textOffsetTime;
            TextOffsetVolume = textOffsetVolume;
            UseCloseValues = useCloseValues;
        }

        /// <summary>
        /// Represents the swing type.
        /// </summary>
        public SwingStyle SwingType { get; set; }
        /// <summary>
        /// Represents the swing size. e.g. 1 = small swings and 5 = bigger swings.
        /// </summary>
        public double SwingSize { get; set; }
        /// <summary>
        /// Represents the double top and double bottom strength.
        /// </summary>
        public int DtbStrength { get; set; }
        /// <summary>
        /// Represents the swing length visualization type.
        /// </summary>
        public SwingLengthStyle SwingLengthType { get; set; }
        /// <summary>
        /// Represents the swing duration visualization type.
        /// </summary>
        public SwingDurationStyle SwingDurationType { get; set; }
        /// <summary>
        /// Indicates if the swing price is shown.
        /// </summary>
        public bool ShowSwingPrice { get; set; }
        /// <summary>
        /// Indicates if the swing label is shown.
        /// </summary>
        public bool ShowSwingLabel { get; set; }
        /// <summary>
        /// Indicates if the swing percentage in relation to the last swing is shown.
        /// </summary>
        public bool ShowSwingPercent { get; set; }
        /// <summary>
        /// Represents the swing time visualization type.
        /// </summary>
        public SwingTimeStyle SwingTimeType { get; set; }
        /// <summary>
        /// Represents the swing volume visualization type.
        /// </summary>
        public SwingVolumeStyle SwingVolumeType { get; set; }
        /// <summary>
        /// Represents the swing visualization type. 
        /// </summary>
        public VisualizationStyle VisualizationType { get; set; }
        /// <summary>
        /// Indicates if the Gann swings are updated if the last swing high/low is broken.
        /// </summary>
        public bool UseBreakouts { get; set; }
        /// <summary>
        /// Indicates if inside bars are ignored for the Gann swing calculation. If set to true 
        /// it is possible that between consecutive up/down bars are inside bars.
        /// </summary>
        public bool IgnoreInsideBars { get; set; }
        /// <summary>
        /// Indicates if AutoScale is used. 
        /// </summary>
        public bool UseAutoScale { get; set; }
        /// <summary>
        /// Represents the colour of the zig-zag up lines.
        /// </summary>
        public Color ZigZagColorUp { get; set; }
        /// <summary>
        /// Represents the colour of the zig-zag down lines.
        /// </summary>
        public Color ZigZagColorDn { get; set; }
        /// <summary>
        /// Represents the line style of the zig-zag lines.
        /// </summary>
        public DashStyle ZigZagStyle { get; set; }
        /// <summary>
        /// Represents the line width of the zig-zag lines.
        /// </summary>
        public int ZigZagWidth { get; set; }
        /// <summary>
        /// Represents the colour of the swing value output for higher highs.
        /// </summary>
        public Color TextColorHigherHigh { get; set; }
        /// <summary>
        /// Represents the colour of the swing value output for lower highs.
        /// </summary>
        public Color TextColorLowerHigh { get; set; }
        /// <summary>
        /// Represents the colour of the swing value output for double tops.
        /// </summary>
        public Color TextColorDoubleTop { get; set; }
        /// <summary>
        /// Represents the colour of the swing value output for higher lows.
        /// </summary>
        public Color TextColorHigherLow { get; set; }
        /// <summary>
        /// Represents the colour of the swing value output for lower lows.
        /// </summary>
        public Color TextColorLowerLow { get; set; }
        /// <summary>
        /// Represents the colour of the swing value output for double bottems.
        /// </summary>
        public Color TextColorDoubleBottom { get; set; }
        /// <summary>
        /// Represents the text font for the swing value output.
        /// </summary>
        public Font TextFont { get; set; }
        /// <summary>
        /// Represents the text offset in pixel for the swing length.
        /// </summary>
        public int TextOffsetLength { get; set; }
        /// <summary>
        /// Represents the text offset in pixel for the retracement value.
        /// </summary>
        public int TextOffsetPercent { get; set; }
        /// <summary>
        /// Represents the text offset in pixel for the swing price.
        /// </summary>
        public int TextOffsetPrice { get; set; }
        /// <summary>
        /// Represents the text offset in pixel for the swing labels.
        /// </summary>
        public int TextOffsetLabel { get; set; }
        /// <summary>
        /// Represents the text offset in pixel for the swing time.
        /// </summary>
        public int TextOffsetTime { get; set; }
        /// <summary>
        /// Represents the text offset in pixel for the swing volume.
        /// </summary>
        public int TextOffsetVolume { get; set; }
        /// <summary>
        /// Indicates if high and low prices are used for the swing calculations or close values.
        /// </summary>
        public bool UseCloseValues { get; set; }
    }
    //=============================================================================================
    #endregion

    #region public struct SwingStruct
    //=============================================================================================
    public struct SwingStruct
    {
        /// <summary>
        /// Swing price.
        /// </summary>
        public double price;
        /// <summary>
        /// Swing bar number.
        /// </summary>
        public int barNumber;
        /// <summary>
        /// Swing time.
        /// </summary>
        public DateTime time;
        /// <summary>
        /// Swing duration in bars.
        /// </summary>
        public int duration;
        /// <summary>
        /// Swing length in ticks.
        /// </summary>
        public int length;
        /// <summary>
        /// Swing relation.
        /// -1 = Lower | 0 = Double | 1 = Higher
        /// </summary>
        public int relation;
        /// <summary>
        /// Swing volume.
        /// </summary>
        public long volume;

        public SwingStruct(double swingPrice, int swingBarNumber, DateTime swingTime,
                int swingDuration, int swingLength, int swingRelation, long swingVolume)
        {
            price = swingPrice;
            barNumber = swingBarNumber;
            time = swingTime;
            duration = swingDuration;
            length = swingLength;
            relation = swingRelation;
            volume = swingVolume;
        }
    }
    //=============================================================================================
    #endregion

    #region Enums
    //=============================================================================================
    public enum VisualizationStyle
    {
        False,
        Dots,
        Dots_ZigZag,
        ZigZag,
        ZigZagVolume,
        GannStyle,
    }

    public enum SwingStyle
    {
        Standard,
        Gann,
        Ticks,
        Percent,
    }

    public enum SwingLengthStyle
    {
        False,
        Ticks,
        Ticks_Price,
        Price_Ticks,
        Points,
        Points_Price,
        Price_Points,
        Price,
        Percent,
    }

    public enum SwingDurationStyle
    {
        False,
        Bars,
        MMSS,
        HHMM,
        SecondsTotal,
        MinutesTotal,
        HoursTotal,
        Days,
    }

    public enum SwingTimeStyle
    {
        False,
        Integer,
        HHMM,
        HHMMSS,
        DDMM,
    }
    
    public enum SwingVolumeStyle
    {
        False,
        Absolute,
        Relative,
    }

    public enum AbcPatternMode
    {
        False,
        Long_Short,
        Long,
        Short,
    }

    public enum StatisticPositionStyle
    {
        False,
        Bottom,
        Top,
    }

    public enum RiskManagementStyle
    {
        False,
        ToolStrip,
        Tab,
    }

    public enum DivergenceMode
    {
        Custom,
        False,
        GomCD,
        MACD,
        Stochastics,
    }
    public enum DivergenceDirection
    {
        Long,
        Long_Short,
        Short
    }
    public enum Show
    {
        Trend,
        Relation,
        Volume,
    }
    //=============================================================================================
    #endregion
}

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// This file holds all user defined indicator methods.
    /// </summary>
    partial class Indicator
    {
        #region Initialize swing calculation
        //#########################################################################################
        protected void InitializeSwingCalculation(Swings swingHigh, Swings swingLow,
            SwingCurrent swingCur, BoolSeries upFlip, List<SwingStruct> swingHighs,
            BoolSeries dnFlip, List<SwingStruct> swingLows)
        {
            if (FirstTickOfBar)
            {
                swingCur.StopOutsideBarCalc = false;

                // Initialize first swing
                if (swingHighs.Count == 0)
                {
                    swingHigh.CurBar = CurrentBars[BarsInProgress];
                    swingHigh.CurPrice = Highs[BarsInProgress][CurrentBars[BarsInProgress]];
                    swingHigh.CurDateTime = swingHigh.LastDateTime =
                        Times[BarsInProgress][CurrentBars[BarsInProgress]];
                    SwingStruct up = new SwingStruct(swingHigh.CurPrice, swingHigh.CurBar,
                        Times[BarsInProgress][CurrentBars[BarsInProgress] - 1], 0, 0, -1,
                        Convert.ToInt64(Volumes[BarsInProgress][0]));
                    swingHighs.Add(up);
                    swingHigh.ListCount = swingHighs.Count;
                }
                if (swingLows.Count == 0)
                {
                    swingLow.CurBar = CurrentBars[BarsInProgress];
                    swingLow.CurPrice = Lows[BarsInProgress][CurrentBars[BarsInProgress]];
                    swingLow.CurDateTime = swingLow.LastDateTime =
                        Times[BarsInProgress][CurrentBars[BarsInProgress]];
                    SwingStruct dn = new SwingStruct(swingLow.CurPrice, swingLow.CurBar,
                        Times[BarsInProgress][CurrentBars[BarsInProgress] - 1], 0, 0, -1,
                        Convert.ToInt64(Volumes[BarsInProgress][0]));
                    swingLows.Add(dn);
                    swingLow.ListCount = swingLows.Count;
                }
            }
            // Set new/update high/low back to false, to avoid function calls which depends on
            // them
            dnFlip.Set(false);
            upFlip.Set(false);
            swingHigh.New = swingLow.New = swingHigh.Update = swingLow.Update = false;
        }
        //#########################################################################################
        #endregion

        #region Calculate Swing Standard
        //#########################################################################################
        protected void CalculateSwingStandard(Swings swingHigh, Swings swingLow, 
            SwingCurrent swingCur, SwingProperties swingProp, BoolSeries upFlip,
            List<SwingStruct> swingHighs, BoolSeries dnFlip, List<SwingStruct> swingLows,
            int decimalPlaces, bool useCloseValues, DataSeries doubleBottom, DataSeries lowerLow,
            DataSeries higherLow, DataSeries doubleTop, DataSeries lowerHigh,
            DataSeries higherHigh, DataSeries gannSwing)
        {
            // Check if high and low values are used or only close values
            IDataSeries[] highs;
            IDataSeries[] lows;
            if (useCloseValues == true)
            {
                lows = Closes;
                highs = Closes;
            }
            else
            {
                lows = Lows;
                highs = Highs;
            }

            // For a new swing high in an uptrend, Highs[BarsInProgress][0] must be 
            // greater than the current swing high
            if (swingCur.SwingSlope == 1 && highs[BarsInProgress][0] <= swingHigh.CurPrice)
                swingHigh.New = false;
            else
                swingHigh.New = true;

            // For a new swing low in a downtrend, Lows[BarsInProgress][0] must be 
            // smaller than the current swing low
            if (swingCur.SwingSlope == -1 && lows[BarsInProgress][0] >= swingLow.CurPrice)
                swingLow.New = false;
            else
                swingLow.New = true;

            // CalculatOnBarClose == true
            if (CalculateOnBarClose)
            {
                // test if Highs[BarsInProgress][0] is higher than the last 
                // calculationSize highs = new swing high
                if (swingHigh.New)
                {
                    for (int i = 1; i < swingProp.SwingSize + 1; i++)
                    {
                        if (highs[BarsInProgress][0] <= highs[BarsInProgress][i])
                        {
                            swingHigh.New = false;
                            break;
                        }
                    }
                }
                // test if Lows[BarsInProgress][0] is lower than the last 
                // calculationSize lows = new swing low
                if (swingLow.New)
                {
                    for (int i = 1; i < swingProp.SwingSize + 1; i++)
                    {
                        if (lows[BarsInProgress][0] >= lows[BarsInProgress][i])
                        {
                            swingLow.New = false;
                            break;
                        }
                    }
                }

                // New swing high and new swing low
                if (swingHigh.New && swingLow.New)
                {
                    // Downtrend - ignore the swing high
                    if (swingCur.SwingSlope == -1)
                        swingHigh.New = false;
                    // Uptrend   - ignore the swing low
                    else
                        swingLow.New = false;
                }
            }
            // CalculatOnBarClose == false
            else
            {
                // Used to control, that only one swing is set for 
                // each bar
                if (FirstTickOfBar)
                    swingCur.NewSwing = 0;

                // No swing or an up swing is found
                if (swingCur.NewSwing != -1)
                {
                    // test if Highs[BarsInProgress][0] is higher than the last 
                    // calculationSize highs = new swing high
                    if (swingHigh.New)
                    {
                        for (int i = 1; i < swingProp.SwingSize + 1; i++)
                        {
                            if (highs[BarsInProgress][0] <= highs[BarsInProgress][i])
                            {
                                swingHigh.New = false;
                                break;
                            }
                        }
                        // Found a swing high
                        if (swingHigh.New)
                            swingCur.NewSwing = 1;
                    }
                }

                // No swing or an down swing is found
                if (swingCur.NewSwing != 1)
                {
                    // test if Lows[BarsInProgress][0] is lower than the last 
                    // calculationSize lows = new swing low
                    if (swingLow.New)
                    {
                        for (int i = 1; i < swingProp.SwingSize + 1; i++)
                        {
                            if (lows[BarsInProgress][0] >= lows[BarsInProgress][i])
                            {
                                swingLow.New = false;
                                break;
                            }
                        }
                        // Found a swing low
                        if (swingLow.New)
                            swingCur.NewSwing = -1;
                    }
                }

                // Set newLow back to false
                if (swingCur.NewSwing == 1)
                    swingLow.New = false;
                // Set newHigh back to false
                if (swingCur.NewSwing == -1)
                    swingHigh.New = false;
            }

            // Swing high
            if (swingHigh.New)
            {
                int bar;
                double price;
                // New swing high
                if (swingCur.SwingSlope != 1)
                {
                    bar = CurrentBars[BarsInProgress] -
                        HighestBar(highs[BarsInProgress], CurrentBars[BarsInProgress] -
                        swingLow.CurBar);
                    price = highs[BarsInProgress][HighestBar(highs[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingLow.CurBar)];
                    swingHigh.Update = false;
                }
                // Update swing high
                else
                {
                    bar = CurrentBars[BarsInProgress];
                    price = highs[BarsInProgress][0];
                    swingHigh.Update = true;
                }
                CalcUpSwing(bar, price, swingHigh.Update, swingHigh, swingLow, swingCur,
                    swingProp, upFlip, swingHighs, decimalPlaces, doubleBottom, lowerLow, 
                    higherLow, doubleTop, lowerHigh, higherHigh, gannSwing);
            }
            // Swing low
            else if (swingLow.New)
            {
                int bar;
                double price;
                // New swing low
                if (swingCur.SwingSlope != -1)
                {
                    bar = CurrentBars[BarsInProgress] - LowestBar(lows[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingHigh.CurBar);
                    price = lows[BarsInProgress][LowestBar(lows[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingHigh.CurBar)];
                    swingLow.Update = false;
                }
                // Update swing low
                else
                {
                    bar = CurrentBars[BarsInProgress];
                    price = lows[BarsInProgress][0];
                    swingLow.Update = true;
                }
                CalcDnSwing(bar, price, swingLow.Update, swingHigh, swingLow, swingCur,
                    swingProp, dnFlip, swingLows, decimalPlaces, doubleBottom, lowerLow, 
                    higherLow, doubleTop, lowerHigh, higherHigh, gannSwing);
            }            
        }
        //#########################################################################################
        #endregion

        #region Calculate Swing Gann
        //#########################################################################################
        protected void CalculateSwingGann(Swings swingHigh, Swings swingLow, SwingCurrent swingCur,
            SwingProperties swingProp, BoolSeries upFlip, List<SwingStruct> swingHighs,
            BoolSeries dnFlip, List<SwingStruct> swingLows, int decimalPlaces, 
            DataSeries doubleBottom, DataSeries lowerLow, DataSeries higherLow, 
            DataSeries doubleTop, DataSeries lowerHigh, DataSeries higherHigh, 
            DataSeries gannSwing)
        {
            #region Set bar property
            //=================================================================================
            // Represents the bar type. -1 = Down | 0 = Inside | 1 = Up | 2 = Outside
            int barType = 0;
            if (Highs[BarsInProgress][0] > Highs[BarsInProgress][1])
            {
                if (Lows[BarsInProgress][0] < Lows[BarsInProgress][1])
                    barType = 2;
                else
                    barType = 1;
            }
            else
            {
                if (Lows[BarsInProgress][0] < Lows[BarsInProgress][1])
                    barType = -1;
                else
                    barType = 0;
            }
            //=================================================================================
            #endregion

            #region Up swing
            //=================================================================================
            if (swingCur.SwingSlope == 1)
            {
                switch (barType)
                {
                    // Up bar
                    case 1:
                        swingCur.ConsecutiveBars = 0;
                        swingCur.ConsecutiveBarValue = 0.0;
                        if (Highs[BarsInProgress][0] > swingHigh.CurPrice)
                        {
                            swingHigh.New = true;
                            swingHigh.Update = true;
                            CalcUpSwing(CurrentBars[BarsInProgress],
                                Highs[BarsInProgress][0], swingHigh.Update, swingHigh,
                                swingLow, swingCur, swingProp, upFlip, swingHighs,
                                decimalPlaces, doubleBottom, lowerLow, higherLow, doubleTop, 
                                lowerHigh, higherHigh, gannSwing);
                            if ((swingCur.ConsecutiveBars + 1) == swingProp.SwingSize)
                                swingCur.StopOutsideBarCalc = true;
                        }
                        break;
                    // Down bar
                    case -1:
                        if (swingCur.ConsecutiveBarNumber != CurrentBars[BarsInProgress])
                        {
                            if (swingCur.ConsecutiveBarValue == 0.0)
                            {
                                swingCur.ConsecutiveBars++;
                                swingCur.ConsecutiveBarNumber = CurrentBars[BarsInProgress];
                                swingCur.ConsecutiveBarValue = Lows[BarsInProgress][0];
                            }
                            else if (Lows[BarsInProgress][0] < swingCur.ConsecutiveBarValue)
                            {
                                swingCur.ConsecutiveBars++;
                                swingCur.ConsecutiveBarNumber = CurrentBars[BarsInProgress];
                                swingCur.ConsecutiveBarValue = Lows[BarsInProgress][0];
                            }
                        }
                        else if (Lows[BarsInProgress][0] < swingCur.ConsecutiveBarValue)
                            swingCur.ConsecutiveBarValue = Lows[BarsInProgress][0];
                        if (swingCur.ConsecutiveBars == swingProp.SwingSize ||
                            (swingProp.UseBreakouts && Lows[BarsInProgress][0] <
                            swingLow.CurPrice))
                        {
                            swingCur.ConsecutiveBars = 0;
                            swingCur.ConsecutiveBarValue = 0.0;
                            swingLow.New = true;
                            swingLow.Update = false;
                            int bar = CurrentBars[BarsInProgress] -
                                LowestBar(Lows[BarsInProgress],
                                CurrentBars[BarsInProgress] - swingHigh.CurBar);
                            double price =
                                Lows[BarsInProgress][LowestBar(Lows[BarsInProgress],
                                CurrentBars[BarsInProgress] - swingHigh.CurBar)];
                            CalcDnSwing(bar, price, swingLow.Update, swingHigh, swingLow,
                                swingCur, swingProp, dnFlip, swingLows, decimalPlaces, 
                                doubleBottom, lowerLow, higherLow, doubleTop, lowerHigh, 
                                higherHigh, gannSwing);
                        }
                        break;
                    // Inside bar
                    case 0:
                        if (!swingProp.IgnoreInsideBars)
                        {
                            swingCur.ConsecutiveBars = 0;
                            swingCur.ConsecutiveBarValue = 0.0;
                        }
                        break;
                    // Outside bar
                    case 2:
                        if (Highs[BarsInProgress][0] > swingHigh.CurPrice)
                        {
                            swingHigh.New = true;
                            swingHigh.Update = true;
                            CalcUpSwing(CurrentBars[BarsInProgress],
                                Highs[BarsInProgress][0], swingHigh.Update, swingHigh,
                                swingLow, swingCur, swingProp, upFlip, swingHighs,
                                decimalPlaces, doubleBottom, lowerLow, higherLow, doubleTop,
                                lowerHigh, higherHigh, gannSwing);
                        }
                        else if (!swingCur.StopOutsideBarCalc)
                        {
                            if (swingCur.ConsecutiveBarNumber != CurrentBars[BarsInProgress])
                            {
                                if (swingCur.ConsecutiveBarValue == 0.0)
                                {
                                    swingCur.ConsecutiveBars++;
                                    swingCur.ConsecutiveBarNumber =
                                        CurrentBars[BarsInProgress];
                                    swingCur.ConsecutiveBarValue = Lows[BarsInProgress][0];
                                }
                                else if (Lows[BarsInProgress][0] <
                                    swingCur.ConsecutiveBarValue)
                                {
                                    swingCur.ConsecutiveBars++;
                                    swingCur.ConsecutiveBarNumber =
                                        CurrentBars[BarsInProgress];
                                    swingCur.ConsecutiveBarValue = Lows[BarsInProgress][0];
                                }
                            }
                            else if (Lows[BarsInProgress][0] < swingCur.ConsecutiveBarValue)
                                swingCur.ConsecutiveBarValue = Lows[BarsInProgress][0];
                            if (swingCur.ConsecutiveBars == swingProp.SwingSize ||
                                (swingProp.UseBreakouts && Lows[BarsInProgress][0] <
                                swingLow.CurPrice))
                            {
                                swingCur.ConsecutiveBars = 0;
                                swingCur.ConsecutiveBarValue = 0.0;
                                swingLow.New = true;
                                swingLow.Update = false;
                                int bar = CurrentBars[BarsInProgress] -
                                    LowestBar(Lows[BarsInProgress],
                                    CurrentBars[BarsInProgress] - swingHigh.CurBar);
                                double price =
                                    Lows[BarsInProgress][LowestBar(Lows[BarsInProgress],
                                    CurrentBars[BarsInProgress] - swingHigh.CurBar)];
                                CalcDnSwing(bar, price, swingLow.Update, swingHigh, swingLow,
                                    swingCur, swingProp, dnFlip, swingLows, decimalPlaces, 
                                    doubleBottom, lowerLow, higherLow, doubleTop, lowerHigh,
                                    higherHigh, gannSwing);
                            }
                        }
                        break;
                }
            }
            //=================================================================================
            #endregion

            #region Down swing
            //=================================================================================
            else
            {
                switch (barType)
                {
                    // Dwon bar
                    case -1:
                        swingCur.ConsecutiveBars = 0;
                        swingCur.ConsecutiveBarValue = 0.0;
                        if (Lows[BarsInProgress][0] < swingLow.CurPrice)
                        {
                            swingLow.New = true;
                            swingLow.Update = true;
                            CalcDnSwing(CurrentBars[BarsInProgress],
                                Lows[BarsInProgress][0], swingLow.Update, swingHigh,
                                swingLow, swingCur, swingProp, dnFlip, swingLows,
                                decimalPlaces, doubleBottom, lowerLow, higherLow, doubleTop,
                                lowerHigh, higherHigh, gannSwing);
                            if ((swingCur.ConsecutiveBars + 1) == swingProp.SwingSize)
                                swingCur.StopOutsideBarCalc = true;
                        }
                        break;
                    // Up bar
                    case 1:
                        if (swingCur.ConsecutiveBarNumber != CurrentBars[BarsInProgress])
                        {
                            if (swingCur.ConsecutiveBarValue == 0.0)
                            {
                                swingCur.ConsecutiveBars++;
                                swingCur.ConsecutiveBarNumber = CurrentBars[BarsInProgress];
                                swingCur.ConsecutiveBarValue = Highs[BarsInProgress][0];
                            }
                            else if (Highs[BarsInProgress][0] > swingCur.ConsecutiveBarValue)
                            {
                                swingCur.ConsecutiveBars++;
                                swingCur.ConsecutiveBarNumber = CurrentBars[BarsInProgress];
                                swingCur.ConsecutiveBarValue = Highs[BarsInProgress][0];
                            }
                        }
                        else if (Highs[BarsInProgress][0] > swingCur.ConsecutiveBarValue)
                            swingCur.ConsecutiveBarValue = Highs[BarsInProgress][0];
                        if (swingCur.ConsecutiveBars == swingProp.SwingSize ||
                            (swingProp.UseBreakouts && Highs[BarsInProgress][0] >
                            swingHigh.CurPrice))
                        {
                            swingCur.ConsecutiveBars = 0;
                            swingCur.ConsecutiveBarValue = 0.0;
                            swingHigh.New = true;
                            swingHigh.Update = false;
                            int bar = CurrentBars[BarsInProgress] -
                                HighestBar(Highs[BarsInProgress],
                                CurrentBars[BarsInProgress] - swingLow.CurBar);
                            double price =
                                Highs[BarsInProgress][HighestBar(Highs[BarsInProgress],
                                CurrentBars[BarsInProgress] - swingLow.CurBar)];
                            CalcUpSwing(bar, price, swingHigh.Update, swingHigh, swingLow,
                                swingCur, swingProp, upFlip, swingHighs, decimalPlaces, 
                                doubleBottom, lowerLow, higherLow, doubleTop, lowerHigh,
                                higherHigh, gannSwing);
                        }
                        break;
                    // Inside bar
                    case 0:
                        if (!swingProp.IgnoreInsideBars)
                        {
                            swingCur.ConsecutiveBars = 0;
                            swingCur.ConsecutiveBarValue = 0.0;
                        }
                        break;
                    // Outside bar
                    case 2:
                        if (Lows[BarsInProgress][0] < swingLow.CurPrice)
                        {
                            swingLow.New = true;
                            swingLow.Update = true;
                            CalcDnSwing(CurrentBars[BarsInProgress],
                                Lows[BarsInProgress][0], swingLow.Update, swingHigh,
                                swingLow, swingCur, swingProp, dnFlip, swingLows,
                                decimalPlaces, doubleBottom, lowerLow, higherLow, doubleTop,
                                lowerHigh, higherHigh, gannSwing);
                        }
                        else if (!swingCur.StopOutsideBarCalc)
                        {
                            if (swingCur.ConsecutiveBarNumber != CurrentBars[BarsInProgress])
                            {
                                if (swingCur.ConsecutiveBarValue == 0.0)
                                {
                                    swingCur.ConsecutiveBars++;
                                    swingCur.ConsecutiveBarNumber =
                                        CurrentBars[BarsInProgress];
                                    swingCur.ConsecutiveBarValue = Highs[BarsInProgress][0];
                                }
                                else if (Highs[BarsInProgress][0] >
                                    swingCur.ConsecutiveBarValue)
                                {
                                    swingCur.ConsecutiveBars++;
                                    swingCur.ConsecutiveBarNumber =
                                        CurrentBars[BarsInProgress];
                                    swingCur.ConsecutiveBarValue = Highs[BarsInProgress][0];
                                }
                            }
                            else if (Highs[BarsInProgress][0] > swingCur.ConsecutiveBarValue)
                                swingCur.ConsecutiveBarValue = Highs[BarsInProgress][0];
                            if (swingCur.ConsecutiveBars == swingProp.SwingSize ||
                                (swingProp.UseBreakouts && Highs[BarsInProgress][0] >
                                swingHigh.CurPrice))
                            {
                                swingCur.ConsecutiveBars = 0;
                                swingCur.ConsecutiveBarValue = 0.0;
                                swingHigh.New = true;
                                swingHigh.Update = false;
                                int bar = CurrentBars[BarsInProgress] -
                                    HighestBar(Highs[BarsInProgress],
                                    CurrentBars[BarsInProgress] - swingLow.CurBar);
                                double price =
                                    Highs[BarsInProgress][HighestBar(Highs[BarsInProgress],
                                    CurrentBars[BarsInProgress] - swingLow.CurBar)];
                                CalcUpSwing(bar, price, swingHigh.Update, swingHigh,
                                    swingLow, swingCur, swingProp, upFlip, swingHighs,
                                    decimalPlaces, doubleBottom, lowerLow, higherLow, 
                                    doubleTop, lowerHigh, higherHigh, gannSwing);
                            }
                        }
                        break;
                }
            }
            //=================================================================================
            #endregion
        }
        //#########################################################################################
        #endregion

        #region Calculate Swing Ticks
        //#########################################################################################
        protected void CalculateSwingTicks(Swings swingHigh, Swings swingLow, 
            SwingCurrent swingCur, SwingProperties swingProp, BoolSeries upFlip,
            List<SwingStruct> swingHighs, BoolSeries dnFlip, List<SwingStruct> swingLows,
            int decimalPlaces, bool useCloseValues, DataSeries doubleBottom, DataSeries lowerLow,
            DataSeries higherLow, DataSeries doubleTop, DataSeries lowerHigh,
            DataSeries higherHigh, DataSeries gannSwing)
        {
            // Check if high and low values are used or only close values
            IDataSeries[] highs;
            IDataSeries[] lows;
            if (useCloseValues == true)
            {
                lows = Closes;
                highs = Closes;
            }
            else
            {
                lows = Lows;
                highs = Highs;
            }

            // For a new swing high in an uptrend, Highs[BarsInProgress][0] must be 
            // greater than the current swing high
            if (swingCur.SwingSlope == 1 && highs[BarsInProgress][0] <= swingHigh.CurPrice)
                swingHigh.New = false;
            else
                swingHigh.New = true;

            // For a new swing low in a downtrend, Lows[BarsInProgress][0] must be 
            // smaller than the current swing low
            if (swingCur.SwingSlope == -1 && lows[BarsInProgress][0] >= swingLow.CurPrice)
                swingLow.New = false;
            else
                swingLow.New = true;

            // CalculatOnBarClose == true
            if (CalculateOnBarClose)
            {
                if (swingHigh.New 
                    && highs[BarsInProgress][0] < 
                    (swingLow.CurPrice + swingProp.SwingSize * TickSize))
                        swingHigh.New = false;

                if (swingLow.New
                    && lows[BarsInProgress][0] > 
                    (swingHigh.CurPrice - swingProp.SwingSize * TickSize))
                    swingLow.New = false;

                // New swing high and new swing low
                if (swingHigh.New && swingLow.New)
                {
                    // Downtrend - ignore the swing high
                    if (swingCur.SwingSlope == -1)
                        swingHigh.New = false;
                    // Uptrend   - ignore the swing low
                    else
                        swingLow.New = false;
                }
            }
            // CalculatOnBarClose == false
            else
            {
                // Used to control, that only one swing is set for 
                // each bar
                if (FirstTickOfBar)
                    swingCur.NewSwing = 0;

                // No swing or an up swing is found
                if (swingCur.NewSwing != -1)
                {
                    // test if Highs[BarsInProgress][0] is higher than the last 
                    // calculationSize highs = new swing high
                    if (swingHigh.New)
                    {
                        if (highs[BarsInProgress][0] < 
                            (swingLow.CurPrice + swingProp.SwingSize * TickSize))
                            swingHigh.New = false;
                        // Found a swing high
                        if (swingHigh.New)
                            swingCur.NewSwing = 1;
                    }
                }

                // No swing or an down swing is found
                if (swingCur.NewSwing != 1)
                {
                    // test if Lows[BarsInProgress][0] is lower than the last 
                    // calculationSize lows = new swing low
                    if (swingLow.New)
                    {
                        if (lows[BarsInProgress][0] > 
                            (swingHigh.CurPrice - swingProp.SwingSize * TickSize))
                            swingLow.New = false;
                        // Found a swing low
                        if (swingLow.New)
                            swingCur.NewSwing = -1;
                    }
                }

                // Set newLow back to false
                if (swingCur.NewSwing == 1)
                    swingLow.New = false;
                // Set newHigh back to false
                if (swingCur.NewSwing == -1)
                    swingHigh.New = false;
            }

            // Swing high
            if (swingHigh.New)
            {
                int bar;
                double price;
                // New swing high
                if (swingCur.SwingSlope != 1)
                {
                    bar = CurrentBars[BarsInProgress] -
                        HighestBar(highs[BarsInProgress], CurrentBars[BarsInProgress] -
                        swingLow.CurBar);
                    price = highs[BarsInProgress][HighestBar(highs[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingLow.CurBar)];
                    swingHigh.Update = false;
                }
                // Update swing high
                else
                {
                    bar = CurrentBars[BarsInProgress];
                    price = highs[BarsInProgress][0];
                    swingHigh.Update = true;
                }
                CalcUpSwing(bar, price, swingHigh.Update, swingHigh, swingLow, swingCur,
                    swingProp, upFlip, swingHighs, decimalPlaces, doubleBottom, lowerLow,
                    higherLow, doubleTop, lowerHigh, higherHigh, gannSwing);
            }
            // Swing low
            else if (swingLow.New)
            {
                int bar;
                double price;
                // New swing low
                if (swingCur.SwingSlope != -1)
                {
                    bar = CurrentBars[BarsInProgress] - LowestBar(lows[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingHigh.CurBar);
                    price = lows[BarsInProgress][LowestBar(lows[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingHigh.CurBar)];
                    swingLow.Update = false;
                }
                // Update swing low
                else
                {
                    bar = CurrentBars[BarsInProgress];
                    price = lows[BarsInProgress][0];
                    swingLow.Update = true;
                }
                CalcDnSwing(bar, price, swingLow.Update, swingHigh, swingLow, swingCur,
                    swingProp, dnFlip, swingLows, decimalPlaces, doubleBottom, lowerLow,
                    higherLow, doubleTop, lowerHigh, higherHigh, gannSwing);
            }
        }
        //#########################################################################################
        #endregion

        #region Calculate Swing Percent
        //#########################################################################################
        protected void CalculateSwingPercent(Swings swingHigh, Swings swingLow, 
            SwingCurrent swingCur, SwingProperties swingProp, BoolSeries upFlip, 
            List<SwingStruct> swingHighs, BoolSeries dnFlip, List<SwingStruct> swingLows,
            int decimalPlaces, bool useCloseValues, DataSeries doubleBottom, DataSeries lowerLow, 
            DataSeries higherLow, DataSeries doubleTop, DataSeries lowerHigh, 
            DataSeries higherHigh, DataSeries gannSwing)
        {
            // Check if high and low values are used or only close values
            IDataSeries[] highs;
            IDataSeries[] lows;
            if (useCloseValues == true)
            {
                lows = Closes;
                highs = Closes;
            }
            else
            {
                lows = Lows;
                highs = Highs;
            }

            // For a new swing high in an uptrend, Highs[BarsInProgress][0] must be 
            // greater than the current swing high
            if (swingCur.SwingSlope == 1 && highs[BarsInProgress][0] <= swingHigh.CurPrice)
                swingHigh.New = false;
            else
                swingHigh.New = true;

            // For a new swing low in a downtrend, Lows[BarsInProgress][0] must be 
            // smaller than the current swing low
            if (swingCur.SwingSlope == -1 && lows[BarsInProgress][0] >= swingLow.CurPrice)
                swingLow.New = false;
            else
                swingLow.New = true;

            // CalculatOnBarClose == true
            if (CalculateOnBarClose)
            {
                if (swingHigh.New
                    && highs[BarsInProgress][0] < 
                    (swingLow.CurPrice + swingLow.CurPrice / 100 * swingProp.SwingSize))
                    swingHigh.New = false;

                if (swingLow.New
                    && lows[BarsInProgress][0] > 
                    (swingHigh.CurPrice - swingLow.CurPrice / 100 * swingProp.SwingSize))
                    swingLow.New = false;

                // New swing high and new swing low
                if (swingHigh.New && swingLow.New)
                {
                    // Downtrend - ignore the swing high
                    if (swingCur.SwingSlope == -1)
                        swingHigh.New = false;
                    // Uptrend   - ignore the swing low
                    else
                        swingLow.New = false;
                }
            }
            // CalculatOnBarClose == false
            else
            {
                // Used to control, that only one swing is set for 
                // each bar
                if (FirstTickOfBar)
                    swingCur.NewSwing = 0;

                // No swing or an up swing is found
                if (swingCur.NewSwing != -1)
                {
                    // test if Highs[BarsInProgress][0] is higher than the last 
                    // calculationSize highs = new swing high
                    if (swingHigh.New)
                    {
                        if (highs[BarsInProgress][0] <
                            (swingLow.CurPrice + swingLow.CurPrice / 100 * swingProp.SwingSize))
                            swingHigh.New = false;
                        // Found a swing high
                        if (swingHigh.New)
                            swingCur.NewSwing = 1;
                    }
                }

                // No swing or an down swing is found
                if (swingCur.NewSwing != 1)
                {
                    // test if Lows[BarsInProgress][0] is lower than the last 
                    // calculationSize lows = new swing low
                    if (swingLow.New)
                    {
                        if (lows[BarsInProgress][0] >
                            (swingHigh.CurPrice - swingLow.CurPrice / 100 * swingProp.SwingSize))
                            swingLow.New = false;
                        // Found a swing low
                        if (swingLow.New)
                            swingCur.NewSwing = -1;
                    }
                }

                // Set newLow back to false
                if (swingCur.NewSwing == 1)
                    swingLow.New = false;
                // Set newHigh back to false
                if (swingCur.NewSwing == -1)
                    swingHigh.New = false;
            }

            // Swing high
            if (swingHigh.New)
            {
                int bar;
                double price;
                // New swing high
                if (swingCur.SwingSlope != 1)
                {
                    bar = CurrentBars[BarsInProgress] -
                        HighestBar(highs[BarsInProgress], CurrentBars[BarsInProgress] -
                        swingLow.CurBar);
                    price = highs[BarsInProgress][HighestBar(highs[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingLow.CurBar)];
                    swingHigh.Update = false;
                }
                // Update swing high
                else
                {
                    bar = CurrentBars[BarsInProgress];
                    price = highs[BarsInProgress][0];
                    swingHigh.Update = true;
                }
                CalcUpSwing(bar, price, swingHigh.Update, swingHigh, swingLow, swingCur,
                    swingProp, upFlip, swingHighs, decimalPlaces, doubleBottom, lowerLow,
                    higherLow, doubleTop, lowerHigh, higherHigh, gannSwing);
            }
            // Swing low
            else if (swingLow.New)
            {
                int bar;
                double price;
                // New swing low
                if (swingCur.SwingSlope != -1)
                {
                    bar = CurrentBars[BarsInProgress] - LowestBar(lows[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingHigh.CurBar);
                    price = lows[BarsInProgress][LowestBar(lows[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingHigh.CurBar)];
                    swingLow.Update = false;
                }
                // Update swing low
                else
                {
                    bar = CurrentBars[BarsInProgress];
                    price = lows[BarsInProgress][0];
                    swingLow.Update = true;
                }
                CalcDnSwing(bar, price, swingLow.Update, swingHigh, swingLow, swingCur,
                    swingProp, dnFlip, swingLows, decimalPlaces, doubleBottom, lowerLow,
                    higherLow, doubleTop, lowerHigh, higherHigh, gannSwing);
            }
        }
        //#########################################################################################
        #endregion

        #region Calculate down swing
        //#########################################################################################
        protected void CalcDnSwing(int bar, double low, bool updateLow, Swings swingHigh,
            Swings swingLow, SwingCurrent swingCur, SwingProperties swingProp, BoolSeries dnFlip,
            List<SwingStruct> swingLows, int decimalPlaces, DataSeries doubleBottom,
            DataSeries lowerLow, DataSeries higherLow, DataSeries doubleTop, DataSeries lowerHigh,
            DataSeries higherHigh, DataSeries gannSwing)
        {
            #region New and update Swing values
            //=====================================================================================
            if (!updateLow)
            {
                if (swingProp.VisualizationType == VisualizationStyle.GannStyle)
                {
                    for (int i = CurrentBar - swingCur.SwingSlopeChangeBar; i >= 0; i--)
                        gannSwing.Set(i, swingHigh.CurPrice);
                    gannSwing.Set(low);
                }
                swingLow.LastPrice = swingLow.CurPrice;
                swingLow.LastBar = swingLow.CurBar;
                swingLow.LastDateTime = swingLow.CurDateTime;
                swingLow.LastDuration = swingLow.CurDuration;
                swingLow.LastLength = swingLow.CurLength;
                swingLow.LastTime = swingLow.CurTime;
                swingLow.LastPercent = swingLow.CurPercent;
                swingLow.LastRelation = swingLow.CurRelation;
                swingLow.LastVolume = swingLow.CurVolume;
                swingLow.Counter++;
                swingCur.SwingSlope = -1;
                swingCur.SwingSlopeChangeBar = bar;
                dnFlip.Set(true);
            }
            else
            {
                if (swingProp.VisualizationType == VisualizationStyle.Dots
                    || swingProp.VisualizationType == VisualizationStyle.Dots_ZigZag)
                {
                    doubleBottom.Reset(CurrentBar - swingLow.CurBar);
                    higherLow.Reset(CurrentBar - swingLow.CurBar);
                    lowerLow.Reset(CurrentBar - swingLow.CurBar);
                }
                swingLows.RemoveAt(swingLows.Count - 1);
            }
            swingLow.CurBar = bar;
            swingLow.CurPrice = Math.Round(low, decimalPlaces, MidpointRounding.AwayFromZero);
            swingLow.CurTime = ToTime(Times[BarsInProgress][CurrentBars[BarsInProgress] -
                swingLow.CurBar]);
            swingLow.CurDateTime = Times[BarsInProgress][CurrentBars[BarsInProgress] -
                swingLow.CurBar];
            swingLow.CurLength = Convert.ToInt32(Math.Round((swingLow.CurPrice -
                swingHigh.CurPrice) / TickSize, 0, MidpointRounding.AwayFromZero));
            if (swingHigh.CurLength != 0)
                swingLow.CurPercent = Math.Round(100.0 / swingHigh.CurLength *
                    Math.Abs(swingLow.CurLength), 1);
            swingLow.CurDuration = swingLow.CurBar - swingHigh.CurBar;
            double dtbOffset = ATR(BarsArray[BarsInProgress], 14)[CurrentBars[BarsInProgress] -
                swingLow.CurBar] * swingProp.DtbStrength / 100;
            if (swingLow.CurPrice > swingLow.LastPrice - dtbOffset && swingLow.CurPrice <
                swingLow.LastPrice + dtbOffset)
                swingLow.CurRelation = 0;
            else if (swingLow.CurPrice < swingLow.LastPrice)
                swingLow.CurRelation = -1;
            else
                swingLow.CurRelation = 1;
            if (!CalculateOnBarClose)
                swingHigh.SignalBarVolume = Volumes[BarsInProgress][0];
            double swingVolume = 0.0;
            for (int i = 0; i < swingLow.CurDuration; i++)
                swingVolume = swingVolume + Volumes[BarsInProgress][i];
            if (!CalculateOnBarClose)
                swingVolume = swingVolume + (Volumes[BarsInProgress][CurrentBars[BarsInProgress] -
                    swingHigh.CurBar] - swingLow.SignalBarVolume);
            if (swingProp.SwingVolumeType == SwingVolumeStyle.Relative)
                swingVolume = Math.Round(swingVolume / swingLow.CurDuration, 0,
                    MidpointRounding.AwayFromZero);
            swingLow.CurVolume = Convert.ToInt64(swingVolume);
            //=====================================================================================
            #endregion

            #region Visualize swing
            //=====================================================================================
            switch (swingProp.VisualizationType)
            {
                case VisualizationStyle.False:
                    break;
                case VisualizationStyle.Dots:
                    switch (swingLow.CurRelation)
                    {
                        case 1:
                            higherLow.Set(CurrentBar - swingLow.CurBar, swingLow.CurPrice);
                            break;
                        case -1:
                            lowerLow.Set(CurrentBar - swingLow.CurBar, swingLow.CurPrice);
                            break;
                        case 0:
                            doubleBottom.Set(CurrentBar - swingLow.CurBar, swingLow.CurPrice);
                            break;
                    }
                    break;
                case VisualizationStyle.Dots_ZigZag:
                    switch (swingLow.CurRelation)
                    {
                        case 1:
                            higherLow.Set(CurrentBar - swingLow.CurBar, swingLow.CurPrice);
                            break;
                        case -1:
                            lowerLow.Set(CurrentBar - swingLow.CurBar, swingLow.CurPrice);
                            break;
                        case 0:
                            doubleBottom.Set(CurrentBar - swingLow.CurBar, swingLow.CurPrice);
                            break;
                    }
                    DrawLine("ZigZagDown" + swingLow.Counter,
                        swingProp.UseAutoScale, CurrentBar - swingHigh.CurBar, swingHigh.CurPrice,
                        CurrentBar - swingLow.CurBar, swingLow.CurPrice, swingProp.ZigZagColorDn,
                        swingProp.ZigZagStyle, swingProp.ZigZagWidth);
                    break;
                case VisualizationStyle.ZigZag:
                    DrawLine("ZigZagDown" + swingLow.Counter,
                        swingProp.UseAutoScale, CurrentBar - swingHigh.CurBar, swingHigh.CurPrice,
                        CurrentBar - swingLow.CurBar, swingLow.CurPrice, swingProp.ZigZagColorDn,
                        swingProp.ZigZagStyle, swingProp.ZigZagWidth);
                    break;
                case VisualizationStyle.GannStyle:
                    for (int i = CurrentBar - swingCur.SwingSlopeChangeBar; i >= 0; i--)
                        gannSwing.Set(i, swingLow.CurPrice);
                    break;
                case VisualizationStyle.ZigZagVolume:
                    if (swingLow.CurVolume > swingHigh.CurVolume)
                        DrawLine("ZigZagDown" + swingLow.Counter,
                            swingProp.UseAutoScale, CurrentBar - swingHigh.CurBar, 
                            swingHigh.CurPrice, CurrentBar - swingLow.CurBar, swingLow.CurPrice, 
                            swingProp.ZigZagColorDn, swingProp.ZigZagStyle, swingProp.ZigZagWidth);
                    else
                        DrawLine("ZigZagDown" + swingLow.Counter,
                            swingProp.UseAutoScale, CurrentBar - swingHigh.CurBar,
                            swingHigh.CurPrice, CurrentBar - swingLow.CurBar, swingLow.CurPrice,
                            swingProp.ZigZagColorUp, swingProp.ZigZagStyle, swingProp.ZigZagWidth);
                    break;
            }
            //=====================================================================================
            #endregion

            #region Swing value output
            //=====================================================================================
            string output = "";
            switch (swingProp.SwingLengthType)
            {
                case SwingLengthStyle.False:
                    break;
                case SwingLengthStyle.Ticks:
                    output = swingLow.CurLength.ToString();
                    break;
                case SwingLengthStyle.Ticks_Price:
                    output = swingLow.CurLength.ToString() + " / " + swingLow.CurPrice.ToString();
                    break;
                case SwingLengthStyle.Price_Ticks:
                    output = swingLow.CurPrice.ToString() + " / " + swingLow.CurLength.ToString();
                    break;
                case SwingLengthStyle.Points:
                    output = (swingLow.CurLength * TickSize).ToString();
                    break;
                case SwingLengthStyle.Points_Price:
                    output = (swingLow.CurLength * TickSize).ToString() + " / " +
                        swingLow.CurPrice.ToString();
                    break;
                case SwingLengthStyle.Price_Points:
                    output = swingLow.CurPrice.ToString() + " / " +
                        (swingLow.CurLength * TickSize).ToString();
                    break;
                case SwingLengthStyle.Price:
                    output = swingLow.CurPrice.ToString();
                    break;
                case SwingLengthStyle.Percent:
                    output = (Math.Round((100.0 / swingHigh.CurPrice * (swingLow.CurLength *
                        TickSize)), 2, MidpointRounding.AwayFromZero)).ToString();
                    break;
            }
            string outputDuration = "";
            TimeSpan timeSpan;
            int hours, minutes, seconds = 0;
            switch (swingProp.SwingDurationType)
            {
                case SwingDurationStyle.False:
                    break;
                case SwingDurationStyle.Bars:
                    outputDuration = swingLow.CurDuration.ToString();
                    break;
                case SwingDurationStyle.MMSS:
                    timeSpan = swingLow.CurDateTime.Subtract(swingHigh.CurDateTime);
                    minutes = timeSpan.Minutes;
                    seconds = timeSpan.Seconds;
                    if (minutes == 0)
                        outputDuration = "0:" + seconds.ToString();
                    else if (seconds == 0)
                        outputDuration = minutes + ":00";
                    else
                        outputDuration = minutes + ":" + seconds;
                    break;
                case SwingDurationStyle.HHMM:
                    timeSpan = swingLow.CurDateTime.Subtract(swingHigh.CurDateTime);
                    hours = timeSpan.Hours;
                    minutes = timeSpan.Minutes;
                    if (hours == 0)
                        outputDuration = "0:" + minutes.ToString();
                    else if (minutes == 0)
                        outputDuration = hours + ":00";
                    else
                        outputDuration = hours + ":" + minutes;
                    break;
                case SwingDurationStyle.SecondsTotal:
                    timeSpan = swingLow.CurDateTime.Subtract(swingHigh.CurDateTime);
                    outputDuration = Math.Round(timeSpan.TotalSeconds, 1,
                        MidpointRounding.AwayFromZero).ToString();
                    break;
                case SwingDurationStyle.MinutesTotal:
                    timeSpan = swingLow.CurDateTime.Subtract(swingHigh.CurDateTime);
                    outputDuration = Math.Round(timeSpan.TotalMinutes, 1,
                        MidpointRounding.AwayFromZero).ToString();
                    break;
                case SwingDurationStyle.HoursTotal:
                    timeSpan = swingLow.CurDateTime.Subtract(swingHigh.CurDateTime);
                    outputDuration = timeSpan.TotalHours.ToString();
                    break;
                case SwingDurationStyle.Days:
                    timeSpan = swingLow.CurDateTime.Subtract(swingHigh.CurDateTime);
                    outputDuration = Math.Round(timeSpan.TotalDays, 1,
                        MidpointRounding.AwayFromZero).ToString();
                    break;
            }
            if (swingProp.SwingLengthType != SwingLengthStyle.False)
            {
                if (swingProp.SwingDurationType != SwingDurationStyle.False)
                    output = output + " / " + outputDuration;
            }
            else
                output = outputDuration;

            string swingLabel = null;
            Color textColor = Color.Transparent;
            switch (swingLow.CurRelation)
            {
                case 1:
                    swingLabel = "HL";
                    textColor = swingProp.TextColorHigherLow;
                    break;
                case -1:
                    swingLabel = "LL";
                    textColor = swingProp.TextColorLowerLow;
                    break;
                case 0:
                    swingLabel = "DB";
                    textColor = swingProp.TextColorDoubleBottom;
                    break;
            }
            if (output != null)
                DrawText("DnLength" + swingLow.Counter, swingProp.UseAutoScale,
                    output.ToString(), CurrentBar - swingLow.CurBar, swingLow.CurPrice,
                    -swingProp.TextOffsetLength, textColor, swingProp.TextFont,
                    StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
            if (swingProp.ShowSwingLabel)
                DrawText("DnLabel" + swingLow.Counter, swingProp.UseAutoScale,
                    swingLabel, CurrentBar - swingLow.CurBar, swingLow.CurPrice,
                    -swingProp.TextOffsetLabel, textColor, swingProp.TextFont,
                    StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
            if (swingProp.ShowSwingPrice)
                DrawText("DnPrice" + swingLow.Counter, swingProp.UseAutoScale,
                    String.Format("{0:F" + decimalPlaces + "}", swingLow.CurPrice),
                    CurrentBar - swingLow.CurBar, swingLow.CurPrice, -swingProp.TextOffsetPrice,
                    textColor, swingProp.TextFont, StringAlignment.Center, Color.Transparent,
                    Color.Transparent, 0);
            if (swingProp.ShowSwingPercent && swingLow.CurPercent != 0)
                DrawText("DnPerc" + swingLow.Counter, swingProp.UseAutoScale,
                    String.Format("{0:F1}", swingLow.CurPercent) + "%", CurrentBar - swingLow.CurBar,
                    swingLow.CurPrice, -swingProp.TextOffsetPercent, textColor,
                    swingProp.TextFont, StringAlignment.Center, Color.Transparent,
                    Color.Transparent, 0);
            if (swingProp.SwingTimeType != SwingTimeStyle.False)
            {
                string timeOutput = "";
                switch (swingProp.SwingTimeType)
                {
                    case SwingTimeStyle.False:
                        break;
                    case SwingTimeStyle.Integer:
                        timeOutput = swingLow.CurTime.ToString();
                        break;
                    case SwingTimeStyle.HHMM:
                        timeOutput = string.Format("{0:t}",
                            Times[BarsInProgress][CurrentBars[BarsInProgress] - swingLow.CurBar]);
                        break;
                    case SwingTimeStyle.HHMMSS:
                        timeOutput = string.Format("{0:T}",
                            Times[BarsInProgress][CurrentBars[BarsInProgress] - swingLow.CurBar]);
                        break;
                    case SwingTimeStyle.DDMM:
                        timeOutput = string.Format("{0:dd.MM}",
                            Times[BarsInProgress][CurrentBars[BarsInProgress] - swingHigh.CurBar]);
                        break;
                }
                DrawText("DnTime" + swingLow.Counter, swingProp.UseAutoScale,
                    timeOutput, CurrentBar - swingLow.CurBar, swingLow.CurPrice, -swingProp.TextOffsetTime,
                    textColor, swingProp.TextFont, StringAlignment.Center, Color.Transparent,
                    Color.Transparent, 0);
            }
            if (swingProp.SwingVolumeType != SwingVolumeStyle.False)
                DrawText("DnVolume" + swingLow.Counter, swingProp.UseAutoScale,
                    TruncIntToStr(swingLow.CurVolume), CurrentBar - swingLow.CurBar,
                    swingLow.CurPrice, -swingProp.TextOffsetVolume, textColor, swingProp.TextFont,
                    StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
            //=====================================================================================
            #endregion

            SwingStruct dn = new SwingStruct(swingLow.CurPrice, swingLow.CurBar,
                swingLow.CurDateTime, swingLow.CurDuration, swingLow.CurLength,
                swingLow.CurRelation, swingLow.CurVolume);
            swingLows.Add(dn);
            swingLow.ListCount = swingLows.Count - 1;
        }
        //#########################################################################################
        #endregion

        #region Calculate up swing
        //#########################################################################################
        private void CalcUpSwing(int bar, double high, bool updateHigh, Swings swingHigh,
            Swings swingLow, SwingCurrent swingCur, SwingProperties swingProp, BoolSeries upFlip,
            List<SwingStruct> swingHighs, int decimalPlaces, DataSeries doubleBottom, 
            DataSeries lowerLow, DataSeries higherLow, DataSeries doubleTop, DataSeries lowerHigh, 
            DataSeries higherHigh, DataSeries gannSwing)
        {
            #region New and update swing values
            //=====================================================================================
            if (!updateHigh)
            {
                if (swingProp.VisualizationType == VisualizationStyle.GannStyle)
                {
                    for (int i = CurrentBar - swingCur.SwingSlopeChangeBar; i >= 0; i--)
                        gannSwing.Set(i, swingLow.CurPrice);
                    gannSwing.Set(high);
                }
                swingHigh.LastPrice = swingHigh.CurPrice;
                swingHigh.LastBar = swingHigh.CurBar;
                swingHigh.LastDateTime = swingHigh.CurDateTime;
                swingHigh.LastDuration = swingHigh.CurDuration;
                swingHigh.LastLength = swingHigh.CurLength;
                swingHigh.LastTime = swingHigh.CurTime;
                swingHigh.LastPercent = swingHigh.CurPercent;
                swingHigh.LastRelation = swingHigh.CurRelation;
                swingHigh.LastVolume = swingHigh.CurVolume;
                swingHigh.Counter++;
                swingCur.SwingSlope = 1;
                swingCur.SwingSlopeChangeBar = bar;
                upFlip.Set(true);
            }
            else
            {
                if (swingProp.VisualizationType == VisualizationStyle.Dots
                    || swingProp.VisualizationType == VisualizationStyle.Dots_ZigZag)
                {
                    doubleTop.Reset(CurrentBar - swingHigh.CurBar);
                    higherHigh.Reset(CurrentBar - swingHigh.CurBar);
                    lowerHigh.Reset(CurrentBar - swingHigh.CurBar);
                }
                swingHighs.RemoveAt(swingHighs.Count - 1);
            }
            swingHigh.CurBar = bar;
            swingHigh.CurPrice = Math.Round(high, decimalPlaces, MidpointRounding.AwayFromZero);
            swingHigh.CurTime = ToTime(Times[BarsInProgress][CurrentBars[BarsInProgress] -
                swingHigh.CurBar]);
            swingHigh.CurDateTime = Times[BarsInProgress][CurrentBars[BarsInProgress] -
                swingHigh.CurBar];
            swingHigh.CurLength = Convert.ToInt32(Math.Round((swingHigh.CurPrice -
                swingLow.CurPrice) / TickSize, 0, MidpointRounding.AwayFromZero));
            if (swingLow.CurLength != 0)
                swingHigh.CurPercent = Math.Round(100.0 / Math.Abs(swingLow.CurLength) *
                    swingHigh.CurLength, 1);
            swingHigh.CurDuration = swingHigh.CurBar - swingLow.CurBar;
            double dtbOffset = ATR(BarsArray[BarsInProgress], 14)[CurrentBars[BarsInProgress] -
                swingHigh.CurBar] * swingProp.DtbStrength / 100;
            if (swingHigh.CurPrice > swingHigh.LastPrice - dtbOffset && swingHigh.CurPrice <
                swingHigh.LastPrice + dtbOffset)
                swingHigh.CurRelation = 0;
            else if (swingHigh.CurPrice < swingHigh.LastPrice)
                swingHigh.CurRelation = -1;
            else
                swingHigh.CurRelation = 1;
            if (!CalculateOnBarClose)
                swingLow.SignalBarVolume = Volumes[BarsInProgress][0];
            double swingVolume = 0.0;
            for (int i = 0; i < swingHigh.CurDuration; i++)
                swingVolume = swingVolume + Volumes[BarsInProgress][i];
            if (!CalculateOnBarClose)
                swingVolume = swingVolume + (Volumes[BarsInProgress][CurrentBars[BarsInProgress] -
                    swingLow.CurBar] - swingHigh.SignalBarVolume);
            if (swingProp.SwingVolumeType == SwingVolumeStyle.Relative)
                swingVolume = Math.Round(swingVolume / swingHigh.CurDuration, 0,
                    MidpointRounding.AwayFromZero);
            swingHigh.CurVolume = Convert.ToInt64(swingVolume);
            //=====================================================================================
            #endregion

            #region Visualize swing
            //=====================================================================================
            switch (swingProp.VisualizationType)
            {
                case VisualizationStyle.False:
                    break;
                case VisualizationStyle.Dots:
                    switch (swingHigh.CurRelation)
                    {
                        case 1:
                            higherHigh.Set(CurrentBar - swingHigh.CurBar, swingHigh.CurPrice);
                            break;
                        case -1:
                            lowerHigh.Set(CurrentBar - swingHigh.CurBar, swingHigh.CurPrice);
                            break;
                        case 0:
                            doubleTop.Set(CurrentBar - swingHigh.CurBar, swingHigh.CurPrice);
                            break;
                    }
                    break;
                case VisualizationStyle.Dots_ZigZag:
                    switch (swingHigh.CurRelation)
                    {
                        case 1:
                            higherHigh.Set(CurrentBar - swingHigh.CurBar, swingHigh.CurPrice);
                            break;
                        case -1:
                            lowerHigh.Set(CurrentBar - swingHigh.CurBar, swingHigh.CurPrice);
                            break;
                        case 0:
                            doubleTop.Set(CurrentBar - swingHigh.CurBar, swingHigh.CurPrice);
                            break;
                    }
                    DrawLine("ZigZagUp" + swingHigh.Counter,
                        swingProp.UseAutoScale, CurrentBar - swingLow.CurBar, swingLow.CurPrice,
                        CurrentBar - swingHigh.CurBar, swingHigh.CurPrice, swingProp.ZigZagColorUp,
                        swingProp.ZigZagStyle, swingProp.ZigZagWidth);
                    break;
                case VisualizationStyle.ZigZag:
                    DrawLine("ZigZagUp" + swingHigh.Counter,
                        swingProp.UseAutoScale, CurrentBar - swingLow.CurBar, swingLow.CurPrice,
                        CurrentBar - swingHigh.CurBar, swingHigh.CurPrice, swingProp.ZigZagColorUp,
                        swingProp.ZigZagStyle, swingProp.ZigZagWidth);
                    break;
                case VisualizationStyle.GannStyle:
                    for (int i = CurrentBar - swingCur.SwingSlopeChangeBar; i >= 0; i--)
                        gannSwing.Set(i, swingHigh.CurPrice);
                    break;
                case VisualizationStyle.ZigZagVolume:
                    if (swingHigh.CurVolume > swingLow.CurVolume)
                        DrawLine("ZigZagUp" + swingHigh.Counter,
                            swingProp.UseAutoScale, CurrentBar - swingLow.CurBar, 
                            swingLow.CurPrice, CurrentBar - swingHigh.CurBar, swingHigh.CurPrice, 
                            swingProp.ZigZagColorUp, swingProp.ZigZagStyle, swingProp.ZigZagWidth);
                    else
                        DrawLine("ZigZagUp" + swingHigh.Counter,
                            swingProp.UseAutoScale, CurrentBar - swingLow.CurBar,
                            swingLow.CurPrice, CurrentBar - swingHigh.CurBar, swingHigh.CurPrice,
                            swingProp.ZigZagColorDn, swingProp.ZigZagStyle, swingProp.ZigZagWidth);
                    break;
            }
            //=====================================================================================
            #endregion

            #region Swing value output
            //=====================================================================================
            string output = "";
            switch (swingProp.SwingLengthType)
            {
                case SwingLengthStyle.False:
                    break;
                case SwingLengthStyle.Ticks:
                    output = swingHigh.CurLength.ToString();
                    break;
                case SwingLengthStyle.Ticks_Price:
                    output = swingHigh.CurLength.ToString() + " / " +
                        swingHigh.CurPrice.ToString();
                    break;
                case SwingLengthStyle.Price_Ticks:
                    output = swingHigh.CurPrice.ToString() + " / " +
                        swingHigh.CurLength.ToString();
                    break;
                case SwingLengthStyle.Points:
                    output = (swingHigh.CurLength * TickSize).ToString();
                    break;
                case SwingLengthStyle.Points_Price:
                    output = (swingHigh.CurLength * TickSize).ToString() + " / " +
                        swingHigh.CurPrice.ToString();
                    break;
                case SwingLengthStyle.Price_Points:
                    output = swingHigh.CurPrice.ToString() + " / " +
                        (swingHigh.CurLength * TickSize).ToString();
                    break;
                case SwingLengthStyle.Price:
                    output = swingHigh.CurPrice.ToString();
                    break;
                case SwingLengthStyle.Percent:
                    output = (Math.Round((100.0 / swingLow.CurPrice * (swingHigh.CurLength *
                        TickSize)), 2, MidpointRounding.AwayFromZero)).ToString();
                    break;
            }
            string outputDuration = "";
            TimeSpan timeSpan;
            int hours, minutes, seconds = 0;
            switch (swingProp.SwingDurationType)
            {
                case SwingDurationStyle.False:
                    break;
                case SwingDurationStyle.Bars:
                    outputDuration = swingHigh.CurDuration.ToString();
                    break;
                case SwingDurationStyle.MMSS:
                    timeSpan = swingHigh.CurDateTime.Subtract(swingLow.CurDateTime);
                    minutes = timeSpan.Minutes;
                    seconds = timeSpan.Seconds;
                    if (minutes == 0)
                        outputDuration = "0:" + seconds.ToString();
                    else if (seconds == 0)
                        outputDuration = minutes + ":00";
                    else
                        outputDuration = minutes + ":" + seconds;
                    break;
                case SwingDurationStyle.HHMM:
                    timeSpan = swingHigh.CurDateTime.Subtract(swingLow.CurDateTime);
                    hours = timeSpan.Hours;
                    minutes = timeSpan.Minutes;
                    if (hours == 0)
                        outputDuration = "0:" + minutes.ToString();
                    else if (minutes == 0)
                        outputDuration = hours + ":00";
                    else
                        outputDuration = hours + ":" + minutes;
                    break;
                case SwingDurationStyle.SecondsTotal:
                    timeSpan = swingHigh.CurDateTime.Subtract(swingLow.CurDateTime);
                    outputDuration = Math.Round(timeSpan.TotalSeconds, 1,
                        MidpointRounding.AwayFromZero).ToString();
                    break;
                case SwingDurationStyle.MinutesTotal:
                    timeSpan = swingHigh.CurDateTime.Subtract(swingLow.CurDateTime);
                    outputDuration = Math.Round(timeSpan.TotalMinutes, 1,
                        MidpointRounding.AwayFromZero).ToString();
                    break;
                case SwingDurationStyle.HoursTotal:
                    timeSpan = swingHigh.CurDateTime.Subtract(swingLow.CurDateTime);
                    outputDuration = Math.Round(timeSpan.TotalHours, 1,
                        MidpointRounding.AwayFromZero).ToString();
                    break;
                case SwingDurationStyle.Days:
                    timeSpan = swingHigh.CurDateTime.Subtract(swingLow.CurDateTime);
                    outputDuration = Math.Round(timeSpan.TotalDays, 1,
                        MidpointRounding.AwayFromZero).ToString();
                    break;
            }
            if (swingProp.SwingLengthType != SwingLengthStyle.False)
            {
                if (swingProp.SwingDurationType != SwingDurationStyle.False)
                    output = output + " / " + outputDuration;
            }
            else
                output = outputDuration;

            string swingLabel = null;
            Color textColor = Color.Transparent;
            switch (swingHigh.CurRelation)
            {
                case 1:
                    swingLabel = "HH";
                    textColor = swingProp.TextColorHigherHigh;
                    break;
                case -1:
                    swingLabel = "LH";
                    textColor = swingProp.TextColorLowerHigh;
                    break;
                case 0:
                    swingLabel = "DT";
                    textColor = swingProp.TextColorDoubleTop;
                    break;
            }
            if (output != null)
                DrawText("UpLength" + swingHigh.Counter,
                    swingProp.UseAutoScale, output.ToString(), CurrentBar - swingHigh.CurBar,
                    swingHigh.CurPrice, swingProp.TextOffsetLength, textColor, swingProp.TextFont,
                    StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
            if (swingProp.ShowSwingLabel)
                DrawText("UpLabel" + swingHigh.Counter, swingProp.UseAutoScale,
                    swingLabel, CurrentBar - swingHigh.CurBar, swingHigh.CurPrice,
                    swingProp.TextOffsetLabel, textColor, swingProp.TextFont,
                    StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
            if (swingProp.ShowSwingPrice)
                DrawText("UpPrice" + swingHigh.Counter, swingProp.UseAutoScale,
                    String.Format("{0:F" + decimalPlaces + "}", swingHigh.CurPrice),
                    CurrentBar - swingHigh.CurBar, swingHigh.CurPrice, swingProp.TextOffsetPrice,
                    textColor, swingProp.TextFont, StringAlignment.Center, Color.Transparent,
                    Color.Transparent, 0);
            if (swingProp.ShowSwingPercent && swingHigh.CurPercent != 0)
                DrawText("UpPerc" + swingHigh.Counter, swingProp.UseAutoScale,
                    String.Format("{0:F1}", swingHigh.CurPercent) + "%", CurrentBar - swingHigh.CurBar,
                    swingHigh.CurPrice, swingProp.TextOffsetPercent, textColor, swingProp.TextFont,
                    StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
            if (swingProp.SwingTimeType != SwingTimeStyle.False)
            {
                string timeOutput = "";
                switch (swingProp.SwingTimeType)
                {
                    case SwingTimeStyle.False:
                        break;
                    case SwingTimeStyle.Integer:
                        timeOutput = swingHigh.CurTime.ToString();
                        break;
                    case SwingTimeStyle.HHMM:
                        timeOutput = string.Format("{0:t}",
                            Times[BarsInProgress][CurrentBars[BarsInProgress] - swingHigh.CurBar]);
                        break;
                    case SwingTimeStyle.HHMMSS:
                        timeOutput = string.Format("{0:T}",
                            Times[BarsInProgress][CurrentBars[BarsInProgress] - swingHigh.CurBar]);
                        break;
                    case SwingTimeStyle.DDMM:
                        timeOutput = string.Format("{0:dd.MM}",
                            Times[BarsInProgress][CurrentBars[BarsInProgress] - swingHigh.CurBar]);
                        break;
                }
                DrawText("UpTime" + swingHigh.Counter, swingProp.UseAutoScale,
                    timeOutput, CurrentBar - swingHigh.CurBar, swingHigh.CurPrice,
                    swingProp.TextOffsetTime, textColor, swingProp.TextFont,
                    StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
            }
            if (swingProp.SwingVolumeType != SwingVolumeStyle.False)
                DrawText("UpVolume" + swingHigh.Counter, swingProp.UseAutoScale,
                    TruncIntToStr(swingHigh.CurVolume), CurrentBar - swingHigh.CurBar,
                    swingHigh.CurPrice, swingProp.TextOffsetVolume, textColor, swingProp.TextFont,
                    StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
            //=========================================================================================
            #endregion

            SwingStruct up = new SwingStruct(swingHigh.CurPrice, swingHigh.CurBar,
                swingHigh.CurDateTime, swingHigh.CurDuration, swingHigh.CurLength,
                swingHigh.CurRelation, swingHigh.CurVolume);
            swingHighs.Add(up);
            swingHigh.ListCount = swingHighs.Count - 1;
        }
        //#########################################################################################
        #endregion

        #region Trunc integer to string
        //#########################################################################################
        /// <summary>
        /// Converts long integer numbers in a number-string format.
        /// </summary>
        protected string TruncIntToStr(long number)
        {
            long numberAbs = Math.Abs(number);
            string output = "";
            double convertedNumber = 0.0;
            if (numberAbs > 1000000000)
            {
                convertedNumber = Math.Round(number / 1000000000.0, 1,
                    MidpointRounding.AwayFromZero);
                output = convertedNumber.ToString() + "B";
            }
            else if (numberAbs > 1000000)
            {
                convertedNumber = Math.Round(number / 1000000.0, 1,
                    MidpointRounding.AwayFromZero);
                output = convertedNumber.ToString() + "M";
            }
            else if (numberAbs > 1000)
            {
                convertedNumber = Math.Round(number / 1000.0, 1,
                    MidpointRounding.AwayFromZero);
                output = convertedNumber.ToString() + "K";
            }
            else
                output = number.ToString();

            return output;
        }
        //#########################################################################################
        #endregion

        // Functions for the oscillator
        #region Calculate Swing Standard
        //#########################################################################################
        protected void CalculateSwingStandard(Swings swingHigh, Swings swingLow,
            SwingCurrent swingCur, SwingProperties swingProp, int decimalPlaces, 
            bool useCloseValues)
        {
            // Check if high and low values are used or only close values
            IDataSeries[] highs;
            IDataSeries[] lows;
            if (useCloseValues == true)
            {
                lows = Closes;
                highs = Closes;
            }
            else
            {
                lows = Lows;
                highs = Highs;
            }

            // For a new swing high in an uptrend, Highs[BarsInProgress][0] must be 
            // greater than the current swing high
            if (swingCur.SwingSlope == 1 && highs[BarsInProgress][0] <= swingHigh.CurPrice)
                swingHigh.New = false;
            else
                swingHigh.New = true;

            // For a new swing low in a downtrend, Lows[BarsInProgress][0] must be 
            // smaller than the current swing low
            if (swingCur.SwingSlope == -1 && lows[BarsInProgress][0] >= swingLow.CurPrice)
                swingLow.New = false;
            else
                swingLow.New = true;

            // CalculatOnBarClose == true
            if (CalculateOnBarClose)
            {
                // test if Highs[BarsInProgress][0] is higher than the last 
                // calculationSize highs = new swing high
                if (swingHigh.New)
                {
                    for (int i = 1; i < swingProp.SwingSize + 1; i++)
                    {
                        if (highs[BarsInProgress][0] <= highs[BarsInProgress][i])
                        {
                            swingHigh.New = false;
                            break;
                        }
                    }
                }
                // test if Lows[BarsInProgress][0] is lower than the last 
                // calculationSize lows = new swing low
                if (swingLow.New)
                {
                    for (int i = 1; i < swingProp.SwingSize + 1; i++)
                    {
                        if (lows[BarsInProgress][0] >= lows[BarsInProgress][i])
                        {
                            swingLow.New = false;
                            break;
                        }
                    }
                }

                // New swing high and new swing low
                if (swingHigh.New && swingLow.New)
                {
                    // Downtrend - ignore the swing high
                    if (swingCur.SwingSlope == -1)
                        swingHigh.New = false;
                    // Uptrend   - ignore the swing low
                    else
                        swingLow.New = false;
                }
            }
            // CalculatOnBarClose == false
            else
            {
                // Used to control, that only one swing is set for 
                // each bar
                if (FirstTickOfBar)
                    swingCur.NewSwing = 0;

                // No swing or an up swing is found
                if (swingCur.NewSwing != -1)
                {
                    // test if Highs[BarsInProgress][0] is higher than the last 
                    // calculationSize highs = new swing high
                    if (swingHigh.New)
                    {
                        for (int i = 1; i < swingProp.SwingSize + 1; i++)
                        {
                            if (highs[BarsInProgress][0] <= highs[BarsInProgress][i])
                            {
                                swingHigh.New = false;
                                break;
                            }
                        }
                        // Found a swing high
                        if (swingHigh.New)
                            swingCur.NewSwing = 1;
                    }
                }

                // No swing or an down swing is found
                if (swingCur.NewSwing != 1)
                {
                    // test if Lows[BarsInProgress][0] is lower than the last 
                    // calculationSize lows = new swing low
                    if (swingLow.New)
                    {
                        for (int i = 1; i < swingProp.SwingSize + 1; i++)
                        {
                            if (lows[BarsInProgress][0] >= lows[BarsInProgress][i])
                            {
                                swingLow.New = false;
                                break;
                            }
                        }
                        // Found a swing low
                        if (swingLow.New)
                            swingCur.NewSwing = -1;
                    }
                }

                // Set newLow back to false
                if (swingCur.NewSwing == 1)
                    swingLow.New = false;
                // Set newHigh back to false
                if (swingCur.NewSwing == -1)
                    swingHigh.New = false;
            }

            // Swing high
            if (swingHigh.New)
            {
                int bar;
                double price;
                // New swing high
                if (swingCur.SwingSlope != 1)
                {
                    bar = CurrentBars[BarsInProgress] -
                        HighestBar(highs[BarsInProgress], CurrentBars[BarsInProgress] -
                        swingLow.CurBar);
                    price = highs[BarsInProgress][HighestBar(highs[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingLow.CurBar)];
                    swingHigh.Update = false;
                }
                // Update swing high
                else
                {
                    bar = CurrentBars[BarsInProgress];
                    price = highs[BarsInProgress][0];
                    swingHigh.Update = true;
                }
                CalcUpSwing(bar, price, swingHigh.Update, swingHigh, swingCur, swingProp, 
                    decimalPlaces);
            }
            // Swing low
            else if (swingLow.New)
            {
                int bar;
                double price;
                // New swing low
                if (swingCur.SwingSlope != -1)
                {
                    bar = CurrentBars[BarsInProgress] - LowestBar(lows[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingHigh.CurBar);
                    price = lows[BarsInProgress][LowestBar(lows[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingHigh.CurBar)];
                    swingLow.Update = false;
                }
                // Update swing low
                else
                {
                    bar = CurrentBars[BarsInProgress];
                    price = lows[BarsInProgress][0];
                    swingLow.Update = true;
                }
                CalcDnSwing(bar, price, swingLow.Update, swingLow, swingCur, swingProp, 
                    decimalPlaces);
            }
        }
        //#########################################################################################
        #endregion

        #region Calculate Swing Gann
        //#########################################################################################
        protected void CalculateSwingGann(Swings swingHigh, Swings swingLow, SwingCurrent swingCur,
            SwingProperties swingProp, int decimalPlaces)
        {
            #region Set bar property
            //=================================================================================
            // Represents the bar type. -1 = Down | 0 = Inside | 1 = Up | 2 = Outside
            int barType = 0;
            if (Highs[BarsInProgress][0] > Highs[BarsInProgress][1])
            {
                if (Lows[BarsInProgress][0] < Lows[BarsInProgress][1])
                    barType = 2;
                else
                    barType = 1;
            }
            else
            {
                if (Lows[BarsInProgress][0] < Lows[BarsInProgress][1])
                    barType = -1;
                else
                    barType = 0;
            }
            //=================================================================================
            #endregion

            #region Up swing
            //=================================================================================
            if (swingCur.SwingSlope == 1)
            {
                switch (barType)
                {
                    // Up bar
                    case 1:
                        swingCur.ConsecutiveBars = 0;
                        swingCur.ConsecutiveBarValue = 0.0;
                        if (Highs[BarsInProgress][0] > swingHigh.CurPrice)
                        {
                            swingHigh.New = true;
                            swingHigh.Update = true;
                            CalcUpSwing(CurrentBars[BarsInProgress],
                                Highs[BarsInProgress][0], swingHigh.Update, swingHigh,
                                swingCur, swingProp, decimalPlaces);
                            if ((swingCur.ConsecutiveBars + 1) == swingProp.SwingSize)
                                swingCur.StopOutsideBarCalc = true;
                        }
                        break;
                    // Down bar
                    case -1:
                        if (swingCur.ConsecutiveBarNumber != CurrentBars[BarsInProgress])
                        {
                            if (swingCur.ConsecutiveBarValue == 0.0)
                            {
                                swingCur.ConsecutiveBars++;
                                swingCur.ConsecutiveBarNumber = CurrentBars[BarsInProgress];
                                swingCur.ConsecutiveBarValue = Lows[BarsInProgress][0];
                            }
                            else if (Lows[BarsInProgress][0] < swingCur.ConsecutiveBarValue)
                            {
                                swingCur.ConsecutiveBars++;
                                swingCur.ConsecutiveBarNumber = CurrentBars[BarsInProgress];
                                swingCur.ConsecutiveBarValue = Lows[BarsInProgress][0];
                            }
                        }
                        else if (Lows[BarsInProgress][0] < swingCur.ConsecutiveBarValue)
                            swingCur.ConsecutiveBarValue = Lows[BarsInProgress][0];
                        if (swingCur.ConsecutiveBars == swingProp.SwingSize ||
                            (swingProp.UseBreakouts && Lows[BarsInProgress][0] <
                            swingLow.CurPrice))
                        {
                            swingCur.ConsecutiveBars = 0;
                            swingCur.ConsecutiveBarValue = 0.0;
                            swingLow.New = true;
                            swingLow.Update = false;
                            int bar = CurrentBars[BarsInProgress] -
                                LowestBar(Lows[BarsInProgress],
                                CurrentBars[BarsInProgress] - swingHigh.CurBar);
                            double price =
                                Lows[BarsInProgress][LowestBar(Lows[BarsInProgress],
                                CurrentBars[BarsInProgress] - swingHigh.CurBar)];
                            CalcDnSwing(bar, price, swingLow.Update, swingLow, swingCur, 
                                swingProp, decimalPlaces);
                        }
                        break;
                    // Inside bar
                    case 0:
                        if (!swingProp.IgnoreInsideBars)
                        {
                            swingCur.ConsecutiveBars = 0;
                            swingCur.ConsecutiveBarValue = 0.0;
                        }
                        break;
                    // Outside bar
                    case 2:
                        if (Highs[BarsInProgress][0] > swingHigh.CurPrice)
                        {
                            swingHigh.New = true;
                            swingHigh.Update = true;
                            CalcUpSwing(CurrentBars[BarsInProgress],
                                Highs[BarsInProgress][0], swingHigh.Update, swingHigh,
                                swingCur, swingProp, decimalPlaces);
                        }
                        else if (!swingCur.StopOutsideBarCalc)
                        {
                            if (swingCur.ConsecutiveBarNumber != CurrentBars[BarsInProgress])
                            {
                                if (swingCur.ConsecutiveBarValue == 0.0)
                                {
                                    swingCur.ConsecutiveBars++;
                                    swingCur.ConsecutiveBarNumber =
                                        CurrentBars[BarsInProgress];
                                    swingCur.ConsecutiveBarValue = Lows[BarsInProgress][0];
                                }
                                else if (Lows[BarsInProgress][0] <
                                    swingCur.ConsecutiveBarValue)
                                {
                                    swingCur.ConsecutiveBars++;
                                    swingCur.ConsecutiveBarNumber =
                                        CurrentBars[BarsInProgress];
                                    swingCur.ConsecutiveBarValue = Lows[BarsInProgress][0];
                                }
                            }
                            else if (Lows[BarsInProgress][0] < swingCur.ConsecutiveBarValue)
                                swingCur.ConsecutiveBarValue = Lows[BarsInProgress][0];
                            if (swingCur.ConsecutiveBars == swingProp.SwingSize ||
                                (swingProp.UseBreakouts && Lows[BarsInProgress][0] <
                                swingLow.CurPrice))
                            {
                                swingCur.ConsecutiveBars = 0;
                                swingCur.ConsecutiveBarValue = 0.0;
                                swingLow.New = true;
                                swingLow.Update = false;
                                int bar = CurrentBars[BarsInProgress] -
                                    LowestBar(Lows[BarsInProgress],
                                    CurrentBars[BarsInProgress] - swingHigh.CurBar);
                                double price =
                                    Lows[BarsInProgress][LowestBar(Lows[BarsInProgress],
                                    CurrentBars[BarsInProgress] - swingHigh.CurBar)];
                                CalcDnSwing(bar, price, swingLow.Update, swingLow, swingCur, 
                                    swingProp, decimalPlaces);
                            }
                        }
                        break;
                }
            }
            //=================================================================================
            #endregion

            #region Down swing
            //=================================================================================
            else
            {
                switch (barType)
                {
                    // Dwon bar
                    case -1:
                        swingCur.ConsecutiveBars = 0;
                        swingCur.ConsecutiveBarValue = 0.0;
                        if (Lows[BarsInProgress][0] < swingLow.CurPrice)
                        {
                            swingLow.New = true;
                            swingLow.Update = true;
                            CalcDnSwing(CurrentBars[BarsInProgress],
                                Lows[BarsInProgress][0], swingLow.Update, swingLow, swingCur, 
                                swingProp, decimalPlaces);
                            if ((swingCur.ConsecutiveBars + 1) == swingProp.SwingSize)
                                swingCur.StopOutsideBarCalc = true;
                        }
                        break;
                    // Up bar
                    case 1:
                        if (swingCur.ConsecutiveBarNumber != CurrentBars[BarsInProgress])
                        {
                            if (swingCur.ConsecutiveBarValue == 0.0)
                            {
                                swingCur.ConsecutiveBars++;
                                swingCur.ConsecutiveBarNumber = CurrentBars[BarsInProgress];
                                swingCur.ConsecutiveBarValue = Highs[BarsInProgress][0];
                            }
                            else if (Highs[BarsInProgress][0] > swingCur.ConsecutiveBarValue)
                            {
                                swingCur.ConsecutiveBars++;
                                swingCur.ConsecutiveBarNumber = CurrentBars[BarsInProgress];
                                swingCur.ConsecutiveBarValue = Highs[BarsInProgress][0];
                            }
                        }
                        else if (Highs[BarsInProgress][0] > swingCur.ConsecutiveBarValue)
                            swingCur.ConsecutiveBarValue = Highs[BarsInProgress][0];
                        if (swingCur.ConsecutiveBars == swingProp.SwingSize ||
                            (swingProp.UseBreakouts && Highs[BarsInProgress][0] >
                            swingHigh.CurPrice))
                        {
                            swingCur.ConsecutiveBars = 0;
                            swingCur.ConsecutiveBarValue = 0.0;
                            swingHigh.New = true;
                            swingHigh.Update = false;
                            int bar = CurrentBars[BarsInProgress] -
                                HighestBar(Highs[BarsInProgress],
                                CurrentBars[BarsInProgress] - swingLow.CurBar);
                            double price =
                                Highs[BarsInProgress][HighestBar(Highs[BarsInProgress],
                                CurrentBars[BarsInProgress] - swingLow.CurBar)];
                            CalcUpSwing(bar, price, swingHigh.Update, swingHigh, swingCur, 
                                swingProp, decimalPlaces);
                        }
                        break;
                    // Inside bar
                    case 0:
                        if (!swingProp.IgnoreInsideBars)
                        {
                            swingCur.ConsecutiveBars = 0;
                            swingCur.ConsecutiveBarValue = 0.0;
                        }
                        break;
                    // Outside bar
                    case 2:
                        if (Lows[BarsInProgress][0] < swingLow.CurPrice)
                        {
                            swingLow.New = true;
                            swingLow.Update = true;
                            CalcDnSwing(CurrentBars[BarsInProgress],
                                Lows[BarsInProgress][0], swingLow.Update, swingLow, swingCur, 
                                swingProp, decimalPlaces);
                        }
                        else if (!swingCur.StopOutsideBarCalc)
                        {
                            if (swingCur.ConsecutiveBarNumber != CurrentBars[BarsInProgress])
                            {
                                if (swingCur.ConsecutiveBarValue == 0.0)
                                {
                                    swingCur.ConsecutiveBars++;
                                    swingCur.ConsecutiveBarNumber =
                                        CurrentBars[BarsInProgress];
                                    swingCur.ConsecutiveBarValue = Highs[BarsInProgress][0];
                                }
                                else if (Highs[BarsInProgress][0] >
                                    swingCur.ConsecutiveBarValue)
                                {
                                    swingCur.ConsecutiveBars++;
                                    swingCur.ConsecutiveBarNumber =
                                        CurrentBars[BarsInProgress];
                                    swingCur.ConsecutiveBarValue = Highs[BarsInProgress][0];
                                }
                            }
                            else if (Highs[BarsInProgress][0] > swingCur.ConsecutiveBarValue)
                                swingCur.ConsecutiveBarValue = Highs[BarsInProgress][0];
                            if (swingCur.ConsecutiveBars == swingProp.SwingSize ||
                                (swingProp.UseBreakouts && Highs[BarsInProgress][0] >
                                swingHigh.CurPrice))
                            {
                                swingCur.ConsecutiveBars = 0;
                                swingCur.ConsecutiveBarValue = 0.0;
                                swingHigh.New = true;
                                swingHigh.Update = false;
                                int bar = CurrentBars[BarsInProgress] -
                                    HighestBar(Highs[BarsInProgress],
                                    CurrentBars[BarsInProgress] - swingLow.CurBar);
                                double price =
                                    Highs[BarsInProgress][HighestBar(Highs[BarsInProgress],
                                    CurrentBars[BarsInProgress] - swingLow.CurBar)];
                                CalcUpSwing(bar, price, swingHigh.Update, swingHigh,
                                    swingCur, swingProp, decimalPlaces);
                            }
                        }
                        break;
                }
            }
            //=================================================================================
            #endregion
        }
        //#########################################################################################
        #endregion

        #region Calculate Swing Ticks
        //#########################################################################################
        protected void CalculateSwingTicks(Swings swingHigh, Swings swingLow,
            SwingCurrent swingCur, SwingProperties swingProp, int decimalPlaces, 
            bool useCloseValues)
        {
            // Check if high and low values are used or only close values
            IDataSeries[] highs;
            IDataSeries[] lows;
            if (useCloseValues == true)
            {
                lows = Closes;
                highs = Closes;
            }
            else
            {
                lows = Lows;
                highs = Highs;
            }

            // For a new swing high in an uptrend, Highs[BarsInProgress][0] must be 
            // greater than the current swing high
            if (swingCur.SwingSlope == 1 && highs[BarsInProgress][0] <= swingHigh.CurPrice)
                swingHigh.New = false;
            else
                swingHigh.New = true;

            // For a new swing low in a downtrend, Lows[BarsInProgress][0] must be 
            // smaller than the current swing low
            if (swingCur.SwingSlope == -1 && lows[BarsInProgress][0] >= swingLow.CurPrice)
                swingLow.New = false;
            else
                swingLow.New = true;

            // CalculatOnBarClose == true
            if (CalculateOnBarClose)
            {
                if (swingHigh.New
                    && highs[BarsInProgress][0] <
                    (swingLow.CurPrice + swingProp.SwingSize * TickSize))
                    swingHigh.New = false;

                if (swingLow.New
                    && lows[BarsInProgress][0] >
                    (swingHigh.CurPrice - swingProp.SwingSize * TickSize))
                    swingLow.New = false;

                // New swing high and new swing low
                if (swingHigh.New && swingLow.New)
                {
                    // Downtrend - ignore the swing high
                    if (swingCur.SwingSlope == -1)
                        swingHigh.New = false;
                    // Uptrend   - ignore the swing low
                    else
                        swingLow.New = false;
                }
            }
            // CalculatOnBarClose == false
            else
            {
                // Used to control, that only one swing is set for 
                // each bar
                if (FirstTickOfBar)
                    swingCur.NewSwing = 0;

                // No swing or an up swing is found
                if (swingCur.NewSwing != -1)
                {
                    // test if Highs[BarsInProgress][0] is higher than the last 
                    // calculationSize highs = new swing high
                    if (swingHigh.New)
                    {
                        if (highs[BarsInProgress][0] <
                            (swingLow.CurPrice + swingProp.SwingSize * TickSize))
                            swingHigh.New = false;
                        // Found a swing high
                        if (swingHigh.New)
                            swingCur.NewSwing = 1;
                    }
                }

                // No swing or an down swing is found
                if (swingCur.NewSwing != 1)
                {
                    // test if Lows[BarsInProgress][0] is lower than the last 
                    // calculationSize lows = new swing low
                    if (swingLow.New)
                    {
                        if (lows[BarsInProgress][0] >
                            (swingHigh.CurPrice - swingProp.SwingSize * TickSize))
                            swingLow.New = false;
                        // Found a swing low
                        if (swingLow.New)
                            swingCur.NewSwing = -1;
                    }
                }

                // Set newLow back to false
                if (swingCur.NewSwing == 1)
                    swingLow.New = false;
                // Set newHigh back to false
                if (swingCur.NewSwing == -1)
                    swingHigh.New = false;
            }

            // Swing high
            if (swingHigh.New)
            {
                int bar;
                double price;
                // New swing high
                if (swingCur.SwingSlope != 1)
                {
                    bar = CurrentBars[BarsInProgress] -
                        HighestBar(highs[BarsInProgress], CurrentBars[BarsInProgress] -
                        swingLow.CurBar);
                    price = highs[BarsInProgress][HighestBar(highs[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingLow.CurBar)];
                    swingHigh.Update = false;
                }
                // Update swing high
                else
                {
                    bar = CurrentBars[BarsInProgress];
                    price = highs[BarsInProgress][0];
                    swingHigh.Update = true;
                }
                CalcUpSwing(bar, price, swingHigh.Update, swingHigh, swingCur, swingProp,
                    decimalPlaces);
            }
            // Swing low
            else if (swingLow.New)
            {
                int bar;
                double price;
                // New swing low
                if (swingCur.SwingSlope != -1)
                {
                    bar = CurrentBars[BarsInProgress] - LowestBar(lows[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingHigh.CurBar);
                    price = lows[BarsInProgress][LowestBar(lows[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingHigh.CurBar)];
                    swingLow.Update = false;
                }
                // Update swing low
                else
                {
                    bar = CurrentBars[BarsInProgress];
                    price = lows[BarsInProgress][0];
                    swingLow.Update = true;
                }
                CalcDnSwing(bar, price, swingLow.Update, swingLow, swingCur, swingProp,
                    decimalPlaces);
            }
        }
        //#########################################################################################
        #endregion

        #region Calculate Swing Percent
        //#########################################################################################
        protected void CalculateSwingPercent(Swings swingHigh, Swings swingLow,
            SwingCurrent swingCur, SwingProperties swingProp, int decimalPlaces, 
            bool useCloseValues)
        {
            // Check if high and low values are used or only close values
            IDataSeries[] highs;
            IDataSeries[] lows;
            if (useCloseValues == true)
            {
                lows = Closes;
                highs = Closes;
            }
            else
            {
                lows = Lows;
                highs = Highs;
            }

            // For a new swing high in an uptrend, Highs[BarsInProgress][0] must be 
            // greater than the current swing high
            if (swingCur.SwingSlope == 1 && highs[BarsInProgress][0] <= swingHigh.CurPrice)
                swingHigh.New = false;
            else
                swingHigh.New = true;

            // For a new swing low in a downtrend, Lows[BarsInProgress][0] must be 
            // smaller than the current swing low
            if (swingCur.SwingSlope == -1 && lows[BarsInProgress][0] >= swingLow.CurPrice)
                swingLow.New = false;
            else
                swingLow.New = true;

            // CalculatOnBarClose == true
            if (CalculateOnBarClose)
            {
                if (swingHigh.New
                    && highs[BarsInProgress][0] <
                    (swingLow.CurPrice + swingLow.CurPrice / 100 * swingProp.SwingSize))
                    swingHigh.New = false;

                if (swingLow.New
                    && lows[BarsInProgress][0] >
                    (swingHigh.CurPrice - swingLow.CurPrice / 100 * swingProp.SwingSize))
                    swingLow.New = false;

                // New swing high and new swing low
                if (swingHigh.New && swingLow.New)
                {
                    // Downtrend - ignore the swing high
                    if (swingCur.SwingSlope == -1)
                        swingHigh.New = false;
                    // Uptrend   - ignore the swing low
                    else
                        swingLow.New = false;
                }
            }
            // CalculatOnBarClose == false
            else
            {
                // Used to control, that only one swing is set for 
                // each bar
                if (FirstTickOfBar)
                    swingCur.NewSwing = 0;

                // No swing or an up swing is found
                if (swingCur.NewSwing != -1)
                {
                    // test if Highs[BarsInProgress][0] is higher than the last 
                    // calculationSize highs = new swing high
                    if (swingHigh.New)
                    {
                        if (highs[BarsInProgress][0] <
                            (swingLow.CurPrice + swingLow.CurPrice / 100 * swingProp.SwingSize))
                            swingHigh.New = false;
                        // Found a swing high
                        if (swingHigh.New)
                            swingCur.NewSwing = 1;
                    }
                }

                // No swing or an down swing is found
                if (swingCur.NewSwing != 1)
                {
                    // test if Lows[BarsInProgress][0] is lower than the last 
                    // calculationSize lows = new swing low
                    if (swingLow.New)
                    {
                        if (lows[BarsInProgress][0] >
                            (swingHigh.CurPrice - swingLow.CurPrice / 100 * swingProp.SwingSize))
                            swingLow.New = false;
                        // Found a swing low
                        if (swingLow.New)
                            swingCur.NewSwing = -1;
                    }
                }

                // Set newLow back to false
                if (swingCur.NewSwing == 1)
                    swingLow.New = false;
                // Set newHigh back to false
                if (swingCur.NewSwing == -1)
                    swingHigh.New = false;
            }

            // Swing high
            if (swingHigh.New)
            {
                int bar;
                double price;
                // New swing high
                if (swingCur.SwingSlope != 1)
                {
                    bar = CurrentBars[BarsInProgress] -
                        HighestBar(highs[BarsInProgress], CurrentBars[BarsInProgress] -
                        swingLow.CurBar);
                    price = highs[BarsInProgress][HighestBar(highs[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingLow.CurBar)];
                    swingHigh.Update = false;
                }
                // Update swing high
                else
                {
                    bar = CurrentBars[BarsInProgress];
                    price = highs[BarsInProgress][0];
                    swingHigh.Update = true;
                }
                CalcUpSwing(bar, price, swingHigh.Update, swingHigh, swingCur, swingProp,
                    decimalPlaces);
            }
            // Swing low
            else if (swingLow.New)
            {
                int bar;
                double price;
                // New swing low
                if (swingCur.SwingSlope != -1)
                {
                    bar = CurrentBars[BarsInProgress] - LowestBar(lows[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingHigh.CurBar);
                    price = lows[BarsInProgress][LowestBar(lows[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingHigh.CurBar)];
                    swingLow.Update = false;
                }
                // Update swing low
                else
                {
                    bar = CurrentBars[BarsInProgress];
                    price = lows[BarsInProgress][0];
                    swingLow.Update = true;
                }
                CalcDnSwing(bar, price, swingLow.Update, swingLow, swingCur, swingProp,
                    decimalPlaces);
            }
        }
        //#########################################################################################
        #endregion

        #region Calculate down swing
        //#########################################################################################
        protected void CalcDnSwing(int bar, double low, bool updateLow, Swings swingLow,
            SwingCurrent swingCur, SwingProperties swingProp, int decimalPlaces)
        {
            if (!updateLow)
            {
                swingLow.LastPrice = swingLow.CurPrice;
                swingLow.LastBar = swingLow.CurBar;
                swingLow.LastRelation = swingLow.CurRelation;
                swingLow.Counter++;
                swingCur.SwingSlope = -1;
                swingCur.SwingSlopeChangeBar = bar;
            }
            swingLow.CurBar = bar;
            swingLow.CurPrice = Math.Round(low, decimalPlaces, MidpointRounding.AwayFromZero);
            double dtbOffset = ATR(BarsArray[BarsInProgress], 14)[CurrentBars[BarsInProgress] -
                swingLow.CurBar] * swingProp.DtbStrength / 100;
            if (swingLow.CurPrice > swingLow.LastPrice - dtbOffset && swingLow.CurPrice <
                swingLow.LastPrice + dtbOffset)
                swingLow.CurRelation = 0;
            else if (swingLow.CurPrice < swingLow.LastPrice)
                swingLow.CurRelation = -1;
            else
                swingLow.CurRelation = 1;
        }
        //#########################################################################################
        #endregion

        #region Calculate up swing
        //#########################################################################################
        private void CalcUpSwing(int bar, double high, bool updateHigh, Swings swingHigh,
            SwingCurrent swingCur, SwingProperties swingProp, int decimalPlaces)
        {
            if (!updateHigh)
            {
                swingHigh.LastPrice = swingHigh.CurPrice;
                swingHigh.LastBar = swingHigh.CurBar;
                swingHigh.LastRelation = swingHigh.CurRelation;
                swingHigh.Counter++;
                swingCur.SwingSlope = 1;
                swingCur.SwingSlopeChangeBar = bar;
            }
            swingHigh.CurBar = bar;
            swingHigh.CurPrice = Math.Round(high, decimalPlaces, MidpointRounding.AwayFromZero);

            double dtbOffset = ATR(BarsArray[BarsInProgress], 14)[CurrentBars[BarsInProgress] -
                swingHigh.CurBar] * swingProp.DtbStrength / 100;
            if (swingHigh.CurPrice > swingHigh.LastPrice - dtbOffset && swingHigh.CurPrice <
                swingHigh.LastPrice + dtbOffset)
                swingHigh.CurRelation = 0;
            else if (swingHigh.CurPrice < swingHigh.LastPrice)
                swingHigh.CurRelation = -1;
            else
                swingHigh.CurRelation = 1;
        }
        //#########################################################################################
        #endregion
    }
}
