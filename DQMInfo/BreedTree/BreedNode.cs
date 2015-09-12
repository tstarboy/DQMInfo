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
			if(!BreedList.AsQueryable().Where(x => x.Result.Name.ToUpper() == startMonster.Name.ToUpper()).Any())
			{
				thisBreed = startMonster;
				this.Parent1 = null;
				this.Parent2 = null;
				this.Cost = 10;
			}
			else
			{
				BreedNode result = BreedNodeInner(startMonster, BreedList, new List<Monster>(), new List<BreedNode>());

				this.thisBreed = result.thisBreed;
				this.Parent1 = result.Parent1;
				this.Parent2 = result.Parent2;
				this.Cost = result.Cost;
			}
		}

		public BreedNode(IBreedable inThisBreed, BreedNode inParent1, BreedNode inParent2)
		{
			thisBreed = inThisBreed;
			Parent1 = inParent1;
			Parent2 = inParent2;
			Cost = (this.Parent1.Cost == -1 || this.Parent2.Cost == -1) ? -1 : this.Parent1.Cost + this.Parent2.Cost;
		}

		public BreedNode(IBreedable inThisBreed, BreedNode inParent1, BreedNode inParent2, int inCost)
		{
			thisBreed = inThisBreed;
			Parent1 = inParent1;
			Parent2 = inParent2;
			Cost = inCost;
		}

		private static BreedNode BreedNodeInner(IBreedable inputResult, List<Breed> BreedList, List<Monster> UsedMonsters, List<BreedNode> PartialPaths)
		{
			if(PartialPaths.AsQueryable().Where(x => x.thisBreed == inputResult).Any()) //Check if the current result has already been calculated, and return the already calculated optimal path if it has
				return PartialPaths.AsQueryable().Where(x => x.thisBreed == inputResult && x.Cost > 0).OrderBy(x => x.Cost).First();

			if(inputResult.isFinal) //If the current result is a "Family" type, make this node a leaf with a cost of 1 (10 if Family is Boss) and return it.
				return new BreedNode(inputResult, null, null, inputResult.BreedName == "Any Boss" ? 10 : 1);
				
			List<Breed> PotentialBreeds = BreedList.AsQueryable().Where(x => x.Result == inputResult).ToList(); //Get the list of possible breed combinations that result in the requested monster

			if(PotentialBreeds.Count == 0) //If this monster has no breeds (e.g. Slime, BigRoost, CopyCat, Darck), make this node a leaf with a cost of 10 and return it.
				return new BreedNode(inputResult, null, null, 10);


			PotentialBreeds.RemoveAll(x => UsedMonsters.Take(UsedMonsters.Count - 1).Contains(x.Parent1) || UsedMonsters.Take(UsedMonsters.Count - 1).Contains(x.Parent2)); 
			//Remove all breeding combinations that require a monster already used higher up in the tree (to avoid circular dependencies)
				
			if(PotentialBreeds.Count == 0) //If the above removal fails, then this breed choice should not be used. Return a leaf with a cost of -1 (signifies do not use).
				return new BreedNode(inputResult, null, null, -1);
				
			UsedMonsters.Add((Monster)inputResult); //Add current monster to the Used Monsters list.
			if(PotentialBreeds.Count == 1) //If there is only one possible breed combination for this monster...
			{
				BreedNode thisBreedNode = new BreedNode //Calculate the optimal breed tree for each parent.
					(
						inputResult,
						BreedNodeInner(PotentialBreeds[0].Parent1, BreedList, UsedMonsters, PartialPaths),
						BreedNodeInner(PotentialBreeds[0].Parent2, BreedList, UsedMonsters, PartialPaths)
				    );

				if(PotentialBreeds[0].RequiredDepth.HasValue)
				{
					thisBreedNode.Cost = Math.Max(thisBreedNode.Cost, PotentialBreeds[0].RequiredDepth.Value);
				}
						
				UsedMonsters.Remove((Monster)inputResult); //Remove the current monster from the used monsters list

				if(thisBreedNode.Cost > 0) PartialPaths.Add(thisBreedNode); //Add the calculated optimal path for this monster to the partial paths list if it has a cost > 0

				return thisBreedNode; //Return the node
			}
				
			List<BreedNode> PotentialReturn = new List<BreedNode>();
			foreach(Breed thisBreed in PotentialBreeds) //For each potential breed resulting in the requested result, generate a breed node (using the same logic as above). Skip any nodes with a cost of -1.
			{
				BreedNode tempAdd = new BreedNode
					(
						inputResult,
						BreedNodeInner(thisBreed.Parent1, BreedList, UsedMonsters, PartialPaths),
						BreedNodeInner(thisBreed.Parent2, BreedList, UsedMonsters, PartialPaths)
					);

				if(thisBreed.RequiredDepth.HasValue)
				{
					tempAdd.Cost = Math.Max(tempAdd.Cost, thisBreed.RequiredDepth.Value);
				}

				if(tempAdd.Cost > 0) PotentialReturn.Add(tempAdd);
			}
				
			if(PotentialReturn.Count == 0) //If there are no nodes created above (all have a cost of -1), then return this node as a leaf with a cost of -1.
				return new BreedNode(inputResult, null, null, -1);
				
			BreedNode result = PotentialReturn.OrderBy(x => x.Cost).First(); //Sort the list of nodes created above by cost and take the node with the lowest cost.
			UsedMonsters.Remove((Monster)inputResult); //Remove the current monster from the used monsters list
			if(result.Cost > 0) PartialPaths.Add(result); //Add the calculated optimal path for this monster to the partial paths list if it has a cost > 0
			return result; //Return the node

		}

		public String[] OutputTree()
		{
			List<String> OutputList = new List<String>();

			OutputList.Add(String.Format("{0}, Cost: {1}",thisBreed.BreedName,this.Cost));

			if(Parent1 == null && Parent2 == null)
			{
				if (!thisBreed.isFinal)
					OutputList.Add(String.Format("└──Find in wild"));
				return OutputList.ToArray();
			}

			String[] Parent1Output = Parent1.OutputTree();
			OutputList.Add(String.Format("├──{0}", Parent1Output[0]));
			for(int i = 1; i < Parent1Output.Count(); i++)
			{
					OutputList.Add(String.Format("│  {0}", Parent1Output[i]));
			}

			String[] Parent2Output = Parent2.OutputTree();
			OutputList.Add(String.Format("└──{0}", Parent2Output[0]));
			for(int i = 1; i < Parent2Output.Count(); i++)
			{
				OutputList.Add(String.Format("   {0}", Parent2Output[i]));
			}

			return OutputList.ToArray();
		}
	}
}

