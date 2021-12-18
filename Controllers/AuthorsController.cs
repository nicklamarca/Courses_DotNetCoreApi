using AutoMapper;
using CourseLibrary.Api.Helpers;
using CourseLibrary.Api.Models;
using CourseLibrary.Api.ResourceParameters;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public AuthorsController(ICourseLibraryRepository courseLibraryRepository,
                                 IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException(nameof(courseLibraryRepository));

            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet()]
        //[HttpHead] this is to get the Header information with no response payload
        //public ActionResult<IEnumerable<AuthorDto>> GetAuthors([FromQuery] string mainCategory, //by default we DON'T want to apply the mainCategory filter
        //                                                       [FromQuery] string searchQuery) 
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors([FromQuery] AuthorsResourceParameters authorsResourceParameters)
        {
            //var authorsFromRepo = _courseLibraryRepository.GetAuthors(mainCategory, //the overload handles the optional filter 
            //                                                          searchQuery); 
            var authorsFromRepo = _courseLibraryRepository.GetAuthors(authorsResourceParameters);

            //use auto mapper here, this code is prone to errors, its not decoupled

            //var authors = new List<AuthorDto>();
            //foreach (var author in authorsFromRepo)
            //{
            //    authors.Add(new AuthorDto()
            //    {
            //        Id = author.Id,
            //        Name = $"{author.FirstName} {author.LastName}",
            //        MainCategory = author.MainCategory,
            //        Age = author.DateOfBirth.GetCurrentAge()
            //    });
            //}

            var mappedAuthors = _mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);

            return Ok(mappedAuthors);
        }

        [HttpGet("{authorId:guid}", Name = "GetAuthor")]
        public ActionResult<AuthorDto> GetAuthor(Guid authorId)
        {
            var authorsFromRepo = _courseLibraryRepository.GetAuthor(authorId);

            if (authorsFromRepo == null)
            {
                return NotFound();
            }

            var mappedAuthor = _mapper.Map<AuthorDto>(authorsFromRepo);

            return Ok(mappedAuthor);
        }

        [HttpPost]
        public ActionResult<AuthorDto> CreateAuthor(AuthorForCreationDto author)
        {
            //HttpPost attribute handles this
            //if(author == null)
            //{
            //    return BadRequest();
            //}

            var authorEntity = _mapper.Map<Author>(author);
            _courseLibraryRepository.AddAuthor(authorEntity);
            _courseLibraryRepository.Save();

            var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);

            return CreatedAtRoute("GetAuthor",
                                  new { authorId = authorToReturn.Id },
                                  authorToReturn);
        }
    }
}
