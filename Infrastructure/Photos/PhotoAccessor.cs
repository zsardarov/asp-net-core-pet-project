using System;
using System.IO;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Application.Photos;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Photos
{
    public class PhotoAccessor : IPhotoAccessor
    {
        private readonly Cloudinary _client;
        
        public PhotoAccessor(IOptions<CloudinarySettings> config)
        {
            var configValue = config.Value;
            var account = new Account(cloud: configValue.CloudName, apiKey: configValue.ApiKey,
                apiSecret: configValue.ApiSecret);
            _client = new Cloudinary(account);
        }
        
        public async Task<PhotoUploadResult> AddPhoto(IFormFile file)
        {
            if (file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill")
                };

                var uploadResult = await _client.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception(uploadResult.Error.Message);
                }

                return new PhotoUploadResult
                {
                    Url = uploadResult.SecureUrl.ToString(),
                    PublicId = uploadResult.PublicId
                };
            }

            return null;
        }

        public async Task<string> DeletePhoto(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var deleteResult = await _client.DestroyAsync(deleteParams);

            return deleteResult.Result == "ok" ? deleteResult.Result : null;
        }
    }
}