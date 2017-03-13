using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper
{
    public class DocumentMapper
    {
        static readonly Lazy<DocumentMapper> _default = new Lazy<DocumentMapper>(() => new DocumentMapperBuilder().WithDefaults().Create(), true);
        public static DocumentMapper Default => _default.Value;
        readonly Dictionary<Type, AttributeMapping> _attributeMappings;
        readonly ConcurrentDictionary<Type, Lazy<DocumentMapping>> _mappings = new ConcurrentDictionary<Type, Lazy<DocumentMapping>>();

        internal DocumentMapper(Dictionary<Type, AttributeMapping> AttributeMappings)
        {
            _attributeMappings = new Dictionary<Type, AttributeMapping>(AttributeMappings);
        }

        public Dictionary<string, AttributeValue> ToAttributes(object document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            return GetMapping(document.GetType()).ToAttributesFunc(document);
        }

        public T ToDocument<T>(Dictionary<string, AttributeValue> attributes)
        {
            if (attributes == null) throw new ArgumentNullException(nameof(attributes));
            return (T)GetMapping(typeof(T)).ToDocumentFunc(attributes);
        }

        private DocumentMapping GetMapping(Type type)
        {
            Lazy<DocumentMapping> mapping;
            if (!_mappings.TryGetValue(type, out mapping))
            {
                mapping = _mappings.GetOrAdd(type, new Lazy<DocumentMapping>(() => CreateMapping(type), LazyThreadSafetyMode.ExecutionAndPublication));
            }
            return mapping.Value;
        }

        private DocumentMapping CreateMapping(Type type)
        {
            return new DocumentMapping
            {
                ToAttributesFunc = CreateToAttributesFunc(type),
                ToDocumentFunc = CreateToDocumentFunc(type),
            };
        }

        private Func<object, Dictionary<string, AttributeValue>> CreateToAttributesFunc(Type type)
        {
            return o => { throw new NotImplementedException(); };
        }

        private Func<Dictionary<string, AttributeValue>, object> CreateToDocumentFunc(Type type)
        {
            var mapperExceptionCtor = typeof(DynamoDBMapperException).GetConstructor(new[] { typeof(string) });
            var attributes = Expression.Parameter(typeof(Dictionary<string, AttributeValue>), "attributes");
            var attributeValue = Expression.Variable(typeof(AttributeValue), "attributeValue");
            var result = Expression.Variable(type, "result");
            var steps = new List<Expression>();
            var tryGetValue = typeof(Dictionary<string, AttributeValue>).GetMethod("TryGetValue", BindingFlags.Instance | BindingFlags.Public);
            Dictionary<Type, ParameterExpression> tempLocals = new Dictionary<Type, ParameterExpression>();
            steps.Add(Expression.Assign(result, Expression.New(type)));
            foreach (var spec in GetAttributeSpecifications(type))
            {
                ParameterExpression temp;
                if (!tempLocals.TryGetValue(spec.Prop.PropertyType, out temp))
                {
                    temp = Expression.Variable(spec.Prop.PropertyType);
                    tempLocals.Add(spec.Prop.PropertyType, temp);
                }
                var expr = Expression.IfThen(
                    Expression.Call(attributes, tryGetValue, Expression.Constant(spec.AttributeName), attributeValue),
                    Expression.IfThenElse(
                        Expression.Call(spec.Mapping.From, attributeValue, temp),
                        Expression.Assign(
                            Expression.MakeMemberAccess(result, spec.Prop),
                            temp
                        ),
                        Expression.Throw(
                            Expression.New(mapperExceptionCtor, Expression.Constant(spec.Prop.Name))
                        )
                    )
                );
                steps.Add(expr);
            }
            steps.Add(Expression.Convert(result, typeof(object)));
            return Expression.Lambda<Func<Dictionary<string, AttributeValue>, object>>(
                Expression.Block(
                    typeof(object),
                    new[] { attributeValue, result }.Concat(tempLocals.Values),
                    steps
                ),
                attributes
            ).Compile();
        }

        private IEnumerable<AttributeSpec> GetAttributeSpecifications(Type type) =>
            from prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            let attr = prop.GetCustomAttributes<DynamoDBRenamableAttribute>().FirstOrDefault()
            select new AttributeSpec
            {
                Prop = prop,
                Attribute = attr,
                Mapping = GetAttributeMapping(prop, attr),
            };

        private AttributeMapping GetAttributeMapping(PropertyInfo prop, DynamoDBRenamableAttribute attr)
        {
            AttributeMapping attrMapping;
            if (_attributeMappings.TryGetValue(prop.PropertyType, out attrMapping)) return attrMapping;
            throw new NotSupportedException($"Mapping to {prop.DeclaringType.Name}.{prop.Name} failed because type, {prop.PropertyType.Name}, is not supported.");
        }

        private class AttributeSpec
        {
            public PropertyInfo Prop;
            public DynamoDBRenamableAttribute Attribute;
            public AttributeMapping Mapping;
            public string AttributeName => Attribute?.AttributeName ?? Prop.Name;
        }
    }
}
