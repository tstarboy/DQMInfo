using System;
using System.Collections.Generic;
using System.Linq;

namespace DQMInfo.Data
{
	public class Breed
	{
		public Monster Result;

		public IBreedable Parent1;
		public IBreedable Parent2;

		public int? RequiredDepth;

		public Breed()
		{
		}

		public Breed
		(
			Monster inResult,
			IBreedable inParent1,
			IBreedable inParent2,
			int? inRequiredDepth
		)
		{
			this.Result = inResult;
			this.Parent1 = inParent1;
			this.Parent2 = inParent2;
			this.RequiredDepth = inRequiredDepth;
		}

		public static List<Breed> BuildBreedList(List<String[]> ParsedCSV, List<Monster> MonsterList, List<Family> FamilyList)
		{
			List<Breed> ret = new List<Breed>();

			foreach(String[] thisBreed in ParsedCSV)
			{
				if (thisBreed.Length < 3 || thisBreed.Length > 4)
					throw new Exception();

				//check if there is a required depth (+x) for this breed to succeed
				int? thisRequiredDepth = null;
				if (thisBreed.Length == 4)
					thisRequiredDepth = Int32.Parse(thisBreed[3]);

				IBreedable thisParent1;
				//create first parent (either family or monster)
				if(thisBreed[1].StartsWith("*") && thisBreed[1].EndsWith("*"))
				{
					//this parent is any member of the specified family
					thisParent1 = FamilyList.AsQueryable().Where(x => x.Name.ToUpper() == thisBreed[1].TrimStart('*').TrimEnd('*')).First();
				}
				else
				{
					thisParent1 = MonsterList.AsQueryable().Where(x => x.Name.ToUpper() == thisBreed[1].ToUpper()).First();
				}

				IBreedable thisParent2;
				//create second parent (either family or monster)
				if(thisBreed[2].StartsWith("*") && thisBreed[2].EndsWith("*"))
				{
					//this parent is any member of the specified family
					thisParent2 = FamilyList.AsQueryable().Where(x => x.Name.ToUpper() == thisBreed[2].TrimStart('*').TrimEnd('*')).First();
				}
				else
				{
					thisParent2 = MonsterList.AsQueryable().Where(x => x.Name.ToUpper() == thisBreed[2].ToUpper()).First();
				}


				ret.Add
				(
					new Breed
					(
						MonsterList.AsQueryable().Where(x => x.Name.ToUpper() == thisBreed[0]).First(),
						thisParent1,
						thisParent2,
						thisRequiredDepth
					)
				);
			}

			return ret;
		}

		public String Output()
		{
			return String.Format("{0,9} = {1, 12} + {2, 12}", this.Result.Name, Parent1.BreedName, Parent2.BreedName);
		}


	}
}

