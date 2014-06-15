using AquaLightControl.Configuration;
using Raspberry.IO.Components.Controllers.Tlc59711;
using Raspberry.IO.SerialPeripheralInterface;

namespace AquaLightControl.Service.Devices
{
    internal sealed class ConnectionFactory : IConnectionFactory {
        private const string DEVICE_PATH_KEY = "DeviceFilePath";
        private const string DEVICE_SPEED_KEY = "DeviceSpeed";
        private const string DEVICE_DELAY_KEY = "DeviceDelay";
        private const string DEVICE_COUNT_KEY = "NumberOfDevices";
        
        private const int BITS_PER_WORD = 8;
        private const SpiMode SPI_MODE = SpiMode.Mode0;

        private readonly IConfigProvider _config_provider;

        public ConnectionFactory(IConfigProvider config_provider) {
            _config_provider = config_provider;
        }

        public ITlc59711Connection Establish() {
            var device_path = _config_provider.GetKey(DEVICE_PATH_KEY);

            var device_speed_string = _config_provider.GetKey(DEVICE_SPEED_KEY);
            var device_speed = uint.Parse(device_speed_string);

            var device_delay_string = _config_provider.GetKey(DEVICE_DELAY_KEY);
            var device_delay = ushort.Parse(device_delay_string);

            var device_count_string = _config_provider.GetKey(DEVICE_COUNT_KEY);
            var device_count = int.Parse(device_count_string);

            var spi_connection = new NativeSpiConnection(device_path);
            spi_connection.SetBitsPerWord(BITS_PER_WORD);
            spi_connection.SetSpiMode(SPI_MODE);
            spi_connection.SetMaxSpeed(device_speed);
            spi_connection.SetDelay(device_delay);

            var device_connection = new Tlc59711Connection(spi_connection, false, device_count);
            return device_connection;
        }
    }
}