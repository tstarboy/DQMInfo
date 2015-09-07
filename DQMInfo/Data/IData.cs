using System;
using System.Collections.Generic;
using System.Linq;

namespace DQMInfo.Data
{
	public interface IData
	{
		String SearchName { get; }

		List<String> OutputSingle();

		String OutputMultipleHeader();

		String OutputMultipleLine();
	}
}