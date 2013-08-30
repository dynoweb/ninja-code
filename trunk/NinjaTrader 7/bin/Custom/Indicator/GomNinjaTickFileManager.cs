using System;
using System.ComponentModel;
using System.IO;
using System.Globalization;
using System.Text;
using System.Diagnostics;
using System.Reflection;


////THANKS TO MR JOE FOR HAVING REVERSE ENGINEERED NINJA SPEC
/// see http://www.bigmiketrading.com/ninjatrader-programming/7396-ntd-file-specification.html

namespace Gom
{

	partial class GomNinjaTickFileManager : IDataManager, IDisposable
	{
	
        public string Name { get {return "NinjaTickFile"; }}
        public bool IsWritable { get { return true; } }
        public bool IsMillisecCompliant { get { return false; } }

		private string _InstrName;
		public double _TickSize;

		protected DateTime curReadDate = Gom.Utils.nullDT;
		DateTime curTime;
		double multiplier;
		double curPrice;
		ulong firstVolume;
	
		bool newFile=true;

		protected BinaryReader br;
		protected BinaryWriter bw;
		
		private bool _writeOK = true;
		
		long curDateTicks=0L;
		
		bool isRealNinja=false;
		
		private void initWrite()
		{
			freewriter();

			string FileName = GetFileName(new DateTime(curDateTicks),false);

			if (!File.Exists(FileName))
			{
				FileStream fs = File.Create(FileName);
				fs.Close();
			}
			
			try
			{
				bw = new BinaryWriter(File.Open(FileName, FileMode.Append, FileAccess.Write, FileShare.Read));
				_writeOK = true;
			}
			catch (IOException)
			{
				_writeOK = false;
			}
			
		}
		
		private bool initread(string FileName)
		{
			bool found;

			freereader();

			try
			{
				br = new BinaryReader(File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
				newFile=true;
				found = true;
			}
			catch (IOException)
			{
				found = false;
			}
			return (found);
		}

		
		private bool FindFileNameAndOpen(DateTime date)
		{
			bool found = false;

			while (date <= DateTime.Now)
			{
				string FileName;
				
				FileName= GetFileName(date,true);
				
				if (!File.Exists(FileName))
				{
					isRealNinja=false;
					FileName=GetFileName(date,false);
				}
				else
				{	
					isRealNinja=true;
					if (File.Exists(GetFileName(date,false)))
						File.Delete(GetFileName(date,false));
				}
				
				if (File.Exists(FileName))
				{
					FileInfo f = new FileInfo(FileName);
					if (f.Length > 0)
					{
						curReadDate = date;
						found = initread(FileName);
						break;
					}
				}
				date = date.AddHours(1);
			}
			return found;
		}

		private bool ManageFileChange()
		{
			bool found = false;

			freereader();

			found = FindFileNameAndOpen(curReadDate.AddHours(1));

			return found;
		}

		private string GetFileName(DateTime date,bool isRN)
		{
			string folder;
			
			if (!isRN)
			{
				folder = Environment.GetEnvironmentVariable("GOMFOLDER");

				if (String.IsNullOrEmpty(folder))
					folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				
				folder=folder+@"\"+_InstrName+".";
			}
			else
				folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+@"\NinjaTrader 7\db\tick\"+_InstrName+@"\";
			
			return(folder+date.AddHours(1).ToString("yyyyMMddHH")+"00.Last.ntd");
			
		}

		private void ReadFirstLine()
		{	
			uint  reccount;
			double price1;
			double price2;
			double price3;
			
			multiplier=-br.ReadDouble();
			reccount=br.ReadUInt32();
			reccount=br.ReadUInt32();
			curPrice=br.ReadDouble();
			curPrice=br.ReadDouble();
			curPrice=br.ReadDouble();
			curPrice=br.ReadDouble();;
			curTime=new DateTime(br.ReadInt64());
			firstVolume=br.ReadUInt64();
			
		}
		
		public bool RecordTick(DateTime date, double bid, double ask, double price, int volume)
		{
			
			long newDateTicks = new DateTime(date.Year,date.Month,date.Day,date.Hour,0,0).Ticks;

			if (((_writeOK) && (newDateTicks > curDateTicks)) || (curDateTicks == 0))
			{
				curDateTicks = newDateTicks;
				initWrite();
			}

			if (_writeOK)
			{
				bw.Write(date.Ticks);
				bw.Write(price);
				bw.Write(volume);
				bw.Flush();
			}
			
			return _writeOK;
		}

		
		private int GetBigEndian(int nbBytes)
		{
			int retval=0;
			
			for (int i=0;i<nbBytes;i++)
				{
					retval = retval << 8 ;
					retval += br.ReadByte();
				}
			
			return retval;
				
		}
		
		
		public  void GetNextTick(ref MarketDataType gomData)
		{
			if (isRealNinja)
			{
			if (newFile)
			{
				ReadFirstLine();
				gomData.Price=curPrice;
				gomData.TickType=TickTypeEnum.Unknown;
				gomData.Volume=(int)firstVolume;
				gomData.Time=curTime;
				newFile=false;
			}
			else
			{	
				byte statbyte;
				int i;

				try
				{
					statbyte = br.ReadByte();
				}
				catch (EndOfStreamException)
				{
					gomData.Time = Gom.Utils.nullDT;
					if (ManageFileChange())
						GetNextTick(ref  gomData);
					return;
				}
				

				//time

				int nbbytestimes=statbyte & 3 ; //0000011
				int nbsec = GetBigEndian(nbbytestimes);

				curTime = curTime.AddSeconds(nbsec);
				gomData.Time=curTime;
				
				
				//price
				int nbbytesprice=(statbyte & 12)>>2;//0001100		
				if (nbbytesprice==3)
					nbbytesprice=4;
				
				int deltaprice=GetBigEndian(nbbytesprice);
	
				switch(nbbytesprice)
				{
					case 1:
						deltaprice -= 0x80;
						break;
					case 2:
						deltaprice -= 0x4000;
						break;
					case 4:
						deltaprice -= 0x40000000;
						break;
				}
				
				curPrice += multiplier*deltaprice;
				gomData.Price=curPrice;
				

				//volume
				int nbbytesvolume=0;
				int statusvol=(statbyte & 112)>>4;
				
				switch (statusvol)    //01110000
				{
					case 1: //001
						nbbytesvolume=1;
						break;
					case 6: //110
						nbbytesvolume=2;
						break;
					case 7: //111
						nbbytesvolume=4;
						break;
					case 2: //010
						nbbytesvolume=8;
						break;
					case 3: //011
						nbbytesvolume=1;
						break;
					case 4: //100
						nbbytesvolume=1;
						break;
					case 5: //101
						nbbytesvolume=1;
						break;
				}	
				
				int volume=GetBigEndian(nbbytesvolume);
					
				if (statusvol==3)
					volume *= 100;
				else if (statusvol==4)
					volume *= 500;
				else if (statusvol==5)
					volume *= 1000;
				
				gomData.Volume=volume;
				gomData.TickType=TickTypeEnum.Unknown;
				
			}
			}
			else
			{				
				try
				{
					gomData.Time = new DateTime(br.ReadInt64());
				}
				catch (EndOfStreamException)
				{
					gomData.Time = Gom.Utils.nullDT;
					if (ManageFileChange())
						GetNextTick(ref  gomData);
					return;
				}
				
				gomData.Price=br.ReadDouble();
				gomData.Volume=br.ReadInt32();
				gomData.TickType=TickTypeEnum.Unknown;
			}
				
		}
		

		public void SetCursorTime(DateTime time0, ref MarketDataType gomData)
		{
			bool found = false;
			gomData.Time=Gom.Utils.nullDT;
			long time0tick = time0.Ticks;
			long ticktime;
			
			found = FindFileNameAndOpen(new DateTime(time0.Year,time0.Month,time0.Day,time0.Hour,0,0));

			if (found)
				do
				{
					GetNextTick(ref gomData);
					ticktime = gomData.Time.Ticks;
				}
				while ((ticktime != 0L) && (ticktime < time0tick));
		}


		//IDisposable
		#region IDisposable
		private void freereader()
		{
			if (br != null)
			{
				br.Close();
				br = null;
			}
		}
		
		private void freewriter()
		{
			if (bw != null)
			{
				bw.Close();
				bw = null;
			}
		}
		

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				freereader();
				freewriter();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion


		public GomNinjaTickFileManager(bool isInstr, string name, double tickSize, bool writeData, Gom.FileModeType fileMode)
		{
			if (isInstr)
			{	_InstrName = name;
				_TickSize=tickSize;
			}
			
		}


		public GomNinjaTickFileManager()
		{
		}

	}



}

