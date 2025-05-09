var builder = DistributedApplication.CreateBuilder(args);

var catalogAPI = builder.AddProject<Projects.FN_CatalogService>("apiservice-catalog");
var orderAPI = builder.AddProject<Projects.FN_OrderService>("apiservice-order");
var userAPI = builder.AddProject<Projects.FN_UserService>("apiservice-user");
var emailAPI = builder.AddProject<Projects.FN_EmailService>("apiservice-email");
var gateway = builder.AddProject<Projects.FN_APIGateway>("gateway");

builder.Build().Run();
