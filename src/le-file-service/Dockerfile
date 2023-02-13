#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["LE.FileService/NuGet.config", "LE.FileService/"]
COPY ["LE.FileService/LE.FileService.csproj", "LE.FileService/"]
COPY ["LE.FileService.S3/LE.FileService.S3.csproj", "LE.FileService.S3/"]
COPY ["LE.FileService.MinIO/LE.FileService.MinIO.csproj", "LE.FileService.MinIO/"]
COPY ["LE.FileService.Shared/LE.FileService.Shared.csproj", "LE.FileService.Shared/"]
COPY ["LE.Library/LE.Library.csproj", "LE.Library/"]
RUN dotnet restore "LE.FileService/LE.FileService.csproj" --configfile ./LE.FileService/NuGet.config
COPY ./ ./
WORKDIR "/src/LE.FileService"
RUN dotnet build "LE.FileService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LE.FileService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ARG ASPNETCORE_ENVIRONMENT              
ARG CONSUL_URL
ARG MINIO_HOST

ARG S3_BUCKET_NAME
ARG AWS_ACCESS_KEY_ID
ARG AWS_SECRET_ACCESS_KEY

ENV ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT
ENV CONSUL_URL=$CONSUL_URL
ENV MINIO_HOST=$MINIO_HOST

ENV S3_BUCKET_NAME=$S3_BUCKET_NAME
ENV AWS_ACCESS_KEY_ID=$AWS_ACCESS_KEY_ID
ENV AWS_SECRET_ACCESS_KEY=$AWS_SECRET_ACCESS_KEY
ENTRYPOINT ["dotnet", "LE.FileService.dll"]

# docker build 
#             --build-arg ASPNETCORE_ENVIRONMENT=Develop
#             --build-arg MINIO_HOST
#             --build-arg CONSUL_URL
#             --build-arg S3_BUCKET_NAME
#             --build-arg AWS_ACCESS_KEY_ID
#             --build-arg AWS_SECRET_ACCESS_KEY
#             -t uploader .