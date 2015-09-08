using System;
using DQMInfo.Data;
using System.Collections.Generic;

namespace DQMInfo.Output
{
	public static class OutputData<TData> where TData : IData
	{
		public static List<String> OutputSingle(TData output)
		{
			return output.OutputSingle();
		}

		public static List<String> OutputMultiple(List<TData> outputList)
		{
			List<String> ret = new List<String>();
			ret.Add(outputList[0].OutputMultipleHeader());
			foreach(TData thisDataLine in outputList)
			{
				ret.Add(thisDataLine.OutputMultipleLine());
			}

			return ret;
		}
	}
}

