using AutoMapper;
using Grpc.Core;
using Image.Grpc.Service.Models;
using Image.Grpc.Service.Repository;

namespace Image.Grpc.Service.Services
{
    public class ImagesService : Images.ImagesBase
    {
        private readonly ILogger<ImagesService> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<FoodImage> _foodImageRepository;

        public ImagesService(ILogger<ImagesService> logger, IMapper mapper, IRepository<FoodImage> foodImageRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _foodImageRepository = foodImageRepository;
        }

        public override async Task<PostFoodImageReply> PostFoodImage(PostFoodImageRequest request, ServerCallContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var foodImage = _mapper.Map<FoodImage>(request);
            var createdImage = await _foodImageRepository.CreateAsync(foodImage);
            if(createdImage.Id is null)
            {
                _logger.LogError("Post Food Image: Failure");
                return new PostFoodImageReply { Message = "Failure" };
            }
            _logger.LogInformation("Post Food Image: Success");
            return new PostFoodImageReply { Message = "Success" };
        }
    }
}
