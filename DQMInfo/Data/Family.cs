using System;
using System.Collections.Generic;

namespace DQMInfo.Data
{
	public class Family : IData, IBreedable
	{
		public String Name;

		public String SearchName { get { return this.Name; }}

		public String BreedName { get{ return String.Format("Any {0}", this.Name); } }

		public bool isFinal { get { return true; } }

		public Family()
		{
		}

		public Family
			(
				String inName
			)
		{
			Name = inName;
		}

		public static List<Family> BuildFamilyList(List<String[]> ParsedCSV)
		{
			List<Family> ret = new List<Family>();

			foreach(String[] thisFamily in ParsedCSV)
			{
				if (thisFamily.Length != 1)
					throw new Exception();
				ret.Add
				(
					new Family(
						thisFamily[0]
					)
				);
			}

			return ret;
		}

		public List<String> OutputSingle()
		{
			List<String> ret = new List<String>();
			ret.Add(String.Format("Family: {0}", this.Name));
			return ret;
		}

		public String OutputMultipleHeader()
		{
			return String.Format("{0,8}", "Family");
		}

		public String OutputMultipleLine()
		{
			return String.Format("{0,8}", this.Name);
		}
	}
}

