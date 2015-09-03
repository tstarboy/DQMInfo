using System;
using System.Collections.Generic;
using DQMInfo.Data;
using System.Linq;
using DQMInfo.BreedTree;

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
			SkillList = Skill.BuildSkillList(CSVParser.ParseCSV(@"Information/skills.csv"));
			FamilyList = Family.BuildFamilyList(CSVParser.ParseCSV(@"Information/families.csv"));
			MonsterList = Monster.BuildMonsterList(CSVParser.ParseCSV(@"Information/monsters.csv"), SkillList, FamilyList);
			BreedList = Breed.BuildBreedList(CSVParser.ParseCSV(@"Information/breeding.csv"), MonsterList, FamilyList);
			BreedList.RemoveAll(x => x.Result.Name == "WonderEgg" || x.Parent1.BreedName == "WonderEgg" || x.Parent2.BreedName == "WonderEgg");
			KeyList = Key.BuildKeyList(CSVParser.ParseCSV(@"Information/keyprefix.csv"), CSVParser.ParseCSV(@"Information/keysuffix.csv"), FamilyList);
		}

		public void MainMenu()
		{
			System.Console.WriteLine();
			bool Completed = false;
			while(!Completed)
			{
				System.Console.WriteLine("Main Menu:");
				System.Console.WriteLine(" 1. Search for Monster");
				System.Console.WriteLine(" 2. Search for Skill");
				System.Console.WriteLine(" 0. Exit");
				System.Console.WriteLine("Your Choice (Enter Number): ");
				
				string choice = System.Console.ReadLine();
				
				switch(choice)
				{
					case "1":
						MonsterMenu();
						break;
					case "2":
						SkillMenu();
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

		public void MonsterMenu()
		{
			System.Console.WriteLine();
			System.Console.WriteLine("Type the name of the monster to search for: ");
			string MonsterName = System.Console.ReadLine();
			IQueryable<Monster> BaseQuery = MonsterList.AsQueryable().Where(x => x.Name.ToUpper().Contains(MonsterName.ToUpper()));
			if(!BaseQuery.Any())
			{
				System.Console.WriteLine("Monster \"{0}\" not found. ", MonsterName);
				return;
			}
			else
			{
				Monster thisMonster = BaseQuery.First();
				if(BaseQuery.ToList().Count > 1)
				{
					bool Selected = false;
					while(!Selected)
					{
						System.Console.WriteLine("Multiple Monsters found. Select which one you want (by number): ");
						for(int i = 1; i <= BaseQuery.ToList().Count; i++)
						{
							System.Console.WriteLine(" {0}. {1}", i, BaseQuery.ToList()[i - 1].Name);
						}
						System.Console.WriteLine(" 0. Go Back");
						string choice = System.Console.ReadLine();

						if (choice == "0")
							return;

						int result = new int();

						if (!Int32.TryParse(choice, out result) || result < 1 || result > BaseQuery.ToList().Count)
							System.Console.WriteLine("Invalid choice. ");
						else
						{
							Selected = true;
							thisMonster = BaseQuery.ToList()[result - 1];
						}
					}
				}

				thisMonster.Output();
				System.Console.WriteLine("Breeding Information: ");
				List<Breed> BreedingInformation = BreedList.AsQueryable().Where(x => x.Result == thisMonster).ToList();
				if (BreedingInformation.Count == 0)
					System.Console.WriteLine("\tNo Breed combinations found for {0}.", thisMonster.Name);
				else
				{
					foreach(Breed thisBreed in BreedingInformation)
					{
						System.Console.WriteLine("\t{0}", thisBreed.Output());
					}
				}

				System.Console.WriteLine("Process Breed Tree? Type Yes to continue, anything else quits.");
				string choice2 = System.Console.ReadLine();
				switch(choice2)
				{
					case "Yes":
						BreedNode thisTree = new BreedNode(thisMonster, BreedList);
						System.Console.WriteLine();
						foreach(String thisLine in thisTree.OutputTree())
						{
							System.Console.WriteLine(thisLine);
						}
						System.Console.WriteLine();
						break;
					default:
						System.Console.WriteLine();
						break;
				}
			}
		}

		public void SkillMenu()
		{
			System.Console.WriteLine();
			System.Console.WriteLine("Type the name of the skill to search for: ");
			string SkillName = System.Console.ReadLine();
			IQueryable<Skill> BaseQuery = SkillList.AsQueryable().Where(x => x.Name.ToUpper().Contains(SkillName.ToUpper()));
			if(!BaseQuery.Any())
			{
				System.Console.WriteLine("Skill \"{0}\" not found. ", SkillName);
				return;
			}
			else
			{
				if (BaseQuery.ToList().Count > 1)
				{
					foreach (String thisLine in Skill.OutputMultiple(BaseQuery.ToList()))
					{
						System.Console.WriteLine(thisLine);
					}
				}
				else
					BaseQuery.Single().Output();
			}

			System.Console.WriteLine();
		}
	}
}

