using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
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

        public void Ping() {
            var request = new RestRequest("ping", Method.GET);

            var response = _client.Execute(request);

            ThrowOnError(response);

            if (response.StatusCode == HttpStatusCode.OK && response.Content == PONG) {
                return;
            }
            
            throw new Exception(string.Format("Unbekannte Antwort ({0}): {1}", response.StatusCode, response.Content));
        }

        public IEnumerable<LedStripe> GetAllStripes() {
            var request = new RestRequest("stripes", Method.GET);

            var response = _client.Execute<List<LedStripe>>(request);

            return (response.StatusCode == HttpStatusCode.OK)
                ? response.Data
                : Enumerable.Empty<LedStripe>();
        }

        public void Save(LedStripe led_stripe) {
            if (ReferenceEquals(led_stripe, null)) {
                throw new ArgumentNullException("led_stripe");
            }

            if (led_stripe.Id == Guid.Empty) {
                throw new ArgumentException("Id required", "led_stripe");
            }

            var request = new RestRequest("stripes", Method.POST) {
                RequestFormat = DataFormat.Json
            };
            request.AddBody(led_stripe);

            var response = _client.Execute<LedStripe>(request);

            ThrowOnError(response);

            if (response.StatusCode == HttpStatusCode.Created) {
                return;
            }
            
            throw new Exception(string.Format("Unerwarteter Fehler beim Speichern ({0}): {1}", response.StatusCode, response.Content));
        }

        public void Delete(Guid led_stripe_id) {
            if (led_stripe_id == Guid.Empty) {
                throw new ArgumentException("Id required", "led_stripe_id");
            }

            var request = new RestRequest("stripes/{id}", Method.DELETE);
            request.AddUrlSegment("id", led_stripe_id.ToString());

            var response = _client.Execute<LedStripe>(request);

            ThrowOnError(response);

            if (response.StatusCode == HttpStatusCode.OK) {
                return;
            }

            throw new Exception(string.Format("Unerwarteter Fehler beim Löschen ({0}): {1}", response.StatusCode, response.Content));
        }

        private static void ThrowOnError(IRestResponse response) {
            if (ReferenceEquals(response, null)) {
                throw new Exception("Unerwarteter Fehler: keine Antwort erhalten.");
            }

            if (!ReferenceEquals(response.ErrorException, null)) {
                throw response.ErrorException;
            }

            if (!ReferenceEquals(response.ErrorMessage, null)) {
                throw new Exception(response.ErrorMessage);
            }
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