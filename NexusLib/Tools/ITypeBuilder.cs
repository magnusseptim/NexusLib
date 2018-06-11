using NexusLib.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusLib.Tools
{
    public interface ITypeBuilder
    {
        Type BuildType(string typeName, IEnumerable<PropertyModel> propertyModel);
        bool Equals(object obj);
        IList<Type> GetCreatedTypes();
        int GetHashCode();
    }
}
