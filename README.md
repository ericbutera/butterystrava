# butterystrava

Attempting to build an auto hide heart rate. Why? Because Strava refuses to add this privacy feature.

# Docs
https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-3.1&tabs=visual-studio-code
https://developers.strava.com/playground/#/Activities/updateActivityById

# Required Software
https://dotnet.microsoft.com/download/dotnet-core/3.1
https://docs.microsoft.com/en-us/dotnet/core/install/linux-ubuntu


## linux notes
- use tmux to run these concurrent:
  - (tab1) dotnet run
  - (tab2) butterystrava git:(master) curl -v -k https://localhost:5001/WeatherForecast
