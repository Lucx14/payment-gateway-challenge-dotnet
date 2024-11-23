# Payment Gateway Test solution

Initial thoughts on structure:
* Do not over-engineer the solution
* Use both unit testing and integration testing to demonstrate understanding of both, use Test containers?
* split the project into a nice domain driven design using an application layer an api layer and infrastructure layer maybe split a host layer out too?
* No need for a db according to the notes, still can use a nice repository pattern tho with some DI
* Error handling - Result pattern over exceptions with a global exception handler to cover unexpected errors?
* Should i bump to dotnet 9?
* Containerize my solution?
* Should I enforce code analysis in the build for good code quality?
* Have a think about what is the core domain model in this project

Summary of Requirements
* The payment Gateway is an api that a merchant will use to create payments
* Store card information
* need to make (Api requests I think) to the acquiring bank (mocked with docker)
* Fully test the payment flow
* A merchant should be able to process a payment through the payment gateway and receive one of the following types of response:
  * Authorized - the payment was authorized by the call to the acquiring bank
  * Declined - the payment was declined by the call to the acquiring bank
  * Rejected - No payment could be created as invalid information was supplied to the payment gateway, and therefore it has rejected the request without calling the acquiring bank
* A merchant should be able to retrieve the details of a previously made payment
* Validation of request from merchant - should I use library Fluent Validation maybe?
* Requirements make clear I can use a test in memory db instead of a real one
* Good documentation
* Code must compile
* Automated tests - as in does it want me to have a CI run them? and display the result in my readme?
* API Design and architecture should focus on meeting the functional requirements

Notes on using the simulator
* docker compose up
* make a post request to http://localhost:8080/payments
* with body
```json
{
  "card_number": "2222405343248877",
  "expiry_date": "04/2025",
  "currency": "GBP",
  "amount": 100,
  "cvv": "123"
}
```
* response we get is (200)
```json
{
    "authorized": true,
    "authorization_code": "0bb07405-6d44-4b50-a14f-7ae0beff13ad"
}
```
or this body to get an example of a payment which is not authorised
```json
{
  "card_number": "2222405343248112",
  "expiry_date": "01/2026",
  "currency": "USD",
  "amount": 60000,
  "cvv": "456"
}
```
* response in this case (200)
```json
{
    "authorized": false,
    "authorization_code": ""
}
```
* any other details provided in the body that dont match the supplied examples just get a 400
```json
{
    "errorMessage": "The request supplied is not supported by the simulator"
}
```
* And what if the Mock bank is down do we get a 500 server error?