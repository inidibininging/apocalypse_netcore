FROM mcr.microsoft.com/dotnet/core/runtime:2.1-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["Apocalypse.Any.GameServer/Apocalypse.Any.GameServer.csproj", "Apocalypse.Any.GameServer/"]
COPY ["Apocalypse.Any.Infrastructure.Server.Services.Network/Apocalypse.Any.Infrastructure.Server.Services.Network.csproj", "Apocalypse.Any.Infrastructure.Server.Services.Network/"]
COPY ["Apocalypse.Any.Domain.Common.Network/Apocalypse.Any.Domain.Common.Network.csproj", "Apocalypse.Any.Domain.Common.Network/"]
COPY ["Apocalypse.Any.Domain.Common.Model.Network/Apocalypse.Any.Domain.Common.Model.Network.csproj", "Apocalypse.Any.Domain.Common.Model.Network/"]
COPY ["Apocalypse.Any.Core/Apocalypse.Any.Core.csproj", "Apocalypse.Any.Core/"]
COPY ["Apocalypse.Any.Collections/Apocalypse.Any.Core.Collections.csproj", "Apocalypse.Any.Collections/"]
COPY ["Apocalypse.Any.Domain.Common.Model/Apocalypse.Any.Domain.Common.Model.csproj", "Apocalypse.Any.Domain.Common.Model/"]
COPY ["Apocalypse.Any.Domain.Server.Model.Network/Apocalypse.Any.Domain.Server.Model.Network.csproj", "Apocalypse.Any.Domain.Server.Model.Network/"]
COPY ["Apocalypse.Any.Infrastructure.Common.Services.Network/Apocalypse.Any.Infrastructure.Common.Services.Network.csproj", "Apocalypse.Any.Infrastructure.Common.Services.Network/"]
COPY ["Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces/Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.csproj", "Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces/"]
COPY ["Apocalypse.Any.Infrastructure.Server.Services.Data/Apocalypse.Any.Infrastructure.Server.Services.Data.csproj", "Apocalypse.Any.Infrastructure.Server.Services.Data/"]
COPY ["Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces/Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces.csproj", "Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces/"]
COPY ["States/States.Core/States.Core.csproj", "States/States.Core/"]
COPY ["Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces/Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces.csproj", "Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces/"]
COPY ["Apocalypse.Any.Domain.Server.Model/Apocalypse.Any.Domain.Server.Model.csproj", "Apocalypse.Any.Domain.Server.Model/"]
COPY ["Apocalypse.Any.Domain.Common.Mechanics/Apocalypse.Any.Domain.Common.Mechanics.csproj", "Apocalypse.Any.Domain.Common.Mechanics/"]
COPY ["Apocalypse.Any.Domain.Server.Model.Interfaces/Apocalypse.Any.Domain.Server.Model.Interfaces.csproj", "Apocalypse.Any.Domain.Server.Model.Interfaces/"]
COPY ["Apocalypse.Any.Infrastructure.Server.Services.Mechanics/Apocalypse.Any.Infrastructure.Server.Services.Mechanics.csproj", "Apocalypse.Any.Infrastructure.Server.Services.Mechanics/"]
COPY ["Apocalypse.Any.Infrastructure.Server.Services.Factories/Apocalypse.Any.Infrastructure.Server.Services.Factories.csproj", "Apocalypse.Any.Infrastructure.Server.Services.Factories/"]
COPY ["Apocalypse.Any.Domain.Server.Sector.Model/Apocalypse.Any.Domain.Server.Sector.Model.csproj", "Apocalypse.Any.Domain.Server.Sector.Model/"]
COPY ["Apocalypse.Any.Domain.Common.DrawingOrder/Apocalypse.Any.Domain.Common.DrawingOrder.csproj", "Apocalypse.Any.Domain.Common.DrawingOrder/"]
COPY ["Apocalypse.Any.Infrastructure.Server.Sector.Interfaces/Apocalypse.Any.Infrastructure.Server.Sector.Interfaces.csproj", "Apocalypse.Any.Infrastructure.Server.Sector.Interfaces/"]
COPY ["Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Sector.Interfaces/Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Sector.Interfaces.csproj", "Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Sector.Interfaces/"]
COPY ["Apocalypse.Any.Infrastructure.Server.Language/Apocalypse.Any.Infrastructure.Server.Language.csproj", "Apocalypse.Any.Infrastructure.Server.Language/"]
COPY ["Apocalypse.Any.Infrastructure.Server.Services.Transformations/Apocalypse.Any.Infrastructure.Server.Services.Transformations.csproj", "Apocalypse.Any.Infrastructure.Server.Services.Transformations/"]
COPY ["Apocalypse.Any.Infrastructure.Common.Services/Apocalypse.Any.Infrastructure.Common.Services.csproj", "Apocalypse.Any.Infrastructure.Common.Services/"]
COPY ["Apocalypse.Any.Infrastructure.Server.States.Interfaces/Apocalypse.Any.Infrastructure.Server.States.Interfaces.csproj", "Apocalypse.Any.Infrastructure.Server.States.Interfaces/"]
COPY ["Apocalypse.Any.Infrastructure.Server.States/Apocalypse.Any.Infrastructure.Server.States.csproj", "Apocalypse.Any.Infrastructure.Server.States/"]
COPY ["Apocalypse.Any.Infrastructure.Server.Adapters.Redis/Apocalypse.Any.Infrastructure.Server.Adapters.Redis.csproj", "Apocalypse.Any.Infrastructure.Server.Adapters.Redis/"]
RUN dotnet restore "Apocalypse.Any.GameServer/Apocalypse.Any.GameServer.csproj"
COPY . .
WORKDIR "/src/Apocalypse.Any.GameServer"
RUN dotnet build "Apocalypse.Any.GameServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Apocalypse.Any.GameServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Apocalypse.Any.GameServer.dll"]