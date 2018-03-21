using System;
using System.Collections.Generic;
using NexusLib.Model;

namespace NexusLib.Tools
{
    public interface ITypeBuilders
    {
        Type BuildType(string typeName, IEnumerable<PropertyModel> propertyModel);
        bool Equals(object obj);
        IList<Type> GetCreatedTypes();
        int GetHashCode();
    }
}