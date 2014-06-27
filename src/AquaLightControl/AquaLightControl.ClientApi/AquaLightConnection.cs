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
        private const string DEFAULT_REMOTE_ENDPOINT = "http://raspberrypi:8080/";
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
            _client = new RestClient(DEFAULT_REMOTE_ENDPOINT);
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

        public IEnumerable<Device> GetAllDevices() {
            var request = new RestRequest("devices", Method.GET);

            var response = _client.Execute<List<Device>>(request);

            return (response.StatusCode == HttpStatusCode.OK)
                ? response.Data
                : Enumerable.Empty<Device>();
        }

        public void Save(Device device) {
            if (ReferenceEquals(device, null)) {
                throw new ArgumentNullException("device");
            }

            if (device.Id == Guid.Empty) {
                throw new ArgumentException("Id required", "device");
            }

            var request = new RestRequest("devices", Method.POST) {
                RequestFormat = DataFormat.Json
            };
            request.AddBody(device);

            var response = _client.Execute<Device>(request);

            ThrowOnError(response);

            if (response.StatusCode == HttpStatusCode.Created) {
                return;
            }
            
            throw new Exception(string.Format("Unerwarteter Fehler beim Speichern ({0}): {1}", response.StatusCode, response.Content));
        }

        public void Delete(Guid device_id) {
            if (device_id == Guid.Empty) {
                throw new ArgumentException("Id required", "device_id");
            }

            var request = new RestRequest("devices/{id}", Method.DELETE);
            request.AddUrlSegment("id", device_id.ToString());

            var response = _client.Execute<Device>(request);

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