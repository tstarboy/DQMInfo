using System;
using System.Collections.Generic;
using System.Linq;

namespace DQMInfo.Data
{
	public interface IBreedable
	{
		string BreedName { get; }

		bool isFinal { get; }
	}

}

