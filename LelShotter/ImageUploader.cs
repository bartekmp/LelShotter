using System.Drawing;
using System.Threading.Tasks;
using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using LelShotter.Utils;

namespace LelShotter
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
                Logger.LogError($"Exception occurred while uploading an image: {ie.Message}");
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
