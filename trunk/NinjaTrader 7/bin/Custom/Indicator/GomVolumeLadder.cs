#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using System.Collections.Generic;
using System.Windows.Forms;


#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Enter the description of your new custom indicator here
    /// </summary>
    [Description("Enter the description of your new custom indicator here")]
	

    public class GomVolumeLadder : GomDeltaIndicator
    {
//#region Variables					
				
		class DrawData
		{	
			public string FormattedData;
			public Font DrawFont=myFont;
			public Brush FontBrush=myBlackBrush;
			public Color FillColor;
			public float FillStartRatio=0f;
			public float FillEndRatio=1f;
			
			public DrawData(Helper helper)
			{
				FillColor=helper.colorBackGround;
			}
			
		}

		
	//	public enum LadderTypeEnum{Delta,TotalVolume,DeltaDiff,Levels};
		public enum LadderTypeEnum{Delta,TotalVolume,DeltaDiff};
		
		public enum AccumulationTypeEnum {AllDays,NDays,ZeroDay}
		
		public enum FormattingType
				{
					Normal,
					Kilo,
					Kilo1Digit
				}
		
				
		public enum RatioType
				{
					MaximumOfLadder,
					FixedNumber
				}
		
		
		static void DrawLadderCell(RectangleF rect,DrawData param, Helper helper)		
			{
				RectangleF rectToFill=new RectangleF();
				
				rectToFill.X=rect.X+param.FillStartRatio*rect.Width;
				rectToFill.Width=rect.Width*(param.FillEndRatio-param.FillStartRatio);
				rectToFill.Y=rect.Y;
				rectToFill.Height=rect.Height;

				helper.graphics.FillRectangle(new SolidBrush(param.FillColor),rectToFill);
				if (helper.showBordersNumbers)
				{	
					helper.graphics.DrawRectangle(myBlackPen,rect.X,rect.Y,rect.Width, rect.Height);
					helper.graphics.DrawString(param.FormattedData, param.DrawFont,param.FontBrush,rect,mySF);
				}
			}
			
		
		class LadderClass
		{	
			//Fields & Properties
			private Dictionary<int,int> cells=new Dictionary<int,int>();
			public Dictionary<int,int> Cells {get {return cells;}}
									
			private int _minPriceLevel=Int32.MaxValue;
			public int MinPriceLevel{ get {return _minPriceLevel;}}

			private int _maxPriceLevel=Int32.MinValue;
			public int MaxPriceLevel{get {return _maxPriceLevel;}}
			
			private int _sum=0;
			public int Sum{get {return _sum;}}
						
			private int _maxLadderValue=Int32.MinValue;
			public int MaxLadderValue{get {return _maxLadderValue;}}
			
			public delegate DrawData  FillCellsDelegate (int volume, int maxValue);
			protected FillCellsDelegate _fillCells;
			public FillCellsDelegate FillCells {set{_fillCells=value;}}
					
			public delegate string Volume2StringDelegate(int vol);
			protected Volume2StringDelegate _volume2String;		
			public Volume2StringDelegate Volume2String {set{_volume2String=value;}}
			
			public Helper _helper;
			public Helper helper { get {return _helper;}}	
			
			public int _lastTickPrice=0;
			
						
			//Constructors
			public  LadderClass(Helper paramHelper)
			{
				_helper=paramHelper;
			}
						
			public void ComputeMax()
				
			{	int max=Int32.MinValue;
				
				foreach( int value in cells.Values)
				{
					max=Math.Max(Math.Abs(value),max);
				}
					
				_maxLadderValue=max;
			}
				
			public LadderClass(LadderClass previousLadder,Helper paramHelper):this(paramHelper)
			{
				if (previousLadder!=null)
				{
					foreach(KeyValuePair<int,int> iteratorpair in previousLadder.Cells)
						{  
							cells.Add(iteratorpair.Key,iteratorpair.Value);					
						}
					_minPriceLevel=previousLadder._minPriceLevel;
					_maxPriceLevel=previousLadder._maxPriceLevel;
				}
			}
			
			public void Add(LadderClass ladderToAdd)
			{
				if (ladderToAdd!=null)
				{
					foreach(KeyValuePair<int,int> iteratorpair in ladderToAdd.Cells)
						{  
							this.Increment(iteratorpair.Key,iteratorpair.Value,false);					
						}
				}
			}
			
			
			
			//Ladder management Methods
			public void CreatePriceLevel(int priceTick)
			{
				cells.Add(priceTick,0);
				_minPriceLevel=Math.Min(MinPriceLevel,priceTick);
				_maxPriceLevel=Math.Max(MaxPriceLevel,priceTick);
			}
			
			public void Increment(int priceTick, int dataToAdd, bool inLineCalcMax)
			{				
				if( !cells.ContainsKey(priceTick))
					CreatePriceLevel(priceTick);				

				cells[priceTick]+=dataToAdd;
				if (inLineCalcMax)
					_maxLadderValue= Math.Max(_maxLadderValue,Math.Abs(cells[priceTick]));	
				_sum += dataToAdd;	
				_lastTickPrice=priceTick;
			}
	
			public void Set(int priceTick, int dataToAdd)
			{				
				if( !cells.ContainsKey(priceTick))
					CreatePriceLevel(priceTick);				

				cells[priceTick]=dataToAdd;
				_lastTickPrice=priceTick;

			}
			
			public void CopyWithOffset(LadderClass ladderToAdd,int offset, bool ladderpostive)
			{
				if (ladderToAdd!=null)
				{
					foreach(KeyValuePair<int,int> iteratorpair in ladderToAdd.Cells)
						{  
							if( !cells.ContainsKey(iteratorpair.Key))
								CreatePriceLevel(iteratorpair.Key);				

							cells[iteratorpair.Key]=((ladderpostive)?1:-1)*iteratorpair.Value+offset;
						}
				}
			}
/*
			public void CalcLevels(int pricetick)
			{
				int curlevel;
				
				int level0;
				
				if (!cells.ContainsKey(pricetick))
				{
					for (int j=_minPriceLevel;j<=_maxPriceLevel;j++)
					if (cells.ContainsKey(j))
						cells[j]=0;
					return;
				}
					
				level0=cells[pricetick];
				cells[pricetick]=0;
				
				
				int level=level0;
				int i;
				int posmax=pricetick;
				
				for ( i=pricetick+1;i<=_maxPriceLevel;i++)
					if (cells.ContainsKey(i))
					{
						curlevel=cells[i];						
						if (curlevel<level)
						{

							level=curlevel;
							posmax=i;
						}

						cells[i]=0;
					}

				cells[posmax]=level;
				helper.dsHighCOT.Set(level);
					
				level=level0;	
				int posmin=pricetick;
				for ( i=pricetick-1;i>=_minPriceLevel;i--)
					if (cells.ContainsKey(i))
					{
						curlevel=cells[i];						
						if (curlevel>level)
						{

							level=curlevel;
							posmin=i;
						}

						cells[i]=0;
					}

				cells[posmin]=level;
				helper.dsLowCOT.Set(level);


				
				
			}
*/			
			
			//Graphical  Methods
			public void  DrawBorder(float x, float cellWidth,Pen pen)
			{
				if (cells.Count>0)
					helper.graphics.DrawRectangle(pen,x,helper.GetY(MaxPriceLevel+0.5f),cellWidth,helper.scalingfactor*(MaxPriceLevel-MinPriceLevel+1));
			}
			
			public void  DrawCell(float x, float cellWidth,int curTick, Pen pen)
			{	
				
				if ((curTick<=helper.maxTick) && (curTick>=helper.minTick))
					helper.graphics.DrawRectangle(pen,x,helper.GetY(curTick+0.5f),cellWidth,helper.scalingfactor);
			}
			
			public void EmptyLadder(float x,float cellWidth)
			{	
				if (Cells.Count>0)
				{
					int minPrice=Math.Max(helper.minTick,MinPriceLevel);
					int maxPrice=Math.Min(helper.maxTick,MaxPriceLevel);
			
					helper.graphics.FillRectangle(helper.myWhiteBrush,x,helper.GetY(maxPrice+0.5f),cellWidth,helper.scalingfactor*(maxPrice-minPrice+1));
				}
			}
			
			public void DrawAndFillCells(float x,float cellWidth)
			{	
				RectangleF curLadderCellRect= new RectangleF(x,0,cellWidth,helper.scalingfactor);
				int curTick,curVol;
				DrawData curDrawData;
				
				curLadderCellRect.X=x;
				foreach (KeyValuePair<int, int> cell in Cells)
					{ 
						curTick=cell.Key;
						
						if ((curTick<=helper.maxTick) && (curTick>=helper.minTick))
						{
							curVol=cell.Value;
							curLadderCellRect.Y=helper.GetY(curTick+0.5f);

							curDrawData=_fillCells(curVol,_maxLadderValue);
							DrawLadderCell(curLadderCellRect,curDrawData,helper);
							curDrawData=null;
						}
					}
			}		
						
			//Volume To String Formatter
			public static string Formatter(int vol,FormattingType format)
			{
				if (format==FormattingType.Kilo)
					 return (((float)((vol)/1000f)).ToString("F0"));
				else if (format==FormattingType.Kilo1Digit)
					return (((float)((vol)/1000f)).ToString("F1"));
				else 
					return vol.ToString("N0");
			}
		}	
	
		sealed class BALadderClass
		{  
			//Fields & Properties
 			private LadderClass _bidLadder;
			public LadderClass BidLadder{get {return  _bidLadder;}}
			
			private LadderClass _askLadder;
			public LadderClass AskLadder{get {return  _askLadder;}}
			
			private LadderClass _deltaLadder;
			public LadderClass DeltaLadder{get {return  _deltaLadder;}}
			
			private int _barIndex;
			private int _currentdelta=0;
			
			private bool _isLastTickABid=false;
			
			private Helper _helper;
			private Helper helper{get {return _helper;}}
			
			private int BAlastTickPrice;
			
			public int HighCOT=0,LowCOT=0;

			
			//Constructors 
			
			public BALadderClass(int index,Helper paramHelper) 
			{ 			
				_helper=paramHelper;
				
				_bidLadder=new LadderClass(paramHelper);
				_askLadder=new LadderClass(paramHelper);				
				_deltaLadder=new LadderClass(paramHelper);
				
				_bidLadder.FillCells=BidLadderFillParams;
				_askLadder.FillCells=AskLadderFillParams;
							
				_barIndex=index; 
			}

			//Ladder Management methods
			public void AddAskVolume(int priceTick, int volume)
			{	
					
				_askLadder.Increment(priceTick,volume,true);	
				_isLastTickABid=false;
				BAlastTickPrice=priceTick;
				_currentdelta+=volume;
				_deltaLadder.Set(priceTick,_currentdelta);
				
			}
			
			public void AddBidVolume(int priceTick, int volume)
			{					
				_bidLadder.Increment(priceTick,volume,true);
				_isLastTickABid=true;
				BAlastTickPrice=priceTick;
				
				_currentdelta-=volume;
				_deltaLadder.Set(priceTick,_currentdelta);
			}	
			
			
			public void CalcLevelsBA(int pricetick)
			{
				int curlevel;			
				
				if (!DeltaLadder.Cells.ContainsKey(pricetick))
				{
					return;
				}
					
				int level0=DeltaLadder.Cells[pricetick];
				int level=level0;
				
				int i;
				int posmax=pricetick;
				
				for ( i=pricetick+1;i<=DeltaLadder.MaxPriceLevel;i++)
					if (DeltaLadder.Cells.ContainsKey(i))
					{
						curlevel=DeltaLadder.Cells[i];						
						if (curlevel>level)
						{

							level=curlevel;
							posmax=i;
						}
					}

				
				HighCOT=level0-level;
					
				level=level0;	
				int posmin=pricetick;
				for ( i=pricetick-1;i>=DeltaLadder.MinPriceLevel;i--)
					if (DeltaLadder.Cells.ContainsKey(i))
					{
						curlevel=DeltaLadder.Cells[i];						
						if (curlevel<level)
						{
							level=curlevel;
							posmin=i;
						}
					}

				
				LowCOT=level0-level;


				
				
			}
			private void DrawMaxVolume(float x)
			{
				RectangleF Rect=new RectangleF(0f,0f,0f,0f);
				DrawData param=new DrawData(helper);

				Rect.Width=2*(helper.BAcellwidth+helper.chartcontrol.BarWidth)+2;
				Rect.Height=helper.scalingfactor;
				Rect.X=x;
				
				//get max level			
				int min=Math.Min(_bidLadder.MinPriceLevel,_askLadder.MinPriceLevel);
				int max=Math.Max(_bidLadder.MaxPriceLevel,_askLadder.MaxPriceLevel);
				
				int maxpricelevel=0;
				int maxvol=0;
				
				for (int pricelevel=min;pricelevel<=max;pricelevel++)
				{
					int vol=0;
					if (_bidLadder.Cells.ContainsKey(pricelevel))
						vol += _bidLadder.Cells[pricelevel];
					if (_askLadder.Cells.ContainsKey(pricelevel))
						vol += _askLadder.Cells[pricelevel];
					
					if (vol>maxvol)
					{	
						maxvol=vol;
						maxpricelevel=pricelevel;
					}	
				}
								
				Rect.Y=helper.GetY(maxpricelevel+0.5f);
				

				helper.graphics.DrawRectangle(helper.myVolPen,Rect.X,Rect.Y,Rect.Width, Rect.Height);
			
			
			}
			
			
			//Graphical methods
			private void DrawBottomTotalDelta(float x)
			{	
				RectangleF Rect=new RectangleF(0f,0f,0f,0f);
				DrawData param=new DrawData(helper);

				Rect.Width=2*(helper.BAcellwidth+helper.chartcontrol.BarWidth)+2;
				Rect.Height=helper.ladderBADeltaFont.GetHeight();
				Rect.X=x;
				Rect.Y=helper.bounds.Bottom-2-Rect.Height;
				
				int sum=_askLadder.Sum-_bidLadder.Sum;
				param.FillColor=Color.FromArgb(128,(sum>0)?helper.colorUp:helper.colorDown);
				param.FormattedData=LadderClass.Formatter((helper.ladderBANoMinus)?Math.Abs(sum):sum,helper.ladderBAFormat);
				
				param.DrawFont=helper.ladderBADeltaFont;
				
				helper.graphics.FillRectangle(helper.myWhiteBrush,Rect);
				DrawLadderCell(Rect,param,helper);
			}
			
			private void DrawCOTs(float x)
			{	
				if (_deltaLadder.Cells.Count>0)
				{
					RectangleF Rect=new RectangleF(0f,0f,0f,0f);
					DrawData param=new DrawData(helper);
	
					Rect.Width=helper.BAcellwidth+2*helper.chartcontrol.BarWidth+2;
					Rect.Height=helper.scalingfactor;
					Rect.X=x+helper.BAcellwidth/2;

					
					Rect.Y=helper.GetY(_deltaLadder.MaxPriceLevel+2.0f);
					param.FillColor=Color.FromArgb(128,helper.colorDown);
					param.FormattedData=LadderClass.Formatter((helper.ladderBANoMinus)?Math.Abs(HighCOT):HighCOT,helper.ladderBAFormat);
					helper.graphics.FillRectangle(helper.myWhiteBrush,Rect);
					DrawLadderCell(Rect,param,helper);
					
					
					Rect.Y=helper.GetY(_deltaLadder.MinPriceLevel-1.0f);					
					param.FillColor=Color.FromArgb(128,helper.colorUp);
					param.FormattedData=LadderClass.Formatter(LowCOT,helper.ladderBAFormat);
					helper.graphics.FillRectangle(helper.myWhiteBrush,Rect);
					DrawLadderCell(Rect,param,helper);

				}
			//
			}

			public void DrawLadder()
			{	
				float xbid,xask;			
				float cellwidth=helper.BAcellwidth;
				ChartControl CC=helper.chartcontrol;
				LadderClass VolumeLadder,DeltaLadder;
				
				#if !NT7
					xbid=CC.GetXByBarIdx(_barIndex)-CC.BarWidth-helper.BAcellwidth-1;
					xask=CC.GetXByBarIdx(_barIndex)+CC.BarWidth+1;
				#else
					xbid=CC.GetXByBarIdx(helper.bars,_barIndex)-CC.BarWidth-helper.BAcellwidth-1;
					xask=CC.GetXByBarIdx(helper.bars,_barIndex)+CC.BarWidth+1;
				#endif
				
				if (helper.showBALadder)
				{
					if (helper.BAdeltaMode==false)
					{
						_bidLadder.EmptyLadder(xbid,cellwidth);	
						_askLadder.EmptyLadder(xask,cellwidth);
											
						_bidLadder.DrawAndFillCells(xbid,cellwidth);			
						_askLadder.DrawAndFillCells(xask,cellwidth);	
											
						if (_askLadder.Sum>_bidLadder.Sum)
							_askLadder.DrawBorder(xask, cellwidth,new Pen(helper.colorUp,2));
						else if (_bidLadder.Sum>_askLadder.Sum)
							_bidLadder.DrawBorder(xbid,cellwidth,new Pen(helper.colorDown,2));
						
						if (helper.ladderBAShowMaxLevel)
							DrawMaxVolume(xbid);
						
						if (_isLastTickABid)
							_bidLadder.DrawCell(xbid,cellwidth,BAlastTickPrice,helper.myTickPen);
						else
							_askLadder.DrawCell(xask,cellwidth,BAlastTickPrice,helper.myTickPen);
	
					}
					else
					{
						VolumeLadder=new LadderClass(_helper);
						
						VolumeLadder.FillCells=VolumeFillParams;
						VolumeLadder.CopyWithOffset(_bidLadder,0,true);
						VolumeLadder.Add(_askLadder);
							
						VolumeLadder.ComputeMax();
						VolumeLadder.EmptyLadder(xbid,cellwidth);
						VolumeLadder.DrawAndFillCells(xbid,cellwidth);
						
						
						DeltaLadder=new LadderClass(_helper);
						
						DeltaLadder.FillCells=DeltaFillParams;
						DeltaLadder.CopyWithOffset(_bidLadder,0,false);
						DeltaLadder.Add(_askLadder);
							
						DeltaLadder.ComputeMax();
						DeltaLadder.EmptyLadder(xask,cellwidth);
						DeltaLadder.DrawAndFillCells(xask,cellwidth);
						
						if (helper.ladderBAShowMaxLevel)
							DrawMaxVolume(xbid);
										
						DeltaLadder.DrawCell(xask,cellwidth,BAlastTickPrice,helper.myTickPen);
					}
				}
				
				if (helper.ladderBAShowCOT)
					DrawCOTs(xbid);
					
				if (helper.ladderBAShowDelta)				
					DrawBottomTotalDelta(xbid);
				


				
			}
			
			
			void GetRatioAndColor(int volume,int maxValue, ref float ratio, ref Color color)
				
			{
				
				if (maxValue==0) ratio=0.0F;
				
				if (helper.ladderBARatio==RatioType.MaximumOfLadder)
					ratio=(float)Math.Abs(volume)/(float)maxValue;
				else
					ratio=Math.Min(1f,(float)Math.Abs(volume)/(float)helper.ladderBAFixedNumber);
				
				if (!helper.ladderBAShowBars)
					color=Color.FromArgb((int)(255f*ratio),color);
				else if (!((helper.ladderBARatio==RatioType.FixedNumber)&& (Math.Abs(volume)>helper.ladderBAFixedNumber)))
					color=Color.FromArgb(192,color);

			}
			
			DrawData BasicFillParams(int volume, int maxValue,Color color)
			{
				DrawData returnData=new DrawData(helper);
				
				float ratio=0.0F;
				returnData.FillColor=color;
				GetRatioAndColor(volume,maxValue,ref ratio,ref returnData.FillColor);
				
				if (helper.ladderBAShowBars)
				{
					returnData.FillStartRatio=(1f-ratio);
					returnData.FillEndRatio=ratio;					
				}
				
				if (helper.ladderBAShowNumbers)
					returnData.FormattedData=LadderClass.Formatter(volume,helper.ladderBAFormat);
				
				
				if (volume==maxValue)
					returnData.DrawFont=myFontBold;
				
				return returnData;
			
			}
			
			

			//2 Ladder Custom Fill Delegates
			DrawData BidLadderFillParams(int volume,int maxValue)
			{
				DrawData returnData= BasicFillParams(volume,maxValue,helper.colorDown);
			
				returnData.FillEndRatio=1f;
				
				return returnData;
					
			}
				
			DrawData AskLadderFillParams(int volume,int maxValue)
				{
				DrawData returnData=BasicFillParams(volume,maxValue,helper.colorUp);
				
				returnData.FillStartRatio=0f;
				
				return returnData;
					
				}	
				
			DrawData VolumeFillParams(int volume, int maxValue)
			{
				DrawData returnData=BasicFillParams(volume,maxValue,helper.colorVolume);

				returnData.FillEndRatio=1f;
				
				return returnData;
			}
				
			DrawData DeltaFillParams(int volume, int maxValue)
			{
				DrawData returnData=BasicFillParams((helper.ladderBANoMinus)?Math.Abs(volume):volume,maxValue,(volume>0)?helper.colorUp:helper.colorDown);	
				
				returnData.FillStartRatio=0f;
				
				return returnData;
			}
				
			
			
		}
			
		sealed class GLadderClass
		{
			//Fields & Properties
			
			//here is the collection of BidAskladders.
			Dictionary<int,LadderClass> _ladders = new Dictionary<int,LadderClass>();
			Dictionary<int,int> _cds=new Dictionary<int,int>();
			int _curBarNum;
			
			//here is the collection of daily ladders.
			//Dictionary<int,LadderClass> dailyVolLadders = new Dictionary<int,LadderClass>();
			//Dictionary<int,LadderClass> dailyDeltaLadders = new Dictionary<int,LadderClass>();
			
			private LadderClass _curLadder=null;
			public LadderClass curLadder{get{ return _curLadder;}}
			
			private Helper _helper;
			private Helper helper{get {return _helper;}}
			
			protected FormattingType _ladderFormattingType;
			public FormattingType LadderFormattingType{get {return _ladderFormattingType;} set { _ladderFormattingType=value;}}
			
			protected RatioType _ladderRatioType;
			public RatioType LadderRatioType {get {return _ladderRatioType;} set { _ladderRatioType=value;}}
			
			protected int _ladderFixedNumber;
			public int LadderFixedNumber {get {return _ladderFixedNumber;} set { _ladderFixedNumber=value;}}
						
			protected LadderTypeEnum _ladderType;
			public LadderTypeEnum LadderType {get {return _ladderType;} set { _ladderType=value;}}
			
			protected AccumulationTypeEnum _accumulationType;
			public AccumulationTypeEnum AccumulationType {get {return _accumulationType;} set { _accumulationType=value;}}
			
			protected int _accumulationDays;
			public int AccumulationDays {get {return _accumulationDays;} set { _accumulationDays=value;}}
			
			protected float _width;
			public float Width {get {return _width;} set { _width=value;}}
			
			protected bool _show=true;
			public bool Show {get {return _show;} set { _show=value;}}		
						
						
			//Constructor
			
			public GLadderClass(LadderTypeEnum paramLadderType,AccumulationTypeEnum paramAccumulationType, 
								int paramAccumulationDays,FormattingType paramFormat, RatioType paramLadderRatioType,int paramLadderFixedNumber, float paramWidth)
			{
				_ladderType=paramLadderType;
				_accumulationType=paramAccumulationType;
				_accumulationDays=paramAccumulationDays;
				_ladderFormattingType=paramFormat;
				_ladderRatioType=paramLadderRatioType;
				_width=paramWidth;
				_ladderFixedNumber=paramLadderFixedNumber;
						
			}
			
			
			public void AddLadder(int barNum,bool newSession,int endedsession,Helper paramHelper)
			{ 
				if (!paramHelper.liveMode || newSession || (_curLadder==null))
				{
				LadderClass initLadder=new LadderClass(null); 
				
	/*			if 	(_ladderType==LadderTypeEnum.Levels)
				{
	//				initLadder.Increment(_curLadder._lastTickPrice, _curLadder.Cells[_curLadder._lastTickPrice],true);
				    _ladders.Add(barNum,new LadderClass(initLadder,paramHelper));
					_cds.Add(barNum, 0);
				}
				else
*/				{
					
				if ((!newSession) || (_accumulationType==AccumulationTypeEnum.AllDays))
				{	
					_ladders.Add(barNum,new LadderClass(curLadder,paramHelper));
					if (_cds.ContainsKey(barNum-1))
                        _cds.Add(barNum,_cds[barNum-1]);
                    else
                        _cds.Add(barNum, 0);
				}
				else if (_accumulationType==AccumulationTypeEnum.ZeroDay)
				{
					_ladders.Add(barNum,new LadderClass(paramHelper));
					_cds.Add(barNum,0);
				}
				else
					{	_cds.Add(barNum,0);
						
						for(int i=_accumulationDays-1;i>=0;i--)
						{	
							if (_ladderType==LadderTypeEnum.Delta)
								{
									if (helper.dailyDeltaLadders.ContainsKey(endedsession-i))
										initLadder.Add(helper.dailyDeltaLadders[endedsession-i]);
								}
							else if (_ladderType==LadderTypeEnum.TotalVolume)
								{	
									if (helper.dailyVolLadders.ContainsKey(endedsession-i))
										initLadder.Add(helper.dailyVolLadders[endedsession-i]);
								}
							else
								{
									if (helper.dailyDeltaDiffLadders.ContainsKey(endedsession-i))
									{	
										initLadder.CopyWithOffset(helper.dailyDeltaDiffLadders[endedsession-i],_cds[barNum],true);
										_cds[barNum]+= helper.dailyCDs[endedsession-i];
										
									}
								}
									
						}
						_ladders.Add(barNum,new LadderClass(initLadder,paramHelper));
					}
				}
				
				_curLadder=_ladders[barNum];
				_curLadder.FillCells=TotFillParams;
				_helper=paramHelper;
				_curBarNum=barNum;
				}
				
			}
				
			public void Add(int priceTick, int volume)
			{		
				_curLadder.Increment(priceTick,volume,false);

			}
			
			public void SetCD(int priceTick, int volume)
			{		
				_cds[_curBarNum]+=volume;
				_curLadder.Set(priceTick,_cds[_curBarNum]);


			}
			
			//Grahical methods
			public void DrawLadder(int barNum,float x)
			{	
				LadderClass ladderToDraw=null;
				LadderClass DiffLadder;
				int bNum=barNum;
				
				if (_ladders.ContainsKey(barNum))
					ladderToDraw=_ladders[barNum];				
				else 
				{	
					ladderToDraw=_curLadder;	
					bNum=_curBarNum;
				}
				
				if (ladderToDraw != null)
				{	
					if (_ladderType==LadderTypeEnum.DeltaDiff)
					{
						DiffLadder=new LadderClass(_helper);
						DiffLadder.FillCells=TotDeltaFillParams;
						DiffLadder.CopyWithOffset(ladderToDraw,_cds[bNum],false);
						ladderToDraw=DiffLadder;
					}
					
			/*		else if (_ladderType==LadderTypeEnum.Levels)
					{
						DiffLadder=new LadderClass(_helper);
						DiffLadder.FillCells=LevelsFillParams;
						DiffLadder.CopyWithOffset(ladderToDraw,_cds[bNum],false);
						
						DiffLadder.CalcLevels(ladderToDraw._lastTickPrice);
						ladderToDraw=DiffLadder;
					}
			*/		
					
					ladderToDraw.ComputeMax();
					ladderToDraw.EmptyLadder(x,_width);
					ladderToDraw.DrawAndFillCells(x,_width);
				}	
			}
			
			
			//2 Ladder Custom Fill Delegates			
			DrawData TotDeltaFillParams(int volume, int maxValue)
			{
				DrawData returnData=new DrawData(helper);
				float ratio;
				
				if (maxValue==0) 
					ratio=0;
				else
				{	
					if (_ladderRatioType==RatioType.MaximumOfLadder)
						ratio=(float)Math.Abs(volume)/(float)maxValue;
					else
						ratio=Math.Min(1f,(float)Math.Abs(volume)/(float)_ladderFixedNumber);	
				}
				
				returnData.FillColor=Color.FromArgb((int)(255f*ratio),(volume>0)?helper.colorUp:helper.colorDown);
				returnData.FormattedData=LadderClass.Formatter(volume,_ladderFormattingType);
				
				if (Math.Abs(volume)==maxValue)
					returnData.DrawFont=myFontBold;				

				return returnData;
			}
			
		/*	DrawData LevelsFillParams(int volume, int maxValue)
			{
				DrawData returnData=new DrawData(helper);
				float ratio;
				
				if (maxValue==0) 
					ratio=0;
				else
				{	
					if (_ladderRatioType==RatioType.MaximumOfLadder)
						ratio=(float)Math.Abs(volume)/(float)maxValue;
					else
						ratio=Math.Min(1f,(float)Math.Abs(volume)/(float)_ladderFixedNumber);	
				}
				
				if (volume!=0)
				{
					//returnData.FillColor=Color.FromArgb((int)(255f*ratio),(volume>0)?helper.colorUp:helper.colorDown);
					//returnData.FillColor=(volume>0)?helper.colorUp:helper.colorDown;
					returnData.FillColor=Color.FromArgb(128,(volume>0)?helper.colorUp:helper.colorDown);
					returnData.FormattedData=LadderClass.Formatter(volume,_ladderFormattingType);
					//returnData.FillStartRatio=(1f-ratio);
					returnData.FillStartRatio=0f;

				}
				//else
					//returnData.FillColor=Color.White;
				


				
				//if (Math.Abs(volume)==maxValue)
					returnData.DrawFont=myFontBold;	

				return returnData;
			}
		*/
			
			DrawData TotVolFillParams(int volume, int maxValue)
			{
				DrawData returnData=new DrawData(helper);
				float ratio;
				
				if (maxValue==0) 
					ratio=0;
				else
				{	
					if (_ladderRatioType==RatioType.MaximumOfLadder)
						ratio=(float)volume/(float)maxValue;
					else
						ratio=Math.Min(1f,(float)volume/(float)_ladderFixedNumber);	
				}		
				
				returnData.FillColor=helper.colorVolume;
				returnData.FillStartRatio=(1f-ratio);
				returnData.FormattedData=LadderClass.Formatter(volume,_ladderFormattingType);
				returnData.DrawFont=myFontBold;
				
//				if (helper.graphics.MeasureString(returnData.FormattedData,returnData.DrawFont).Width<=ratio*_width)
//					returnData.FontBrush=helper.myWhiteBrush;
				
				return returnData;
			}
			
			DrawData TotFillParams(int volume, int maxValue)
			{
				if (_ladderType==LadderTypeEnum.Delta) 	return TotDeltaFillParams(volume,maxValue);
				else 									return TotVolFillParams(volume,maxValue);
			}
			
		}
		
		class Helper
		{
			public float scalingfactor;
			public float graphoffset;
			public float y0;
			public Graphics graphics;
			public ChartControl chartcontrol;
			public float BAcellwidth;
			public int minTick,maxTick;
			public Rectangle bounds;
			public double currentmin=0f,currentmax=0f;
			public Color colorUp=Color.Green;
			public Color colorDown=Color.Red;
			public Color colorVolume=Color.Blue;
			public Color colorBackGround=Color.Snow;
			public Color colorLastTick=Color.Orange;
			public Color colorMaxVolLevel=Color.Cyan;
			public SolidBrush myWhiteBrush;
			public Pen myTickPen;
			public Pen myVolPen;
			public FormattingType ladderBAFormat=FormattingType.Normal;
			public bool showBALadder=true;
			public bool liveMode=true;
			public bool BAdeltaMode=false;
			public bool showBordersNumbers=true;
			public RatioType ladderBARatio=RatioType.MaximumOfLadder;
			public int ladderBAFixedNumber=15000;
			public bool ladderBAShowMaxLevel=true;	
			public bool ladderBAShowCOT=true;	
			public bool ladderBAShowDelta=true;
			public bool ladderBAShowNumbers=true;
			public bool ladderBAShowBars=false;	
			public bool ladderBANoMinus=false;	
			public Font ladderBADeltaFont=myFont;
			public int agregationFib=1;
			public int agregationFactor;		
			public Dictionary<int,LadderClass> dailyVolLadders = new Dictionary<int,LadderClass>();
			public Dictionary<int,LadderClass> dailyDeltaLadders = new Dictionary<int,LadderClass>();
			public Dictionary<int,LadderClass> dailyDeltaDiffLadders= new Dictionary<int,LadderClass>();
			public Dictionary<int,int> dailyCDs= new Dictionary<int,int>();
			public Bars bars;
			public IntSeries dsHighCOT,dsLowCOT;
			public IntSeries dsMaxBid,dsMaxAsk,dsSumBid,dsSumAsk,dsDelta,dsVolume;
			
			public Helper()
			{
				myWhiteBrush =new SolidBrush(colorBackGround);
				myTickPen=new Pen(colorLastTick,2);
				myVolPen=new Pen(colorMaxVolLevel,2);
				agregationFactor=Fibonacci(agregationFib);
			}
			
			public float GetY(float ticklevel)
			{
					return(graphoffset - (ticklevel+((float)agregationFactor-1)/(2*(float)agregationFactor)- y0) * scalingfactor);
			}		
			
		}

		
		//here is the collection of BidAskladders.
		Dictionary<int,BALadderClass> baLadders = new Dictionary<int,BALadderClass>();
		
		BALadderClass curBALadder=null;
		LadderClass curDailyVolLadder,curDailyDeltaLadder,curDailyDeltaDiffLadder;

		Dictionary<string,GLadderClass> gLadders=new Dictionary<string,GLadderClass>();

		GLadderClass gLadderExtremeLeft= new GLadderClass(LadderTypeEnum.TotalVolume,AccumulationTypeEnum.AllDays,0,
										FormattingType.Kilo,RatioType.MaximumOfLadder,5000,50);
			
		GLadderClass gLadderLeft=new GLadderClass(LadderTypeEnum.TotalVolume,AccumulationTypeEnum.ZeroDay,0,
										FormattingType.Kilo,RatioType.MaximumOfLadder,5000,40);
		
		GLadderClass gLadderRight=new GLadderClass(LadderTypeEnum.DeltaDiff,AccumulationTypeEnum.ZeroDay,0,
										FormattingType.Normal,RatioType.MaximumOfLadder,5000,40);
		
		GLadderClass gLadderExtremeRight=new GLadderClass(LadderTypeEnum.Delta,AccumulationTypeEnum.ZeroDay,0,
										FormattingType.Normal,RatioType.MaximumOfLadder,25000,50);
		
		Helper helper=new Helper();

		int lastBar=-1;
		int curSession=0;
		int curDailyCD=0;
			
		double askPrice,bidPrice;
		
		static Font myFont=new Font("Arial", 8,FontStyle.Regular);
		static Font myFontBold = new Font(myFont,FontStyle.Bold);

		static StringFormat mySF=new StringFormat();


		bool firstinitdone=false;
		bool forcegoodsize=true;

		System.Windows.Forms.KeyEventHandler keyEvtH;
		
		double initCellHeight=myFont.GetHeight();
		const int initBarSpace=80;
		const int initBarWidth=2;
				
		static SolidBrush myBlackBrush=new SolidBrush(Color.Black);
		static Pen myBlackPen=new Pen(Color.Black);
		

		
//#endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
		/// 
		
		static int Fibonacci (int x)
		{

   			if (x <= 1)
   			{
      			return 1;
   			}
   		return Fibonacci (x-1) + Fibonacci (x-2);
		}		
		
		
        protected override void GomInitialize()
        {		
            CalculateOnBarClose	= false;
            Overlay				= true;
            PriceTypeSupported	= false;
			AutoScale 			= true;

			mySF.Alignment=StringAlignment.Far;
			mySF.LineAlignment=StringAlignment.Center;
			
			helper.dailyVolLadders.Add(0,new LadderClass(null));
			curDailyVolLadder=helper.dailyVolLadders[0];
			
			helper.dailyDeltaLadders.Add(0,new LadderClass(null));
			curDailyDeltaLadder=helper.dailyDeltaLadders[0];
			
			helper.dailyDeltaDiffLadders.Add(0,new LadderClass(null));
			curDailyDeltaDiffLadder=helper.dailyDeltaDiffLadders[0];

			helper.dailyCDs.Add(0,0);
//			curDailyCD=helper.dailyCDs[0];

			
			gLadders.Add("EL",gLadderExtremeLeft);
			gLadders.Add("L",gLadderLeft);
			gLadders.Add("R",gLadderRight);
			gLadders.Add("ER",gLadderExtremeRight);
			
			GC.Collect();
      		GC.WaitForPendingFinalizers();
			
			
			//	if (!initdone)
			//	{
			#if NT7
					helper.dsHighCOT = new IntSeries(this,MaximumBarsLookBack.Infinite);
					helper.dsLowCOT = new IntSeries(this,MaximumBarsLookBack.Infinite);
					helper.dsMaxBid = new IntSeries(this,MaximumBarsLookBack.Infinite);
					helper.dsMaxAsk = new IntSeries(this,MaximumBarsLookBack.Infinite);
					helper.dsSumBid = new IntSeries(this,MaximumBarsLookBack.Infinite);
					helper.dsSumAsk = new IntSeries(this,MaximumBarsLookBack.Infinite);
					helper.dsVolume = new IntSeries(this,MaximumBarsLookBack.Infinite);
					helper.dsDelta = new IntSeries(this,MaximumBarsLookBack.Infinite);
			#else
					helper.dsHighCOT = new IntSeries(this);
					helper.dsHighCOT = new IntSeries(this);
					helper.dsLowCOT = new IntSeries(this);
					helper.dsMaxBid = new IntSeries(this);
					helper.dsMaxAsk = new IntSeries(this);
					helper.dsSumBid = new IntSeries(this);
					helper.dsSumAsk = new IntSeries(this);
					helper.dsVolume = new IntSeries(this);
					helper.dsDelta = new IntSeries(this);
			#endif

			//		initdone=true;
				
			//	}

				
			
		}
       
		private void chart_KeyDown (object sender, KeyEventArgs e)
		{
			
			if (e.KeyCode==Keys.Space)
			{
				helper.BAdeltaMode=!helper.BAdeltaMode;
				//SendKeys.Send("{F5}");
			}
				
			else if (e.KeyCode==Keys.Add)
			{	
				helper.agregationFib++;
				SendKeys.Send("{F5}");
			}
			else if (e.KeyCode==Keys.Subtract)
			{ 	
				if (helper.agregationFactor>1)					
					{
						helper.agregationFib--;
						SendKeys.Send("{F5}");
					}
			}
			else if (e.KeyCode==Keys.Up)
			{ 	
				double delta;	
				delta=this.TickSize*helper.agregationFactor;
				helper.currentmax -= delta;
				helper.currentmin -= delta;
			}
			else if (e.KeyCode==Keys.Down)
			{ 	
				double delta;	
				delta=this.TickSize*helper.agregationFactor;
				helper.currentmax += delta;
				helper.currentmin += delta;
			}
			
			else if (e.KeyCode==Keys.Decimal)
			{ 	
				helper.ladderBAShowBars=!helper.ladderBAShowBars;
			}


		}
		
		
		
		protected override void GomOnBarUpdate()
			
		{
			if (CurrentBar != lastBar)
			{
				lastBar=CurrentBar;
				helper.dsHighCOT.Set(0);
				helper.dsLowCOT.Set(0);
				helper.dsMaxBid.Set(0);
				helper.dsMaxAsk.Set(0);
				helper.dsSumBid.Set(0);
				helper.dsSumAsk.Set(0);
				helper.dsDelta.Set(0);
				helper.dsVolume.Set(0);
				
				//add new ladders			

				if (Bars.SessionBreak)
				{
					helper.dailyCDs[curSession]=curDailyCD;
					curDailyCD=0;
					
					curSession++;	
					helper.dailyVolLadders.Add(curSession,new LadderClass(null));
					helper.dailyDeltaLadders.Add(curSession,new LadderClass(null));
					helper.dailyDeltaDiffLadders.Add(curSession,new LadderClass(null));	
					helper.dailyCDs.Add(curSession,0);
					
					curDailyVolLadder = helper.dailyVolLadders[curSession];
                    curDailyDeltaLadder = helper.dailyDeltaLadders[curSession];
                    curDailyDeltaDiffLadder = helper.dailyDeltaDiffLadders[curSession];

				}
				
				foreach (GLadderClass ladder in gLadders.Values)
				{
					if (ladder.Show)
						ladder.AddLadder(CurrentBar,Bars.SessionBreak,curSession-1,helper);
					
					
				}
				
//				if (helper.showBALadder)
				{
					baLadders.Add(CurrentBar,new BALadderClass(CurrentBar,helper));
					curBALadder=baLadders[CurrentBar];	
				}
				
				
			}
			
			//example of how to access BA Ladder info
			
/*			
				if (CurrentBar<5) return;
				
				BALadderClass lad1;
				LadderClass lad2;
			
				// let's get BA ladders pair 3 bars ago. It's a collection indexed by barnum
			
				lad1=baLadders[CurrentBar-3];
			
				// let's get the ask ladder
			
				lad2=lad1.AskLadder;
			
				//let's print values at max and min levels
				Print( lad2.Cells[lad2.MaxPriceLevel]+" - " +lad2.Cells[lad2.MinPriceLevel]);
			
				//let's print total volume as stored 
				Print(lad2.Sum);
			
				//let's iterate by ourselves
				foreach(KeyValuePair<int,int> iteratorpair in lad2.Cells)
				{	
					//have to multiply level by ticksize to get actual price.
					Print(iteratorpair.Key*this.TickSize+" - "+iteratorpair.Value);
				}
				
				Print(" ");
*/				
		
		}
		
		
		protected override void GomOnMarketData(TickTypeEnum tickType,double price,int volume)
        {	
			int priceTick; int deltavol=0;
					
			priceTick=Convert.ToInt32(price/this.TickSize)/helper.agregationFactor;
			
			//let's update the ladders

			deltavol=CalcDelta(tickType,price,volume);
			
			if (deltavol>0) curBALadder.AddAskVolume(priceTick,volume);		
			if (deltavol<0) curBALadder.AddBidVolume(priceTick,volume);
			
//			if (helper.showBALadder)

			{ 
				helper.dsSumAsk.Set(curBALadder.AskLadder.Sum);
				helper.dsSumBid.Set(curBALadder.BidLadder.Sum);
			
				if (curBALadder.AskLadder.MaxLadderValue != Int32.MinValue )
					helper.dsMaxAsk.Set(curBALadder.AskLadder.MaxLadderValue);
				if (curBALadder.BidLadder.MaxLadderValue != Int32.MinValue )
					helper.dsMaxBid.Set(curBALadder.BidLadder.MaxLadderValue);
				helper.dsDelta.Set(curBALadder.AskLadder.Sum-curBALadder.BidLadder.Sum);
				helper.dsVolume.Set(curBALadder.AskLadder.Sum+curBALadder.BidLadder.Sum);
				
				curBALadder.CalcLevelsBA(priceTick);
				helper.dsHighCOT.Set(curBALadder.HighCOT);
				helper.dsLowCOT.Set(curBALadder.LowCOT);

			}

			
			if ((volume !=0))// && (tickTime.TimeOfDay > ChartControl.SessionBegin.TimeOfDay))
			{
				foreach (GLadderClass ladder in gLadders.Values)
				{
					if (ladder.Show) 
					{
						if (ladder.LadderType==LadderTypeEnum.Delta)
							ladder.Add(priceTick,deltavol);
						else if (ladder.LadderType==LadderTypeEnum.TotalVolume)
							ladder.Add(priceTick,volume);
						else 
							ladder.SetCD(priceTick,deltavol);
					}
				}
				
			helper.dailyCDs[curSession]+=deltavol;
				
			curDailyCD+=deltavol;
			curDailyDeltaLadder.Increment(priceTick,deltavol,false);	
			curDailyVolLadder.Increment(priceTick,volume,false);
			curDailyDeltaDiffLadder.Set(priceTick,curDailyCD);
			}
					
		}

		
		public override void GetMinMaxValues(ChartControl chartControl, ref double min, ref double max)
		{
			if (Bars==null)	return;
			
			int lastBar		= Math.Min(ChartControl.LastBarPainted, Bars.Count - 1);
			int firstBar	= Math.Max((ChartControl.LastBarPainted - ChartControl.BarsPainted) + 1,0);
			
			double newmax, newmin;
			double excursion;
					
			newmax=((((int)(Bars.Get(lastBar).High/this.TickSize))/helper.agregationFactor+2)*helper.agregationFactor)*this.TickSize;
			newmin=((((int)(Bars.Get(lastBar).Low/this.TickSize))/helper.agregationFactor-1)*helper.agregationFactor-2)*this.TickSize;
			excursion=(Math.Floor(helper.bounds.Height/initCellHeight)-1)*helper.agregationFactor*this.TickSize;
			
			if (helper.bounds.Height>0)
			{	
				if (helper.currentmax<1) //init
				{
					helper.currentmax=(((int)(((newmax+newmin+excursion)/2.0)/this.TickSize))/helper.agregationFactor)*helper.agregationFactor*this.TickSize;
					helper.currentmin=helper.currentmax-excursion;
	
				}
				else if (newmax>helper.currentmax)
				{
					helper.currentmax=newmax;
					helper.currentmin=newmax-excursion;
				}
				else if (newmin<helper.currentmin)
				{
					helper.currentmin=newmin;
					helper.currentmax=newmin+excursion;
				}			
	
				min=helper.currentmin;
				max=helper.currentmax;
			}
		}
			
			
				
		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{		
			if (Bars==null)	return;

			#if NT7
				int lastBar		= Math.Min(this.LastBarIndexPainted, Bars.Count - 1);
				int firstBar	= this.FirstBarIndexPainted;
			#else
				int lastBar		= Math.Min(ChartControl.LastBarPainted, Bars.Count - 1);
				int firstBar	= Math.Max((lastBar - ChartControl.BarsPainted) + 1,0);
			#endif
			
			helper.agregationFactor=Fibonacci(helper.agregationFib);
			helper.scalingfactor=(float)(bounds.Height/ChartControl.MaxMinusMin(max, min)*this.TickSize)*helper.agregationFactor;
			helper.y0=(float)(((float)(min/this.TickSize)))/helper.agregationFactor;
			helper.graphoffset=(float)(bounds.Y + bounds.Height);
			helper.graphics=graphics;
			helper.chartcontrol=base.ChartControl;
			helper.BAcellwidth=ChartControl.BarSpace/2.0f-2*ChartControl.BarWidth-1;
			helper.minTick=((int)(min/this.TickSize))/helper.agregationFactor;
			helper.maxTick=((int)(max/this.TickSize))/helper.agregationFactor;			
			helper.bounds=bounds;
			#if NT7
				helper.bars=BarsArray[0];
			#endif
			
			//we need room on the right
			if 	(ChartControl.BarMarginRight!=(int)(helper.BAcellwidth+gLadderRight.Width+gLadderExtremeRight.Width+2))
				ChartControl.BarMarginRight=(int)(helper.BAcellwidth+gLadderRight.Width+gLadderExtremeRight.Width+2);
						
			if (!firstinitdone)
			{	
				firstinitdone=true;
				if (helper.showBALadder)
				{
					ChartControl.BarSpace=initBarSpace;
					ChartControl.BarWidth=initBarWidth;
				}
				keyEvtH=new System.Windows.Forms.KeyEventHandler(this.chart_KeyDown);				
				this.ChartControl.ChartPanel.KeyDown += keyEvtH;
			}
			
			
			for (int index = lastBar; index >= firstBar; index--)
			{
				if (baLadders.ContainsKey(index))
					baLadders[index].DrawLadder();		
			}	

					
			float xleft,xright;
			xleft=bounds.Left ;
			#if !NT7
				xright=ChartControl.GetXByBarIdx(ChartControl.LastBarPainted)+ChartControl.BarWidth+helper.BAcellwidth+3;
			#else
				xright=ChartControl.GetXByBarIdx(BarsArray[0],ChartControl.LastBarPainted)+ChartControl.BarWidth+helper.BAcellwidth+3;
			#endif
			
			foreach (KeyValuePair<string,GLadderClass> ladderIterator in gLadders)
				{
					if (ladderIterator.Value.Show) 
					{
						if (string.Compare(ladderIterator.Key,"EL")==0)
							ladderIterator.Value.DrawLadder(lastBar,xleft);
						else if (string.Compare(ladderIterator.Key,"L")==0)
							ladderIterator.Value.DrawLadder(lastBar,xleft+gLadderExtremeLeft.Width);
						else if (string.Compare(ladderIterator.Key,"R")==0)
							ladderIterator.Value.DrawLadder(lastBar,xright);
						else if (string.Compare(ladderIterator.Key,"ER")==0)
							ladderIterator.Value.DrawLadder(lastBar,xright+gLadderRight.Width);
					}
				}		
			
			//show recording mode
			
			Color ColorNeutral=Color.FromArgb(255,255-ChartControl.BackColor.R,255-ChartControl.BackColor.G,255-ChartControl.BackColor.B);
			string msg=recordingMessage+" - Agreg Cells : "+helper.agregationFactor;
			graphics.FillRectangle(new SolidBrush(ChartControl.BackColor),bounds.Left,bounds.Bottom-20,graphics.MeasureString(msg,ChartControl.Font).Width,graphics.MeasureString(msg,ChartControl.Font).Height);

			graphics.DrawString(msg, ChartControl.Font, new SolidBrush(ColorNeutral), bounds.Left, bounds.Bottom-20,new StringFormat());

		}
		
		public override void Dispose()
        {

			if (keyEvtH != null)
			{	
				this.ChartControl.ChartPanel.KeyDown -= keyEvtH;
			   	keyEvtH=null;
			}
				base.Dispose();
		}

        #region Properties
		[XmlIgnore()]
		[Description("Color on ask and positive values")]
		[Category("6. Colors")]
		[Gui.Design.DisplayName("Positive")]
		
		public Color ColorUp
		{
			get { return (helper.colorUp); }
			set {  helper.colorUp=value;}
		}
		
		[Browsable(false)]
		public string ColorUpSerialize
			{
    			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(helper.colorUp); }
     			set { helper.colorUp = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
			}
			
		
		[XmlIgnore()]
		[Description("Color on bid and negative values")]
		[Category("6. Colors")]
		[Gui.Design.DisplayName("Negative")]
		
		public Color ColorDown
		{
			get { return helper.colorDown; }
			set {  helper.colorDown=value;}
		}
		
		[Browsable(false)]
		public string ColorDownSerialize
			{
    			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(helper.colorDown); }
     			set { helper.colorDown = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
			}
		
		[Description("Color of the volume bars")]
		[Category("6. Colors")]
		[Gui.Design.DisplayName("Volume Bar")]
		
		[XmlIgnore()]
		public Color ColorVolume
		{
			get { return helper.colorVolume; }
			set {  helper.colorVolume=value;}
		}
		
		[Browsable(false)]
		public string ColorVolumeSerialize
			{
    			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(helper.colorVolume); }
     			set { helper.colorVolume = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
			}
		
		
		[Description("Color of ladder background")]
		[Category("6. Colors")]
		[Gui.Design.DisplayName("Background")]
		
		[XmlIgnore()]
		public Color ColorBackground
		{
			get { return helper.colorBackGround; }
			set {  
				helper.colorBackGround=value;
				helper.myWhiteBrush.Color=helper.colorBackGround;
				}
		}
		
		[Browsable(false)]
		public string ColorBackGroundSerialize
			{
    			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(helper.colorBackGround); }
     			set { helper.colorBackGround = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
			}

		[XmlIgnore()]
		[Description("Color of last tick")]
		[Category("6. Colors")]
		[Gui.Design.DisplayName("Last Tick")]
		
		public Color ColorLastTick
		{
			get { return helper.colorLastTick; }
			set {  
				helper.colorLastTick=value;
				helper.myTickPen.Color=helper.colorLastTick;
				}
		}
			
		[Browsable(false)]
		public string ColorLastTickSerialize
			{
    			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(helper.colorLastTick); }
     			set { helper.colorLastTick = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
			}
		
		[XmlIgnore()]
		[Description("Color of maximum Volume Lzvel")]
		[Category("6. Colors")]
		[Gui.Design.DisplayName("Max Volume Level")]
		
		public Color ColorMaxVolLevel
		{
			get { return helper.colorMaxVolLevel; }
			set {  
				helper.colorMaxVolLevel=value;
				helper.myVolPen.Color=helper.colorMaxVolLevel;
				}
		}
			
		[Browsable(false)]
		public string ColorMaxVolLevelSerialize
			{
    			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(helper.colorMaxVolLevel); }
     			set { helper.colorMaxVolLevel = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
			}
		
		
		
		[Description("")]
		[Category("1. Extreme Left Ladder")]
		[Gui.Design.DisplayName("1. Show")]
		
		public bool ELshow
		{
			get { return gLadderExtremeLeft.Show; }
			set {   gLadderExtremeLeft.Show=value;}
		}		
		
		[Description("")]
		[Category("1. Extreme Left Ladder")]
		[Gui.Design.DisplayName("2. Ladder Type")]
		
		public LadderTypeEnum ELLadderType
		{
			get { return gLadderExtremeLeft.LadderType; }
			set {   gLadderExtremeLeft.LadderType=value;}
		}	
		
		[Description("Determines how the ladder is initialzed in the begginning of each session. It's the sum of 0 to max number of previous session lose ladders")]
		[Category("1. Extreme Left Ladder")]
		[Gui.Design.DisplayName("3. Accumulation Type")]
		
		public AccumulationTypeEnum ELAccumulationType
		{
			get { return gLadderExtremeLeft.AccumulationType; }
			set {  gLadderExtremeLeft.AccumulationType=value;}
		}	

		[Description("Used only if above type is nDays")]
		[Category("1. Extreme Left Ladder")]
		[Gui.Design.DisplayName("4. Accumulation Days")]
		
		public int ELAccumulationDays
		{
			get { return gLadderExtremeLeft.AccumulationDays; }
			set {  gLadderExtremeLeft.AccumulationDays=value;}
		}
		
		[Description("")]
		[Category("1. Extreme Left Ladder")]
		[Gui.Design.DisplayName("5. Number Format")]
		
		public FormattingType ELNumberFormat
		{
			get { return gLadderExtremeLeft.LadderFormattingType; }
			set {  gLadderExtremeLeft.LadderFormattingType=value;}
		}
		
		[Description("Determines how are alpha blended colors and sized rectangles. Max value of current ladder or fixes value")]
		[Category("1. Extreme Left Ladder")]
		[Gui.Design.DisplayName("6. Ratio Type")]

		public RatioType ELRatioType
		{
			get { return gLadderExtremeLeft.LadderRatioType; }
			set {  gLadderExtremeLeft.LadderRatioType=value;}
		}
		
		[Description("Used only if above is fixed number. Determines max value expected. Above values are displayed 100%")]
		[Category("1. Extreme Left Ladder")]
		[Gui.Design.DisplayName("7. Ratio Max Number")]

		public int ELRatioNumber
		{
			get { return gLadderExtremeLeft.LadderFixedNumber; }
			set {  gLadderExtremeLeft.LadderFixedNumber=value;}
		}
		
		
		
		
		
		[Description("")]
		[Category("2. Left Ladder")]
		[Gui.Design.DisplayName("1. Show")]
		
		public bool Lshow
		{
			get { return gLadderLeft.Show; }
			set {   gLadderLeft.Show=value;}
		}		
		
		[Description("")]
		[Category("2. Left Ladder")]
		[Gui.Design.DisplayName("2. Ladder Type")]
		
		public LadderTypeEnum LLadderType
		{
			get { return gLadderLeft.LadderType; }
			set {   gLadderLeft.LadderType=value;}
		}	
		
		[Description("Determines how the ladder is initialzed in the begginning of each session. It's the sum of 0 to max number of previous session lose ladders")]
		[Category("2. Left Ladder")]
		[Gui.Design.DisplayName("3. Accumulation Type")]
		
		public AccumulationTypeEnum LAccumulationType
		{
			get { return gLadderLeft.AccumulationType; }
			set {  gLadderLeft.AccumulationType=value;}
		}	

		[Description("Used only if above type is nDays")]
		[Category("2. Left Ladder")]
		[Gui.Design.DisplayName("4. Accumulation Days")]
		
		public int LAccumulationDays
		{
			get { return gLadderLeft.AccumulationDays; }
			set {  gLadderLeft.AccumulationDays=value;}
		}
		
		[Description("")]
		[Category("2. Left Ladder")]
		[Gui.Design.DisplayName("5. Number Format")]
		
		public FormattingType LNumberFormat
		{
			get { return gLadderLeft.LadderFormattingType; }
			set {  gLadderLeft.LadderFormattingType=value;}
		}
		
		[Description("Determines how are alpha blended colors and sized rectangles. Max value of current ladder or fixes value")]
		[Category("2. Left Ladder")]
		[Gui.Design.DisplayName("6. Ratio Type")]

		public RatioType LRatioType
		{
			get { return gLadderLeft.LadderRatioType; }
			set {  gLadderLeft.LadderRatioType=value;}
		}
		
		[Description("Used only if above is fixed number. Determines max value expected. Above values are displayed 100%")]
		[Category("2. Left Ladder")]
		[Gui.Design.DisplayName("7. Ratio Max Number")]

		public int LRatioNumber
		{
			get { return gLadderLeft.LadderFixedNumber; }
			set {  gLadderLeft.LadderFixedNumber=value;}
		}
		
		
		
		
		[Description("")]
		[Category("3. Right Ladder")]
		[Gui.Design.DisplayName("1. Show")]
		
		public bool Rshow
		{
			get { return gLadderRight.Show; }
			set {   gLadderRight.Show=value;}
		}		
		
		[Description("")]
		[Category("3. Right Ladder")]
		[Gui.Design.DisplayName("2. Ladder Type")]
		
		public LadderTypeEnum RLadderType
		{
			get { return gLadderRight.LadderType; }
			set {   gLadderRight.LadderType=value;}
		}	
		
		[Description("Determines how the ladder is initialzed in the begginning of each session. It's the sum of 0 to max number of previous session lose ladders")]
		[Category("3. Right Ladder")]
		[Gui.Design.DisplayName("3. Accumulation Type")]
		
		public AccumulationTypeEnum RAccumulationType
		{
			get { return gLadderRight.AccumulationType; }
			set {  gLadderRight.AccumulationType=value;}
		}	

		[Description("Used only if above type is nDays")]
		[Category("3. Right Ladder")]
		[Gui.Design.DisplayName("4. Accumulation Days")]
		
		public int RAccumulationDays
		{
			get { return gLadderRight.AccumulationDays; }
			set {  gLadderRight.AccumulationDays=value;}
		}
		
		[Description("")]
		[Category("3. Right Ladder")]
		[Gui.Design.DisplayName("5. Number Format")]
		
		public FormattingType RNumberFormat
		{
			get { return gLadderRight.LadderFormattingType; }
			set {  gLadderRight.LadderFormattingType=value;}
		}
		
		[Description("Determines how are alpha blended colors and sized rectangles. Max value of current ladder or fixes value")]
		[Category("3. Right Ladder")]
		[Gui.Design.DisplayName("6. Ratio Type")]

		public RatioType RRatioType
		{
			get { return gLadderRight.LadderRatioType; }
			set {  gLadderRight.LadderRatioType=value;}
		}
		
		[Description("Used only if above is fixed number. Determines max value expected. Above values are displayed 100%")]
		[Category("3. Right Ladder")]
		[Gui.Design.DisplayName("7. Ratio Max Number")]

		public int RRatioNumber
		{
			get { return gLadderRight.LadderFixedNumber; }
			set {  gLadderRight.LadderFixedNumber=value;}
		}
		
		
				
		
		[Description("")]
		[Category("4. Extreme Right Ladder")]
		[Gui.Design.DisplayName("1. Show")]
		
		public bool ERshow
		{
			get { return gLadderExtremeRight.Show; }
			set {   gLadderExtremeRight.Show=value;}
		}		
		
		[Description("")]
		[Category("4. Extreme Right Ladder")]
		[Gui.Design.DisplayName("2. Ladder Type")]
		
		public LadderTypeEnum ERLadderType
		{
			get { return gLadderExtremeRight.LadderType; }
			set {   gLadderExtremeRight.LadderType=value;}
		}	
		
		[Description("Determines how the ladder is initialzed in the begginning of each session. It's the sum of 0 to max number of previous session lose ladders")]
		[Category("4. Extreme Right Ladder")]
		[Gui.Design.DisplayName("3. Accumulation Type")]
		
		public AccumulationTypeEnum ERAccumulationType
		{
			get { return gLadderExtremeRight.AccumulationType; }
			set {  gLadderExtremeRight.AccumulationType=value;}
		}	

		[Description("Used only if above type is nDays")]
		[Category("4. Extreme Right Ladder")]
		[Gui.Design.DisplayName("4. Accumulation Days")]
		
		public int ERAccumulationDays
		{
			get { return gLadderExtremeRight.AccumulationDays; }
			set {  gLadderExtremeRight.AccumulationDays=value;}
		}
		
		[Description("")]
		[Category("4. Extreme Right Ladder")]
		[Gui.Design.DisplayName("5. Number Format")]
		
		public FormattingType ERNumberFormat
		{
			get { return gLadderExtremeRight.LadderFormattingType; }
			set {  gLadderExtremeRight.LadderFormattingType=value;}
		}
		
		[Description("Determines how are alpha blended colors and sized rectangles. Max value of current ladder or fixes value")]
		[Category("4. Extreme Right Ladder")]
		[Gui.Design.DisplayName("6. Ratio Type")]

		public RatioType ERRatioType
		{
			get { return gLadderExtremeRight.LadderRatioType; }
			set {  gLadderExtremeRight.LadderRatioType=value;}
		}
		
		[Description("Used only if above is fixed number. Determines max value expected. Above values are displayed 100%")]
		[Category("4. Extreme Right Ladder")]
		[Gui.Design.DisplayName("7. Ratio Max Number")]

		public int ERRatioNumber
		{
			get { return gLadderExtremeRight.LadderFixedNumber; }
			set {  gLadderExtremeRight.LadderFixedNumber=value;}
		}
		
		[Description("Determines initial Fib aggregation Level of the cells")]
		[Category("Settings")]
		[Gui.Design.DisplayName("Aggregation Fib Level")]

		public int FibLevel
		{
			get {return helper.agregationFib;}
			set {helper.agregationFib=value;
				 helper.agregationFactor=Fibonacci(helper.agregationFib);}
			
		}
		
		[Description("Show or hide the BA ladders")]
		[Category("5. BA Ladder")]
		[Gui.Design.DisplayName("01. Show")]

		public bool ShowBALadder
		{
			get {return helper.showBALadder;}
			set {helper.showBALadder=value;}
		}
		
		[Description("")]
		[Category("5. BA Ladder")]
		[Gui.Design.DisplayName("02. Number Format")]
		
		public FormattingType BALadderFormat
		{
			get { return helper.ladderBAFormat; }
			set {  helper.ladderBAFormat=value;}
		}
		
		[Description("To spare memory, in live mode there is 1 ladder per session, not per bar (except for BA ladders of course)")]
		[Category("Settings")]
		[Gui.Design.DisplayName("Live Mode")]

		public bool LiveMode
		{
			get {return helper.liveMode;}
			set {helper.liveMode=value;}
		}
		
		[Description("Delta mode on BA Ladder : it will show Total Volume on the Left and Delta on the right")]
		[Category("5. BA Ladder")]
		[Gui.Design.DisplayName("03. Delta Mode")]

		public bool BADeltaMode
		{
			get {return helper.BAdeltaMode;}
			set {helper.BAdeltaMode=value;}
		}
		 
		[Description("Determines how are alpha blended colors and sized rectangles. Max value of current ladder or fixes value")]
		[Category("5. BA Ladder")]
		[Gui.Design.DisplayName("04. Ratio Type")]

		public RatioType BARatioType
		{
			get { return helper.ladderBARatio; }
			set {  helper.ladderBARatio=value;}
		}
		
		[Description("Used only if above is fixed number. Determines max value expected. Above values are displayed 100%")]
		[Category("5. BA Ladder")]
		[Gui.Design.DisplayName("05. Max Number")]

		public int BARatioNumber
		{
			get { return helper.ladderBAFixedNumber; }
			set {  helper.ladderBAFixedNumber=value;}
		}
				
		[Description("Show Maximum Volume Level : POC of bar")]
		[Category("5. BA Ladder")]
		[Gui.Design.DisplayName("06. Show Max Volume Level")]

		public bool ShowMaxLevel
		{
			get { return helper.ladderBAShowMaxLevel; }
			set {  helper.ladderBAShowMaxLevel=value;}
		}
		
		[Description("Show COT")]
		[Category("5. BA Ladder")]
		[Gui.Design.DisplayName("07. Show COT")]

		public bool ShowCOT
		{
			get { return helper.ladderBAShowCOT; }
			set {  helper.ladderBAShowCOT=value;}
		}
		
		[Description("Show Delta")]
		[Category("5. BA Ladder")]
		[Gui.Design.DisplayName("08. Show Delta")]

		public bool ShowDelta
		{
			get { return helper.ladderBAShowDelta; }
			set {  helper.ladderBAShowDelta=value;}
		}
		
		[Description("Show Numbers")]
		[Category("5. BA Ladder")]
		[Gui.Design.DisplayName("09. Show Numbers")]

		public bool ShowNumbers
		{
			get { return helper.ladderBAShowNumbers; }
			set {  helper.ladderBAShowNumbers=value;}
		}
		
		[Description("Show Bars instead of saturation variation")]
		[Category("5. BA Ladder")]
		[Gui.Design.DisplayName("10. Show Bars")]

		public bool ShowBars
		{
			get { return helper.ladderBAShowBars; }
			set {  helper.ladderBAShowBars=value;}
		}
		
		[Description("No minus sign")]
		[Category("5. BA Ladder")]
		[Gui.Design.DisplayName("11. No Minus Sign")]

		public bool NoMinus
		{
			get { return helper.ladderBANoMinus; }
			set {  helper.ladderBANoMinus=value;}
		}
		
		[XmlIgnore()]
		[Description("Delta Font")]
		[Category("5. BA Ladder")]
		[Gui.Design.DisplayName("12. Delta Font")]
		public Font DeltaFont		
		{
			get { return helper.ladderBADeltaFont; }
			set { helper.ladderBADeltaFont = value; }
		}
		
		
		
		
		[Description("Shows or hides borders and numbers")]
		[Category("Settings")]
		[Gui.Design.DisplayName("Show Numbers/Borders")]

		public bool ShowBorderNumbers
		{
			get {return helper.showBordersNumbers;}
			set {helper.showBordersNumbers=value;}
		}
				
		[XmlIgnore()]
		[Description("Font Properties")]
		[Category("6. Colors")]
		[Gui.Design.DisplayNameAttribute("_Font Properties")]
		public Font MyFont
			
		{
			get { return myFont; }
			set { myFont = value; }
		}
		
		
		[Browsable(false)]
		[XmlIgnore()]
		public IntSeries HighCOT
		{
			get 
			{ 
				return helper.dsHighCOT; 
			}
		}

		[Browsable(false)]
		[XmlIgnore()]
		public IntSeries LowCOT
		{
			get 
			{ 
				return helper.dsLowCOT; 
			}
		}
		
		[Browsable(false)]
		[XmlIgnore()]
		public IntSeries MaxBid
		{
			get 
			{ 
				return helper.dsMaxBid; 
			}
		}
		
		[Browsable(false)]
		[XmlIgnore()]
		public IntSeries MaxAsk
		{
			get 
			{ 
				return helper.dsMaxAsk; 
			}
		}
		
		[Browsable(false)]
		[XmlIgnore()]
		public IntSeries SumBid
		{
			get 
			{ 
				return helper.dsSumBid; 
			}
		}
				
		[Browsable(false)]
		[XmlIgnore()]
		public IntSeries SumAsk
		{
			get 
			{ 
				return helper.dsSumAsk; 
			}
		}
				
		[Browsable(false)]
		[XmlIgnore()]
		public IntSeries Delta
		{
			get 
			{ 
				return helper.dsDelta; 
			}
		}
				
		[Browsable(false)]
		[XmlIgnore()]
		public IntSeries Volume
		{
			get 
			{ 
				return helper.dsVolume; 
			}
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
        private GomVolumeLadder[] cacheGomVolumeLadder = null;

        private static GomVolumeLadder checkGomVolumeLadder = new GomVolumeLadder();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public GomVolumeLadder GomVolumeLadder()
        {
            return GomVolumeLadder(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public GomVolumeLadder GomVolumeLadder(Data.IDataSeries input)
        {
            if (cacheGomVolumeLadder != null)
                for (int idx = 0; idx < cacheGomVolumeLadder.Length; idx++)
                    if (cacheGomVolumeLadder[idx].EqualsInput(input))
                        return cacheGomVolumeLadder[idx];

            lock (checkGomVolumeLadder)
            {
                if (cacheGomVolumeLadder != null)
                    for (int idx = 0; idx < cacheGomVolumeLadder.Length; idx++)
                        if (cacheGomVolumeLadder[idx].EqualsInput(input))
                            return cacheGomVolumeLadder[idx];

                GomVolumeLadder indicator = new GomVolumeLadder();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                GomVolumeLadder[] tmp = new GomVolumeLadder[cacheGomVolumeLadder == null ? 1 : cacheGomVolumeLadder.Length + 1];
                if (cacheGomVolumeLadder != null)
                    cacheGomVolumeLadder.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheGomVolumeLadder = tmp;
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
        public Indicator.GomVolumeLadder GomVolumeLadder()
        {
            return _indicator.GomVolumeLadder(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.GomVolumeLadder GomVolumeLadder(Data.IDataSeries input)
        {
            return _indicator.GomVolumeLadder(input);
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
        public Indicator.GomVolumeLadder GomVolumeLadder()
        {
            return _indicator.GomVolumeLadder(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.GomVolumeLadder GomVolumeLadder(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.GomVolumeLadder(input);
        }
    }
}
#endregion
