using System;
using System.Linq;
using System.Reflection;

public static class Utils
{
    public static void CopyPublicFieldsAndProperties(object source, object destination)
    {
        if (source == null || destination == null)
            throw new Exception("source or destination objects is null");
        Type typeDest = destination.GetType();
        Type typeSrc = source.GetType();
        var propertyResults = from sourceProperty in typeSrc.GetProperties()
                              let targetProperty = typeDest.GetProperty(sourceProperty.Name)
                              where sourceProperty.CanRead
                              && targetProperty != null
                              && (targetProperty.GetSetMethod(true) != null && !targetProperty.GetSetMethod(true).IsPrivate)
                              && (targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) == 0
                              && targetProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType)
                              select new { sourceProperty, targetProperty };
        foreach (var propPair in propertyResults)
        {
            propPair.targetProperty.SetValue(destination, propPair.sourceProperty.GetValue(source, null), null);
        }

        var fieldResults = from sourceField in typeSrc.GetFields()
                           let targetField = typeDest.GetField(sourceField.Name)
                           where !sourceField.IsPrivate
                           && targetField != null
                           && (!targetField.IsPrivate)
                           && (targetField.Attributes & FieldAttributes.Static) == 0
                           && targetField.FieldType.IsAssignableFrom(sourceField.FieldType)
                           select new { sourceField, targetField };
        foreach (var fieldPair in fieldResults)
        {
            fieldPair.targetField.SetValue(destination, fieldPair.sourceField.GetValue(source));
        }

    }


}