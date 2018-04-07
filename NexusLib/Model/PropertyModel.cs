using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NexusLib.Model
{
    public enum UsedAccesModificator
    {
        Property,
        Field
    }
    public class PropertyModel
    {
        public string Name { get; set; }
        public Type PropertyType { get; set; }
        public PropertyAttributes Attribute { get; set; }
        public FieldAttributes UnderlyingFieldAttribute { get; set; }

        public PropertyModel(string name, Type propertyValue, PropertyAttributes attribute, FieldAttributes underlyingFieldAttribute )
        {
            this.Name = name;
            this.PropertyType = propertyValue;
            this.Attribute = attribute;
            this.UnderlyingFieldAttribute = underlyingFieldAttribute;
        }
    }
}
