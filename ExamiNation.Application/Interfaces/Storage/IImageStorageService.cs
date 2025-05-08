using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.Interfaces.Storage
{
    public interface IImageStorageService
    {
        /// <summary>
        /// Saves an image and returns its relative URL.
        /// </summary>
        /// <param name="image">Image file</param>
        /// <param name="category">Folder or category (e.g. "tests", "questions")</param>
        /// <returns>Relative URL of the image</returns>
        Task<string> SaveImageAsync(IFormFile image, string category);
    }
}
