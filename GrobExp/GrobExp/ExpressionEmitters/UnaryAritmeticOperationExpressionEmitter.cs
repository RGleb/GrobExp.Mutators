﻿using System;
using System.Linq.Expressions;
using System.Reflection;

using GrEmit;

namespace GrobExp.ExpressionEmitters
{
    internal class UnaryAritmeticOperationExpressionEmitter : ExpressionEmitter<UnaryExpression>
    {
        protected override bool Emit(UnaryExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            Type operandType;
            var result = ExpressionEmittersCollection.Emit(node.Operand, context, returnDefaultValueLabel, ResultType.Value, extend, out operandType);
            GroboIL il = context.Il;
            if(node.NodeType == ExpressionType.IsTrue || node.NodeType == ExpressionType.IsFalse)
            {
                if(operandType == typeof(bool))
                {
                    if(node.NodeType == ExpressionType.IsFalse)
                    {
                        il.Ldc_I4(1);
                        il.Xor();
                    }
                }
                else if(operandType == typeof(bool?))
                {
                    using(var temp = context.DeclareLocal(operandType))
                    {
                        il.Stloc(temp);
                        il.Ldloca(temp);
                        il.Ldfld(operandType.GetField("hasValue", BindingFlags.Instance | BindingFlags.NonPublic));
                        var returnFalseLabel = il.DefineLabel("returnFalse");
                        il.Brfalse(returnFalseLabel);
                        il.Ldloca(temp);
                        il.Ldfld(operandType.GetField("value", BindingFlags.Instance | BindingFlags.NonPublic));
                        if(node.NodeType == ExpressionType.IsFalse)
                        {
                            il.Ldc_I4(1);
                            il.Xor();
                        }
                        var doneLabel = il.DefineLabel("done");
                        il.Br(doneLabel);
                        il.MarkLabel(returnFalseLabel);
                        il.Ldc_I4(0);
                        il.MarkLabel(doneLabel);
                    }
                }
                else throw new InvalidOperationException("Cannot perform operation '" + node.NodeType + "' to a type '" + operandType + "'");
            }
            else
            {
                if(!operandType.IsNullable())
                {
                    if(node.Method != null)
                        il.Call(node.Method);
                    else
                    {
                        if(operandType.IsStruct())
                            throw new InvalidOperationException("Cannot perform operation '" + node.NodeType + "' to a struct '" + operandType + "'");
                        switch(node.NodeType)
                        {
                        case ExpressionType.UnaryPlus:
                            break;
                        case ExpressionType.Negate:
                            il.Neg();
                            break;
                        case ExpressionType.NegateChecked:
                            using(var temp = context.DeclareLocal(operandType))
                            {
                                il.Stloc(temp);
                                il.Ldc_I4(0);
                                context.EmitConvert(typeof(int), operandType);
                                il.Ldloc(temp);
                                il.Sub_Ovf(operandType);
                            }
                            break;
                        case ExpressionType.Increment:
                            il.Ldc_I4(1);
                            if(operandType != typeof(int))
                                context.EmitConvert(typeof(int), operandType);
                            il.Add();
                            break;
                        case ExpressionType.Decrement:
                            il.Ldc_I4(1);
                            if(operandType != typeof(int))
                                context.EmitConvert(typeof(int), operandType);
                            il.Sub();
                            break;
                        default:
                            throw new InvalidOperationException("Node type '" + node.NodeType + "' invalid at this point");
                        }
                    }
                }
                else
                {
                    using(var temp = context.DeclareLocal(operandType))
                    {
                        il.Stloc(temp);
                        il.Ldloca(temp);
                        il.Ldfld(operandType.GetField("hasValue", BindingFlags.Instance | BindingFlags.NonPublic));
                        var returnNullLabel = il.DefineLabel("returnLabel");
                        il.Brfalse(returnNullLabel);
                        Type argumentType = operandType.GetGenericArguments()[0];
                        if(node.Method != null)
                        {
                            il.Ldloca(temp);
                            il.Ldfld(operandType.GetField("value", BindingFlags.Instance | BindingFlags.NonPublic));
                            il.Call(node.Method);
                        }
                        else
                        {
                            switch(node.NodeType)
                            {
                            case ExpressionType.UnaryPlus:
                                il.Ldloca(temp);
                                il.Ldfld(operandType.GetField("value", BindingFlags.Instance | BindingFlags.NonPublic));
                                break;
                            case ExpressionType.Negate:
                                il.Ldloca(temp);
                                il.Ldfld(operandType.GetField("value", BindingFlags.Instance | BindingFlags.NonPublic));
                                il.Neg();
                                break;
                            case ExpressionType.NegateChecked:
                                il.Ldc_I4(0);
                                context.EmitConvert(typeof(int), argumentType);
                                il.Ldloca(temp);
                                il.Ldfld(operandType.GetField("value", BindingFlags.Instance | BindingFlags.NonPublic));
                                il.Sub_Ovf(argumentType);
                                break;
                            case ExpressionType.Increment:
                                il.Ldloca(temp);
                                il.Ldfld(operandType.GetField("value", BindingFlags.Instance | BindingFlags.NonPublic));
                                il.Ldc_I4(1);
                                if(argumentType != typeof(int))
                                    context.EmitConvert(typeof(int), argumentType);
                                il.Add();
                                break;
                            case ExpressionType.Decrement:
                                il.Ldloca(temp);
                                il.Ldfld(operandType.GetField("value", BindingFlags.Instance | BindingFlags.NonPublic));
                                il.Ldc_I4(1);
                                if(argumentType != typeof(int))
                                    context.EmitConvert(typeof(int), argumentType);
                                il.Sub();
                                break;
                            default:
                                throw new InvalidOperationException("Node type '" + node.NodeType + "' invalid at this point");
                            }
                        }
                        il.Newobj(operandType.GetConstructor(new[] {argumentType}));
                        var doneLabel = il.DefineLabel("done");
                        il.Br(doneLabel);
                        il.MarkLabel(returnNullLabel);
                        context.EmitLoadDefaultValue(operandType);
                        il.MarkLabel(doneLabel);
                    }
                }
            }
            resultType = node.Type;
            return result;
        }
    }
}