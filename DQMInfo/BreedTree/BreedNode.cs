using System;
using DQMInfo.Data;
using System.Linq;
using System.Collections.Generic;

namespace DQMInfo.BreedTree
{
	
	public class BreedNode
	{
		public IBreedable thisBreed;

		public BreedNode Parent1;
		public BreedNode Parent2;

		int Cost;

		public BreedNode()
		{
			thisBreed = null;
			Parent1 = null;
			Parent2 = null;
			Cost = -1;
		}

		public BreedNode(Monster startMonster, List<Breed> BreedList)
		{
			BreedNode result = BreedNodeInner(startMonster, BreedList, new List<Monster>(), new List<BreedNode>());

			this.thisBreed = result.thisBreed;
			this.Parent1 = result.Parent1;
			this.Parent2 = result.Parent2;
			this.Cost = result.Cost;
		}

		public static BreedNode BreedNodeInner(IBreedable inputResult, List<Breed> BreedList, List<Monster> UsedMonsters, List<BreedNode> PartialPaths)
		{
			if(PartialPaths.AsQueryable().Where(x => x.thisBreed == inputResult).Any())
			{
				return PartialPaths.AsQueryable().Where(x => x.thisBreed == inputResult && x.Cost > 0).OrderBy(x => x.Cost).First();
			}

			BreedNode thisBreedNode = new BreedNode();
			thisBreedNode.thisBreed = inputResult;

			if(inputResult.isFinal || inputResult.GetType().Name == "Family")
			{
				thisBreedNode.Parent1 = null;
				thisBreedNode.Parent2 = null;
				thisBreedNode.Cost = inputResult.BreedName == "Any Boss" ? 10 : 1;
				return thisBreedNode;
			}
			else
			{
				List<Breed> PotentialBreeds = BreedList.AsQueryable().Where(x => x.Result == inputResult).ToList();

				if(PotentialBreeds.Count == 0)
				{
					thisBreedNode.Parent1 = null;
					thisBreedNode.Parent2 = null;
					thisBreedNode.Cost = 10;
					UsedMonsters.Remove((Monster)inputResult);
					return thisBreedNode;
				}

				//check for circular breeding dependency
				PotentialBreeds.RemoveAll(x => UsedMonsters.Take(UsedMonsters.Count - 1).Contains(x.Parent1) || UsedMonsters.Take(UsedMonsters.Count - 1).Contains(x.Parent2));
				if(PotentialBreeds.Count == 0)
				{
					thisBreedNode.Parent1 = null;
					thisBreedNode.Parent2 = null;

					//abandon ship
					thisBreedNode.Cost = -1;
					UsedMonsters.Remove((Monster)inputResult);
					return thisBreedNode;
				}

				//Add current monster to Used Monsters list.
				UsedMonsters.Add((Monster)inputResult);
				
				if(PotentialBreeds.Count == 1)
				{
					thisBreedNode.Parent1 = BreedNodeInner(PotentialBreeds[0].Parent1, BreedList, UsedMonsters, PartialPaths);
					thisBreedNode.Parent2 = BreedNodeInner(PotentialBreeds[0].Parent2, BreedList, UsedMonsters, PartialPaths);
					if (thisBreedNode.Parent1.Cost == -1 || thisBreedNode.Parent2.Cost == -1)
						thisBreedNode.Cost = -1;
					else
						thisBreedNode.Cost = thisBreedNode.Parent1.Cost + thisBreedNode.Parent2.Cost;

					UsedMonsters.Remove((Monster)inputResult);
					return thisBreedNode;
				}

				else
				{
					List<BreedNode> PotentialReturn = new List<BreedNode>();


					foreach(Breed thisBreed in PotentialBreeds)
					{
						BreedNode tempAdd = new BreedNode();

						tempAdd.thisBreed = inputResult;
						tempAdd.Parent1 = BreedNodeInner(thisBreed.Parent1, BreedList, UsedMonsters, PartialPaths);
						tempAdd.Parent2 = BreedNodeInner(thisBreed.Parent2, BreedList, UsedMonsters, PartialPaths);
						if (tempAdd.Parent1.Cost == -1 || tempAdd.Parent2.Cost == -1)
							tempAdd.Cost = -1;
						else
							tempAdd.Cost = tempAdd.Parent1.Cost + tempAdd.Parent2.Cost;

						if(tempAdd.Cost > 0) PotentialReturn.Add(tempAdd);
					}

					if(PotentialReturn.Count == 0)
					{
						thisBreedNode.Parent1 = null;
						thisBreedNode.Parent2 = null;

						//abandon ship
						thisBreedNode.Cost = -1;
						UsedMonsters.Remove((Monster)inputResult);
						return thisBreedNode;
					}

					thisBreedNode = PotentialReturn.OrderBy(x => x.Cost).First();

					foreach(BreedNode RemoveFromUsedList in PotentialReturn)
					{
						if(RemoveFromUsedList.thisBreed.GetType().Name == "Monster")
						{
							UsedMonsters.Remove((Monster)RemoveFromUsedList.thisBreed);
						}
					}

					UsedMonsters.Remove((Monster)inputResult);

					if(thisBreedNode.Cost > 0) PartialPaths.Add(thisBreedNode);
					return thisBreedNode;
				}
			}
		}

		public String[] OutputTree()
		{
			List<String> OutputList = new List<String>();

			OutputList.Add(String.Format("{0}, Cost: {1}",thisBreed.BreedName,this.Cost));

			if(Parent1 == null && Parent2 == null)
			{
				if (!thisBreed.isFinal)
					OutputList.Add(String.Format("└Find in wild"));
				return OutputList.ToArray();
			}

			String[] Parent1Output = Parent1.OutputTree();
			OutputList.Add(String.Format("├{0}", Parent1Output[0]));
			for(int i = 1; i < Parent1Output.Count(); i++)
			{
					OutputList.Add(String.Format("│{0}", Parent1Output[i]));
			}

			String[] Parent2Output = Parent2.OutputTree();
			OutputList.Add(String.Format("└{0}", Parent2Output[0]));
			for(int i = 1; i < Parent2Output.Count(); i++)
			{
				OutputList.Add(String.Format(" {0}", Parent2Output[i]));
			}

			return OutputList.ToArray();
		}

		public List<IBreedable> ReferencesSuck(List<IBreedable> input)
		{
			List<IBreedable> ret = new List<IBreedable>();
			foreach(IBreedable thisBreed in input)
			{
				ret.Add(thisBreed);
			}

			return ret;
		}
	}
}

