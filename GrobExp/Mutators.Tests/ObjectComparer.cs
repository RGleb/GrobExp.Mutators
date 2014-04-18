﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using GroBuf;
using GroBuf.DataMembersExtracters;

using GrobExp.Mutators;
using GrobExp.Mutators.MultiLanguages;

using NUnit.Framework;

namespace Mutators.Tests
{
    public static class ObjectComparer
    {
        public static void AssertEqualsToUsingGrobuf<T>(this T actual, T expected, string message = "", params object[] args)
        {
            var expectedBytes = serializer.Serialize(expected);
            var actualBytes = serializer.Serialize(actual);
            CollectionAssert.AreEqual(expectedBytes, actualBytes, message, args);
        }

        public static void AssertEqualsExpression(this Expression actual, Expression expected)
        {
            Assert.AreEqual(ExpressionCompiler.DebugViewGetter(actual.Simplify()), ExpressionCompiler.DebugViewGetter(expected.Simplify()));
        }

        public static void AssertEqualsExpression<T>(this Expression<T> actual, Expression<T> expected)
        {
            Assert.AreEqual(ExpressionCompiler.DebugViewGetter(actual.Simplify()), ExpressionCompiler.DebugViewGetter(expected.Simplify()));
        }

        private static readonly ISerializer serializer = new Serializer(new AllFieldsExtractor(), new TestGroBufCustomSerializerCollection());
    }

    public class TestGroBufCustomSerializerCollection : IGroBufCustomSerializerCollection
    {
        private readonly IGroBufCustomSerializerCollection defaultCollection = new DefaultGroBufCustomSerializerCollection();

        public IGroBufCustomSerializer Get(Type declaredType, Func<Type, IGroBufCustomSerializer> factory, IGroBufCustomSerializer baseSerializer)
        {
            bool isAbstractMultiLanguageText = (declaredType == typeof(MultiLanguageTextBase) || declaredType.IsSubclassOf(typeof(MultiLanguageTextBase))) && declaredType.IsAbstract;
            return defaultCollection.Get(isAbstractMultiLanguageText ? typeof(MultiLanguageTextSerializer) : declaredType, factory, baseSerializer);
        }
    }

    [GroBufCustomSerialization]
    public static class MultiLanguageTextSerializer
    {
        [GroBufSizeCounter]
        public static SizeCounterDelegate GetSizeCounter(Func<Type, SizeCounterDelegate> sizeCountersFactory, SizeCounterDelegate baseSizeCounter)
        {
            return DerivedTypesSerializationBase<MultiLanguageTextBase, MultiLanguageTextTypeAttribute>.GetSizeCounter(sizeCountersFactory, baseSizeCounter, attribute => attribute.Name);
        }

        [GroBufWriter]
        public static WriterDelegate GetWriter(Func<Type, WriterDelegate> writersFactory, WriterDelegate baseWriter)
        {
            return DerivedTypesSerializationBase<MultiLanguageTextBase, MultiLanguageTextTypeAttribute>.GetWriter(writersFactory, baseWriter, attribute => attribute.Name);
        }

        [GroBufReader]
        public static ReaderDelegate GetReader(Func<Type, ReaderDelegate> readersFactory, ReaderDelegate baseReader)
        {
            return DerivedTypesSerializationBase<MultiLanguageTextBase, MultiLanguageTextTypeAttribute>.GetReader(readersFactory, baseReader, attribute => attribute.Name);
        }
    }

    public static class DerivedTypesSerializationBase<TBase, TAttribute> where TAttribute : Attribute
    {
        public static SizeCounterDelegate GetSizeCounter(Func<Type, SizeCounterDelegate> sizeCountersFactory, SizeCounterDelegate baseSizeCounter, Func<TAttribute, string> attributeKeySelector)
        {
            return (o, writeEmpty) =>
            {
                var type = o.GetType();
                return sizeCountersFactory(typeof(string))(GetTypeNameByType(type, attributeKeySelector), true) + sizeCountersFactory(type)(o, true);
            };
        }

        public static WriterDelegate GetWriter(Func<Type, WriterDelegate> writersFactory, WriterDelegate baseWriter, Func<TAttribute, string> attributeKeySelector)
        {
            return (object o, bool writeEmpty, IntPtr result, ref int index, int dataLength) =>
            {
                var type = o.GetType();
                writersFactory(typeof(string))(GetTypeNameByType(type, attributeKeySelector), true, result, ref index, dataLength);
                writersFactory(type)(o, true, result, ref index, dataLength);
                if (index < 0) index = 0;
            };
        }

        public static ReaderDelegate GetReader(Func<Type, ReaderDelegate> readersFactory, ReaderDelegate baseReader, Func<TAttribute, string> attributeKeySelector)
        {
            return (IntPtr data, ref int index, int length, ref object result) =>
            {
                object textType = null;
                readersFactory(typeof(string))(data, ref index, length, ref textType);
                var type = GetTypeByTypeNameType((string)textType, attributeKeySelector) ?? typeof(string);
                readersFactory(type)(data, ref index, length, ref result);
            };
        }

        public static Type GetTypeByTypeNameType(string typeName, Func<TAttribute, string> attributeKeySelector)
        {
            if (string.IsNullOrEmpty(typeName)) return null;
            if (typeNameToTypeMap == null)
            {
                var types = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             where !assembly.IsDynamic
                             from type in GetTypes(assembly)
                             where type.IsSubclassOf(typeof(TBase))
                             let attribute = (TAttribute)type.GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault()
                             where attribute != null
                             select new { key = attributeKeySelector(attribute), type });
                var dict = new Dictionary<string, Type>();
                foreach (var type in types)
                {
                    if (dict.ContainsKey(type.key))
                        throw new InvalidOperationException();
                    dict.Add(type.key, type.type);
                }
                typeNameToTypeMap = dict;
            }
            Type result;
            return typeNameToTypeMap.TryGetValue(typeName, out result) ? result : null;
        }

        private static string GetTypeNameByType(Type type, Func<TAttribute, string> attributeKeySelector)
        {
            var attributes = type.GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>().ToArray();
            if (attributes.Length == 0)
                throw new InvalidOperationException();
            return attributeKeySelector(attributes[0]);
        }

        private static IEnumerable<Type> GetTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                var sb = new StringBuilder();
                foreach (var loaderException in e.LoaderExceptions)
                    sb.AppendLine(loaderException.Message);
                throw new Exception(
                    string.Format("Ошибка при получении типов из сборки '{0}'\r\n(Path:'{1}')\n{2}", assembly.FullName, assembly.Location, sb), e);
            }
        }

        // ReSharper disable StaticFieldInGenericType
        private static Dictionary<string, Type> typeNameToTypeMap;
        // ReSharper restore StaticFieldInGenericType
    }

}