# microservice-aspnetcore-servicediscvoery
Service discovery patterns, in asp-net-core 

In this project i use asp-net-core-api to show how to use service discovery pattern and service heath checking.
I use https://www.consul.io as Service Regustry, and at the start up, I register api as a service from Consul,
then in the client i can get all registered servced from Consul, check theirs status, and using some load balancing algorithms, to choice on the services.
Note that for simplcity i don't use any load balancing algorithm in this sample.

You can follow my repository, to see more samples on Microservices
