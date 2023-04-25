using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaskeAutomaten
{
	internal abstract class Drink
	{
		private string name;

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public Drink(string name)
		{
			this.name = name;
		}
	}
}
