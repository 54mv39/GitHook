using GitHookReciever.ServiceAdapters;
using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitHookReciever.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HookController : Controller
    {
        private IGoogDriveServiceAdapter _googDriveServiceAdapter;
        public HookController(IGoogDriveServiceAdapter googDriveServiceAdapter)
        {
            _googDriveServiceAdapter = googDriveServiceAdapter;
        }

        [HttpGet]
        [GoogleScopedAuthorize(DriveService.ScopeConstants.Drive)]
        [GoogleScopedAuthorize(DriveService.ScopeConstants.DriveScripts)]
        public async Task<IActionResult> GetAuthorization([FromServices] IGoogleAuthProvider auth)
        {
            await _googDriveServiceAdapter.GetAuthorizationAsync(auth);
            return Ok("Authentication Success");
        }

        [HttpGet]
        public async Task<IActionResult> DriveFileList()
        {
            return Ok(await _googDriveServiceAdapter.DriveFileListAsync());
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PushFiles(List<string> FileList)
        {
            var resp = await _googDriveServiceAdapter.PushFilesAsync(FileList);
            return Ok(resp.IsSuccessStatusCode? "Success": "Failed");
        }
    }
}
