#r "Microsoft.Azure.NotificationHubs"

using System.Net;
using System.Net.Http.Headers;
using Microsoft.Azure.NotificationHubs;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    // parse query parameter
    string message = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "message", true) == 0)
        .Value;

    string targetLanguage = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "langcode", true) == 0)
        .Value;

    string badgeValue = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "badgevalue", true) == 0)
        .Value;

    if (message == null)
    {
        // Get request body
        dynamic data = await req.Content.ReadAsAsync<object>();
        message = data?.message;

        targetLanguage = data?.langcode;
        badgeValue = data?.badgeValue;
    }

    if (string.IsNullOrWhiteSpace(message))
        return req.CreateResponse(HttpStatusCode.BadRequest);

    if (string.IsNullOrWhiteSpace(badgeValue))
        badgeValue = "0";

    // Define the notification hub.
     NotificationHubClient hub =
         NotificationHubClient.CreateClientFromConnectionString(
              #error insert your Azure Notification Hub namespace and access key with write permissions
             "Endpoint=sb://<your namespace>.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=<your access key>",
             "YourHubName");

    if (!string.IsNullOrWhiteSpace(targetLanguage))
        # error insert your translation API key https://azure.microsoft.com/nl-nl/services/cognitive-services/translator-text-api/
        message = await TranslateText(message, targetLanguage, "yourapikey");

    // Define an iOS alert  
    var iOSalert = 
        "{\"aps\":{\"alert\":\""+ message + "\", \"badge\":" + badgeValue + ", \"sound\":\"default\"},"
        + "\"inAppMessage\":\"" + message + "\"}";

    hub.SendAppleNativeNotificationAsync(iOSalert).Wait();

    // Define an Anroid alert.
    var androidAlert = "{\"data\":{\"message\": \"" + message + "\"}}";

    hub.SendGcmNativeNotificationAsync(androidAlert).Wait();

    return req.CreateResponse(HttpStatusCode.OK);
}

static async Task<string> TranslateText(string inputText, string language, string accessToken)
{
    string url = "http://api.microsofttranslator.com/v2/Http.svc/Translate";
    string query = $"?text={System.Net.WebUtility.UrlEncode(inputText)}&to={language}&contentType=text/plain";

    using (var client = new HttpClient())
    {
        // Set API key
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", accessToken); 
        
        var response = await client.GetAsync(url + query);
        var result = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return "Error: " + result;

        System.Xml.XmlDocument xmlResponse = new System.Xml.XmlDocument();
        xmlResponse.LoadXml(result);

        // Return the translation
        return xmlResponse.InnerText;
    }
}
