using System;
using System.Collections.Generic;
using System.Linq;

namespace DQMInfo.Data
{
	public class Key : IData 
	{
		public String Name;

		public String SearchName { get { return this.Name; } }

		public int Rarity;
		public List<Family> MonsterTypes;

		public Key()
		{
		}

		public Key
			(
				String inName,
				int inRarity,
				Family inMonsterType1,
				Family inMonsterType2
			)
		{
			this.Name = inName;
			this.Rarity = inRarity;
			this.MonsterTypes = new List<Family>();
			this.MonsterTypes.Add(inMonsterType1);
			if (inMonsterType2 != null)
				this.MonsterTypes.Add(inMonsterType2);
		}

		public static List<Key> BuildKeyList(List<String[]> ParsedPrefix, List<String[]> ParsedSuffix, List<Family> FamilyList)
		{
			List<Key> ret = new List<Key>();

			foreach(String[] thisKeyPrefix in ParsedPrefix)
			{
				if (thisKeyPrefix.Length != 2)
					throw new Exception();
				foreach(String[] thisKeySuffix in ParsedSuffix)
				{
					if (thisKeySuffix.Length < 2 || thisKeySuffix.Length > 3)
						throw new Exception();

					Family Family2 = null;
					if (thisKeySuffix.Length == 3)
						Family2 = FamilyList.AsQueryable().Where(x => x.Name == thisKeySuffix[2]).First();

					ret.Add
					(
						new Key
						(
							String.Format("{0}{1}", thisKeyPrefix[0], thisKeySuffix[0]),
							Int32.Parse(thisKeyPrefix[1]),
							FamilyList.AsQueryable().Where(x => x.Name == thisKeySuffix[1]).First(),
							Family2
						)
					);
				}
			}

			return ret;
		}

		public List<String> OutputSingle()
		{
			List<String> ret = new List<String>();

			ret.Add(String.Format("Key: {0}", this.Name));
			ret.Add(String.Format("Rarity: {0}/16", this.Rarity));
			ret.Add(String.Format("Monster Families Found: "));
			foreach(Family thisMonsterType in MonsterTypes)
			{
				ret.Add(String.Format("\t{0}", thisMonsterType.Name));
			}

			return ret;
		}

		public String OutputMultipleHeader()
		{
			return String.Format("{0,10} {1,6} {2}", "Key", "Rarity", "Families");
		}

		public String OutputMultipleLine()
		{
			return String.Format("{0,10} {1,3}/16 {2}", this.Name, this.Rarity, String.Join(", ", this.MonsterTypes.Select(i => i.Name).ToList()));
		}
	}
}

