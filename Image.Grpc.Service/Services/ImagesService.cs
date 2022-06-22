using AutoMapper;
using Google.Protobuf;
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
        private readonly IRepository<RestaurantImage> _restaurantImageRepository;
        private readonly IRepository<FoodCategoryImage> _foodCategoryImageRepository;

        public ImagesService(ILogger<ImagesService> logger, 
            IMapper mapper, 
            IRepository<FoodImage> foodImageRepository,
            IRepository<RestaurantImage> restaurantImageRepository,
            IRepository<FoodCategoryImage> foodCategoryImageRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _foodImageRepository = foodImageRepository;
            _restaurantImageRepository = restaurantImageRepository;
            _foodCategoryImageRepository = foodCategoryImageRepository;
        }

        public override async Task<PostFoodImageReply> PostFoodImage(PostFoodImageRequest request, 
            ServerCallContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var foodImage = _mapper.Map<FoodImage>(request);
            var createdImage = await _foodImageRepository.CreateAsync(foodImage);
            if(createdImage.Id is null)
            {
                _logger.LogError("--> Post Food Image: Failure");
                return new PostFoodImageReply { Message = "Failure" };
            }
            _logger.LogInformation("--> Post Food Image: Success");
            return new PostFoodImageReply { Message = "Success" };
        }

        public override async Task<GetFoodImageReply> GetFoodImage(GetFoodImageRequest request, 
            ServerCallContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var foodImage = await _foodImageRepository.GetAsync(img =>
                img.Restaurant == request.Restaurant &&
                img.Food == request.Food);
            if (foodImage is null)
            {
                return new GetFoodImageReply { IsSuccess = false };
            }

            var foodImageBytes = foodImage.Image;


            return new GetFoodImageReply 
            { 
                IsSuccess = true, 
                Image = ByteString.CopyFrom(foodImageBytes) 
            };
        }

        public override async Task<PostRestaurantImageReply> PostRestaurantImage(PostRestaurantImageRequest request, 
            ServerCallContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var restaurantImage = _mapper.Map<RestaurantImage>(request);
            var createdImage = await _restaurantImageRepository.CreateAsync(restaurantImage);
            if(createdImage.Id is null)
            {
                _logger.LogError("--> Post Restaurant Image: Failure");
                return new PostRestaurantImageReply { Message = "Failure" };
            }
            _logger.LogInformation("--> Post Restaurant Image: Success");
            return new PostRestaurantImageReply { Message = "Success" };
        }

        public override async Task<GetRestaurantImageReply> GetRestaurantImage(GetRestaurantImageRequest request,
            ServerCallContext context)
        {
            var restaurantImage = await _restaurantImageRepository.GetAsync(img => img.Restaurant == request.Restaurant);
            if(restaurantImage == null)
            {
                return new GetRestaurantImageReply { };
            }
            return new GetRestaurantImageReply { Image = ByteString.CopyFrom(restaurantImage.Image) };
        }

        public override async Task<PostFoodCategoryImageReply> PostFoodCategoryImage(PostFoodCategoryImageRequest request,
            ServerCallContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var foodCategoryImage = _mapper.Map<FoodCategoryImage>(request);
            var createdImage = await _foodCategoryImageRepository.CreateAsync(foodCategoryImage);
            if(createdImage.Id is null)
            {
                _logger.LogError("--> Post Food Category Image: Failure");
                return new PostFoodCategoryImageReply { Message = "Failure" };
            }
            _logger.LogInformation("--> Post Food Category Image: Success");
            return new PostFoodCategoryImageReply { Message = "Success" };
        }

        public override async Task<GetFoodCategoryImageReply> GetFoodCategoryImage(GetFoodCategoryImageRequest request,
            ServerCallContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var foodCategoryImage = await _foodCategoryImageRepository.GetAsync(img => 
                img.Restaurant == request.Restaurant && img.Category == request.Category);
            if(foodCategoryImage is null)
            {
                return new GetFoodCategoryImageReply { };
            }
            return new GetFoodCategoryImageReply { Image = ByteString.CopyFrom(foodCategoryImage.Image) };
        }
    }
}
