using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OAHub.Storage.Data;
using OAHub.Storage.Models;

namespace OAHub.Storage.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly StorageDbContext _context;

        public FilesController(StorageDbContext context)
        {
            _context = context;
        }
    }
}