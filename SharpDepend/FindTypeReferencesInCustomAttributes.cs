using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace SharpDepend
{
	public class FindTypeReferencesInCustomAttributes : IFindTypeReferencesInCustomAttributes
	{
		public IEnumerable<TypeReference> OfProvider(ICustomAttributeProvider provider)
		{
			if (provider == null)
				throw new ArgumentNullException("provider");

			return provider.CustomAttributes.Select(x => x.AttributeType);
		}
	}
}