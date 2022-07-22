using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reviews.Service.Models;
using Reviews.Service.Repository;
using static Reviews.Service.Dtos;

namespace Reviews.Service.Controllers
{
    [Route("api/v1/reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IRepository<Review> _reviewsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(IRepository<Review> reviewsRepository, 
            IMapper mapper, ILogger<ReviewsController> logger)
        {
            _reviewsRepository = reviewsRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Route("get")]
        public async Task<ActionResult> Get([FromQuery] string reviewItem, int reviewItemId)
        {
            var reviews = await _reviewsRepository.GetAllAsync(rev => 
                rev.ReviewItem == reviewItem && rev.ReviewItemId == reviewItemId);

            if (reviews.Count == 0)
            {
                return NotFound("No Reviews");
            }

            ReviewsListDto reviewsListDto = new(reviews);
            return Ok(reviewsListDto);
        }

        [HttpGet]
        [Route("get-statistics")]
        public async Task<ActionResult> GetStatistics(string reviewItem, [FromQuery] int[] ids)
        {
            List<ReviewStatisticsDto> reviewStatisticsListDto = new();

            foreach(var id in ids)
            {
                var reviews = await _reviewsRepository.GetAllAsync(rev =>
                        rev.ReviewItem == reviewItem && rev.ReviewItemId == id);

                if (reviews.Count == 0)
                {
                    ReviewStatisticsDto reviewStatisticsDto = new(id, 0, "No Reviews");
                    reviewStatisticsListDto.Add(reviewStatisticsDto);
                }
                else
                {
                    double numberOfPositiveReviews = 0;
                    foreach (var r in reviews)
                    {
                        if (r.IsReviewItemLiked) { numberOfPositiveReviews++; }
                    }
                    double percentageOfPositiveReviews = (numberOfPositiveReviews / reviews.Count) * 100;
                    ReviewStatisticsDto reviewStatisticsDto = new(id, Math.Round(percentageOfPositiveReviews), $"{reviews.Count}");
                    reviewStatisticsListDto.Add(reviewStatisticsDto);
                }
            }
            
            return Ok(reviewStatisticsListDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(string id)
        {
            var review = await _reviewsRepository.GetAsync(rev => rev.Id == id);
            if(review is null)
            {
                return NotFound();
            }
            return Ok(review);
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> Post(CreateReviewDto createReviewDto)
        {
            var reviewModel = _mapper.Map<Review>(createReviewDto);
            reviewModel.Date = DateTime.UtcNow;

            var createdReview = await _reviewsRepository.CreateAsync(reviewModel);

            if(createdReview is null)
            {
                _logger.LogInformation($"{DateTime.UtcNow}.  Unable to create a review! CreatedReview is null!");
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to create a review!");
            }
            return CreatedAtAction(nameof(GetById), createdReview, new { id = createdReview.Id });
        }
    }
}
