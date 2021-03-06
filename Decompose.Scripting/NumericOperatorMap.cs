﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Decompose.Numerics;

namespace Decompose.Scripting
{
    public class NumericOperatorMap<T> : IOperatorMap
    {
        private Dictionary<Op, Func<T, object>> unaryOps = new Dictionary<Op, Func<T, object>>();
        private Dictionary<Op, Func<T, T, object>> binaryOps = new Dictionary<Op, Func<T, T, object>>();
        private Dictionary<Op, Func<T, T, T, object>> ternaryOps = new Dictionary<Op, Func<T, T, T, object>>();

        public NumericOperatorMap(IRandomNumberGenerator generator)
        {
            var ops = Operations.Get<T>();
            var rand = generator.Create<T>();
            unaryOps.Add(Op.Negate, a => ops.Negate(a));
            unaryOps.Add(Op.OnesComplement, a => ops.OnesComplement(a));
            unaryOps.Add(Op.Random, a => rand.Next(a));
            unaryOps.Add(Op.Factorial, a => ops.Factorial(a));
            binaryOps.Add(Op.Add, (a, b) => ops.Add(a, b));
            binaryOps.Add(Op.Subtract, (a, b) => ops.Subtract(a, b));
            binaryOps.Add(Op.Multiply, (a, b) => ops.Multiply(a, b));
            binaryOps.Add(Op.Divide, (a, b) => ops.Divide(a, b));
            binaryOps.Add(Op.Remainder, (a, b) => ops.Remainder(a, b));
            binaryOps.Add(Op.Modulo, (a, b) => ops.Modulo(a, b));
            binaryOps.Add(Op.Power, (a, b) => ops.Power(a, b));
            binaryOps.Add(Op.And, (a, b) => ops.And(a, b));
            binaryOps.Add(Op.Or, (a, b) => ops.Or(a, b));
            binaryOps.Add(Op.ExclusiveOr, (a, b) => ops.ExclusiveOr(a, b));
            binaryOps.Add(Op.LeftShift, (a, b) => ops.LeftShift(a, ops.ToInt32(b)));
            binaryOps.Add(Op.RightShift, (a, b) => ops.RightShift(a, ops.ToInt32(b)));
            binaryOps.Add(Op.Equals, (a, b) => ops.Equals(a, b));
            binaryOps.Add(Op.NotEquals, (a, b) => !ops.Equals(a, b));
            binaryOps.Add(Op.LessThan, (a, b) => ops.Compare(a, b) < 0);
            binaryOps.Add(Op.LessThanOrEqual, (a, b) => ops.Compare(a, b) <= 0);
            binaryOps.Add(Op.GreaterThan, (a, b) => ops.Compare(a, b) > 0);
            binaryOps.Add(Op.GreaterThanOrEqual, (a, b) => ops.Compare(a, b) >= 0);
            binaryOps.Add(Op.GreatestCommonDivisor, (a, b) => ops.GreatestCommonDivisor(a, b));
            binaryOps.Add(Op.Divides, (a, b) => ops.IsZero(ops.Remainder(b, a)));
            binaryOps.Add(Op.NotDivides, (a, b) => !ops.IsZero(ops.Remainder(b, a)));
            binaryOps.Add(Op.ModularNegate, (a, b) => ops.ModularDifference(ops.Zero, a, b));
            ternaryOps.Add(Op.ModularSum, (a, b, c) => ops.ModularSum(a, b, c));
            ternaryOps.Add(Op.ModularDifference, (a, b, c) => ops.ModularDifference(a, b, c));
            ternaryOps.Add(Op.ModularProduct, (a, b, c) => ops.ModularProduct(a, b, c));
            ternaryOps.Add(Op.ModularQuotient, (a, b, c) => ops.ModularProduct(a, ops.ModularInverse(b, c), c));
            ternaryOps.Add(Op.ModularPower, (a, b, c) => ops.ModularPower(a, b, c));
            ternaryOps.Add(Op.ModularEquals, (a, b, c) => ops.Equals(ops.Modulo(a, c), ops.Modulo(b, c)));
            ternaryOps.Add(Op.ModularNotEquals, (a, b, c) => !ops.Equals(ops.Modulo(a, c), ops.Modulo(b, c)));
        }

        public object Operator(Op op, params object[] args)
        {
            if (unaryOps.ContainsKey(op))
                return unaryOps[op]((T)args[0]);
            if (binaryOps.ContainsKey(op))
                return binaryOps[op]((T)args[0], (T)args[1]);
            if (ternaryOps.ContainsKey(op))
                return ternaryOps[op]((T)args[0], (T)args[1], (T)args[2]);
            throw new NotImplementedException();
        }
    }
}
