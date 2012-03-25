using System.Collections.Generic;
using Mono.Cecil;

namespace SharpDepend
{
	public interface IFindTypeReferencesInCustomAttributes
	{
		IEnumerable<TypeReference> OfProvider(ICustomAttributeProvider provider);
	}
}