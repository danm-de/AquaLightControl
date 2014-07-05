using System;
using System.Collections;
using AquaLightControl.Service.Clock;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
using NUnit.Framework;



// ReSharper disable InconsistentNaming
namespace Tests.AquaLightControl.Service.Clock.ClockSpecs
{
    [TestFixture]
    public class If_the_ticks_for_a_specific_time_of_day_are_requested : Spec {
        private static IEnumerable TestCases {
            get { 
                yield return new TestCaseData(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(1))).Returns(0);
                yield return new TestCaseData(new DateTimeOffset(2000, 1, 1, 8, 0, 0, TimeSpan.FromHours(1))).Returns(TimeSpan.FromHours(8).Ticks);
                yield return new TestCaseData(new DateTimeOffset(2000, 1, 1, 8, 30, 0, TimeSpan.FromHours(1))).Returns(TimeSpan.FromHours(8.5).Ticks);
                yield return new TestCaseData(new DateTimeOffset(2000, 1, 1, 23, 0, 0, TimeSpan.FromHours(1))).Returns(TimeSpan.FromHours(23).Ticks);
                
                // switch from winter to summer time (day light savings are currently not supported)
                yield return new TestCaseData(new DateTimeOffset(2014, 3, 30, 1, 0, 0, TimeSpan.FromHours(1))).Returns(TimeSpan.FromHours(1).Ticks);
                yield return new TestCaseData(new DateTimeOffset(2014, 3, 30, 2, 0, 0, TimeSpan.FromHours(1))).Returns(TimeSpan.FromHours(2).Ticks);
                yield return new TestCaseData(new DateTimeOffset(2014, 3, 30, 2, 0, 0, TimeSpan.FromHours(2))).Returns(TimeSpan.FromHours(2).Ticks);
                yield return new TestCaseData(new DateTimeOffset(2014, 3, 30, 23, 59, 59, TimeSpan.FromHours(2))).Returns(TimeSpan.FromHours(23).Ticks + TimeSpan.FromMinutes(59).Ticks + TimeSpan.FromSeconds(59).Ticks);

                // switch from summer to winter time (day light savings are currently not supported)
                yield return new TestCaseData(new DateTimeOffset(2014, 10, 26, 1, 0, 0, TimeSpan.FromHours(2))).Returns(TimeSpan.FromHours(1).Ticks);
                yield return new TestCaseData(new DateTimeOffset(2014, 10, 26, 3, 0, 0, TimeSpan.FromHours(1))).Returns(TimeSpan.FromHours(3).Ticks);
                yield return new TestCaseData(new DateTimeOffset(2014, 10, 26, 23, 59, 59, TimeSpan.FromHours(1))).Returns(TimeSpan.FromHours(23).Ticks + TimeSpan.FromMinutes(59).Ticks + TimeSpan.FromSeconds(59).Ticks);
            }
        }

        [Test,TestCaseSource("TestCases")]
        public long Shall_the_result_be_correct(DateTimeOffset test_time) {
            var reference_clock = A.Fake<IReferenceClock>();
            reference_clock.CallsTo(c => c.GetLocalTime()).Returns(test_time);

            var clock = new global::AquaLightControl.Service.Clock.Clock(reference_clock);

            return clock.GetTicksForTimeOfDay();
        }
    }
    
}