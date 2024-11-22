namespace PaymentGateway.Api.Tests;

// These will form the basis of Integration testing 
// Simple implementation of the web application factory already in use
public class PaymentsControllerTests
{
    // private readonly Random _random = new();

    [Fact]
    public void RetrievesAPaymentSuccessfully()
    {
        // Arrange
        // var payment = new PostPaymentResponse
        // {
        //     Id = Guid.NewGuid(),
        //     ExpiryYear = _random.Next(2023, 2030),
        //     ExpiryMonth = _random.Next(1, 12),
        //     Amount = _random.Next(1, 10000),
        //     CardNumberLastFour = _random.Next(1111, 9999),
        //     Currency = "GBP"
        // };
        //
        // var paymentsRepository = new PaymentsRepository();
        // paymentsRepository.Add(payment);
        //
        // var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        // var client = webApplicationFactory.WithWebHostBuilder(builder =>
        //     builder.ConfigureServices(services => ((ServiceCollection)services)
        //         .AddSingleton(paymentsRepository)))
        //     .CreateClient();
        //
        // // Act
        // var response = await client.GetAsync($"/api/Payments/{payment.Id}");
        // var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        //
        // // Assert
        // Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        // Assert.NotNull(paymentResponse);
    }

    [Fact]
    public void Returns404IfPaymentNotFound()
    {
        // Arrange
        // var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        // var client = webApplicationFactory.CreateClient();
        //
        // // Act
        // var response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");
        //
        // // Assert
        // Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}