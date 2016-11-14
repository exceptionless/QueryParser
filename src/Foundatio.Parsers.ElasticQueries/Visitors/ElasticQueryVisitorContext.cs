﻿using System;
using Foundatio.Parsers.LuceneQueries.Visitors;
using Nest;
using System.Linq;

namespace Foundatio.Parsers.ElasticQueries.Visitors {
    public class ElasticQueryVisitorContext : QueryVisitorContextWithAliasResolver, IElasticQueryVisitorContext {
        public Operator DefaultOperator { get; set; } = Operator.And;
        public bool UseScoring { get; set; }
        public string DefaultField { get; set; }
        public Func<string, IProperty> GetPropertyMappingFunc { get; set; }
    }

    public static class ElasticQueryVisitorContextExtensions {
        public static IProperty GetPropertyMapping(this IElasticQueryVisitorContext context, string field) {
            return context.GetPropertyMappingFunc?.Invoke(field);
        }

        public static string GetNonAnalyzedFieldName(this IElasticQueryVisitorContext context, string field) {
            if (String.IsNullOrEmpty(field))
                return field;

            var property = context.GetPropertyMapping(field);
            if (property == null || !context.IsPropertyAnalyzed(property))
                return field;

            var multiFieldProperty = property as ICoreProperty;
            var nonAnalyzedProperty = multiFieldProperty.Fields.FirstOrDefault(kvp => {
                if (kvp.Value is IKeywordProperty)
                    return true;

                if (!context.IsPropertyAnalyzed(kvp.Value))
                    return true;

                return false;
            });

            if (nonAnalyzedProperty.Value != null)
                return field + "." + nonAnalyzedProperty.Key.Name;

            return field;
        }

        public static bool IsPropertyAnalyzed(this IElasticQueryVisitorContext context, string field) {
            if (String.IsNullOrEmpty(field))
                return true;

            var property = context.GetPropertyMapping(field);
            if (property == null)
                return false;

            return context.IsPropertyAnalyzed(property);
        }

        public static bool IsPropertyAnalyzed(this IElasticQueryVisitorContext context, IProperty property) {
            var textProperty = property as TextProperty;
            if (textProperty != null)
                return !textProperty.Index.HasValue || textProperty.Index.Value;

#pragma warning disable 618
            var stringMapping = property as StringProperty;
            if (stringMapping != null)
                return stringMapping.Index == FieldIndexOption.Analyzed || stringMapping.Index == null;
#pragma warning restore 618

            return false;
        }

        public static bool IsNestedPropertyType(this IElasticQueryVisitorContext context, string field) {
            if (String.IsNullOrEmpty(field))
                return false;

            var mapping = context.GetPropertyMapping(field) as NestedProperty;
            return mapping != null;
        }

        public static bool IsGeoPropertyType(this IElasticQueryVisitorContext context, string field) {
            if (String.IsNullOrEmpty(field))
                return false;

            var mapping = context.GetPropertyMapping(field) as GeoPointProperty;
            return mapping != null;
        }

        public static bool IsNumericPropertyType(this IElasticQueryVisitorContext context, string field) {
            if (String.IsNullOrEmpty(field))
                return false;

            var mapping = context.GetPropertyMapping(field) as NumberProperty;
            return mapping != null;
        }
    }
}
