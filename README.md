# Githook
GitHook

A git pre-commit hook and reciever server used with google drive api to deploy google appscript code directly to google server.<br /> 

-> 1. File: "pre-commit" -> hook code contains diff file prep and use curl to push to githook reciever.<br /> 
-> 2. "GitHookReciever" -> the dotnet core 5 based webapi acting as a reciever for git hook. Following configurable properties under "appsettings.json":<br /> 
    &emsp;-> a. ListenPort - port to listen on to.<br /> 
    &emsp;-> b. ProjBaseDir - base project directory to pickup google app script files from.<br /> 
    &emsp;-> c. GoogleDriveUploadURI - google drive upload uri. - provided by google<br /> 
    &emsp;-> d. ClientId - google oauth client ID for accessing drive api.<br /> 
    &emsp;-> e. ClientSecret - google oauth client secret for accessing drive api.<br /> 
