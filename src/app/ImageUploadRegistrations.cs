using System;
using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using Nancy;
using Nancy.Bootstrapper;
using wikibus.images.Cloudinary;
using Wikibus.Sources.Images;

namespace Brochures.Wikibus.Org
{
    public class ImageUploadRegistrations : Registrations
    {
        public ImageUploadRegistrations(ITypeCatalog typeCatalog, IConfiguration configuration)
            : base(typeCatalog)
        {
            if (configuration["cloudinary:name"] == null)
            {
                this.Register<IImageStorage>(new FakeImageStore());
                return;
            }

            this.Register<IImageStorage>(typeof(CloudinaryImagesStore));
                var account = new Account(
                    configuration["cloudinary:name"],
                    configuration["cloudinary:key"],
                    configuration["cloudinary:secret"]);

                this.Register(new Cloudinary(account));
        }
    }
}
