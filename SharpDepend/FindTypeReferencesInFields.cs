using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace SharpDepend
{
	public class FindTypeReferencesInFields : IFindTypeReferencesInFields
	{
		public IEnumerable<TypeReference> OfType(TypeDefinition definition)
		{
			if (definition == null)
				throw new ArgumentNullException("definition");

			return definition.Fields.Select(x => x.FieldType);
		}
	}
}