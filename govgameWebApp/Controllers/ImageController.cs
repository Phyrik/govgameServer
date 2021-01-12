using FirebaseAdmin.Auth;
using govgameWebApp.Helpers;
using govgameWebApp.Models.MongoDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.IO;

namespace govgameWebApp.Controllers
{
    public class ImageController : Controller
    {
        private readonly IWebHostEnvironment env;

        private readonly int countryMapCroppedMargin = 20;

        public ImageController(IWebHostEnvironment hostingEnvironment)
        {
            this.env = hostingEnvironment;
        }

        public IActionResult OwnCountryMap()
        {
            string authSessionCookie = Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie);

            if (userLoggedIn)
            {
                FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
                string firebaseUid = firebaseToken.Uid;

                PublicUser publicUser = MongoDBHelper.GetPublicUser(firebaseUid);

                Bitmap worldMap = new Bitmap(Path.Combine(env.WebRootPath, "images", "maps", "world map.png"));

                Location[] countryLocations = MongoDBHelper.GetCountrysLocations(publicUser.CountryId);
                GlobalLocationIdentifier[] countryGlobalLocationIdentifiers = LocationHelper.GetGLIsFromLocations(countryLocations);

                Bitmap countryMap = (Bitmap)worldMap.Clone();
                Bitmap countryOverlay = new Bitmap(MapHelper.worldMapWidth, MapHelper.worldMapHeight);
                foreach (GlobalLocationIdentifier globalLocationIdentifier in countryGlobalLocationIdentifiers)
                {
                    countryOverlay.SetPixel(globalLocationIdentifier.GlobalX, globalLocationIdentifier.GlobalY, MapColourHelper.ApplyOverlayAlpha(MapColourHelper.Colours.OwnCountry));
                }

                Graphics finalImageGraphics = Graphics.FromImage(countryMap);
                finalImageGraphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                finalImageGraphics.DrawImage(countryOverlay, 0, 0);

                MemoryStream imageStream = new MemoryStream();
                countryMap.Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);
                imageStream.Seek(0, SeekOrigin.Begin);

                return File(imageStream, "image/png");
            }
            else
            {
                return Content("invalid authSession cookie");
            }
        }

        public IActionResult OwnCountryMapCropped()
        {
            string authSessionCookie = Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie);

            if (userLoggedIn)
            {
                FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
                string firebaseUid = firebaseToken.Uid;

                PublicUser publicUser = MongoDBHelper.GetPublicUser(firebaseUid);

                Bitmap worldMap = new Bitmap(Path.Combine(env.WebRootPath, "images", "maps", "world map.png"));

                Location[] countryLocations = MongoDBHelper.GetCountrysLocations(publicUser.CountryId);
                GlobalLocationIdentifier[] countryGlobalLocationIdentifiers = LocationHelper.GetGLIsFromLocations(countryLocations);
                LocationsDimensions countryLocationsDimensions = LocationHelper.GetDimensionsOfLocations(countryGlobalLocationIdentifiers);

                Bitmap countryMapCropped = ImageHelper.CropBitmap(worldMap, new Rectangle(countryLocationsDimensions.TopLeft.GlobalX - countryMapCroppedMargin, countryLocationsDimensions.TopLeft.GlobalY - countryMapCroppedMargin, countryLocationsDimensions.Width + (countryMapCroppedMargin * 2), countryLocationsDimensions.Height + (countryMapCroppedMargin * 2)));
                Bitmap countryOverlay = new Bitmap(countryLocationsDimensions.Width + (countryMapCroppedMargin * 2), countryLocationsDimensions.Height + (countryMapCroppedMargin * 2));
                foreach (GlobalLocationIdentifier globalLocationIdentifier in countryGlobalLocationIdentifiers)
                {
                    countryOverlay.SetPixel(globalLocationIdentifier.GlobalX - countryLocationsDimensions.TopLeft.GlobalX + countryMapCroppedMargin, globalLocationIdentifier.GlobalY - countryLocationsDimensions.TopLeft.GlobalY + countryMapCroppedMargin, MapColourHelper.ApplyOverlayAlpha(MapColourHelper.Colours.OwnCountry));
                }

                Graphics finalImageGraphics = Graphics.FromImage(countryMapCropped);
                finalImageGraphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                finalImageGraphics.DrawImage(countryOverlay, 0, 0);

                MemoryStream imageStream = new MemoryStream();
                countryMapCropped.Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);
                imageStream.Seek(0, SeekOrigin.Begin);

                return File(imageStream, "image/png");
            }
            else
            {
                return Content("invalid authSession cookie");
            }
        }
    }
}
