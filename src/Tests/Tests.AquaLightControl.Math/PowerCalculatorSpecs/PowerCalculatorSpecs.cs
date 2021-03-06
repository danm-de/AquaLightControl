﻿using System;
using System.Collections;
using AquaLightControl;
using AquaLightControl.Math;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace Tests.AquaLightControl.Math.PowerCalculatorSpecs
{
    [TestFixture]
    public class If_the_power_value_of_a_specific_x_is_requested : Spec {
        private PowerCalculator _calculator;

        protected override void EstablishContext() {
            var p1 = new Point(0, 10);
            var p2 = new Point(10, 10);
            var p3 = new Point(20, 20);
            
            var line_function_1 = A.Fake<ILineFunction>();
            line_function_1.CallsTo(f => f.GetY(A<long>.Ignored)).Returns(10);

            var line_function_2 = A.Fake<ILineFunction>();
            line_function_2.CallsTo(f => f.GetY(A<long>.Ignored)).Returns(20);

            var factory = A.Fake<ILineFunctionFactory>();
            factory.CallsTo(f => f.Create(p1, p2))
                .Returns(line_function_1);
            factory.CallsTo(f => f.Create(p2, p3))
                .Returns(line_function_2);

            _calculator = new PowerCalculator(factory, new[] {p1, p2, p3});
        }

        private static IEnumerable TestCases {
            get {
                yield return new TestCaseData(-1).Throws(typeof(ArgumentOutOfRangeException));
                yield return new TestCaseData(0).Returns(10);
                yield return new TestCaseData(10).Returns(20);
                yield return new TestCaseData(11).Returns(20);
                yield return new TestCaseData(20).Returns(20);
                yield return new TestCaseData(21).Throws(typeof(ArgumentOutOfRangeException));
            }
        }

        [Test,TestCaseSource("TestCases")]
        public long Shall_the_result_be_correct(long x) {
            return _calculator.GetY(x);
        }
    }
}