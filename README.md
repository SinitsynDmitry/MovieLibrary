#  Movie Library.

The system consists of three parts :

	WPF application MVVM pattern (Front-end)
 
	Windows service (Back-end)
 
	RabbitMQ is used for communication between Front-end and Back-end

 ## Settings.

File: appsettings.json
```
  {
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "QueueName": "data_queue"
  }
```
