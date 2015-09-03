using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace DQMInfo
{
	public static class CSVParser
	{
		public static List<String[]> ParseCSV(string fileName)
		{
			List<String> unsplit = new List<String>();
			List<String[]> ret = new List<String[]>();
			try
			{
				unsplit = new List<String>(System.IO.File.ReadAllLines(fileName));
			}
			catch (Exception ex)
			{
				Console.WriteLine("Unable to read file: {0}", fileName);
				Console.WriteLine(ex.Message);
				throw ex;
			}

			Regex CSVParserRegex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

			foreach(String thisLine in unsplit)
			{
				String[] SplitLine = CSVParserRegex.Split(thisLine);

				for(int i = 0; i < SplitLine.Length; i++)
				{
					SplitLine[i] = SplitLine[i].TrimStart(' ', '"').TrimEnd('"').Trim();
				}
				ret.Add(SplitLine);
			}

			return ret;
		}
	}
}

