using System;
using System.Windows.Media;
using AquaLightControl.ClientApi.Annotations;

namespace AquaLightControl.Gui.Model
{
    public sealed class LedStripeModel 
    {
        private readonly LedStripe _led_stripe;

        public LedStripeModel([NotNull] LedStripe led_stripe) {
            if (ReferenceEquals(led_stripe, null)) {
                throw new ArgumentNullException("led_stripe");
            }

            _led_stripe = led_stripe;
        }

        public LedStripe Item {
            get { return _led_stripe; }
        }

        public Brush Color {
            get { return new SolidColorBrush(System.Windows.Media.Color.FromRgb(_led_stripe.Color.Red, _led_stripe.Color.Green, _led_stripe.Color.Blue)); }
        }

        public string Name {
            get { return _led_stripe.Name; }
        }

        public int DeviceNumber {
            get { return _led_stripe.DeviceNumber; }
        }

        public int Channel {
            get { return _led_stripe.ChannelNumber; }
        }
    }
}