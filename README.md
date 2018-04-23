# PushFunction
Boilerplate code for an Azure Function that sends push notifications to iOS and Android.
Notifications are sent through a Azure Notification Hub you have to host yourself.

## Configuration
Insert the [Notification Hub connection string and hub name](https://github.com/jfversluis/PushFunction/blob/master/Send/run.csx#L44) to start sending.
As a bonus, notifications can be translated through [Azure Translator Text APIs](https://azure.microsoft.com/nl-nl/services/cognitive-services/translator-text-api/). You will also need to [insert a key](https://github.com/jfversluis/PushFunction/blob/master/Send/run.csx#L49) for that.

## Usage

Supports GET and POST with these parameters:

- `message` (required): The message you would like to send as a push notification
- `langcode` (optional): Two-letter language code to translate the text to
- `badgevalue` (optional, iOS only): numeric value for a badge on the iOS app icon

Sample GET request: https://yourhost.azurewebsites.net/api/Send?message=This%20is%20a%20nice%20test%20message&langcode=nl&badgevalue=1337

Sample POST request:

https://yourhost.azurewebsites.net/api/Send

    {
       "message": "This is a nice test message",
       "langcode": "nl",
       "badgevalue": "1337"
    }
