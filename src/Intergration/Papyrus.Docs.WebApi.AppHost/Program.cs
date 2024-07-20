var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Papyrus_Docs_GatewayApi>("papyrus-docs-gatewayapi");

builder.AddProject<Projects.Papyrus_Docs_AuthApi>("papyrus-docs-authapi");

builder.AddProject<Projects.Papyrus_Docs_DocumentApi>("papyrus-docs-documentapi");

builder.Build().Run();
