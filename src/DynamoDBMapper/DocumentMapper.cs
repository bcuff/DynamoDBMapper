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
        readonly IPropertyMapper[] _propertyMappers;
        readonly ConcurrentDictionary<Type, Lazy<DocumentMapping>> _mappings = new ConcurrentDictionary<Type, Lazy<DocumentMapping>>();

        internal DocumentMapper(IEnumerable<IPropertyMapper> propertyMappers)
        {
            _propertyMappers = propertyMappers.ToArray();
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
            var obj = Expression.Parameter(typeof(object), "document");
            var document = Expression.Variable(type, "doc");
            var result = Expression.Variable(typeof(Dictionary<string, AttributeValue>), "result");
            var add = typeof(Dictionary<string, AttributeValue>).GetMethod("Add", new[] { typeof(string), typeof(AttributeValue) });
            var steps = new List<Expression>();
            var tempVariables = new Dictionary<Type, ParameterExpression>();
            steps.Add(Expression.Assign(document, Expression.ConvertChecked(obj, type)));
            steps.Add(Expression.Assign(result, Expression.New(result.Type)));
            foreach (var spec in GetAttributeSpecifications(type))
            {
                var mapper = GetPropertyMapper(spec);
                if (!spec.TargetTypeInfo.IsValueType)
                {
                    ParameterExpression temp;
                    if (!tempVariables.TryGetValue(spec.Property.PropertyType, out temp))
                    {
                        temp = Expression.Variable(spec.Property.PropertyType);
                        tempVariables.Add(spec.Property.PropertyType, temp);
                    }
                    steps.Add(Expression.Assign(temp, Expression.MakeMemberAccess(document, spec.Property)));
                    steps.Add(Expression.IfThen(
                        Expression.NotEqual(temp, Expression.Constant(null)),
                        Expression.Call(
                            result,
                            add,
                            Expression.Constant(spec.Name),
                            mapper.GetToAttributeValueExpression(spec, temp)
                        )
                    ));
                }
                else
                {
                    steps.Add(Expression.Call(
                        result,
                        add,
                        Expression.Constant(spec.Name),
                        mapper.GetToAttributeValueExpression(spec, Expression.MakeMemberAccess(document, spec.Property))
                    ));
                }
            }
            steps.Add(result);
            return Expression.Lambda<Func<object, Dictionary<string, AttributeValue>>>(
                Expression.Block(
                    typeof(Dictionary<string, AttributeValue>),
                    new[] { document, result }.Concat(tempVariables.Values),
                    steps
                ),
                obj
            ).Compile();
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
                var mapper = GetPropertyMapper(spec);
                ParameterExpression temp;
                if (!tempLocals.TryGetValue(spec.TargetType, out temp))
                {
                    temp = Expression.Variable(spec.TargetType);
                    tempLocals.Add(spec.TargetType, temp);
                }
                var expr = Expression.IfThen(
                    Expression.Call(attributes, tryGetValue, Expression.Constant(spec.Name), attributeValue),
                    Expression.Assign(
                        Expression.MakeMemberAccess(result, spec.Property),
                        mapper.GetToDocumentPropertyExpression(spec, attributeValue)
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
            let ignore = prop.GetCustomAttributes<DynamoDBIgnoreAttribute>().FirstOrDefault()
            where ignore == null
            select new AttributeSpec(prop, attr);

        private IPropertyMapper GetPropertyMapper(AttributeSpec spec)
        {
            var mapper = _propertyMappers.FirstOrDefault(pm => pm.CanMap(spec));
            if (mapper == null)
            {
                throw new NotSupportedException($"Mapping on {spec.TargetType} {spec.Property.DeclaringType.Name}.{spec.Property.Name} is not supported by this configuration.");
            }
            return mapper;
        }
    }
}
