# Payment Gateway Test solution

## Initial thoughts on structure:
* Do not over-engineer the solution
* Use both unit testing and integration testing to demonstrate understanding of both, use Test containers?
* split the project into a nice domain driven design using an application layer an api layer and infrastructure layer maybe split a host layer out too?
* No need for a db according to the notes, still can use a nice repository pattern tho with some DI
* Error handling - Result pattern over exceptions with a global exception handler to cover unexpected errors?
* Should i bump to dotnet 9?
* Containerize my solution?
* Should I enforce code analysis in the build for good code quality?
* Have a think about what is the core domain model in this project

## Summary of Requirements
* A merchant can process a payment using card A and get an Authorized payment ✅
* A merchant can process a payment using card B and get a Declined payment ✅
* A merchant can process a payment with invalid Info and get a Rejected Payment ✅
* A merchant can retrieve the details of a previously made payment ✅
* Payment gateway performs validation on the merchants request ✅
* Persistence: Use in memory repository ✅
* Provide Documentation in a Readme ✅
* Code can compile ✅
* Code is covered by tests ✅
* Code is simple and maintainable ✅

## Key design considerations and assumption
* Introduce some **centralised package & build management** for simpler maintenance & bump to latest version of dotnet (Intention is to turn on all code quality tags, however as im under time pressure ill leave these off for now)
* I want to follow some good architectural practices with a focus on **DDD**. I wanted to have a core central domain that has no dependencies. An API later, an application layer for services, an infrastructure layer for external apis and repositories
* **Error handling** - I considered using a Result pattern for understood errors however in the interests of time and simplicity I chose to use exception handling through my project layers. I have given each layer an exception type with internal error types these are used to handle expected errors, any unexpected exceptions will be caught by the apps global error handling to be returned as a problem response
* My **Core Domain entity** is the Payment (I was not sure if the Authorization code should be stored in this entity, for now I chose to leave it out as im not totally sure what we would use it for, also I assume it is sensitive data and so would need to be encrypted anyway)
* For the **PaymentStatus** I wanted to represent state before the request had been made to the Acquiring bank and also a failure status that represents problems with the external service so it might be down or something so for this implementation the payment is persisted but in a failed status
* For the **Persistence** to make unit testing easy and so that the project can easily switch in a real db I extracted an interface for the Repository, the interface exists at the domain level with the concrete implementation at the infrastructure level.
* The InMemory Db is registered as a singleton (because it is in memory) if we proceed to switch into a real db this will be scoped.
* The **Application layer** holds the Service which the api will call and this service co-ordinates the creation of the payment with call to external service and persistence via interfaces injected to the constructor for easy unit testing and mocking
* Each layer is separated with its own request and response models and DTOs
* I took a decision to represent the **Card details** as strings and not ints as they are not to be used for math operations so it feels more appropriate to represent these fields as strings
* To perform **Request validation** at the controller level I decided to use **Fluent validation** as it is a nice library and easy to use making the validation in the controller look quite clean and simple, if validation fails I return a 400 Bad request with a list of the validation errors
* **Key design decision**: save a version of the payment in the Db in status Initiated before we call the Acquiring bank, if that call fails then update the payment status to Failed, This means we can better communicate what happened to the payment to the merchant.

## Instructions to run the project
* Run docker compose up to start the bank simulator
* Build & run the Api project
* Use Swagger or postman to send in requests - example
```
http://localhost:5067/api/Payments
{
"cardNumber": "2222405343248877",
"expiryMonth": "04",
"expiryYear": "2025",
"currency": "GBP",
"amount": 100,
"cvv": "123"
}
```

## Future considerations
* can I improve the global error handling - for this I followed the microsoft docs, I think there is a better way to do it
* what do I do with the authorization code?
* introduce a real db
* add to unit and integration tests
* Turn on all code quality enforcement and fix any issues
* Run tests in CI
* Maybe use test containers library for the integration testing