﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using MovieAPI.Helpers;
using MovieAPI.Models;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly MovieAPIContext _context;
        private IConfiguration _configuration;

        public MovieController(MovieAPIContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Movie
        [HttpGet]
        public IEnumerable<MovieItem> GetMovieItem()
        {
            return _context.MovieItem;
        }

        // GET: api/Movie/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var movieItem = await _context.MovieItem.FindAsync(id);

            if (movieItem == null)
            {
                return NotFound();
            }

            return Ok(movieItem);
        }

        // PUT: api/Movie/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovieItem([FromRoute] int id, [FromBody] MovieItem movieItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != movieItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(movieItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Movie
        [HttpPost]
        public async Task<IActionResult> PostMovieItem([FromBody] MovieItem movieItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.MovieItem.Add(movieItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovieItem", new { id = movieItem.Id }, movieItem);
        }

        // DELETE: api/Movie/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovieItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var movieItem = await _context.MovieItem.FindAsync(id);
            if (movieItem == null)
            {
                return NotFound();
            }

            _context.MovieItem.Remove(movieItem);
            await _context.SaveChangesAsync();

            return Ok(movieItem);
        }

        // GET: api/Meme/Tags

        [HttpGet]
        [Route("title")]
        public async Task<List<MovieItem>> GetTitleItem([FromQuery] string tags)
        {
            var movies = from m in _context.MovieItem
                        select m; //get all the memes


            if (!String.IsNullOrEmpty(tags)) //make sure user gave a tag to search
            {
                movies = movies.Where(s => s.Title.ToLower().Equals(tags.ToLower())); // find the entries with the search tag and reassign
            }

            var returned = await movies.ToListAsync(); //return the memes

            return returned;
        }

        [HttpPost, Route("upload")]
        public async Task<IActionResult> UploadFile([FromForm]MovieImage movie)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }
            try
            {
                using (var stream = movie.Image.OpenReadStream())
                {
                    var cloudBlock = await UploadToBlob(movie.Image.FileName, null, stream);
                    //// Retrieve the filename of the file you have uploaded
                    //var filename = provider.FileData.FirstOrDefault()?.LocalFileName;
                    if (string.IsNullOrEmpty(cloudBlock.StorageUri.ToString()))
                    {
                        return BadRequest("An error has occured while uploading your file. Please try again.");
                    }

                    MovieItem movieItem = new MovieItem();
                    movieItem.Title = movie.Title;
                    movieItem.Playtime = movie.Playtime;
                    movieItem.Genre = movie.Genre;
                    movieItem.Rating = movie.Rating;
                    movieItem.Trailer = movie.Trailer;

                    System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                    movieItem.Height = image.Height.ToString();
                    movieItem.Width = image.Width.ToString();
                    movieItem.Url = cloudBlock.SnapshotQualifiedUri.AbsoluteUri;
                    movieItem.Uploaded = DateTime.Now.ToString();

                    _context.MovieItem.Add(movieItem);
                    await _context.SaveChangesAsync();

                    return Ok($"File: {movie.Title} has successfully uploaded");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error has occured. Details: {ex.Message}");
            }


        }

        private async Task<CloudBlockBlob> UploadToBlob(string filename, byte[] imageBuffer = null, System.IO.Stream stream = null)
        {

            var accountName = _configuration["AzureBlob:name"];
            var accountKey = _configuration["AzureBlob:key"]; ;
            var storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer imagesContainer = blobClient.GetContainerReference("images");

            string storageConnectionString = _configuration["AzureBlob:connectionString"];

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    // Generate a new filename for every new blob
                    var fileName = Guid.NewGuid().ToString();
                    fileName += GetFileExtention(filename);

                    // Get a reference to the blob address, then upload the file to the blob.
                    CloudBlockBlob cloudBlockBlob = imagesContainer.GetBlockBlobReference(fileName);

                    if (stream != null)
                    {
                        await cloudBlockBlob.UploadFromStreamAsync(stream);
                    }
                    else
                    {
                        return new CloudBlockBlob(new Uri(""));
                    }

                    return cloudBlockBlob;
                }
                catch (StorageException ex)
                {
                    return new CloudBlockBlob(new Uri(""));
                }
            }
            else
            {
                return new CloudBlockBlob(new Uri(""));
            }

        }

        private string GetFileExtention(string fileName)
        {
            if (!fileName.Contains("."))
                return ""; //no extension
            else
            {
                var extentionList = fileName.Split('.');
                return "." + extentionList.Last(); //assumes last item is the extension 
            }
        }

        private bool MovieItemExists(int id)
        {
            return _context.MovieItem.Any(e => e.Id == id);
        }
    }
}