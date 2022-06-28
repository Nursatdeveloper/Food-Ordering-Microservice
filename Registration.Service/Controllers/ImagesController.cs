using Google.Protobuf;
using Grpc.Net.Client;
using Image.Grpc.Service;
using Microsoft.AspNetCore.Mvc;

namespace Registration.Service.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private string grpcServiceProductionAddress =  Environment.GetEnvironmentVariable("GrpcServiceAddress")!;
        private string grpcServiceDevelopmentAddress = "https://localhost:5061";
        private readonly IWebHostEnvironment _env;

        public ImagesController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        [Route("food")]
        public async Task<ActionResult> PostFoodImage([FromForm] CreateFoodImageDto createFoodImageDto)
        {
            var request = new PostFoodImageRequest
            {
                Restaurant = createFoodImageDto.Restaurant,
                Food = createFoodImageDto.Food
            };

            using (var stream = new MemoryStream())
            {
                await createFoodImageDto.FoodImage.CopyToAsync(stream);
                request.Image = ByteString.CopyFrom(stream.ToArray());
            }
            string address;
            if(_env.IsDevelopment()) { address = grpcServiceDevelopmentAddress; }
            else { address = grpcServiceProductionAddress; }

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            using var channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions { HttpHandler = httpHandler });
            var grpcClient = new Images.ImagesClient(channel);
            var message = await grpcClient.PostFoodImageAsync(request);
            return Ok(message);
        }

        [HttpPost]
        [Route("restaurant")]
        public async Task<ActionResult> PostRestaurantImage([FromForm] CreateRestaurantImageDto createRestaurantImageDto)
        {
            var request = new PostRestaurantImageRequest { Restaurant = createRestaurantImageDto.Restaurant };

            using var stream = new MemoryStream();
            await createRestaurantImageDto.RestaurantImage.CopyToAsync(stream);
            request.Image = ByteString.CopyFrom(stream.ToArray());

            string address;
            if (_env.IsDevelopment()) { address = grpcServiceDevelopmentAddress; }
            else { address = grpcServiceProductionAddress; }

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            using var channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions { HttpHandler = httpHandler });
            var grpcClient = new Images.ImagesClient(channel);
            var message = await grpcClient.PostRestaurantImageAsync(request);
            return Ok(message);
        }

        [HttpPost]
        [Route("food-category")]
        public async Task<ActionResult> PostFoodCategoryImage([FromForm] CreateFoodCategoryImageDto createFoodCategoryImageDto)
        {
            var request = new PostFoodCategoryImageRequest
            {
                Restaurant = createFoodCategoryImageDto.Restaurant,
                Category = createFoodCategoryImageDto.Category
            };

            using var stream = new MemoryStream();
            await createFoodCategoryImageDto.CategoryImage.CopyToAsync(stream);
            request.Image = ByteString.CopyFrom(stream.ToArray());

            string address;
            if (_env.IsDevelopment()) { address = grpcServiceDevelopmentAddress; }
            else { address = grpcServiceProductionAddress; }

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            using var channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions { HttpHandler = httpHandler });
            var grpcClient = new Images.ImagesClient(channel);
            var message = await grpcClient.PostFoodCategoryImageAsync(request);
            return Ok(message);
        }

    }
}
