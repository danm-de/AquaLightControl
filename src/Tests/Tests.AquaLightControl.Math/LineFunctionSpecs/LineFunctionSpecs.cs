using System;
using System.Collections;
using AquaLightControl;
using AquaLightControl.Math;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace Tests.AquaLightControl.Math.LineFunctionSpecs
{
    [TestFixture]
    public class If_y_is_requested_and_the_line_is_upward : Spec
    {
        private LineFunction _line_function;

        protected override void EstablishContext() {
            var start = new Point(0, 0);
            var end = new Point(10, 10);

            _line_function = new LineFunction(start, end);

        }

        private static IEnumerable TestCases {
            get {
                yield return new TestCaseData(-1).Returns(-1.0);
                yield return new TestCaseData(0).Returns(0.0);
                yield return new TestCaseData(1).Returns(1.0);
                yield return new TestCaseData(2).Returns(2.0);
                yield return new TestCaseData(5).Returns(5.0);
                yield return new TestCaseData(10).Returns(10.0);
                yield return new TestCaseData(11).Returns(11.0);
            }
        }

        [Test, TestCaseSource("TestCases")]
        public double Shall_y_be_correct(long x) {
            return _line_function.GetY(x);
        }
    }

    [TestFixture]
    public class If_y_is_requested_and_the_line_is_downward : Spec
    {
        private LineFunction _line_function;

        protected override void EstablishContext() {
            var start = new Point(0, 10);
            var end = new Point(10, 0);

            _line_function = new LineFunction(start, end);

        }

        private static IEnumerable TestCases {
            get {
                yield return new TestCaseData(-1).Returns(11.0);
                yield return new TestCaseData(0).Returns(10.0);
                yield return new TestCaseData(1).Returns(9.0);
                yield return new TestCaseData(2).Returns(8.0);
                yield return new TestCaseData(5).Returns(5.0);
                yield return new TestCaseData(10).Returns(0.0);
                yield return new TestCaseData(11).Returns(-1.0);
            }
        }

        [Test, TestCaseSource("TestCases")]
        public double Shall_y_be_correct(long x) {
            return _line_function.GetY(x);
        }
    }

    [TestFixture]
    public class If_y_is_requested_and_the_line_is_straight_at_10 : Spec
    {
        private LineFunction _line_function;

        protected override void EstablishContext() {
            var start = new Point(0, 10);
            var end = new Point(10, 10);

            _line_function = new LineFunction(start, end);

        }

        private static IEnumerable TestCases {
            get {
                yield return new TestCaseData(-1).Returns(10.0);
                yield return new TestCaseData(0).Returns(10.0);
                yield return new TestCaseData(1).Returns(10.0);
                yield return new TestCaseData(2).Returns(10.0);
                yield return new TestCaseData(5).Returns(10.0);
                yield return new TestCaseData(10).Returns(10.0);
                yield return new TestCaseData(11).Returns(10.0);
            }
        }

        [Test, TestCaseSource("TestCases")]
        public double Shall_y_be_correct(long x) {
            return _line_function.GetY(x);
        }
    }

    [TestFixture]
    public class If_y_is_requested_and_the_line_is_straight_at_0 : Spec
    {
        private LineFunction _line_function;

        protected override void EstablishContext() {
            var start = new Point(0, 0);
            var end = new Point(10, 0);

            _line_function = new LineFunction(start, end);

        }

        private static IEnumerable TestCases {
            get {
                yield return new TestCaseData(-1).Returns(0.0);
                yield return new TestCaseData(0).Returns(0.0);
                yield return new TestCaseData(1).Returns(0.0);
                yield return new TestCaseData(2).Returns(0.0);
                yield return new TestCaseData(5).Returns(0.0);
                yield return new TestCaseData(10).Returns(0.0);
                yield return new TestCaseData(11).Returns(0.0);
            }
        }

        [Test, TestCaseSource("TestCases")]
        public double Shall_y_be_correct(long x) {
            return _line_function.GetY(x);
        }
    }
}