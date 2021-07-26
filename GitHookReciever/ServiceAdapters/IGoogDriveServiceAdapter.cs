using Google.Apis.Auth.AspNetCore3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GitHookReciever.ServiceAdapters
{
    public interface IGoogDriveServiceAdapter
    {
        Task<List<string>> DriveFileListAsync();
        Task<HttpResponseMessage> PushFilesAsync(List<string> FilesList);
        Task GetAuthorizationAsync(IGoogleAuthProvider auth);
    }
}
