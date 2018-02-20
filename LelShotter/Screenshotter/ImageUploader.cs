using System;
using System.Drawing;
using System.Threading.Tasks;
using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using LelShotter.Models;
using LelShotter.Utils;

namespace LelShotter.Screenshotter
{
    public class ImageUploader
    {
        public async Task<string> UploadImageAsync(Bitmap image)
        {
            var client = new ImgurClient("[REDACTED]", "[REDACTED]");
            var endpoint = new ImageEndpoint(client);
            var bytes = ImageToByte(image);
            try
            {
                var response = await endpoint.UploadImageBinaryAsync(bytes);
                return response.Link;
            }
            catch (ImgurException ie)
            {
                Logger.Log(Level.Info, $"Exception occurred while uploading an image: {ie.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"Unrecognized error occurred during image upload: {ex.Message}");
                return null;
            }
        }
        private static byte[] ImageToByte(Image img)
        {
            var converter = new ImageConverter();
            return converter.ConvertTo(img, typeof(byte[])) as byte[];
        }
    }
}
