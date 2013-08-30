#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using System.Collections;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using System.Xml.Serialization;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Enter the description of your new custom indicator here
    /// </summary>
    [Description("Enter the description of your new custom indicator here")]
    public class JAMImpulse100 : Indicator
    {
        #region Variables
        private int strength = 20;
		private double leg2Pullback = .4;
		private double leg4Pullback = .2;
		private double legAPullback = .5;
		private double legCPullback = .2;
		private int eraseBefore = 3;
		private double waveBTolerance = .1;
		private double wave3SlopeTolerance = .1;
		private bool drawLines = true;
		private int fontSize = 12;
		private Color upColor = Color.Green;
		private Color dnColor = Color.Red;
		private int minWaveBars = 3;
		private double leg3Confirm = .5;
		private double textOffset = 1;
        // User defined variables (add any user defined variables below)
		private int lookback = 2;
		string[] waveNum = new string[8]{"1","2","3","4","5","A","B","C"};		
		private DataSeries bias = null;
		private Impulse upimpulse, dnimpulse;
		int[] hbar = new int[20];
		int[] lbar = new int[20];
		public int plotnum = 0;
		private ArrayList patterns = new ArrayList();
        #endregion
		
		public class Impulse
		{	
			public struct Leg
			{
				public int StartBar;
				public int EndBar;
				public double StartPrice;
				public double EndPrice;
				public int Direction;
				public bool Confirmed;	
				public int PlotID;
				public bool WasReset;
				public int WaveLength
				{
					get { return Math.Abs(this.EndBar-this.StartBar); }	
				}
				public double Range
				{
					get { return Math.Abs(this.StartPrice-this.EndPrice); }				
				}
				public double Slope
				{
					get { 
						double rise = (this.EndPrice-this.StartPrice);
						double run = this.EndBar-this.StartBar;
						
						return rise/run;}	
				}
			}
			
			public Leg[] Legs = new Leg[8];	
			
			public void Reset()
			{
				for (int x=0;x<8;x++)
				{								
					Legs[x].Confirmed=false;
					Legs[x].Direction=0;
					Legs[x].EndBar=0;
					Legs[x].EndPrice=0;
					Legs[x].StartBar=0;
					Legs[x].StartPrice=0;					
					Legs[x].PlotID=-1;
					Legs[x].WasReset=false;
				}				
			}
			
			public void Reset(int leg)
			{											
				Legs[leg].Confirmed=false;
				Legs[leg].Direction=0;
				Legs[leg].EndBar=0;
				Legs[leg].EndPrice=0;
				Legs[leg].StartBar=0;
				Legs[leg].StartPrice=0;					
				Legs[leg].PlotID=-1;
				Legs[leg].WasReset=false;
			}
		}
		
        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
//			VendorLicense("SignalTrading","JAMImpulse","www.signaltrading.net","customersupport@signaltrading.net");
            Overlay				= true;			
			bias = new DataSeries(this);
			upimpulse = new Impulse();
			dnimpulse = new Impulse();
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            #region Swing Bars
			
			int cb = CurrentBar;
			int barColor = 0;
			
			if (cb<strength)
				return;						
			
			int maxhigh = 0;
			int minlow = 0;
			bool newhigh=false;
			bool newlow=false;
			if (High[0]>MAX(High,strength)[1])
			{				
				if (lbar[0]>hbar[0])
				{
					for (int x=lookback-1;x>0;x--)
					{
						hbar[x]=hbar[x-1];
						if (High[cb-hbar[x]]>High[cb-maxhigh])
						{
							maxhigh=hbar[x];
						}
					}	
					newhigh=true;
				}											
				hbar[0]=cb;	
				if (High[0]>High[cb-maxhigh])
					maxhigh=cb;
			}
			if (Low[0]<MIN(Low,strength)[1])
			{
				if (hbar[0]>lbar[0])
				{
					for (int x=lookback-1;x>0;x--)
					{
						lbar[x]=lbar[x-1];
						if (Low[cb-lbar[x]]<Low[cb-minlow])
						{
							minlow=lbar[x];	
						}
					}
					newlow=true;
				}								
				lbar[0]=cb;	
				if (Low[0]<Low[cb-minlow])
					minlow=cb;
			}
			
			#endregion
			
			if (Math.Max(lbar[lookback-1],hbar[lookback-1])<=0)
				return;												
			
			#region Find Possible Leg1's 
			
			// identify new swing highs			
			if (newhigh && Low[cb-lbar[0]]<Low[cb-lbar[1]])
			{
				Impulse imp = new Impulse();				
				imp.Legs[0].StartBar=lbar[0];
				imp.Legs[0].StartPrice=Low[cb-lbar[0]];
				imp.Legs[0].Direction=1;
				imp.Legs[0].EndBar=cb;
				imp.Legs[0].EndPrice=High[0];
				imp.Legs[0].Confirmed=false;
				
				patterns.Add(imp);
			}
			
			// identify new swing lows			
			if (newlow && High[cb-hbar[0]]>High[cb-hbar[1]])
			{
				Impulse imp = new Impulse();				
				imp.Legs[0].StartBar=hbar[0];
				imp.Legs[0].StartPrice=High[cb-hbar[0]];
				imp.Legs[0].Direction=-1;
				imp.Legs[0].EndBar=cb;
				imp.Legs[0].EndPrice=Low[0];
				imp.Legs[0].Confirmed=false;
				
				patterns.Add(imp);
			}
			
			#endregion 
			
			foreach (Impulse i in patterns.ToArray())
			{										
				#region Leg1
												
				if (!i.Legs[0].Confirmed)
				{
					#region up
					
					if (i.Legs[0].Direction==1)
					{
						// store new highs for leg1
						if (High[0]>i.Legs[0].EndPrice)
						{
							i.Legs[0].EndBar=cb;	
							i.Legs[0].EndPrice=High[0];
						}
						
						// reset if starting point is broken
						else if (Low[0]<i.Legs[0].StartPrice)
						{
							erasePlots(i);
							i.Reset();
							//patterns.Remove(i);
							continue;
						}
						
						// Confirm leg 1
						if (Close[0]<i.Legs[0].EndPrice-leg2Pullback*i.Legs[0].Range && cb-i.Legs[0].EndBar>=minWaveBars)
						{
							plotnum++;
							int legNum=0;
							i.Legs[legNum].PlotID=plotnum;
							i.Legs[legNum].Confirmed=true;										
							i.Legs[legNum+1].StartBar=i.Legs[legNum].EndBar;
							i.Legs[legNum+1].StartPrice=i.Legs[legNum].EndPrice;
							i.Legs[legNum+1].EndBar= cb;
							i.Legs[legNum+1].EndPrice=i.Legs[legNum].Direction==1 ? Low[0] : High[0];
							i.Legs[legNum+1].Confirmed=false;
							i.Legs[legNum+1].Direction=i.Legs[legNum].Direction*-1;
							PlotLeg(i,legNum);	
						}
					}
								
					#endregion
				
					#region down
				
				
					if (i.Legs[0].Direction==-1)
					{
						// store new highs for leg1
						if (Low[0]<i.Legs[0].EndPrice)
						{
							i.Legs[0].EndBar=cb;	
							i.Legs[0].EndPrice=Low[0];
						}
						
						// reset if starting point is broken
						else if (High[0]>i.Legs[0].StartPrice)
						{
							erasePlots(i);
							i.Reset();
							//patterns.Remove(i);
							continue;
						}
						
						// Confirm leg 1
						if (Close[0]>i.Legs[0].EndPrice+leg2Pullback*i.Legs[0].Range && cb-i.Legs[0].EndBar>=minWaveBars)
						{
							plotnum++;
							int legNum=0;
							i.Legs[legNum].PlotID=plotnum;
							i.Legs[legNum].Confirmed=true;										
							i.Legs[legNum+1].StartBar=i.Legs[legNum].EndBar;
							i.Legs[legNum+1].StartPrice=i.Legs[legNum].EndPrice;
							i.Legs[legNum+1].EndBar= cb;
							i.Legs[legNum+1].EndPrice=i.Legs[legNum].Direction==1 ? Low[0] : High[0];
							i.Legs[legNum+1].Confirmed=false;
							i.Legs[legNum+1].Direction=i.Legs[legNum].Direction*-1;
							PlotLeg(i,legNum);	
						}
					}				
					
					#endregion
				}
				
				#endregion
			
				#region Leg2								
				
				else if (!i.Legs[1].Confirmed)
				{
					#region up
					
					if (i.Legs[1].Direction==-1)
					{
						// store new lows for leg2
						if (Low[0]<i.Legs[1].EndPrice)
						{
							i.Legs[1].EndBar=cb;
							i.Legs[1].EndPrice=Low[0];
						}
						
						// reset
						if (Low[0]<i.Legs[0].StartPrice)
						{
							erasePlots(i);
							i.Reset();
							patterns.Remove(i);
							continue;
						}
						
						// Confirm leg 2
						//Print(Time[0].ToString() + ", " + upimpulse.Legs[1].WaveLength.ToString());
						if (High[0]>i.Legs[0].EndPrice && cb-i.Legs[1].EndBar>=minWaveBars)
						{
							plotnum++;
							int legNum=1;
							i.Legs[legNum].PlotID=plotnum;
							i.Legs[legNum].Confirmed=true;										
							i.Legs[legNum+1].StartBar=i.Legs[legNum].EndBar;
							i.Legs[legNum+1].StartPrice=i.Legs[legNum].EndPrice;
							i.Legs[legNum+1].EndBar= cb;
							i.Legs[legNum+1].EndPrice=i.Legs[legNum].Direction==1 ? Low[0] : High[0];
							i.Legs[legNum+1].Confirmed=false;
							i.Legs[legNum+1].Direction=i.Legs[legNum].Direction*-1;
							PlotLeg(i,legNum);									
						}
					}							
				
					#endregion
				
					#region down

					if (i.Legs[1].Direction==1)
					{
						// store new highs for leg2
						if (High[0]>i.Legs[1].EndPrice)
						{
							i.Legs[1].EndBar=cb;
							i.Legs[1].EndPrice=High[0];
						}
						
						// reset
						if (High[0]>i.Legs[0].StartPrice)
						{
							erasePlots(i);
							i.Reset();
							patterns.Remove(i);
							continue;
						}
						
						// Confirm leg 2
						//Print(Time[0].ToString() + ", " + upimpulse.Legs[1].WaveLength.ToString());
						if (Low[0]<i.Legs[0].EndPrice && cb-i.Legs[1].EndBar>=minWaveBars)
						{
							plotnum++;
							int legNum=1;
							i.Legs[legNum].PlotID=plotnum;
							i.Legs[legNum].Confirmed=true;										
							i.Legs[legNum+1].StartBar=i.Legs[legNum].EndBar;
							i.Legs[legNum+1].StartPrice=i.Legs[legNum].EndPrice;
							i.Legs[legNum+1].EndBar= cb;
							i.Legs[legNum+1].EndPrice=i.Legs[legNum].Direction==1 ? Low[0] : High[0];
							i.Legs[legNum+1].Confirmed=false;
							i.Legs[legNum+1].Direction=i.Legs[legNum].Direction*-1;
							PlotLeg(i,legNum);									
						}
					}
					
					#endregion
				}								
				
				#endregion
				
				#region Leg3								
				
				else if (!i.Legs[2].Confirmed)
				{
					#region up
					
					if (i.Legs[2].Direction==1)
					{						 							
						// store new highs for leg3					
						if (High[0]>i.Legs[2].EndPrice)	
						{
							i.Legs[2].EndBar=cb;
							i.Legs[2].EndPrice=High[0];
							i.Legs[2].WasReset=false;
							continue;;
						}	
						
						if (Close[0]<i.Legs[1].EndPrice)
						{
							erasePlots(i);
							i.Reset();
							patterns.Remove(i);
							continue;
						}												
						
						// Confirm leg 3
						if (!i.Legs[2].WasReset && i.Legs[2].EndPrice>i.Legs[0].StartPrice+(i.Legs[0].Range*(1+leg3Confirm))
							&& Close[0]<i.Legs[2].EndPrice-leg4Pullback*i.Legs[2].Range && cb-i.Legs[2].EndBar>=minWaveBars)
						{
							if (i.Legs[2].Slope>=i.Legs[0].Slope*(1-wave3SlopeTolerance))
							{
//								erasePlots(i);
//								i.Reset();
								continue;
							}
							plotnum++;
							int legNum=2;
							i.Legs[legNum].PlotID=plotnum;
							i.Legs[legNum].Confirmed=true;	
							i.Legs[legNum].WasReset=false;
							i.Legs[legNum+1].StartBar=i.Legs[legNum].EndBar;
							i.Legs[legNum+1].StartPrice=i.Legs[legNum].EndPrice;
							i.Legs[legNum+1].EndBar= cb;
							i.Legs[legNum+1].EndPrice=i.Legs[legNum].Direction==1 ? Low[0] : High[0];
							i.Legs[legNum+1].Confirmed=false;
							i.Legs[legNum+1].WasReset=false;
							i.Legs[legNum+1].Direction=i.Legs[legNum].Direction*-1;
							PlotLeg(i,legNum);
						}
					}
					
					#endregion												
				
					#region down
								
					if (i.Legs[2].Direction==-1)
					{						 							
						// store new lows for leg3					
						if (Low[0]<i.Legs[2].EndPrice)	
						{
							i.Legs[2].EndBar=cb;
							i.Legs[2].EndPrice=Low[0];
							i.Legs[2].WasReset=false;
							continue;;
						}	
						
						if (Close[0]>i.Legs[1].EndPrice)
						{
							erasePlots(i);
							i.Reset();
							patterns.Remove(i);
							continue;
						}												
						
						// Confirm leg 3
//						Print(Time[0].ToString() + ", " + i.Legs[2].WasReset + ", " + i.Legs[2].EndPrice);
						if (!i.Legs[2].WasReset && 
							i.Legs[2].EndPrice<i.Legs[0].StartPrice-(i.Legs[0].Range*(1+leg3Confirm))
							&& Close[0]>i.Legs[2].EndPrice+leg4Pullback*i.Legs[2].Range && cb-i.Legs[2].EndBar>=minWaveBars)
						{
//							Print(Time[0].ToString() + ", " + i.Legs[2].Slope*100000 + ", " + i.Legs[0].Slope*(1-wave3SlopeTolerance)*100000);
							if (i.Legs[2].Slope>=i.Legs[0].Slope*(1-wave3SlopeTolerance))
							{
//								erasePlots(i);
//								i.Reset();
								continue;
							}
							plotnum++;
							int legNum=2;
							i.Legs[legNum].PlotID=plotnum;
							i.Legs[legNum].Confirmed=true;	
							i.Legs[legNum].WasReset=false;
							i.Legs[legNum+1].StartBar=i.Legs[legNum].EndBar;
							i.Legs[legNum+1].StartPrice=i.Legs[legNum].EndPrice;
							i.Legs[legNum+1].EndBar= cb;
							i.Legs[legNum+1].EndPrice=i.Legs[legNum].Direction==1 ? Low[0] : High[0];
							i.Legs[legNum+1].Confirmed=false;
							i.Legs[legNum+1].WasReset=false;
							i.Legs[legNum+1].Direction=i.Legs[legNum].Direction*-1;
							PlotLeg(i,legNum);
							
							Print(Time[0].ToString() + ": Leg Plotted");
						}
					}
					
					#endregion
					
				}								
				
				#endregion
				
				#region Leg4								
				
				else if (!i.Legs[3].Confirmed)
				{
					#region up
					
					if (i.Legs[3].Direction==-1)
					{
						// store new lows for leg4
						if (Low[0]<i.Legs[3].EndPrice)
						{
							i.Legs[3].EndBar=cb;
							i.Legs[3].EndPrice=Low[0];
						}
						
						// reset
						if (Low[0]<i.Legs[0].EndPrice)
						{
							i.Legs[2].Confirmed=false;
							i.Legs[2].WasReset=true;		
							
							// added 11.10.10
//							i.Legs[2].EndBar=cb;
//							i.Legs[2].EndPrice=High[0];
							// end added 11.10.10
							
							erasePlots(i.Legs[2]);						
						}
						
						// Confirm leg 4
						if (High[0]>i.Legs[3].StartPrice && i.Legs[3].WaveLength>=minWaveBars)
						{
							plotnum++;
							int legNum=3;
							i.Legs[legNum].PlotID=plotnum;
							i.Legs[legNum].Confirmed=true;										
							i.Legs[legNum+1].StartBar=i.Legs[legNum].EndBar;
							i.Legs[legNum+1].StartPrice=i.Legs[legNum].EndPrice;
							i.Legs[legNum+1].EndBar= cb;
							i.Legs[legNum+1].EndPrice=i.Legs[legNum].Direction==1 ? Low[0] : High[0];
							i.Legs[legNum+1].Confirmed=false;
							i.Legs[legNum+1].Direction=i.Legs[legNum].Direction*-1;
							PlotLeg(i,legNum);								
						}
					}
					
					#endregion											
				
					#region down

					if (i.Legs[3].Direction==1)
					{
						// store new highs for leg4
						if (High[0]>i.Legs[3].EndPrice)
						{
							i.Legs[3].EndBar=cb;
							i.Legs[3].EndPrice=High[0];
						}
						
						// reset
						if (High[0]>i.Legs[0].EndPrice)
						{							
							i.Legs[2].Confirmed=false;
							i.Legs[2].WasReset=true;		
							
							// added 11.10.10
//							i.Legs[2].EndBar=cb;
//							i.Legs[2].EndPrice=Low[0];
							// end added 11.10.10
							
							erasePlots(i.Legs[2]);						
						}
						
						// Confirm leg 4
						if (Low[0]<i.Legs[3].StartPrice && cb-i.Legs[3].EndBar>=minWaveBars)
						{
							plotnum++;
							int legNum=3;
							i.Legs[legNum].PlotID=plotnum;
							i.Legs[legNum].Confirmed=true;										
							i.Legs[legNum+1].StartBar=i.Legs[legNum].EndBar;
							i.Legs[legNum+1].StartPrice=i.Legs[legNum].EndPrice;
							i.Legs[legNum+1].EndBar= cb;
							i.Legs[legNum+1].EndPrice=i.Legs[legNum].Direction==1 ? Low[0] : High[0];
							i.Legs[legNum+1].Confirmed=false;
							i.Legs[legNum+1].Direction=i.Legs[legNum].Direction*-1;
							PlotLeg(i,legNum);								
						}
					}
					
					#endregion
				}								
				
				#endregion
				
				#region Leg5
				
				else if (!i.Legs[4].Confirmed)
				{
					#region up								
					
					if (i.Legs[4].Direction==1)
					{
						// store new highs for leg5					
						if (High[0]>i.Legs[4].EndPrice)	
						{
							i.Legs[4].EndBar=cb;
							i.Legs[4].EndPrice=High[0];
						}		
						
						if (Low[0]<i.Legs[4].StartPrice)
						{
							erasePlots(i);
							i.Reset();
							patterns.Remove(i);
							continue;
						}
						
						// Confirm leg 5
						if (Close[0]<i.Legs[4].EndPrice-legAPullback*i.Legs[4].Range && cb-i.Legs[4].EndBar>=minWaveBars)
						{
							plotnum++;
							int legNum=4;
							i.Legs[legNum].PlotID=plotnum;
							i.Legs[legNum].Confirmed=true;										
							i.Legs[legNum+1].StartBar=i.Legs[legNum].EndBar;
							i.Legs[legNum+1].StartPrice=i.Legs[legNum].EndPrice;
							i.Legs[legNum+1].EndBar= cb;
							i.Legs[legNum+1].EndPrice=i.Legs[legNum].Direction==1 ? Low[0] : High[0];
							i.Legs[legNum+1].Confirmed=false;
							i.Legs[legNum+1].Direction=i.Legs[legNum].Direction*-1;
							PlotLeg(i,legNum);																		
						}
					}
					
					#endregion
												
					#region down
								
					if (i.Legs[4].Direction==-1)
					{
						// store new lows for leg5					
						if (Low[0]<i.Legs[4].EndPrice)	
						{
							i.Legs[4].EndBar=cb;
							i.Legs[4].EndPrice=Low[0];
						}		
						
						if (High[0]>i.Legs[4].StartPrice)
						{
							erasePlots(i);
							i.Reset();
							patterns.Remove(i);
							continue;
						}
						
						// Confirm leg 5
						if (Close[0]>i.Legs[4].EndPrice+legAPullback*i.Legs[4].Range && cb-i.Legs[4].EndBar>=minWaveBars)
						{
							plotnum++;
							int legNum=4;
							i.Legs[legNum].PlotID=plotnum;
							i.Legs[legNum].Confirmed=true;										
							i.Legs[legNum+1].StartBar=i.Legs[legNum].EndBar;
							i.Legs[legNum+1].StartPrice=i.Legs[legNum].EndPrice;
							i.Legs[legNum+1].EndBar= cb;
							i.Legs[legNum+1].EndPrice=i.Legs[legNum].Direction==1 ? Low[0] : High[0];
							i.Legs[legNum+1].Confirmed=false;
							i.Legs[legNum+1].Direction=i.Legs[legNum].Direction*-1;
							PlotLeg(i,legNum);																		
						}
					}
					
					#endregion
				}								
				
				#endregion
				
				#region Leg6 - A								
				
				else if (!i.Legs[5].Confirmed)
				{
					#region up
					
					if (i.Legs[5].Direction==-1)
					{
						// store new highs for leg6					
						if (Low[0]<i.Legs[5].EndPrice)	
						{
							i.Legs[5].EndBar=cb;
							i.Legs[5].EndPrice=Low[0];
						}		
						
						if (Low[0]<i.Legs[4].StartPrice)
						{
	//						erasePlots(upimpulse);
	//						upimpulse.Reset();
	//						return;
							
							i.Legs[3].Confirmed=false;							
							i.Legs[3].EndPrice=Low[0];
							i.Legs[3].EndBar=cb;
							erasePlots(i.Legs[3]);						
							erasePlots(i.Legs[4]);
							continue;
						}
						
						// Confirm leg 6
						if (Close[0]>i.Legs[5].EndPrice+leg2Pullback*i.Legs[5].Range && cb-i.Legs[5].EndBar>=minWaveBars)
						{
							plotnum++;
							int legNum=5;
							i.Legs[legNum].PlotID=plotnum;
							i.Legs[legNum].Confirmed=true;										
							i.Legs[legNum+1].StartBar=i.Legs[legNum].EndBar;
							i.Legs[legNum+1].StartPrice=i.Legs[legNum].EndPrice;
							i.Legs[legNum+1].EndBar= cb;
							i.Legs[legNum+1].EndPrice=i.Legs[legNum].Direction==1 ? Low[0] : High[0];
							i.Legs[legNum+1].Confirmed=false;
							i.Legs[legNum+1].Direction=i.Legs[legNum].Direction*-1;
							PlotLeg(i,legNum);												
						}
					}
					
					#endregion								
				
					#region down
								
					if (i.Legs[5].Direction==1)
					{
						// store new highs for leg6					
						if (High[0]>i.Legs[5].EndPrice)	
						{
							i.Legs[5].EndBar=cb;
							i.Legs[5].EndPrice=High[0];
						}		
						
						if (High[0]>i.Legs[4].StartPrice)
						{
	//						erasePlots(upimpulse);
	//						upimpulse.Reset();
	//						return;
							
							i.Legs[3].Confirmed=false;							
							i.Legs[3].EndPrice=High[0];
							i.Legs[3].EndBar=cb;
							erasePlots(i.Legs[3]);						
							erasePlots(i.Legs[4]);
							continue;
						}
						
						// Confirm leg 6
						if (Close[0]<i.Legs[5].EndPrice-leg2Pullback*i.Legs[5].Range && cb-i.Legs[5].EndBar>=minWaveBars)
						{
							plotnum++;
							int legNum=5;
							i.Legs[legNum].PlotID=plotnum;
							i.Legs[legNum].Confirmed=true;										
							i.Legs[legNum+1].StartBar=i.Legs[legNum].EndBar;
							i.Legs[legNum+1].StartPrice=i.Legs[legNum].EndPrice;
							i.Legs[legNum+1].EndBar= cb;
							i.Legs[legNum+1].EndPrice=i.Legs[legNum].Direction==1 ? Low[0] : High[0];
							i.Legs[legNum+1].Confirmed=false;
							i.Legs[legNum+1].Direction=i.Legs[legNum].Direction*-1;
							PlotLeg(i,legNum);												
						}
					}
					
					#endregion
				}
				
				
				
				#endregion
				
				#region Leg7 - B
												
				else if (!i.Legs[6].Confirmed)
				{
					#region up
					
					if (i.Legs[6].Direction==1)
					{
						// store new highs for leg7					
						if (High[0]>i.Legs[6].EndPrice)	
						{
							i.Legs[6].EndBar=cb;
							i.Legs[6].EndPrice=High[0];
						}		
						
						if (High[0]>i.Legs[5].StartPrice+waveBTolerance*i.Legs[4].Range)
						{
	//						erasePlots(upimpulse);
	//						upimpulse.Reset();
	//						return;
							i.Legs[4].Confirmed=false;
							i.Legs[5].Confirmed=false;	
							i.Legs[4].EndPrice=High[0];
							i.Legs[4].EndBar=cb;
							erasePlots(i.Legs[4]);
							erasePlots(i.Legs[5]);
							continue;
						}
						
						// Confirm leg 7
						if (Low[0]<i.Legs[5].EndPrice && cb-i.Legs[6].EndBar>=minWaveBars)
						{
							plotnum++;
							int legNum=6;
							i.Legs[legNum].PlotID=plotnum;
							i.Legs[legNum].Confirmed=true;										
							i.Legs[legNum+1].StartBar=i.Legs[legNum].EndBar;
							i.Legs[legNum+1].StartPrice=i.Legs[legNum].EndPrice;
							i.Legs[legNum+1].EndBar= cb;
							i.Legs[legNum+1].EndPrice=i.Legs[legNum].Direction==1 ? Low[0] : High[0];
							i.Legs[legNum+1].Confirmed=false;
							i.Legs[legNum+1].Direction=i.Legs[legNum].Direction*-1;
							PlotLeg(i,legNum);						
						}
					}
					
					#endregion								
				
					#region down
					
					if (i.Legs[6].Direction==-1)
					{
						// store new lows for leg7					
						if (Low[0]<i.Legs[6].EndPrice)	
						{
							i.Legs[6].EndBar=cb;
							i.Legs[6].EndPrice=Low[0];
						}		
						
						if (Low[0]<i.Legs[5].StartPrice-waveBTolerance*i.Legs[4].Range)
						{
	//						erasePlots(upimpulse);
	//						upimpulse.Reset();
	//						return;
							i.Legs[4].Confirmed=false;
							i.Legs[5].Confirmed=false;	
							i.Legs[4].EndPrice=Low[0];
							i.Legs[4].EndBar=cb;
							erasePlots(i.Legs[4]);
							erasePlots(i.Legs[5]);
							continue;
						}
						
						// Confirm leg 7
						if (High[0]>i.Legs[5].EndPrice && cb-i.Legs[6].EndBar>=minWaveBars)
						{
							plotnum++;
							int legNum=6;
							i.Legs[legNum].PlotID=plotnum;
							i.Legs[legNum].Confirmed=true;										
							i.Legs[legNum+1].StartBar=i.Legs[legNum].EndBar;
							i.Legs[legNum+1].StartPrice=i.Legs[legNum].EndPrice;
							i.Legs[legNum+1].EndBar= cb;
							i.Legs[legNum+1].EndPrice=i.Legs[legNum].Direction==1 ? Low[0] : High[0];
							i.Legs[legNum+1].Confirmed=false;
							i.Legs[legNum+1].Direction=i.Legs[legNum].Direction*-1;
							PlotLeg(i,legNum);						
						}
					}
					
					#endregion
				}								
				
				#endregion
				
				#region Leg8 - C
												
				else if (!i.Legs[7].Confirmed)
				{
					#region up
					
					if (i.Legs[7].Direction==-1)
					{												
						if (High[0]>i.Legs[6].EndPrice)
						{
							erasePlots(i);
							i.Reset();
							//patterns.Remove(i);
							continue;
						}
						
						// Confirm leg 8
						if (Close[0]>i.Legs[7].EndPrice+legCPullback*i.Legs[7].Range  && cb-i.Legs[7].EndBar>=minWaveBars)
						{
							plotnum++;
							int legNum=7;
							i.Legs[legNum].PlotID=plotnum;
							i.Legs[legNum].Confirmed=true;																	
							PlotLeg(i,legNum);					
							i.Reset();
							patterns.Remove(i);
						}
						
						// store new highs for leg8					
						if (Low[0]<i.Legs[7].EndPrice)	
						{
							i.Legs[7].EndBar=cb;
							i.Legs[7].EndPrice=Low[0];
						}
						
					}
					
					#endregion								
				
					#region down
								
					if (i.Legs[7].Direction==1)
					{												
						if (Low[0]<i.Legs[6].EndPrice)
						{
							erasePlots(i);
							i.Reset();
							//patterns.Remove(i);
							continue;
						}
						
						// Confirm leg 8
						if (Close[0]<i.Legs[7].EndPrice-legCPullback*i.Legs[7].Range && cb-i.Legs[7].EndBar>=minWaveBars)
						{
							plotnum++;
							int legNum=7;
							i.Legs[legNum].PlotID=plotnum;
							i.Legs[legNum].Confirmed=true;																	
							PlotLeg(i,legNum);					
							i.Reset();
							patterns.Remove(i);
						}
						
						// store new highs for leg8					
						if (High[0]>i.Legs[7].EndPrice)	
						{
							i.Legs[7].EndBar=cb;
							i.Legs[7].EndPrice=High[0];
						}
						
					}
					
					#endregion
				}								
				
				#endregion					
			}
        }
		
		public void PlotLeg(Impulse imp, int legNum)
		{		
//			plotnum++;
//			imp.Legs[legNum].PlotID=plotnum;
			int cb = CurrentBar;
			Impulse.Leg leg = imp.Legs[legNum];
			double offset = leg.Direction*textOffset*TickSize*(legNum+1);
			string text = waveNum[legNum];// + " - " + imp.Legs[legNum].Slope.ToString();
			Color clr = imp.Legs[0].Direction==1 ? upColor : dnColor;
			DrawText(plotnum.ToString(),true,text,cb-leg.EndBar,leg.EndPrice+offset,0,clr,new Font("Arial",fontSize,FontStyle.Bold),StringAlignment.Center,Color.Transparent,Color.Transparent,2);
			if (drawLines)
				DrawLine(plotnum.ToString()+"1",true,cb-leg.StartBar,leg.StartPrice,cb-leg.EndBar,leg.EndPrice,clr,DashStyle.Solid,2);	
			
//			imp.Legs[legNum].Confirmed=true;
//			
//			if (legNum<imp.Legs.GetUpperBound(0))
//			{
//				imp.Legs[legNum+1].StartBar=leg.EndBar;
//				imp.Legs[legNum+1].StartPrice=leg.EndPrice;
//				imp.Legs[legNum+1].EndBar= cb;
//				imp.Legs[legNum+1].EndPrice=leg.Direction==1 ? Low[0] : High[0];
//				imp.Legs[legNum+1].Confirmed=false;
//				imp.Legs[legNum+1].Direction=leg.Direction*-1;
//			}
		}				
		
		void erasePlots(Impulse imp)
		{
			for (int x=0; x<8; x++)
			{
				if (!imp.Legs[eraseBefore-1].Confirmed)
				{
					RemoveDrawObject(imp.Legs[x].PlotID.ToString());
					RemoveDrawObject(imp.Legs[x].PlotID.ToString()+"1");
				}
			}
		}
		
		void erasePlots(Impulse.Leg leg)
		{
			RemoveDrawObject(leg.PlotID.ToString());
			RemoveDrawObject(leg.PlotID.ToString()+"1");
		}

        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int Strength
        {
            get { return strength; }
            set { strength = Math.Max(1, value); }
        }
		
		[Description("")]
        [GridCategory("Parameters")]
        public double Leg2Pullback
        {
            get { return leg2Pullback; }
            set { leg2Pullback = Math.Max(0, value); }
        }
		
		[Description("")]
        [GridCategory("Parameters")]
        public double Leg4Pullback
        {
            get { return leg4Pullback; }
            set { leg4Pullback = Math.Max(0, value); }
        }
		
		[Description("")]
        [GridCategory("Parameters")]
        public double LegAPullback
        {
            get { return legAPullback; }
            set { legAPullback = Math.Max(0, value); }
        }
		
		[Description("")]
        [GridCategory("Parameters")]
        public double LegCPullback
        {
            get { return legCPullback; }
            set { legCPullback = Math.Max(0, value); }
        }
		
		[Description("")]
        [GridCategory("Parameters")]
        public double Leg3Confirm
        {
            get { return leg3Confirm; }
            set { leg3Confirm = Math.Max(0, value); }
        }
		
		[Description("Historical plots will be removed if we have not exceeded this wave count. 0 will disable and plot all historical waves.")]
        [GridCategory("Parameters")]
        public int EraseBefore
        {
            get { return eraseBefore; }
            set { eraseBefore = Math.Max(0, value); }
        }
		
		[Description("")]
        [GridCategory("Parameters")]
        public double WaveBTolerance
        {
            get { return waveBTolerance; }
            set { waveBTolerance = value; }
        }
		
		[Description("")]
        [GridCategory("Parameters")]
        public bool DrawLines
        {
            get { return drawLines; }
            set { drawLines = value; }
        }
		
		[Browsable(false)]
		public string UpColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(upColor); }
			set { upColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[XmlIgnore()]
		[Description("")]
        [Category("Parameters")]		
        public Color UpColor
        {
            get { return upColor; }
            set { upColor = value; }
        }
		
		[Browsable(false)]
		public string DownColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(dnColor); }
			set { dnColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[XmlIgnore()]
		[Description("")]
        [Category("Parameters")]		
        public Color DownColor
        {
            get { return dnColor; }
            set { dnColor = value; }
        }
		
		[Description("")]
        [GridCategory("Parameters")]
        public int FontSize
        {
            get { return fontSize; }
            set { fontSize = Math.Max(0, value); }
        }
		
		[Description("")]
        [GridCategory("Parameters")]
        public double Wave3SlopeTolerance
        {
            get { return wave3SlopeTolerance; }
            set { wave3SlopeTolerance = value; }
        }	
		
		[Description("")]
        [GridCategory("Parameters")]
        public int MinWaveBars
        {
            get { return minWaveBars; }
            set { minWaveBars = Math.Max(0, value); }
        }
		
		[Description("")]
        [GridCategory("Parameters")]
        public double TextOffset
        {
            get { return textOffset; }
            set { textOffset = Math.Max(0, value); }
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
        private JAMImpulse100[] cacheJAMImpulse100 = null;

        private static JAMImpulse100 checkJAMImpulse100 = new JAMImpulse100();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public JAMImpulse100 JAMImpulse100(Color downColor, bool drawLines, int eraseBefore, int fontSize, double leg2Pullback, double leg3Confirm, double leg4Pullback, double legAPullback, double legCPullback, int minWaveBars, int strength, double textOffset, Color upColor, double wave3SlopeTolerance, double waveBTolerance)
        {
            return JAMImpulse100(Input, downColor, drawLines, eraseBefore, fontSize, leg2Pullback, leg3Confirm, leg4Pullback, legAPullback, legCPullback, minWaveBars, strength, textOffset, upColor, wave3SlopeTolerance, waveBTolerance);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public JAMImpulse100 JAMImpulse100(Data.IDataSeries input, Color downColor, bool drawLines, int eraseBefore, int fontSize, double leg2Pullback, double leg3Confirm, double leg4Pullback, double legAPullback, double legCPullback, int minWaveBars, int strength, double textOffset, Color upColor, double wave3SlopeTolerance, double waveBTolerance)
        {
            if (cacheJAMImpulse100 != null)
                for (int idx = 0; idx < cacheJAMImpulse100.Length; idx++)
                    if (cacheJAMImpulse100[idx].DownColor == downColor && cacheJAMImpulse100[idx].DrawLines == drawLines && cacheJAMImpulse100[idx].EraseBefore == eraseBefore && cacheJAMImpulse100[idx].FontSize == fontSize && Math.Abs(cacheJAMImpulse100[idx].Leg2Pullback - leg2Pullback) <= double.Epsilon && Math.Abs(cacheJAMImpulse100[idx].Leg3Confirm - leg3Confirm) <= double.Epsilon && Math.Abs(cacheJAMImpulse100[idx].Leg4Pullback - leg4Pullback) <= double.Epsilon && Math.Abs(cacheJAMImpulse100[idx].LegAPullback - legAPullback) <= double.Epsilon && Math.Abs(cacheJAMImpulse100[idx].LegCPullback - legCPullback) <= double.Epsilon && cacheJAMImpulse100[idx].MinWaveBars == minWaveBars && cacheJAMImpulse100[idx].Strength == strength && Math.Abs(cacheJAMImpulse100[idx].TextOffset - textOffset) <= double.Epsilon && cacheJAMImpulse100[idx].UpColor == upColor && Math.Abs(cacheJAMImpulse100[idx].Wave3SlopeTolerance - wave3SlopeTolerance) <= double.Epsilon && Math.Abs(cacheJAMImpulse100[idx].WaveBTolerance - waveBTolerance) <= double.Epsilon && cacheJAMImpulse100[idx].EqualsInput(input))
                        return cacheJAMImpulse100[idx];

            lock (checkJAMImpulse100)
            {
                checkJAMImpulse100.DownColor = downColor;
                downColor = checkJAMImpulse100.DownColor;
                checkJAMImpulse100.DrawLines = drawLines;
                drawLines = checkJAMImpulse100.DrawLines;
                checkJAMImpulse100.EraseBefore = eraseBefore;
                eraseBefore = checkJAMImpulse100.EraseBefore;
                checkJAMImpulse100.FontSize = fontSize;
                fontSize = checkJAMImpulse100.FontSize;
                checkJAMImpulse100.Leg2Pullback = leg2Pullback;
                leg2Pullback = checkJAMImpulse100.Leg2Pullback;
                checkJAMImpulse100.Leg3Confirm = leg3Confirm;
                leg3Confirm = checkJAMImpulse100.Leg3Confirm;
                checkJAMImpulse100.Leg4Pullback = leg4Pullback;
                leg4Pullback = checkJAMImpulse100.Leg4Pullback;
                checkJAMImpulse100.LegAPullback = legAPullback;
                legAPullback = checkJAMImpulse100.LegAPullback;
                checkJAMImpulse100.LegCPullback = legCPullback;
                legCPullback = checkJAMImpulse100.LegCPullback;
                checkJAMImpulse100.MinWaveBars = minWaveBars;
                minWaveBars = checkJAMImpulse100.MinWaveBars;
                checkJAMImpulse100.Strength = strength;
                strength = checkJAMImpulse100.Strength;
                checkJAMImpulse100.TextOffset = textOffset;
                textOffset = checkJAMImpulse100.TextOffset;
                checkJAMImpulse100.UpColor = upColor;
                upColor = checkJAMImpulse100.UpColor;
                checkJAMImpulse100.Wave3SlopeTolerance = wave3SlopeTolerance;
                wave3SlopeTolerance = checkJAMImpulse100.Wave3SlopeTolerance;
                checkJAMImpulse100.WaveBTolerance = waveBTolerance;
                waveBTolerance = checkJAMImpulse100.WaveBTolerance;

                if (cacheJAMImpulse100 != null)
                    for (int idx = 0; idx < cacheJAMImpulse100.Length; idx++)
                        if (cacheJAMImpulse100[idx].DownColor == downColor && cacheJAMImpulse100[idx].DrawLines == drawLines && cacheJAMImpulse100[idx].EraseBefore == eraseBefore && cacheJAMImpulse100[idx].FontSize == fontSize && Math.Abs(cacheJAMImpulse100[idx].Leg2Pullback - leg2Pullback) <= double.Epsilon && Math.Abs(cacheJAMImpulse100[idx].Leg3Confirm - leg3Confirm) <= double.Epsilon && Math.Abs(cacheJAMImpulse100[idx].Leg4Pullback - leg4Pullback) <= double.Epsilon && Math.Abs(cacheJAMImpulse100[idx].LegAPullback - legAPullback) <= double.Epsilon && Math.Abs(cacheJAMImpulse100[idx].LegCPullback - legCPullback) <= double.Epsilon && cacheJAMImpulse100[idx].MinWaveBars == minWaveBars && cacheJAMImpulse100[idx].Strength == strength && Math.Abs(cacheJAMImpulse100[idx].TextOffset - textOffset) <= double.Epsilon && cacheJAMImpulse100[idx].UpColor == upColor && Math.Abs(cacheJAMImpulse100[idx].Wave3SlopeTolerance - wave3SlopeTolerance) <= double.Epsilon && Math.Abs(cacheJAMImpulse100[idx].WaveBTolerance - waveBTolerance) <= double.Epsilon && cacheJAMImpulse100[idx].EqualsInput(input))
                            return cacheJAMImpulse100[idx];

                JAMImpulse100 indicator = new JAMImpulse100();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.DownColor = downColor;
                indicator.DrawLines = drawLines;
                indicator.EraseBefore = eraseBefore;
                indicator.FontSize = fontSize;
                indicator.Leg2Pullback = leg2Pullback;
                indicator.Leg3Confirm = leg3Confirm;
                indicator.Leg4Pullback = leg4Pullback;
                indicator.LegAPullback = legAPullback;
                indicator.LegCPullback = legCPullback;
                indicator.MinWaveBars = minWaveBars;
                indicator.Strength = strength;
                indicator.TextOffset = textOffset;
                indicator.UpColor = upColor;
                indicator.Wave3SlopeTolerance = wave3SlopeTolerance;
                indicator.WaveBTolerance = waveBTolerance;
                Indicators.Add(indicator);
                indicator.SetUp();

                JAMImpulse100[] tmp = new JAMImpulse100[cacheJAMImpulse100 == null ? 1 : cacheJAMImpulse100.Length + 1];
                if (cacheJAMImpulse100 != null)
                    cacheJAMImpulse100.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheJAMImpulse100 = tmp;
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
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.JAMImpulse100 JAMImpulse100(Color downColor, bool drawLines, int eraseBefore, int fontSize, double leg2Pullback, double leg3Confirm, double leg4Pullback, double legAPullback, double legCPullback, int minWaveBars, int strength, double textOffset, Color upColor, double wave3SlopeTolerance, double waveBTolerance)
        {
            return _indicator.JAMImpulse100(Input, downColor, drawLines, eraseBefore, fontSize, leg2Pullback, leg3Confirm, leg4Pullback, legAPullback, legCPullback, minWaveBars, strength, textOffset, upColor, wave3SlopeTolerance, waveBTolerance);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.JAMImpulse100 JAMImpulse100(Data.IDataSeries input, Color downColor, bool drawLines, int eraseBefore, int fontSize, double leg2Pullback, double leg3Confirm, double leg4Pullback, double legAPullback, double legCPullback, int minWaveBars, int strength, double textOffset, Color upColor, double wave3SlopeTolerance, double waveBTolerance)
        {
            return _indicator.JAMImpulse100(input, downColor, drawLines, eraseBefore, fontSize, leg2Pullback, leg3Confirm, leg4Pullback, legAPullback, legCPullback, minWaveBars, strength, textOffset, upColor, wave3SlopeTolerance, waveBTolerance);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.JAMImpulse100 JAMImpulse100(Color downColor, bool drawLines, int eraseBefore, int fontSize, double leg2Pullback, double leg3Confirm, double leg4Pullback, double legAPullback, double legCPullback, int minWaveBars, int strength, double textOffset, Color upColor, double wave3SlopeTolerance, double waveBTolerance)
        {
            return _indicator.JAMImpulse100(Input, downColor, drawLines, eraseBefore, fontSize, leg2Pullback, leg3Confirm, leg4Pullback, legAPullback, legCPullback, minWaveBars, strength, textOffset, upColor, wave3SlopeTolerance, waveBTolerance);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.JAMImpulse100 JAMImpulse100(Data.IDataSeries input, Color downColor, bool drawLines, int eraseBefore, int fontSize, double leg2Pullback, double leg3Confirm, double leg4Pullback, double legAPullback, double legCPullback, int minWaveBars, int strength, double textOffset, Color upColor, double wave3SlopeTolerance, double waveBTolerance)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.JAMImpulse100(input, downColor, drawLines, eraseBefore, fontSize, leg2Pullback, leg3Confirm, leg4Pullback, legAPullback, legCPullback, minWaveBars, strength, textOffset, upColor, wave3SlopeTolerance, waveBTolerance);
        }
    }
}
#endregion
