using AquaLightControl;
using AquaLightControl.Math;
using AquaLightControl.Math.Factories;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
using FluentAssertions;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace Tests.AquaLightControl.Math.Factories.PowerCalculatorFactorySpecs
{
    [TestFixture]
    public class If_the_user_requests_a_power_calculator_with_three_valid_points : Spec
    {
        private const long FIRST_LINE_RESULT = 5;
        private const long SECOND_LINE_RESULT = 15;

        private Point _p1, _p2, _p3;
        private ILineFunctionFactory _line_function_factory;
        private PowerCalculatorFactory _factory;
        private ILineFunction _first_line;
        private ILineFunction _second_line;
        private IPowerCalculator _power_calculator;

        protected override void EstablishContext() {
            _p1 = new Point(0, 0);
            _p2 = new Point(10, 10);
            _p3 = new Point(20, 20);

            _first_line = A.Fake<ILineFunction>();
            _first_line
                .CallsTo(line_function => line_function.GetY(A<long>.Ignored))
                .Returns(FIRST_LINE_RESULT);
            _second_line = A.Fake<ILineFunction>();
            _second_line
                .CallsTo(line_function => line_function.GetY(A<long>.Ignored))
                .Returns(SECOND_LINE_RESULT);

            _line_function_factory = A.Fake<ILineFunctionFactory>();
            _line_function_factory
                .CallsTo(factory => factory.Create(A<Point>.Ignored, A<Point>.Ignored))
                .ReturnsNextFromSequence(_first_line, _second_line);

            _factory = new PowerCalculatorFactory(_line_function_factory);
        }

        protected override void BecauseOf() {
            _power_calculator = _factory.Create(new[] {_p1, _p2, _p3});
        }

        [Test]
        public void Shall_the_LineFunctionFactory_being_called_for_p1_and_p2() {
            _line_function_factory
                .CallsTo(f => f.Create(_p1, _p2))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Shall_the_LineFunctionFactory_being_called_for_p2_and_p3() {
            _line_function_factory
                .CallsTo(f => f.Create(_p2, _p3))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Shall_the_LineFunctionFactory_only_being_called_two_times() {
            _line_function_factory
                .CallsTo(f => f.Create(A<Point>.Ignored, A<Point>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Test]
        public void Shall_the_power_calculator_return_the_correct_result_for_X_values_located_at_the_first_line([Values(0, 1, 2, 3, 4, 5, 6, 7, 8, 9)] long x) {
            _power_calculator.GetY(x).Should().Be(FIRST_LINE_RESULT);
        }

        [Test]
        public void Shall_the_power_calculator_return_the_correct_result_for_X_values_located_at_the_second_line([Values(10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20)] long x) {
            _power_calculator.GetY(x).Should().Be(SECOND_LINE_RESULT);
        }

    }
}