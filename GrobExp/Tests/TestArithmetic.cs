﻿using System;
using System.Linq.Expressions;

using GrobExp;

using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class TestArithmetic
    {
        [Test]
        public void TestAdd1()
        {
            Expression<Func<int, int, int>> exp = (a, b) => a + b;
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(3, f(1, 2));
            Assert.AreEqual(1, f(-1, 2));
            unchecked
            {
                Assert.AreEqual(2000000000 + 2000000000, f(2000000000, 2000000000));
            }
        }

        [Test]
        public void TestAdd2()
        {
            Expression<Func<int?, int?, int?>> exp = (a, b) => a + b;
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(3, f(1, 2));
            Assert.AreEqual(1, f(-1, 2));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
            unchecked
            {
                Assert.AreEqual(2000000000 + 2000000000, f(2000000000, 2000000000));
            }
        }

        [Test]
        public void TestAdd3()
        {
            Expression<Func<int?, long?, long?>> exp = (a, b) => a + b;
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(3, f(1, 2));
            Assert.AreEqual(1, f(-1, 2));
            Assert.AreEqual(12000000000, f(2000000000, 10000000000));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
        }

        [Test]
        public void TestAdd4()
        {
            ParameterExpression a = Expression.Parameter(typeof(int));
            ParameterExpression b = Expression.Parameter(typeof(int));
            Expression<Func<int, int, int>> exp = Expression.Lambda<Func<int, int, int>>(Expression.AddChecked(a, b), a, b);
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(3, f(1, 2));
            Assert.AreEqual(1, f(-1, 2));
            Assert.Throws<OverflowException>(() => f(2000000000, 2000000000));
        }

        [Test]
        public void TestAdd5()
        {
            ParameterExpression a = Expression.Parameter(typeof(int?));
            ParameterExpression b = Expression.Parameter(typeof(int?));
            Expression<Func<int?, int?, int?>> exp = Expression.Lambda<Func<int?, int?, int?>>(Expression.AddChecked(a, b), a, b);
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(3, f(1, 2));
            Assert.AreEqual(1, f(-1, 2));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
            Assert.Throws<OverflowException>(() => f(2000000000, 2000000000));
        }

        [Test]
        public void TestAdd6()
        {
            ParameterExpression a = Expression.Parameter(typeof(uint));
            ParameterExpression b = Expression.Parameter(typeof(uint));
            Expression<Func<uint, uint, uint>> exp = Expression.Lambda<Func<uint, uint, uint>>(Expression.AddChecked(a, b), a, b);
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(3, f(1, 2));
            Assert.AreEqual(3000000000, f(1000000000, 2000000000));
            Assert.Throws<OverflowException>(() => f(3000000000, 2000000000));
        }

        [Test]
        public void TestAdd7()
        {
            ParameterExpression a = Expression.Parameter(typeof(uint?));
            ParameterExpression b = Expression.Parameter(typeof(uint?));
            Expression<Func<uint?, uint?, uint?>> exp = Expression.Lambda<Func<uint?, uint?, uint?>>(Expression.AddChecked(a, b), a, b);
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(3, f(1, 2));
            Assert.AreEqual(3000000000, f(1000000000, 2000000000));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
            Assert.Throws<OverflowException>(() => f(3000000000, 2000000000));
        }

        [Test]
        public void TestSub1()
        {
            Expression<Func<int, int, int>> exp = (a, b) => a - b;
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(-1, f(1, 2));
            Assert.AreEqual(1, f(-1, -2));
            unchecked
            {
                Assert.AreEqual(2000000000 - -2000000000, f(2000000000, -2000000000));
            }
        }

        [Test]
        public void TestSub2()
        {
            Expression<Func<int?, int?, int?>> exp = (a, b) => a - b;
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(-1, f(1, 2));
            Assert.AreEqual(1, f(-1, -2));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
            unchecked
            {
                Assert.AreEqual(2000000000 - -2000000000, f(2000000000, -2000000000));
            }
        }

        [Test]
        public void TestSub3()
        {
            Expression<Func<int?, long?, long?>> exp = (a, b) => a - b;
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(-1, f(1, 2));
            Assert.AreEqual(1, f(-1, -2));
            Assert.AreEqual(-8000000000, f(2000000000, 10000000000));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
        }

        [Test]
        public void TestSub4()
        {
            ParameterExpression a = Expression.Parameter(typeof(int));
            ParameterExpression b = Expression.Parameter(typeof(int));
            Expression<Func<int, int, int>> exp = Expression.Lambda<Func<int, int, int>>(Expression.SubtractChecked(a, b), a, b);
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(-1, f(1, 2));
            Assert.AreEqual(1, f(-1, -2));
            Assert.Throws<OverflowException>(() => f(2000000000, -2000000000));
        }

        [Test]
        public void TestSub5()
        {
            ParameterExpression a = Expression.Parameter(typeof(int?));
            ParameterExpression b = Expression.Parameter(typeof(int?));
            Expression<Func<int?, int?, int?>> exp = Expression.Lambda<Func<int?, int?, int?>>(Expression.SubtractChecked(a, b), a, b);
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(-1, f(1, 2));
            Assert.AreEqual(1, f(-1, -2));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
            Assert.Throws<OverflowException>(() => f(2000000000, -2000000000));
        }

        [Test]
        public void TestSub6()
        {
            ParameterExpression a = Expression.Parameter(typeof(uint));
            ParameterExpression b = Expression.Parameter(typeof(uint));
            Expression<Func<uint, uint, uint>> exp = Expression.Lambda<Func<uint, uint, uint>>(Expression.SubtractChecked(a, b), a, b);
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(3000000000, f(4000000000, 1000000000));
            Assert.Throws<OverflowException>(() => f(1, 2));
        }

        [Test]
        public void TestSub7()
        {
            ParameterExpression a = Expression.Parameter(typeof(uint?));
            ParameterExpression b = Expression.Parameter(typeof(uint?));
            Expression<Func<uint?, uint?, uint?>> exp = Expression.Lambda<Func<uint?, uint?, uint?>>(Expression.SubtractChecked(a, b), a, b);
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(3000000000, f(4000000000, 1000000000));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
            Assert.Throws<OverflowException>(() => f(1, 2));
        }

        [Test]
        public void TestMul1()
        {
            Expression<Func<int, int, int>> exp = (a, b) => a * b;
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(0, 1));
            Assert.AreEqual(2, f(1, 2));
            Assert.AreEqual(6, f(-2, -3));
            Assert.AreEqual(-20, f(-2, 10));
            unchecked
            {
                Assert.AreEqual(2000000000 * 2000000000, f(2000000000, 2000000000));
            }
        }

        [Test]
        public void TestMul2()
        {
            Expression<Func<int?, int?, int?>> exp = (a, b) => a * b;
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(2, f(1, 2));
            Assert.AreEqual(6, f(-2, -3));
            Assert.AreEqual(-20, f(-2, 10));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
            unchecked
            {
                Assert.AreEqual(2000000000 * 2000000000, f(2000000000, 2000000000));
            }
        }

        [Test]
        public void TestMul3()
        {
            Expression<Func<int?, long?, long?>> exp = (a, b) => a * b;
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(2, f(1, 2));
            Assert.AreEqual(6, f(-2, -3));
            Assert.AreEqual(-20, f(-2, 10));
            Assert.AreEqual(-20000000000, f(2000000000, -10));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
        }

        [Test]
        public void TestMul4()
        {
            ParameterExpression a = Expression.Parameter(typeof(int));
            ParameterExpression b = Expression.Parameter(typeof(int));
            Expression<Func<int, int, int>> exp = Expression.Lambda<Func<int, int, int>>(Expression.MultiplyChecked(a, b), a, b);
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(0, 1));
            Assert.AreEqual(2, f(1, 2));
            Assert.AreEqual(6, f(-2, -3));
            Assert.AreEqual(-20, f(-2, 10));
            Assert.Throws<OverflowException>(() => f(2000000000, 2000000000));
        }

        [Test]
        public void TestMul5()
        {
            ParameterExpression a = Expression.Parameter(typeof(int?));
            ParameterExpression b = Expression.Parameter(typeof(int?));
            Expression<Func<int?, int?, int?>> exp = Expression.Lambda<Func<int?, int?, int?>>(Expression.MultiplyChecked(a, b), a, b);
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(2, f(1, 2));
            Assert.AreEqual(6, f(-2, -3));
            Assert.AreEqual(-20, f(-2, 10));
            Assert.AreEqual(-2000000000, f(200000000, -10));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
            Assert.Throws<OverflowException>(() => f(2000000000, 2000000000));
        }

        [Test]
        public void TestMul6()
        {
            ParameterExpression a = Expression.Parameter(typeof(uint));
            ParameterExpression b = Expression.Parameter(typeof(uint));
            Expression<Func<uint, uint, uint>> exp = Expression.Lambda<Func<uint, uint, uint>>(Expression.MultiplyChecked(a, b), a, b);
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(0, 1));
            Assert.AreEqual(2, f(1, 2));
            Assert.AreEqual(4000000000, f(2000000000, 2));
            Assert.Throws<OverflowException>(() => f(2000000000, 2000000000));
        }

        [Test]
        public void TestMul7()
        {
            ParameterExpression a = Expression.Parameter(typeof(uint?));
            ParameterExpression b = Expression.Parameter(typeof(uint?));
            Expression<Func<uint?, uint?, uint?>> exp = Expression.Lambda<Func<uint?, uint?, uint?>>(Expression.MultiplyChecked(a, b), a, b);
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(0, 0));
            Assert.AreEqual(2, f(1, 2));
            Assert.AreEqual(4000000000, f(2000000000, 2));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
            Assert.Throws<OverflowException>(() => f(2000000000, 2000000000));
        }

        [Test]
        public void TestDiv1()
        {
            Expression<Func<int, int, int>> exp = (a, b) => a / b;
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(1, 2));
            Assert.AreEqual(2, f(5, 2));
            Assert.AreEqual(-1, f(-3, 2));
        }

        [Test]
        public void TestDiv2()
        {
            Expression<Func<int?, int?, int?>> exp = (a, b) => a / b;
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(1, 2));
            Assert.AreEqual(2, f(5, 2));
            Assert.AreEqual(-1, f(-3, 2));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
        }

        [Test]
        public void TestDiv3()
        {
            Expression<Func<int?, long?, long?>> exp = (a, b) => a / b;
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(1, 2));
            Assert.AreEqual(2, f(5, 2));
            Assert.AreEqual(-1, f(-3, 2));
            Assert.AreEqual(0, f(2000000000, 20000000000));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
        }

        [Test]
        public void TestDiv4()
        {
            Expression<Func<double, double, double>> exp = (a, b) => a / b;
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0.5, f(1, 2));
            Assert.AreEqual(2.5, f(5, 2));
            Assert.AreEqual(-1.5, f(-3, 2));
        }

        [Test]
        public void TestDiv5()
        {
            Expression<Func<uint, uint, uint>> exp = (a, b) => a / b;
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(0, f(1, 2));
            Assert.AreEqual(2, f(5, 2));
            Assert.AreEqual(2147483646, f(uint.MaxValue - 3 + 1, 2));
        }

        [Test]
        public void TestModulo1()
        {
            Expression<Func<int, int, int>> exp = (a, b) => a % b;
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(1, f(1, 2));
            Assert.AreEqual(2, f(5, 3));
            Assert.AreEqual(-1, f(-3, 2));
        }

        [Test]
        public void TestModulo2()
        {
            Expression<Func<int?, int?, int?>> exp = (a, b) => a % b;
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(1, f(1, 2));
            Assert.AreEqual(2, f(5, 3));
            Assert.AreEqual(-1, f(-3, 2));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
        }

        [Test]
        public void TestModulo3()
        {
            Expression<Func<int?, long?, long?>> exp = (a, b) => a % b;
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(1, f(1, 2));
            Assert.AreEqual(2, f(5, 3));
            Assert.AreEqual(-1, f(-3, 2));
            Assert.AreEqual(2000000000, f(2000000000, 20000000000));
            Assert.IsNull(f(null, 2));
            Assert.IsNull(f(1, null));
            Assert.IsNull(f(null, null));
        }

        [Test]
        public void TestModulo4()
        {
            Expression<Func<uint, uint, uint>> exp = (a, b) => a % b;
            var f = LambdaCompiler.Compile(exp);
            Assert.AreEqual(1, f(1, 2));
            Assert.AreEqual(2, f(5, 3));
            Assert.AreEqual(1, f(uint.MaxValue - 3 + 1, 2));
        }
    }
}