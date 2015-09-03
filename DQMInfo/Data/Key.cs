using System;
using System.Collections.Generic;
using System.Linq;

namespace DQMInfo.Data
{
	public class Key : IOutputable 
	{
		public String Name;
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

		public void Output()
		{
			System.Console.WriteLine("Key: {0}", this.Name);
			System.Console.WriteLine("Rarity: {0}/16", this.Rarity);
			System.Console.WriteLine("Monster Families Found: ");
			foreach(Family thisMonsterType in MonsterTypes)
			{
				System.Console.WriteLine("\t{0}", thisMonsterType.Name);
			}
			System.Console.ReadKey();
		}

		public static List<String> OutputMultiple(List<Key> outputList)
		{
			List<String> ret = new List<String>();
			ret.Add
			(
				String.Format
				(
					"{0,10} {1,6} {2}",
					"Key",
					"Rarity",
					"Families"
				)
			);

			foreach(var thisOutput in outputList)
			{
				ret.Add
				(
					String.Format
					(
						"{0,10} {1,3}/16 {2}",
						thisOutput.Name,
						thisOutput.Rarity,
						String.Join(", ", thisOutput.MonsterTypes.Select(i => i.Name).ToList())
					)
				);
			}

			return ret;
		}
	}
}

