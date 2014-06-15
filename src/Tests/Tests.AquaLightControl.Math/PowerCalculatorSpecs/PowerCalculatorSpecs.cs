using System.Collections;
using AquaLightControl;
using AquaLightControl.Math;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace Tests.AquaLightControl.Math.PowerCalculatorSpecs
{
    [TestFixture]
    public class If_the_calculator_gets_requests_with_valid_input_parameters : Spec {
        private PowerCalculator _calculator;

        protected override void EstablishContext() {
            _calculator = new PowerCalculator();
        }

        private static IEnumerable TestCases {
            get { 
                var config = new LightConfiguration {
                Start = 0,
                StartValue = 0,
                End = 10,
                EndValue = 10
            };
                yield return new TestCaseData(config, 1).Returns(1m);
                yield return new TestCaseData(config, 1.5m).Returns(1.5m);
                yield return new TestCaseData(config, 5).Returns(5m);
                yield return new TestCaseData(config, 10).Returns(10m);
            }
        }

        [Test,TestCaseSource("TestCases")]
        public decimal Shall_the_answer_be_correct(LightConfiguration config, long timepoint) {
            return _calculator.Get(config, timepoint);
        }
    }
}