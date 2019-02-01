using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetToWinTool {
	public long bet;
	public long[] listGets;
	public long[] listPercents;

	public BetToWinTool(long _bet,long[] _listGets) {
		bet=_bet;
		listGets=_listGets;
	}
	
	private sbyte Validate(int numberWins,long[] listGW,long[] listPW,int numberLoses,long[] listGL,long[] listPL) {
		long sumOut=0;
		long sumPercent=0;
		for(int i=0;i<numberWins;i++) {
//			System.out.println(listGW[i]+"	"+listPW[i]);
			sumOut=sumOut+listGW[i]*listPW[i];
			sumPercent=sumPercent+listPW[i];
		}
		for(int i=0;i<numberLoses;i++) {
//			System.out.println(listGL[i]+"	"+listPL[i]);
			sumOut=sumOut+listGL[i]*listPL[i];
			sumPercent=sumPercent+listPL[i];
		}
		long sumIn=bet*sumPercent;
		if(sumOut*100<sumIn*92) 
			return -1;//Cần tăng win lên
		else
			if(sumOut*100<sumIn*98)
				return 0;//Hợp lệ : nằm trong khoảng từ 92in --> 98in
			else
				return 1;//Out>In
	}
	
	public void ProcessWeight() {
		int numberCase=listGets.Length;
		if(numberCase<2) {
			Debug.LogError("Lỗi Gets");
			return;
		}
		int countLose=0;
		int countWin=0;
		bool isExitZero=false;
		for (int i = 0; i < numberCase; i++)
			if (listGets[i] == 0)
				isExitZero = true;
			else {
				if (listGets[i] > 0)
					if (listGets[i] < bet)
						countLose++;
					else
						countWin++;
			}
		
		long[] listGW,listPW,listGL,listPL;
		listGW=new long[countWin];
		listGL=new long[countLose];
		countWin=0;
		countLose=0;
		for(int i=0;i<numberCase;i++)
			if(listGets[i]>0)
				if(listGets[i]<bet) {
					listGL[countLose]=listGets[i];
					countLose++;
				}else {
					listGW[countWin]=listGets[i];
					countWin++;
				}
		
		if(isExitZero) {
			if(countWin+countLose+1!=numberCase) {
				Debug.LogError("Sai list gets");
				return;
			}
			if(countWin==0) {
				long __bcnn=listGL[0];
				for(int i=1;i<countLose;i++)
					__bcnn=BSCNN(__bcnn, listGL[i]);
				
				listPercents=new long[countLose+1];
				long ___tempSum=0;
				for(int i=0;i<countLose;i++) {
					listPercents[i+1]=__bcnn/listGL[i];
					___tempSum+=listPercents[i+1];
				}
				listPercents[0]=___tempSum/countLose;
				if(listPercents[0]==0)
					listPercents[0]=1;
				return;
			}
			
			listPW=CreateWeight(listGW);
			long __sumPW=0;
			for(int i=0;i<countWin;i++)
				__sumPW+=listPW[i];
			
			if(countLose==0) {
				listPL=null;
			}else {
				listPL=CreateWeight(listGL);
			}
			
			long[] __listTempGL=new long[countLose+1];
			long[] __listTempPL=new long[countLose+1];
			
			for(int i=0;i<countLose;i++) {
				__listTempGL[i+1]=listGL[i];
				__listTempPL[i+1]=listPL[i];//countLose=0 --> không chạy vào đây
			}
			__listTempGL[0]=0;
			__listTempPL[0]=__sumPW/countWin;
			listGL=__listTempGL;
			listPL=__listTempPL;
			countLose++;
		}else {
			if(countLose==0 || countWin+countLose!=numberCase) {
				Debug.LogError("Sai list gets");
				return;
			}
			if(countWin==0) {
				long __bcnn=listGets[0];
				for(int i=1;i<numberCase;i++)
					__bcnn=BSCNN(__bcnn, listGets[i]);
				
				listPercents=new long[numberCase];
				for(int i=0;i<numberCase;i++)
					listPercents[i]=__bcnn/listGets[i];
				return;
			}
			listPW=CreateWeight(listGW);
			listPL=CreateWeight(listGL);
		}
		////////////////////////////////////////////////////////////////////Giai đoạn cân bằng chỉ số
		sbyte caseCheck = Validate(countWin, listGW, listPW, countLose, listGL, listPL);
		float k=0;
		float delta=1;
		long timeStop= MyConstant.currentTimeMilliseconds+100;
		if(caseCheck<0) {//Lose ca0 --> cần tăng win lên
			Debug.LogError("Lose cao --> cần tăng win lên");
			long[] _testPWin=new long[countWin];
			while(MyConstant.currentTimeMilliseconds<timeStop) {
				for(int i=0;i<countWin;i++)
					_testPWin[i]=(long) (listPW[i]*(k+delta));
				switch (Validate(countWin, listGW, _testPWin, countLose, listGL, listPL)) {
					case 1://Lose cao --> cần tăng win lên
						delta=delta/2;
						break;
					case -1://Win cao --> cần tăng lose lên
						k+=delta;
						delta+=delta;
						break;
					default:CopyToListPercent(countWin, _testPWin, countLose, listPL);return;
				}
			}
			Debug.LogError("Trường hợp này xử lý lỗi");
		}else if(caseCheck==0) {
			CopyToListPercent(countWin, listPW, countLose, listPL);
		}else {//Win cao --> cần tăng lose lên
			Debug.LogError("Win cao --> cần tăng lose lên");
			long[] _testPLose=new long[countLose];
			while(MyConstant.currentTimeMilliseconds<timeStop) {
				for(int i=0;i<countLose;i++)
					_testPLose[i]=(long) (listPL[i]*(k+delta));
				switch (Validate(countWin, listGW, listPW, countLose, listGL, _testPLose)) {
					case 1://Lose cao --> cần tăng win lên
						k+=delta;
						delta+=delta;
						break;
					case -1://Win cao --> cần tăng lose lên
						delta=delta/2;
						break;
					default:CopyToListPercent(countWin, listPW, countLose, _testPLose);return;
				}
			}
			Debug.LogError("Trường hợp này xử lý lỗi");
		}
	}
	
	public void TraceMini() {
		int numberCase=listGets.Length;
		long sumOut=0;
		long countPercent=0;
		for(int i=0;i<numberCase;i++) {
			countPercent+=listPercents[i];
			sumOut=sumOut+listGets[i]*listPercents[i];
		}
		Debug.LogError("SumIn : "+bet*countPercent+"	sumOut : "+sumOut+"===>"+((double)sumOut/(bet*countPercent)));
	}

	public void Trace() {
		int numberCase=listGets.Length;
		long sumOut=0;
		long countPercent=0;
		for(int i=0;i<numberCase;i++) {
			Debug.Log(listGets[i]+"	"+listPercents[i]);
			countPercent+=listPercents[i];
			sumOut=sumOut+listGets[i]*listPercents[i];
		}
		Debug.Log("SumIn : "+bet*countPercent);
		Debug.Log("sumOut : "+sumOut);
		Debug.Log("===>"+((double)sumOut/(bet*countPercent)));
	}

	private void CopyToListPercent(int numberWins,long[] listPW,int numberLoses,long[] listPL) {
		listPercents=new long[numberWins+numberLoses];
		int numberCase=listGets.Length;
		int countLose=0;
		int countWin=0;
		int countPercent=0;
		for(int i=0;i<numberCase;i++)
			if(listGets[i]==0) {
				listPercents[0]=listPL[0];
				countPercent++;
			}
		
		for(int i=0;i<numberCase;i++)
			if(listGets[i]>0)
				if(listGets[i]<bet) {
					listPercents[countPercent]=listPL[countLose];
					countLose++;
					countPercent++;
				}else {
					listPercents[countPercent]=listPW[countWin];
					countWin++;
					countPercent++;
				}
	}
	
	private long[] CreateWeight(long[] listValue) {
		int n=listValue.Length;
		if(n==0) return null;
		long bcnn=listValue[0];
		for(int i=1;i<n;i++)
			bcnn=BSCNN(bcnn, listValue[i]);
		
		long[] listResult=new long[n];
		listResult[0]=1000;
		for(int i=0;i<n;i++)
			listResult[i]=bcnn/listValue[i];
		return listResult;
	}
	private long BSCNN(long a, long b) {
		long multi = a*b;
	    if (a == 0 || b == 0)
	        return multi/(a + b);
	    while (a != b)
	        if (a > b)
	            a -= b; // a = a - b
	        else
	            b -= a;
	    return multi/a; // return a or b, bởi vì lúc này a và b bằng nhau
	}

	public static double[] CreateRatioWin(long[] listScore) { // chỉ xài riêng cho video poker
		int numberCase=listScore.Length;
		
		long maxValue=listScore[0];
		long sumValue=listScore[0];
		for(int i=1;i<numberCase;i++) {
			sumValue+=listScore[i];
			if(maxValue<listScore[i])
				maxValue=listScore[i];
		}
		
		double[] listRatioWin=new double[numberCase];
		for(int i=0;i<numberCase;i++)
			if(listScore[i]==maxValue)
				listRatioWin[i]=0;
			else
				listRatioWin[i]=(double)sumValue/listScore[i];
		return listRatioWin;
	}
}
