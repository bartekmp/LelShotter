using System.Drawing;
using System.Threading.Tasks;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;

namespace LelShotter
{
    public class ImageUploader
    {
        public async Task<string> UploadImageAsync(Bitmap image)
        {
            var client = new ImgurClient("[REDACTED]", "[REDACTED]");
            var endpoint = new ImageEndpoint(client);
            var bytes = ImageToByte(image);
            var response = await endpoint.UploadImageBinaryAsync(bytes);
            return response.Link;
        }
        private static byte[] ImageToByte(Image img)
        {
            var converter = new ImageConverter();
            return converter.ConvertTo(img, typeof(byte[])) as byte[];
        }
    }
}
