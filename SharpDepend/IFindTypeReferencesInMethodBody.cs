using System.Collections.Generic;
using Mono.Cecil;

namespace SharpDepend
{
	public interface IFindTypeReferencesInMethodBody
	{
		IEnumerable<TypeReference> OfMethod(MethodDefinition body);
	}
}