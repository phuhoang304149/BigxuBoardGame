using System;

public class MessageSending {
    public int timeWithNetwork;
	private short cmd;
	private byte[] data;
	public MessageSending(short CMD) {
		cmd=CMD;
		data = new byte[2];
        data[0] = (byte)((int)((uint)CMD >> 8) & 0xFF);
        data[1] = (byte)((int)((uint)CMD >> 0) & 0xFF);
	}

	public void ClearData(){
		data = new byte[2];
		data[0] = (byte)((int)((uint)cmd >> 8) & 0xFF);
		data[1] = (byte)((int)((uint)cmd >> 0) & 0xFF);
	}
    public int avaiable() {return data.Length;}
	public short getCMD() {return cmd;}
	public string getCMDName() { return CMD_REALTIME.getCMD(cmd); }
	
	public void writeBoolean(bool value) {
        int length=data.Length;
		byte[] temp=new byte[length+1];
		for(int i=0;i<length;i++)
			temp[i]=data[i];
		if(value)
			temp[length]=1;
		else
			temp[length]=0;
		data=temp;
	}
	
	public void writeByte(byte value) {
        int length=data.Length;
		byte[] temp=new byte[length+1];
		for(int i=0;i<length;i++)
			temp[i]=data[i];
		temp[length]=value;
		data=temp;
	}
	
	public void writeByteArray(byte[] arr) {
        int l=data.Length;
		byte[] temp;
		if(arr==null){
			temp=new byte[l+4];
			for(int i=0;i<l;i++)
				temp[i]=data[i];
		}else{
            int larr=arr.Length;
			temp=new byte[l+4+larr];
			for(int i=0;i<l;i++)
				temp[i]=data[i];
			for(int i=0;i<larr;i++)
				temp[l+4+i]=arr[i];
            temp[l]  =(byte)((int)((uint)larr >> 24) & 0xFF);
            temp[l+1]=(byte)((int)((uint)larr >> 16) & 0xFF);
            temp[l+2]=(byte)((int)((uint)larr >> 8) & 0xFF);
            temp[l+3]=(byte)((int)((uint)larr >> 0) & 0xFF);
		}
		data=temp;
	}

    public void writeshort(short paramInt) {
        int length=data.Length;
		byte[] temp=new byte[length+2];
		for(int i=0;i<length;i++)
			temp[i]=data[i];
        temp[length]	= (byte) ((int)((uint)paramInt >> 8) & 0xFF);
        temp[length+1]	= (byte) ((int)((uint)paramInt >> 0) & 0xFF);
		data=temp;
	}

    public void writeInt(int paramInt) {
        int length=data.Length;
		byte[] temp=new byte[length+4];
		for(int i=0;i<length;i++)
			temp[i]=data[i];
        temp[length]  =(byte)((int)((uint)paramInt >> 24) & 0xFF);
        temp[length+1]=(byte)((int)((uint)paramInt >> 16) & 0xFF);
        temp[length+2]=(byte)((int)((uint)paramInt >> 8) & 0xFF);
        temp[length+3]=(byte)((int)((uint)paramInt >> 0) & 0xFF);
		data=temp;
	}

    public void writeLong(long paramLong) {
        int length=data.Length;
		byte[] temp=new byte[length+8];
		for(int i=0;i<length;i++)
			temp[i]=data[i];
        temp[length]   = (byte)((ulong)paramLong >> 56);
        temp[length+1] = (byte)((ulong)paramLong >> 48);
        temp[length+2] = (byte)((ulong)paramLong >> 40);
        temp[length+3] = (byte)((ulong)paramLong >> 32);
        temp[length+4] = (byte)((ulong)paramLong >> 24);
        temp[length+5] = (byte)((ulong)paramLong >> 16);
        temp[length+6] = (byte)((ulong)paramLong >> 8);
        temp[length+7] = (byte)((ulong)paramLong >> 0);
		data=temp;
	}
	
	public void writeCopyData(byte[] copyData) {
		if(copyData==null)
			return;
        int lengthCopy=copyData.Length;
		if(lengthCopy==0)
			return;
        int currentLength=data.Length;
		byte[] temp=new byte[currentLength+lengthCopy];
		for(int i=0;i<currentLength;i++)
			temp[i]=data[i];
		for(int i=0;i<lengthCopy;i++)
			temp[i+currentLength]=copyData[i];
		data=temp;
	}
	
	public void writeFloatFromInt(float n) {writeInt((int) (n*1000));}
	public void writeDoubleFromLong(double n) {writeLong((long) (n*1000000));}
	
	public int writeString(String value) {
		if(value==null)
			value="";
        int stringLenth = value.Length;
		int j = 0;
	    int k;
		for (int n = 0; n < stringLenth; n++) {
			k = value[n];
			if ((k >= 1) && (k <= 127)) {
				j++;
			} else if (k > 2047) {
				j += 3;
			} else {
				j += 2;
			}
		}
//		if (j > 65535) {
//			throw new UTFDataFormatException("encoded string too long: " + j + " bytes");
//		}
		byte[] arrayOfString = new byte[j * 2 + 2];
        arrayOfString[0] = (byte)((int)((uint)j >> 8) & 0xFF);
        arrayOfString[1] = (byte)((int)((uint)j >> 0) & 0xFF);

		int count=2;
		int i1 = 0;
		for (i1 = 0; i1 < stringLenth; i1++) {
            k = value[i1];
			if ((k < 1) || (k > 127)) {
				break;
			}
			arrayOfString[(count++)] = ((byte) k);
		}
		while (i1 < stringLenth) {
			k = value[i1];
			if ((k >= 1) && (k <= 127)) {
				arrayOfString[(count++)] = ((byte) k);
			} else if (k > 2047) {
				arrayOfString[(count++)] = ((byte) (0xE0 | k >> 12 & 0xF));
				arrayOfString[(count++)] = ((byte) (0x80 | k >> 6 & 0x3F));
				arrayOfString[(count++)] = ((byte) (0x80 | k >> 0 & 0x3F));
			} else {
				arrayOfString[(count++)] = ((byte) (0xC0 | k >> 6 & 0x1F));
				arrayOfString[(count++)] = ((byte) (0x80 | k >> 0 & 0x3F));
			}
			i1++;
		}
//	    paramDataOutput.write(arrayOfString, 0, j + 2);
		int lengString = j+2;
        int lengthData=data.Length;
		byte[] temp=new byte[lengthData+lengString];
		for(int i=0;i<lengthData;i++)
			temp[i]=data[i];
		for(int i=0;i<lengString;i++)
			temp[lengthData+i]=arrayOfString[i];
		data=temp;
	    return j + 2;
	}

	public byte[] getBytesArray() {return data;}
}
