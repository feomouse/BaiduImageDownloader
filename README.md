# BaiduImageDownloader
download baidu image using dotnet core, jquery, restsharp, newtonsoft json in mvc pattern
### A sample to download image from baidu image,you can choose the topic and the number of pic you want to download;but the default folder
is set to `d:/baiduimg`, and you can change it in your own way;
## Tech
### 1.Dotnet Core: 
      dotnet core is open source, creat by microsoft term, and not base on .net framework; you can creat a console app, web app, and so on;
      I think the main purpose of this thing comes out is to create a ablity to create web app without the .net framework,which is only 
      based on windows;it give us more choice to build cross platform web app, reduce coupling, and face the modern web tech;My app is
      base on this, using mvc way, a controller, a view, and a web api; The main logic is in the web api call Download, which in the 
      SearchController class;
### 2.RestSharp: 
      Simple .NET REST Client;i use it to send a requset to server and get json data and image back which i need;
### 3.The way how to spider image from baidu image: 
      To get the pic,the first step is send a format url with some specify query string, then server send json data back,the         inside logic is the json data, which give me the raw images url in different server;but baidu encryption it, so we have       to decrypt it, and use restsharp to get the real image url;
### 4.jquery as frontend tech
      How the frontend and backend interact in mvc? Haha,jquery ajax is a perfect way; using $.ajax to post a request in a
      specify way,send data to the controller, controller process the.backend logic, then send back the response to jquery
      ajax success callback method, which contain the response data:)
