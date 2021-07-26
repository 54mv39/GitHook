using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GitHookReciever.ServiceAdapters
{
    public class GoogDriveServiceAdapter: IGoogDriveServiceAdapter
    {
        private DriveService _driveService;
        private IConfiguration _config;

        public GoogDriveServiceAdapter(IConfiguration configuration)
        {
            _config = configuration;
        }
        public async Task GetAuthorizationAsync([FromServices] IGoogleAuthProvider auth)
        {
            GoogleCredential cred = await auth.GetCredentialAsync();
            _driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = cred
            });
        }
        public async Task<List<string>> DriveFileListAsync()
        {            
            var files = await _driveService.Files.List().ExecuteAsync();
            var fileNames = files.Files.Select(x => x.Name).ToList();
            return fileNames;
        }

        public async Task<HttpResponseMessage> PushFilesAsync(List<string> FilesList)
        {
            FilesResource.ListRequest query = _driveService.Files.List();
            query.Q = "mimeType='application/vnd.google-apps.script'";
            query.Fields = "*";
            var files = await query.ExecuteAsync();
            var murfAddOnContent = await _driveService.HttpClient.GetAsync(files.Files[0].ExportLinks.Values.ToList()[0]);
            var contentBuff = await murfAddOnContent.Content.ReadAsStringAsync();
            var content = JsonConvert.DeserializeObject<AddOnProjFiles>(contentBuff);

            FilesList.ForEach(file => 
            {
                var fileNameFrontList = file.Split('.').ToList();
                fileNameFrontList.RemoveAt(fileNameFrontList.Count() - 1);
                var fileNameFront = String.Join('.', fileNameFrontList);
                var exisitingFile = content.Files.Where(r => r.Name == fileNameFront).SingleOrDefault();
                content.Files.RemoveAt(content.Files.FindIndex(r => r.Name == fileNameFront));
                try
                {
                    var contentToCommit = File.ReadAllText(_config.GetSection("ProjBaseDir").Value + file);
                    exisitingFile.Source = contentToCommit;
                    content.Files.Add(exisitingFile);
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    throw;
                }
            });
            //var body = new Google.Apis.Drive.v3.Data.File();
            //body.Name = files.Files[0].Name;
            //byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(content, new JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() }));
            //var updateReq = _driveService.Files.Update(body,
            //    files.Files[0].Id,
            //    new MemoryStream(byteArray),
            //    "application/vnd.google-apps.script+json");
            //var updateResp = await updateReq.UploadAsync();
            var stringContent = new StringContent(JsonConvert.SerializeObject(content, new JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() }), Encoding.UTF8, "application/vnd.google-apps.script+json");
            var tes = await _driveService.HttpClient.PutAsync(_config.GetSection("GoogleDriveUploadURI").Value + files.Files[0].Id + "?uploadType=media", stringContent);
            return tes;
        }
    }

    class AddOnProjFiles
    {
        public List<HookFile> Files { get; set; } = new List<HookFile>();
    }

    class HookFile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Source { get; set; }
    }
}
