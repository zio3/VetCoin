using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Codes
{

	public static class EnumExtensions
	{
		// enumに適用されている属性を取得
		public static IEnumerable<TAttribute> GetApplied<TAttribute>(this Enum value) where TAttribute : Attribute
		{

			return value
				.GetType()
				.GetField(value.ToString())
				.GetCustomAttributes(false)
				.OfType<TAttribute>();
		}
	}

}
