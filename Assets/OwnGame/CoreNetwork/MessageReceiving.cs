using System;
 
public class MessageReceiving {
	private int lengData;
	private int currentReader;
	
	private short cmd;
	private byte[] buffer;
	private bool isMessageCorrect;
	public MessageReceiving(byte[] DATA) {
		int ch1 = DATA[0] & 0xFF;
		int ch2 = DATA[1] & 0xFF;
		cmd=(short)((ch1 << 8) | (ch2 << 0));

        lengData=DATA.Length;
		currentReader=2;
		buffer=DATA;

		isMessageCorrect=true;
	}
	public short getCMD() {return cmd;}
	public string getCMDName(){return CMD_REALTIME.getCMD(cmd);}
	public int avaiable() {return lengData-currentReader;}
	public byte[] getEndByte() {
		int lengClone=lengData-currentReader;
		byte[] data=new byte[lengClone];
		for(int i=0;i<lengClone;i++)
			data[i]=buffer[i+currentReader];
		return data;
	}
	public bool validate() {return isMessageCorrect && (lengData-currentReader)==0;}
    public bool isCorrect() {return isMessageCorrect;}
    public bool isRelease() {return lengData-currentReader==0;}
    public int lengthReceive() { return lengData; }
	
	public bool readBoolean() {
		if(currentReader<lengData){
			bool result=buffer[currentReader]!=0;
			currentReader++;
			return result;
		}else{
			isMessageCorrect=false;
			return false;
		}
	}
	public sbyte readByte() {
		if(currentReader<lengData){
			sbyte result= (sbyte)buffer[currentReader];
			currentReader++;
			return result;
		}else{
			isMessageCorrect=false;
			return 0;
		}
	}
	
	public short readShort() {
		if(currentReader+1<lengData){
			int ch1 = buffer[currentReader] & 0xFF;
			int ch2 = buffer[currentReader+1] & 0xFF;
			currentReader=currentReader+2;
			return (short)((ch1 << 8) + (ch2 << 0));
		}else{
			isMessageCorrect=false;
			return 0;
		}
	}
	
	public int readInt() {
		if(currentReader+3<lengData){
	        int ch1 = buffer[currentReader] & 0xFF;
	        int ch2 = buffer[currentReader+1] & 0xFF;
	        int ch3 = buffer[currentReader+2] & 0xFF;
	        int ch4 = buffer[currentReader+3] & 0xFF;
			currentReader=currentReader+4;
			return ((ch1 << 24) + (ch2 << 16) + (ch3 << 8) + (ch4 << 0));
		}else{
			isMessageCorrect=false;
			return 0;
		}
	}
	
	public long readLong() {
		if(currentReader+7<lengData){
			long l0 = buffer[currentReader] & 0xFF;
			long l1 = buffer[currentReader+1] & 0xFF;
			long l2 = buffer[currentReader+2] & 0xFF;
			long l3 = buffer[currentReader+3] & 0xFF;
			long l4 = buffer[currentReader+4] & 0xFF;
			long l5 = buffer[currentReader+5] & 0xFF;
			long l6 = buffer[currentReader+6] & 0xFF;
			long l7 = buffer[currentReader+7] & 0xFF;

			long r0 = l0 << 56;
			long r1 = l1 << 48;
			long r2 = l2 << 40;
			long r3 = l3 << 32;
			long r4 = l4 << 24;
			long r5 = l5 << 16;
			long r6 = l6 << 8;
			long r7 = l7;
			currentReader=currentReader+8;
			return r0 + r1 + r2 + r3 + r4 + r5 + r6 + r7;
		}else{
			isMessageCorrect=false;
			return 0;
		}
	}
	
	public float readFloatFromInt() {return ((float)readInt())/1000;}
	public double readDoubleFromLong() {return ((double)readLong())/1000000;}
	public sbyte[] readMiniByte(){
		sbyte lengthReceive = readByte();
		if (lengthReceive < 1)
			return null;
		sbyte[] dataReceive = new sbyte[lengthReceive];
		for (sbyte i = 0; i < lengthReceive; i++)
			dataReceive[i] = readByte();
		return dataReceive;
	}

	public String readString(){
		if(lengData-currentReader<2){
			isMessageCorrect=false;
			return "";
		}
		int utflen = (((buffer[currentReader] & 0xff) << 8) | (buffer[currentReader+1] & 0xff));
		if(lengData-currentReader<utflen+2){
			isMessageCorrect=false;
			return "";
		}
		byte[] bytearr = null;
		char[] chararr = null;
//		if(data.length<utflen){
			bytearr = new byte[utflen*2];
			chararr = new char[utflen*2];
//		}
		
		int c, char2, char3;
		int count = 0;
		int chararr_count=0;
		
		for(int i=0;i<utflen;i++)
			bytearr[i]=buffer[i+currentReader+2];
		
		currentReader=currentReader+utflen+2;
		
		while (count < utflen) {
			c = (int) bytearr[count] & 0xff;
			if (c > 127) break;
			count++;
			chararr[chararr_count++]=(char)c;
		}
		
		while (count < utflen) {
			c = (int) bytearr[count] & 0xff;
			switch (c >> 4) {
				case 0: case 1: case 2: case 3: case 4: case 5: case 6: case 7:
					/* 0xxxxxxx*/
					count++;
					chararr[chararr_count++]=(char)c;
					break;
				case 12: case 13:
					/* 110x xxxx   10xx xxxx*/
					count += 2;
					if (count > utflen){
//						throw new UTFDataFormatException("malformed input: partial character at end");
						return "";
					}
					char2 = (int) bytearr[count-1];
					if ((char2 & 0xC0) != 0x80){
//						throw new UTFDataFormatException("malformed input around byte " + count);
						return "";
					}
					chararr[chararr_count++]=(char)(((c & 0x1F) << 6) |
							(char2 & 0x3F));
					break;
				case 14:
					/* 1110 xxxx  10xx xxxx  10xx xxxx */
					count += 3;
					if (count > utflen){
//						throw new UTFDataFormatException("malformed input: partial character at end");
						return "";
					}
					char2 = (int) bytearr[count-2];
					char3 = (int) bytearr[count-1];
					if (((char2 & 0xC0) != 0x80) || ((char3 & 0xC0) != 0x80)){
//						throw new UTFDataFormatException("malformed input around byte " + (count-1));
						return "";
					}
					chararr[chararr_count++]=(char)(((c     & 0x0F) << 12) |
							((char2 & 0x3F) << 6)  |
							((char3 & 0x3F) << 0));
					break;
				default:
					/* 10xx xxxx,  1111 xxxx */
//					throw new UTFDataFormatException("malformed input around byte " + count);
					return "";
			}
		}
		// The number of chars produced may be less than utflen
		return new String(chararr, 0, chararr_count);
	}
}
