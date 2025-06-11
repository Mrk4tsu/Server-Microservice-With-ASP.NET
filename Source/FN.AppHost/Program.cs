var builder = DistributedApplication.CreateBuilder(args);

var catalogAPI = builder.AddProject<Projects.FN_CatalogService>("apiservice-catalog");
var orderAPI = builder.AddProject<Projects.FN_OrderService>("apiservice-order");
var userAPI = builder.AddProject<Projects.FN_UserService>("apiservice-user");
var emailAPI = builder.AddProject<Projects.FN_EmailService>("apiservice-email");
var aiAPI = builder.AddProject<Projects.FN_AIService>("apiservice-ai");
var gateway = builder.AddProject<Projects.FN_APIGateway>("gateway");
//var forum = builder.AddProject<Projects.FN_Forum>("apiforum");

builder.Build().Run();
