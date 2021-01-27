# ADFScanner
A scanning service for serving web clients with requests for documents from scenner devices.</br>
Supported OS: </br>
  -Windows 10</br>
  -Windows 10x</br>
  </br>
  Install the application and restart your computer. The application is a background service and listens for requests from WebSocket.
  The key work is "scan". To receive a path to the pdf of the scanned document, you need to send a message "scan" to "localhost:10902", it opens a Form for scaning configurations.
