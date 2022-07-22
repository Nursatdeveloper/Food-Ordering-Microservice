# Food Ordering Application with Microservice Architecture

# Services:
`Registration Service` is responsible for all data registrations (e.g. new Restaurants, new Foods, new Users etc.), sends data to RabbitMQ Message Bus. <br/>
`Catalog Service` is responsible to show all foods, food categories, restaurants available in DB and accept orders. Orders will be sent to RabbitMQ Message Bus. Customer mobile and web clients will call this api endpoint. <br/>
`Order Service` is responsible to handle new orders and send them to client applications of delivery and restaurants in real time with SignalR. <br/>
`Identity Service` is responsible for authentication and authorization. Customers, Restaurants must be authenticated before making any api calls. <br/>
`Image Grpc Service` is responsible to process and store images of restaurants, foods etc. Grpc is used because it allows fast request & response exchange between microservices. <br/>
`Api Gateway` by Ocelot accepts almost all requests from client apps and redirects them to appropriate services. Acts as gateway, reverse proxy, and load balancer. <br/>

# Stack of technologies used:
## Main: .Net WebApi, .Net Grpc
## DB: Postgres, MongoDB
## Data Exchange: RabbitMQ, SignalR (for real-time communication)

