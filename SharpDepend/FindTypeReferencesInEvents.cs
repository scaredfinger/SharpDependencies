using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace SharpDepend
{
	public class FindTypeReferencesInEvents : IFindTypeReferencesInMember
	{
		public IEnumerable<TypeReference> OfType(TypeDefinition definition)
		{
			if (definition == null)
				throw new ArgumentNullException("definition");

			return new TypeReference[0];
		}
	}
}