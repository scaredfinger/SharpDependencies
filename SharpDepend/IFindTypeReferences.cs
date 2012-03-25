using System.Collections.Generic;
using Mono.Cecil;

namespace SharpDepend
{
	public interface IFindTypeReferences {
		IEnumerable<TypeReference> OfType(TypeDefinition definition);
	}
}