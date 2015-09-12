using System;
using System.Collections.Generic;
using DQMInfo.Data;
using System.Linq;
using DQMInfo.BreedTree;
using DQMInfo.Output;
using System.Linq.Expressions;

namespace DQMInfo
{
	public class Processor
	{
		private List<Skill> SkillList;
		private List<Family> FamilyList;
		private List<Monster> MonsterList;
		private List<Breed> BreedList;
		private List<Key> KeyList;

		public Processor()
		{
			SkillList = Skill.BuildSkillList(CSVParser.ParseCSV(@"CSV/skills.csv"));

			FamilyList = Family.BuildFamilyList(CSVParser.ParseCSV(@"CSV/families.csv"));

			MonsterList = Monster.BuildMonsterList(CSVParser.ParseCSV(@"CSV/monsters.csv"), SkillList, FamilyList);
			SkillList = Skill.AddMonsterList(SkillList, MonsterList);

			BreedList = Breed.BuildBreedList(CSVParser.ParseCSV(@"CSV/breeding.csv"), MonsterList, FamilyList);
			BreedList.RemoveAll(x => x.Result.Name == "WonderEgg" || x.Parent1.BreedName == "WonderEgg" || x.Parent2.BreedName == "WonderEgg");
			MonsterList = Monster.AddBreedList(MonsterList, BreedList);

			KeyList = Key.BuildKeyList(CSVParser.ParseCSV(@"CSV/keyprefix.csv"), CSVParser.ParseCSV(@"Information/keysuffix.csv"), FamilyList);
		}

		public void MainMenu()
		{
			System.Console.WriteLine();
			bool Completed = false;
			while(!Completed)
			{
				System.Console.WriteLine("Main Menu:");
				System.Console.WriteLine(" 1. Search for Monster by Name");
				System.Console.WriteLine(" 2. Search for Monster by Family");
				System.Console.WriteLine(" 2. Search for Skill by Name");
				System.Console.WriteLine(" 3. Search for Key by Name");
				System.Console.WriteLine(" 0. Exit");
				System.Console.WriteLine("Your Choice (Enter Number): ");
				
				string choice = System.Console.ReadLine();
				
				switch(choice)
				{
					case "1":
						Monster thisMonster = SearchMenu<Monster>(MonsterList);

						if(thisMonster != default(Monster))
						{
							BreedNode thisTree = new BreedNode(thisMonster, BreedList);
							foreach(String thisLine in thisTree.OutputTree())
							{
								System.Console.WriteLine(thisLine);
							}
						}
						break;
					case "2":
						Monster thisMonsterFamily = SearchMenu<Monster>(MonsterList, ( query => ( x => x.MemberOf.Name.ToUpper().Contains(query.ToUpper()) ) ), "Family");
						break;
					case "3":
						SearchMenu<Skill>(SkillList);
						break;
					case "4":
						SearchMenu<Key>(KeyList);
						break;
					case "0":
						Completed = true;
						break;
					default:
						System.Console.WriteLine("Invalid choice. ");
						break;
				}
			}
		}

		public TData SearchMenu<TData>(List<TData> TList, Func<String, Expression<Func<TData, bool> > > Comparer, String searchName) where TData : IData
		{
			System.Console.WriteLine();
			System.Console.WriteLine("Type the name of the {0} to search for: ", searchName);
			String Query = System.Console.ReadLine().Trim();
			IEnumerable<TData> BaseQuery = TList.AsQueryable().Where(Comparer(Query));
			if(!BaseQuery.Any())
			{
				System.Console.WriteLine("{0} \"{1}\" not found. ", searchName, Query);
				return default(TData);
			}
			else
			{
				if (BaseQuery.ToList().Count > 1)
				{
					System.Console.WriteLine(String.Format("Multiple {0}s found. Select which one you want (by number): ", searchName));

					List<String> OutputList = OutputData<TData>.OutputMultiple(BaseQuery.ToList());

					int padSize = ( OutputList.Count - 1 ).ToString().Length;
					System.Console.WriteLine(OutputList[0].PadLeft(OutputList[0].Length + padSize + 2, ' '));

					for(int i = 1; i < OutputList.Count; i++)
					{
						System.Console.WriteLine("{0}. {1}", i.ToString().PadLeft(padSize, ' '), OutputList[i]);
					}

					System.Console.WriteLine("{0}. Quit", "0".PadLeft(padSize, ' '));

					bool selected = false;

					string choiceS = "";
					int choiceI = -1;

					while(!selected)
					{
						choiceS = System.Console.ReadLine();
						selected = Int32.TryParse(choiceS, out choiceI);

						if (selected)
						{
							if (choiceI == 0)
								return default(TData);


							foreach (String thisLine in OutputData<TData>.OutputSingle(BaseQuery.ToList()[choiceI - 1]))
							{
								System.Console.WriteLine(thisLine);
							}
						}
						else
							System.Console.WriteLine("Invalid choice. ");
					}

					System.Console.WriteLine();
					return BaseQuery.ToList()[choiceI - 1];
				}
				else
				{
					foreach (String thisLine in OutputData<TData>.OutputSingle(BaseQuery.Single()))
					{
						System.Console.WriteLine(thisLine);
					}
					System.Console.WriteLine();
					return BaseQuery.Single();
				}
			}
		}

		public TData SearchMenu<TData>(List<TData> TList) where TData : IData
		{
			return SearchMenu<TData>(TList, ( query => ( x => x.SearchName.ToUpper().Contains(query.ToUpper()) ) ), typeof(TData).Name );
		}
	}
}

