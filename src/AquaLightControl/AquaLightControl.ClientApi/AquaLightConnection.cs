using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AquaLightControl.ClientApi.Annotations;
using RestSharp;

namespace AquaLightControl.ClientApi
{
    public class AquaLightConnection : IAquaLightConnection, INotifyPropertyChanged
    {
        private const string DEFAULT_BASE_URL = "http://raspberrypi/";
        private const string PONG = "pong";
        private readonly IRestClient _client;
        
        public event PropertyChangedEventHandler PropertyChanged;

        public string BaseUrl {
            get { 
                return _client.BaseUrl; 
            }
            set { 
                _client.BaseUrl = value;
                OnPropertyChanged();
            }
        }

        public AquaLightConnection() {
            _client = new RestClient(DEFAULT_BASE_URL);
        }

        public AquaLightConnection(string base_url) {
            _client = new RestClient(base_url);
        }

        public async Task Ping() {
            var request = new RestRequest("ping", Method.GET);

            var cancel = new CancellationTokenSource();
            var response = await _client.ExecuteTaskAsync(request, cancel.Token);

            if (response.StatusCode == HttpStatusCode.OK && response.Content == PONG) {
                return;
            }
            throw new Exception("Unbekannte Antwort: " + response.Content);
        }

        public async Task<IEnumerable<LedStripe>> GetAllStripes() {
            var request = new RestRequest("stripes", Method.GET);

            var cancel = new CancellationTokenSource();
            var response = await _client.ExecuteTaskAsync<LedStripe[]>(request, cancel.Token);

            return (response.StatusCode == HttpStatusCode.OK)
                ? response.Data
                : Enumerable.Empty<LedStripe>();
        }

        public async Task Save(LedStripe led_stripe) {
            if (ReferenceEquals(led_stripe, null)) {
                throw new ArgumentNullException("led_stripe");
            }

            if (led_stripe.Id == Guid.Empty) {
                throw new ArgumentException("Id required", "led_stripe");
            }

            var request = new RestRequest("stripes/{id}", Method.POST);
            request.AddUrlSegment("id", led_stripe.Id.ToString());
            request.RequestFormat = DataFormat.Json;
            request.AddBody(led_stripe);

            var cancel = new CancellationTokenSource();
            var response = await _client.ExecuteTaskAsync<LedStripe>(request, cancel.Token);

            if (response.StatusCode == HttpStatusCode.Created) {
                return;
            }

            throw new Exception("Unerwarteter Fehler beim Speichern: " + response.Content);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string property_name = null) {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(property_name));
            }
        }
    }
}