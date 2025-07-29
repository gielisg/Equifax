using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Linq;
using IdMatrixSoapLib.Models;
using Microsoft.Extensions.Logging;

namespace IdMatrixSoapLib
{
    public class SoapClient : IDisposable
    {
        private readonly string _username;
        private readonly string _password;
        private readonly string _endpoint;
        private readonly ILogger<SoapClient> _logger;
        private readonly HttpClient _httpClient;

        public SoapClient(string username, string password, string endpoint = "https://vedaxml.com/sy2/idmatrix-v4", ILogger<SoapClient> logger = null)
        {
            _username = username ?? throw new ArgumentNullException(nameof(username));
            _password = password ?? throw new ArgumentNullException(nameof(password));
            _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public async Task<IdMatrixResponse> SendRequestAsync(IdMatrixRequest request)
        {
            try
            {
                var soapXml = BuildSoapEnvelope(request);
                _logger?.LogDebug("SOAP Request: {SoapXml}", soapXml);

                var content = new StringContent(soapXml, Encoding.UTF8, "text/xml");
                content.Headers.Add("SOAPAction", "IdMatrixOperation");

                var response = await _httpClient.PostAsync(_endpoint, content);
                var responseXml = await response.Content.ReadAsStringAsync();
                
                _logger?.LogDebug("SOAP Response: {ResponseXml}", responseXml);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"SOAP request failed with status {response.StatusCode}: {responseXml}");
                }

                return ParseSoapResponse(responseXml);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error sending SOAP request");
                throw;
            }
        }

        public async Task<IdMatrixResponse> SendRequestAsync(string jsonInput)
        {
            var requestModel = JsonConvert.DeserializeObject<IdMatrixRequest>(jsonInput);
            return await SendRequestAsync(requestModel);
        }

        private string BuildSoapEnvelope(IdMatrixRequest model)
        {
            var soapEnvelope = $@"
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/""
                  xmlns:idm=""http://vedaxml.com/schemas/idmatrix-v4""
                  xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
  <soapenv:Header>
    <wsse:Security>
      <wsse:UsernameToken>
        <wsse:Username>{_username}</wsse:Username>
        <wsse:Password>{_password}</wsse:Password>
      </wsse:UsernameToken>
    </wsse:Security>
  </soapenv:Header>
  <soapenv:Body>
    <idm:IdMatrixRequest>
      <idm:client-reference>{model.ClientReference}</idm:client-reference>
      <idm:reason-for-enquiry>{model.Reason}</idm:reason-for-enquiry>
      <idm:given-name>{model.FirstGivenName}</idm:given-name>
      <idm:family-name>{model.FamilyName}</idm:family-name>
      <idm:other-given-name>{model.OtherGivenName}</idm:other-given-name>
      <idm:date-of-birth>{model.DateOfBirth:yyyy-MM-dd}</idm:date-of-birth>
      <idm:gender>{model.Gender}</idm:gender>
      {BuildAddressXml(model.Addresses?.CurrentAddress, "current")}
      {BuildAddressXml(model.Addresses?.PreviousAddress, "previous")}
      {BuildConsentsXml(model.Consents)}
    </idm:IdMatrixRequest>
  </soapenv:Body>
</soapenv:Envelope>";

            return soapEnvelope.Trim();
        }

        private string BuildAddressXml(Address address, string type)
        {
            if (address == null) return string.Empty;

            return $@"
      <idm:{type}-address>
        <idm:property>{address.Property}</idm:property>
        <idm:unit-number>{address.UnitNumber}</idm:unit-number>
        <idm:street-number>{address.StreetNumber}</idm:street-number>
        <idm:street-name>{address.StreetName}</idm:street-name>
        <idm:street-type>{address.StreetType}</idm:street-type>
        <idm:suburb>{address.Suburb}</idm:suburb>
        <idm:state>{address.State}</idm:state>
        <idm:postcode>{address.Postcode}</idm:postcode>
        <idm:country>{address.Country}</idm:country>
      </idm:{type}-address>";
        }

        private string BuildConsentsXml(ConsentInfo consents)
        {
            if (consents == null) return string.Empty;

            return $@"
      <idm:consents>
        <idm:veda-credit-bureau>{consents.VedaCreditBureau.ToString().ToLower()}</idm:veda-credit-bureau>
        <idm:drivers-licence>{consents.DriversLicence.ToString().ToLower()}</idm:drivers-licence>
        <idm:medicare>{consents.Medicare.ToString().ToLower()}</idm:medicare>
        <idm:australian-passport>{consents.AustralianPassport.ToString().ToLower()}</idm:australian-passport>
        <idm:visa-entitlement-verification-online>{consents.VisaEntitlementVerificationOnline.ToString().ToLower()}</idm:visa-entitlement-verification-online>
      </idm:consents>";
        }

        private IdMatrixResponse ParseSoapResponse(string xml)
        {
            try
            {
                var doc = XDocument.Parse(xml);
                var responseElement = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "IdMatrixResponse");
                
                if (responseElement == null)
                {
                    throw new InvalidOperationException("Invalid SOAP response format");
                }

                // Parse the response XML and map to IdMatrixResponse
                var response = new IdMatrixResponse
                {
                    MessageId = GetElementValue(responseElement, "message-id"),
                    ClientReference = GetElementValue(responseElement, "client-reference"),
                    OverallOutcome = GetElementValue(responseElement, "overall-outcome"),
                    VerificationOutcome = GetElementValue(responseElement, "verification-outcome"),
                    Status = "Completed"
                };

                if (decimal.TryParse(GetElementValue(responseElement, "total-points"), out var points))
                {
                    response.TotalPoints = points;
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error parsing SOAP response");
                throw new InvalidOperationException("Failed to parse SOAP response", ex);
            }
        }

        private string GetElementValue(XElement parent, string elementName)
        {
            return parent.Descendants().FirstOrDefault(x => x.Name.LocalName == elementName)?.Value;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
