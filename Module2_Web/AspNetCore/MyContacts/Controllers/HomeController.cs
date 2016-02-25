﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;
using System.Security.Claims;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using MyContacts.Utils;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace MyContacts.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public async Task<IActionResult> Index()
        {
            JArray jsonArray = null;

            // Get access token for calling into Microsoft Graph
            string userObjectId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(i => i.Type == SettingsHelper.ObjectIdentifierKey).Value;
            ClientCredential clientCredential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.ClientSecrent);
            UserIdentifier userIdentifier = new UserIdentifier(userObjectId, UserIdentifierType.UniqueId);
            AuthenticationContext authContext = new AuthenticationContext(SettingsHelper.Authority, new SessionTokenCache(userObjectId, HttpContext));
            var token = await authContext.AcquireTokenSilentAsync(SettingsHelper.GraphResourceId, clientCredential, userIdentifier);

            // Use the token to call Microsoft Graph
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.AccessToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            using (var response = await client.GetAsync(SettingsHelper.GraphResourceId + "/v1.0/me/contacts"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    JObject jObj = JObject.Parse(json);
                    jsonArray = jObj.Value<JArray>("value");
                }
            }

            return View(jsonArray);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
