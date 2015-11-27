﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

using GrEmit;
using GrEmit.Utils;

using GrobExp.Compiler;

namespace GrobExp.Mutators
{
    public static class ExpressionTypeBuilder
    {
        public static Type GetType(Expression[] expressionsToExtract, string[] fieldNames, out FieldInfo[] fieldInfos)
        {
            var key = new TypeWrapper(expressionsToExtract.Select(exp => exp.Type));

            if(TypeCache.ContainsKey(key))
            {
                var fromCache = (Type)TypeCache[key];
                fieldInfos = fieldNames.Select(fromCache.GetField).ToArray();
                return fromCache;
            }
            return (Type)(TypeCache[key] = BuildType(expressionsToExtract, fieldNames, out fieldInfos));
        }

        private static Type BuildType(Expression[] expressionsToExtract, string[] fieldNames, out FieldInfo[] fieldInfos)
        {
            var typeBuilder = module.DefineType("Closure__" + id++, TypeAttributes.Class | TypeAttributes.Public);
            var fieldBuilders = new List<FieldBuilder>();
            for (var i = 0; i < fieldNames.Length; ++i)
            {
                var type = expressionsToExtract[i].Type;
                if (!type.IsStronglyPublic())
                {
                    if (type.IsValueType)
                        throw new NotSupportedException("Non-public value types are not supported");
                    type = typeof(object);
                }
                fieldBuilders.Add(typeBuilder.DefineField(fieldNames[i], type, FieldAttributes.Public));
            }

            BuildDefaultConstructor(typeBuilder);
            BuildConstructorByFields(typeBuilder, fieldBuilders);

            var result = typeBuilder.CreateType();
            fieldInfos = fieldNames.Select(result.GetField).ToArray();
            return result;
        }

        private static void BuildDefaultConstructor(TypeBuilder typeBuilder)
        {
            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, Type.EmptyTypes);
            using(var il = new GroboIL(constructor))
            {
                il.Ret();
            }
        }

        private static void BuildConstructorByFields(TypeBuilder typeBuilder, List<FieldBuilder> fieldBuilders)
        {
            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new[] {typeof(object[])});
            using(var il = new GroboIL(constructor))
            {
                for(var i = 0; i < fieldBuilders.Count; ++i)
                {
                    var field = fieldBuilders[i];
                    il.Ldarg(0); // this
                    il.Ldarg(1); // this, arr                        
                    il.Ldc_I4(i); // this, arr, index
                    il.Ldelem(typeof(object)); // this, arr[index]
                    if(field.FieldType.IsValueType)
                        il.Unbox_Any(field.FieldType); // this, (fieldType)arr[index]
                    else
                        il.Castclass(field.FieldType); // this, (fieldType)arr[index]
                    il.Stfld(field);
                }
                il.Ret();
            }
        }

        public static string[] GenerateFieldNames(Expression[] extractedExpressions)
        {
            var indexes = new Dictionary<Type, int>();
            var result = new string[extractedExpressions.Length];
            for (var i = 0; i < extractedExpressions.Length; ++i)
            {
                var expressionType = extractedExpressions[i].Type;
                var index = indexes.ContainsKey(expressionType) ? indexes[expressionType] + 1 : 0;
                indexes[expressionType] = index;
                result[i] = Formatter.Format(expressionType) + "_" + index;
            }
            return result;
        }

        private class TypeWrapper
        {
            public Type[] FieldTypes { get; private set; }

            public TypeWrapper(IEnumerable<Type> fieldTypes)
            {
                FieldTypes = fieldTypes.OrderBy(f => f.Name).ToArray();
            }

            public override bool Equals(object obj)
            {
                var other = obj as TypeWrapper;
                if(other == null)
                    return false;
                if(FieldTypes.Length != other.FieldTypes.Length)
                    return false;
                return Enumerable.Range(0, FieldTypes.Length).All(i => FieldTypes[i] == other.FieldTypes[i]);
            }

            public override int GetHashCode()
            {
                return FieldTypes.Aggregate(0, (h, f) => unchecked (h * prime + f.GetHashCode()));
            }

            private const int prime = 997;
        }

        public static readonly Hashtable TypeCache = new Hashtable();

        private static int id = 0;

        private static readonly AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.RunAndSave);
        private static readonly ModuleBuilder module = assembly.DefineDynamicModule(Guid.NewGuid().ToString());
    }
}
